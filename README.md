# WakaTime for FL Studio
Tracks time when working on FL Studio projects via WakaTime.

## Installing
1. Download the latest `wakatime-flstudio.exe` executable.
2. Copy the executable to the startup folder to avoid manually running it by opening the `shell:startup` folder. You can access it either via File Explorer or by navigating to it through <kbd>WIN</kbd> + <kbd>R</kbd>. This step is optional.
3. Run it, and start working on a saved project. The timer widget should appear in the lower right-hand corner.
4. Your FL Studio activity will show up as "designing" on your WakaTime Dashboard.

If you choose to copy the executable to your startup directory, you will not have to launch it manually in order to count time.

## Activity metrics
WakaTime for FL Studio will track the sum of the amount of notes in patterns, playlist items, and mixer effects as lines. The "line count" can then be used to estimate progress on any given project.

## Building
In order to build WakaTime for FL Studio, make sure you have .NET >= 9.0 installed.

You can build the project by using `dotnet publish -r win-x64 -c release`. This will create a `bin/Release/net9.0/win-x64/publish/wakatime-flstudio.exe` binary.