// ****************************************************************************
// BitmapFontInfo.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System.IO;

namespace SharpFNT
{
    public sealed class BitmapFontInfo
    {
        public const int MinSizeInBytes = 15;

        public int Size { get; set; }

        public bool Smooth { get; set; }
        public bool Unicode { get; set; }
        public bool Italic { get; set; }
        public bool Bold { get; set; }
        public bool FixedHeight { get; set; }

        public int Charset { get; set; }
        public int StretchHeight { get; set; }
        public int SuperSamplingLevel { get; set; }

        public int PaddingUp { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingDown { get; set; }
        public int PaddingLeft { get; set; }

        public int SpacingHorizontal { get; set; }
        public int SpacingVertical { get; set; }

        public int Outline { get; set; }
        public string FontName { get; set; }

        public void WriteBinary(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(MinSizeInBytes + this.FontName.Length + 1);
            binaryWriter.Write((short)this.Size);

            byte bitField = 0;

            bitField = bitField.SetBit(0, this.Smooth);
            bitField = bitField.SetBit(1, this.Unicode);
            bitField = bitField.SetBit(2, this.Italic);
            bitField = bitField.SetBit(3, this.Bold);
            bitField = bitField.SetBit(4, this.FixedHeight);

            binaryWriter.Write(bitField);

            binaryWriter.Write((byte)this.Charset);
            binaryWriter.Write((ushort)this.StretchHeight);
            binaryWriter.Write((byte)this.SuperSamplingLevel);

            binaryWriter.Write((byte)this.PaddingUp);
            binaryWriter.Write((byte)this.PaddingRight);
            binaryWriter.Write((byte)this.PaddingDown);
            binaryWriter.Write((byte)this.PaddingLeft);

            binaryWriter.Write((byte)this.SpacingHorizontal);
            binaryWriter.Write((byte)this.SpacingVertical);

            binaryWriter.Write((byte)this.Outline);
            binaryWriter.Write(this.FontName, true);
        }

        public static BitmapFontInfo ReadBinary(BinaryReader binaryReader)
        {
            if (binaryReader.ReadInt32() < MinSizeInBytes)
            {
                throw new InvalidDataException("Invalid info block size.");
            }

            BitmapFontInfo bitmapFontInfo = new BitmapFontInfo();

            bitmapFontInfo.Size = binaryReader.ReadInt16();

            byte bitField = binaryReader.ReadByte();

            bitmapFontInfo.Smooth = bitField.IsBitSet(0);
            bitmapFontInfo.Unicode = bitField.IsBitSet(1);
            bitmapFontInfo.Italic = bitField.IsBitSet(2);
            bitmapFontInfo.Bold = bitField.IsBitSet(3);
            bitmapFontInfo.FixedHeight = bitField.IsBitSet(4);

            bitmapFontInfo.Charset = binaryReader.ReadByte();
            bitmapFontInfo.StretchHeight = binaryReader.ReadUInt16();
            bitmapFontInfo.SuperSamplingLevel = binaryReader.ReadByte();

            bitmapFontInfo.PaddingUp = binaryReader.ReadByte();
            bitmapFontInfo.PaddingRight = binaryReader.ReadByte();
            bitmapFontInfo.PaddingDown = binaryReader.ReadByte();
            bitmapFontInfo.PaddingLeft = binaryReader.ReadByte();

            bitmapFontInfo.SpacingHorizontal = binaryReader.ReadByte();
            bitmapFontInfo.SpacingVertical = binaryReader.ReadByte();

            bitmapFontInfo.Outline = binaryReader.ReadByte();
            bitmapFontInfo.FontName = binaryReader.ReadCString();

            return bitmapFontInfo;
        }
    }
}