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
    public partial class ExportNOVA : Window
    {
        private static ExportNOVA? Instance;

        public ExportNOVA()
        {
            Instance = this;
            Icon = new(new Bitmap($"{Assets.TEXTURES}\\empty.png"));

            InitializeComponent();

            TitleBox.Text = Settings.songTitle.Value;
            ArtistBox.Text = Settings.songArtist.Value;
            MapperBox.Text = Settings.mapCreator.Value;
            LinkBox.Text = Settings.mapCreatorPersonalLink.Value;
            CoverPathBox.Text = Settings.novaCover.Value;
            IconPathBox.Text = Settings.novaIcon.Value;
            SongOffsetBox.Text = Settings.songOffset.Value;
            PreviewStartBox.Text = Settings.previewStartTime.Value;
            PreviewDurationBox.Text = Settings.previewDuration.Value;
        }

        public static void ShowWindow()
        {
            Instance?.Close();

            ExportNOVA window = new();

            window.Show();

            if (Platform.IsLinux)
            {
                window.Topmost = true;
                BackgroundWindow.YieldWindow(window);
            }
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            Exporting.NovaInfo["songOffset"] = long.TryParse(SongOffsetBox.Text, out long offset) ? offset.ToString() : "0";
            Exporting.NovaInfo["songTitle"] = TitleBox.Text;
            Exporting.NovaInfo["songArtist"] = ArtistBox.Text;
            Exporting.NovaInfo["mapCreator"] = MapperBox.Text;
            Exporting.NovaInfo["mapCreatorPersonalLink"] = LinkBox.Text;
            Exporting.NovaInfo["previewStartTime"] = long.TryParse(PreviewStartBox.Text, out long start) ? start.ToString() : "0";
            Exporting.NovaInfo["previewDuration"] = long.TryParse(PreviewDurationBox.Text, out long duration) ? duration.ToString() : "0";
            Exporting.NovaInfo["coverPath"] = CoverPathBox.Text;
            Exporting.NovaInfo["iconPath"] = IconPathBox.Text;

            Settings.songOffset.Value = Exporting.NovaInfo["songOffset"];
            Settings.songTitle.Value = Exporting.NovaInfo["songTitle"];
            Settings.songArtist.Value = Exporting.NovaInfo["songArtist"];
            Settings.mapCreator.Value = Exporting.NovaInfo["mapCreator"];
            Settings.mapCreatorPersonalLink.Value = Exporting.NovaInfo["mapCreatorPersonalLink"];
            Settings.previewStartTime.Value = Exporting.NovaInfo["previewStartTime"];
            Settings.previewDuration.Value = Exporting.NovaInfo["previewDuration"];
            Settings.novaCover.Value = Exporting.NovaInfo["coverPath"];
            Settings.novaIcon.Value = Exporting.NovaInfo["iconPath"];

            Close();

            Exporting.ExportNOVA();
        }

        private void SelectCover_Click(object sender, RoutedEventArgs e)
        {
            DialogResult result = new OpenFileDialog()
            {
                Title = "Select Cover Image",
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg"
            }.RunWithSetting(Settings.coverPath, out string fileName);

            if (result == DialogResult.OK)
                CoverPathBox.Text = fileName;
            else
                CoverPathBox.Text = "";
        }

        private void SelectIcon_Click(object sender, RoutedEventArgs e)
        {
            DialogResult result = new OpenFileDialog()
            {
                Title = "Select Profile Icon",
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg"
            }.RunWithSetting(Settings.coverPath, out string fileName);

            if (result == DialogResult.OK)
                IconPathBox.Text = fileName;
            else
                IconPathBox.Text = "";
        }

        private void SongOffset_Click(object sender, RoutedEventArgs e)
        {
            SongOffsetBox.Text = ((long)Settings.currentTime.Value.Value).ToString();
        }

        private void PreviewStart_Click(object sender, RoutedEventArgs e)
        {
            long origin = long.TryParse(PreviewStartBox.Text, out long temp) ? temp : 0;
            long duration = long.TryParse(PreviewDurationBox.Text, out temp) ? temp : 0;
            PreviewStartBox.Text = ((long)Settings.currentTime.Value.Value).ToString();
            PreviewDurationBox.Text = (duration + origin - (long)Settings.currentTime.Value.Value).ToString();
        }

        private void PreviewDuration_Click(object sender, RoutedEventArgs e)
        {
            long start = long.TryParse(PreviewStartBox.Text, out long temp) ? temp : 0;
            PreviewDurationBox.Text = ((long)Settings.currentTime.Value.Value - start).ToString();
        }
    }
}
