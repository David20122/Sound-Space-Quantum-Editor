using Sound_Space_Editor;
using System;
using System.Diagnostics;
using System.IO;

namespace Logic
{
    public class Game
    {
        public static void TryStart(string path, string MapData)
        {
            try
            {
                var data = EditorWindow.Instance.ParseData();

                if (data == null)
                {
                    data = MapData;
                }

                if (data == null)
                {
                    return; // Data doesnt exist?
                }

                string mapcurrent = Environment.CurrentDirectory + @"\temp.txt";

                if (!File.Exists(mapcurrent))
                {
                    File.Create(mapcurrent);
                }

                File.WriteAllText(mapcurrent, data);

                string[] mapsplit = mapcurrent.Split(char.Parse(@"\"));

                string MapPath = mapcurrent;

                string[] split = path.Split(char.Parse(@"\"));

                string newPath = "";

                for (int i = 0; i < split.Length; i++)
                {
                    string current = split[i];

                    bool hasSpace = current.Contains(" ");

                    if (hasSpace)
                    {
                        current = current.Insert(0, @"""");
                        current = current.Insert(current.Length, @"""");
                    }

                    Console.WriteLine(hasSpace + ":" + current);

                    if (i == split.Length-1)
                    {
                        newPath += current;
                        continue;
                    }

                    newPath += current + @"\";
                }
                string gamePath = newPath;

                if (!File.Exists(gamePath))
                {
                    new Exception("Game File Not Found!");
                }
                using (Process myProcess = new Process())
                {
                    string Arg = @"/C " + gamePath + @" -play """ + MapPath + @"""";
                    Console.WriteLine(newPath + "\n" + Arg);
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.FileName = "cmd.exe";
                    myProcess.StartInfo.Arguments = Arg;
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
