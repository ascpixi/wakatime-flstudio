// Uncomment this to test heartbeat data generation. This should always be
// left commented when commiting to the git repo.
//#define MOCK_HEARTBEATS

using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using FluentResults;
using Monad.FLParser;

using Ascpixi.Wakatime.FLStudio.LanguageExtensions;

namespace Ascpixi.Wakatime.FLStudio;

public static class WakaTime
{
    // probably the closest we can get to anything regarding audio
    public const string PreferredCategory = "designing";
    
    const string CliDownloadUrl = "https://github.com/wakatime/wakatime-cli/releases/latest/download";
    
    static string? CliLocation;
    
    // File-system roots. If a directory contains a file-system entry that has a name
    // specified in this array, it's picked. Each entry of this array specifies a
    // priority level. All names are case-insensitive.
    static readonly string[][] FsRoots = [
        [
            ".wakatime-project",
            ".git"
        ],
        [
            ".gitattributes",
            ".gitignore",
            "readme.md",
            "license"
        ],
        [
            "Assembly-CSharp.csproj", // Unity
            "readme.txt",
            "license.md",
            "license.txt"
        ],
        [
            ".vscode",
            ".vs",
            ".idea"
        ]
    ];

    static readonly HashSet<string> ForbiddenDirs = [
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
    ];
    
