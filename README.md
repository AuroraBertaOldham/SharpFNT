# SharpFNT

## Introduction
SharpFNT is a C# library for reading and writing [AngelCode bitmap fonts (.fnt)](http://www.angelcode.com/products/bmfont/) in binary, XML, and text. It is built for .NET Standard 1.3 or higher. I have a command line application for converting and inspecting AngelCode bitmap fonts available [here](https://github.com/AuroraBertaOldham/FNTTools).

## Example
The following loads a bitmap font from a file, outputs the name of the font, changes the font name, and then saves it as a new binary bitmap font.
```csharp
using SharpFNT;

BitmapFont bitmapFont = BitmapFont.FromFile("ExampleFont.fnt");

Console.WriteLine(bitmapFont.Info.Face);

bitmapFont.Info.Face = "New Name";

bitmapFont.Save("ExampleFont2.fnt", FormatHint.Binary);
```

## Documentation
See the [documentation for BMFont](http://www.angelcode.com/products/bmfont/documentation.html) for information on rendering text and the properties of the file format.

## Download
SharpFNT can be downloaded on [NuGet](https://www.nuget.org/packages/SharpFNT/) or from the releases section.

## Changelog
Available [here](CHANGELOG.md).

## License
Licensed under [MIT](LICENSE).
