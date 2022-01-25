using System;
using System.Linq;
using System.Windows.Forms;
using Sound_Space_Editor.Gui;
using OpenTK;
using System.Collections.Generic;
using System.Globalization;

namespace Sound_Space_Editor
{
    public partial class TimingsWindow : Form
    {
        public static TimingsWindow inst;
        public TimingsWindow()
        {
            inst = this;
            InitializeComponent();
            ResetList(0);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(BPMBox.Text, out float BPM) && BPM > 5000)
                BPMBox.Text = "5000";
            if (long.TryParse(OffsetBox.Text, out long Offset) && Offset > EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds)
                OffsetBox.Text = EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds.ToString();
            if (float.TryParse(BPMBox.Text, out float bpm) && bpm > 33 && long.TryParse(OffsetBox.Text, out long offset))
            {
                var exists = false;
                foreach (var point in GuiTrack.BPMs)
                {
                    if (point.Ms == offset)
                        exists = true;
                }
                if (!exists)
                {
                    var point = new BPM(bpm, offset);
                    GuiTrack.BPMs.Add(point);
                    GuiTrack.BPMs = GuiTrack.BPMs.OrderBy(o => o.Ms).ToList();
                    ResetList(GuiTrack.BPMs.IndexOf(point));
                }
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in PointList.SelectedRows)
            {
                GuiTrack.BPMs.RemoveAt(row.Index);
                GuiTrack.BPMs = GuiTrack.BPMs.OrderBy(o => o.Ms).ToList();
            }
            ResetList(PointList.CurrentRow.Index - 1);
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(BPMBox.Text, out float BPM) && BPM > 5000)
                BPMBox.Text = "5000";
            if (long.TryParse(OffsetBox.Text, out long Offset) && Offset > EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds)
                OffsetBox.Text = EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds.ToString();
            if (PointList.SelectedRows.Count > 0)
            {
                if (float.TryParse(BPMBox.Text, out float bpm) && bpm > 33 && long.TryParse(OffsetBox.Text, out long offset))
                {
                    var selectedpoint = GuiTrack.BPMs[PointList.CurrentRow.Index];
                    var exists = false;
                    foreach (var point in GuiTrack.BPMs)
                    {
                        if (point.Ms == offset && point != selectedpoint)
                            exists = true;
                    }
                    if (!exists)
                    {
                        selectedpoint.bpm = bpm;
                        selectedpoint.Ms = offset;
                        GuiTrack.BPMs = GuiTrack.BPMs.OrderBy(o => o.Ms).ToList();
                        ResetList(GuiTrack.BPMs.IndexOf(selectedpoint));
                    }
                }
            }
    }

    private void CurrentButton_Click(object sender, EventArgs e)
    {
      OffsetBox.Text = EditorWindow.Instance.MusicPlayer.CurrentTime.TotalMilliseconds.ToString();
    }

    public void ResetList(int index)
        {
            PointList.Rows.Clear();
            foreach (var point in GuiTrack.BPMs)
            {
                PointList.Rows.Add(point.bpm, point.Ms);
            }
            if (GuiTrack.BPMs.Count > 0)
            {
                index = MathHelper.Clamp(index, 0, GuiTrack.BPMs.Count - 1);
                PointList.CurrentCell = PointList[0, index];
                var tpoint = GuiTrack.BPMs[index];
                BPMBox.Text = tpoint.bpm.ToString();
                OffsetBox.Text = tpoint.Ms.ToString();
            }
            else
            {
                BPMBox.Text = "0";
                OffsetBox.Text = "0";
            }
        }

        private void PointList_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in PointList.SelectedRows)
            {
                var index = row.Index;
                var point = GuiTrack.BPMs[index];
                BPMBox.Text = point.bpm.ToString();
                OffsetBox.Text = point.Ms.ToString();
            }
        }

        private void MoveButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in PointList.SelectedRows)
            {
                var index = row.Index;
                var point = GuiTrack.BPMs[index];
                point.Ms += (long)MoveBox.Value;
            }
            ResetList(0);
        }

        private void ImportOSU_Click(object sender, EventArgs e)
        {
            try
            {
                var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                culture.NumberFormat.NumberDecimalSeparator = ".";
                string data = Clipboard.GetText();
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
                            GuiTrack.BPMs.Clear();
                            cleared = true;
                        }
                        string[] items = line.Split(',');
                        var time = long.Parse(items[0]);
                        var bpm = Math.Round(60000 / double.Parse(items[1]), 5);
                        if (bpm > 0)
                        {
                            GuiTrack.BPMs.Add(new BPM((float)bpm, time));
                        }
                    }
                }
                ResetList(0);
            }
            catch
            {

            }
        }

        private void ImportCH_Click(object sender, EventArgs e)
        {
            try
            {
                var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                culture.NumberFormat.NumberDecimalSeparator = ".";
                string data = Clipboard.GetText();
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
                resolution = decimal.Parse(reps2);
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
                        difference = decimal.Parse(item) - decimal.Parse(timeList[listIndex - 1]);
                        bpm = decimal.Parse(bpmList[listIndex - 1]);
                        difference = Math.Round(1000 * (difference / (bpm * resolution / 60)), 2);
                        diffFinal = difference + decimal.Parse(msList[listIndex - 1]);
                        msList.Add(diffFinal.ToString());
                    }
                    else
                    {
                        difference = decimal.Parse(timeList[listIndex]);
                        msList.Add(timeList[listIndex]);
                    }
                }
                if (msList.Count > 0)
                    GuiTrack.BPMs.Clear();
                foreach (var item in msList)
                {
                    listIndex = msList.IndexOf(item);
                    GuiTrack.BPMs.Add(new BPM(float.Parse(bpmList[listIndex]), long.Parse(item)));
                }
                ResetList(0);
            }
            catch
            {

            }
        }

        public double Deg(char c)
        {
            double degree = 69;

            List<char> characters = new List<char>()
            {
                'R','J','E','T','U','G','Q','H','L','N','Z','F','D','B','C','M','x','W','A','p','q','Y','o','V',
            };
            List<double> degrees = new List<double>()
            {
                0,30,45,60,90,120,135,150,180,210,225,240,270,300,315,330,195,165,345,15,105,285,75,255,
            };

            int index = characters.IndexOf(c);
            if (index == -1 && !double.TryParse(c.ToString(), out var _))
                Console.WriteLine(c);
            else
                degree = degrees[index];

            return degree;
        }

        private void ImportADOFAI_Click(object sender, EventArgs e)
        {
            try
            {
                var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                culture.NumberFormat.NumberDecimalSeparator = ".";
                string data = Clipboard.GetText();
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
                        bpmnotes.Add(int.Parse(liner));
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
                        twirlnotes.Add(int.Parse(liner));
                    }
                }
                bool clock = true;
                multipliers.Add((180 - Deg(mapdata[0])) / 180);
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
                        double prevangle = Deg(prevnote) + 180;
                        if (mapdata[i] == '!')
                        {
                            prevangle -= 180;
                        }
                        if (prevangle > 360)
                        {
                            prevangle -= 360;
                        }
                        double nextangle = Deg(nextnote);
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
                    GuiTrack.BPMs.Clear();
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
                        GuiTrack.BPMs.Add(new BPM((float)bpmd, (long)time));
                        prevbpmd = bpmd;
                        prevbpmdindex = i;
                    }
                }
                ResetList(0);
            }
            catch
            {

            }
        }

        private void OpenTapper_Click(object sender, EventArgs e)
        {
            if (BPMTapper.inst != null)
                BPMTapper.inst.Close();
            new BPMTapper().Show();
        }
    }
}
