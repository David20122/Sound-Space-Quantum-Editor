using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using New_SSQE.Audio;
using New_SSQE.ExternalUtils;
using New_SSQE.Maps;
using New_SSQE.Misc.Dialogs;
using New_SSQE.Misc.Static;
using New_SSQE.Objects;
using New_SSQE.Preferences;
using System.Collections;
using System.Collections.ObjectModel;
using OpenFileDialog = New_SSQE.Misc.Dialogs.OpenFileDialog;

namespace New_SSQE
{
    internal partial class TimingsWindow : Window
    {
        internal static TimingsWindow? Instance;
        private static readonly ObservableCollection<TimingPoint> Dataset = new();

        public TimingsWindow()
        {
            Instance = this;
            Icon = new(new Bitmap($"{Assets.TEXTURES}\\Empty.png"));

            InitializeComponent();
            PointList.Items = Dataset;
            AdjustNotes.IsChecked = Settings.adjustNotes.Value;
        }

        private void AdjustNotes_Checked(object sender, RoutedEventArgs e)
        {
            Settings.adjustNotes.Value = AdjustNotes.IsChecked ?? false;
        }

        public static void ShowWindow()
        {
            Instance?.Close();

            TimingsWindow window = new();

            Instance?.ResetList();

            if (Dataset.Count > 0)
            {
                TimingPoint point = Timing.GetCurrentBpm(Settings.currentTime.Value.Value);

                foreach (TimingPoint item in Dataset)
                {
                    if (item.Ms == point.Ms && item.BPM == point.BPM && Instance?.PointList.SelectedItems.Count == 0)
                        Instance?.PointList.SelectedItems.Add(item);
                }
            }

            window.Show();

            if (Platform.IsLinux)
            {
                window.Topmost = true;
                BackgroundWindow.YieldWindow(window);
            }
        }

        private void AddPoint_Click(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(BPMBox.Text, out float bpm) && long.TryParse(OffsetBox.Text, out long offset))
            {
                foreach (TimingPoint item in CurrentMap.TimingPoints)
                    if (item.Ms == offset)
                        return;

                CurrentMap.TimingPoints.Add(new(bpm, offset));
                CurrentMap.SortTimings();
            }
        }

        private void UpdatePoint_Click(object sender, RoutedEventArgs e)
        {
            if (PointList.SelectedItems.Count > 0)
            {
                TimingPoint point = GetPointFromSelected(0);

                if (float.TryParse(BPMBox.Text, out float bpm) && long.TryParse(OffsetBox.Text, out long offset))
                {
                    foreach (TimingPoint item in CurrentMap.TimingPoints)
                        if (item.Ms == offset && item != point)
                            return;

                    if (Settings.adjustNotes.Value)
                        Timing.UpdatePoint(point, bpm, offset);

                    point.BPM = bpm;
                    point.Ms = offset;

                    CurrentMap.SortTimings();
                }
            }
        }

