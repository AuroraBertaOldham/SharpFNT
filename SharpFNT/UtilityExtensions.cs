// ****************************************************************************
// Utility.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace SharpFNT
{
    internal static class UtilityExtensions
    {
        public static bool IsBitSet(this byte @byte, int index)
        {
            if (index < 0 || index > 7) throw new ArgumentOutOfRangeException(nameof(index));
            return (@byte & (1 << index)) != 0;
        }
        public static byte SetBit(this byte @byte, int index, bool set)
        {
            if (set)
            {
                return (byte)(@byte | (1 << index));
            }

            return (byte)(@byte & ~(1 << index));
        }

        public static string ReadCString(this BinaryReader binaryReader)
        {
            StringBuilder stringBuilder = new StringBuilder();

            while (true)
            {
                byte character = binaryReader.ReadByte();

                if (character == 0)
                {
                    break;
                }

                stringBuilder.Append((char)character);
            }

            return stringBuilder.ToString();
        }
        public static void Write(this BinaryWriter binaryWriter, string @string, bool asCString)
        {
            if (asCString)
            {
                foreach (char character in @string)
                {
                    binaryWriter.Write((byte)character);
                }

                binaryWriter.Write((byte)0);
            }
            else
            {
                binaryWriter.Write(@string);
            }
        }

        public static T GetEnumValue<T>(this XAttribute xAttribute) 
        {
            return (T)Enum.Parse(typeof(T), xAttribute.Value);
        }
    }
}