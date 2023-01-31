using System;
using System.Windows.Forms;

namespace Sound_Space_Editor
{
	class Program
	{
        static Program()
        {
            try
            {
                Start();
            }
            catch (Exception ex)
            {
                var lines = new string[]
                {
                    "A fatal error has occurred while attempting to run this application:",
                    ex.Message,
                    ex.StackTrace,
                    "The following lines are possible methods you can try to resolve this error *IF* it occurred on load:",
                    "Check if you are running this application in a zipped folder. If so, please extract the entire directory before attempting to run the editor.",
                    "If you are missing a DLL file from the main directory, copy it from the latest release of the editor into the current directory to ensure all required files are present.",
                    "If a missing DLL error is thrown but the main directory contains said file, try replacing it with the file from the latest release with the same name so all mentioned files are up to date.",
                    "Try updating your graphics driver to the latest version if none of the previous solutions apply to your situation.",
                    "If none of these work or this error was thrown while the editor was already running, report the bug in the official Sound Space Discord server to attempt to resolve the issue if possible."
                };

                MessageBox.Show(string.Join("\n\n", lines), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [STAThread]
        static void Start()
        {
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow window = new MainWindow();

            using (window)
                window.Run();
        }

        [STAThread]
        static void Main()
        {

        }
    }
}