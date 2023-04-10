using OpenTK.Graphics.OpenGL;
using SkiaSharp;
using System;
using System.Drawing;
using System.IO;

namespace New_SSQE.GUI
{
    internal class GuiSquare : WindowControl
    {
        public Color Color;
        private Color prevColor;

        private readonly bool IsTextured;
        private readonly string FileName = "";
        private readonly string TextureName = "";

        private readonly bool Outline;

        public GuiSquare(float posx, float posy, float sizex, float sizey, Color color, bool outline = false, string fileName = "", string textureName = "") : base(posx, posy, sizex, sizey)
        {
            Color = color;
            prevColor = Color;
            Outline = outline;

            if (fileName != "" && File.Exists(fileName))
            {
                IsTextured = true;
                FileName = fileName;
                TextureName = textureName;

                using var fs = File.OpenRead(FileName);
                tHandle = TextureManager.GetOrRegister(TextureName, SKBitmap.Decode(fs), false);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, tHandle);
            }

            Init();
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (!IsTextured)
            {
                if (prevColor != Color)
                {
                    Update();

                    prevColor = Color;
                }

                var type = Outline ? PrimitiveType.TriangleStrip : PrimitiveType.Triangles;
                var indexCount = Outline ? 10 : 6;

                GL.BindVertexArray(VaO);
                GL.DrawArrays(type, 0, indexCount);
            }
        }

        public override void RenderTexture()
        {
            if (IsTextured)
            {
                TextureManager.SetActive(0);

                GL.BindVertexArray(tVaO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }
        }

        public override Tuple<float[], float[]> GetVertices()
        {
            var c = new float[] { Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f };

            float[] fill = Outline ? GLU.Outline(Rect, 2, c) : GLU.Rect(Rect, c);
            float[] texture = Array.Empty<float>();

            if (IsTextured)
                texture = GLU.TexturedRect(Rect, Color.A / 255f);

            return new Tuple<float[], float[]>(fill, texture);
        }
    }
}
