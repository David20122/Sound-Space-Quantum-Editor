using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using New_SSQE.ExternalUtils;
using New_SSQE.FileParsing;
using New_SSQE.Misc.Dialogs;
using New_SSQE.Misc.Static;
using New_SSQE.Preferences;
using OpenFileDialog = New_SSQE.Misc.Dialogs.OpenFileDialog;

namespace New_SSQE
{
    public partial class ExportSSPM : Window
    {
        public static ExportSSPM? Instance;

        public ExportSSPM()
        {
            Instance = this;
            Icon = new(new Bitmap($"{Assets.TEXTURES}\\Empty.png"));

            InitializeComponent();

            MapIDBox.IsReadOnly = !MainWindow.DebugVersion;

            MapperBox.Text = Settings.mappers.Value;
            SongNameBox.Text = Settings.songName.Value;
            UseCover.IsChecked = Settings.useCover.Value;
            CoverPathBox.Text = Settings.cover.Value;
            CustomDifficultyBox.Text = Settings.customDifficulty.Value;

            foreach (ComboBoxItem item in DifficultyBox.Items)
            {
                if (item?.Content.ToString() == Settings.difficulty.Value)
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

            return str.Replace(",", "");
        }

        private string GetSongName()
        {
            return !string.IsNullOrWhiteSpace(SongNameBox.Text) ? SongNameBox.Text : "Untitled Song";
        }

        private string[] GetMappers()
        {
            if (MapperBox.Text == null)
                return new string[] { "None" };

            string[] mappers = MapperBox.Text.Split("\n");
            for (int i = 0; i < mappers.Length; i++)
                mappers[i] = mappers[i].Replace("\r", "");

            List<string> mappersList = new();

            foreach (string mapper in mappers)
                if (!string.IsNullOrWhiteSpace(mapper))
                    mappersList.Add(mapper);

            return mappersList.Count > 0 ? mappersList.ToArray() : new string[] { "None" };
        }

        private string prevString = "";

        private void CreateID()
        {
            string songName = GetSongName();
            string mappers = string.Join(" ", GetMappers());
            string curString = FixString($"{mappers} - {songName}");

            if (curString != prevString)
            {
                MapIDBox.Text = curString;
                prevString = curString;
            }
        }

        public static void UpdateID()
        {
            Instance?.CreateID();
        }

        public static void ShowWindow()
        {
            Instance?.Close();

            ExportSSPM window = new();

            window.Show();

            if (Platform.IsLinux)
            {
                window.Topmost = true;
                BackgroundWindow.YieldWindow(window);
            }
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            Exporting.Info["songId"] = MapIDBox.Text ?? "";
            Exporting.Info["mapName"] = GetSongName();
            Exporting.Info["mappers"] = string.Join("\n", GetMappers());
            Exporting.Info["coverPath"] = (UseCover.IsChecked ?? false) ? CoverPathBox.Text : "";
            ComboBoxItem? item = DifficultyBox.SelectedItem as ComboBoxItem;
            Exporting.Info["difficulty"] = Exporting.Difficulties.ContainsKey(item?.Content.ToString() ?? "") ? (item?.Content.ToString() ?? "") : "N/A";
            Exporting.Info["customDifficulty"] = CustomDifficultyBox.Text;

            Settings.mappers.Value = Exporting.Info["mappers"];
            Settings.songName.Value = Exporting.Info["mapName"];
            Settings.difficulty.Value = Exporting.Info["difficulty"];
            Settings.useCover.Value = UseCover.IsChecked ?? false;
            Settings.cover.Value = CoverPathBox.Text;
            Settings.customDifficulty.Value = CustomDifficultyBox.Text;

            Close();

            Exporting.ExportSSPM();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult result = new OpenFileDialog()
            {
                Title = "Select Cover Image",
                Filter = "PNG Images (*.png)|*.png"
            }.RunWithSetting(Settings.coverPath, out string fileName);

            if (result == DialogResult.OK)
                CoverPathBox.Text = fileName;
            else
                CoverPathBox.Text = "Default";
        }
    }
}
