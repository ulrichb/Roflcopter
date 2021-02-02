# ReSharper Roflcopter

_Roflcopter_ (noun) 1. a breeding ground for new ReSharper extensions 2. a collection of small, handy ReSharper utilities

[![Build status](https://teamcity.jetbrains.com/app/rest/builds/buildType:(id:OpenSourceProjects_Roflcopter_Ci),branch:master,running:any/statusIcon.svg)](https://teamcity.jetbrains.com/viewType.html?buildTypeId=OpenSourceProjects_Roflcopter_Ci)
<a href="https://teamcity.jetbrains.com/viewType.html?buildTypeId=OpenSourceProjects_Roflcopter_Ci&branch=master"><img src="https://dl.dropbox.com/s/t4kzgq58t4cfi1z/master-linecoverage.svg" alt="Line Coverage" title="Line Coverage"></a>
<a href="https://teamcity.jetbrains.com/viewType.html?buildTypeId=OpenSourceProjects_Roflcopter_Ci&branch=master"><img src="https://dl.dropbox.com/s/unjpcx06rztot8k/master-branchcoverage.svg" alt="Branch Coverage" title="Branch Coverage"></a>

[ReSharper Gallery Page](https://resharper-plugins.jetbrains.com/packages/ReSharper.Roflcopter/)

[History of changes](History.md)

## Features

### Git Branch Name Live Template Macro

A [Live Template Macro](https://www.jetbrains.com/help/resharper/Templates__Template_Basics__Template_Macros.html) (Name: "Current branch name, stripped by an optional regex") which returns the current Git branch (read from the Git HEAD file).

This macro can be used to create templates like `// TODO UB $BRANCH$: $END$`.

Roflcopter also provides default Live Template, just type `branch` and hit TAB.

### "To-do items count" in To-do Explorer window

<img src="/Doc/TodoItemsCount.png" alt="TodoItemsCount" />

### "Mismatch between type and file name" warning

<img src="/Doc/MismatchedFileName.png" alt="MismatchedFileName" width="657" />

<img src="/Doc/MismatchedFileNameQuickFix.png" alt="MismatchedFileNameQuickFix" width="410" />

Allowed file name postfixes can be configured in _Options | Code Inspection | Mismatched file names_.

### Parameterized test support for NUnit tests

#### Warnings for incorrect test parameters/attributes

<img src="/Doc/ParameterizedTestHighlightings.png" alt="ParameterizedTestHighlightings" width="675" />

<img src="/Doc/ParameterizedTestHighlightingsQuickFixes.gif" alt="ParameterizedTestHighlightingsQuickFixes" width="623" />

#### "Convert to parameterized test" context action

<img src="/Doc/ConvertToParameterizedTestContextAction.gif" alt="ConvertToParameterizedTestContextAction" width="623" />

### "Assertion message is invalid" warning

<img src="/Doc/InvalidAssertionMessage.png" alt="InvalidAssertionMessage" width="673"  />

### "Run All Tests in File" action

<img src="/Doc/UnitTestRunFileAction.png" alt="UnitTestRunFileAction" width="490"  />

To apply the short cut (Ctrl+R, F) executing _Apply Scheme_ in the [_Environment & Menus_](https://www.jetbrains.com/help/resharper/Reference__Options__Environment__Visual_Studio_Integration.html) options may be necessary.

### Additional "Copy Fully-qualified name/ Source browser URI to Clipboard" providers

* "Short names" (e.g. `<TypeShortName>.<MemberName>`)
* Custom source URLs (including Git repository support)

The providers can be configured in _Options | Search & Navigation | Copy names to clipboard_.

<img src="/Doc/CopyToClipboard.gif" alt="CopyToClipboard" width="804" />
