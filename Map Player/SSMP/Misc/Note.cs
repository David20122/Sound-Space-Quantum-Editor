using System.Drawing;

namespace SSQE_Player.Misc
{
    class Note
    {
        public float X;
        public float Y;
        public long Ms;

        public Color Color;

        public Note(float x, float y, long ms)
        {
            X = x;
            Y = y;

            Ms = ms;
        }
    }
}
