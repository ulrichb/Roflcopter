[CmdletBinding()]
Param(
  [Parameter()] [string] $NugetExecutable = "Shared\.nuget\nuget.exe",
  [Parameter()] [string] $Configuration = "Debug",
  [Parameter()] [string] $Version = "0.0.1.0-dev",
#  [Parameter()] [string] $BranchName,
#  [Parameter()] [string] $CoverageBadgeUploadToken,
  [Parameter()] [string] $NugetPushKey
)

Set-StrictMode -Version 2.0; $ErrorActionPreference = "Stop"; $ConfirmPreference = "None"

. Shared\Build\BuildFunctions

$BuildOutputPath = "Build\Output"
$SolutionFilePath = "Roflcopter.sln"
$AssemblyVersionFilePath = "Src\Roflcopter.Plugin\Properties\AssemblyInfo.cs"
$MSBuildPath = "${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe"
# $NUnitExecutable = "nunit-console-x86.exe"
# $NUnitTestAssemblyPaths = @(
#   "Src\Roflcopter.Plugin.Tests\bin.R82\$Configuration\Roflcopter.Plugin.Tests.dll"
# )
# $NUnitFrameworkVersion = "net-4.5"
# $TestCoverageFilter = "+[Roflcopter*]* -[Roflcopter*]ReSharperExtensionsShared.*"
$NuspecPath = "Src\Roflcopter.nuspec"
$NugetPackProperties = @(
    "Version=$(CalcNuGetPackageVersion 20162);Configuration=$Configuration;DependencyVer=[6.0];BinDirInclude=bin"
)
$NugetPushServer = "https://www.myget.org/F/ulrichb/api/v2/package"

Clean
PackageRestore
Build
# Test
NugetPack

if ($NugetPushKey) {
    NugetPush
}
