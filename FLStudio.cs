using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Ascpixi.Wakatime.FLStudio;

public static partial class FLStudio
{
    /// <summary>
    /// Gets the path to the user's Image-Line data folder.
    /// </summary>
    public static string GetUserDataFolder()
    {
        static string GetDefault()
        {
            Log.Warning("The user data folder registry key is missing. Using the default path.");
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Image-Line");
        }
        
        using var hkcu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
        using var paths = Registry.CurrentUser.OpenSubKey(@"Software\Image-Line\Shared\Paths");
        if (paths == null)
            return GetDefault();

        return (string?)paths.GetValue("Shared data") ?? GetDefault();
    }

    [GeneratedRegex(@"^(.+) \(autosaved at .+\)(?:_[0-9]+)?\.flp$")]
    private static partial Regex BackupRegex();
    
    /// <summary>
    /// Checks if a filename 'fullName' (includes extension) represents an FL Studio
    /// backup of a project with the filename of 'target' (does not include extension).
    /// </summary>
    public static bool IsBackupOf(string fullName, ReadOnlySpan<char> target)
    {
        var match = BackupRegex().Match(fullName);
        return match.Groups[1].ValueSpan.SequenceEqual(target);
    }

    public static FileInfo? GetLastBackup(string projectPath)
    {
        string backupDir = Path.Combine(GetUserDataFolder(), "FL Studio", "Projects", "Backup");

        if (!Directory.Exists(backupDir))
            return null;
        
        string projectName = Path.GetFileNameWithoutExtension(projectPath);

        return new DirectoryInfo(backupDir)
            .EnumerateFiles()
            .Where(x => IsBackupOf(x.Name, projectName))
            .MaxBy(x => x.LastWriteTime);
    }
}