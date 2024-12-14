namespace SSQE_Player.GUI
{
    internal class GLU
    {
        public static float[] Line(float x1, float y1, float z1, float x2, float y2, float z2, float lw, params float[] c)
        {
            bool horizontal = x1 != x2;

            if (horizontal)
            {
                y1 -= lw / 2;
                y2 += lw / 2;
            }
            else
            {
                x1 -= lw / 2;
                x2 += lw / 2;
            }

            return new float[]
            {
                x1, y1, z1, c[0], c[1], c[2], c[3],
                x2, y1, z1, c[0], c[1], c[2], c[3],
                x1, y2, z2, c[0], c[1], c[2], c[3],

                x2, y2, z2, c[0], c[1], c[2], c[3],
                x1, y2, z2, c[0], c[1], c[2], c[3],
                x2, y1, z1, c[0], c[1], c[2], c[3],
            };
        }

        public static float[] FadingLine(float x1, float y1, float z1, float x2, float y2, float z2, float lw, params float[] c)
        {
            bool horizontal = x1 != x2;

            if (horizontal)
            {
                y1 -= lw / 2;
                y2 += lw / 2;
            }
            else
            {
                x1 -= lw / 2;
                x2 += lw / 2;
            }

            return new float[]
            {
                x1, y1, z1, c[0], c[1], c[2], c[3],
                x2, y1, z1, c[0], c[1], c[2], c[3],
                x1, y2, z2, 0, 0, 0, c[3],

                x2, y2, z2, 0, 0, 0, c[3],
                x1, y2, z2, 0, 0, 0, c[3],
                x2, y1, z1, c[0], c[1], c[2], c[3],
            };
        }

        public static float[] Outline(float x, float y, float z, float sx, float sy, float lw, params float[] c)
        {
            x -= lw / 2;
            y -= lw / 2;

            return new float[]
            {
                x, y, z, c[0], c[1], c[2], c[3],
                x + sx, y, z, c[0], c[1], c[2], c[3],
                x, y + lw, z, c[0], c[1], c[2], c[3],

                x + sx, y + lw, z, c[0], c[1], c[2], c[3],
                x, y + lw, z, c[0], c[1], c[2], c[3],
                x + sx, y, z, c[0], c[1], c[2], c[3],

                x + sx + lw, y, z, c[0], c[1], c[2], c[3],
                x + sx + lw, y + sy, z, c[0], c[1], c[2], c[3],
                x + sx, y, z, c[0], c[1], c[2], c[3],

                x + sx, y + sy, z, c[0], c[1], c[2], c[3],
                x + sx, y, z, c[0], c[1], c[2], c[3],
                x + sx + lw, y + sy, z, c[0], c[1], c[2], c[3],

                x + sx + lw, y + sy + lw, z, c[0], c[1], c[2], c[3],
                x + lw, y + sy + lw, z, c[0], c[1], c[2], c[3],
                x + sx + lw, y + sy, z, c[0], c[1], c[2], c[3],

                x + lw, y + sy, z, c[0], c[1], c[2], c[3],
                x + sx + lw, y + sy, z, c[0], c[1], c[2], c[3],
                x + lw, y + sy + lw, z, c[0], c[1], c[2], c[3],

                x, y + sy + lw, z, c[0], c[1], c[2], c[3],
                x, y + lw, z, c[0], c[1], c[2], c[3],
                x + lw, y + sy + lw, z, c[0], c[1], c[2], c[3],

                x + lw, y + lw, z, c[0], c[1], c[2], c[3],
                x + lw, y + sy + lw, z, c[0], c[1], c[2], c[3],
                x, y + lw, z, c[0], c[1], c[2], c[3],
            };
        }
    }
}
