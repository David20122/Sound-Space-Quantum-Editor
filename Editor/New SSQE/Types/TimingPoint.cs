using System;
using System.Globalization;

namespace New_SSQE
{
    [Serializable]
    internal class TimingPoint
    {
        public float BPM { get; set; }
        public long Ms { get; set; }

        public long DragStartMs;

        public TimingPoint(float bpm, long ms)
        {
            BPM = bpm;
            Ms = ms;
        }

        public TimingPoint(string data, CultureInfo culture)
        {
            var split = data.Split('|');

            BPM = float.Parse(split[0], culture);
            Ms = long.Parse(split[1]);
        }

        public string ToString(CultureInfo culture)
        {
            var bpm = Math.Round(BPM, 2);

            return $",{bpm.ToString(culture)}|{Ms}";
        }
    }
}
