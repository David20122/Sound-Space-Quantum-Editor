using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using New_SSQE.ExternalUtils;
using New_SSQE.Misc.Static;

namespace New_SSQE
{
    public partial class BPMTapper : Window
    {
        public static BPMTapper? Instance;

        private static int taps = 0;
        private static bool tapping = false;

        private static float bpm = 0;
        private static DateTime startTime;

        public BPMTapper()
        {
            Instance = this;
            Icon = new(new Bitmap($"{Assets.TEXTURES}\\Empty.png"));

            InitializeComponent();
        }

        public static void ShowWindow()
        {
            Instance?.Close();

            BPMTapper window = new();

            window.Show();
            window.Reset();

            if (Platform.IsLinux)
            {
                window.Topmost = true;
                BackgroundWindow.YieldWindow(window);
            }
        }

        public void Reset()
        {
            tapping = false;
            taps = 0;
            bpm = 0;

            BPMBox.Content = "";
            BPMDecimalBox.Content = "";
        }

        private bool ButtonsFocused()
        {
            return ResetButton.IsFocused || TapButton.IsFocused;
        }

        private void IncrementBPM()
        {
            taps++;

            if (!tapping)
            {
                tapping = true;
                startTime = DateTime.Now;

                BPMBox.Content = "0";
                BPMDecimalBox.Content = "0";
            }
            else if (int.TryParse(DecimalPlacesBox.Text, out int decimals))
            {
                float mins = (float)(DateTime.Now - startTime).TotalMilliseconds / 60000f;

                bpm = (taps - 1f) / mins;

                BPMBox.Content = Math.Round(bpm).ToString();
                BPMDecimalBox.Content = Math.Round(bpm, decimals).ToString();
            }
        }

        private void TapButton_Click(object sender, RoutedEventArgs e)
        {
            IncrementBPM();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!DecimalPlacesBox.IsFocused && (e.Key != Key.Space || !ButtonsFocused()))
                IncrementBPM();

            base.OnKeyDown(e);
        }
    }
}
