//**************************************************************************************************
// BitmapFont.cs                                                                                   *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

[assembly: InternalsVisibleTo("SharpFNT.Tests")]

namespace SharpFNT
{
    public sealed class BitmapFont
    {
        public const int ImplementedVersion = 3;

        internal const byte MagicOne = 66;
        internal const byte MagicTwo = 77;
        internal const byte MagicThree = 70;

        public BitmapFontInfo Info { get; set; }

        public BitmapFontCommon Common { get; set; }

        public IDictionary<int, string> Pages { get; set; }

        public IDictionary<int, Character> Characters { get; set; }

        public IDictionary<KerningPair, int> KerningPairs { get; set; }

        public void Save(string path, FormatHint formatHint)
        {
            using (var fileStream = File.Create(path))
            {
                switch (formatHint)
                {
                    case FormatHint.Binary:
                    {
                        using (var binaryWriter = new BinaryWriter(fileStream))
                        {
                            WriteBinary(binaryWriter);
                        }

                        break;
                    }

                    case FormatHint.XML:
                    {
                        var settings = new XmlWriterSettings
                        {
                            Indent = true,
                            IndentChars = "  "
                        };

                        using (var xmlWriter = XmlWriter.Create(fileStream, settings))
                        {
                            WriteXML(xmlWriter);
                        }

                        break;
                    }

                    case FormatHint.Text:
                    {
                        using (var streamWriter = new StreamWriter(fileStream))
                        {
                            WriteText(streamWriter);
                        }

                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(formatHint), formatHint, null);
                }
            }
        }

