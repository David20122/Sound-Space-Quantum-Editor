using OpenTK.Graphics;

namespace SSQE_Player.Misc
{
    class Cube
    {
        public float Z;
        public float XIndex;
        public float YIndex;

        public long Ms;

        public Color4 Color;

        public Cube(float z, float xindex, float yindex, long ms, Color4 color)
        {
            Z = z;
            XIndex = xindex;
            YIndex = yindex;

            Ms = ms;

            Color = color;
        }
    }
}
