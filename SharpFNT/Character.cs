// ****************************************************************************
// Character.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System.IO;

namespace SharpFNT
{
    public sealed class Character
    {
        public const int SizeInBytes = 20;

        public char Char { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public int XAdvance { get; set; }
        public int Page { get; set; }
        public Channel Channel { get; set; }

        public void WriteBinary(BinaryWriter binaryWriter) 
        {
            binaryWriter.Write((uint) this.Char);
            binaryWriter.Write((ushort) this.X);
            binaryWriter.Write((ushort) this.Y);
            binaryWriter.Write((ushort) this.Width);
            binaryWriter.Write((ushort) this.Height);
            binaryWriter.Write((short) this.XOffset);
            binaryWriter.Write((short) this.YOffset);
            binaryWriter.Write((short) this.XAdvance);
            binaryWriter.Write((byte) this.Page);
            binaryWriter.Write((byte) this.Channel);
        }

        public static Character ReadBinary(BinaryReader binaryReader)
        {
            return new Character
            {
                Char = (char)binaryReader.ReadUInt32(),
                X = binaryReader.ReadUInt16(),
                Y = binaryReader.ReadUInt16(),
                Width = binaryReader.ReadUInt16(),
                Height = binaryReader.ReadUInt16(),
                XOffset = binaryReader.ReadInt16(),
                YOffset = binaryReader.ReadInt16(),
                XAdvance = binaryReader.ReadInt16(),
                Page = binaryReader.ReadByte(),
                Channel = (Channel)binaryReader.ReadByte()
            };
        }
    }
}