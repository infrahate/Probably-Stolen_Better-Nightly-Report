## Build

This folder belongs in the game install at `Modding/BetterNightlyReport`, so the project can see `Probably Stolen_Data/Managed`. You need the [.NET SDK](https://dotnet.microsoft.com/download) 6.0 or later.

```bash
dotnet build -c Release
```

The `build/` folder is the mod: `BetterNightlyReport.dll`, `manifest.xml`, `Icons/`, and `preview.png` if that file is in the project. Copy `build/` into the game's `Mods` folder.
