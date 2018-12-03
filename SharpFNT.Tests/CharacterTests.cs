// ****************************************************************************
// CharacterTests.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SharpFNT.Tests
{
    [TestClass]
    public class CharacterTests
    {
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
    }
}