using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace New_SSQE
{
    public partial class ExportSSPM : Window
    {
        public static ExportSSPM? Instance;

        public ExportSSPM()
        {
            Instance = this;
            Icon = new WindowIcon(new Bitmap("assets/textures/Empty.png"));

            InitializeComponent();

            MapperBox.Text = Settings.settings["mappers"];
            SongNameBox.Text = Settings.settings["songName"];
            UseCover.IsChecked = Settings.settings["useCover"];
            CoverPathBox.Text = Settings.settings["cover"];
            CustomDifficultyBox.Text = Settings.settings["customDifficulty"];

            foreach (ComboBoxItem item in DifficultyBox.Items)
            {
                if (item?.Content.ToString() == Settings.settings["difficulty"])
                    DifficultyBox.SelectedItem = item;
            }

            CreateID();
        }

        private static readonly char[] invalidChars = { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };

        private static string FixString(string input)
        {
            string str = input.ToLower().Replace(" ", "_");

            for (int i = 0; i < str.Length; i++)
            {
                if (Array.IndexOf(invalidChars, str[i]) > -1)
                    str = str.Remove(i, 1).Insert(i, "_");
            }

            return str;
        }

        private string GetSongName()
        {
            return !string.IsNullOrWhiteSpace(SongNameBox.Text) ? SongNameBox.Text : "Untitled Song";
        }

        private string[] GetMappers()
        {
            if (MapperBox.Text == null)
                return new string[] { "None" };

            var mappers = MapperBox.Text.Split("\n");
            for (int i = 0; i < mappers.Length; i++)
                mappers[i] = mappers[i].Replace("\r", "");

            var mappersList = new List<string>();

            foreach (var mapper in mappers)
                if (!string.IsNullOrWhiteSpace(mapper))
                    mappersList.Add(mapper);

            return mappersList.Count > 0 ? mappersList.ToArray() : new string[] { "None" };
        }

        private void CreateID()
        {
            var songName = GetSongName();
            var mappers = string.Join(" ", GetMappers());

            MapIDBox.Content = FixString($"SSQE Export - {mappers} - {songName}");
        }

        public static void UpdateID()
        {
            Instance?.CreateID();
        }

        public static void ShowWindow()
        {
            Instance?.Close();

            new ExportSSPM().Show();
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            var editor = MainWindow.Instance;

            editor.info["songId"] = MapIDBox.Content.ToString() ?? "";
            editor.info["mapName"] = GetSongName();
            editor.info["mappers"] = string.Join("\n", GetMappers());
            editor.info["coverPath"] = (UseCover.IsChecked ?? false) ? CoverPathBox.Text : "";
            var item = DifficultyBox.SelectedItem as ComboBoxItem;
            editor.info["difficulty"] = MainWindow.difficulties.ContainsKey(item?.Content.ToString() ?? "") ? (item?.Content.ToString() ?? "") : "N/A";
            editor.info["customDifficulty"] = CustomDifficultyBox.Text;

            Settings.settings["mappers"] = editor.info["mappers"];
            Settings.settings["songName"] = editor.info["mapName"];
            Settings.settings["difficulty"] = editor.info["difficulty"];
            Settings.settings["useCover"] = UseCover.IsChecked ?? false;
            Settings.settings["cover"] = CoverPathBox.Text;
            Settings.settings["customDifficulty"] = CustomDifficultyBox.Text;

            Close();

            editor.RunSSPMExport();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Select Cover Image",
                Filter = "PNG Images (*.png)|*.png"
            };

            if (Settings.settings["coverPath"] != "")
                dialog.InitialDirectory = Settings.settings["coverPath"];

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.settings["coverPath"] = Path.GetDirectoryName(dialog.FileName) ?? "";

                CoverPathBox.Text = dialog.FileName;
            }
            else
                CoverPathBox.Text = "Default";
        }
    }
}
