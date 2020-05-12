//**************************************************************************************************
// Character.cs                                                                                    *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

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
            binaryWriter.Write((ushort)X);
            binaryWriter.Write((ushort)Y);
            binaryWriter.Write((ushort)Width);
            binaryWriter.Write((ushort)Height);
            binaryWriter.Write((short)XOffset);
            binaryWriter.Write((short)YOffset);
            binaryWriter.Write((short)XAdvance);
            binaryWriter.Write((byte)Page);
            binaryWriter.Write((byte)Channel);
        }
        public void WriteXML(XElement element, int id) 
        {
            element.SetAttributeValue("id", id);
            element.SetAttributeValue("x", X);
            element.SetAttributeValue("y", Y);
            element.SetAttributeValue("width", Width);
            element.SetAttributeValue("height", Height);
            element.SetAttributeValue("xoffset", XOffset);
            element.SetAttributeValue("yoffset", YOffset);
            element.SetAttributeValue("xadvance", XAdvance);
            element.SetAttributeValue("page", Page);
            element.SetAttributeValue("chnl", (int)Channel);
        }
        public void WriteText(TextWriter textWriter, int id)
        {
            TextFormatUtility.WriteInt("id", id, textWriter);
            TextFormatUtility.WriteInt("x", X, textWriter);
            TextFormatUtility.WriteInt("y", Y, textWriter);
            TextFormatUtility.WriteInt("width", Width, textWriter);
            TextFormatUtility.WriteInt("height", Height, textWriter);
            TextFormatUtility.WriteInt("xoffset", XOffset, textWriter);
            TextFormatUtility.WriteInt("yoffset", YOffset, textWriter);
            TextFormatUtility.WriteInt("xadvance", XAdvance, textWriter);
            TextFormatUtility.WriteInt("page", Page, textWriter);
            TextFormatUtility.WriteEnum("chnl", Channel, textWriter);
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
            id = (int?)element.Attribute("id") ?? 0;

            return new Character
            {
                X = (int?)element.Attribute("x") ?? 0,
                Y = (int?)element.Attribute("y") ?? 0,
                Width = (int?)element.Attribute("width") ?? 0,
                Height = (int?)element.Attribute("height") ?? 0,
                XOffset = (int?)element.Attribute("xoffset") ?? 0,
                YOffset = (int?)element.Attribute("yoffset") ?? 0,
                XAdvance = (int?)element.Attribute("xadvance") ?? 0,
                Page = (int?)element.Attribute("page") ?? 0,
                Channel = element.Attribute("chnl")?.GetEnumValue<Channel>() ?? Channel.None
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