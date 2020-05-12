//**************************************************************************************************
// TextFormatUtilityTests.cs                                                                       *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SharpFNT.Tests
{
    [TestClass]
    public class TextFormatUtilityTests
    {
        [TestMethod]
        public void ReadInvalidValue()
        {
            var segments = new List<string>
            {
                "This is not a valid property.", "This is also not a valid property."
            };

            Assert.AreEqual(null, TextFormatUtility.ReadValue("Test", segments));
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ReadInvalidBool()
        {
            var segments = new List<string>
            {
                "Test=2"
            };

            TextFormatUtility.ReadBool("Test", segments);
        }

        [TestMethod]
        public void ReadMissingBool()
        {
            var segments = new List<string>();
            Assert.AreEqual(false, TextFormatUtility.ReadBool("Test", segments));
        }
    }
}