using New_SSQE.GUI.Font;
using New_SSQE.Preferences;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiLabel : WindowControl
    {
        public string Text;

        public bool Centered;

        private string prevText = "";
        public Color Color = Color.White;
        private Color prevColor = Color.White;

        public GuiLabel(float x, float y, float w, float h, string text, int textSize, bool lockSize = false, bool moveWithOffset = false, string font = "main", bool centered = true, Setting<Color>? color = null) : base(x, y, w, h)
        {
            if (color != null)
                Color = color.Value;

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

        public GuiLabel(float x, float y, float w, float h, string text, int textSize, string font, bool centered = true, Setting<Color>? color = null) : this(x, y, w, h, text, textSize, false, false, font, centered, color) { }
        public GuiLabel(float x, float y, float w, float h, int textSize) : this(x, y, w, h, "", textSize, false, false, "main", true, null) { }
        public GuiLabel(int textSize) : this(0, 0, 0, 0, "", textSize, false, false, "main", true, null) { }

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
                int width = FontRenderer.GetWidth(Text, TextSize, Font);
                int height = FontRenderer.GetHeight(TextSize, Font);

                txX += txW / 2f - width / 2f;
                txY += txH / 2f - height / 2f;
            }

            FontVertices = FontRenderer.Print(txX, txY, Text, TextSize, Font);

            return new(Array.Empty<float>(), Array.Empty<float>());
        }
    }
}
