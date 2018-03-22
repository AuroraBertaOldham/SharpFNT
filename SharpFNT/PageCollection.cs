// ****************************************************************************
// PageCollection.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpFNT
{
    public sealed class PageCollection : ICollection<string>, IReadOnlyCollection<string>
    {
        public int Count => _list.Count;

        bool ICollection<string>.IsReadOnly => false;

        private readonly List<string> _list;

        public PageCollection(int capacity)
        {
            _list = new List<string>(capacity);
        }

        public void WriteBinary(BinaryWriter binaryWriter)
        {
            int totalSize = _list.Sum(page => page.Length + 1);
            binaryWriter.Write(totalSize);

            foreach (string page in _list)
            {
                binaryWriter.Write(page, true);
            }
        }

        public void Add(string page)
        {
            _list.Add(page);
        }

        public bool Remove(string page)
        {
            return _list.Remove(page);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Contains(string page)
        {
            return _list.Contains(page);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public static PageCollection ReadBinary(BinaryReader binaryReader, int pageCount)
        {
            binaryReader.ReadInt32();

            PageCollection pageCollection = new PageCollection(pageCount);

            for (int i = 0; i < pageCount; i++)
            {
                pageCollection.Add(binaryReader.ReadCString());
            }

            return pageCollection;
        }
    }
}