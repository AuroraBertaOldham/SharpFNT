// ****************************************************************************
// Program.cs
// Copyright 2018 Todd Berta-Oldham
// This code is licensed under MIT.
// ****************************************************************************

using SharpFNT;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TestApp
{
    public static class Program
    {
        private static void Main()
        {
            Console.WriteLine("SharpFNT Test App");
            Console.WriteLine("This program can display information from three different bitmap fonts.");

            BitmapFont binaryFont = BitmapFont.FromFile("Binary.fnt");
            BitmapFont xmlFont = BitmapFont.FromFile("XML.fnt");
            BitmapFont textFont = BitmapFont.FromFile("Text.fnt");

            while (true)
            {
                Console.WriteLine("Press [A] for binary font, [S] for XML font, [D] for text font, or any other key to exit.");

                BitmapFont bitmapFont;

                ConsoleKey consoleKey = Console.ReadKey().Key;
                Console.WriteLine();

                switch (consoleKey)
                {
                    case ConsoleKey.A:
                        bitmapFont = binaryFont;
                        break;
                    case ConsoleKey.S:
                        bitmapFont = xmlFont;
                        break;
                    case ConsoleKey.D:
                        bitmapFont = textFont;
                        break;
                    default:
                        return;
                }

                Console.WriteLine("Press [A] for all, [S] for info, [D] for common, [F] for pages, [G] for characters, [H] for kerning pairs, or any other key to exit.");

                consoleKey = Console.ReadKey().Key;
                Console.WriteLine();

                switch (consoleKey)
                {
                    case ConsoleKey.A:
                        PrintInfo(bitmapFont);
                        PrintCommon(bitmapFont);
                        PrintPages(bitmapFont);
                        PrintCharacters(bitmapFont);
                        PrintKerningPairs(bitmapFont);
                        break;
                    case ConsoleKey.S:
                        PrintInfo(bitmapFont);
                        break;
                    case ConsoleKey.D:
                        PrintCommon(bitmapFont);
                        break;
                    case ConsoleKey.F:
                        PrintPages(bitmapFont);
                        break;
                    case ConsoleKey.G:
                        PrintCharacters(bitmapFont);
                        break;
                    case ConsoleKey.H:
                        PrintKerningPairs(bitmapFont);
                        break;
                    default:
                        return;
                }
            }
        }

        private static void PrintInfo(BitmapFont bitmapFont)
        {
            if (bitmapFont.Info == null)
            {
                Console.WriteLine("No info block.");
                return;
            }

            PrintPropertyValues(bitmapFont.Info);
        }
        private static void PrintCommon(BitmapFont bitmapFont)
        {
            if (bitmapFont.Common == null)
            {
                Console.WriteLine("No common block.");
                return;
            }

            PrintPropertyValues(bitmapFont.Common);
        }
        private static void PrintPages(BitmapFont bitmapFont)
        {
            if (bitmapFont.Pages == null)
            {
                Console.WriteLine("No pages.");
                return;
            }

            Console.WriteLine("Pages Count: {0}", bitmapFont.Pages.Count);
            foreach (KeyValuePair<int, string> page in bitmapFont.Pages)
            {
                Console.WriteLine("ID: {0}", page.Key);
                Console.WriteLine("File: {0}", page.Value);
            }
        }
        private static void PrintCharacters(BitmapFont bitmapFont)
        {
            if (bitmapFont.Characters == null)
            {
                Console.WriteLine("No characters.");
                return;
            }

            Console.WriteLine("Characters Count: {0}", bitmapFont.Characters.Count);
            foreach (KeyValuePair<int, Character> character in bitmapFont.Characters)
            {
                PrintPropertyValues(character.Value);
            }
        }
        private static void PrintKerningPairs(BitmapFont bitmapFont)
        {
            if (bitmapFont.KerningPairs == null)
            {
                Console.WriteLine("No kerning pairs.");
                return;
            }

            Console.WriteLine("Kerning Pairs Count: {0}", bitmapFont.KerningPairs.Count);
            foreach (KeyValuePair<KerningPair, int> kerningPair in bitmapFont.KerningPairs)
            {
                PrintPropertyValues(kerningPair.Key);
            }
        }

        private static void PrintPropertyValues(object @object)
        {
            Type type = @object.GetType();
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                Console.WriteLine("{0}: {1}", propertyInfo.Name, propertyInfo.GetValue(@object));
            }
        }
    }
}
