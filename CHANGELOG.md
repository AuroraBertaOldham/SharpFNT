# Changelog

## 1.1.1
* Updated package links and copyright info.

## 1.1.0
This version made small breaking changes. Future versions now follow semantic versioning.
* Fixed a bug when writing text files without kerning pairs.
* Fixed a bug when writing binary file pages.
* Removed ID property from Character and Amount from KerningPair to avoid confusion when writing files. These are breaking changes.
* Writing text files now writes to the TextWriter directly instead of a StringBuilder. This is a breaking change.
* Other bug fixes and general improvements.

## 1.0.1
* Fixed a bug with null strings.

## 1.0.0
* Initial release.