        public void WriteBinary(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(MagicOne);
            binaryWriter.Write(MagicTwo);
            binaryWriter.Write(MagicThree);
            binaryWriter.Write((byte)ImplementedVersion);

            if (Info != null)
            {
                binaryWriter.Write((byte)BlockID.Info);
                Info.WriteBinary(binaryWriter);
            }

            if (Common != null)
            {
                binaryWriter.Write((byte)BlockID.Common);
                Common.WriteBinary(binaryWriter, Pages.Count);
            }

            if (Pages != null)
            {
                binaryWriter.Write((byte)BlockID.Pages);
                binaryWriter.Write(Pages.Values.Sum(page => page.Length + 1));

                // Unlike the XML and text formats, the binary format requires page IDs to be consecutive and zero based. 
                
                var index = 0;
                foreach (var keyValuePair in Pages.OrderBy(pair => pair.Key))
                {
                    if (keyValuePair.Key != index) throw new InvalidDataException("The binary format requires that page IDs be consecutive and zero based.");
                    binaryWriter.WriteNullTerminatedString(keyValuePair.Value);
                    index++;
                }
            }

            if (Characters != null)
            {
                binaryWriter.Write((byte)BlockID.Characters);
                binaryWriter.Write(Characters.Values.Count * Character.SizeInBytes);

                foreach (var keyValuePair in Characters)
                {
                    keyValuePair.Value.WriteBinary(binaryWriter, keyValuePair.Key);
                }
            }

            if (KerningPairs != null && KerningPairs.Count > 0)
            {
                binaryWriter.Write((byte)BlockID.KerningPairs);
                binaryWriter.Write(KerningPairs.Keys.Count * KerningPair.SizeInBytes);

                foreach (var keyValuePair in KerningPairs)
                {
                    keyValuePair.Key.WriteBinary(binaryWriter, keyValuePair.Value);
                }
            }
        }
        public void WriteXML(XmlWriter xmlWriter) 
        {
            var document = new XDocument();

            var fontElement = new XElement("font");
            document.Add(fontElement);

            // Info

            if (Info != null)
            {
                var infoElement = new XElement("info");
                Info.WriteXML(infoElement);
                fontElement.Add(infoElement);
            }

            // Common

            if (Common != null)
            {
                var commonElement = new XElement("common");
                Common.WriteXML(commonElement, Pages.Count);
                fontElement.Add(commonElement);
            }

            // Pages

            if (Pages != null)
            {
                var pagesElement = new XElement("pages");

                foreach (var page in Pages)
                {
                    var pageElement = new XElement("page");
                    pageElement.SetAttributeValue("id", page.Key);
                    pageElement.SetAttributeValue("file", page.Value);
                    pagesElement.Add(pageElement);
                }

                fontElement.Add(pagesElement);
            }

            // Characters

            if (Characters != null)
            {
                var charactersElement = new XElement("chars");
                charactersElement.SetAttributeValue("count", Characters.Count);

                foreach (var keyValuePair in Characters)
                {
                    var characterElement = new XElement("char");
                    keyValuePair.Value.WriteXML(characterElement, keyValuePair.Key);
                    charactersElement.Add(characterElement);
                }

                fontElement.Add(charactersElement);
            }

            // Kernings

            if (KerningPairs != null && KerningPairs.Count > 0)
            {
                var kerningsElement = new XElement("kernings");
                kerningsElement.SetAttributeValue("count", KerningPairs.Count);

                foreach (var keyValuePair in KerningPairs)
                {
                    var kerningElement = new XElement("kerning");
                    keyValuePair.Key.WriteXML(kerningElement, keyValuePair.Value);
                    kerningsElement.Add(kerningElement);
                }

                fontElement.Add(kerningsElement);
            }

            document.WriteTo(xmlWriter);
        }
        public void WriteText(TextWriter textWriter) 
        {
            // Info

            if (Info != null)
            {
                textWriter.Write("info");
                Info.WriteText(textWriter);
                textWriter.WriteLine();
            }

            // Common

            if (Common != null)
            {
                textWriter.Write("common");
                Common.WriteText(textWriter, Pages.Count);
                textWriter.WriteLine();
            }

            // Pages

            if (Pages != null)
            {
                foreach (var page in Pages)
                {
                    textWriter.Write("page");
                    TextFormatUtility.WriteInt("id", page.Key, textWriter);
                    TextFormatUtility.WriteString("file", page.Value, textWriter);
                    textWriter.WriteLine();
                }
            }

            // Characters

            if (Characters != null)
            {
                textWriter.Write("chars");
                TextFormatUtility.WriteInt("count", Characters.Count, textWriter);
                textWriter.WriteLine();

                foreach (var keyValuePair in Characters)
                {
                    textWriter.Write("char");
                    keyValuePair.Value.WriteText(textWriter, keyValuePair.Key);
                    textWriter.WriteLine();
                }

            }

            // Kernings

            if (KerningPairs != null && KerningPairs.Count > 0)
            {
                textWriter.Write("kernings");
                TextFormatUtility.WriteInt("count", KerningPairs.Count, textWriter);
                textWriter.WriteLine();

                foreach (var keyValuePair in KerningPairs)
                {
                    textWriter.Write("kerning");
                    keyValuePair.Key.WriteText(textWriter, keyValuePair.Value);
                    textWriter.WriteLine();
                }
            }
        }

        public int GetKerningAmount(char left, char right)
        {
            if (KerningPairs == null)
            {
                return 0;
            }

            KerningPairs.TryGetValue(new KerningPair(left, right), out var kerningValue);

            return kerningValue;
        }
        public Character GetCharacter(char character, bool tryInvalid = true)
        {
            if (Characters == null)
            {
                return null;
            }

            if (Characters.TryGetValue(character, out var result))
            {
                return result;
            }

            if (tryInvalid && Characters.TryGetValue(-1, out result))
            {
                return result;
            }

            return null;
        }

