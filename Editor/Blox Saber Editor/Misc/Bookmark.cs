using System;

namespace Sound_Space_Editor.Misc
{
    [Serializable]
    class Bookmark
    {
        public string Text;
        public long Ms;

        public Bookmark(string text, long ms)
        {
            Text = text;
            Ms = ms;
        }
    }
}
