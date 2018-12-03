// ****************************************************************************
// Character.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace SharpFNT
{
    public sealed class Character
    {
        public const int SizeInBytes = 20;

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public int XAdvance { get; set; }
        public int Page { get; set; }
        public Channel Channel { get; set; }

        public void WriteBinary(BinaryWriter binaryWriter, int id) 
        {
            binaryWriter.Write((uint)id);
            binaryWriter.Write((ushort)this.X);
            binaryWriter.Write((ushort)this.Y);
            binaryWriter.Write((ushort)this.Width);
            binaryWriter.Write((ushort)this.Height);
            binaryWriter.Write((short)this.XOffset);
            binaryWriter.Write((short)this.YOffset);
            binaryWriter.Write((short)this.XAdvance);
            binaryWriter.Write((byte)this.Page);
            binaryWriter.Write((byte)this.Channel);
        }
        public void WriteXML(XElement element, int id) 
        {
            element.SetAttributeValue("id", id);
            element.SetAttributeValue("x", this.X);
            element.SetAttributeValue("y", this.Y);
            element.SetAttributeValue("width", this.Width);
            element.SetAttributeValue("height", this.Height);
            element.SetAttributeValue("xoffset", this.XOffset);
            element.SetAttributeValue("yoffset", this.YOffset);
            element.SetAttributeValue("xadvance", this.XAdvance);
            element.SetAttributeValue("page", this.Page);
            element.SetAttributeValue("chnl", (int)this.Channel);
        }
        public void WriteText(TextWriter textWriter, int id)
        {
            TextFormatUtility.WriteInt("id", id, textWriter);
            TextFormatUtility.WriteInt("x", this.X, textWriter);
            TextFormatUtility.WriteInt("y", this.Y, textWriter);
            TextFormatUtility.WriteInt("width", this.Width, textWriter);
            TextFormatUtility.WriteInt("height", this.Height, textWriter);
            TextFormatUtility.WriteInt("xoffset", this.XOffset, textWriter);
            TextFormatUtility.WriteInt("yoffset", this.YOffset, textWriter);
            TextFormatUtility.WriteInt("xadvance", this.XAdvance, textWriter);
            TextFormatUtility.WriteInt("page", this.Page, textWriter);
            TextFormatUtility.WriteEnum("chnl", this.Channel, textWriter);
        }

        public static Character ReadBinary(BinaryReader binaryReader, out int id)
        {
            id = (int)binaryReader.ReadUInt32();

            return new Character
            {
                X = binaryReader.ReadUInt16(),
                Y = binaryReader.ReadUInt16(),
                Width = binaryReader.ReadUInt16(),
                Height = binaryReader.ReadUInt16(),
                XOffset = binaryReader.ReadInt16(),
                YOffset = binaryReader.ReadInt16(),
                XAdvance = binaryReader.ReadInt16(),
                Page = binaryReader.ReadByte(),
                Channel = (Channel) binaryReader.ReadByte()
            };
        }
        public static Character ReadXML(XElement element, out int id)
        {
            id = (int) element.Attribute("id");

            return new Character
            {
                X = (int)element.Attribute("x"),
                Y = (int)element.Attribute("y"),
                Width = (int)element.Attribute("width"),
                Height = (int)element.Attribute("height"),
                XOffset = (int)element.Attribute("xoffset"),
                YOffset = (int)element.Attribute("yoffset"),
                XAdvance = (int)element.Attribute("xadvance"),
                Page = (int)element.Attribute("page"),
                Channel = element.Attribute("chnl").GetEnumValue<Channel>()
            };
        }
        public static Character ReadText(IReadOnlyList<string> lineSegments, out int id)
        {
            id = TextFormatUtility.ReadInt("id", lineSegments);

            return new Character
            {
                X = TextFormatUtility.ReadInt("x", lineSegments),
                Y = TextFormatUtility.ReadInt("y", lineSegments),
                Width = TextFormatUtility.ReadInt("width", lineSegments),
                Height = TextFormatUtility.ReadInt("height", lineSegments),
                XOffset = TextFormatUtility.ReadInt("xoffset", lineSegments),
                YOffset = TextFormatUtility.ReadInt("yoffset", lineSegments),
                XAdvance = TextFormatUtility.ReadInt("xadvance", lineSegments),
                Page = TextFormatUtility.ReadInt("page", lineSegments),
                Channel = TextFormatUtility.ReadEnum<Channel>("chnl", lineSegments)
            };
        }
    }
}