using New_SSQE.Audio;
using New_SSQE.GUI.Font;
using New_SSQE.Preferences;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiCheckbox : WindowControl
    {
        public string Text;

        private readonly Setting<bool>? Bool;
        public bool? Toggle = null;

        private float alpha;
        private float prevAlpha;
        private Color textColor;
        private Color prevColor = Color.White;

        public GuiCheckbox(float x, float y, float w, float h, Setting<bool>? setting, string text, int textSize, bool lockSize = false, bool moveWithOffset = false, string font = "main") : base(x, y, w, h)
        {
            Bool = setting;

            Text = text;
            Font = font;

            TextSize = textSize;
            OriginTextSize = textSize;

            LockSize = lockSize;
            MoveWithOffset = moveWithOffset;

            Init();
        }

        public GuiCheckbox(float x, float y, float w, float h, Setting<bool> setting, string text, int textSize) : this(x, y, w, h, setting, text, textSize, false, false, "main") { }
        public GuiCheckbox(float x, float y, float w, float h) : this(x, y, w, h, null, "", 0, false, false, "main") { }
        public GuiCheckbox(float x, float y, float w, float h, string text, int textSize) : this(x, y, w, h, null, text, textSize, false, false, "main") { }

        public override void Render(float mousex, float mousey, float frametime)
        {
            alpha = (Toggle ?? Bool?.Value ?? false) ? Math.Min(1, alpha + frametime * 8) : Math.Max(0, alpha - frametime * 8);

            if (alpha != prevAlpha || prevColor != textColor)
            {
                Update();

                prevAlpha = alpha;
                prevColor = textColor;
            }

            GL.BindVertexArray(VaO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 6, 10);
            GL.DrawArrays(PrimitiveType.Triangles, 16, 6);
        }

        public override void RenderTexture()
        {
            GL.Uniform4f(TexColorLocation, textColor.R / 255f, textColor.G / 255f, textColor.B / 255f, textColor.A / 255f);
            FontRenderer.RenderData(Font, FontVertices);
        }

        public override Tuple<float[], float[]> GetVertices()
        {
            bool colored = MainWindow.Instance.CurrentWindow is GuiWindowEditor || MainWindow.Instance.CurrentWindow is GuiWindowKeybinds;

            Color color1 = colored ? Settings.color1.Value : Color.FromArgb(255, 255, 255);
            Color color2 = colored ? Settings.color2.Value : Color.FromArgb(75, 75, 75);

            float[] fill = GLU.Rect(Rect, 0.05f, 0.05f, 0.05f);
            float[] outline = GLU.Outline(Rect, 2, 0.2f, 0.2f, 0.2f);

            float checkSizeX = Rect.Width * 0.75f * alpha;
            float checkSizeY = Rect.Height * 0.75f * alpha;
            float gapX = (Rect.Width - checkSizeX) / 2;
            float gapY = (Rect.Height - checkSizeY) / 2;

            float[] check = GLU.Rect(Rect.X + gapX, Rect.Y + gapY, checkSizeX, checkSizeY, color2.R / 255f, color2.G / 255f, color2.B / 255f);

            List<float> vertices = new(fill);
            vertices.AddRange(outline);
            vertices.AddRange(check);

            float txH = FontRenderer.GetHeight(TextSize, Font);
            float txX = Rect.Right + Rect.Height / 4f;
            float txY = Rect.Y + Rect.Height / 2f - txH / 2f;

            FontVertices = FontRenderer.Print(txX, txY, Text, TextSize, Font);
            textColor = color1;

            return new(vertices.ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right)
        {
            if (Bool != null)
            {
                Bool.Value ^= true;

                switch (Bool.Name)
                {
                    case "numpad":
                        Settings.RefreshKeyMapping();
                        break;
                }

                SoundPlayer.Play(Settings.clickSound.Value);
            }
        }
    }
}