        public static BitmapFont ReadBinary(BinaryReader binaryReader)
        {
            var bitmapFont = new BitmapFont();

            var magicOne = binaryReader.ReadByte();
            var magicTwo = binaryReader.ReadByte();
            var magicThree = binaryReader.ReadByte();

            if (magicOne != MagicOne || magicTwo != MagicTwo || magicThree != MagicThree)
            {
                throw new InvalidDataException("File is not an FNT bitmap font or it is not in the binary format.");
            }

            if (binaryReader.ReadByte() != ImplementedVersion)
            {
                throw new InvalidDataException("The version specified is different from the implemented version.");
            }

            var pageCount = 32;

            while (binaryReader.PeekChar() != -1)
            {
                var blockID = (BlockID)binaryReader.ReadByte();

                switch (blockID)
                {
                    case BlockID.Info:
                    {
                        bitmapFont.Info = BitmapFontInfo.ReadBinary(binaryReader);
                        break;
                    }
                    case BlockID.Common:
                    {
                        bitmapFont.Common = BitmapFontCommon.ReadBinary(binaryReader, out pageCount);
                        break;
                    }
                    case BlockID.Pages:
                    {
                        binaryReader.ReadInt32();

                        bitmapFont.Pages = new Dictionary<int, string>(pageCount);

                        for (var i = 0; i < pageCount; i++)
                        {
                            bitmapFont.Pages[i] = binaryReader.ReadNullTerminatedString();
                        }

                        break;
                    }
                    case BlockID.Characters:
                    {
                        var characterBlockSize = binaryReader.ReadInt32();

                        if (characterBlockSize % Character.SizeInBytes != 0)
                        {
                            throw new InvalidDataException("Invalid character block size.");
                        }

                        var characterCount = characterBlockSize / Character.SizeInBytes;

                        bitmapFont.Characters = new Dictionary<int, Character>(characterCount);

                        for (var i = 0; i < characterCount; i++)
                        {
                            var character = Character.ReadBinary(binaryReader, out var id);
                            bitmapFont.Characters[id] = character;
                        }

                        break;
                    }
                    case BlockID.KerningPairs:
                    {
                        var kerningBlockSize = binaryReader.ReadInt32();

                        if (kerningBlockSize % KerningPair.SizeInBytes != 0)
                        {
                            throw new InvalidDataException("Invalid kerning block size.");
                        }

                        var kerningCount = kerningBlockSize / KerningPair.SizeInBytes;

                        bitmapFont.KerningPairs = new Dictionary<KerningPair, int>(kerningCount);

                        for (var i = 0; i < kerningCount; i++)
                        {
                            var kerningPair = KerningPair.ReadBinary(binaryReader, out var amount);
                            if (bitmapFont.KerningPairs.ContainsKey(kerningPair)) continue;
                            bitmapFont.KerningPairs[kerningPair] = amount;
                        }

                        break;
                    }
                    default:
                    {
                        throw new InvalidDataException("Invalid block ID.");
                    }
                }
            }

            return bitmapFont;
        }
        public static BitmapFont ReadXML(TextReader textReader) 
        {
            var bitmapFont = new BitmapFont();

            var document = XDocument.Load(textReader);

            var fontElement = document.Element("font");

            if (fontElement == null)
            {
                throw new InvalidDataException("Missing root font element in XML file.");
            }

            // Info

            var infoElement = fontElement.Element("info");
            if (infoElement != null)
            {
                bitmapFont.Info = BitmapFontInfo.ReadXML(infoElement);
            }

            // Common

            var pages = 32;

            var commonElement = fontElement.Element("common");
            if (commonElement != null)
            {
                bitmapFont.Common = BitmapFontCommon.ReadXML(commonElement, out pages);
            }

            // Pages

            var pagesElement = fontElement.Element("pages");
            if (pagesElement != null)
            {
                bitmapFont.Pages = new Dictionary<int, string>(pages);

                foreach (var pageElement in pagesElement.Elements("page"))
                {
                    var id = (int)pageElement.Attribute("id");
                    var name = (string)pageElement.Attribute("file");
                    bitmapFont.Pages[id] = name;
                }
            }

            // Characters

            var charactersElement = fontElement.Element("chars");
            if (charactersElement != null)
            {
                var count = (int)charactersElement.Attribute("count");

                bitmapFont.Characters = new Dictionary<int, Character>(count);

                foreach (var characterElement in charactersElement.Elements("char"))
                {
                    var character = Character.ReadXML(characterElement, out var id);
                    bitmapFont.Characters[id] = character;
                }
            }

            // Kernings
             
            var kerningsElement = fontElement.Element("kernings");
            if (kerningsElement != null)
            {
                var count = (int)kerningsElement.Attribute("count");

                bitmapFont.KerningPairs = new Dictionary<KerningPair, int>(count);

                foreach (var kerningElement in kerningsElement.Elements("kerning"))
                {
                    var kerningPair = KerningPair.ReadXML(kerningElement, out var amount);
                    if (bitmapFont.KerningPairs.ContainsKey(kerningPair)) continue;
                    bitmapFont.KerningPairs[kerningPair] = amount;
                }
            }

            return bitmapFont;
        }
        public static BitmapFont ReadText(TextReader textReader) 
        {
            var bitmapFont = new BitmapFont();

            while (textReader.Peek() != -1)
            {
                var lineSegments = TextFormatUtility.GetSegments(textReader.ReadLine());

                switch (lineSegments[0])
                {
                    case "info":
                    {
                        bitmapFont.Info = BitmapFontInfo.ReadText(lineSegments);
                        break;
                    }
                    case "common":
                    {
                        bitmapFont.Common = BitmapFontCommon.ReadText(lineSegments, out var pageCount);
                        bitmapFont.Pages = new Dictionary<int, string>(pageCount);
                        break;
                    }
                    case "page":
                    {
                        bitmapFont.Pages = bitmapFont.Pages ?? new Dictionary<int, string>(32);
                        var id = TextFormatUtility.ReadInt("id", lineSegments);
                        var file = TextFormatUtility.ReadString("file", lineSegments);
                        bitmapFont.Pages[id] = file;
                        break;
                    }
                    case "chars":
                    {
                        var count = TextFormatUtility.ReadInt("count", lineSegments);

                        bitmapFont.Characters = new Dictionary<int, Character>(count);

                        for (var i = 0; i < count; i++)
                        {
                            var characterLineSegments = TextFormatUtility.GetSegments(textReader.ReadLine());
                            var character = Character.ReadText(characterLineSegments, out var id);
                            bitmapFont.Characters[id] = character;
                        }

                        break;
                    }
                    case "kernings":
                    {
                        var count = TextFormatUtility.ReadInt("count", lineSegments);

                        bitmapFont.KerningPairs = new Dictionary<KerningPair, int>(count);

                        for (var i = 0; i < count; i++)
                        {
                            var kerningLineSegments = TextFormatUtility.GetSegments(textReader.ReadLine());
                            var kerningPair = KerningPair.ReadText(kerningLineSegments, out var amount);
                            if (bitmapFont.KerningPairs.ContainsKey(kerningPair)) continue;
                            bitmapFont.KerningPairs[kerningPair] = amount;
                        }

                        break;
                    }
                }
            }

            return bitmapFont;
        }

