// ****************************************************************************
// BitmapFont.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SharpFNT
{
    public sealed class BitmapFont
    {
        public const int ImplementedVersion = 3;

        private const byte MagicOne = 66;
        private const byte MagicTwo = 77;
        private const byte MagicThree = 70;

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
                        using (XmlWriter xmlWriter = XmlWriter.Create(fileStream))
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

                int index = 0;
                foreach (KeyValuePair<int, string> page in this.Pages)
                {
                    if (page.Key != index) throw new InvalidDataException("The binary format requires that the page IDs be zero based.");

                    binaryWriter.Write(page.Value, true);

                    index++;
                }
            }

            if (this.Characters != null)
            {
                binaryWriter.Write((byte)BlockID.Characters);
                binaryWriter.Write(this.Characters.Values.Count * Character.SizeInBytes);

                foreach (Character character in this.Characters.Values)
                {
                    character.WriteBinary(binaryWriter);
                }
            }

            if (this.KerningPairs != null && this.KerningPairs.Count > 0)
            {
                binaryWriter.Write((byte)BlockID.KerningPairs);
                binaryWriter.Write(this.KerningPairs.Keys.Count * KerningPair.SizeInBytes);

                foreach (KerningPair kerningPair in this.KerningPairs.Keys)
                {
                    kerningPair.WriteBinary(binaryWriter);
                }
            }
        }
        public void WriteXML(XmlWriter xmlWriter) 
        {
            XDocument document = new XDocument();

            // Info

            if (this.Info != null)
            {
                XElement infoElement = new XElement("info");
                this.Info.WriteXML(infoElement);
                document.Add(infoElement);
            }

            // Common

            if (this.Common != null)
            {
                XElement commonElement = new XElement("common");
                this.Common.WriteXML(commonElement, this.Pages.Count);
                document.Add(commonElement);
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

                document.Add(pagesElement);
            }

            // Characters

            if (this.Characters != null)
            {
                XElement charactersElement = new XElement("chars");
                charactersElement.SetAttributeValue("count", this.Characters.Count);

                foreach (Character character in this.Characters.Values)
                {
                    XElement characterElement = new XElement("char");
                    character.WriteXML(characterElement);
                    charactersElement.Add(characterElement);
                }

                document.Add(charactersElement);
            }

            // Kernings

            if (this.KerningPairs != null && this.KerningPairs.Count > 0)
            {
                XElement kerningsElement = new XElement("kernings");
                kerningsElement.SetAttributeValue("count", this.KerningPairs.Count);

                foreach (KerningPair kerningPair in this.KerningPairs.Keys)
                {
                    XElement kerningElement = new XElement("kerning");
                    kerningPair.WriteXML(kerningElement);
                    kerningsElement.Add(kerningElement);
                }

                document.Add(kerningsElement);
            }

            document.WriteTo(xmlWriter);
        }
        public void WriteText(TextWriter textWriter) 
        {
            StringBuilder stringBuilder = new StringBuilder(4096);

            // Info

            if (this.Info != null)
            {
                stringBuilder.Append("info");
                this.Info.WriteText(stringBuilder);
            }

            // Common

            if (this.Common != null)
            {
                stringBuilder.AppendLine("common");
                this.Common.WriteText(stringBuilder, this.Pages.Count);
            }

            // Pages

            if (this.Pages != null)
            {
                foreach (KeyValuePair<int, string> page in this.Pages)
                {
                    stringBuilder.AppendLine("page");
                    TextFormatUtility.WriteInt("id", page.Key, stringBuilder);
                    TextFormatUtility.WriteString("id", page.Value, stringBuilder);
                }
            }

            // Characters

            if (this.Characters != null)
            {
                stringBuilder.AppendLine("chars");
                TextFormatUtility.WriteInt("count", this.Characters.Count, stringBuilder);

                foreach (Character character in this.Characters.Values)
                {
                    stringBuilder.AppendLine("char");
                    character.WriteText(stringBuilder);
                }

            }

            // Kernings

            if (this.KerningPairs != null && this.KerningPairs.Count > 0)
            {
                stringBuilder.AppendLine("kernings");
                TextFormatUtility.WriteInt("count", this.KerningPairs.Count, stringBuilder);

                foreach (KerningPair kerningPair in this.KerningPairs.Keys)
                {
                    stringBuilder.AppendLine("kerning");
                    kerningPair.WriteText(stringBuilder);
                }

                textWriter.Write(stringBuilder);
            }
        }

        public int GetKerningAmount(char left, char right)
        {
            if (this.KerningPairs == null)
            {
                return 0;
            }

            this.KerningPairs.TryGetValue(new KerningPair(left, right, 0), out int kerningValue);

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

        public static BitmapFont ReadBinary(BinaryReader binaryReader, bool skipMagic = false)
        {
            BitmapFont bitmapFont = new BitmapFont();

            if (!skipMagic)
            {
                byte magicOne = binaryReader.ReadByte();
                byte magicTwo = binaryReader.ReadByte();
                byte magicThree = binaryReader.ReadByte();

                if (magicOne != MagicOne || magicTwo != MagicTwo || magicThree != MagicThree)
                {
                    throw new InvalidDataException("File is not an FNT bitmap font or it is not in the binary format.");
                }
            }

            if (binaryReader.ReadByte() != ImplementedVersion)
            {
                throw new InvalidDataException("The version specified is different from the implemented version.");
            }

            int pageCount = 32;

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                BlockID blockId = (BlockID)binaryReader.ReadByte();

                switch (blockId)
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
                            bitmapFont.Pages.Add(i, binaryReader.ReadCString());
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
                            Character character = Character.ReadBinary(binaryReader);
                            bitmapFont.Characters.Add(character.ID, character);
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
                            KerningPair kerningPair = KerningPair.ReadBinary(binaryReader);
                            bitmapFont.KerningPairs.Add(kerningPair, kerningPair.Amount);
                        }

                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return bitmapFont;
        }
        public static BitmapFont ReadXML(TextReader textReader) 
        {
            BitmapFont bitmapFont = new BitmapFont();

            XDocument document = XDocument.Load(textReader);

            // Info

            XElement infoElement = document.Element("info");
            if (infoElement != null)
            {
                bitmapFont.Info = BitmapFontInfo.ReadXML(infoElement);
            }

            // Common

            int pages = 32;

            XElement commonElement = document.Element("common");
            if (commonElement != null)
            {
                bitmapFont.Common = BitmapFontCommon.ReadXML(commonElement, out pages);
            }

            // Pages

            XElement pagesElement = document.Element("pages");
            if (pagesElement != null)
            {
                bitmapFont.Pages = new Dictionary<int, string>(pages);

                foreach (XElement pageElement in pagesElement.Elements("page"))
                {
                    int id = (int)pageElement.Attribute("id");
                    string name = (string)pageElement.Attribute("file");
                    bitmapFont.Pages.Add(id, name);
                }
            }

            // Characters

            XElement charactersElement = document.Element("chars");
            if (charactersElement != null)
            {
                int count = (int)charactersElement.Attribute("count");

                bitmapFont.Characters = new Dictionary<int, Character>(count);

                foreach (XElement characterElement in charactersElement.Elements("char"))
                {
                    Character character = Character.ReadXML(characterElement);
                    bitmapFont.Characters.Add(character.ID, character);
                }
            }

            // Kernings
             
            XElement kerningsElement = document.Element("kernings");
            if (kerningsElement != null)
            {
                int count = (int)kerningsElement.Attribute("count");

                bitmapFont.KerningPairs = new Dictionary<KerningPair, int>(count);

                foreach (XElement kerningElement in kerningsElement.Elements("kerning"))
                {
                    KerningPair kerningPair = KerningPair.ReadXML(kerningElement);
                    bitmapFont.KerningPairs.Add(kerningPair, kerningPair.Amount);
                }
            }

            return bitmapFont;
        }
        public static BitmapFont ReadText(TextReader textReader) 
        {
            BitmapFont bitmapFont = new BitmapFont();

            while (textReader.Peek() != -1)
            {
                string[] lineSegments = textReader.ReadLine().Split();

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
                        bitmapFont.Pages.Add(id, file);
                        break;
                    }

                    case "chars":
                    {
                        int count = TextFormatUtility.ReadInt("count", lineSegments);

                        bitmapFont.Characters = new Dictionary<int, Character>(count);

                        for (int i = 0; i < count; i++)
                        {
                            string[] characterLineSegments = textReader.ReadLine().Split();
                            Character character = Character.ReadText(characterLineSegments);
                            bitmapFont.Characters.Add(character.ID, character);
                        }

                        break;
                    }

                    case "kernings":
                    {
                        int count = TextFormatUtility.ReadInt("count", lineSegments);

                        bitmapFont.KerningPairs = new Dictionary<KerningPair, int>(count);

                        for (int i = 0; i < count; i++)
                        {
                            string[] kerningLineSegments = textReader.ReadLine().Split();
                            KerningPair kerningPair = KerningPair.ReadText(kerningLineSegments);
                            bitmapFont.KerningPairs.Add(kerningPair, kerningPair.Amount);
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
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        return ReadXML(streamReader);
                    }
                }

                case FormatHint.Text:
                {
                    using (StreamReader streamReader = new StreamReader(stream))
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
                byte magicOne = binaryReader.ReadByte();
                byte magicTwo = binaryReader.ReadByte();
                byte magicThree = binaryReader.ReadByte();

                if (magicOne == MagicOne && magicTwo == MagicTwo && magicThree == MagicThree)
                {
                    BitmapFont bitmapFont = ReadBinary(binaryReader, true);
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
            using (FileStream fileStream = File.Create(path))
            {
                return FromStream(fileStream, formatHint, true);
            }
        }
        public static BitmapFont FromFile(string path)
        {
            using (FileStream fileStream = File.Create(path))
            {
                return FromStream(fileStream, true);
            }
        }
    }
}