<#
.SYNOPSIS
    Tests, publishes and packages BG Password Generator into a Setup.exe and a portable zip.

.EXAMPLE
    ./build.ps1
    ./build.ps1 -Runtimes win-x64, win-x86, win-arm64
    ./build.ps1 -Runtimes win-x64, win-x86, win-arm64 -Publish -NotesFile notes.md

.NOTES
    Requires the .NET 10 SDK and the Velopack CLI matching the Velopack package version:
    dotnet tool install -g vpk --version 1.2.158
    -Publish also needs the GitHub CLI (gh), signed in to an account that can create releases.
    Output goes to Releases/<runtime>/. Keep that folder between versions: vpk builds small delta
    updates from the previous version's package when it finds one there.
#>
param(
    [string[]] $Runtimes = @("win-x64"),
    # Create a GitHub release named after the version with the setup, portable and update files
    [switch] $Publish,
    [string] $NotesFile
)

$ErrorActionPreference = "Stop"
# powershell -File passes "a,b,c" as one string; accept that as well as a real array
$Runtimes = $Runtimes -split "," | ForEach-Object { $_.Trim() } | Where-Object { $_ }
$root = $PSScriptRoot
$project = Join-Path $root "src/BgPasswordGenerator/BgPasswordGenerator.csproj"

function Invoke-Step([string] $name, [scriptblock] $command) {
    Write-Host "==> $name" -ForegroundColor Cyan
    & $command
    if ($LASTEXITCODE -ne 0) { throw "$name failed with exit code $LASTEXITCODE" }
}

# The csproj <Version> is the single source of truth for the release version
$version = (dotnet msbuild $project -getProperty:Version).Trim()
if ($Publish -and -not ($NotesFile -and (Test-Path $NotesFile))) { throw "-Publish needs -NotesFile pointing to the release notes" }

Invoke-Step "Test" { dotnet test (Join-Path $root "BgPasswordGenerator.slnx") -c Release }

foreach ($runtime in $Runtimes) {
    $publishDir = Join-Path $root "publish/$runtime"
    $releaseDir = Join-Path $root "Releases/$runtime"
    if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }

    Invoke-Step "Publish $runtime" {
        dotnet publish $project -c Release -r $runtime -o $publishDir
    }
    # Without these the packaged app crashes on startup (see PublishAppXamlResources in the csproj)
    foreach ($file in "BgPasswordGenerator.pri", "MainWindow.xbf") {
        if (-not (Test-Path (Join-Path $publishDir $file))) { throw "Publish output is missing $file" }
    }
    Invoke-Step "Pack $runtime $version" {
        vpk pack `
            --packId BgPasswordGenerator `
            --packVersion $version `
            --packDir $publishDir `
            --mainExe BgPasswordGenerator.exe `
            --packTitle "BG Password Generator" `
            --packAuthors "Berk Guclukol" `
            --icon (Join-Path $root "src/BgPasswordGenerator/Assets/icon.ico") `
            --runtime $runtime `
            --channel $runtime `
            --outputDir $releaseDir
    }
}

Write-Host "`nPackages:" -ForegroundColor Green
Get-ChildItem (Join-Path $root "Releases") -Recurse -Include *-Setup.exe, *-Portable.zip |
    ForEach-Object { "{0,8:N1} MB  {1}" -f ($_.Length / 1MB), $_.FullName.Substring($root.Length + 1) }

if ($Publish) {
    # The app's updater reads releases.<runtime>.json from the latest GitHub release and downloads the
    # packages it lists, so upload those alongside this version's installers. Older versions' packages
    # in Releases/ are already attached to their own releases.
    $assets = foreach ($runtime in $Runtimes) {
        Get-ChildItem (Join-Path $root "Releases/$runtime") -File | Where-Object {
            $_.Name -like "*-$version-*.nupkg" -or
            $_.Name -like "*-Setup.exe" -or $_.Name -like "*-Portable.zip" -or
            $_.Name -like "releases.*.json" -or $_.Name -like "assets.*.json" -or $_.Name -like "RELEASES-*"
        } | ForEach-Object FullName
    }
    Invoke-Step "GitHub release $version" {
        gh release create $version @assets --target main --title "BG Password Generator v$version" --notes-file $NotesFile --latest
    }
}
