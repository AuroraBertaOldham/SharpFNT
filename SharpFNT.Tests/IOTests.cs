// ****************************************************************************
// WriteTests.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace SharpFNT.Tests
{
    [TestClass]
    public class IOTests
    {
        [TestMethod]
        public void BinaryWrite()
        {
            BitmapFont one = BitmapFont.FromFile("Binary.fnt");
            BitmapFont two = ReadBackBinary(one);

            Compare(one, two);
        }

        [TestMethod]
        public void XMLWrite()
        {
            BitmapFont one = BitmapFont.FromFile("XML.fnt");
            BitmapFont two = ReadBackXML(one);

            Compare(one, two);
        }

        [TestMethod]
        public void TextWrite()
        {
            BitmapFont one = BitmapFont.FromFile("Text.fnt");
            BitmapFont two = ReadBackText(one);

            Compare(one, two);
        }

        [TestMethod]
        public void BinaryCharsetNull()
        {
            BitmapFont bitmapFont = new BitmapFont {Info = new BitmapFontInfo {Charset = null}};
            Assert.AreEqual(CharacterSet.ANSI.ToString(), ReadBackBinary(bitmapFont).Info.Charset);
        }

        [TestMethod]
        public void XMLCharsetNull()
        {
            BitmapFont bitmapFont = new BitmapFont { Info = new BitmapFontInfo { Charset = null } };
            Assert.AreEqual(string.Empty, ReadBackXML(bitmapFont).Info.Charset);
        }

        [TestMethod]
        public void TextCharsetNull()
        {
            BitmapFont bitmapFont = new BitmapFont { Info = new BitmapFontInfo { Charset = null } };
            Assert.AreEqual(string.Empty, ReadBackText(bitmapFont).Info.Charset);
        }

        private static void Compare(BitmapFont one, BitmapFont two)
        {
            Assert.AreEqual(one.Info == null, two.Info == null);

            if (one.Info != null)
            {
                Assert.AreEqual(one.Info.Unicode, two.Info.Unicode);
                Assert.AreEqual(one.Info.Bold, two.Info.Bold);
                Assert.AreEqual(one.Info.Charset, two.Info.Charset);
                Assert.AreEqual(one.Info.Face, two.Info.Face);
                Assert.AreEqual(one.Info.Italic, two.Info.Italic);
                Assert.AreEqual(one.Info.Outline, two.Info.Outline);
                Assert.AreEqual(one.Info.PaddingDown, two.Info.PaddingDown);
                Assert.AreEqual(one.Info.PaddingLeft, two.Info.PaddingLeft);
                Assert.AreEqual(one.Info.PaddingRight, two.Info.PaddingRight);
                Assert.AreEqual(one.Info.PaddingUp, two.Info.PaddingUp);
                Assert.AreEqual(one.Info.Size, two.Info.Size);
                Assert.AreEqual(one.Info.Smooth, two.Info.Smooth);
                Assert.AreEqual(one.Info.SpacingHorizontal, two.Info.SpacingHorizontal);
                Assert.AreEqual(one.Info.SpacingVertical, two.Info.SpacingVertical);
                Assert.AreEqual(one.Info.StretchHeight, two.Info.StretchHeight);
                Assert.AreEqual(one.Info.Outline, two.Info.Outline);
                Assert.AreEqual(one.Info.SuperSamplingLevel, two.Info.SuperSamplingLevel);
            }

            Assert.AreEqual(one.Common == null, two.Common == null);

            if (one.Common != null)
            {
                Assert.AreEqual(one.Common.AlphaChannel, two.Common.AlphaChannel);
                Assert.AreEqual(one.Common.Base, two.Common.Base);
                Assert.AreEqual(one.Common.BlueChannel, two.Common.BlueChannel);
                Assert.AreEqual(one.Common.GreenChannel, two.Common.GreenChannel);
                Assert.AreEqual(one.Common.LineHeight, two.Common.LineHeight);
                Assert.AreEqual(one.Common.Packed, two.Common.Packed);
                Assert.AreEqual(one.Common.RedChannel, two.Common.RedChannel);
                Assert.AreEqual(one.Common.ScaleHeight, two.Common.ScaleHeight);
                Assert.AreEqual(one.Common.ScaleWidth, two.Common.ScaleWidth);
            }

            Assert.AreEqual(one.Pages.Count, two.Pages.Count);
            Assert.AreEqual(one.Pages == null, two.Pages == null);

            if (one.Pages != null)
            {
                foreach (KeyValuePair<int, string> keyValuePair in one.Pages)
                {
                    if (!two.Pages.TryGetValue(keyValuePair.Key, out string value) || value != keyValuePair.Value)
                    {
                        Assert.Fail();
                    }
                }
            }

            Assert.AreEqual(one.Characters.Count, two.Characters.Count);
            Assert.AreEqual(one.Characters == null, two.Characters == null);

            if (one.Characters != null)
            {
                foreach (KeyValuePair<int, Character> keyValuePair in one.Characters)
                {
                    if (!two.Characters.TryGetValue(keyValuePair.Key, out Character value))
                    {
                        Assert.Fail();
                    }

                    Assert.AreEqual(keyValuePair.Value.ID, value.ID);
                    Assert.AreEqual(keyValuePair.Value.Channel, value.Channel);
                    Assert.AreEqual(keyValuePair.Value.Height, value.Height);
                    Assert.AreEqual(keyValuePair.Value.Page, value.Page);
                    Assert.AreEqual(keyValuePair.Value.Width, value.Width);
                    Assert.AreEqual(keyValuePair.Value.X, value.X);
                    Assert.AreEqual(keyValuePair.Value.Y, value.Y);
                    Assert.AreEqual(keyValuePair.Value.XAdvance, value.XAdvance);
                    Assert.AreEqual(keyValuePair.Value.XOffset, value.XOffset);
                    Assert.AreEqual(keyValuePair.Value.YOffset, value.YOffset);
                }
            }

            Assert.AreEqual(one.KerningPairs.Count, two.KerningPairs.Count);
            Assert.AreEqual(one.KerningPairs == null, two.KerningPairs == null);

            if (one.KerningPairs != null)
            {
                foreach (KeyValuePair<KerningPair, int> keyValuePair in one.KerningPairs)
                {
                    if (!two.KerningPairs.TryGetValue(keyValuePair.Key, out int value))
                    {
                        Assert.Fail("Missing kerning pair.");
                    }

                    Assert.AreEqual(keyValuePair.Value, value, "One's kerning amount does not equal the two's.");
                    Assert.AreEqual(keyValuePair.Key.Amount, value, "The kerning amount does not match the property value.");
                }
            }
        }

        private static BitmapFont ReadBackBinary(BitmapFont bitmapFont)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.Unicode, true))
                {
                    bitmapFont.WriteBinary(binaryWriter);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    return BitmapFont.ReadBinary(binaryReader);
                }
            }
        }
        private static BitmapFont ReadBackXML(BitmapFont bitmapFont)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    CloseOutput = false,
                    Encoding = Encoding.Unicode
                };

                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    bitmapFont.WriteXML(xmlWriter);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.Unicode))
                {
                    return BitmapFont.ReadXML(streamReader);
                }
            }
        }
        private static BitmapFont ReadBackText(BitmapFont bitmapFont)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.Unicode, 1024, true))
                {
                    bitmapFont.WriteText(streamWriter);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.Unicode))
                {
                    return BitmapFont.ReadText(streamReader);
                }
            }
        }
    }
}
