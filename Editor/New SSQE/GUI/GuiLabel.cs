using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiLabel : WindowControl
    {
        public string Text;

        public bool Centered;

        private string prevText = "";
        public Color Color;
        private Color prevColor = Color.White;

        public GuiLabel(float posx, float posy, float sizex, float sizey, string text, int textSize, bool lockSize = false, bool moveWithOffset = false, string font = "main", bool centered = true, Color? color = null) : base(posx, posy, sizex, sizey)
        {
            Color = color ?? Color.White;

            Text = text;
            prevText = text;
            Font = font;

            TextSize = textSize;
            OriginTextSize = textSize;

            LockSize = lockSize;
            MoveWithOffset = moveWithOffset;

            Centered = centered;

            Init();
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (prevText != Text || prevColor != Color)
            {
                Update();

                prevText = Text;
                prevColor = Color;
            }
        }

        public override void RenderTexture()
        {
            GL.Uniform4f(TexColorLocation, Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);
            FontRenderer.RenderData(Font, FontVertices);
        }

        public override Tuple<float[], float[]> GetVertices()
        {
            float txX = Rect.X;
            float txY = Rect.Y;
            float txW = Rect.Width;
            float txH = Rect.Height;

            if (Centered)
            {
                var width = FontRenderer.GetWidth(Text, TextSize, Font);
                var height = FontRenderer.GetHeight(TextSize, Font);

                txX += txW / 2f - width / 2f;
                txY += txH / 2f - height / 2f;
            }

            FontVertices = FontRenderer.Print(txX, txY, Text, TextSize, Font);

            return new Tuple<float[], float[]>(Array.Empty<float>(), Array.Empty<float>());
        }
    }
}
