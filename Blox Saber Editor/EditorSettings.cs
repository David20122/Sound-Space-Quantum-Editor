using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Sound_Space_Editor
{
    class EditorSettings
    {
        public static readonly string file = "settings.txt";
        public static bool Waveform;
        public static int EditorBGOpacity;
        public static int GridOpacity;
        public static int TrackOpacity;
        public static string Color1;
        public static string Color2;
        public static string NoteColor1;
        public static string NoteColor2;

        public static void Load()
        {
            try
            {
                foreach (string text in File.ReadAllLines(file))
                {
                    if (text.Contains("--")) 
                        continue;
                    string[] settings = text.Trim().Replace(" ", "").Split(new char[] { '=' });
                    string setting = settings[0];
                    string v = settings[1];
                    Console.WriteLine("{0}, {1}", setting, v);
                    switch (setting)
                    {
                        case "Waveform":
                            bool resA;
                            bool.TryParse(v, out resA);
                            Waveform = resA;
                            break;
                        case "EditorBGOpacity":
                            int resB;
                            int.TryParse(v, out resB);
                            EditorBGOpacity = resB;
                            break;
                        case "GridOpacity":
                            int resC;
                            int.TryParse(v, out resC);
                            GridOpacity = resC;
                            break;
                        case "TrackOpacity":
                            int resD;
                            int.TryParse(v, out resD);
                            TrackOpacity = resD;
                            break;
                        case "Color1":
                            Color1 = v;
                            break;
                        case "Color2":
                            Color2 = v;
                            break;
                        case "NoteColor1":
                            NoteColor1 = v;
                            break;
                        case "NoteColor2":
                            NoteColor2 = v;
                            break;
                    }
                }
            } 
            catch
            {
                Reset();
                Console.WriteLine("no settings.txt - loading default settings");
            }
        }

        public static void Reset()
        {
            Waveform = true;
            EditorBGOpacity = 255;
            GridOpacity = 255;
            TrackOpacity = 255;
            Color1 = "0,255,200";
            Color2 = "255,0,255";
            NoteColor1 = "255,0,255";
            NoteColor2 = "0,255,200";
        }

        public static void Save()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-- DON'T EDIT THIS, LAUNCH EDITOR AND GO IN SETTINGS INSTEAD");
            sb.AppendLine(string.Format("Waveform={0}", Waveform));
            sb.AppendLine(string.Format("EditorBGOpacity={0}", EditorBGOpacity));
            sb.AppendLine(string.Format("GridOpacity={0}", GridOpacity));
            sb.AppendLine(string.Format("TrackOpacity={0}", TrackOpacity));
            sb.AppendLine(string.Format("Color1={0}", Color1));
            sb.AppendLine(string.Format("Color2={0}", Color2));
            sb.AppendLine(string.Format("NoteColor1={0}", NoteColor1));
            sb.AppendLine(string.Format("NoteColor2={0}", NoteColor2));
            try
            {
                File.WriteAllText(file, sb.ToString());
            }
            catch
            {

            }
        }
    }
}
