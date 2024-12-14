using New_SSQE.GUI;
using New_SSQE.Objects.Managers;
using New_SSQE.Objects;
using New_SSQE.Preferences;
using OpenTK.Mathematics;
using System.Numerics;
using System.Drawing;

namespace New_SSQE.Maps
{
    internal static class Patterns
    {
        public static void StorePattern(int index)
        {
            string pattern = "";
            long minDist = 0;

            for (int i = 0; i + 1 < CurrentMap.Notes.Selected.Count; i++)
            {
                long dist = Math.Abs(CurrentMap.Notes.Selected[i].Ms - CurrentMap.Notes.Selected[i + 1].Ms);

                if (dist > 0)
                    minDist = minDist > 0 ? Math.Min(minDist, dist) : dist;
            }

            foreach (Note note in CurrentMap.Notes.Selected)
            {
                long offset = CurrentMap.Notes.Selected[0].Ms;

                string x = note.X.ToString(Program.Culture);
                string y = note.Y.ToString(Program.Culture);
                string time = (minDist > 0 ? Math.Round((double)(note.Ms - offset) / minDist) : 0).ToString();

                pattern += $",{x}|{y}|{time}";
            }

            if (pattern.Length > 0)
                pattern = pattern[1..];

            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                editor.ShowToast($"BOUND PATTERN {index}", Settings.color1.Value);

            Settings.patterns.Value[index] = pattern;
        }

        public static void ClearPattern(int index)
        {
            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
            {
                Settings.patterns.Value[index] = "";
                editor.ShowToast($"UNBOUND PATTERN {index}", Settings.color1.Value);
            }
        }

        public static void RecallPattern(int index)
        {
            string pattern = Settings.patterns.Value[index];

            if (pattern == "")
                return;

            string[] patternSplit = pattern.Split(',');
            List<Note> toAdd = new();

            foreach (string note in patternSplit)
            {
                string[] noteSplit = note.Split('|');
                float x = float.Parse(noteSplit[0], Program.Culture);
                float y = float.Parse(noteSplit[1], Program.Culture);
                int time = int.Parse(noteSplit[2]);
                long ms = Timing.GetClosestBeatScroll((long)Settings.currentTime.Value.Value, false, time);

                toAdd.Add(new(x, y, ms));
            }

            NoteManager.Add("ADD PATTERN", toAdd);
        }



        public static void HorizontalFlip(Note note)
        {
            note.X = 2 - note.X;
        }

        public static void VerticalFlip(Note note)
        {
            note.Y = 2 - note.Y;
        }

        public static void Rotate(Note note, float deg)
        {
            double angle = MathHelper.RadiansToDegrees(Math.Atan2(note.Y - 1, note.X - 1));
            double distance = Math.Sqrt(Math.Pow(note.X - 1, 2) + Math.Pow(note.Y - 1, 2));
            double anglef = MathHelper.DegreesToRadians(angle + deg);

            note.X = (float)(Math.Cos(anglef) * distance + 1);
            note.Y = (float)(Math.Sin(anglef) * distance + 1);

            if (Settings.clampSR.Value)
            {
                note.X = Math.Clamp(note.X, -0.85f, 2.85f);
                note.Y = Math.Clamp(note.Y, -0.85f, 2.85f);
            }
        }

        public static void Scale(Note note, float scale)
        {
            float scalef = scale / 100f;

            note.X = (note.X - 1) * scalef + 1;
            note.Y = (note.Y - 1) * scalef + 1;

            if (Settings.clampSR.Value)
            {
                note.X = Math.Clamp(note.X, -0.85f, 2.85f);
                note.Y = Math.Clamp(note.Y, -0.85f, 2.85f);
            }
        }



        private static BigInteger FactorialApprox(int k)
        {
            BigInteger result = new(1);

            if (k < 10)
                for (int i = 1; i <= k; i++)
                    result *= i;
            else
                result = (BigInteger)(Math.Sqrt(2 * Math.PI * k) * Math.Pow(k / Math.E, k));

            return result;
        }

