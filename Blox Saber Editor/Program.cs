using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;

namespace Sound_Space_Editor
{
	class Program
	{

		[STAThread]
		static void Main(string[] args)
		{
			Application.SetCompatibleTextRenderingDefault(false);

			EditorWindow w;

			try
			{
				long offset = 0;

				var launcherDir = Environment.CurrentDirectory;
				if (args.Length > 0)
                {
					launcherDir = "";
					var i = 0;
					foreach (string argument in args.ToList<string>())
                    {
						i++;
						if (i == 1)
                        {
							launcherDir = argument;
                        } else
                        {
							launcherDir += " " + argument;
                        }
                    }
                }

				w = new EditorWindow(offset, launcherDir);
			}
			catch(Exception e)
			{
				MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			using (w)
			{
				w.Run();
			}

			INativeWindow window = new OpenTK.NativeWindow(1080, 600, "Timings Setup", GameWindowFlags.Default, new GraphicsMode(32, 8, 0, 8), DisplayDevice.Default);

			
		}
	}
}