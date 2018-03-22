// ****************************************************************************
// BitmapFontCommon.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System.IO;

namespace SharpFNT
{
    public sealed class BitmapFontCommon
    {
        public const int SizeInBytes = 15;

        public int LineHeight { get; set; }
        public int Base { get; set; }
        public int ScaleWidth { get; set; }
        public int ScaleHeight { get; set; }
        public bool Packed { get; set; }
        public ChannelData AlphaChannel { get; set; }
        public ChannelData RedChannel { get; set; }
        public ChannelData GreenChannel { get; set; }
        public ChannelData BlueChannel { get; set; }

        public void WriteBinary(BinaryWriter binaryWriter, int pages)
        {
            binaryWriter.Write(SizeInBytes);
            binaryWriter.Write((ushort)this.LineHeight);
            binaryWriter.Write((ushort)this.Base);
            binaryWriter.Write((ushort)this.ScaleWidth);
            binaryWriter.Write((ushort)this.ScaleHeight);
            binaryWriter.Write((ushort)pages);

            byte packed = 0;
            packed = packed.SetBit(7, this.Packed);
            binaryWriter.Write(packed);

            binaryWriter.Write((byte)this.AlphaChannel);
            binaryWriter.Write((byte)this.RedChannel);
            binaryWriter.Write((byte)this.GreenChannel);
            binaryWriter.Write((byte)this.BlueChannel);
        }

        public static BitmapFontCommon ReadBinary(BinaryReader binaryReader, out int pageCount)
        {
            if (binaryReader.ReadInt32() != SizeInBytes)
            {
                throw new InvalidDataException("Invalid common block size.");
            }

            BitmapFontCommon binary = new BitmapFontCommon();

            binary.LineHeight = binaryReader.ReadUInt16();
            binary.Base = binaryReader.ReadUInt16();
            binary.ScaleWidth = binaryReader.ReadUInt16();
            binary.ScaleHeight = binaryReader.ReadUInt16();

            pageCount = binaryReader.ReadUInt16();

            binary.Packed = binaryReader.ReadByte().IsBitSet(7);
            binary.AlphaChannel = (ChannelData)binaryReader.ReadByte();
            binary.RedChannel = (ChannelData)binaryReader.ReadByte();
            binary.GreenChannel = (ChannelData)binaryReader.ReadByte();
            binary.BlueChannel = (ChannelData)binaryReader.ReadByte();

            return binary;
        }
    }
}