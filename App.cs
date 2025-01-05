using System.Reflection;

[assembly:
    AssemblyVersion(Ascpixi.Wakatime.FLStudio.App.Version),
    AssemblyFileVersion(Ascpixi.Wakatime.FLStudio.App.Version),
    AssemblyCopyright(Ascpixi.Wakatime.FLStudio.App.Author),
    AssemblyTitle(Ascpixi.Wakatime.FLStudio.App.Title),
]

namespace Ascpixi.Wakatime.FLStudio;

/// <summary>
/// Contains metadata constants.
/// </summary>
public static class App
{
    public const string Version = "0.1.0";
    public const string Author = "ascpixi";
    public const string Title = "WakaTime for FL Studio";
}