        public static BitmapFont FromStream(Stream stream, FormatHint formatHint, bool leaveOpen)
        {
            switch (formatHint)
            {
                case FormatHint.Binary:
                    using (var binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen))
                        return ReadBinary(binaryReader);
                case FormatHint.XML:
                    using (var streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen))
                        return ReadXML(streamReader);
                case FormatHint.Text:
                    using (var streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen))
                        return ReadText(streamReader);
                default:
                    throw new ArgumentOutOfRangeException(nameof(formatHint), formatHint, null);
            }
        }
        public static BitmapFont FromStream(Stream stream, bool leaveOpen)
        {
            using (var binaryReader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (binaryReader.PeekChar() == MagicOne)
                {
                    var bitmapFont = ReadBinary(binaryReader);
                    if (!leaveOpen)
                    {
                        stream.Dispose();
                    }
                    return bitmapFont;
                }
            }

            using (var streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen))
            {
                return streamReader.Peek() == '<' ? ReadXML(streamReader) : ReadText(streamReader);
            }
        }

        public static BitmapFont FromFile(string path, FormatHint formatHint)
        {
            using (var fileStream = File.OpenRead(path))
            {
                return FromStream(fileStream, formatHint, true);
            }
        }
        public static BitmapFont FromFile(string path)
        {
            using (var fileStream = File.OpenRead(path))
            {
                return FromStream(fileStream, true);
            }
        }
    }
}