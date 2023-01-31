using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor
{
	class GLSpecial
	{
        public static void Rect(float posx, float posy, float sizex, float sizey)
        {
            var xf = posx + sizex;
            var yf = posy + sizey;

            GL.Begin(PrimitiveType.Polygon);
            GL.Vertex2(posx, posy);
            GL.Vertex2(posx, yf);
            GL.Vertex2(xf, yf);
            GL.Vertex2(xf, posy);
            GL.End();
        }

        public static void Rect(RectangleF rect)
        {
            Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static void Outline(float posx, float posy, float sizex, float sizey)
        {
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);

            posx += 0.5f;
            posy += 0.5f;

            var xf = posx + sizex - 1;
            var yf = posy + sizey - 1;

            GL.Begin(PrimitiveType.Polygon);
            GL.Vertex2(posx, posy);
            GL.Vertex2(posx, yf);
            GL.Vertex2(xf, yf);
            GL.Vertex2(xf, posy);
            GL.End();

            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
        }

        public static void Outline(RectangleF rect)
        {
            Outline(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static void Line(float pos1x, float pos1y, float pos2x, float pos2y)
        {
            pos1x = (int)(pos1x + 0.5f);
            pos2x = (int)(pos2x + 0.5f);
            pos1y = (int)(pos1y + 0.5f);
            pos2y = (int)(pos2y + 0.5f);

            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(pos1x, pos1y);
            GL.Vertex2(pos2x, pos2y);
            GL.End();
        }

        public static void Circle(float posx, float posy, float radius, int sides = 8, bool outline = false)
        {
            GL.PolygonMode(MaterialFace.Front, outline ? PolygonMode.Line : PolygonMode.Fill);
            GL.Begin(PrimitiveType.Polygon);

            for (int i = 0; i < sides; i++)
            {
                var angle = i / (float)sides * Math.PI * 2;
                var x = Math.Cos(angle) * radius;
                var y = -Math.Sin(angle) * radius;

                GL.Vertex2(posx + x, posy + y);
            }

            GL.End();
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
        }

        public static int LoadTexture(string file)
        {
            var bitmap = new Bitmap(file);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out int tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return tex;
        }

        public static void Image(float posx, float posy, float sizex, float sizey, int textureID)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PushMatrix();
            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, textureID);

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0, 0);
            GL.Vertex2(posx, posy);

            GL.TexCoord2(1, 0);
            GL.Vertex2(posx + sizex, posy);

            GL.TexCoord2(1, 1);
            GL.Vertex2(posx + sizex, posy + sizey);

            GL.TexCoord2(0, 1);
            GL.Vertex2(posx, posy + sizey);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.PopMatrix();
        }

        public static void TexturedQuad(RectangleF rect, float us, float vs, float ue, float ve, int texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Translate(rect.X, rect.Y, 0);
            GL.Scale(rect.Width, rect.Height, 1);

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(us, vs);
            GL.Vertex2(0, 0);

            GL.TexCoord2(us, ve);
            GL.Vertex2(0, 1);

            GL.TexCoord2(ue, ve);
            GL.Vertex2(1, 1);

            GL.TexCoord2(ue, vs);
            GL.Vertex2(1, 0);

            GL.End();

            GL.Scale(1f / rect.Width, 1f / rect.Height, 1);
            GL.Translate(-rect.X, -rect.Y, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}