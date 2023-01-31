using System;
using System.Drawing;

namespace Sound_Space_Editor.Misc
{
    [Serializable]
    class Note
    {
        public float X;
        public float Y;
        public long Ms;

        public long DragStartMs;

        public bool Anchored;

        public Color Color;
        public Color GridColor;

        public Note(float x, float y, long ms)
        {
            X = x;
            Y = y;

            Ms = ms;
        }
    }
}
