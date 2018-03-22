// ****************************************************************************
// CharacterCollection.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SharpFNT
{
    public sealed class CharacterCollection : ICollection<Character>, IReadOnlyCollection<Character>
    {
        public int Count => _dictionary.Count;

        bool ICollection<Character>.IsReadOnly => false;

        private readonly Dictionary<uint, Character> _dictionary;

        public CharacterCollection(int capacity) 
        {
            _dictionary = new Dictionary<uint, Character>(capacity);
        }

        public void WriteBinary(BinaryWriter binaryWriter)
        {
            int sizeInBytes = _dictionary.Values.Count * Character.SizeInBytes;
            binaryWriter.Write(sizeInBytes);

            foreach (Character character in _dictionary.Values)
            {
                character.WriteBinary(binaryWriter);
            }
        }

        void ICollection<Character>.Add(Character character)
        {
            this.Set(character);
        }

        public void Set(Character character)
        {
            _dictionary[character.Char] = character;
        }

        public Character Get(char character)
        {
            return _dictionary[character];
        }

        public bool Remove(Character character)
        {
            return _dictionary.Remove(character.Char);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public void CopyTo(Character[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (Character character in _dictionary.Values)
            {
                if (index >= array.Length) return;
                array[index] = character;
                index++;
            }
        }

        public bool Contains(Character character)
        {
            return _dictionary.ContainsValue(character);
        }

        public IEnumerator<Character> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public static CharacterCollection ReadBinary(BinaryReader binaryReader)
        {
            int characterBlockSize = binaryReader.ReadInt32();

            if (characterBlockSize % Character.SizeInBytes != 0)
            {
                throw new InvalidDataException("Invalid character block size.");
            }

            int characterCount = characterBlockSize / Character.SizeInBytes;

            CharacterCollection characterCollection = new CharacterCollection(characterCount);

            for (int i = 0; i < characterCount; i++)
            {
                Character character = Character.ReadBinary(binaryReader);
                characterCollection.Set(character);
            }

            return characterCollection;
        }
    }
}