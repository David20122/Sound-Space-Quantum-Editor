using System.Diagnostics;
using System.Runtime.InteropServices;
using New_SSQE.Misc.Static;

namespace New_SSQE.ExternalUtils
{
    internal class Platform
    {
        public static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static readonly string Extension = IsLinux ? "" : ".exe";
        public static readonly string ExecutableFilter = IsLinux ? "Executable Files|*.*" : "Executable Files (*.exe)|*.exe";

        public static void OpenDirectory(string directory)
        {
            if (IsLinux)
                Process.Start("xdg-open", directory);
            else
                Process.Start("explorer.exe", directory);
        }
        public static void OpenDirectory() => OpenDirectory(Assets.THIS);

        public static Process RunExecutable(string path, string exe, string args) => Process.Start($"{path}\\{GetExecutableName(exe)}", args);
        public static Process RunExecutable(string path, string exe, params string[] args) => RunExecutable(path, exe, string.Join(" ", args));
        public static Process RunExecutable(string exe, string args) => RunExecutable(Assets.THIS, exe, args);

        public static bool ExecutableExists(string path, string exe) => File.Exists($"{path}\\{GetExecutableName(exe)}");
        public static bool ExecutableExists(string exe) => ExecutableExists(Assets.THIS, exe);

        public static FileVersionInfo GetExecutableVersionInfo(string path, string exe) => FileVersionInfo.GetVersionInfo($"{path}\\{exe}{Extension}");
        public static FileVersionInfo GetExecutableVersionInfo(string exe) => GetExecutableVersionInfo(Assets.THIS, exe);

        public static string GetExecutableName(string exe) => $"{exe}{Extension}";

        public static bool CheckDependency(string path, string dependency)
        {
            if (dependency == "bassenc") // bassenc is not a dependency on linux
                dependency = "bass";
            dependency = IsLinux ? $"lib{dependency}.so" : $"{dependency}.dll";

            return File.Exists($"{path}\\{dependency}");
        }
        public static bool CheckDependency(string dependency) => CheckDependency(Assets.THIS, dependency);

        public static Process? OpenLink(string url)
        {
            ProcessStartInfo ps = new(url)
            {
                UseShellExecute = true,
                Verb = "open"
            };

            return Process.Start(ps);
        }
    }
}