        private static BigInteger BinomialCoefficient(int k, int v)
        {
            return FactorialApprox(k) / (FactorialApprox(v) * FactorialApprox(k - v));
        }

        public static List<Note> DrawBezier(List<Note> nodes, int divisor)
        {
            List<Note> final = new();

            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
            {
                int k = nodes.Count - 1;
                decimal tdiff = nodes[k].Ms - nodes[0].Ms;
                decimal d = 1m / (divisor * k);

                if (Settings.curveBezier.Value)
                {
                    for (decimal t = 0; t <= 1 + d / 2m; t += d)
                    {
                        float xf = 0;
                        float yf = 0;
                        decimal tf = nodes[0].Ms + tdiff * t;

                        for (int v = 0; v <= k; v++)
                        {
                            Note note = nodes[v];
                            double bez = (double)BinomialCoefficient(k, v) * (Math.Pow(1 - (double)t, k - v) * Math.Pow((double)t, v));

                            xf += (float)(bez * note.X);
                            yf += (float)(bez * note.Y);
                        }

                        final.Add(new(xf, yf, (long)tf));
                    }
                }
                else
                {
                    d = 1m / divisor;

                    for (int v = 0; v < k; v++)
                    {
                        Note note = nodes[v];
                        Note nextnote = nodes[v + 1];
                        decimal xdist = (decimal)(nextnote.X - note.X);
                        decimal ydist = (decimal)(nextnote.Y - note.Y);
                        decimal tdist = nextnote.Ms - note.Ms;

                        for (decimal t = 0; t < 1 + d / 2m; t += d)
                        {
                            if (t > 0)
                            {
                                decimal xf = (decimal)note.X + xdist * t;
                                decimal yf = (decimal)note.Y + ydist * t;
                                decimal tf = note.Ms + tdist * t;

                                final.Add(new((float)xf, (float)yf, (long)tf));
                            }
                        }
                    }
                }
            }

            return final;
        }

        public static void RunBezier()
        {
            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
            {
                int divisor = (int)(Settings.bezierDivisor.Value + 0.5f);

                if (divisor > 0 && ((CurrentMap.BezierNodes != null && CurrentMap.BezierNodes.Count > 1) || CurrentMap.Notes.Selected.Count > 1))
                {
                    bool success = true;
                    List<Note> finalNodes = CurrentMap.BezierNodes != null && CurrentMap.BezierNodes.Count > 1 ? CurrentMap.BezierNodes.ToList() : CurrentMap.Notes.Selected.ToList();
                    List<Note> finalNotes = new();

                    List<int> anchored = new() { 0 };

                    for (int i = 0; i < finalNodes.Count; i++)
                        if (finalNodes[i].Anchored && !anchored.Contains(i))
                            anchored.Add(i);

                    if (!anchored.Contains(finalNodes.Count - 1))
                        anchored.Add(finalNodes.Count - 1);

                    for (int i = 1; i < anchored.Count; i++)
                    {
                        List<Note> newNodes = new();

                        for (int j = anchored[i - 1]; j <= anchored[i]; j++)
                            newNodes.Add(finalNodes[j]);

                        try
                        {
                            List<Note> finalbez = DrawBezier(newNodes, divisor);
                            success = finalbez.Count > 0;

                            if (success)
                                for (int j = 1; j < finalbez.Count; j++)
                                    finalNotes.Add(finalbez[j]);
                        }
                        catch (OverflowException)
                        {
                            editor.ShowToast("TOO MANY NODES", Color.FromArgb(255, 255, 200, 0));
                            return;
                        }
                        catch
                        {
                            editor.ShowToast("FAILED TO DRAW BEZIER", Color.FromArgb(255, 255, 200, 0));
                            return;
                        }
                    }

                    CurrentMap.Notes.ClearSelection();
                    CurrentMap.SelectedPoint = null;

                    finalNotes.Add(finalNodes[0]);

                    if (success)
                        NoteManager.Replace("DRAW BEZIER", finalNodes, finalNotes);
                }

                foreach (Note note in CurrentMap.Notes)
                    note.Anchored = false;

                CurrentMap.BezierNodes?.Clear();
            }
        }
    }
}
