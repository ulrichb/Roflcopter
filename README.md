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

<img src="/Doc/MismatchedFileName.png" alt="MismatchedFileName" width="580" />

Allowed file name postfixes can be configured on the "Mismatched file names" options page.

### "Assertion message is invalid" warning

<img src="/Doc/InvalidAssertionMessage.png" alt="InvalidAssertionMessage" width="673"  />

### Provider for "Copy Fully-qualified name" which returns "short names"

E.g. `<TypeShortName>.<MemberShortName>` (not including the name spaces)

<img src="/Doc/CopyShortNames.png" alt="CopyShortNames" width="552" />
