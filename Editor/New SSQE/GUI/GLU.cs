using OpenTK.Mathematics;
using System.Drawing;

namespace New_SSQE.GUI
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

        public static float[] TexturedRectNoAlpha(float x, float y, float w, float h, float tx = 0f, float ty = 0f, float tw = 1f, float th = 1f)
        {
            return new float[]
            {
                x, y, tx, ty,
                x + w, y, tx + tw, ty,
                x, y + h, tx, ty + th,

                x + w, y + h, tx + tw, ty + th,
                x, y + h, tx, ty + th,
                x + w, y, tx + tw, ty
            };
        }

        public static float[] TexturedRectNoAlpha(RectangleF rect, float tx = 0f, float ty = 0f, float tw = 1f, float th = 1f)
        {
            return TexturedRectNoAlpha(rect.X, rect.Y, rect.Width, rect.Height, tx, ty, tw, th);
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

            float[] vertices = new float[sides * 6];
            double rad = MathHelper.DegreesToRadians(angle);

            int index = 0;

            for (int i = 0; i < sides; i++)
            {
                double a = rad + i / (float)sides * Math.PI * 2;
                double vx = Math.Cos(a) * radius;
                double vy = -Math.Sin(a) * radius;

                vertices[index++] = (float)vx + x;
                vertices[index++] = (float)vy + y;
                vertices[index++] = c[0];
                vertices[index++] = c[1];
                vertices[index++] = c[2];
                vertices[index++] = alpha;
            }

            return vertices;
        }

        public static float[] CircleAsTriangles(float x, float y, float radius, int sides, float angle, params float[] c)
        {
            float alpha = c.Length == 4 ? c[3] : 1f;

            Vector2[] vertices = new Vector2[sides];
            double rad = MathHelper.DegreesToRadians(angle);

            for (int i = 0; i < sides; i++)
            {
                double a = rad + i / (float)sides * Math.PI * 2;
                double vx = Math.Cos(a) * radius;
                double vy = -Math.Sin(a) * radius;

                vertices[i] = ((float)vx + x, (float)vy + y);
            }

            float[] final = new float[sides * 18];
            int index = 0;

            for (int i = 0; i < sides; i++)
            {
                int next = i + 1 == sides ? 0 : i + 1;

                final[index++] = vertices[i].X;
                final[index++] = vertices[i].Y;
                final[index++] = c[0];
                final[index++] = c[1];
                final[index++] = c[2];
                final[index++] = alpha;

                final[index++] = x;
                final[index++] = y;
                final[index++] = c[0];
                final[index++] = c[1];
                final[index++] = c[2];
                final[index++] = alpha;

                final[index++] = vertices[next].X;
                final[index++] = vertices[next].Y;
                final[index++] = c[0];
                final[index++] = c[1];
                final[index++] = c[2];
                final[index++] = alpha;
            }

            return final;
        }

        public static float[] CircleOutline(float x, float y, float radius, float width, int sides, float angle, params float[] c)
        {
            float alpha = c.Length == 4 ? c[3] : 1f;

            Vector4[] vertices = new Vector4[sides];
            double rad = MathHelper.DegreesToRadians(angle);

            for (int i = 0; i < sides; i++)
            {
                double a = rad + i / (float)sides * Math.PI * 2;
                float vx1 = (float)Math.Cos(a) * (radius + width / 2);
                float vy1 = (float)-Math.Sin(a) * (radius + width / 2);

                float vx2 = (float)Math.Cos(a) * (radius - width / 2);
                float vy2 = (float)-Math.Sin(a) * (radius - width / 2);

                vertices[i] = (vx1 + x, vy1 + y, vx2 + x, vy2 + y);
            }

            float[] final = new float[sides * 36];
            int index = 0;

            for (int i = 0; i < sides; i++)
            {
                int next = i + 1 == sides ? 0 : i + 1;

                final[index++] = vertices[i].X;
                final[index++] = vertices[i].Y;
                final[index++] = c[0];
                final[index++] = c[1];
                final[index++] = c[2];
                final[index++] = alpha;

                final[index++] = vertices[i].Z;
                final[index++] = vertices[i].W;
                final[index++] = c[0];
                final[index++] = c[1];
                final[index++] = c[2];
                final[index++] = alpha;

                final[index++] = vertices[next].X;
                final[index++] = vertices[next].Y;
                final[index++] = c[0];
                final[index++] = c[1];
                final[index++] = c[2];
                final[index++] = alpha;

                final[index++] = vertices[next].X;
                final[index++] = vertices[next].Y;
                final[index++] = c[0];
                final[index++] = c[1];
                final[index++] = c[2];
                final[index++] = alpha;

                final[index++] = vertices[next].Z;
                final[index++] = vertices[next].W;
                final[index++] = c[0];
                final[index++] = c[1];
                final[index++] = c[2];
                final[index++] = alpha;

                final[index++] = vertices[i].Z;
                final[index++] = vertices[i].W;
                final[index++] = c[0];
                final[index++] = c[1];
                final[index++] = c[2];
                final[index++] = alpha;
            }

            return final;
        }
    }
}
