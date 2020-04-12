//**************************************************************************************************
// KerningTests.cs                                                                                 *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpFNT.Tests
{
    [TestClass]
    public class KerningPairTests
    {
        [TestMethod]
        public void KerningPairToString()
        {
            var kerningPair = new KerningPair(5, 2);
            Assert.AreEqual("First: 5, Second: 2", kerningPair.ToString());
        }

        [TestMethod]
        public void KerningPairEqualOp()
        {
            var one = new KerningPair(6, 4);
            var two = new KerningPair(6, 4);
            Assert.IsTrue(one == two);
        }

        [TestMethod]
        public void KerningPairInequalityOp()
        {
            var one = new KerningPair(6, 4);
            var two = new KerningPair(7, 3);
            Assert.IsTrue(one != two);
        }

        [TestMethod]
        public void KerningPairEqualNotKerningPair()
        {
            var one = new object();
            var two = new KerningPair(2, 4);
            Assert.IsFalse(two.Equals(one));
        }

        [TestMethod]
        public void KerningPairEqualNull()
        {
            var two = new KerningPair(2, 4);
            Assert.IsFalse(two.Equals(null));
        }
    }
}