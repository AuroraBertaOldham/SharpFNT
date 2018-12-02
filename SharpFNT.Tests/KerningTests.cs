// ****************************************************************************
// KerningTests.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SharpFNT.Tests
{
    [TestClass]
    public class KerningTests
    {
        [TestMethod]
        public void GetKerning()
        {
            BitmapFont bitmapFont = new BitmapFont
            {
                KerningPairs = new Dictionary<KerningPair, int> {{new KerningPair(2, 6), 5}}
            };

            int kerningAmount = bitmapFont.GetKerningAmount((char) 2, (char) 6);
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
    }
}