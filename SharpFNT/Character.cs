// ****************************************************************************
// Character.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System.IO;
using System.Text;
using System.Xml.Linq;

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

        public void WriteXML(XElement element) 
        {
            element.SetAttributeValue("id", (int) this.Char);
            element.SetAttributeValue("x", this.X);
            element.SetAttributeValue("y", this.Y);
            element.SetAttributeValue("width", this.Width);
            element.SetAttributeValue("height", this.Height);
            element.SetAttributeValue("xoffset", this.XOffset);
            element.SetAttributeValue("yoffset", this.YOffset);
            element.SetAttributeValue("xadvance", this.XAdvance);
            element.SetAttributeValue("page", this.Page);
            element.SetAttributeValue("chnl", this.Channel);
        }

        public void WriteText(StringBuilder stringBuilder)
        {
            TextFormatUtility.WriteInt("id", (int) this.Char, stringBuilder);
            TextFormatUtility.WriteInt("x", this.X, stringBuilder);
            TextFormatUtility.WriteInt("y", this.Y, stringBuilder);
            TextFormatUtility.WriteInt("width", this.Width, stringBuilder);
            TextFormatUtility.WriteInt("height", this.Height, stringBuilder);
            TextFormatUtility.WriteInt("xoffset", this.XOffset, stringBuilder);
            TextFormatUtility.WriteInt("yoffset", this.YOffset, stringBuilder);
            TextFormatUtility.WriteInt("xadvance", this.XAdvance, stringBuilder);
            TextFormatUtility.WriteInt("page", this.Page, stringBuilder);
            TextFormatUtility.WriteValue("chnl", this.Channel.ToString(), stringBuilder);
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

        public static Character ReadXML(XElement element)
        {
            Character character = new Character();

            character.Char = (char)(int)element.Attribute("id");
            character.X = (int)element.Attribute("x");
            character.Y = (int)element.Attribute("y");
            character.Width = (int)element.Attribute("width");
            character.Height = (int)element.Attribute("height");
            character.XOffset = (int)element.Attribute("xoffset");
            character.YOffset = (int)element.Attribute("yoffset");
            character.XAdvance = (int)element.Attribute("xadvance");
            character.Page = (int)element.Attribute("page");
            character.Channel = element.Attribute("chnl").GetEnumValue<Channel>();

            return character;
        }

        public static Character ReadText(string[] lineSegments) 
        {
            Character character = new Character();

            character.Char = (char)TextFormatUtility.ReadInt("id", lineSegments);
            character.X = TextFormatUtility.ReadInt("x", lineSegments);
            character.Y = TextFormatUtility.ReadInt("y", lineSegments);
            character.Width = TextFormatUtility.ReadInt("width", lineSegments);
            character.Height = TextFormatUtility.ReadInt("height", lineSegments);
            character.XOffset = TextFormatUtility.ReadInt("xoffset", lineSegments);
            character.YOffset = TextFormatUtility.ReadInt("yoffset", lineSegments);
            character.XAdvance = TextFormatUtility.ReadInt("xadvance", lineSegments);
            character.Page = TextFormatUtility.ReadInt("page", lineSegments);
            character.Channel = TextFormatUtility.ReadEnum<Channel>("chnl", lineSegments);

            return character;
        }
    }
}