//**************************************************************************************************
// KerningPair.cs                                                                                  *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace SharpFNT
{
    public struct KerningPair : IEquatable<KerningPair>
    {
        public const int SizeInBytes = 10;

        public int First { get; }

        public int Second { get; }

        public KerningPair(int first, int second)
        {
            First = first;
            Second = second;
        }

        public void WriteBinary(BinaryWriter binaryWriter, int amount)
        {
            binaryWriter.Write((uint)First);
            binaryWriter.Write((uint)Second);
            binaryWriter.Write((short)amount);
        }
        public void WriteXML(XElement element, int amount) 
        {
            element.SetAttributeValue("first", First);
            element.SetAttributeValue("second", Second);
            element.SetAttributeValue("amount", amount);
        }
        public void WriteText(TextWriter textWriter, int amount)
        {
            TextFormatUtility.WriteInt("first", First, textWriter);
            TextFormatUtility.WriteInt("second", Second, textWriter);
            TextFormatUtility.WriteInt("amount", amount, textWriter);
        }

        public bool Equals(KerningPair other)
        {
            return First == other.First && Second == other.Second;
        }
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is KerningPair pair && Equals(pair);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (First.GetHashCode() * 397) ^ Second.GetHashCode();
            }
        }
        public static bool operator ==(KerningPair left, KerningPair right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(KerningPair left, KerningPair right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{nameof(First)}: {First}, {nameof(Second)}: {Second}";
        }

        public static KerningPair ReadBinary(BinaryReader binaryReader, out int amount)
        {
            var first = (int)binaryReader.ReadUInt32();
            var second = (int)binaryReader.ReadUInt32();
            amount = binaryReader.ReadInt16();

            return new KerningPair(first, second);
        }
        public static KerningPair ReadXML(XElement element, out int amount)
        {
            var first = (int?)element.Attribute("first") ?? 0;
            var second = (int?)element.Attribute("second") ?? 0; 
            amount = (int?)element.Attribute("amount") ?? 0;

            return new KerningPair(first, second); 
        }
        public static KerningPair ReadText(IReadOnlyList<string> lineSegments, out int amount)
        {
            var first = TextFormatUtility.ReadInt("first", lineSegments);
            var second = TextFormatUtility.ReadInt("second", lineSegments);
            amount = TextFormatUtility.ReadInt("amount", lineSegments);

            return new KerningPair(first, second);
        }
    }
}