using System;
using System.Windows.Forms;

namespace Sound_Space_Editor
{
    public partial class BPMTapper : Form
    {
        public static BPMTapper Instance;

        private DateTime startTime;

        private float bpm = 0;

        private int taps = 0;
        private bool tapping = false;

        public BPMTapper()
        {
            InitializeComponent();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            tapping = false;
            taps = 0;
            bpm = 0;

            BPM.Text = "";
            BPMDecimals.Text = "";
        }

        private bool ButtonsFocused()
        {
            return TapButton.Focused || ResetButton.Focused;
        }

        private void IncrementBPM()
        {
            taps++;

            if (!tapping)
            {
                tapping = true;
                startTime = DateTime.Now;

                BPM.Text = "0";
                BPMDecimals.Text = "0";
            }
            else
            {
                var mins = (float)(DateTime.Now - startTime).TotalMilliseconds / 60000f;

                bpm = (taps - 1f) / mins;

                BPM.Text = Math.Round(bpm).ToString();
                BPMDecimals.Text = Math.Round(bpm, (int)DecimalPlaces.Value).ToString();
            }
        }

        private void TapButton_Click(object sender, EventArgs e)
        {
            IncrementBPM();
        }

        private void BPMTapper_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!DecimalPlaces.Focused && (e.KeyChar != ' ' || !ButtonsFocused()))
                IncrementBPM();
        }
    }
}
