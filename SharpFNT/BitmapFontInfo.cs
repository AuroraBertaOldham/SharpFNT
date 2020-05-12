//**************************************************************************************************
// BitmapFontInfo.cs                                                                               *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
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
            binaryWriter.Write(MinSizeInBytes + (Face?.Length ?? 0));
            binaryWriter.Write((short)Size);

            byte bitField = 0;

            bitField = bitField.SetBit(7, Smooth);
            bitField = bitField.SetBit(6, Unicode);
            bitField = bitField.SetBit(5, Italic);
            bitField = bitField.SetBit(4, Bold);

            binaryWriter.Write(bitField);

            byte characterSetID = 0;

            if (!string.IsNullOrEmpty(Charset))
            {
                if (!Enum.TryParse(Charset, true, out CharacterSet characterSet))
                {
                    throw new FormatException("Invalid character set.");
                }

                characterSetID = (byte)characterSet;
            }

            binaryWriter.Write(characterSetID);

            binaryWriter.Write((ushort)StretchHeight);
            binaryWriter.Write((byte)SuperSamplingLevel);

            binaryWriter.Write((byte)PaddingUp);
            binaryWriter.Write((byte)PaddingRight);
            binaryWriter.Write((byte)PaddingDown);
            binaryWriter.Write((byte)PaddingLeft);

            binaryWriter.Write((byte)SpacingHorizontal);
            binaryWriter.Write((byte)SpacingVertical);

            binaryWriter.Write((byte)Outline);
            binaryWriter.WriteNullTerminatedString(Face);
        }
        public void WriteXML(XElement element) 
        {
            element.SetAttributeValue("face", Face ?? string.Empty);
            element.SetAttributeValue("size", Size); 
            element.SetAttributeValue("bold", Convert.ToInt32(Bold));
            element.SetAttributeValue("italic", Convert.ToInt32(Italic));

            element.SetAttributeValue("charset", Charset ?? string.Empty);

            element.SetAttributeValue("unicode", Convert.ToInt32(Unicode));
            element.SetAttributeValue("stretchH", StretchHeight);
            element.SetAttributeValue("smooth", Convert.ToInt32(Smooth));
            element.SetAttributeValue("aa", SuperSamplingLevel);

            var padding = $"{PaddingUp},{PaddingRight},{PaddingDown},{PaddingLeft}";
            element.SetAttributeValue("padding", padding);

            var spacing = $"{SpacingHorizontal},{SpacingVertical}";
            element.SetAttributeValue("spacing", spacing);

            element.SetAttributeValue("outline", Outline);
        }
        public void WriteText(TextWriter textWriter) 
        {
            TextFormatUtility.WriteString("face", Face ?? string.Empty, textWriter);
            TextFormatUtility.WriteInt("size", Size, textWriter);
            TextFormatUtility.WriteBool("bold", Bold, textWriter);
            TextFormatUtility.WriteBool("italic", Italic, textWriter);

            TextFormatUtility.WriteString("charset", Charset ?? string.Empty, textWriter);

            TextFormatUtility.WriteBool("unicode", Unicode, textWriter);
            TextFormatUtility.WriteInt("stretchH", StretchHeight, textWriter);
            TextFormatUtility.WriteBool("smooth", Smooth, textWriter);
            TextFormatUtility.WriteInt("aa", SuperSamplingLevel, textWriter);

            var padding = $"{PaddingUp},{PaddingRight},{PaddingDown},{PaddingLeft}";
            TextFormatUtility.WriteValue("padding", padding, textWriter);

            var spacing = $"{SpacingHorizontal},{SpacingVertical}";
            TextFormatUtility.WriteValue("spacing", spacing, textWriter);

            TextFormatUtility.WriteInt("outline", Outline, textWriter);
        }

        public static BitmapFontInfo ReadBinary(BinaryReader binaryReader)
        {
            if (binaryReader.ReadInt32() < MinSizeInBytes)
            {
                throw new InvalidDataException("Invalid info block size.");
            }

            var bitmapFontInfo = new BitmapFontInfo();

            bitmapFontInfo.Size = binaryReader.ReadInt16();

            var bitField = binaryReader.ReadByte();

            bitmapFontInfo.Smooth = bitField.IsBitSet(7);
            bitmapFontInfo.Unicode = bitField.IsBitSet(6);
            bitmapFontInfo.Italic = bitField.IsBitSet(5);
            bitmapFontInfo.Bold = bitField.IsBitSet(4);

            var characterSet = (CharacterSet)binaryReader.ReadByte();
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
            bitmapFontInfo.Face = binaryReader.ReadNullTerminatedString();

            return bitmapFontInfo;
        }
        public static BitmapFontInfo ReadXML(XElement element)
        {
            var bitmapFontInfo = new BitmapFontInfo();

            bitmapFontInfo.Face = (string)element.Attribute("face") ?? string.Empty;
            bitmapFontInfo.Size = (int?)element.Attribute("size") ?? 0;
            bitmapFontInfo.Bold = (bool?)element.Attribute("bold") ?? false;
            bitmapFontInfo.Italic = (bool?)element.Attribute("italic") ?? false;

            bitmapFontInfo.Charset = (string)element.Attribute("charset") ?? string.Empty;

            bitmapFontInfo.Unicode = (bool?)element.Attribute("unicode") ?? false;
            bitmapFontInfo.StretchHeight = (int?)element.Attribute("stretchH") ?? 0;
            bitmapFontInfo.Smooth = (bool?)element.Attribute("smooth") ?? false;
            bitmapFontInfo.SuperSamplingLevel = (int?)element.Attribute("aa") ?? 0;
            
            var padding = ((string)element.Attribute("padding"))?.Split(',');
            if (padding != null)
            {
                bitmapFontInfo.PaddingLeft = padding.Length > 3 ? int.Parse(padding[3]) : 0;
                bitmapFontInfo.PaddingDown = padding.Length > 2 ? int.Parse(padding[2]) : 0;
                bitmapFontInfo.PaddingRight = padding.Length > 1 ? int.Parse(padding[1]) : 0;
                bitmapFontInfo.PaddingUp = padding.Length > 0 ? int.Parse(padding[0]) : 0;
            }

            var spacing = ((string)element.Attribute("spacing"))?.Split(',');
            if (spacing != null)
            {
                bitmapFontInfo.SpacingVertical = spacing.Length > 1 ? int.Parse(spacing[1]) : 0;
                bitmapFontInfo.SpacingHorizontal = spacing.Length > 0 ? int.Parse(spacing[0]) : 0;
            }

            bitmapFontInfo.Outline = (int?)element.Attribute("outline") ?? 0;

            return bitmapFontInfo;
        }
        public static BitmapFontInfo ReadText(IReadOnlyList<string> lineSegments) 
        {
            var bitmapFontInfo = new BitmapFontInfo();

            bitmapFontInfo.Face = TextFormatUtility.ReadString("face", lineSegments, string.Empty);
            bitmapFontInfo.Size = TextFormatUtility.ReadInt("size", lineSegments);
            bitmapFontInfo.Bold = TextFormatUtility.ReadBool("bold", lineSegments);
            bitmapFontInfo.Italic = TextFormatUtility.ReadBool("italic", lineSegments);

            bitmapFontInfo.Charset = TextFormatUtility.ReadString("charset", lineSegments, string.Empty);

            bitmapFontInfo.Unicode = TextFormatUtility.ReadBool("unicode", lineSegments);
            bitmapFontInfo.StretchHeight = TextFormatUtility.ReadInt("stretchH", lineSegments);
            bitmapFontInfo.Smooth = TextFormatUtility.ReadBool("smooth", lineSegments);
            bitmapFontInfo.SuperSamplingLevel = TextFormatUtility.ReadInt("aa", lineSegments);

            var padding = TextFormatUtility.ReadValue("padding", lineSegments)?.Split(',');
            if (padding != null)
            {
                bitmapFontInfo.PaddingLeft = padding.Length > 3 ? int.Parse(padding[3]) : 0;
                bitmapFontInfo.PaddingDown = padding.Length > 2 ? int.Parse(padding[2]) : 0;
                bitmapFontInfo.PaddingRight = padding.Length > 1 ? int.Parse(padding[1]) : 0;
                bitmapFontInfo.PaddingUp = padding.Length > 0 ? int.Parse(padding[0]) : 0;
            }

            var spacing = TextFormatUtility.ReadValue("spacing", lineSegments)?.Split(',');
            if (spacing != null)
            {
                bitmapFontInfo.SpacingVertical = spacing.Length > 1 ? int.Parse(spacing[1]) : 0;
                bitmapFontInfo.SpacingHorizontal = spacing.Length > 0 ? int.Parse(spacing[0]) : 0;
            }

            bitmapFontInfo.Outline = TextFormatUtility.ReadInt("outline", lineSegments);

            return bitmapFontInfo;
        }
    }
}
