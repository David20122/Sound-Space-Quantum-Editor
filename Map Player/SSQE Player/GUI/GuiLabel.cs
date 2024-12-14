using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;

namespace SSQE_Player.GUI
{
    internal class GuiLabel
    {
        public RectangleF Rect;
        public RectangleF OriginRect;

        public string Text;
        private string prevText;
        public int TextSize;
        public int OriginTextSize;

        public bool Visible = true;

        private Vector4[] FontVertices;

        private readonly int TexColorLocation;

        private bool Centered;
        private Color Color;
        private Color prevColor = Color.White;

        public GuiLabel(float posx, float posy, float sizex, float sizey, string text, int textSize, bool centered = true, Color? color = null)
        {
            Rect = new(posx, posy, sizex, sizey);
            OriginRect = new(posx, posy, sizex, sizey);

            TexColorLocation = GL.GetUniformLocation(Shader.FontTexProgram, "TexColor");

            Color = color ?? Color.White;

            Text = text;
            prevText = text;

            TextSize = textSize;
            OriginTextSize = textSize;

            Centered = centered;

            Update();
        }

        public void Render()
        {
            if (!Visible)
                return;

            if (prevText != Text || prevColor != Color)
            {
                Update();

                prevText = Text;
                prevColor = Color;
            }

            GL.Uniform4f(TexColorLocation, Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);
            FontRenderer.RenderData(FontVertices);
        }

        public void Update()
        {
            float txX = Rect.X;
            float txY = Rect.Y;
            float txW = Rect.Width;
            float txH = Rect.Height;

            if (Centered)
            {
                int width = FontRenderer.GetWidth(Text, TextSize);
                int height = FontRenderer.GetHeight(TextSize);

                txX += txW / 2f - width / 2f;
                txY += txH / 2f - height / 2f;
            }

            FontVertices = FontRenderer.Print(txX, txY, Text, TextSize);
        }
    }
}
