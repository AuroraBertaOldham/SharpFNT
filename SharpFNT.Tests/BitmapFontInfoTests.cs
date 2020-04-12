//**************************************************************************************************
// BitmapFontInfoTests.cs                                                                          *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace SharpFNT.Tests
{
    [TestClass]
    public class BitmapFontInfoTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ReadBinaryWrongBlockSize()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(8);
                    binaryWriter.Write(2);
                    binaryWriter.Write(1);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var binaryReader = new BinaryReader(memoryStream, Encoding.UTF8, true))
                {
                    BitmapFontInfo.ReadBinary(binaryReader);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void WriteBinaryInvalidCharset()
        {
            var bitmapFontInfo = new BitmapFontInfo
            {
                Charset = "This is not a valid charset."
            };

            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    bitmapFontInfo.WriteBinary(binaryWriter);
                }
            }
        }
    }
}