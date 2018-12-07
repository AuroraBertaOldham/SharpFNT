// ****************************************************************************
// WriteTests.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SharpFNT.Tests
{
    [TestClass]
    public class BitmapFontTests
    {
        public BitmapFont TestFont { get; } = new BitmapFont
        {
            Info = new BitmapFontInfo
            {
                Bold = true,
                Italic = true,
                Charset = CharacterSet.ANSI.ToString(),
                Face = "Test",
                Outline = 5,
                PaddingRight = 1,
                PaddingUp = 2,
                PaddingLeft = 3,
                PaddingDown = 4,
                Size = 7,
                SpacingHorizontal = 8,
                SpacingVertical = 9,
                Smooth = true,
                Unicode = false,
                StretchHeight = 10,
                SuperSamplingLevel = 11
            },
            Common = new BitmapFontCommon
            {
                LineHeight = 12,
                AlphaChannel = ChannelData.Glyph,
                Base = 13,
                BlueChannel = ChannelData.GlyphAndOutline,
                GreenChannel = ChannelData.One,
                Packed = true, 
                RedChannel = ChannelData.Outline,
                ScaleWidth = 14,
                ScaleHeight = 15
            },
            Pages = new Dictionary<int, string>
            {
                { 0, "Page_0.png" },
                { 1, "Page_0.png" },
                { 2, "Page_0.png" }
            },
            Characters = new Dictionary<int, Character>()
            {
                { 'A', new Character
                    {
                        Channel = Channel.All,
                        Height = 16,
                        Page = 17,
                        Width = 18,
                        X = 19,
                        XAdvance = -20,
                        XOffset = 21,
                        Y = 22,
                        YOffset = 23
                    }
                },
                { 'B', new Character
                    {
                        Channel = Channel.Alpha,
                        Height = 24,
                        Page = 25,
                        Width = 26,
                        X = 27,
                        XAdvance = 28,
                        XOffset = -29,
                        Y = 30,
                        YOffset = 31
                    }
                },
                { 'C', new Character
                    {
                        Channel = Channel.Blue,
                        Height = 32,
                        Page = 33,
                        Width = 34,
                        X = 35,
                        XAdvance = 36,
                        XOffset = 37,
                        Y = 38,
                        YOffset = -39
                    }
                }           
            },
            KerningPairs = new Dictionary<KerningPair, int>
            {
                { new KerningPair('A', 'B'), 40},
                { new KerningPair('B', 'C'), -41},
                { new KerningPair('A', 'C'), 42}
            }
        };

        // Binary

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ReadBinaryWrongMagic()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(BitmapFont.MagicOne - 1);
                    binaryWriter.Write(BitmapFont.MagicTwo + 1);
                    binaryWriter.Write(BitmapFont.MagicThree);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont.FromStream(memoryStream, FormatHint.Binary, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ReadBinaryWrongVersion()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(BitmapFont.MagicOne);
                    binaryWriter.Write(BitmapFont.MagicTwo);
                    binaryWriter.Write(BitmapFont.MagicThree);
                    binaryWriter.Write((byte)0);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont.FromStream(memoryStream, FormatHint.Binary, true);
            }
        }

        [TestMethod]
        public void ReadBinary()
        {
            BitmapFont result = BitmapFont.FromFile("TestFontBinary.fnt", FormatHint.Binary);
            Compare(this.TestFont, result);
        }

        [TestMethod]
        public void AutoReadBinary()
        {
            Compare(BitmapFont.FromFile("TestFontBinary.fnt"), this.TestFont);
        }

        [TestMethod]
        public void ReadBackBinary()
        {
            this.TestFont.Save("SaveTestBinary.fnt", FormatHint.Binary);
            BitmapFont result = BitmapFont.FromFile("SaveTestBinary.fnt", FormatHint.Binary);

            Compare(this.TestFont, result);
        }

        [TestMethod]
        public void BinaryCharsetNull()
        {
            BitmapFont bitmapFont = new BitmapFont { Info = new BitmapFontInfo { Charset = null } };

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    bitmapFont.WriteBinary(binaryWriter);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont result = BitmapFont.FromStream(memoryStream, FormatHint.Binary, true);
                Assert.AreEqual(CharacterSet.ANSI.ToString(), result.Info.Charset);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ReadBinaryCharactersWrongBlockSize()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(BitmapFont.MagicOne);
                    binaryWriter.Write(BitmapFont.MagicTwo);
                    binaryWriter.Write(BitmapFont.MagicThree);
                    binaryWriter.Write((byte)BitmapFont.ImplementedVersion);
                    binaryWriter.Write((byte)BlockID.Characters);
                    binaryWriter.Write(25);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont.FromStream(memoryStream, FormatHint.Binary, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ReadBinaryKerningWrongBlockSize()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(BitmapFont.MagicOne);
                    binaryWriter.Write(BitmapFont.MagicTwo);
                    binaryWriter.Write(BitmapFont.MagicThree);
                    binaryWriter.Write((byte)BitmapFont.ImplementedVersion);
                    binaryWriter.Write((byte)BlockID.KerningPairs);
                    binaryWriter.Write(KerningPair.SizeInBytes / 2);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont.FromStream(memoryStream, FormatHint.Binary, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void WriteBinaryInvalidPageIndices()
        {
            BitmapFont bitmapFont = new BitmapFont
            {
                Pages = new Dictionary<int, string>
                {
                    { 0, "One.png" },
                    { 2, "One.png" },
                    { 3, "One.png" }
                }
            };

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    bitmapFont.WriteBinary(binaryWriter);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ReadBinaryInvalidBlock()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(BitmapFont.MagicOne);
                    binaryWriter.Write(BitmapFont.MagicTwo);
                    binaryWriter.Write(BitmapFont.MagicThree);
                    binaryWriter.Write((byte)BitmapFont.ImplementedVersion);
                    binaryWriter.Write((byte)6);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont.FromStream(memoryStream, FormatHint.Binary, true);
            }
        }

        [TestMethod]
        public void ReadBinaryKerningDuplicate()
        {
            const char first = 'A';
            const char second = 'B';
            const int expected = 1;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                KerningPair kerningPair = new KerningPair(first, second);

                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(BitmapFont.MagicOne);
                    binaryWriter.Write(BitmapFont.MagicTwo);
                    binaryWriter.Write(BitmapFont.MagicThree);
                    binaryWriter.Write((byte)BitmapFont.ImplementedVersion);
                    binaryWriter.Write((byte)BlockID.KerningPairs);
                    binaryWriter.Write(KerningPair.SizeInBytes * 2);
                    kerningPair.WriteBinary(binaryWriter, expected);
                    kerningPair.WriteBinary(binaryWriter, expected + 1);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont result = BitmapFont.FromStream(memoryStream, FormatHint.Binary, true);
                Assert.AreEqual(expected, result.GetKerningAmount(first, second));
            }
        }

        // XML

        [TestMethod]
        public void ReadXML()
        {
            BitmapFont result = BitmapFont.FromFile("TestFontXML.fnt", FormatHint.XML);
            Compare(this.TestFont, result);
        }

        [TestMethod]
        public void AutoReadXML()
        {
            Compare(BitmapFont.FromFile("TestFontXML.fnt"), this.TestFont);
        }

        [TestMethod]
        public void ReadBackXML()
        {
            this.TestFont.Save("SaveTestXML.fnt", FormatHint.XML);
            BitmapFont result = BitmapFont.FromFile("SaveTestXML.fnt", FormatHint.XML);

            Compare(this.TestFont, result);
        }

        [TestMethod]
        public void XMLCharsetNull()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapFont bitmapFont = new BitmapFont { Info = new BitmapFontInfo { Charset = null } };

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    CloseOutput = false,
                    Encoding = Encoding.UTF8
                };

                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    bitmapFont.WriteXML(xmlWriter);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont result = BitmapFont.FromStream(memoryStream, FormatHint.XML, true);
                Assert.AreEqual(string.Empty, result.Info.Charset);
            }
        }

        [TestMethod]
        public void ReadXMLKerningDuplicate()
        {
            const char first = 'A';
            const char second = 'B';
            const int expected = 1;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                CloseOutput = false
            };

            using (MemoryStream memoryStream = new MemoryStream())
            {
                KerningPair kerningPair = new KerningPair(first, second);

                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    XDocument document = new XDocument();

                    XElement fontElement = new XElement("font");
                    document.Add(fontElement);

                    XElement kerningsElement = new XElement("kernings");
                    kerningsElement.SetAttributeValue("count", 2);

                    for (int i = 0; i < 2; i++)
                    {
                        XElement kerningElement = new XElement("kerning");
                        kerningPair.WriteXML(kerningElement, expected + i);
                        kerningsElement.Add(kerningElement);
                    }

                    fontElement.Add(kerningsElement);

                    document.WriteTo(xmlWriter);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont result = BitmapFont.FromStream(memoryStream, FormatHint.XML, true);
                Assert.AreEqual(expected, result.GetKerningAmount(first, second));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ReadXMLMissingRoot()
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                CloseOutput = false
            };

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    XDocument document = new XDocument();

                    XElement fontElement = new XElement("nothing");
                    document.Add(fontElement);

                    document.WriteTo(xmlWriter);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont.FromStream(memoryStream, FormatHint.XML, true);
            }
        }

        // Text

        [TestMethod]
        public void ReadText()
        {
            BitmapFont result = BitmapFont.FromFile("TestFontText.fnt", FormatHint.Text);
            Compare(this.TestFont, result);
        }

        [TestMethod]
        public void AutoReadText()
        {
            Compare(BitmapFont.FromFile("TestFontText.fnt"), this.TestFont);
        }

        [TestMethod]
        public void ReadBackText()
        {
            this.TestFont.Save("SaveTestText.fnt", FormatHint.Text);
            BitmapFont result = BitmapFont.FromFile("SaveTestText.fnt", FormatHint.Text);

            Compare(this.TestFont, result);
        }

        [TestMethod]
        public void TextCharsetNull()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapFont bitmapFont = new BitmapFont { Info = new BitmapFontInfo { Charset = null } };

                using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, 1024, true))
                {
                    bitmapFont.WriteText(streamWriter);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont result = BitmapFont.FromStream(memoryStream, FormatHint.Text, true);
                Assert.AreEqual(string.Empty, result.Info.Charset);
            }
        }

        [TestMethod]
        public void ReadTextKernelDuplicate()
        {
            const char first = 'A';
            const char second = 'B';
            const int expected = 1;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                KerningPair kerningPair = new KerningPair(first, second);

                using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, 1024, true))
                {
                    streamWriter.Write("kernings");
                    TextFormatUtility.WriteInt("count", 2, streamWriter);
                    streamWriter.WriteLine();

                    for (int i = 0; i < 2; i++)
                    {
                        streamWriter.Write("kerning");
                        kerningPair.WriteText(streamWriter, expected + i);
                        streamWriter.WriteLine();
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapFont result = BitmapFont.FromStream(memoryStream, FormatHint.Text, true);
                Assert.AreEqual(expected, result.GetKerningAmount(first, second));
            }
        }

        // Misc IO

        [TestMethod]
        public void ReadStreamLeaveOpenFalse()
        {
            using (FileStream fileStream = File.Open("TestFontBinary.fnt", FileMode.Open))
            {
                BitmapFont.FromStream(fileStream, false);
                Assert.IsFalse(fileStream.CanRead);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromStreamBadFormatHint()
        {
            BitmapFont.FromStream(null, (FormatHint)3, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveBadFormatHint()
        {
            BitmapFont bitmapFont = new BitmapFont();
            bitmapFont.Save("InvalidFormatHint.fnt", (FormatHint)3);
        }

        // Kerning

        [TestMethod]
        public void GetKerning()
        {
            BitmapFont bitmapFont = new BitmapFont
            {
                KerningPairs = new Dictionary<KerningPair, int> { { new KerningPair(2, 6), 5 } }
            };

            int kerningAmount = bitmapFont.GetKerningAmount((char)2, (char)6);
            Assert.AreEqual(kerningAmount, 5);
        }

        [TestMethod]
        public void GetKerningWhenNull()
        {
            BitmapFont bitmapFont = new BitmapFont
            {
                KerningPairs = null
            };

            int kerningAmount = bitmapFont.GetKerningAmount((char)2, (char)6);
            Assert.AreEqual(kerningAmount, 0);
        }

        // Character

        [TestMethod]
        public void GetCharacter()
        {
            Character character = new Character();

            BitmapFont bitmapFont = new BitmapFont
            {
                Characters = new Dictionary<int, Character>
                {
                    { 5, character }
                }
            };

            Assert.AreEqual(character, bitmapFont.GetCharacter((char)5));
        }

        [TestMethod]
        public void GetInvalidCharacter()
        {
            Character character = new Character();

            BitmapFont bitmapFont = new BitmapFont
            {
                Characters = new Dictionary<int, Character>
                {
                    { -1, character }
                }
            };

            Assert.AreEqual(character, bitmapFont.GetCharacter((char)5));
        }

        [TestMethod]
        public void GetMissingCharacter()
        {
            Character character = new Character();

            BitmapFont bitmapFont = new BitmapFont
            {
                Characters = new Dictionary<int, Character>
                {
                    { -1, character }
                }
            };

            Assert.AreEqual(null, bitmapFont.GetCharacter((char)5, false));
        }

        [TestMethod]
        public void GetCharacterWhenNull()
        {
            BitmapFont bitmapFont = new BitmapFont();
            Assert.AreEqual(null, bitmapFont.GetCharacter((char)5));
        }

        // Utility

        private static void Compare(BitmapFont one, BitmapFont two)
        {
            Assert.AreEqual(one.Info == null, two.Info == null, "An info equals null.");

            if (one.Info != null)
            {
                Assert.AreEqual(one.Info.Unicode, two.Info.Unicode, "Unicode incorrect.");
                Assert.AreEqual(one.Info.Bold, two.Info.Bold, "Bold incorrect.");
                Assert.AreEqual(one.Info.Charset, two.Info.Charset, "Charset incorrect.");
                Assert.AreEqual(one.Info.Face, two.Info.Face, "Face incorrect.");
                Assert.AreEqual(one.Info.Italic, two.Info.Italic, "Italic incorrect.");
                Assert.AreEqual(one.Info.Outline, two.Info.Outline, "Outline incorrect.");
                Assert.AreEqual(one.Info.PaddingDown, two.Info.PaddingDown, "PaddingDown incorrect.");
                Assert.AreEqual(one.Info.PaddingLeft, two.Info.PaddingLeft, "PaddingLeft incorrect.");
                Assert.AreEqual(one.Info.PaddingRight, two.Info.PaddingRight, "PaddingRight incorrect.");
                Assert.AreEqual(one.Info.PaddingUp, two.Info.PaddingUp, "PaddingUp incorrect.");
                Assert.AreEqual(one.Info.Size, two.Info.Size, "Size incorrect.");
                Assert.AreEqual(one.Info.Smooth, two.Info.Smooth, "Smooth incorrect.");
                Assert.AreEqual(one.Info.SpacingHorizontal, two.Info.SpacingHorizontal, "SpacingHorizontal incorrect.");
                Assert.AreEqual(one.Info.SpacingVertical, two.Info.SpacingVertical, "SpacingVertical incorrect.");
                Assert.AreEqual(one.Info.StretchHeight, two.Info.StretchHeight, "StretchHeight incorrect.");
                Assert.AreEqual(one.Info.Outline, two.Info.Outline, "Outline incorrect.");
                Assert.AreEqual(one.Info.SuperSamplingLevel, two.Info.SuperSamplingLevel, "SuperSamplingLevel incorrect.");
            }

            Assert.AreEqual(one.Common == null, two.Common == null, "A common equals null.");

            if (one.Common != null)
            {
                Assert.AreEqual(one.Common.AlphaChannel, two.Common.AlphaChannel, "AlphaChannel incorrect.");
                Assert.AreEqual(one.Common.Base, two.Common.Base, "Base incorrect.");
                Assert.AreEqual(one.Common.BlueChannel, two.Common.BlueChannel, "BlueChannel incorrect.");
                Assert.AreEqual(one.Common.GreenChannel, two.Common.GreenChannel, "GreenChannel incorrect.");
                Assert.AreEqual(one.Common.LineHeight, two.Common.LineHeight, "LineHeight incorrect.");
                Assert.AreEqual(one.Common.Packed, two.Common.Packed, "Packed incorrect.");
                Assert.AreEqual(one.Common.RedChannel, two.Common.RedChannel, "Red channel incorrect.");
                Assert.AreEqual(one.Common.ScaleHeight, two.Common.ScaleHeight, "ScaleHeight incorrect.");
                Assert.AreEqual(one.Common.ScaleWidth, two.Common.ScaleWidth, "ScaleWidth incorrect.");
            }

            Assert.AreEqual(one.Pages.Count, two.Pages.Count, "Page count incorrect.");
            Assert.AreEqual(one.Pages == null, two.Pages == null, "A page dictionary equals null.");

            if (one.Pages != null)
            {
                foreach (KeyValuePair<int, string> keyValuePair in one.Pages)
                {
                    if (!two.Pages.TryGetValue(keyValuePair.Key, out string value) || value != keyValuePair.Value)
                    {
                        Assert.Fail("Page not found.");
                    }
                }
            }

            Assert.AreEqual(one.Characters.Count, two.Characters.Count, "Character count incorrect.");
            Assert.AreEqual(one.Characters == null, two.Characters == null, "A character dictionary equals null.");

            if (one.Characters != null)
            {
                foreach (KeyValuePair<int, Character> keyValuePair in one.Characters)
                {
                    if (!two.Characters.TryGetValue(keyValuePair.Key, out Character value))
                    {
                        Assert.Fail("Character not found.");
                    }

                    Assert.AreEqual(keyValuePair.Value.Channel, value.Channel, "Channel incorrect.");
                    Assert.AreEqual(keyValuePair.Value.Height, value.Height, "Height incorrect.");
                    Assert.AreEqual(keyValuePair.Value.Page, value.Page, "Page incorrect.");
                    Assert.AreEqual(keyValuePair.Value.Width, value.Width, "Width incorrect.");
                    Assert.AreEqual(keyValuePair.Value.X, value.X, "X incorrect.");
                    Assert.AreEqual(keyValuePair.Value.Y, value.Y, "Y incorrect.");
                    Assert.AreEqual(keyValuePair.Value.XAdvance, value.XAdvance, "XAdvance incorrect.");
                    Assert.AreEqual(keyValuePair.Value.XOffset, value.XOffset, "XOffset incorrect.");
                    Assert.AreEqual(keyValuePair.Value.YOffset, value.YOffset, "YOffset incorrect.");
                }
            }

            Assert.AreEqual(one.KerningPairs.Count, two.KerningPairs.Count, "kerningPair count incorrect.");
            Assert.AreEqual(one.KerningPairs == null, two.KerningPairs == null, "A kerning dictionary equals null.");

            if (one.KerningPairs != null)
            {
                foreach (KeyValuePair<KerningPair, int> keyValuePair in one.KerningPairs)
                {
                    if (!two.KerningPairs.TryGetValue(keyValuePair.Key, out int value))
                    {
                        Assert.Fail("KerningPair not found.");
                    }

                    Assert.AreEqual(keyValuePair.Value, value, "Amount incorrect.");
                }
            }
        }
    }
}
