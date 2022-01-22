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
        public double Bpm = 0;

        private DateTime StartTime;
        private int Taps = 0;
        private bool Tapping = false;

        public BPMTapper()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void TapButton_Click(object sender, EventArgs e)
        {
            Taps += 1;
            if (!Tapping)
            {
                Tapping = true;
                StartTime = DateTime.Now;
                BPM.Text = "0";
            }
            else
            {
                var mins = (DateTime.Now - StartTime).TotalMilliseconds / 60000;
                Bpm = (int)((Taps - 1) / mins * 100) / 100d;
                BPM.Text = Bpm.ToString();
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            Tapping = false;
            Taps = 0;
            Bpm = 0;
            BPM.Text = "";
        }
    }
}
