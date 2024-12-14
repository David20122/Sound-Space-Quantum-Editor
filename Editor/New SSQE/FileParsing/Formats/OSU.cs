using New_SSQE.Maps;
using New_SSQE.Misc.Static;
using New_SSQE.Objects;
using System.Globalization;

namespace New_SSQE.FileParsing.Formats
{
    internal class OSU
    {
        public static string Parse(string path)
        {
            List<TimingPoint> points = CurrentMap.TimingPoints;

            string data = File.ReadAllText(path);
            string id = "";

            string[] split = data.Split("\n");

            bool timing = false;
            bool hitObj = false;

            List<string> notes = new();

            for (int i = 0; i < split.Length; i++)
            {
                string line = split[i].Trim();

                try
                {
                    string[] subsplit = line.Split(":");
                    string[] set = line.Split(",");

                    if (!timing && !hitObj && subsplit.FirstOrDefault() == "AudioFilename")
                    {
                        string idPath = subsplit[1].Trim();
                        id = Path.GetFileNameWithoutExtension(idPath);

                        File.Copy($"{Path.GetDirectoryName(path)}\\{idPath}", $"{Assets.CACHED}\\{id}.asset", true);
                    }

                    if (timing && !string.IsNullOrWhiteSpace(line))
                    {
                        bool canParse = double.TryParse(set[0], NumberStyles.Any, Program.Culture, out double time);
                        canParse &= float.TryParse(set[1], NumberStyles.Any, Program.Culture, out float bpm);

                        if (canParse)
                        {
                            bool inhereted = set.Length > 6 ? set[6] == "1" : bpm > 0;

                            bpm = (float)Math.Abs(Math.Round(60000 / bpm, 3));

                            if (bpm > 0 && inhereted)
                                points.Add(new(bpm, (long)time));
                        }
                    }

                    if (hitObj && !string.IsNullOrWhiteSpace(line))
                    {
                        bool canParse = float.TryParse(set[0], NumberStyles.Any, Program.Culture, out float x);
                        canParse &= float.TryParse(set[1], NumberStyles.Any, Program.Culture, out float y);
                        canParse &= double.TryParse(set[2], NumberStyles.Any, Program.Culture, out double time);
                        canParse &= int.TryParse(set[3], NumberStyles.Any, Program.Culture, out int type);

                        if (canParse)
                        {
                            x = 5 - x / 64;
                            y = 4 - y / 64;

                            if ((type & 1) != 0)
                                notes.Add($",{Math.Round(x, 2).ToString(Program.Culture)}|{Math.Round(y, 2).ToString(Program.Culture)}|{(long)time}");
                        }
                    }
                }
                catch { }

                timing = line != "[HitObjects]" && (timing || line == "[TimingPoints]");
                hitObj = line != "[TimingPoints]" && (hitObj || line == "[HitObjects]");
            }

            return id + string.Join("", notes);
        }
    }
}
