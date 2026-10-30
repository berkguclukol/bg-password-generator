<#
.SYNOPSIS
    Tests, publishes and packages BG Password Generator into a Setup.exe and a portable zip.

.EXAMPLE
    ./build.ps1
    ./build.ps1 -Runtimes win-x64, win-x86, win-arm64

.NOTES
    Requires the .NET 10 SDK and the Velopack CLI matching the Velopack package version:
    dotnet tool install -g vpk --version 1.2.158
    Output goes to Releases/<runtime>/.
#>
param(
    [string[]] $Runtimes = @("win-x64")
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$project = Join-Path $root "src/BgPasswordGenerator/BgPasswordGenerator.csproj"

function Invoke-Step([string] $name, [scriptblock] $command) {
    Write-Host "==> $name" -ForegroundColor Cyan
    & $command
    if ($LASTEXITCODE -ne 0) { throw "$name failed with exit code $LASTEXITCODE" }
}

# The csproj <Version> is the single source of truth for the release version
$version = (dotnet msbuild $project -getProperty:Version).Trim()

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
