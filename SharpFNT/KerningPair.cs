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

        public int Left { get; }
        public int Right { get; }
        public int Amount { get; }

        public KerningPair(int left, int right, int amount)
        {
            this.Left = left;
            this.Right = right;
            this.Amount = amount;
        }

        public void WriteBinary(BinaryWriter binaryWriter)
        {
            binaryWriter.Write((uint)this.Left);
            binaryWriter.Write((uint)this.Right);
            binaryWriter.Write((short)this.Amount);
        }

        public void WriteXML(XElement element) 
        {
            element.SetAttributeValue("first", this.Left);
            element.SetAttributeValue("second", this.Right);
            element.SetAttributeValue("amount", this.Amount);
        }

        public void WriteText(StringBuilder stringBuilder)
        {
            TextFormatUtility.WriteInt("first", this.Left, stringBuilder);
            TextFormatUtility.WriteInt("right", this.Right, stringBuilder);
            TextFormatUtility.WriteInt("amount", this.Amount, stringBuilder);
        }

        public bool Equals(KerningPair other)
        {
            return this.Left == other.Left && this.Right == other.Right;
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
                return (this.Left.GetHashCode() * 397) ^ this.Right.GetHashCode();
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
            return $"{nameof(this.Left)}: {this.Left}, {nameof(this.Right)}: {this.Right}, {nameof(this.Amount)}: {this.Amount}";
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