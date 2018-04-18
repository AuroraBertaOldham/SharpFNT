// ****************************************************************************
// BitmapFontCommon.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System.IO;
using System.Text;
using System.Xml.Linq;

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

        public void WriteXML(XElement element, int pages) 
        {
            element.SetAttributeValue("lineHeight", this.LineHeight);
            element.SetAttributeValue("base", this.Base);
            element.SetAttributeValue("scaleW", this.ScaleWidth);
            element.SetAttributeValue("scaleH", this.ScaleHeight);

            element.SetAttributeValue("pages", pages);

            element.SetAttributeValue("packed", this.Packed);

            element.SetAttributeValue("alphaChnl", this.AlphaChannel);
            element.SetAttributeValue("redChnl", this.RedChannel);
            element.SetAttributeValue("greenChnl", this.GreenChannel);
            element.SetAttributeValue("blueChnl", this.BlueChannel);
        }

        public void WriteText(StringBuilder stringBuilder, int pages)
        {
            TextFormatUtility.WriteInt("lineHeight", this.LineHeight, stringBuilder);
            TextFormatUtility.WriteInt("base", this.Base, stringBuilder);
            TextFormatUtility.WriteInt("scaleW", this.ScaleWidth, stringBuilder);
            TextFormatUtility.WriteInt("scaleH", this.ScaleHeight, stringBuilder);

            TextFormatUtility.WriteInt("pages", pages, stringBuilder);

            TextFormatUtility.WriteBool("packed", this.Packed, stringBuilder);

            TextFormatUtility.WriteValue("alphaChnl", this.AlphaChannel.ToString(), stringBuilder);
            TextFormatUtility.WriteValue("redChnl", this.RedChannel.ToString(), stringBuilder);
            TextFormatUtility.WriteValue("greenChnl", this.GreenChannel.ToString(), stringBuilder);
            TextFormatUtility.WriteValue("blueChnl", this.BlueChannel.ToString(), stringBuilder);
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

        public static BitmapFontCommon ReadXML(XElement element, out int pages) 
        {
            BitmapFontCommon bitmapFontCommon = new BitmapFontCommon();

            bitmapFontCommon.LineHeight = (int)element.Attribute("lineHeight");
            bitmapFontCommon.Base = (int)element.Attribute("base");
            bitmapFontCommon.ScaleWidth = (int)element.Attribute("scaleW");
            bitmapFontCommon.ScaleHeight = (int)element.Attribute("scaleH");

            pages = (int)element.Attribute("pages");

            bitmapFontCommon.Packed = (bool)element.Attribute("packed");
            
            bitmapFontCommon.AlphaChannel = element.Attribute("alphaChnl").GetEnumValue<ChannelData>();
            bitmapFontCommon.RedChannel = element.Attribute("redChnl").GetEnumValue<ChannelData>();
            bitmapFontCommon.GreenChannel = element.Attribute("greenChnl").GetEnumValue<ChannelData>();
            bitmapFontCommon.BlueChannel = element.Attribute("blueChnl").GetEnumValue<ChannelData>();

            return bitmapFontCommon;
        }

        public static BitmapFontCommon ReadText(string[] lineSegments, out int pages) 
        {
            BitmapFontCommon bitmapFontCommon = new BitmapFontCommon();

            bitmapFontCommon.LineHeight = TextFormatUtility.ReadInt("lineHeight", lineSegments);
            bitmapFontCommon.Base = TextFormatUtility.ReadInt("base", lineSegments);
            bitmapFontCommon.ScaleWidth = TextFormatUtility.ReadInt("scaleW", lineSegments);
            bitmapFontCommon.ScaleHeight = TextFormatUtility.ReadInt("scaleH", lineSegments);

            pages = TextFormatUtility.ReadInt("pages", lineSegments);

            bitmapFontCommon.Packed = TextFormatUtility.ReadBool("packed", lineSegments);
            
            bitmapFontCommon.AlphaChannel = TextFormatUtility.ReadEnum<ChannelData>("alphaChnl", lineSegments);
            bitmapFontCommon.RedChannel = TextFormatUtility.ReadEnum<ChannelData>("redChnl", lineSegments);
            bitmapFontCommon.GreenChannel = TextFormatUtility.ReadEnum<ChannelData>("greenChnl", lineSegments);
            bitmapFontCommon.BlueChannel = TextFormatUtility.ReadEnum<ChannelData>("blueChnl", lineSegments);

            return bitmapFontCommon;
        }
    }
}