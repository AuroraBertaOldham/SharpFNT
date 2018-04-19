// ****************************************************************************
// TextFormatUtility.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using System;
using System.Text;

namespace SharpFNT
{
    internal static class TextFormatUtility
    {
        public static string ReadValue(string propertyName, string[] segments) 
        {
            for (int i = 1; i < segments.Length; i++)
            {
                string segment = segments[i];
                int firstEqualsSign = segment.IndexOf('=');
                if (string.Compare(segment, 0, propertyName, 0, firstEqualsSign) == 0)
                {
                    return segment.Remove(0, firstEqualsSign + 1);   
                }
            }

            throw new ArgumentException();
        }
        public static bool ReadBool(string propertyName, string[] segments) 
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
        public static int ReadInt(string propertyName, string[] segments) 
        {
            string value = ReadValue(propertyName, segments);
            return int.Parse(value);
        }
        public static string ReadString(string propertyName, string[] segments) 
        {
            string value = ReadValue(propertyName, segments);
            return value.Substring(1, value.Length - 2);
        }
        public static T ReadEnum<T>(string propertyName, string[] segments) 
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