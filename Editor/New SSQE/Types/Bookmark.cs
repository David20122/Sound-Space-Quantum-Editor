using System;

namespace New_SSQE
{
    [Serializable]
    internal class Bookmark
    {
        public string Text { get; set; }
        public long Ms { get; set; }
        public long EndMs { get; set; }

        public Bookmark(string text, long ms, long endMs)
        {
            Text = text;
            Ms = ms;
            EndMs = endMs;
        }
    }
}
