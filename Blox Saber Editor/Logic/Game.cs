using Sound_Space_Editor;
using System;
using System.Diagnostics;
using System.IO;

namespace Logic
{
    public class Game
    {
        //public static string path = @"H:\Game_Dev\BlockGame\""Block Game""\Build\""Block Game.exe""";

        static public void TryStart(string path, string MapData)
        {
            try
            {
                var data = EditorWindow.Instance.ParseData();
                string mapcurrent = Environment.CurrentDirectory + @"\temp.txt";

                if (!File.Exists(mapcurrent))
                {
                    File.Create(mapcurrent);
                }

                File.WriteAllText(mapcurrent, data);

                string[] mapsplit = mapcurrent.Split(char.Parse(@"\"));

                string MapPath = mapcurrent;//"";
                /*/
                for (int i = 0; i < mapsplit.Length; i++)
                {
                    string current = mapsplit[i];

                    bool hasSpace = current.Contains(" ");

                    if (hasSpace)
                    {
                        current = current.Insert(0, @"""");
                        current = current.Insert(current.Length, @"""");
                    }

                    Console.WriteLine(hasSpace + ":" + current);

                    if (i == mapsplit.Length - 1)
                    {
                        MapPath += current;
                        continue;
                    }

                    MapPath += current + @"\";
                }
                /*/

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
                string gamePath = newPath;// + @"""Block Game.exe""";

                if (!File.Exists(gamePath))
                {
                    new Exception("Game File Not Found!");
                }
                using (Process myProcess = new Process())
                {
                    string Arg = @"/K " + gamePath + @" -play """ + MapPath + @"""";
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
