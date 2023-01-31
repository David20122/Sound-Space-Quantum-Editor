using System;
using System.Linq;
using System.Windows.Forms;
using Sound_Space_Editor.Misc;
using OpenTK;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Diagnostics;

namespace Sound_Space_Editor
{
    public partial class TimingsWindow : Form
    {
        public static TimingsWindow Instance;
        private CultureInfo culture;

        public TimingsWindow()
        {
            Instance = this;

            InitializeComponent();
            ResetList();

            culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
        }

        private void OpenTapper_Click(object sender, EventArgs e)
        {
            if (BPMTapper.Instance != null)
                BPMTapper.Instance.Close();

            new BPMTapper().Show();
        }

        public void ResetList(int index = 0)
        {
            var editor = MainWindow.Instance;

            PointList.Rows.Clear();

            foreach (var point in editor.TimingPoints)
                PointList.Rows.Add(point.bpm, point.Ms);

            if (editor.TimingPoints.Count > 0)
            {
                index = MathHelper.Clamp(index, 0, editor.TimingPoints.Count - 1);

                PointList.CurrentCell = PointList[0, index];

                var point = editor.TimingPoints[index];
                BpmBox.Value = (decimal)point.bpm;
                OffsetBox.Value = point.Ms;
            }
            else
            {
                BpmBox.Value = 0;
                OffsetBox.Value = 0;
            }
        }

        private void CurrentButton_Click(object sender, EventArgs e)
        {
            OffsetBox.Value = (decimal)Settings.settings["currentTime"].Value;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in PointList.SelectedRows)
                MainWindow.Instance.TimingPoints.RemoveAt(row.Index);

            ResetList();
        }

        private void PointList_SelectionChanged(object sender, EventArgs e)
        {
            if (PointList.SelectedRows.Count > 0)
            {
                var row = PointList.SelectedRows[0];
                var point = MainWindow.Instance.TimingPoints[row.Index];

                BpmBox.Value = (decimal)point.bpm;
                OffsetBox.Value = point.Ms;
            }
        }

        private void OnClosing(object sender, EventArgs e)
        {
            Instance = null;
        }

        private void OffsetBox_ValueChanged(object sender, EventArgs e)
        {
            OffsetBox.Value = (long)Math.Min(OffsetBox.Value, (decimal)Settings.settings["currentTime"].Max);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var editor = MainWindow.Instance;

            var exists = false;

            foreach (var point in editor.TimingPoints)
                exists = exists || point.Ms == OffsetBox.Value;

            if (!exists && BpmBox.Value > 0)
            {
                var point = new TimingPoint((float)BpmBox.Value, (long)OffsetBox.Value);

                editor.TimingPoints.Add(point);
                editor.SortTimings(false);

                ResetList(editor.TimingPoints.IndexOf(point));
            }
        }

