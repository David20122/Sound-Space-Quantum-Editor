using System;
using System.Globalization;
using System.Drawing;

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

        public Color Color;
        public Color GridColor;

        public Note(float x, float y, long ms)
        {
            X = x;
            Y = y;
            Ms = ms;
        }

        public string ToString(CultureInfo culture)
        {
            var x = Math.Round(2 - X, 2);
            var y = Math.Round(2 - Y, 2);

            return $",{x.ToString(culture)}|{y.ToString(culture)}|{Ms}";
        }

        public static Note New(string data, CultureInfo culture)
        {
            var split = data.Split('|');

            var x = 2 - float.Parse(split[0], culture);
            var y = 2 - float.Parse(split[1], culture);
            var ms = long.Parse(split[2]);

            return new Note(x, y, ms);
        }
    }
}
