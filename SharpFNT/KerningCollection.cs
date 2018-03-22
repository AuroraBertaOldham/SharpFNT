// ****************************************************************************
// KerningCollection.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SharpFNT
{
    public sealed class KerningCollection : ICollection<KerningPair>, IReadOnlyCollection<KerningPair>
    {
        public int Count => _dictionary.Count;

        bool ICollection<KerningPair>.IsReadOnly => false;

        private readonly Dictionary<KerningPair, int> _dictionary;

        public KerningCollection(int capacity)
        {
            _dictionary = new Dictionary<KerningPair, int>(capacity);
        }

        public void WriteBinary(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(_dictionary.Count * KerningPair.SizeInBytes);

            foreach (KerningPair kerningPair in _dictionary.Keys)
            {
                kerningPair.WriteBinary(binaryWriter);
            }
        }

        void ICollection<KerningPair>.Add(KerningPair kerningPair) => this.SetAmount(kerningPair);

        public void SetAmount(KerningPair kerningPair)
        {
            _dictionary[kerningPair] = kerningPair.Amount;
        }
        public void SetAmount(char left, char right, int amount)
        {
            this.SetAmount(new KerningPair(left, right, amount));
        }

        public int GetAmount(char left, char right)
        {
            _dictionary.TryGetValue(new KerningPair(left, right, 0), out int kerningValue);
            return kerningValue;
        }

        public bool Remove(KerningPair kerningPair)
        {
            return _dictionary.Remove(kerningPair);
        }
        public bool Remove(char left, char right)
        {
            return this.Remove(new KerningPair(left, right, 0));
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public void CopyTo(KerningPair[] array, int arrayIndex)
        {
            int count = arrayIndex;
            foreach (KerningPair kerningPair in _dictionary.Keys)
            {
                array[count] = kerningPair;
                count++;
            }
        }

        public bool Contains(KerningPair kerningPair)
        {
            return _dictionary.ContainsKey(kerningPair);
        }
        public bool Contains(char left, char right)
        {
            return this.Contains(new KerningPair(left, right, 0));
        }

        public IEnumerator<KerningPair> GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public static KerningCollection ReadBinary(BinaryReader binaryReader)
        {
            int kerningBlockSize = binaryReader.ReadInt32();

            if (kerningBlockSize % KerningPair.SizeInBytes != 0)
            {
                throw new InvalidDataException("Invalid kerning block size.");
            }

            int kerningCount = kerningBlockSize / KerningPair.SizeInBytes;

            KerningCollection kerningCollection = new KerningCollection(kerningCount);

            for (int i = 0; i < kerningCount; i++)
            {
                KerningPair kerningPair = KerningPair.ReadBinary(binaryReader);
                kerningCollection.SetAmount(kerningPair);
            }

            return kerningCollection;
        }
    }
}