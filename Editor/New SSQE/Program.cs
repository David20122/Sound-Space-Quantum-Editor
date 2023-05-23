using Avalonia;
using System;
using System.IO;
using System.Runtime;

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

                var lines = new string[]
                {
                    logs,
                    "If you see any lines that may have caused undesired actions or bugs within the editor, please submit them as a bug report"
                };

                File.WriteAllText("logs.txt", logs);
            }
            catch (Exception e)
            {
                ActionLogging.Register("[Error encountered in application]", "ERROR");
                var logs = string.Join('\n', ActionLogging.Logs);

                var text = @$"// whoops

{logs}

{e.Message}

{e.StackTrace ?? "[StackTrace was null]"}

|******************|
|  POSSIBLE FIXES  |
|******************|

Check if you are running this application in a zipped folder. If so, please extract the entire directory before attempting to run the editor.

If you are missing a DLL file from the main directory, copy it from the latest release of the editor into the current directory to ensure all required files are present.
If a missing DLL error is thrown but the main directory contains said file, try replacing it with the file from the latest release with the same name so all mentioned files are up to date.

Try updating your graphics driver to the latest version if none of the previous solutions apply to your situation.

If none of these work or this error was thrown while the editor was already running, report the error in the official Sound Space Discord server to attempt to resolve the issue if possible.
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