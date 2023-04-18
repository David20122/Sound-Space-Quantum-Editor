using OpenTK.Mathematics;

namespace SSQE_Player.Types
{
    internal class Cube
    {
        public float Z;
        public float X;
        public float Y;

        public long Ms;

        public Color4 Color;

        public Cube(float z, float x, float y, long ms, Color4 color)
        {
            Z = z;
            X = x;
            Y = y;

            Ms = ms;

            Color = color;
        }
    }
}
