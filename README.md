# SharpFNT

## Introduction
SharpFNT is a pure C# parser for the Angel Code Bitmap Font file. SharpFNT is built for .Net Standard 1.3 and .Net Framework 1.1 making it highly portable.

## Usage
``` csharp
using SharpFNT;

BitmapFont bitmapFont = BitmapFont.FromFile("Example.fnt"); 
```

## Binaries
The simplest way to obtain SharpFNT is through [Nuget](https://www.nuget.org). Binaries are also available under releases.

## Possible Future Additions
1. An optional library for creating the character rectangles for rendering and measuring.
1. Support for pre version 3 of the format.
1. Additional target frameworks.

## License
Licensed under [MIT](LICENSE).
