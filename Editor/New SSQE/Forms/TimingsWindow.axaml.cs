using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace New_SSQE
{
    internal partial class TimingsWindow : Window
    {
        internal static TimingsWindow? Instance;
        private static ObservableCollection<TimingPoint> Dataset = new();

        private CultureInfo culture;

        public TimingsWindow()
        {
            Instance = this;
            Icon = new WindowIcon(new Bitmap("assets/textures/Empty.png"));

            InitializeComponent();
            PointList.Items = Dataset;

            culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
        }

        public static void ShowWindow()
        {
            Instance?.Close();

            new TimingsWindow().Show();
            Instance?.ResetList();
        }

        private void AddPoint_Click(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(BPMBox.Text, out var bpm) && long.TryParse(OffsetBox.Text, out var offset))
            {
                foreach (var item in MainWindow.Instance.TimingPoints)
                    if (item.Ms == offset)
                        return;

                MainWindow.Instance.TimingPoints.Add(new TimingPoint(bpm, offset));

                MainWindow.Instance.SortTimings();
            }
        }

        private void UpdatePoint_Click(object sender, RoutedEventArgs e)
        {
            if (PointList.SelectedItems.Count > 0)
            {
                var point = GetPointFromSelected(0);

                if (float.TryParse(BPMBox.Text, out var bpm) && long.TryParse(OffsetBox.Text, out var offset))
                {
                    foreach (var item in MainWindow.Instance.TimingPoints)
                        if (item.Ms == offset && item != point)
                            return;

                    point.BPM = bpm;
                    point.Ms = offset;

                    MainWindow.Instance.SortTimings();
                }
            }
        }

        private void DeletePoint_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < PointList.SelectedItems.Count; i++)
            {
                var point = GetPointFromSelected(i);
                MainWindow.Instance.TimingPoints.Remove(point);
            }

            MainWindow.Instance.SortTimings();
        }

        private void MovePoint_Click(object sender, RoutedEventArgs e)
        {
            if (long.TryParse(MoveBox.Text, out var offset))
            {
                for (int i = 0; i < PointList.SelectedItems.Count; i++)
                {
                    var point = GetPointFromSelected(i);
                    point.Ms += offset;
                }

                MainWindow.Instance.SortTimings();
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
            var dialog = new OpenFileDialog()
            {
                Title = "Select Beatmap",
                Filter = "Beatmaps (*.osu; *.adofai; *.chart)|*.osu;*.chart;*.adofai"
            };

            if (Settings.settings["importPath"] != "")
                dialog.InitialDirectory = Settings.settings["importPath"];

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.settings["importPath"] = Path.GetDirectoryName(dialog.FileName) ?? "";

                var file = dialog.FileName;
                var extension = Path.GetExtension(file);

                var data = File.ReadAllText(file);

                switch (extension)
                {
                    case ".osu":
                        ParseOSU(data, file);
                        break;

                    case ".adofai":
                        ParseADOFAI(data, file);
                        break;

                    case ".chart":
                        ParseCH(data, file);
                        break;
                }
            }
        }

        private void CurrentPos_Click(object sender, RoutedEventArgs e)
        {
            OffsetBox.Text = ((int)Settings.settings["currentTime"].Value).ToString();
        }

        private void PointSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (PointList.SelectedItems.Count > 0)
            {
                var point = GetPointFromSelected(0);

                BPMBox.Text = point.BPM.ToString();
                OffsetBox.Text = point.Ms.ToString();
            }
        }

        private void OffsetBox_TextChanged(object sender, TextInputEventArgs e)
        {
            if (long.TryParse(OffsetBox.Text, out var offset))
            {
                offset = Math.Min(offset, Settings.settings["currentTime"].Max);

                OffsetBox.Text = offset.ToString();
            }
        }

        private TimingPoint GetPointFromSelected(int index)
        {
            var selected = PointList.SelectedItems;
            var points = MainWindow.Instance.TimingPoints;

            var point = selected[index] as TimingPoint;

            for (int i = 0; i < points.Count; i++)
                if (points[i].BPM == point.BPM && points[i].Ms == point.Ms)
                    return points[i];

            return points[0];
        }

        public void ResetList()
        {
            var points = MainWindow.Instance.TimingPoints;

            Dataset.Clear();
            for (int i = 0; i < points.Count; i++)
                Dataset.Add(new TimingPoint(points[i].BPM, points[i].Ms));

            if (Dataset.Count > 0)
            {
                BPMBox.Text = Dataset[0].BPM.ToString();
                OffsetBox.Text = Dataset[0].Ms.ToString();
            }
        }



        private void ParseOSU(string data, string path)
        {
            ActionLogging.Register($"Begin parse of beatmap - {path}");

            try
            {
                data = data.Replace("\r\n", "\n");

                if (data.Contains("TimingPoints"))
                {
                    data = data[(data.IndexOf("TimingPoints") + 13)..];

                    if (data.Contains('['))
                        data = data[..data.IndexOf("[")];
                }

                var split = data.Split('\n');
                var newPoints = new List<TimingPoint>();

                foreach (var line in split)
                {
                    if (line.Contains(','))
                    {
                        var subsplit = line.Split(',');
                        
                        var time = (long)double.Parse(subsplit[0], culture);
                        var bpm = float.Parse(subsplit[1], culture);
                        var inherited = subsplit.Length > 6 ? subsplit[6] == "1" : bpm > 0;
                        bpm = (float)Math.Abs(Math.Round(60000 / bpm, 3));

                        if (bpm > 0 && inherited)
                            newPoints.Add(new TimingPoint(bpm, time));
                    }
                }

                MainWindow.Instance.TimingPoints = newPoints.ToList();
                MainWindow.Instance.SortTimings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse beatmap\n[ OSU : {ex.GetType().Name} ]", "Warning", "OK");
                ActionLogging.Register($"Failed to parse beatmap - OSU - {ex.GetType().Name}\n{ex.StackTrace}", "WARN");
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
            return Degrees.ContainsKey(c) ? Degrees[c] : 69;
        }

        private void ParseADOFAI(string data, string path)
        {
            ActionLogging.Register($"Begin parse of beatmap - {path}");

            try
            {
                data = data.Replace("\r\n", "\n");

                var firstBpmStr = data[(data.IndexOf("bpm") + 6)..];
                var offsetStr = data[data.IndexOf("offset")..];
                offsetStr = offsetStr[..offsetStr.IndexOf(",")].Replace("offset\": ", "");
                var mapData = data[data.IndexOf("pathData")..data.IndexOf(",")].Replace("pathData\": \"", "").Replace("\"", "");

                var firstBpm = decimal.Parse(firstBpmStr, culture);
                var bpm = firstBpm;
                var offset = decimal.Parse(offsetStr, culture);

                data = data[data.IndexOf("\"actions\":")..].Replace("\"actions\":", "");

                if (data[0] == '\t')
                    data = data[1..];

                data = data[2..];
                data = data[..data.IndexOf("]\n")];

                if (data[^1] == '\t')
                    data = data[..^1];

                data = data[..^1];

                var split = data.Split('\n');

                var bpmNotes = new List<int>();
                var twirlNotes = new List<int>();
                var multIndexes = new List<int>();

                var noteTimes = new List<decimal>();
                var bpms = new List<decimal>();
                var mults = new List<decimal>();

                for (int i = 0; i < split.Length; i++)
                {
                    var line = split[i];

                    if (line.Contains("SetSpeed"))
                    {
                        var noteStr = line[line.IndexOf(":")..];
                        noteStr = noteStr[2..noteStr.IndexOf(",")];

                        bpmNotes.Add(int.Parse(noteStr, culture));

                        if (line.Contains("speedType\": \"Multiplier"))
                        {
                            var multStr = line[line.IndexOf("bpmMultiplier")..].Replace("bpmMultiplier\": ", "").Replace(" }", "").Replace(",", "");

                            bpm *= decimal.Parse(multStr, culture);
                            bpms.Add(bpm);
                        }
                        else
                        {
                            var bpmStr = line[line.IndexOf("beatsPerMinute")..line.IndexOf("bpmMultiplier")].Replace("beatsperMinute\": ", "").Replace(", \"", "");

                            bpm = decimal.Parse(bpmStr, culture);
                            bpms.Add(bpm);
                        }
                    }
                    else if (line.Contains("Twirl"))
                    {
                        var noteStr = line[line.IndexOf(":")..];
                        noteStr = noteStr[2..noteStr.IndexOf(",")];

                        twirlNotes.Add(int.Parse(noteStr, culture));
                    }
                }

                var clockwise = true;

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

                var time = mults[0] * offset;
                bpm = firstBpm;
                var prevBpm = firstBpm;

                int prevBpmIndex = 0;

                var newPoints = new List<TimingPoint>();

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
                        newPoints.Add(new TimingPoint((float)bpm, (long)time));

                        prevBpm = bpm;
                        prevBpmIndex = i;
                    }
                }

                MainWindow.Instance.TimingPoints = newPoints.ToList();
                MainWindow.Instance.SortTimings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse beatmap\n[ ADOFAI : {ex.GetType().Name} ]", "Warning", "OK");
                ActionLogging.Register($"Failed to parse beatmap - ADOFAI - {ex.GetType().Name}\n{ex.StackTrace}", "WARN");
            }
        }

        private void ParseCH(string data, string path)
        {
            ActionLogging.Register($"Begin parse of beatmap - {path}");

            try
            {
                data = data.Replace("\r\n", "\n");

                var resolutionStr = data[data.IndexOf("Resolution")..];
                resolutionStr = resolutionStr[..resolutionStr.IndexOf("\n")].Replace("Resolution = ", "");
                var resolution = decimal.Parse(resolutionStr, culture);

                data = data[data.IndexOf("SyncTrack")..];
                data = data[..data.IndexOf("}")].Replace("SyncTrack]\n{", "");

                var split = data.Split('\n');

                var bpmList = new List<decimal>();
                var timeList = new List<decimal>();
                var msList = new List<decimal>();

                foreach (var line in split)
                {
                    if (line.Contains('B'))
                    {
                        var bpmStr = line[line.IndexOf(" B ")..].Replace(" B ", "");
                        var bpm = decimal.Parse(bpmStr, culture) / 1000m;
                        var timeStr = line[..line.IndexOf(" B ")].Replace(" =", "");
                        var time = decimal.Parse(timeStr, culture);

                        bpmList.Add(bpm);
                        timeList.Add(time);
                    }
                }

                for (int i = 0; i < timeList.Count; i++)
                {
                    if (i > 0)
                    {
                        var difference = timeList[i] - timeList[i - 1];
                        var bpm = timeList[i - 1];

                        difference = Math.Round(1000m * (difference / (bpm * resolution / 60m)), 2);
                        difference += msList[i - 1];

                        msList.Add(difference);
                    }
                    else
                        msList.Add(timeList[i]);
                }

                var newPoints = new List<TimingPoint>();

                for (int i = 0; i < msList.Count; i++)
                    newPoints.Add(new TimingPoint((float)bpmList[i], (long)msList[i]));

                MainWindow.Instance.TimingPoints = newPoints.ToList();
                MainWindow.Instance.SortTimings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse beatmap\n[ CH : {ex.GetType().Name} ]", "Warning", "OK");
                ActionLogging.Register($"Failed to parse beatmap - CH - {ex.GetType().Name}\n{ex.StackTrace}", "WARN");
            }
        }
    }
}
