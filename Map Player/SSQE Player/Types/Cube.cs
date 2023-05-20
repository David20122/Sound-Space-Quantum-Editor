using System.Drawing;

namespace SSQE_Player.Types
{
    internal class Cube
    {
        public float Z;
        public float X;
        public float Y;

        public long Ms;

        public Color Color;

        public Cube(float z, float x, float y, long ms, Color color)
        {
            Z = z;
            X = x;
            Y = y;

            Ms = ms;

            Color = color;
        }
    }
}
