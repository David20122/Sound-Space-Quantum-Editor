using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;

namespace Sound_Space_Editor
{
	internal class Program
	{
		static bool ModsLoaded = false;
		static void MODLOADER()
		{
			if (ModsLoaded)
			{
				return; // Cant reload as of right now
            }
			
			string path = Directory.GetCurrentDirectory();

			//MessageBox.Show(path, "DEBUG", MessageBoxButtons.OK, MessageBoxIcon.Warning);

			if (Directory.Exists(path + @"\mods"))
			{
				var Files = Directory.GetFiles(path + @"\mods");
				//MessageBox.Show(Files.Length.ToString(), "DEBUG", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				foreach (string cfile in Files)
				{
					FileInfo mFile = new FileInfo(cfile);
					//MessageBox.Show(mFile.FullName, "DEBUG", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					bool isMod = mFile.Extension.Contains("dll");

					if (isMod)
					{
						try
						{
                            var DLL = Assembly.LoadFile(mFile.FullName);

							foreach (Type type in DLL.GetExportedTypes())
							{
								var modDll = Activator.CreateInstance(type);
								try
								{
									type.InvokeMember("Main", BindingFlags.InvokeMethod, null, modDll, new object[] { });
								}
								catch (Exception e)
								{
									MessageBox.Show(e.ToString(), "Mod Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
							}
						}
						catch (Exception e)
                        {
							MessageBox.Show(e.ToString(), "Mod Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}
			else
			{
				Directory.CreateDirectory(path + @"\mods");
			}

			ModsLoaded = true;
		}
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

			MODLOADER();

			using (w)
			{
				w.Run();
			}

			INativeWindow window = new OpenTK.NativeWindow(1080, 600, "Timings Setup", GameWindowFlags.Default, new GraphicsMode(32, 8, 0, 8), DisplayDevice.Default);
		}
	}
}