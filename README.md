# Vencord Fixer

A tiny program that automatically downloads and installs/updates Vencord for your Discord client invisibly and quickly. Originally by cheatfreak47 and converted to C# to hopefully avoid being detected as a false positive.

## Usage
1. Download the latest `VencordFixer.exe` from Releases.
2. Put it wherever you keep small programs (Desktop, `C:\Tools\`, etc.)
3. *(Optional)* Right-click > Create shortcut to place on:
   - Desktop
   - Start menu folder (`%APPDATA%\Microsoft\Windows\Start Menu\Programs`)
4. Open it and let it do its thing!

## What It Does
- Asks you if it can download the latest `VencordInstallerCli.exe` if missing
- Force-closes Discord (saves you the trouble)
- Updates VencordInstallerCli via `-update-self`
- Installs Vencord to your local Discord folder
- Restarts Discord automatically
- It does all that invisibly (no windows, popups, nothing)

## Build Instructions
Clone this repo and open it with Visual Studio.

## Requirements
- Windows OS
- .NET Framework 4.8 (comes pre-installed with Windows 10 and 11)
- Discord installed in default `AppData\Local\Discord` location
- Internet connection (for initial VencordInstallerCli download)