        private void MoveButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in PointList.SelectedRows)
            {
                var point = MainWindow.Instance.TimingPoints[row.Index];

                point.Ms += (long)MoveBox.Value;
            }

            ResetList();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            var editor = MainWindow.Instance;

            if (PointList.CurrentRow != null)
            {
                var exists = false;

                var selected = editor.TimingPoints[PointList.CurrentRow.Index];

                foreach (var point in editor.TimingPoints)
                    exists = exists || (point.Ms == OffsetBox.Value && point != selected);

                if (!exists && BpmBox.Value > 0)
                {
                    selected.bpm = (float)BpmBox.Value;
                    selected.Ms = (long)OffsetBox.Value;

                    editor.SortTimings(false);

                    ResetList(editor.TimingPoints.IndexOf(selected));
                }
            }
        }

        private void OpenBeatmap_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Select Beatmap",
                Filter = "Beatmaps (*.osu; *.adofai; *.chart)|*.osu;*.chart;*.adofai"
            })
            {
                if (Settings.settings["importPath"] != "")
                    dialog.InitialDirectory = Settings.settings["importPath"];

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Settings.settings["importPath"] = Path.GetDirectoryName(dialog.FileName);

                    var file = dialog.FileName;
                    var ext = Path.GetExtension(file);
                    var data = File.ReadAllText(file);

                    switch (ext)
                    {
                        case ".osu":
                            ParseOSU(data);
                            break;
                        case ".adofai":
                            ParseADOFAI(data);
                            break;
                        case ".chart":
                            ParseCH(data);
                            break;
                    }
                }
            }
        }

        private void ImportOSU_Click(object sender, EventArgs e)
        {
            ParseOSU(Clipboard.GetText());
        }

        private void ImportADOFAI_Click(object sender, EventArgs e)
        {
            ParseADOFAI(Clipboard.GetText());
        }

        private void ImportCH_Click(object sender, EventArgs e)
        {
            ParseCH(Clipboard.GetText());
        }

        private readonly Dictionary<char, int> Degrees = new Dictionary<char, int>
        {
            {'R', 0 }, {'J', 30 }, {'E', 45 }, {'T', 60 }, {'U', 90 }, {'G', 120 }, {'Q', 135 }, {'H', 150 }, {'L', 180 }, {'N', 210 }, {'Z', 225 }, {'F', 240 },
            {'D', 270 }, {'B', 300 }, {'C', 315 }, {'M', 330 }, {'x', 195 }, {'W', 165 }, {'A', 345 }, {'p', 15 }, {'q', 105 }, {'Y', 285 }, {'o', 75 }, {'V', 255 },
        };

        private int GetDegree(char c)
        {
            if (!Degrees.TryGetValue(c, out int degree))
                degree = 69;

            return degree;
        }

        //i really dont want to rewrite these so im just copying them from the old code

        //i may end up improving it later but it works for the time being

        private void ParseOSU(string data)
        {
            try
            {
                int rep;
                string reps;
                if (data.Contains("TimingPoints") == true)
                {
                    rep = data.IndexOf("TimingPoints");
                    reps = data.Substring(0, rep + 13);
                    data = data.Replace(reps, "");
                    rep = data.IndexOf("[");
                    data = data.Substring(0, rep);
                }
                string[] newdata = data.Split('\n');
                var cleared = false;
                foreach (var line in newdata)
                {
                    if (line.Contains(","))
                    {
                        if (!cleared)
                        {
                            MainWindow.Instance.TimingPoints.Clear();
                            cleared = true;
                        }
                        string[] items = line.Split(',');
                        var time = long.Parse(items[0], culture);
                        var bpm = Math.Abs(Math.Round(60000 / double.Parse(items[1], culture), 5));
                        if (bpm > 0)
                        {
                            MainWindow.Instance.TimingPoints.Add(new TimingPoint((float)bpm, time));
                        }
                    }
                }
                ResetList();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(st.FrameCount - 1);
                var line = frame.GetFileLineNumber();
                MessageBox.Show($"Failed to parse beatmap\n[OSU | {ex.GetType().Name} | {line}]", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResetList();
            }
        }

        private void ParseADOFAI(string data)
        {
            try
            {
                string curbpm = data.Substring(data.IndexOf("bpm"), data.Length - data.IndexOf("bpm"));
                string mapdata = data.Substring(data.IndexOf("pathData"), data.IndexOf(",") - data.IndexOf("pathData"));
                mapdata = mapdata.Replace("pathData\": \"", "");
                mapdata = mapdata.Replace("\"", "");
                string offset = data.Substring(data.IndexOf("offset"), data.Length - data.IndexOf("offset"));
                offset = offset.Substring(0, offset.IndexOf(","));
                offset = offset.Replace("offset\": ", "");
                double offsetnum = double.Parse(offset, culture);
                double currentbpm = double.Parse(curbpm.Substring(6, curbpm.IndexOf(",") - 6), culture);
                double initbpm = currentbpm;
                data = data.Substring(data.IndexOf("\"actions\":"), data.Length - data.IndexOf("\"actions\":"));
                data = data.Replace("\"actions\":\n", "");
                if (data.Substring(0, 1) == "\t")
                {
                    data = data.Substring(1, data.Length - 1);
                }
                data = data.Substring(2, data.Length - 2);
                data = data.Substring(0, data.IndexOf("]\n"));
                if (data.Substring(data.Length - 1, 1) == "\t")
                {
                    data = data.Substring(0, data.Length - 1);
                }
                data = data.Substring(0, data.Length - 1);
                string[] newdata = data.Split('\n');
                List<int> bpmnotes = new List<int>();
                List<int> twirlnotes = new List<int>();
                List<double> notetimes = new List<double>();
                List<double> bpms = new List<double>();
                List<double> multipliers = new List<double>();
                List<int> multiplierindexes = new List<int>();
                for (int i = 0; i < newdata.Count(); i++)
                {
                    string line = newdata[i];
                    if (line.Contains("SetSpeed"))
                    {
                        string liner = line.Substring(line.IndexOf(":"), line.Length - line.IndexOf(":"));
                        liner = liner.Substring(2, liner.IndexOf(',') - 2);
                        bpmnotes.Add(int.Parse(liner, culture));
                        if (line.Contains("speedType\": \"Multiplier"))
                        {
                            string mult = line.Substring(line.IndexOf("bpmMultiplier"), line.Length - line.IndexOf("bpmMultiplier"));
                            mult = mult.Replace("bpmMultiplier\": ", "");
                            mult = mult.Replace(" }", "");
                            mult = mult.Replace(",", "");
                            currentbpm *= double.Parse(mult, culture);
                            bpms.Add(currentbpm);
                        }
                        else
                        {
                            string bpm = line.Substring(line.IndexOf("beatsPerMinute"), line.IndexOf("bpmMultiplier") - line.IndexOf("beatsPerMinute"));
                            bpm = bpm.Replace("beatsPerMinute\": ", "");
                            bpm = bpm.Replace(", \"", "");
                            currentbpm = double.Parse(bpm, culture);
                            bpms.Add(currentbpm);
                        }
                    }
                    else if (line.Contains("Twirl"))
                    {
                        string liner = line.Substring(line.IndexOf(":"), line.Length - line.IndexOf(":"));
                        liner = liner.Substring(2, liner.IndexOf(',') - 2);
                        twirlnotes.Add(int.Parse(liner, culture));
                    }
                }
                bool clock = true;
                multipliers.Add((180 - GetDegree(mapdata[0])) / 180);
                multiplierindexes.Add(0);
                for (int i = 1; i < mapdata.Length; i++)
                {
                    char prevnote = mapdata[i - 1];
                    if (prevnote != '!')
                    {
                        char nextnote = mapdata[i];
                        if (nextnote == '!')
                        {
                            nextnote = mapdata[i + 1];
                        }
                        if (twirlnotes.Contains(i))
                        {
                            clock = !clock;
                        }
                        double prevangle = GetDegree(prevnote) + 180;
                        if (mapdata[i] == '!')
                        {
                            prevangle -= 180;
                        }
                        if (prevangle > 360)
                        {
                            prevangle -= 360;
                        }
                        double nextangle = GetDegree(nextnote);
                        double angle = prevangle - nextangle;
                        if (angle <= 0)
                        {
                            angle += 360;
                        }
                        if (!clock)
                        {
                            angle = 360 - angle;
                        }
                        if (angle <= 0)
                        {
                            angle += 360;
                        }
                        angle /= 180;
                        multipliers.Add(angle);
                        multiplierindexes.Add(i);
                    }

                }
                double time = multipliers[0] * offsetnum;
                double bpmd = initbpm;
                double prevbpmd = initbpm;
                int prevbpmdindex = 0;
                if (multipliers.Count > 0)
                    MainWindow.Instance.TimingPoints.Clear();
                for (int i = 0; i < multipliers.Count; i++)
                {
                    int bpmindex = multiplierindexes[i];
                    if (bpmnotes.Contains(bpmindex))
                    {
                        bpmd = bpms[bpmnotes.IndexOf(bpmindex)];
                    }
                    if (i > 0)
                    {
                        time += 60000 / bpmd * multipliers[i];
                    }
                    if (prevbpmd != bpmd && !(prevbpmd % bpmd == 0 || bpmd % prevbpmd == 0 || bpmd * multipliers[i] % prevbpmd * multipliers[prevbpmdindex] == 0 || prevbpmd * multipliers[prevbpmdindex] % bpmd * multipliers[i] == 0))
                    {
                        MainWindow.Instance.TimingPoints.Add(new TimingPoint((float)bpmd, (long)time));
                        prevbpmd = bpmd;
                        prevbpmdindex = i;
                    }
                }
                ResetList();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(st.FrameCount - 1);
                var line = frame.GetFileLineNumber();
                MessageBox.Show($"Failed to parse beatmap\n[ADOFAI | {ex.GetType().Name} | {line}]", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResetList();
            }
        }

        private void ParseCH(string data)
        {
            try
            {
                int rep;
                string reps;
                string reps2;
                decimal resolution;
                decimal bpm;
                decimal bpmTime;
                decimal difference;
                decimal diffFinal;
                int listIndex = 0;
                rep = data.IndexOf("Resolution");
                reps = data.Substring(0, rep);
                reps2 = data.Replace(reps, "");
                rep = reps2.IndexOf("\n");
                reps2 = reps2.Substring(0, rep);
                reps2 = reps2.Replace("Resolution = ", "");
                resolution = decimal.Parse(reps2, culture);
                rep = data.IndexOf("SyncTrack");
                reps = data.Substring(0, rep);
                reps2 = data.Replace(reps, "");
                rep = reps2.IndexOf("}");
                reps2 = reps2.Substring(0, rep);
                reps2 = reps2.Replace("SyncTrack]\n{", "");
                string[] bpmArray = reps2.Split('\n');
                List<string> bpmList = new List<string>();
                List<string> timeList = new List<string>();
                List<string> msList = new List<string>();
                foreach (var line in bpmArray)
                {
                    if (line.Contains("B"))
                    {
                        rep = line.IndexOf(" B ");
                        reps = line.Substring(0, rep);
                        reps2 = line.Replace(reps, "");
                        reps2 = reps2.Replace(" B ", "");
                        bpm = decimal.Parse(reps2, culture);
                        bpm /= 1000;
                        bpmList.Add(bpm.ToString());
                        reps = reps.Replace(" =", "");
                        bpmTime = decimal.Parse(reps, culture);
                        timeList.Add(bpmTime.ToString());
                    }
                }
                foreach (var item in timeList)
                {
                    listIndex = timeList.IndexOf(item);
                    if (listIndex != 0)
                    {
                        difference = decimal.Parse(item, culture) - decimal.Parse(timeList[listIndex - 1], culture);
                        bpm = decimal.Parse(bpmList[listIndex - 1], culture);
                        difference = Math.Round(1000 * (difference / (bpm * resolution / 60)), 2);
                        diffFinal = difference + decimal.Parse(msList[listIndex - 1], culture);
                        msList.Add(diffFinal.ToString());
                    }
                    else
                    {
                        difference = decimal.Parse(timeList[listIndex], culture);
                        msList.Add(timeList[listIndex]);
                    }
                }
                if (msList.Count > 0)
                    MainWindow.Instance.TimingPoints.Clear();
                foreach (var item in msList)
                {
                    listIndex = msList.IndexOf(item);
                    MainWindow.Instance.TimingPoints.Add(new TimingPoint(float.Parse(bpmList[listIndex]), (long)float.Parse(item)));
                }
                ResetList();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(st.FrameCount - 1);
                var line = frame.GetFileLineNumber();
                MessageBox.Show($"Failed to parse beatmap\n[CH | {ex.GetType().Name} | {line}]", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResetList();
            }
        }
    }
}
