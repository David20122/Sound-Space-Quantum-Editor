using OpenTK.Mathematics;
using System.Drawing;

namespace New_SSQE
{
    internal class GLU
    {
        public static float[] Rect(float x, float y, float w, float h, params float[] c)
        {
            float a = c.Length == 4 ? c[3] : 1f;

            return new float[]
            {
                x, y, c[0], c[1], c[2], a,
                x + w, y, c[0], c[1], c[2], a,
                x, y + h, c[0], c[1], c[2], a,

                x + w, y + h, c[0], c[1], c[2], a,
                x, y + h, c[0], c[1], c[2], a,
                x + w, y, c[0], c[1], c[2], a
            };
        }

        public static float[] Rect(RectangleF rect, params float[] c)
        {
            return Rect(rect.X, rect.Y, rect.Width, rect.Height, c);
        }

        public static float[] TexturedRect(float x, float y, float w, float h, float a, float tx = 0f, float ty = 0f, float tw = 1f, float th = 1f)
        {
            return new float[]
            {
                x, y, tx, ty, a,
                x + w, y, tx + tw, ty, a,
                x, y + h, tx, ty + th, a,

                x + w, y + h, tx + tw, ty + th, a,
                x, y + h, tx, ty + th, a,
                x + w, y, tx + tw, ty, a
            };
        }

        public static float[] TexturedRect(RectangleF rect, float a, float tx = 0f, float ty = 0f, float tw = 1f, float th = 1f)
        {
            return TexturedRect(rect.X, rect.Y, rect.Width, rect.Height, a, tx, ty, tw, th);
        }

        public static float[] Outline(float x, float y, float w, float h, float lw, params float[] c)
        {
            float a = c.Length == 4 ? c[3] : 1f;

            x -= lw / 2f;
            y -= lw / 2f;
            w += lw;
            h += lw;

            return new float[]
            {
                x, y, c[0], c[1], c[2], a,
                x + lw, y + lw, c[0], c[1], c[2], a,
                x + w, y, c[0], c[1], c[2], a,

                x + w - lw, y + lw, c[0], c[1], c[2], a,
                x + w, y + h, c[0], c[1], c[2], a,
                x + w - lw, y + h - lw, c[0], c[1], c[2], a,

                x, y + h, c[0], c[1], c[2], a,
                x + lw, y + h - lw, c[0], c[1], c[2], a,
                x, y, c[0], c[1], c[2], a,

                x + lw, y + lw, c[0], c[1], c[2], a,
            };
        }

        public static float[] Outline(RectangleF rect, float lw, params float[] c)
        {
            return Outline(rect.X, rect.Y, rect.Width, rect.Height, lw, c);
        }

        public static float[] OutlineAsTriangles(float x, float y, float w, float h, float lw, params float[] c)
        {
            float a = c.Length == 4 ? c[3] : 1f;

            x -= lw / 2;
            y -= lw / 2;

            return new float[]
            {
                x, y, c[0], c[1], c[2], a,
                x + w, y, c[0], c[1], c[2], a,
                x, y + lw, c[0], c[1], c[2], a,

                x + w, y + lw, c[0], c[1], c[2], a,
                x, y + lw, c[0], c[1], c[2], a,
                x + w, y, c[0], c[1], c[2], a,

                x + w + lw, y, c[0], c[1], c[2], a,
                x + w + lw, y + h, c[0], c[1], c[2], a,
                x + w, y, c[0], c[1], c[2], a,

                x + w, y + h, c[0], c[1], c[2], a,
                x + w, y, c[0], c[1], c[2], a,
                x + w + lw, y + h, c[0], c[1], c[2], a,

                x + w + lw, y + h + lw, c[0], c[1], c[2], a,
                x + lw, y + h + lw, c[0], c[1], c[2], a,
                x + w + lw, y + h, c[0], c[1], c[2], a,

                x + lw, y + h, c[0], c[1], c[2], a,
                x + w + lw, y + h, c[0], c[1], c[2], a,
                x + lw, y + h + lw, c[0], c[1], c[2], a,

                x, y + h + lw, c[0], c[1], c[2], a,
                x, y + lw, c[0], c[1], c[2], a,
                x + lw, y + h + lw, c[0], c[1], c[2], a,

                x + lw, y + lw, c[0], c[1], c[2], a,
                x + lw, y + h + lw, c[0], c[1], c[2], a,
                x, y + lw, c[0], c[1], c[2], a,
            };
        }

        public static float[] OutlineAsTriangles(RectangleF rect, float lw, params float[] c)
        {
            return OutlineAsTriangles(rect.X, rect.Y, rect.Width, rect.Height, lw, c);
        }

        public static float[] Line(float x1, float y1, float x2, float y2, float lw, params float[] c)
        {
            float a = c.Length == 4 ? c[3] : 1f;
            var horizontal = x1 != x2;

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
                x1, y1, c[0], c[1], c[2], a,
                x2, y1, c[0], c[1], c[2], a,
                x1, y2, c[0], c[1], c[2], a,

                x2, y2, c[0], c[1], c[2], a,
                x1, y2, c[0], c[1], c[2], a,
                x2, y1, c[0], c[1], c[2], a,
            };
        }

        public static float[] Circle(float x, float y, float radius, int sides, float angle, params float[] c)
        {
            float alpha = c.Length == 4 ? c[3] : 1f;

            var vertices = new float[sides * 6];

            for (int i = 0; i < sides; i++)
            {
                var a = MathHelper.DegreesToRadians(angle) + i / (float)sides * Math.PI * 2;
                var vx = Math.Cos(a) * radius;
                var vy = -Math.Sin(a) * radius;

                var index = i * 6;
                vertices[index] = (float)vx + x;
                vertices[index + 1] = (float)vy + y;
                vertices[index + 2] = c[0];
                vertices[index + 3] = c[1];
                vertices[index + 4] = c[2];
                vertices[index + 5] = alpha;
            }

            return vertices;
        }
    }
}
