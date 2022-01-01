using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using Sound_Space_Editor.Properties;

namespace Sound_Space_Editor
{
	class Program
	{

		[STAThread]
		static void Main(string[] args)
		{
			Application.SetCompatibleTextRenderingDefault(false);

			EditorWindow w;

			if (args.Length == 16)
            {
				Settings.Default.MasterVolume = decimal.Parse(args[0]);
				Settings.Default.SFXVolume = decimal.Parse(args[1]);
				Settings.Default.GridNumbers = bool.Parse(args[2]);
				Settings.Default.ApproachSquares = bool.Parse(args[3]);
				Settings.Default.AnimateBackground = bool.Parse(args[4]);
				Settings.Default.Autoplay = bool.Parse(args[5]);
				Settings.Default.BGDim = float.Parse(args[6]);
				Settings.Default.LastFile = args[7];
				Settings.Default.Quantum = bool.Parse(args[8]);
				Settings.Default.AutoAdvance = bool.Parse(args[9]);
				Settings.Default.SfxOffset = args[10];
				Settings.Default.Numpad = bool.Parse(args[11]);
				Settings.Default.QuantumGridLines = bool.Parse(args[12]);
				Settings.Default.QuantumGridSnap = bool.Parse(args[13]);
				Settings.Default.Metronome = bool.Parse(args[14]);
				Settings.Default.LegacyBPM = bool.Parse(args[15]);

			}

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