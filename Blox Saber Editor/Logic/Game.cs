using System;
using System.Diagnostics;

namespace Logic
{
public class Game()
{
public void TryStart(string MapData)
{

try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.FileName = "path.to.file";
                    myProcess.StartInfo.CreateNoWindow = false;
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
