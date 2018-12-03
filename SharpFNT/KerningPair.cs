// ****************************************************************************
// KerningPair.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

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
            this.First = first;
            this.Second = second;
        }

        public void WriteBinary(BinaryWriter binaryWriter, int amount)
        {
            binaryWriter.Write((uint)this.First);
            binaryWriter.Write((uint)this.Second);
            binaryWriter.Write((short)amount);
        }
        public void WriteXML(XElement element, int amount) 
        {
            element.SetAttributeValue("first", this.First);
            element.SetAttributeValue("second", this.Second);
            element.SetAttributeValue("amount", amount);
        }
        public void WriteText(TextWriter textWriter, int amount)
        {
            TextFormatUtility.WriteInt("first", this.First, textWriter);
            TextFormatUtility.WriteInt("second", this.Second, textWriter);
            TextFormatUtility.WriteInt("amount", amount, textWriter);
        }

        public bool Equals(KerningPair other)
        {
            return this.First == other.First && this.Second == other.Second;
        }
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is KerningPair pair && this.Equals(pair);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.First.GetHashCode() * 397) ^ this.Second.GetHashCode();
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
            return $"{nameof(this.First)}: {this.First}, {nameof(this.Second)}: {this.Second}";
        }

        public static KerningPair ReadBinary(BinaryReader binaryReader, out int amount)
        {
            int first = (int)binaryReader.ReadUInt32();
            int second = (int)binaryReader.ReadUInt32();
            amount = binaryReader.ReadInt16();

            return new KerningPair(first, second);
        }
        public static KerningPair ReadXML(XElement element, out int amount)
        {
            int first = (int)element.Attribute("first");
            int second = (int)element.Attribute("second"); 
            amount = (int)element.Attribute("amount");

            return new KerningPair(first, second); 
        }
        public static KerningPair ReadText(IReadOnlyList<string> lineSegments, out int amount)
        {
            int first = TextFormatUtility.ReadInt("first", lineSegments);
            int second = TextFormatUtility.ReadInt("second", lineSegments);
            amount = TextFormatUtility.ReadInt("amount", lineSegments);

            return new KerningPair(first, second);
        }
    }
}