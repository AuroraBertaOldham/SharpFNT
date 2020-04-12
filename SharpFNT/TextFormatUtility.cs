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

            throw new InvalidDataException("Invalid property name.");
        }
        public static bool ReadBool(string propertyName, IReadOnlyList<string> segments) 
        {
            var value = ReadValue(propertyName, segments);

            if (value == "1") 
            {
                return true;
            }

            if (value == "0") 
            {
                return false;
            }

            throw new FormatException("Invalid boolean format. True should use 1 and false should use 0.");
        }
        public static int ReadInt(string propertyName, IReadOnlyList<string> segments) 
        {
            var value = ReadValue(propertyName, segments);
            return int.Parse(value);
        }
        public static string ReadString(string propertyName, IReadOnlyList<string> segments)
        {
            return ReadValue(propertyName, segments);
        }
        public static T ReadEnum<T>(string propertyName, IReadOnlyList<string> segments) 
        {
            var value = ReadInt(propertyName, segments);
            return (T)Enum.ToObject(typeof(T), value);
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