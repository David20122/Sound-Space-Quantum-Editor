using Avalonia;

namespace New_SSQE
{
    internal class Program
    {
        static Program()
        {
            try
            {
                Start();

                ActionLogging.Register("[Normal application exit]");
                var logs = string.Join('\n', ActionLogging.Logs);

                File.WriteAllText("logs.txt", logs);
            }
            catch (Exception e)
            {
                try
                {
                    MainWindow.Instance.CurrentMap?.Save();
                    MainWindow.Instance.CacheMaps();
                }
                catch (Exception ex) { ActionLogging.Register("Map(s) failed to save on abort", "WARN", ex); }

                ActionLogging.Register("[Error encountered in application]", "ERROR");
                var logs = string.Join('\n', ActionLogging.Logs);

                var text = @$"// whoops

{e}

|******************|
|  POSSIBLE FIXES  |
|******************|

Ensure this application is not running inside a zipped folder. Extract the directory if so.

Check if all required DLL files are present and working. If not, add or replace any missing or broken ones with versions from the latest release.

Try updating your graphics driver to the latest version.

If none of these work or aren't applicable, report the error in the official Sound Space Discord server.

{logs}
                ";

                File.WriteAllText("crash-report.txt", text);

                MessageBox.Show("Fatal error encountered while running this application\n\nA crash report has been created at '*\\crash-report.txt'", "Error", "OK");
            }
        }

        static void Start()
        {
            BuildAvaloniaApp();

            using (var window = new MainWindow())
                window.Run();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .SetupWithoutStarting();

        
        static void Main()
        {
            
        }
    }
}