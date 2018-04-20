// ****************************************************************************
// BitmapFontInfo.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

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

        public string Charset { get; set; }
        public int StretchHeight { get; set; }
        public int SuperSamplingLevel { get; set; }

        public int PaddingUp { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingDown { get; set; }
        public int PaddingLeft { get; set; }

        public int SpacingHorizontal { get; set; }
        public int SpacingVertical { get; set; }

        public int Outline { get; set; }
        public string Face { get; set; }

        public void WriteBinary(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(MinSizeInBytes + this.Face.Length + 1);
            binaryWriter.Write((short)this.Size);

            byte bitField = 0;

            bitField = bitField.SetBit(0, this.Smooth);
            bitField = bitField.SetBit(1, this.Unicode);
            bitField = bitField.SetBit(2, this.Italic);
            bitField = bitField.SetBit(3, this.Bold);

            binaryWriter.Write(bitField);

            CharacterSet characterSet = (CharacterSet)Enum.Parse(typeof(CharacterSet), this.Charset, true);
            binaryWriter.Write((byte)characterSet);

            binaryWriter.Write((ushort)this.StretchHeight);
            binaryWriter.Write((byte)this.SuperSamplingLevel);

            binaryWriter.Write((byte)this.PaddingUp);
            binaryWriter.Write((byte)this.PaddingRight);
            binaryWriter.Write((byte)this.PaddingDown);
            binaryWriter.Write((byte)this.PaddingLeft);

            binaryWriter.Write((byte)this.SpacingHorizontal);
            binaryWriter.Write((byte)this.SpacingVertical);

            binaryWriter.Write((byte)this.Outline);
            binaryWriter.Write(this.Face, true);
        }
        public void WriteXML(XElement element) 
        {
            element.SetAttributeValue("face", this.Face);
            element.SetAttributeValue("size", this.Size); 
            element.SetAttributeValue("bold", this.Bold);
            element.SetAttributeValue("italic", this.Italic);

            element.SetAttributeValue("charset", this.Charset);

            element.SetAttributeValue("unicode", this.Unicode);
            element.SetAttributeValue("stretchH", this.StretchHeight);
            element.SetAttributeValue("smooth", this.Smooth);
            element.SetAttributeValue("aa", this.SuperSamplingLevel);

            string padding = $"{this.PaddingUp}, {this.PaddingRight}, {this.PaddingDown}, {this.PaddingLeft}";
            element.SetAttributeValue("padding", padding);

            string spacing = $"{this.SpacingHorizontal}, {this.SpacingVertical}";
            element.SetAttributeValue("spacing", spacing);

            element.SetAttributeValue("outline", this.Outline);
        }
        public void WriteText(StringBuilder stringBuilder) 
        {
            TextFormatUtility.WriteString("face", this.Face, stringBuilder);
            TextFormatUtility.WriteInt("size", this.Size, stringBuilder);
            TextFormatUtility.WriteBool("bold", this.Bold, stringBuilder);
            TextFormatUtility.WriteBool("italic", this.Italic, stringBuilder);

            TextFormatUtility.WriteString("charset", this.Charset, stringBuilder);

            TextFormatUtility.WriteBool("unicode", this.Unicode, stringBuilder);
            TextFormatUtility.WriteInt("stretchH", this.StretchHeight, stringBuilder);
            TextFormatUtility.WriteInt("aa", this.SuperSamplingLevel, stringBuilder);

            string padding = $"{this.PaddingUp}, {this.PaddingRight}, {this.PaddingDown}, {this.PaddingLeft}";
            TextFormatUtility.WriteValue("padding", padding, stringBuilder);

            string spacing = $"{this.SpacingHorizontal}, {this.SpacingVertical}";
            TextFormatUtility.WriteValue("spacing", spacing, stringBuilder);

            TextFormatUtility.WriteInt("outline", this.Outline, stringBuilder);
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

            CharacterSet characterSet = (CharacterSet)binaryReader.ReadByte();
            bitmapFontInfo.Charset = characterSet.ToString().ToUpper();

            bitmapFontInfo.StretchHeight = binaryReader.ReadUInt16();
            bitmapFontInfo.SuperSamplingLevel = binaryReader.ReadByte();

            bitmapFontInfo.PaddingUp = binaryReader.ReadByte();
            bitmapFontInfo.PaddingRight = binaryReader.ReadByte();
            bitmapFontInfo.PaddingDown = binaryReader.ReadByte();
            bitmapFontInfo.PaddingLeft = binaryReader.ReadByte();

            bitmapFontInfo.SpacingHorizontal = binaryReader.ReadByte();
            bitmapFontInfo.SpacingVertical = binaryReader.ReadByte();

            bitmapFontInfo.Outline = binaryReader.ReadByte();
            bitmapFontInfo.Face = binaryReader.ReadCString();

            return bitmapFontInfo;
        }
        public static BitmapFontInfo ReadXML(XElement element)
        {
            BitmapFontInfo bitmapFontInfo = new BitmapFontInfo();

            bitmapFontInfo.Face = (string)element.Attribute("face");
            bitmapFontInfo.Size = (int)element.Attribute("size");
            bitmapFontInfo.Bold = (bool)element.Attribute("bold");
            bitmapFontInfo.Italic = (bool)element.Attribute("italic");

            bitmapFontInfo.Charset = (string)element.Attribute("charset");

            bitmapFontInfo.Unicode = (bool)element.Attribute("unicode");
            bitmapFontInfo.StretchHeight = (int)element.Attribute("stretchH");
            bitmapFontInfo.Smooth = (bool)element.Attribute("smooth");
            bitmapFontInfo.SuperSamplingLevel = (int)element.Attribute("aa");
            
            string[] padding = ((string)element.Attribute("padding")).Split(',');
            bitmapFontInfo.PaddingUp = int.Parse(padding[0]);
            bitmapFontInfo.PaddingRight = int.Parse(padding[1]);
            bitmapFontInfo.PaddingDown = int.Parse(padding[2]);
            bitmapFontInfo.PaddingLeft = int.Parse(padding[3]);

            string[] spacing = ((string)element.Attribute("spacing")).Split(',');
            bitmapFontInfo.SpacingHorizontal = int.Parse(spacing[0]);
            bitmapFontInfo.SpacingVertical = int.Parse(spacing[1]);

            bitmapFontInfo.Outline = (int)element.Attribute("outline");

            return bitmapFontInfo;
        }
        public static BitmapFontInfo ReadText(IReadOnlyList<string> lineSegments) 
        {
            BitmapFontInfo bitmapFontInfo = new BitmapFontInfo();

            bitmapFontInfo.Face = TextFormatUtility.ReadString("face", lineSegments);
            bitmapFontInfo.Size = TextFormatUtility.ReadInt("size", lineSegments);
            bitmapFontInfo.Bold = TextFormatUtility.ReadBool("bold", lineSegments);
            bitmapFontInfo.Italic = TextFormatUtility.ReadBool("italic", lineSegments);

            bitmapFontInfo.Charset = TextFormatUtility.ReadString("charset", lineSegments);

            bitmapFontInfo.Unicode = TextFormatUtility.ReadBool("unicode", lineSegments);
            bitmapFontInfo.StretchHeight = TextFormatUtility.ReadInt("stretchH", lineSegments);
            bitmapFontInfo.Smooth = TextFormatUtility.ReadBool("smooth", lineSegments);
            bitmapFontInfo.SuperSamplingLevel = TextFormatUtility.ReadInt("aa", lineSegments);

            string[] padding = TextFormatUtility.ReadValue("padding", lineSegments).Split(',');
            bitmapFontInfo.PaddingUp = int.Parse(padding[0]);
            bitmapFontInfo.PaddingRight = int.Parse(padding[1]);
            bitmapFontInfo.PaddingDown = int.Parse(padding[2]);
            bitmapFontInfo.PaddingLeft = int.Parse(padding[3]);

            string[] spacing = TextFormatUtility.ReadValue("spacing", lineSegments).Split(',');
            bitmapFontInfo.SpacingHorizontal = int.Parse(spacing[0]);
            bitmapFontInfo.SpacingVertical = int.Parse(spacing[1]);

            bitmapFontInfo.Outline = TextFormatUtility.ReadInt("outline", lineSegments);

            return bitmapFontInfo;
        }
    }
}