        private void DeletePoint_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < PointList.SelectedItems.Count; i++)
            {
                TimingPoint point = GetPointFromSelected(i);
                CurrentMap.TimingPoints.Remove(point);
            }

            CurrentMap.SortTimings();
        }

        private void MovePoint_Click(object sender, RoutedEventArgs e)
        {
            if (long.TryParse(MoveBox.Text, out long offset) && offset != 0)
            {
                List<TimingPoint> points = [];

                for (int i = 0; i < PointList.SelectedItems.Count; i++)
                {
                    TimingPoint point = GetPointFromSelected(i);
                    point.Ms += offset;
                    points.Add(point);
                }

                if (Settings.adjustNotes.Value)
                    Timing.MovePoints(points, offset);
                CurrentMap.SortTimings();
            }
        }



        private void OpenTapper_Click(object sender, RoutedEventArgs e)
        {
            BPMTapper.ShowWindow();
        }

        private void PasteOSU_Click(object sender, RoutedEventArgs e)
        {
            ParseOSU(Clipboard.GetText(), "FromClipboard");
        }

        private void PasteADOFAI_Click(object sender, RoutedEventArgs e)
        {
            ParseADOFAI(Clipboard.GetText(), "FromClipboard");
        }

        private void PasteCH_Click(object sender, RoutedEventArgs e)
        {
            ParseCH(Clipboard.GetText(), "FromClipboard");
        }

        private void OpenBeatmap_Click(object sender, RoutedEventArgs e)
        {
            DialogResult result = new OpenFileDialog()
            {
                Title = "Select Beatmap",
                Filter = "Beatmaps (*.osu; *.adofai; *.chart)|*.osu;*.chart;*.adofai"
            }.RunWithSetting(Settings.importPath, out string fileName);

            if (result == DialogResult.OK)
            {
                string extension = Path.GetExtension(fileName);
                string data = File.ReadAllText(fileName);

                switch (extension)
                {
                    case ".osu":
                        ParseOSU(data, fileName);
                        break;

                    case ".adofai":
                        ParseADOFAI(data, fileName);
                        break;

                    case ".chart":
                        ParseCH(data, fileName);
                        break;
                }
            }
        }

        private void CurrentPos_Click(object sender, RoutedEventArgs e)
        {
            OffsetBox.Text = ((int)Settings.currentTime.Value.Value).ToString();
        }

        private void PointSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (PointList.SelectedItems.Count > 0)
            {
                TimingPoint point = GetPointFromSelected(0);

                BPMBox.Text = point.BPM.ToString();
                OffsetBox.Text = point.Ms.ToString();
            }
        }

        private void OffsetBox_TextChanged(object sender, TextInputEventArgs e)
        {
            if (long.TryParse(OffsetBox.Text, out long offset))
            {
                offset = Math.Min(offset, (long)Settings.currentTime.Value.Max);

                OffsetBox.Text = offset.ToString();
            }
        }

        private TimingPoint GetPointFromSelected(int index)
        {
            IList selected = PointList.SelectedItems;
            List<TimingPoint> points = CurrentMap.TimingPoints;

            TimingPoint? point = selected[index] as TimingPoint;

            for (int i = 0; i < points.Count; i++)
                if (points[i].BPM == point?.BPM && points[i].Ms == point.Ms)
                    return points[i];

            return points[0];
        }

        public void ResetList()
        {
            List<TimingPoint> points = CurrentMap.TimingPoints;

            Dataset.Clear();
            for (int i = 0; i < points.Count; i++)
                Dataset.Add(new(points[i].BPM, points[i].Ms));

            if (Dataset.Count > 0)
            {
                BPMBox.Text = Dataset[0].BPM.ToString();
                OffsetBox.Text = Dataset[0].Ms.ToString();
            }
        }



        private static void ParseOSU(string data, string path)
        {
            Logging.Register($"Begin parse of beatmap - {path}");

            try
            {
                data = data.Replace("\r\n", "\n");

                if (data.Contains("TimingPoints"))
                {
                    data = data[(data.IndexOf("TimingPoints") + 13)..];

                    if (data.Contains('['))
                        data = data[..data.IndexOf('[')];
                }

                string[] split = data.Split('\n');
                List<TimingPoint> newPoints = new();

                foreach (string line in split)
                {
                    if (line.Contains(','))
                    {
                        string[] subsplit = line.Split(',');
                        
                        long time = (long)double.Parse(subsplit[0], Program.Culture);
                        float bpm = float.Parse(subsplit[1], Program.Culture);
                        bool inherited = subsplit.Length > 6 ? subsplit[6] == "1" : bpm > 0;
                        bpm = (float)Math.Abs(Math.Round(60000 / bpm, 3));

                        if (bpm > 0 && inherited)
                            newPoints.Add(new(bpm, time));
                    }
                }

                CurrentMap.TimingPoints = newPoints.ToList();
                CurrentMap.SortTimings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse beatmap\n[ OSU : {ex.GetType().Name} ]", MBoxIcon.Warning, MBoxButtons.OK);
                Logging.Register($"Failed to parse beatmap - OSU", LogSeverity.WARN, ex);
            }
        }

        // may add more letters as their angles are identified
        private static readonly Dictionary<char, int> Degrees = new()
        {
            {'R', 0 }, {'J', 30 }, {'E', 45 }, {'T', 60 }, {'U', 90 }, {'G', 120 }, {'Q', 135 }, {'H', 150 }, {'L', 180 }, {'N', 210 }, {'Z', 225 }, {'F', 240 },
            {'D', 270 }, {'B', 300 }, {'C', 315 }, {'M', 330 }, {'x', 195 }, {'W', 165 }, {'A', 345 }, {'p', 15 }, {'q', 105 }, {'Y', 285 }, {'o', 75 }, {'V', 255 },
        };

        private static int GetDegree(char c)
        {
            return Degrees.TryGetValue(c, out int value) ? value : 69;
        }

        private static void ParseADOFAI(string data, string path)
        {
            Logging.Register($"Begin parse of beatmap - {path}");

            try
            {
                data = data.Replace("\r\n", "\n");

                string firstBpmStr = data[(data.IndexOf("bpm") + 6)..];
                string offsetStr = data[data.IndexOf("offset")..];
                offsetStr = offsetStr[..offsetStr.IndexOf(',')].Replace("offset\": ", "");
                string mapData = data[data.IndexOf("pathData")..data.IndexOf(',')].Replace("pathData\": \"", "").Replace("\"", "");

                decimal firstBpm = decimal.Parse(firstBpmStr, Program.Culture);
                decimal bpm = firstBpm;
                decimal offset = decimal.Parse(offsetStr, Program.Culture);

                data = data[data.IndexOf("\"actions\":")..].Replace("\"actions\":", "");

                if (data[0] == '\t')
                    data = data[1..];

                data = data[2..];
                data = data[..data.IndexOf("]\n")];

                if (data[^1] == '\t')
                    data = data[..^1];

                data = data[..^1];

                string[] split = data.Split('\n');

                List<int> bpmNotes = new();
                List<int> twirlNotes = new();
                List<int> multIndexes = new();

                List<decimal> noteTimes = new();
                List<decimal> bpms = new();
                List<decimal> mults = new();

                for (int i = 0; i < split.Length; i++)
                {
                    string line = split[i];

                    if (line.Contains("SetSpeed"))
                    {
                        string noteStr = line[line.IndexOf(':')..];
                        noteStr = noteStr[2..noteStr.IndexOf(',')];

                        bpmNotes.Add(int.Parse(noteStr, Program.Culture));

                        if (line.Contains("speedType\": \"Multiplier"))
                        {
                            string multStr = line[line.IndexOf("bpmMultiplier")..].Replace("bpmMultiplier\": ", "").Replace(" }", "").Replace(",", "");

                            bpm *= decimal.Parse(multStr, Program.Culture);
                            bpms.Add(bpm);
                        }
                        else
                        {
                            string bpmStr = line[line.IndexOf("beatsPerMinute")..line.IndexOf("bpmMultiplier")].Replace("beatsperMinute\": ", "").Replace(", \"", "");

                            bpm = decimal.Parse(bpmStr, Program.Culture);
                            bpms.Add(bpm);
                        }
                    }
                    else if (line.Contains("Twirl"))
                    {
                        string noteStr = line[line.IndexOf(':')..];
                        noteStr = noteStr[2..noteStr.IndexOf(',')];

                        twirlNotes.Add(int.Parse(noteStr, Program.Culture));
                    }
                }

                bool clockwise = true;

                mults.Add((180m - GetDegree(mapData[0]) ) / 180m);
                multIndexes.Add(0);

                for (int i = 1; i < mapData.Length; i++)
                {
                    char prev = mapData[i - 1];

                    if (prev != '!')
                    {
                        char next = mapData[i];

                        if (next == '!')
                            next = mapData[i + 1];

                        if (twirlNotes.Contains(i))
                            clockwise ^= true;

                        decimal prevAngle = GetDegree(prev) + 180 - (mapData[i] == '!' ? 180 : 0);
                        prevAngle = (prevAngle - 1) % 360 + 1;

                        decimal nextAngle = GetDegree(next);
                        decimal angle = prevAngle - nextAngle;

                        if (angle <= 0)
                            angle += 360;
                        if (!clockwise)
                            angle = 360 - angle;
                        if (angle <= 0)
                            angle += 360;

                        angle /= 180;

                        mults.Add(angle);
                        multIndexes.Add(i);
                    }
                }

                decimal time = mults[0] * offset;
                bpm = firstBpm;
                decimal prevBpm = firstBpm;

                int prevBpmIndex = 0;

                List<TimingPoint> newPoints = new();

                for (int i = 0; i < mults.Count; i++)
                {
                    int bpmIndex = multIndexes[i];

                    if (bpmNotes.Contains(bpmIndex))
                        bpm = bpms[bpmNotes.IndexOf(bpmIndex)];

                    if (i > 0)
                        time += 60000m / bpm * mults[i];

                    if (prevBpm != bpm && !(prevBpm % bpm == 0 ||
                                            bpm * mults[i] % prevBpm * mults[prevBpmIndex] == 0 ||
                                            prevBpm * mults[prevBpmIndex] % bpm * mults[i] == 0))
                    {
                        newPoints.Add(new((float)bpm, (long)time));

                        prevBpm = bpm;
                        prevBpmIndex = i;
                    }
                }

                CurrentMap.TimingPoints = newPoints.ToList();
                CurrentMap.SortTimings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse beatmap\n[ ADOFAI : {ex.GetType().Name} ]", MBoxIcon.Warning, MBoxButtons.OK);
                Logging.Register($"Failed to parse beatmap - ADOFAI", LogSeverity.WARN, ex);
            }
        }

        private static void ParseCH(string data, string path)
        {
            Logging.Register($"Begin parse of beatmap - {path}");

            try
            {
                data = data.Replace("\r\n", "\n");

                string resolutionStr = data[data.IndexOf("Resolution")..];
                resolutionStr = resolutionStr[..resolutionStr.IndexOf('\n')].Replace("Resolution = ", "");
                decimal resolution = decimal.Parse(resolutionStr, Program.Culture);

                data = data[data.IndexOf("SyncTrack")..];
                data = data[..data.IndexOf('}')].Replace("SyncTrack]\n{", "");

                string[] split = data.Split('\n');

                List<decimal> bpmList = new();
                List<decimal> timeList = new();
                List<decimal> msList = new();

                foreach (string line in split)
                {
                    if (line.Contains('B'))
                    {
                        string bpmStr = line[line.IndexOf(" B ")..].Replace(" B ", "");
                        decimal bpm = decimal.Parse(bpmStr, Program.Culture) / 1000m;
                        string timeStr = line[..line.IndexOf(" B ")].Replace(" =", "");
                        decimal time = decimal.Parse(timeStr, Program.Culture);

                        bpmList.Add(bpm);
                        timeList.Add(time);
                    }
                }

                for (int i = 0; i < timeList.Count; i++)
                {
                    if (i > 0)
                    {
                        decimal difference = timeList[i] - timeList[i - 1];
                        decimal bpm = timeList[i - 1];

                        difference = Math.Round(1000m * (difference / (bpm * resolution / 60m)), 2);
                        difference += msList[i - 1];

                        msList.Add(difference);
                    }
                    else
                        msList.Add(timeList[i]);
                }

                List<TimingPoint> newPoints = new();

                for (int i = 0; i < msList.Count; i++)
                    newPoints.Add(new((float)bpmList[i], (long)msList[i]));

                CurrentMap.TimingPoints = newPoints.ToList();
                CurrentMap.SortTimings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse beatmap\n[ CH : {ex.GetType().Name} ]", MBoxIcon.Warning, MBoxButtons.OK);
                Logging.Register($"Failed to parse beatmap - CH", LogSeverity.WARN, ex);
            }
        }

        private void DetectBPM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long start = long.Parse(StartBox.Text);
                long end = long.Parse(EndBox.Text);

                if (end > start)
                {
                    if (!Settings.detectWarningShown.Value)
                    {
                        MessageBox.Show("BPM detection results are rarely accurate! Make sure to test and adjust the result before using it in your map.\n\nThis may be used as a general baseline but not the final answer for this song's BPM.", MBoxIcon.Warning, MBoxButtons.OK);
                        Settings.detectWarningShown.Value = true;
                    }

                    MusicPlayer.Pause();
                    ResultBox.Text = MusicPlayer.DetectBPM(start, end).ToString();
                }
            }
            catch { }
        }
    }
}
