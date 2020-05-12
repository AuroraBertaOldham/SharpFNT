//**************************************************************************************************
// TextFormatUtility.cs                                                                            *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpFNT
{
    internal static class TextFormatUtility
    {
        public static IReadOnlyList<string> GetSegments(string line)
        {
            var ignoreWhiteSpace = false;
            var segments = new List<string>(16);
            var stringBuilder = new StringBuilder(16);

            for (var i = 0; i < line.Length; i++)
            {
                var character = line[i];

                var endSegment = character == ' ' && !ignoreWhiteSpace;

                if (!endSegment)
                {
                    if (character == '\"')
                    {
                        ignoreWhiteSpace = !ignoreWhiteSpace;
                    }
                    else
                    {
                        stringBuilder.Append(character);
                    }
                }

                if ((endSegment || i == line.Length - 1) && stringBuilder.Length > 0)
                {
                    segments.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            }

            return segments;
        }

        public static string ReadValue(string propertyName, IReadOnlyList<string> segments)
        {
            foreach (var segment in segments)
            {
                var equalsSign = segment.IndexOf('=');

                if (equalsSign != propertyName.Length) continue;

                if (string.Compare(segment, 0, propertyName, 0, equalsSign, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return segment.Remove(0, equalsSign + 1);   
                }
            }

            return null;
        }
        public static bool ReadBool(string propertyName, IReadOnlyList<string> segments, bool missingValue = false) 
        {
            var value = ReadValue(propertyName, segments);

            switch (value)
            {
                case null:
                    return missingValue;
                case "1":
                    return true;
                case "0":
                    return false;
                default:
                    // True and false aren't valid but might as well try to use them anyway.
                    return Convert.ToBoolean(value);
            }
        }
        public static int ReadInt(string propertyName, IReadOnlyList<string> segments, int missingValue = 0) 
        {
            var value = ReadValue(propertyName, segments);
            return value != null ? int.Parse(value) : missingValue;
        }
        public static string ReadString(string propertyName, IReadOnlyList<string> segments, string missingValue = null)
        {
            return ReadValue(propertyName, segments) ?? missingValue;
        }
        public static T ReadEnum<T>(string propertyName, IReadOnlyList<string> segments, T missingValue = default) where T : Enum
        {
            var value = ReadValue(propertyName, segments);
            return value != null ? (T)Enum.ToObject(typeof(T), int.Parse(value)) : missingValue;
        }

        public static void WriteValue(string propertyName, string value, TextWriter textWriter)
        {
            textWriter.Write(" {0}={1}", propertyName, value);
        }
        public static void WriteString(string propertyName, string value, TextWriter textWriter) 
        {
            textWriter.Write(" {0}=\"{1}\"", propertyName, value);
        }
        public static void WriteInt(string propertyName, int value, TextWriter textWriter) 
        {
            WriteValue(propertyName, value.ToString(), textWriter);
        }
        public static void WriteBool(string propertyName, bool value, TextWriter textWriter) 
        {
            WriteValue(propertyName, value ? "1" : "0", textWriter);
        }
        public static void WriteEnum<T>(string propertyName, T value, TextWriter textWriter)
        {
            WriteInt(propertyName, Convert.ToInt32(value), textWriter);
        }
    }
}