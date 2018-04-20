// ****************************************************************************
// KerningPair.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace SharpFNT
{
    public struct KerningPair : IEquatable<KerningPair>
    {
        public const int SizeInBytes = 10;

        public int First { get; }
        public int Second { get; }
        public int Amount { get; }

        public KerningPair(int first, int second, int amount)
        {
            this.First = first;
            this.Second = second;
            this.Amount = amount;
        }

        public void WriteBinary(BinaryWriter binaryWriter)
        {
            binaryWriter.Write((uint)this.First);
            binaryWriter.Write((uint)this.Second);
            binaryWriter.Write((short)this.Amount);
        }
        public void WriteXML(XElement element) 
        {
            element.SetAttributeValue("first", this.First);
            element.SetAttributeValue("second", this.Second);
            element.SetAttributeValue("amount", this.Amount);
        }
        public void WriteText(StringBuilder stringBuilder)
        {
            TextFormatUtility.WriteInt("first", this.First, stringBuilder);
            TextFormatUtility.WriteInt("second", this.Second, stringBuilder);
            TextFormatUtility.WriteInt("amount", this.Amount, stringBuilder);
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
            return $"{nameof(this.First)}: {this.First}, {nameof(this.Second)}: {this.Second}, {nameof(this.Amount)}: {this.Amount}";
        }

        public static KerningPair ReadBinary(BinaryReader binaryReader)
        {
            int left = (int)binaryReader.ReadUInt32();
            int right = (int)binaryReader.ReadUInt32();
            short amount = binaryReader.ReadInt16();
            return new KerningPair(left, right, amount);
        }
        public static KerningPair ReadXML(XElement element)
        {
            int left = (int)element.Attribute("first");
            int right = (int)element.Attribute("second"); 
            int amount = (int)element.Attribute("amount");

            return new KerningPair(left, right, amount); 
        }
        public static KerningPair ReadText(string[] lineSegments)
        {
            int left = TextFormatUtility.ReadInt("first", lineSegments);
            int right = TextFormatUtility.ReadInt("second", lineSegments);
            int amount = TextFormatUtility.ReadInt("amount", lineSegments);

            return new KerningPair(left, right, amount);
        }
    }
}