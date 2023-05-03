using System.Diagnostics;
using System.IO.Compression;

namespace SSQE_Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentPath = Directory.GetCurrentDirectory();
            var currentVersion = "";
            if (File.Exists("Sound Space Quantum Editor.exe"))
                currentVersion = FileVersionInfo.GetVersionInfo("Sound Space Quantum Editor.exe").FileVersion;

            var newVersion = CheckVersion();
            var file = $"SSQE{newVersion}.zip";

            if (newVersion != "")
            {
                Console.WriteLine("Starting download...");
                
                try
                {
                    WebClient.DownloadFile($"https://github.com/David20122/Sound-Space-Quantum-Editor/releases/download/{newVersion}/SSQE{newVersion}.zip", file);
                    ExtractFile();
                }
                catch
                {
                    Console.WriteLine("Failed to download new editor version");
                    Quit();
                }
            }

            string[] GetOverwrites()
            {
                try
                {
                    var overwriteList = WebClient.DownloadString("https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/updater_overwrite");

                    return overwriteList.Split('\n');
                }
                catch
                {
                    Console.WriteLine("Failed to fetch overrides");
                    Quit();
                }

                return Array.Empty<string>();
            }

            string CheckVersion()
            {
                try
                {
                    var redirect = WebClient.GetRedirect("https://github.com/David20122/Sound-Space-Quantum-Editor/releases/latest");

                    if (!string.IsNullOrWhiteSpace(redirect))
                    {
                        var version = redirect[(redirect.LastIndexOf("/") + 1)..];
                        
                        if (version != currentVersion)
                            return version;
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to check version");
                    Quit();
                }

                return "";
            }

            void KillProcess()
            {
                foreach (var process in Process.GetProcessesByName("Sound Space Quantum Editor"))
                    process.Kill();

                Thread.Sleep(500);
            }

            bool IsInOverwrites(string[] list, string fileName)
            {
                foreach (var line in list)
                    if (fileName == line)
                        return true;

                return false;
            }

            void ExtractFile()
            {
                var overwriteList = GetOverwrites();

                Console.WriteLine("Completed, extracting...");

                KillProcess();

                using (ZipArchive archive = ZipFile.OpenRead(file))
                {
                    foreach (var entry in archive.Entries)
                    {
                        try
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(currentPath, entry.FullName)) ?? "");
                            entry.ExtractToFile(Path.Combine(currentPath, entry.FullName), IsInOverwrites(overwriteList, entry.FullName));
                        }
                        catch { }
                    }
                }

                Console.WriteLine("Completed, launching...");

                File.Delete(file);
                Process.Start("Sound Space Quantum Editor");

                Quit();
            }

            void Quit()
            {
                Thread.Sleep(1500);
                Environment.Exit(0);
            }
        }
    }
}