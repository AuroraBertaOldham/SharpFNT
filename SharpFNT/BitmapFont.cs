// ****************************************************************************
// BitmapFont.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SharpFNT
{
    public sealed class BitmapFont
    {
        private const int Version = 3;

        private const byte MagicOne = 66;
        private const byte MagicTwo = 77;
        private const byte MagicThree = 70;

        public BitmapFontInfo Info { get; set; }

        public BitmapFontCommon Common { get; set; }

        public PageCollection Pages { get; set; }

        public CharacterCollection Characters { get; set; }

        public KerningCollection KerningCollection { get; set; }

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
            binaryWriter.Write((byte)Version);

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
                this.Pages.WriteBinary(binaryWriter);
            }

            if (this.Characters != null)
            {
                binaryWriter.Write((byte)BlockID.Characters);
                this.Characters.WriteBinary(binaryWriter);
            }

            if (this.KerningCollection != null && this.KerningCollection.Count > 0)
            {
                binaryWriter.Write((byte)BlockID.KerningPairs);
                this.KerningCollection.WriteBinary(binaryWriter);
            }
        }

        public void WriteXML(XmlWriter xmlWriter) 
        {
            XDocument document = new XDocument();

            XElement infoElement = new XElement("info");
            this.Info.WriteXML(infoElement);
            document.Add(infoElement);

            XElement commonElement = new XElement("common");
            this.Common.WriteXML(commonElement, this.Pages.Count);
            document.Add(commonElement);

            XElement pagesElement = new XElement("pages");
            this.Pages.WriteXML(pagesElement);
            document.Add(pagesElement);

            XElement charactersElement = new XElement("chars");
            this.Characters.WriteXML(charactersElement);
            document.Add(charactersElement);

            if (this.KerningCollection != null && this.KerningCollection.Count > 0)
            {
                XElement kerningsElement = new XElement("kernings");
                this.KerningCollection.WriteXML(kerningsElement);
                document.Add(kerningsElement);
            }

            document.WriteTo(xmlWriter);
        }

        public void WriteText(TextWriter textWriter) 
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("info");
            this.Info.WriteText(stringBuilder);

            stringBuilder.AppendLine("common");
            this.Common.WriteText(stringBuilder, this.Pages.Count);
            this.Pages.WriteText(stringBuilder);

            stringBuilder.AppendLine("chars");
            this.Characters.WriteText(stringBuilder);

            stringBuilder.AppendLine("kernings");
            this.KerningCollection.WriteText(stringBuilder);

            textWriter.Write(stringBuilder.ToString());
        }

        public static BitmapFont ReadBinary(BinaryReader binaryReader)
        {
            BitmapFont bitmapFont = new BitmapFont();

            byte magicOne = binaryReader.ReadByte();
            byte magicTwo = binaryReader.ReadByte();
            byte magicThree = binaryReader.ReadByte();

            if (magicOne != MagicOne || magicTwo != MagicTwo || magicThree != MagicThree)
            {
                throw new InvalidDataException("File is not an FNT font or it is not in the binary format.");
            }

            if (binaryReader.ReadByte() != Version)
            {
                throw new InvalidDataException("The version specified is different from the implemented version.");
            }

            int pageCount = -1;

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                BlockID blockId = (BlockID)binaryReader.ReadByte();

                switch (blockId)
                {
                    case BlockID.Info:
                        bitmapFont.Info = BitmapFontInfo.ReadBinary(binaryReader);
                        break;
                    case BlockID.Common:
                        bitmapFont.Common = BitmapFontCommon.ReadBinary(binaryReader, out pageCount);
                        break;
                    case BlockID.Pages:
                        bitmapFont.Pages = PageCollection.ReadBinary(binaryReader, pageCount);
                        break;
                    case BlockID.Characters:
                        bitmapFont.Characters = CharacterCollection.ReadBinary(binaryReader);
                        break;
                    case BlockID.KerningPairs:
                        bitmapFont.KerningCollection = KerningCollection.ReadBinary(binaryReader);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return bitmapFont;
        }

        public static BitmapFont ReadXML(TextReader textReader) 
        {
            BitmapFont bitmapFont = new BitmapFont();

            XDocument document = XDocument.Load(textReader);

            XElement infoElement = document.Element("info");
            bitmapFont.Info = BitmapFontInfo.ReadXML(infoElement);

            XElement commonElement = document.Element("common");
            bitmapFont.Common = BitmapFontCommon.ReadXML(commonElement, out int pages);

            XElement pagesElement = document.Element("pages");
            bitmapFont.Pages = PageCollection.ReadXML(pagesElement, pages);

            XElement charactersElement = document.Element("chars");
            bitmapFont.Characters = CharacterCollection.ReadXML(charactersElement);

            XElement kerningElement = document.Element("kernings");
            if (kerningElement != null)
            {
                bitmapFont.KerningCollection = KerningCollection.ReadXML(kerningElement);
            }

            return bitmapFont;
        }

        public static BitmapFont ReadText(TextReader textReader) 
        {
            BitmapFont bitmapFont = new BitmapFont();

            string[] lineSegments = textReader.ReadLine().Split();

            switch (lineSegments[0])
            {
                case "info":
                    bitmapFont.Info = BitmapFontInfo.ReadText(lineSegments);
                break;
                
                case "common":
                    bitmapFont.Common = BitmapFontCommon.ReadText(lineSegments, out int pageCount);
                    bitmapFont.Pages = PageCollection.ReadText(textReader, pageCount);
                break;

                case "chars":
                    bitmapFont.Characters = CharacterCollection.ReadText(lineSegments, textReader);
                break;

                case "kernings":
                    bitmapFont.KerningCollection = KerningCollection.ReadText(lineSegments, textReader);
                break;                
            }

            return bitmapFont;
        }

        //TODO Auto read format. Check for binary numbers, then for <, then use text.

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
        
        public static BitmapFont FromFile(string path, FormatHint formatHint)
        {
            using (FileStream fileStream = File.Create(path))
            {
                return FromStream(fileStream, formatHint, true);
            }
        }
    }
}