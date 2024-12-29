using OpenTK.Mathematics;
using System.Drawing;

namespace New_SSQE.NewGUI
{
    internal class GLVerts
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
            => Rect(rect.X, rect.Y, rect.Width, rect.Height, c);
        public static float[] Rect(float x, float y, float w, float h, Color color, float alpha = 1)
            => Rect(x, y, w, h, color.R / 255f, color.G / 255f, color.B / 255f, alpha);
        public static float[] Rect(RectangleF rect, Color color, float alpha = 1)
            => Rect(rect.X, rect.Y, rect.Width, rect.Height, color.R / 255f, color.G / 255f, color.B / 255f, alpha);



        public static float[] Outline(float x, float y, float w, float h, float lw, params float[] c)
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

        public static float[] Outline(RectangleF rect, float lw, params float[] c)
            => Outline(rect.X, rect.Y, rect.Width, rect.Height, lw, c);
        public static float[] Outline(float x, float y, float w, float h, float lw, Color color, float alpha = 1)
            => Outline(x, y, w, h, lw, color.R / 255f, color.G / 255f, color.B / 255f, alpha);
        public static float[] Outline(RectangleF rect, float lw, Color color, float alpha = 1)
            => Outline(rect.X, rect.Y, rect.Width, rect.Height, lw, color.R / 255f, color.G / 255f, color.B / 255f, alpha);



        public static float[] Texture(float x, float y, float w, float h, float tx = 0, float ty = 0, float tw = 1, float th = 1, float alpha = 1)
        {
            return new float[]
            {
                x, y, tx, ty, alpha,
                x + w, y, tx + tw, ty, alpha,
                x, y + h, tx, ty + th, alpha,

                x + w, y + h, tx + tw, ty + th, alpha,
                x, y + h, tx, ty + th, alpha,
                x + w, y, tx + tw, ty, alpha
            };
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

        public static float[] Line(float x1, float y1, float x2, float y2, float lw, Color color, float alpha = 1)
            => Line(x1, y1, x2, y2, lw, color.R / 255f, color.G / 255f, color.B / 255f, alpha);



        public static float[] Circle(float x, float y, float radius, int sides, float angle, params float[] c)
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

        public static float[] Circle(float x, float y, float radius, int sides, float angle, Color color, float alpha = 1)
            => Circle(x, y, radius, sides, angle, color.R / 255f, color.G / 255f, color.B / 255f, alpha);



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

        public static float[] CircleOutline(float x, float y, float radius, float width, int sides, float angle, Color color, float alpha = 1)
            => CircleOutline(x, y, radius, width, sides, angle, color.R / 255f, color.G / 255f, color.B / 255f, alpha);
    }
}
