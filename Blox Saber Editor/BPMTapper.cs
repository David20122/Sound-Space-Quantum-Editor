using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sound_Space_Editor
{
    public partial class BPMTapper : Form
    {
        public static BPMTapper inst;

        private double Bpm = 0;

        private DateTime StartTime;
        private int Taps = 0;
        private bool Tapping = false;

        public BPMTapper()
        {
            inst = this;
            InitializeComponent();
        }

        private void TapButton_Click(object sender, EventArgs e)
        {
            Taps += 1;
            if (!Tapping)
            {
                Tapping = true;
                StartTime = DateTime.Now;
                BPM.Text = "0";
                BPMDecimals.Text = "0";
            }
            else
            {
                var mins = (DateTime.Now - StartTime).TotalMilliseconds / 60000;
                Bpm = (Taps - 1) / mins;
                BPM.Text = ((int)(Bpm + 0.5d)).ToString();
                BPMDecimals.Text = Math.Round(Bpm, (int)DecimalPlaces.Value).ToString();
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            Tapping = false;
            Taps = 0;
            Bpm = 0;
            BPM.Text = "";
            BPMDecimals.Text = "";
        }

        private bool ButtonsFocused()
        {
            return TapButton.Focused || ResetButton.Focused;
        }

        private void BPMTapper_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!DecimalPlaces.Focused && (e.KeyChar != ' ' || !ButtonsFocused()))
            {
                Taps += 1;
                if (!Tapping)
                {
                    Tapping = true;
                    StartTime = DateTime.Now;
                    BPM.Text = "0";
                    BPMDecimals.Text = "0";
                }
                else
                {
                    var mins = (DateTime.Now - StartTime).TotalMilliseconds / 60000;
                    Bpm = (Taps - 1) / mins;
                    BPM.Text = ((int)(Bpm + 0.5d)).ToString();
                    BPMDecimals.Text = Math.Round(Bpm, (int)DecimalPlaces.Value).ToString();
                }
            }
        }
    }
}
