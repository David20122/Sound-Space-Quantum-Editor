using System;
using System.Linq;
using System.Windows.Forms;
using Sound_Space_Editor.Gui;

namespace Sound_Space_Editor
{
    public partial class TimingsWindow : Form
    {
        public TimingsWindow()
        {
            InitializeComponent();
            ResetList(0);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(BPMBox.Text, out float BPM) && BPM > 5000)
                BPMBox.Text = "5000";
            if (long.TryParse(OffsetBox.Text, out long Offset) && Offset > EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds)
                OffsetBox.Text = EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds.ToString();
            if (float.TryParse(BPMBox.Text, out float bpm) && bpm > 33 && long.TryParse(OffsetBox.Text, out long offset) && offset >= 0)
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
            if (PointList.SelectedRows.Count > 0)
            {
                GuiTrack.BPMs.RemoveAt(PointList.CurrentRow.Index);
                GuiTrack.BPMs = GuiTrack.BPMs.OrderBy(o => o.Ms).ToList();
                ResetList(PointList.CurrentRow.Index - 1);
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(BPMBox.Text, out float BPM) && BPM > 5000)
                BPMBox.Text = "5000";
            if (long.TryParse(OffsetBox.Text, out long Offset) && Offset > EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds)
                OffsetBox.Text = EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds.ToString();
            if (PointList.SelectedRows.Count > 0)
            {
                if (float.TryParse(BPMBox.Text, out float bpm) && bpm > 33 && long.TryParse(OffsetBox.Text, out long offset) && offset >= 0)
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

        public void ResetList(int index)
        {
            if (index < 0)
                index = 0;
            PointList.Rows.Clear();
            foreach (var point in GuiTrack.BPMs)
            {
                PointList.Rows.Add(point.bpm, point.Ms);
            }
            if (GuiTrack.BPMs.Count > 0)
            {
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
    }
}
