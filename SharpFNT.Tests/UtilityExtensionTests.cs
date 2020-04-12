//**************************************************************************************************
// UtilityTests.cs                                                                                 *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SharpFNT.Tests
{
    [TestClass]
    public class UtilityExtensionTests
    {
        [TestMethod]
        public void ReadBit()
        {
            const byte testNumber = 0b0010000;
            Assert.IsTrue(testNumber.IsBitSet(4));
        }

        [TestMethod]
        public void WriteBitTrue()
        {
            const byte expected = 0b0100;
            byte value = 0;
            value = value.SetBit(2, true);
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public void WriteBitFalse()
        {
            const byte expected = 0b01111111;
            byte value = 0b11111111;
            value = value.SetBit(7, false);
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WriteBitOutOfRange()
        {
            const byte value = 0;
            value.SetBit(int.MaxValue, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ReadBitOutOfRange()
        {
            const byte value = 0;
            value.IsBitSet(int.MaxValue);
        }
    }
}