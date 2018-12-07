// ****************************************************************************
// TextFormatUtilityTests.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

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
            List<string> segments = new List<string>
            {
                "This is not a valid property.", "This is also not a valid property."
            };

            TextFormatUtility.ReadValue("Test", segments);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ReadInvalidBool()
        {
            List<string> segments = new List<string>
            {
                "test=2"
            };

            TextFormatUtility.ReadBool("test", segments);
        }
    }
}