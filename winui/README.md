# BG Password Generator — WinUI 3

Native Windows version of the app, built with WinUI 3 (Windows App SDK) and .NET 10.

- `src/BgPasswordGenerator.Core` — password generation and strength rules (plain .NET, no UI)
- `src/BgPasswordGenerator` — WinUI 3 app, unpackaged and self-contained
- `tests/BgPasswordGenerator.Tests` — xUnit tests for the core library

Copied passwords are kept out of Windows clipboard history (Win+V) and cloud clipboard sync,
and are cleared after 30 seconds or when the window closes, unless something else was copied since.

## Build and run

Requires the .NET 10 SDK.

```sh
dotnet test tests/BgPasswordGenerator.Tests
dotnet run --project src/BgPasswordGenerator
```

## Publish

```sh
dotnet publish src/BgPasswordGenerator -c Release -r win-x64 -o publish
```

The output folder is self-contained and runs without installing .NET or the Windows App Runtime.
