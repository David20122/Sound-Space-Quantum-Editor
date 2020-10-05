using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;
using System.IO.Compression;
using System.ComponentModel;

namespace QE_Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Quantum Editor Launcher v1.0.0";
            void slash()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("/");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");
                Console.ForegroundColor = ConsoleColor.White;
            }
            void question()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("?");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");
                Console.ForegroundColor = ConsoleColor.White;
            }
            void plus()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("+");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");
                Console.ForegroundColor = ConsoleColor.White;
            }
            void warning()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("!");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");
                Console.ForegroundColor = ConsoleColor.White;
            }
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadString("https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/master/versions/ver");
            } 
            catch
            {
                warning();
                Console.WriteLine(" No internet connection.");
                warning();
                Console.WriteLine(" Closing in 2 seconds.");
                Thread.Sleep(2000);
                Environment.Exit(0);
            }
            var reply = wc.DownloadString("https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/master/versions/ver");
            string path = Directory.GetCurrentDirectory();
            if (Directory.Exists("Quantum Editor " + reply))
            {
                slash();
                Console.WriteLine(" Quantum Editor already up-to-date!");
                slash();
                Console.WriteLine(" Closing in 3 seconds..");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
            else
            {
                slash();
                Console.WriteLine(" Didn't find Quantum Editor in launcher directory or there is a new update available.");
                question();
                Console.WriteLine(" Do you want to download latest version? (y/n): ");
                string response = Console.ReadLine();
                if (response == "y")
                {
                    slash();
                    Console.WriteLine(" Downloading version " + reply);
                    wc.DownloadFile("https://david20122.github.io/versions/" + reply + ".zip", reply + ".zip");
                    plus();
                    Console.WriteLine(" Downloaded!");
                    slash();
                    Console.WriteLine(" Extracting the zip file..");
                    ZipFile.ExtractToDirectory(reply + ".zip", path);
                    slash();
                    Console.WriteLine(" Deleting the zip file..");
                    File.Delete(reply + ".zip");
                    Console.Clear();
                    plus();
                    Console.WriteLine(" Quantum Editor " + reply + " successfully installed!");
                    slash();
                    Console.WriteLine(" Closing in 3 seconds..");
                    Thread.Sleep(3000);
                    Environment.Exit(0);
                }
                else
                {
                    slash();
                    Console.WriteLine(" Alright then.");
                    slash();
                    Console.WriteLine(" Closing in 2 seconds.");
                    Thread.Sleep(2000);
                    Environment.Exit(0);

                }
            }
            Console.ReadLine();
        }
    }
}
