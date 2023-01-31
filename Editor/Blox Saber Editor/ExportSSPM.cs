using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace Sound_Space_Editor
{
    public partial class ExportSSPM : Form
    {
        public ExportSSPM()
        {
            InitializeComponent();
        }

        private string FixString(string input)
        {
            return input.ToLower().Replace(" ", "_");
        }

        private string GetSongName()
        {
            return !string.IsNullOrWhiteSpace(SongNameBox.Text) ? SongNameBox.Text : "Untitled Song";
        }

        private string[] GetMappers()
        {
            var mappers = MappersBox.Text.Split('\n');
            var mappersf = new List<string>();

            foreach (var mapper in mappers)
                if (!string.IsNullOrWhiteSpace(mapper))
                    mappersf.Add(mapper);

            return mappersf.Count > 0 ? mappersf.ToArray() : new string[] { "None" };
        }

        private void CreateID()
        {
            var songName = GetSongName();
            var mappers = string.Join(" ", GetMappers());

            MapIDBox.Text = FixString($"SSQE Export - {mappers} - {songName}");
        }

        private void FinishButton_Click(object sender, EventArgs e)
        {
            var editor = MainWindow.Instance;

            editor.info["songId"] = MapIDBox.Text;
            editor.info["mapName"] = GetSongName();
            editor.info["mappers"] = string.Join("\n", GetMappers());
            editor.info["coverPath"] = UseCoverCheckbox.Checked ? CoverPathBox.Text : "";
            editor.info["difficulty"] = editor.difficulties.ContainsKey(DifficultyBox.Text) ? DifficultyBox.Text : "N/A";

            Close();

            editor.ExportSSPM();
        }

        private void SongNameBox_TextChanged(object sender, EventArgs e)
        {
            CreateID();
        }

        private void MappersBox_TextChanged(object sender, EventArgs e)
        {
            CreateID();
        }

        private void ExportSSPM_Load(object sender, EventArgs e)
        {
            SongNameBox.Text = Path.GetFileNameWithoutExtension(MainWindow.Instance.fileName) ?? "Untitled Song";
            CreateID();
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Select Cover Image",
                Filter = "PNG Images (*.png)|*.png"
            })
            {
                if (Settings.settings["coverPath"] != "")
                    dialog.InitialDirectory = Settings.settings["coverPath"];

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Settings.settings["coverPath"] = Path.GetDirectoryName(dialog.FileName);

                    CoverPathBox.Text = dialog.FileName;
                }
                else
                    CoverPathBox.Text = "Default";
            }
        }
    }
}
