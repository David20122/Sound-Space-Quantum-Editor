using System;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

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

            var wc = new WebClient();

            if (newVersion != "")
            {
                var uri = new Uri($"https://github.com/Avibah/Sound-Space-Quantum-Editor/releases/download/{newVersion}/SSQE{newVersion}.zip");

                Console.Write("Starting download...\n");

                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(ExtractFile);

                wc.DownloadFileAsync(uri, file);

                var task = new TaskCompletionSource<bool>().Task;
                task.Wait();
            }

            string[] GetOverwrites()
            {
                try
                {
                    var overwriteList = wc.DownloadString("https://raw.githubusercontent.com/Avibah/Sound-Space-Quantum-Editor/updater/OverwriteList");

                    return overwriteList.Split('\n');
                }
                catch
                {
                    Console.Write("Failed to fetch overrides");

                    Quit();
                }

                return new string[0];
            }

            string CheckVersion()
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create("https://github.com/Avibah/Sound-Space-Quantum-Editor/releases/latest");
                    request.AllowAutoRedirect = false;

                    var response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.Redirect)
                    {
                        var location = response.Headers["Location"];
                        var rep = location.LastIndexOf("/") + 1;
                        var version = location.Substring(rep, location.Length - rep);

                        if (version != currentVersion)
                            return version;
                    }
                }
                catch
                {
                    Console.Write("Failed to check new version\n");
                    
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

            void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
            {
                Console.Write($"\r{e.ProgressPercentage}%");
            }

            bool IsInOverwrites(string[] list, string fileName)
            {
                foreach (var line in list)
                    if (fileName == line)
                        return true;

                return false;
            }

            void ExtractFile(object sender, AsyncCompletedEventArgs e)
            {
                var overwriteList = GetOverwrites();

                Console.Write("\nCompleted, extracting...");

                if (e.Error != null)
                    Console.Write($"\n{e.Error}");
                else
                {
                    KillProcess();
                    
                    using (ZipArchive archive = ZipFile.OpenRead(file))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            try
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(currentPath, entry.FullName)));
                                entry.ExtractToFile(Path.Combine(currentPath, entry.FullName), entry.FullName.Contains(".exe") || IsInOverwrites(overwriteList, entry.FullName));
                            }
                            catch { }
                        }
                    }

                    Console.Write("\nCompleted, launching...");

                    File.Delete(file);
                    Process.Start("Sound Space Quantum Editor");
                    
                    Quit();
                }
            }

            void Quit()
            {
                Thread.Sleep(1500);
                Environment.Exit(0);
            }
        }
    }
}