    static string HomeLocation {
        get {
            string? wakaHome = Environment.GetEnvironmentVariable("WAKATIME_HOME");
            
            return !string.IsNullOrEmpty(wakaHome) && Directory.Exists(wakaHome)
                ? wakaHome
                : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }

    static string ResourceLocation => Path.Combine(HomeLocation, ".wakatime");
    
    public static Result TryInitialize()
    {
        string os =
            OperatingSystem.IsWindows() ? "windows" :
            OperatingSystem.IsMacOS() ? "darwin" :
            throw new NotSupportedException("Unsupported operating system."); // FL only runs on Windows and macOS

        string arch = RuntimeInformation.OSArchitecture switch {
            Architecture.X86     => "386",
            Architecture.X64     => "amd64",
            Architecture.Arm64   => "arm64",
            Architecture.RiscV64 => "riscv64",
            Architecture.Arm     => "arm",
            _ => throw new NotSupportedException("Unsupported architecture.")
        };

        var slug = $"wakatime-cli-{os}-{arch}";
        string exec = OperatingSystem.IsWindows() ? $"{slug}.exe" : slug;
        string execPath = Path.Combine(ResourceLocation, exec);
        
        Log.Info($"Expected WakaTime CLI slug: {slug}");
        
        if (!File.Exists(execPath)) {
            Log.Warning($"No '{exec}' file detected. Downloading the WakaTime CLI...");

            using var ms = new MemoryStream();
            
            try {
                using var http = new HttpClient();
                using var s = http.GetStreamAsync($"{CliDownloadUrl}/{slug}.zip");
                s.Result.CopyTo(ms);
            }
            catch (Exception ex) {
                Log.Error($"Could not download the CLI!\n\n{ex}\n");
                return Result.Fail($"Couldn't download the WakaTime CLI. You can try downloading it manually. Error: {ex.GetType().Name}: {ex.Message}");
            } 
            
            Log.Info("File successfully downloaded - now extracting...");

            using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
            zip.Entries[0].ExtractToFile(execPath);
            
            Log.Info($"File extracted to {execPath}.");
        }
        
        // Ensure that the CLI functions properly
        var proc = Process.Start(new ProcessStartInfo(execPath, ["--version"]) {
             CreateNoWindow = true,
             UseShellExecute = false
        });
        
        proc!.WaitForExit();

        if (proc.ExitCode != 0)
            return Result.Fail($"The invocation of '{execPath} --version' failed with error code {proc.ExitCode}.");

        CliLocation = execPath;
        
        return Result.Ok();
    }
    
    /// <summary>
    /// Emits a heartbeat for the given project, located in the given path.
    /// This method should be invoked in at least two-minute intervals when the
    /// FL Studio window is in the foreground, or when the file is saved.
    /// </summary>
    public static void EmitHeartbeat(string projectPath, bool isWrite = false)
    {
        if (CliLocation == null)
            throw new Exception("Cannot call EmitHeartbeat when CliLocation is null.");

        string projectFilePath = projectPath;
        if (!isWrite) {
            // If this is not a write, we might be able to get a more recent
            // version of the project from the backup directory.
            var lastBackup = FLStudio.GetLastBackup(projectPath);
            if (
                lastBackup != null
                && lastBackup.LastWriteTime > File.GetLastWriteTime(projectPath)
            ) {
                projectFilePath = lastBackup.FullName;
            }
        }
        
        var proj = Project.Load(projectFilePath, false);

        string? wakatimeProject = null;

        // The user can specify the project name inside the "Comments" metadata field of
        // the project, in the form of "WakaTime Project: <NAME>", on its own line. <NAME>
        // can also be "(none)" if the project should be excluded.
        string[] lines = proj.Comment.Split('\r');

        foreach (string line in lines) {
            if (!line.StartsWith("WakaTime Project: ", StringComparison.OrdinalIgnoreCase))
                continue;

            wakatimeProject = line[18..];
            if (wakatimeProject == "(none)") {
                GC.Collect();
                return; // project is explicitly ignored
            }

            break;
        }

        if (wakatimeProject == null) {
            // No "WakaTime Project: " directive in the project. We try to determine the
            // most suitable project name via the directory structure. Directories that
            // have certain files/directories that are hints of project roots are used.
            foreach (string[] level in FsRoots) {
                string? current = Path.GetDirectoryName(projectPath);

                while (current != null) {
                    if (!ForbiddenDirs.Contains(current)) {
                        bool hasRoot = new DirectoryInfo(current)
                            .EnumerateFileSystemInfos()
                            .Any(x => level.Contains(x.Name));

                        if (hasRoot) {
                            wakatimeProject = Path.GetFileName(current);
                            break;
                        }
                    }
                    
                    current = Path.GetDirectoryName(current);
                }

                if (wakatimeProject != null) {
                    break; // We've found a suitable root!
                }
            }
        }
        
        // We count the amount of notes and tracks in the project as the "line" count.
        int entityCount =
            proj.Patterns.Sum(x => x.Notes.Sum(y => y.Value.Count))
            + proj.Tracks.Sum(x => x.Items.Count)
            + proj.Inserts.Sum(x => x.Slots.Sum(y => y.Plugin?.FileName != null ? 1 : 0));
        
#if MOCK_HEARTBEATS
        Log.Info("Mock heartbeat:");
        Log.Info($"  entity = {projectPath}");
        Log.Info($"  lines-in-file = {entityCount}");
        Log.Info($"  write = {isWrite}");
        Log.Info($"  project = {wakatimeProject ?? "(NONE!)"}");
#else
        var proc = Process.Start(new ProcessStartInfo(CliLocation) {
            CreateNoWindow = true,
            UseShellExecute = false,
            ArgumentList = {
                "--entity", projectPath,
                "--category", PreferredCategory,
                "--lines-in-file", entityCount.ToString(),
                "--plugin", $"flstudio/{proj.VersionString} wakatime-flstudio/{App.Version}",
                { "--write", isWrite },
                { ["--project", wakatimeProject!], wakatimeProject != null }
            }
        });

        proc!.WaitForExit();
        if (proc.ExitCode != 0) {
            Log.Error($"The WakaTime CLI returned an error code of {proc.ExitCode}!");
        }
#if DEBUG
        // We only log heartbeats in debug mode.
        else {
            Log.Info($"Heartbeat: {projectPath}, {entityCount} entities, write: {isWrite}");
        }
#endif
#endif

        GC.Collect();
    }

    /// <summary>
    /// Gets a human-readable time string, in the form of "H hrs M mins", that represents
    /// the amount of time spent working on a given category. A list of valid categories
    /// can be found at https://wakatime.com/colors/categories.
    /// </summary>
    public static string GetCategoryTime(string category = PreferredCategory)
    {
        if (CliLocation == null)
            throw new Exception("Cannot call GetCategoryTime when CliLocation is null.");

        var proc = Process.Start(new ProcessStartInfo(CliLocation, "--today") {
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            UseShellExecute = false
        });

        proc!.Start();
        return proc.StandardOutput.ReadToEnd()
            .Split(',')
            .Select(x => x.Trim())
            .FirstOrDefault(x => x.EndsWith(category), $"0 hrs 0 mins {category}")
            [..^(category.Length + 1)];
    }
}
