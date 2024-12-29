using Avalonia;
using New_SSQE.ExternalUtils;
using New_SSQE.Maps;
using New_SSQE.Misc.Dialogs;
using New_SSQE.Misc.Static;
using New_SSQE.Preferences;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace New_SSQE
{
    internal class Program
    {
        public static readonly CultureInfo Culture;
        public static readonly JsonSerializerOptions JsonOptions;
        public static readonly string Version = $"{Assembly.GetExecutingAssembly().GetName().Version}";

        static Program()
        {
            Culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            Culture.NumberFormat.NumberDecimalSeparator = ".";

            JsonOptions = new() { WriteIndented = true };
        }

        private static void Start()
        {
            BuildAvaloniaApp();

            Settings.Load(false);
            MainWindow.DebugVersion |= Settings.debugMode.Value;
            
            MainWindow window = new(Settings.msaa.Value ? 32 : 0);
            ProgramArgs.Watch();

            using (window)
                window.Run();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .SetupWithoutStarting();

        public static bool IsOtherWindowOpen()
        {
            Process[] processes = Process.GetProcessesByName("Sound Space Quantum Editor");
            return processes.Length > 1;
        }
        
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "registerProtocol":
                            Protocol.Finish();
                            return;
                        case not "registerProtocol" when args[0].StartsWith("ssqe://"):
                            string[] final = args[0].Replace("%20", " ").Replace("%5C", "\\").Split('/')[2..];
                            string file = $"{Assets.TEMP}\\tempargs.txt";

                            if (File.Exists(file))
                                File.Delete(file);
                            File.WriteAllText(file, string.Join(' ', final));

                            if (final[0] == "open" && final[1].StartsWith("file ") && !IsOtherWindowOpen())
                            {
                                MainWindow.InitialFile = final[1].Remove(0, 5);
                                break;
                            }
                            return;
                    }
                }
            }
            catch
            {
                return;
            }

            try
            {
                try
                {
                    ProgramArgs.Start(args, IsOtherWindowOpen());
                }
                catch (Exception ae)
                {
                    Logging.Register("Failed to parse args!", LogSeverity.WARN, ae);
                }

                try
                {
                    Protocol.Register();
                }
                catch (Exception pe)
                {
                    Logging.Register("Failed to register protocol!", LogSeverity.WARN, pe);
                }

                TaskScheduler.UnobservedTaskException += (s, e) =>
                {
                    Logging.Register("Unobserved task exception occurred", LogSeverity.ERROR, e.Exception);
                };

                Logging.Register($"Operating System: {RuntimeInformation.OSDescription}");
                Start();

                Logging.Register("[Normal application exit]");
                string logs = string.Join('\n', Logging.Logs);

                File.WriteAllText($"{Assets.THIS}\\logs.txt", logs);
            }
            catch (Exception e)
            {
                try
                {
                    if (MainWindow.Instance != null)
                    {
                        CurrentMap.LoadedMap?.Save();
                        MapManager.SaveCache();
                    }
                }
                catch (Exception ex) { Logging.Register("Map(s) failed to save on abort", LogSeverity.WARN, ex); }

                Logging.Register("[Error encountered in application]", LogSeverity.ERROR);
                string logs = string.Join('\n', Logging.Logs);

                string text = @$"// whoops

{e}

|******************|
|  POSSIBLE FIXES  |
|******************|

If you do not already have it, install the Visual C++ 2015 Redistributable package. This is required for GLFW to work (which SSQE uses).

Check if all required DLL files are present and working. If not, add or replace any missing or broken ones with versions from the latest release, or reinstall.

Try updating your graphics driver to the latest version.

If none of these work or aren't applicable, report the error in the official Sound Space Discord server.

{logs}
                ";

                File.WriteAllText($"{Assets.THIS}\\crash-report.txt", text);

                DialogResult result = MessageBox.Show(@"Fatal error encountered while running this application
A crash report has been created at '*\crash-report.txt'

Would you like to report this crash on GitHub?", MBoxIcon.Error, MBoxButtons.Yes_No);

                if (result == DialogResult.Yes)
                {
                    Platform.OpenLink(Links.NEW_GITHUB_ISSUE);
                    Platform.OpenDirectory();
                }
            }

            ProgramArgs.Unwatch();
        }
    }
}