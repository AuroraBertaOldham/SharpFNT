// ****************************************************************************
// BitmapFont.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System;
using System.IO;
using System.Text;

namespace SharpFNT
{
    public sealed class BitmapFont
    {
        public const int Version = 3;

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