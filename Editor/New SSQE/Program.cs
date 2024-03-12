using Avalonia;
using System;
using System.IO;

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
                catch { }

                ActionLogging.Register("[Error encountered in application]", "ERROR");
                var logs = string.Join('\n', ActionLogging.Logs);

                Exception? ex = e;

                var emsg = "";

                while (ex != null)
                {
                    emsg += $"\n\n{e.Message}\n\n{e.StackTrace ?? "[StackTrace was null]"}";
                    ex = ex.InnerException;
                }

                var text = @$"// whoops{emsg}

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

                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                
                MessageBox.Show("A fatal error has occurred while attempting to run this application.\n\nA crash report has been created at '*\\crash-report.txt'", "Error", "OK");
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