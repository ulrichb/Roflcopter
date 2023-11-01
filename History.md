### 1.20.0 ###
- ReSharper and Rider 2023.3 support

### 1.19.0 ###
- ReSharper and Rider 2023.2 support
- Add missing 'displayName' for options page in Rider plugin metadata

### 1.18.0 ###
- ReSharper and Rider 2023.1 support

### 1.17.0 ###
- ReSharper and Rider 2022.3 support

### 1.16.0 ###
- ReSharper and Rider 2022.2 support

### 1.15.0 ###
- ReSharper and Rider 2022.1 support

### 1.14.0 ###
- Drop the "Copy Fully-qualified name/ Source browser URI to Clipboard" providers feature because it's now part of R# 2021.3
- ReSharper and Rider 2021.3 support

### 1.13.0 ###
- "Update ASP.NET Designer File" context action + reference error quick fix
- ReSharper and Rider 2021.2 support

### 1.12.0 ###
- Removed the "Write Debugging Control .ini file with disabled Jitter optimizations" feature
- Dropped the GitHub default values for "Copy FQN" (it's now a R# feature)
- ReSharper and Rider 2021.1 support

### 1.11.0 ###
- ReSharper and Rider 2020.3 support

### 1.10.0 ###
- ReSharper and Rider 2020.2 support

### 1.9.0 ###
- ReSharper and Rider 2020.1 support

### 1.8.1 ###
- Fix ZoneMarker for Debugging Control feature to avoid exceptions in InspectCode

### 1.8.0 ###
- ReSharper and Rider 2019.3 support

### 1.7.0 ###
- ReSharper and Rider 2019.2 support

### 1.6.0 ###
- ReSharper and Rider 2019.1 support
- Updated default "allowed postfix regex" for 'MismatchedFileName' warning

### 1.5.0 ###
- ReSharper and Rider 2018.3 support

### 1.4.0 ###
- ReSharper and Rider 2018.2 support

### 1.3.0 ###
- ReSharper and Rider 2018.1 support

### 1.2.0 ###
- Rider 2017.3 support
- Change "Run All Tests in File" short cut to "Ctrl+U, I" because "Ctrl+U, F" is now assigned to "Run Failed Tests"

### 1.1.0 ###
- ReSharper 2017.3 support
- Added "Convert to parameterized test" context action
- Added "Custom source links" to "Copy source browser URI to clipboard" feature

### 1.0.0 ###
- Added limited support for Rider 2017.2 (see the package description for supported features)
- Added warnings for NUnit parameterized tests (+ quick fixes)
- Added quick fix for "Mismatch between type and file name" to fix the file name

### 0.11.0 ###
- Added "Run Unit Tests in File" action
- Added possibility to write a debugging control .ini file in the Modules Debug window of Visual Studio to disable Jitter optimization of specific modules

### 0.10.0 ###
- Added "Allowed file name postfix regex" option for "Mismatch between type and file name" warning

### 0.9.0 ###
- First public release
- Added "Assertion message is invalid" warning
- "Mismatch between type and file name" learned the "interface + class pairs" (pattern: `class SomeType` + `interface ISomeType`)

### 0.8.0 ###
- Added "To-do items count" (see options page)
- Added "Mismatch between type and file name" warning

### 0.7.0 ###
- Added provider for "Copy Fully-qualified name" which returns `<TypeShortName>.<MemberShortName>`
- ReSharper 2017.1 support

### 0.6.0 ###
- ReSharper 2016.3 support

### 0.5.0 ###
- ReSharper 2016.2 support

### 0.4.0 ###
- ReSharper 2016.1 support

### 0.3.0 ###
- ReSharper 10.0 support

### 0.2.0 ###
- ReSharper 9.2 support

### 0.1.0 ###
- Added Live Template Macro which returns the current Git branch
- ReSharper 9.1 support
