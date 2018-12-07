// ****************************************************************************
// BitmapFont.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

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
            using (FileStream fileStream = File.Create(path))
            {
                switch (formatHint)
                {
                    case FormatHint.Binary:
                    {
                        using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                        {
                            this.WriteBinary(binaryWriter);
                        }

                        break;
                    }

                    case FormatHint.XML:
                    {
                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            IndentChars = "  "
                        };

                        using (XmlWriter xmlWriter = XmlWriter.Create(fileStream, settings))
                        {
                            this.WriteXML(xmlWriter);
                        }

                        break;
                    }

                    case FormatHint.Text:
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        {
                            this.WriteText(streamWriter);
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

            if (this.Info != null)
            {
                binaryWriter.Write((byte)BlockID.Info);
                this.Info.WriteBinary(binaryWriter);
            }

            if (this.Common != null)
            {
                binaryWriter.Write((byte)BlockID.Common);
                this.Common.WriteBinary(binaryWriter, this.Pages.Count);
            }

            if (this.Pages != null)
            {
                binaryWriter.Write((byte)BlockID.Pages);
                binaryWriter.Write(this.Pages.Values.Sum(page => page.Length + 1));

                // Unlike the XML and text formats, the binary format requires page IDs to be consecutive and zero based. 
                
                int index = 0;
                foreach (KeyValuePair<int, string> keyValuePair in this.Pages.OrderBy(pair => pair.Key))
                {
                    if (keyValuePair.Key != index) throw new InvalidDataException("The binary format requires that page IDs be consecutive and zero based.");
                    binaryWriter.WriteNullTerminated(keyValuePair.Value);
                    index++;
                }
            }

            if (this.Characters != null)
            {
                binaryWriter.Write((byte)BlockID.Characters);
                binaryWriter.Write(this.Characters.Values.Count * Character.SizeInBytes);

                foreach (KeyValuePair<int, Character> keyValuePair in this.Characters)
                {
                    keyValuePair.Value.WriteBinary(binaryWriter, keyValuePair.Key);
                }
            }

            if (this.KerningPairs != null && this.KerningPairs.Count > 0)
            {
                binaryWriter.Write((byte)BlockID.KerningPairs);
                binaryWriter.Write(this.KerningPairs.Keys.Count * KerningPair.SizeInBytes);

                foreach (KeyValuePair<KerningPair, int> keyValuePair in this.KerningPairs)
                {
                    keyValuePair.Key.WriteBinary(binaryWriter, keyValuePair.Value);
                }
            }
        }
        public void WriteXML(XmlWriter xmlWriter) 
        {
            XDocument document = new XDocument();

            XElement fontElement = new XElement("font");
            document.Add(fontElement);

            // Info

            if (this.Info != null)
            {
                XElement infoElement = new XElement("info");
                this.Info.WriteXML(infoElement);
                fontElement.Add(infoElement);
            }

            // Common

            if (this.Common != null)
            {
                XElement commonElement = new XElement("common");
                this.Common.WriteXML(commonElement, this.Pages.Count);
                fontElement.Add(commonElement);
            }

            // Pages

            if (this.Pages != null)
            {
                XElement pagesElement = new XElement("pages");

                foreach (KeyValuePair<int, string> page in this.Pages)
                {
                    XElement pageElement = new XElement("page");
                    pageElement.SetAttributeValue("id", page.Key);
                    pageElement.SetAttributeValue("file", page.Value);
                    pagesElement.Add(pageElement);
                }

                fontElement.Add(pagesElement);
            }

            // Characters

            if (this.Characters != null)
            {
                XElement charactersElement = new XElement("chars");
                charactersElement.SetAttributeValue("count", this.Characters.Count);

                foreach (KeyValuePair<int, Character> keyValuePair in this.Characters)
                {
                    XElement characterElement = new XElement("char");
                    keyValuePair.Value.WriteXML(characterElement, keyValuePair.Key);
                    charactersElement.Add(characterElement);
                }

                fontElement.Add(charactersElement);
            }

            // Kernings

            if (this.KerningPairs != null && this.KerningPairs.Count > 0)
            {
                XElement kerningsElement = new XElement("kernings");
                kerningsElement.SetAttributeValue("count", this.KerningPairs.Count);

                foreach (KeyValuePair<KerningPair, int> keyValuePair in this.KerningPairs)
                {
                    XElement kerningElement = new XElement("kerning");
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

            if (this.Info != null)
            {
                textWriter.Write("info");
                this.Info.WriteText(textWriter);
                textWriter.WriteLine();
            }

            // Common

            if (this.Common != null)
            {
                textWriter.Write("common");
                this.Common.WriteText(textWriter, this.Pages.Count);
                textWriter.WriteLine();
            }

            // Pages

            if (this.Pages != null)
            {
                foreach (KeyValuePair<int, string> page in this.Pages)
                {
                    textWriter.Write("page");
                    TextFormatUtility.WriteInt("id", page.Key, textWriter);
                    TextFormatUtility.WriteString("file", page.Value, textWriter);
                    textWriter.WriteLine();
                }
            }

            // Characters

            if (this.Characters != null)
            {
                textWriter.Write("chars");
                TextFormatUtility.WriteInt("count", this.Characters.Count, textWriter);
                textWriter.WriteLine();

                foreach (KeyValuePair<int, Character> keyValuePair in this.Characters)
                {
                    textWriter.Write("char");
                    keyValuePair.Value.WriteText(textWriter, keyValuePair.Key);
                    textWriter.WriteLine();
                }

            }

            // Kernings

            if (this.KerningPairs != null && this.KerningPairs.Count > 0)
            {
                textWriter.Write("kernings");
                TextFormatUtility.WriteInt("count", this.KerningPairs.Count, textWriter);
                textWriter.WriteLine();

                foreach (KeyValuePair<KerningPair, int> keyValuePair in this.KerningPairs)
                {
                    textWriter.Write("kerning");
                    keyValuePair.Key.WriteText(textWriter, keyValuePair.Value);
                    textWriter.WriteLine();
                }
            }
        }

        public int GetKerningAmount(char left, char right)
        {
            if (this.KerningPairs == null)
            {
                return 0;
            }

            this.KerningPairs.TryGetValue(new KerningPair(left, right), out int kerningValue);

            return kerningValue;
        }
        public Character GetCharacter(char character, bool tryInvalid = true)
        {
            if (this.Characters == null)
            {
                return null;
            }

            if (this.Characters.TryGetValue(character, out Character result))
            {
                return result;
            }

            if (tryInvalid && this.Characters.TryGetValue(-1, out result))
            {
                return result;
            }

            return null;
        }

        public static BitmapFont ReadBinary(BinaryReader binaryReader)
        {
            BitmapFont bitmapFont = new BitmapFont();

            byte magicOne = binaryReader.ReadByte();
            byte magicTwo = binaryReader.ReadByte();
            byte magicThree = binaryReader.ReadByte();

            if (magicOne != MagicOne || magicTwo != MagicTwo || magicThree != MagicThree)
            {
                throw new InvalidDataException("File is not an FNT bitmap font or it is not in the binary format.");
            }

            if (binaryReader.ReadByte() != ImplementedVersion)
            {
                throw new InvalidDataException("The version specified is different from the implemented version.");
            }

            int pageCount = 32;

            while (binaryReader.PeekChar() != -1)
            {
                BlockID blockID = (BlockID)binaryReader.ReadByte();

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

                        for (int i = 0; i < pageCount; i++)
                        {
                            bitmapFont.Pages[i] = binaryReader.ReadCString();
                        }

                        break;
                    }
                    case BlockID.Characters:
                    {
                        int characterBlockSize = binaryReader.ReadInt32();

                        if (characterBlockSize % Character.SizeInBytes != 0)
                        {
                            throw new InvalidDataException("Invalid character block size.");
                        }

                        int characterCount = characterBlockSize / Character.SizeInBytes;

                        bitmapFont.Characters = new Dictionary<int, Character>(characterCount);

                        for (int i = 0; i < characterCount; i++)
                        {
                            Character character = Character.ReadBinary(binaryReader, out int id);
                            bitmapFont.Characters[id] = character;
                        }

                        break;
                    }
                    case BlockID.KerningPairs:
                    {
                        int kerningBlockSize = binaryReader.ReadInt32();

                        if (kerningBlockSize % KerningPair.SizeInBytes != 0)
                        {
                            throw new InvalidDataException("Invalid kerning block size.");
                        }

                        int kerningCount = kerningBlockSize / KerningPair.SizeInBytes;

                        bitmapFont.KerningPairs = new Dictionary<KerningPair, int>(kerningCount);

                        for (int i = 0; i < kerningCount; i++)
                        {
                            KerningPair kerningPair = KerningPair.ReadBinary(binaryReader, out int amount);
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
            BitmapFont bitmapFont = new BitmapFont();

            XDocument document = XDocument.Load(textReader);

            XElement fontElement = document.Element("font");

            if (fontElement == null)
            {
                throw new InvalidDataException("Missing root font element in XML file.");
            }

            // Info

            XElement infoElement = fontElement.Element("info");
            if (infoElement != null)
            {
                bitmapFont.Info = BitmapFontInfo.ReadXML(infoElement);
            }

            // Common

            int pages = 32;

            XElement commonElement = fontElement.Element("common");
            if (commonElement != null)
            {
                bitmapFont.Common = BitmapFontCommon.ReadXML(commonElement, out pages);
            }

            // Pages

            XElement pagesElement = fontElement.Element("pages");
            if (pagesElement != null)
            {
                bitmapFont.Pages = new Dictionary<int, string>(pages);

                foreach (XElement pageElement in pagesElement.Elements("page"))
                {
                    int id = (int)pageElement.Attribute("id");
                    string name = (string)pageElement.Attribute("file");
                    bitmapFont.Pages[id] = name;
                }
            }

            // Characters

            XElement charactersElement = fontElement.Element("chars");
            if (charactersElement != null)
            {
                int count = (int)charactersElement.Attribute("count");

                bitmapFont.Characters = new Dictionary<int, Character>(count);

                foreach (XElement characterElement in charactersElement.Elements("char"))
                {
                    Character character = Character.ReadXML(characterElement, out int id);
                    bitmapFont.Characters[id] = character;
                }
            }

            // Kernings
             
            XElement kerningsElement = fontElement.Element("kernings");
            if (kerningsElement != null)
            {
                int count = (int)kerningsElement.Attribute("count");

                bitmapFont.KerningPairs = new Dictionary<KerningPair, int>(count);

                foreach (XElement kerningElement in kerningsElement.Elements("kerning"))
                {
                    KerningPair kerningPair = KerningPair.ReadXML(kerningElement, out int amount);
                    if (bitmapFont.KerningPairs.ContainsKey(kerningPair)) continue;
                    bitmapFont.KerningPairs[kerningPair] = amount;
                }
            }

            return bitmapFont;
        }
        public static BitmapFont ReadText(TextReader textReader) 
        {
            BitmapFont bitmapFont = new BitmapFont();

            while (textReader.Peek() != -1)
            {
                IReadOnlyList<string> lineSegments = TextFormatUtility.GetSegments(textReader.ReadLine());

                switch (lineSegments[0])
                {
                    case "info":
                    {
                        bitmapFont.Info = BitmapFontInfo.ReadText(lineSegments);
                        break;
                    }

                    case "common":
                    {
                        bitmapFont.Common = BitmapFontCommon.ReadText(lineSegments, out int pageCount);
                        bitmapFont.Pages = new Dictionary<int, string>(pageCount);
                        break;
                    }

                    case "page":
                    {
                        bitmapFont.Pages = bitmapFont.Pages ?? new Dictionary<int, string>(32);
                        int id = TextFormatUtility.ReadInt("id", lineSegments);
                        string file = TextFormatUtility.ReadString("file", lineSegments);
                        bitmapFont.Pages[id] = file;
                        break;
                    }

                    case "chars":
                    {
                        int count = TextFormatUtility.ReadInt("count", lineSegments);

                        bitmapFont.Characters = new Dictionary<int, Character>(count);

                        for (int i = 0; i < count; i++)
                        {
                            IReadOnlyList<string> characterLineSegments = TextFormatUtility.GetSegments(textReader.ReadLine());
                            Character character = Character.ReadText(characterLineSegments, out int id);
                            bitmapFont.Characters[id] = character;
                        }

                        break;
                    }

                    case "kernings":
                    {
                        int count = TextFormatUtility.ReadInt("count", lineSegments);

                        bitmapFont.KerningPairs = new Dictionary<KerningPair, int>(count);

                        for (int i = 0; i < count; i++)
                        {
                            IReadOnlyList<string> kerningLineSegments = TextFormatUtility.GetSegments(textReader.ReadLine());
                            KerningPair kerningPair = KerningPair.ReadText(kerningLineSegments, out int amount);
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
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen))
                    {
                        return ReadBinary(binaryReader);
                    }
                }

                case FormatHint.XML:
                {
                    using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen))
                    {
                        return ReadXML(streamReader);
                    }
                }

                case FormatHint.Text:
                {
                    using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen))
                    {
                        return ReadText(streamReader);
                    }
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(formatHint), formatHint, null);
            }
        }
        public static BitmapFont FromStream(Stream stream, bool leaveOpen)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (binaryReader.PeekChar() == MagicOne)
                {
                    BitmapFont bitmapFont = ReadBinary(binaryReader);
                    if (!leaveOpen)
                    {
                        stream.Dispose();
                    }
                    return bitmapFont;
                }
            }

            using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen))
            {
                return streamReader.Peek() == '<' ? ReadXML(streamReader) : ReadText(streamReader);
            }
        }

        public static BitmapFont FromFile(string path, FormatHint formatHint)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                return FromStream(fileStream, formatHint, true);
            }
        }
        public static BitmapFont FromFile(string path)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                return FromStream(fileStream, true);
            }
        }
    }
}