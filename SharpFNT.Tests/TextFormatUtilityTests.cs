//**************************************************************************************************
// TextFormatUtilityTests.cs                                                                       *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpFNT.Tests
{
    [TestClass]
    public class TextFormatUtilityTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ReadInvalidValue()
        {
            var segments = new List<string>
            {
                "This is not a valid property.", "This is also not a valid property."
            };

            TextFormatUtility.ReadValue("Test", segments);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ReadInvalidBool()
        {
            var segments = new List<string>
            {
                "test=2"
            };

            TextFormatUtility.ReadBool("test", segments);
        }
    }
}