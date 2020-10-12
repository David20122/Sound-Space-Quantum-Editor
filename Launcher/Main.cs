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
        private WebClient wc = new WebClient();
        public Main()
        {
            InitializeComponent();
        }

        public List<Release> releaseList;

        private void Main_Load(object sender, EventArgs e)
        {
            wc.Headers.Add("User-Agent: krmeet");
            var releases = wc.DownloadString("https://api.github.com/repos/David20122/Sound-Space-Quantum-Editor/releases");
            releaseList = JsonSerializer.Deserialize<List<Release>>(releases);
            VersionSelect.Items.Clear();
            VersionSelect.Items.Add("Latest");
            VersionSelect.Text = "Latest";
            foreach (var release in releaseList)
            {
                VersionSelect.Items.Add(release.name);
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
            try
            {
                DownloadInfo.Text = "Getting SSQE directory";
                var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var ssqeDir = Path.Combine(appdata, "SSQE");
                var verDir = Path.Combine(ssqeDir, "Versions/" + release.name);
                if (!Directory.Exists(ssqeDir))
                {
                    DownloadInfo.Text = "Creating new SSQE directory";
                    Directory.CreateDirectory(ssqeDir);
                    Directory.CreateDirectory(Path.Combine(ssqeDir, "Versions"));
                }
                else
                {
                    if (!Directory.Exists(Path.Combine(ssqeDir, "Versions")))
                    {
                        DownloadInfo.Text = "Creating new versions directory";
                        Directory.CreateDirectory(Path.Combine(ssqeDir, "Versions"));
                    }
                }
                DownloadInfo.Text = "Checking if version exists";
                if (Directory.Exists(verDir) && File.Exists(Path.Combine(verDir,"Sound Space Quantum Editor.exe")))
                {
                    DownloadInfo.Text = "Version exists, running";
                    var startInfo = new ProcessStartInfo();
                    startInfo.FileName = Path.Combine(verDir, "Sound Space Quantum Editor.exe");
                    startInfo.WorkingDirectory = verDir;
                    Process.Start(startInfo);
                    Close();
                } else
                {
                    WebClient dl = new WebClient();
                    dl.Headers.Add("User-Agent: krmeet");
                    DownloadInfo.Text = "Downloading version "+release.name;
                    dl.DownloadProgressChanged += Dl_DownloadProgressChanged;
                    var tempFile = Path.Combine(ssqeDir, "dltemp_" + release.name + "._temp");
                    await dl.DownloadFileTaskAsync(new Uri(release.assets.First().browser_download_url), tempFile);
                    DownloadProgress.Value = 0;
                    DownloadInfo.Text = "Unzipping";
                    var zip = ZipFile.Read(tempFile);
                    zip.ExtractProgress += Zip_ExtractProgress;
                    zip.ExtractAll(Path.Combine(ssqeDir, "Versions"));
                    if (Directory.Exists(verDir) && File.Exists(Path.Combine(verDir, "Sound Space Quantum Editor.exe")))
                    {
                        DownloadInfo.Text = "Version exists, running";
                        var startInfo = new ProcessStartInfo();
                        startInfo.FileName = Path.Combine(verDir, "Sound Space Quantum Editor.exe");
                        startInfo.WorkingDirectory = verDir;
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

        private void Zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.TotalBytesToTransfer > 0)
            {
                DownloadProgress.Value = (int)e.BytesTransferred / (int)e.TotalBytesToTransfer;
            }
            DownloadInfo.Text = "Unzipping " + DownloadProgress.Value.ToString() + "%";
        }

        private void Dl_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Value = e.ProgressPercentage;
            DownloadInfo.Text = "Downloading release from GitHub " + e.ProgressPercentage.ToString() + "% (" + e.BytesReceived.ToString() + "B/" + e.TotalBytesToReceive.ToString() + "B)";
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
            }
        }
    }
}
