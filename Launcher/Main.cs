using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using Ionic.Zip;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Launcher
{
    public partial class Main : Form
    {
        public List<Release> releaseList;
        public List<String> modList = new List<String>();
        public string appdata;
        public string ssqeDir;

        private readonly WebClient wc = new WebClient();
        public Main()
        {
            appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            ssqeDir = Path.Combine(appdata, "SSQE");
            if (!Directory.Exists(ssqeDir))
            {
                Directory.CreateDirectory(ssqeDir);
                Directory.CreateDirectory(Path.Combine(ssqeDir, "versions"));
                Directory.CreateDirectory(Path.Combine(ssqeDir, "mods"));
            }
            else
            {
                if (!Directory.Exists(Path.Combine(ssqeDir, "versions")))
                {
                    Directory.CreateDirectory(Path.Combine(ssqeDir, "versions"));
                }
                if (!Directory.Exists(Path.Combine(ssqeDir, "mods")))
                {
                    Directory.CreateDirectory(Path.Combine(ssqeDir, "mods"));
                }
            }
            wc.Headers.Add("User-Agent: krmeet");
            var releases = wc.DownloadString("https://api.github.com/repos/David20122/Sound-Space-Quantum-Editor/releases");
            releaseList = JsonSerializer.Deserialize<List<Release>>(releases);
            foreach (string dir in Directory.GetDirectories(Path.Combine(ssqeDir, "mods")))
            {
                if (File.Exists(Path.Combine(dir, "Sound Space Quantum Editor.exe"))){
                    modList.Add(Path.GetDirectoryName(dir));
                }
            }
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            VersionSelect.Items.Clear();
            VersionSelect.Items.Add("Latest");
            VersionSelect.Text = "Latest";
            foreach (var release in releaseList)
            {
                VersionSelect.Items.Add(release.name);
            }
            foreach (string mod in modList)
            {
                VersionSelect.Items.Add("MOD/ " + mod);
            }
        }

        private async void DownloadRelease(Release release)
        {
            DownloadInfo.Text = "";
            DownloadProgress.Value = 0;
            NoTasks.Visible = false;
            DownloadInfo.Visible = true;
            DownloadProgress.Visible = true;
            LaunchButton.Enabled = false;
            VersionSelect.Enabled = false;
            var verDir = Path.Combine(ssqeDir, "versions/" + release.name);
            try
            {
                DownloadInfo.Text = "Checking if version exists";
                if (Directory.Exists(verDir) && File.Exists(Path.Combine(verDir,"Sound Space Quantum Editor.exe")))
                {
                    DownloadInfo.Text = "Version exists, running";
                    var startInfo = new ProcessStartInfo();
                    startInfo.FileName = Path.Combine(verDir, "Sound Space Quantum Editor.exe");
                    startInfo.WorkingDirectory = verDir;
                    startInfo.Arguments = ssqeDir;
                    Process.Start(startInfo);
                    Close();
                } else
                {
                    WebClient dl = new WebClient();
                    dl.Headers.Add("User-Agent: krmeet");
                    DownloadInfo.Text = "Downloading version "+release.name;
                    dl.DownloadProgressChanged += Dl_DownloadProgressChanged;
                    var tempFile = Path.Combine(Path.GetTempPath(), "dl_" + release.name + "._temp");
                    await dl.DownloadFileTaskAsync(new Uri(release.assets.First().browser_download_url), tempFile);
                    DownloadProgress.Value = 0;
                    DownloadInfo.Text = "Unzipping";
                    var zip = ZipFile.Read(tempFile);
                    zip.ExtractProgress += Zip_ExtractProgress;
                    zip.ExtractAll(Path.Combine(ssqeDir, "versions"), ExtractExistingFileAction.OverwriteSilently);
                    zip.Dispose();
                    File.Delete(tempFile);
                    if (Directory.Exists(verDir) && File.Exists(Path.Combine(verDir, "Sound Space Quantum Editor.exe")))
                    {
                        DownloadInfo.Text = "Version exists, running";
                        var startInfo = new ProcessStartInfo();
                        startInfo.FileName = Path.Combine(verDir, "Sound Space Quantum Editor.exe");
                        startInfo.WorkingDirectory = verDir;
                        startInfo.Arguments = ssqeDir;
                        Process.Start(startInfo);
                        Close();
                    }
                }
            } catch (Exception e) {
                throw e;
            }
            NoTasks.Visible = true;
            DownloadInfo.Visible = false;
            DownloadProgress.Visible = false;
            LaunchButton.Enabled = true;
            VersionSelect.Enabled = true;
        }

        private void LaunchMod(string mod)
        {
            DownloadInfo.Text = "";
            DownloadProgress.Value = 0;
            NoTasks.Visible = false;
            DownloadInfo.Visible = true;
            DownloadProgress.Visible = true;
            LaunchButton.Enabled = false;
            VersionSelect.Enabled = false;

            var verDir = Path.Combine(ssqeDir, "mods/" + mod);
            try
            {
                DownloadInfo.Text = "Checking if version exists";
                if (Directory.Exists(verDir) && File.Exists(Path.Combine(verDir, "Sound Space Quantum Editor.exe")))
                {
                    DownloadInfo.Text = "Version exists, running";
                    var startInfo = new ProcessStartInfo();
                    startInfo.FileName = Path.Combine(verDir, "Sound Space Quantum Editor.exe");
                    startInfo.WorkingDirectory = verDir;
                    startInfo.Arguments = ssqeDir;
                    Process.Start(startInfo);
                    Close();
                }
                else
                {
                    throw new Exception("bro you cant just delete a mod and then try to run it");
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            NoTasks.Visible = true;
            DownloadInfo.Visible = false;
            DownloadProgress.Visible = false;
            LaunchButton.Enabled = true;
            VersionSelect.Enabled = true;
        }

        private void Zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EntriesTotal > 0)
            {
                DownloadProgress.Value = e.EntriesExtracted * 100 / e.EntriesTotal;
                DownloadInfo.Text = "Unzipping " + (e.EntriesExtracted * 100 / e.EntriesTotal).ToString() + "%";
            }
        }

        private void Dl_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Value = e.ProgressPercentage;
            DownloadInfo.Text = "Downloading release from GitHub " + e.ProgressPercentage.ToString() + "% (" + (e.BytesReceived / 1000).ToString() + "kB/" + (e.TotalBytesToReceive / 1000).ToString() + "kB)";
        }

        private void LaunchButton_Click(object sender, EventArgs e)
        {
            if(VersionSelect.Text == "Latest")
            {
                DownloadRelease(releaseList.First());
            } else
            {
                foreach (Release release in releaseList)
                {
                    if (release.name == VersionSelect.Text)
                    {
                        DownloadRelease(release);
                        break;
                    }
                }
                foreach (string mod in modList)
                {
                    if ("MOD/ " + mod == VersionSelect.Text)
                    {
                        LaunchMod(mod);
                        break;
                    }
                }
            }
        }
    }
}
