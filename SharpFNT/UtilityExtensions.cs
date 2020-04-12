//**************************************************************************************************
// UtilityExtensions.cs                                                                            *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

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
            if (index < 0 || index > 7) throw new ArgumentOutOfRangeException(nameof(index));

            if (set)
            {
                return (byte)(@byte | (1 << index));
            }

            return (byte)(@byte & ~(1 << index));
        }

        public static string ReadNullTerminatedString(this BinaryReader binaryReader)
        {
            var stringBuilder = new StringBuilder();

            while (true)
            {
                var character = binaryReader.ReadByte();

                if (character == 0)
                {
                    break;
                }

                stringBuilder.Append((char)character);
            }

            return stringBuilder.ToString();
        }

        public static void WriteNullTerminatedString(this BinaryWriter binaryWriter, string value)
        {
            if (value != null)
            {
                foreach (var character in value)
                {
                    binaryWriter.Write((byte)character);
                }
            }

            binaryWriter.Write((byte)0);
        }

        public static T GetEnumValue<T>(this XAttribute xAttribute) 
        {
            return (T)Enum.Parse(typeof(T), xAttribute.Value);
        }
    }
}