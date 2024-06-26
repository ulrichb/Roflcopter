[CmdletBinding()]
Param(
  [Parameter()] [string] $Configuration = "Debug",
  [Parameter()] [string] $Version = "0.0.0.1",
  [Parameter()] [string] $BranchName,
  [Parameter()] [boolean] $BuildRiderPlugin = $true,
  [Parameter()] [boolean] $RunTests = $true,
  [Parameter()] [string] $CoverageBadgeUploadToken,
  [Parameter()] [string] $NugetPushKey
)

Set-StrictMode -Version 2.0; $ErrorActionPreference = "Stop"; $ConfirmPreference = "None"
trap { $error[0] | Format-List -Force }

. Shared\Build\BuildFunctions

$BuildOutputPath = "Build\Output"
$SolutionFilePath = "Roflcopter.sln"

$NugetPackProjects = gci "Src\Roflcopter.Plugin\Roflcopter.Plugin.RS*.csproj"
$RiderPluginProject = "Src\RiderPlugin"
$NugetPushServer = "https://www.myget.org/F/ulrichb/api/v2/package"

AppendToGitHubStepSummary "Full Version: $(GetFullVersion)"
Clean
PackageRestore
Build
NugetPack
if ($BuildRiderPlugin) { BuildRiderPlugin }
if ($RunTests) { Test }
if ($NugetPushKey) { NugetPush }
