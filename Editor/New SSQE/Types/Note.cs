using System;
using System.Globalization;

namespace New_SSQE
{
    [Serializable]
    internal class Note
    {
        public float X;
        public float Y;
        public long Ms;

        public long DragStartMs;
        public bool Selected;

        public bool Anchored;

        public Note(float x, float y, long ms)
        {
            X = x;
            Y = y;
            Ms = ms;
        }

        public Note(string data, CultureInfo culture)
        {
            var split = data.Split('|');

            X = 2 - float.Parse(split[0], culture);
            Y = 2 - float.Parse(split[1], culture);
            Ms = long.Parse(split[2]);
        }

        public string ToString(CultureInfo culture)
        {
            var x = Math.Round(2 - X, 2);
            var y = Math.Round(2 - Y, 2);

            return $",{x.ToString(culture)}|{y.ToString(culture)}|{Ms}";
        }
    }
}
