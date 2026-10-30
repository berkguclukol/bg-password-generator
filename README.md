# BG Password Generator 👋
#### It is designed for personal use.

A native Windows app for quickly creating passwords. The application works offline and does not require internet. Generated passwords are never stored anywhere and/or sent to a remote location.

- Passwords are generated with a cryptographically secure random number generator, contain every selected character type and are shuffled.
- Copied passwords are kept out of Windows clipboard history (Win+V) and cloud clipboard sync, and are cleared after 30 seconds or when the window closes, unless something else was copied since.

## Tech
![App Tech](https://skillicons.dev/icons?i=cs,dotnet,windows)

WinUI 3 (Windows App SDK) on .NET 10.

- `src/BgPasswordGenerator.Core` — password generation and strength rules (plain .NET, no UI)
- `src/BgPasswordGenerator` — WinUI 3 app, unpackaged and self-contained
- `tests/BgPasswordGenerator.Tests` — xUnit tests for the core library

## Installation

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```sh
git clone https://github.com/berkguclukol/bg-password-generator.git
cd bg-password-generator
dotnet test
dotnet run --project src/BgPasswordGenerator
```

## Packaging

Setup and portable packages are built with [Velopack](https://velopack.io). Install its CLI once, matching the `Velopack` package version in the app project:

```sh
dotnet tool install -g vpk --version 1.2.158
```

Then run the build script, which tests, publishes and packages the app:

```powershell
./build.ps1                                   # win-x64
./build.ps1 -Runtimes win-x64, win-x86, win-arm64
```

Output in `Releases/<runtime>/`:

- `BgPasswordGenerator-<runtime>-Setup.exe` — per-user install (no admin), with Desktop and Start Menu shortcuts and an uninstall entry in Apps & features
- `BgPasswordGenerator-<runtime>-Portable.zip` — extract anywhere and run `BG Password Generator.exe`

Both are self-contained and run without installing .NET or the Windows App Runtime. The release version comes from `<Version>` in `src/BgPasswordGenerator/BgPasswordGenerator.csproj`.

## Screen Shot

![APP UI](docs/screenshot.png)

## License

MIT
