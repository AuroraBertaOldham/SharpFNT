// ****************************************************************************
// TextFormatUtility.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFNT
{
    internal static class TextFormatUtility
    {
        public static IReadOnlyList<string> GetSegments(string line)
        {
            bool ignoreWhiteSpace = false;
            List<string> segments = new List<string>(16);
            StringBuilder stringBuilder = new StringBuilder(16);

            for (int i = 0; i < line.Length; i++)
            {
                char character = line[i];

                bool endSegment = character == ' ' && !ignoreWhiteSpace;

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
            for (int i = 1; i < segments.Count; i++)
            {
                string segment = segments[i];
                int firstEqualsSign = segment.IndexOf('=');
                if (string.Compare(segment, 0, propertyName, 0, propertyName.Length, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return segment.Remove(0, firstEqualsSign + 1);   
                }
            }

            throw new ArgumentException();
        }
        public static bool ReadBool(string propertyName, IReadOnlyList<string> segments) 
        {
            string value = ReadValue(propertyName, segments);

            if (value == "1") 
            {
                return true;
            }

            if (value == "0") 
            {
                return false;
            }

            throw new FormatException();
        }
        public static int ReadInt(string propertyName, IReadOnlyList<string> segments) 
        {
            string value = ReadValue(propertyName, segments);
            return int.Parse(value);
        }
        public static string ReadString(string propertyName, IReadOnlyList<string> segments)
        {
            return ReadValue(propertyName, segments);
        }
        public static T ReadEnum<T>(string propertyName, IReadOnlyList<string> segments) 
        {
            int value = ReadInt(propertyName, segments);
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static void WriteValue(string propertyName, string value, StringBuilder stringBuilder)
        {
            stringBuilder.AppendFormat(" {0}={1}", propertyName, value);
        }
        public static void WriteString(string propertyName, string value, StringBuilder stringBuilder) 
        {
            stringBuilder.AppendFormat(" {0}=\"{1}\"", propertyName, value);
        }
        public static void WriteInt(string propertyName, int value, StringBuilder stringBuilder) 
        {
            WriteValue(propertyName, value.ToString(), stringBuilder);
        }
        public static void WriteBool(string propertyName, bool value, StringBuilder stringBuilder) 
        {
            WriteValue(propertyName, value ? "1" : "0", stringBuilder);
        }
        public static void WriteEnum<T>(string propertyName, T value, StringBuilder stringBuilder)
        {
            WriteInt(propertyName, Convert.ToInt32(value), stringBuilder);
        }
    }
}