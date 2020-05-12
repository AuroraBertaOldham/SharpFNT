# Changelog

## 2.0.0
* Missing properties are now set to default values instead of throwing an exception in the XML and text formats ([I #3](https://github.com/AuroraBertaOldham/SharpFNT/issues/3)). This improves compatibility with alternative bitmap font generation programs that leave out some properties. This could be a breaking change in certain circumstances.
* Changed initial list capacity when properties such as count, or page are missing from 32 to 0.
* The words "true" and "false" are now valid for Boolean properties in the text format.

## 1.1.2
* Fixed a bug when writing info block size for the binary format. Thanks to [Michael Belyaev (@usr-sse2)](https://github.com/usr-sse2) for the contribution ([PR #2](https://github.com/AuroraBertaOldham/SharpFNT/pull/2)).

## 1.1.1
* Corrected package links and copyright info.

## 1.1.0
**This version made small breaking changes.** Future versions now use [Semantic Versioning](https://semver.org/).
* Fixed a bug when writing text files without kerning pairs.
* Fixed a bug when writing binary file pages.
* Removed ID property from Character and Amount from KerningPair to avoid confusion when writing files. These are breaking changes.
* Writing text files now writes to the TextWriter directly instead of a StringBuilder. This is a breaking change.
* Other bug fixes and general improvements.

## 1.0.1
* Fixed a bug with null strings.

## 1.0.0
* Initial release.