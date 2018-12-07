// ****************************************************************************
// KerningTests.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpFNT.Tests
{
    [TestClass]
    public class KerningPairTests
    {
        [TestMethod]
        public void KerningPairToString()
        {
            KerningPair kerningPair = new KerningPair(5, 2);
            Assert.AreEqual("First: 5, Second: 2", kerningPair.ToString());
        }

        [TestMethod]
        public void KerningPairEqualOp()
        {
            KerningPair one = new KerningPair(6, 4);
            KerningPair two = new KerningPair(6, 4);
            Assert.IsTrue(one == two);
        }

        [TestMethod]
        public void KerningPairInequalityOp()
        {
            KerningPair one = new KerningPair(6, 4);
            KerningPair two = new KerningPair(7, 3);
            Assert.IsTrue(one != two);
        }

        [TestMethod]
        public void KerningPairEqualNotKerningPair()
        {
            object one = new object();
            KerningPair two = new KerningPair(2, 4);
            Assert.IsFalse(two.Equals(one));
        }

        [TestMethod]
        public void KerningPairEqualNull()
        {
            KerningPair two = new KerningPair(2, 4);
            Assert.IsFalse(two.Equals(null));
        }
    }
}