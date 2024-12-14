using New_SSQE.Audio;
using New_SSQE.GUI.Font;
using New_SSQE.Preferences;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiButton : WindowControl
    {
        public int ID;

        public string Text;
        private string prevText;

        public bool Hovering;

        private float alpha = 0f;
        private float prevAlpha = 0f;
        private readonly Color textColor = Color.White;

        public bool HasSubTexture = false;

        public GuiButton(float x, float y, float w, float h, int id, string text, int textSize, bool lockSize = false, bool moveWithOffset = false, string font = "main") : base(x, y, w, h)
        {
            ID = id;

            Text = text;
            prevText = text;
            Font = font;

            TextSize = textSize;
            OriginTextSize = textSize;

            LockSize = lockSize;
            MoveWithOffset = moveWithOffset;

            if (text != "")
                tHandle = GL.GenTexture();

            Init();
        }

        public GuiButton(float x, float y, float w, float h, int id, string text, int textSize) : this(x, y, w, h, id, text, textSize, false, false, "main") { }
        public GuiButton(float x, float y, float w, float h, int id, string text, int textSize, string font = "main") : this(x, y, w, h, id, text, textSize, false, false, font) { }
        public GuiButton(int id, string text, int textSize, string font = "main") : this(0, 0, 0, 0, id, text, textSize, false, false, font) { }
        public GuiButton(float x, float y, float w, float h, int id) : this(x, y, w, h, id, "", 0, false, false, "main") { }
        public GuiButton(int id) : this(0, 0, 0, 0, id, "", 0, false, false, "main") { }

        public override void Render(float mousex, float mousey, float frametime)
        {
            Hovering = Rect.Contains(mousex, mousey);

            if (!HasSubTexture)
            {
                alpha = MathHelper.Clamp((alpha / 0.025f) + (Hovering ? 10 : -10) * frametime, 0, 1) * 0.025f;
                
                if (alpha != prevAlpha || prevText != Text)
                {
                    Update();

                    prevAlpha = alpha;
                    prevText = Text;
                }

                GL.BindVertexArray(VaO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                GL.DrawArrays(PrimitiveType.TriangleStrip, 6, 10);
            }
        }

        public override void RenderTexture()
        {
            GL.Uniform4f(TexColorLocation, textColor.R / 255f, textColor.G / 255f, textColor.B / 255f, textColor.A / 255f);
            FontRenderer.RenderData(Font, FontVertices);
        }

        public override Tuple<float[], float[]> GetVertices()
        {
            float fillColor = 0.1f + alpha;
            float outlineColor = 0.2f + alpha;

            float[] fill = GLU.Rect(Rect, fillColor, fillColor, fillColor);
            float[] outline = GLU.Outline(Rect, 2, outlineColor, outlineColor, outlineColor);

            List<float> vertices = new(fill);
            vertices.AddRange(outline);

            float txW = FontRenderer.GetWidth(Text, TextSize, Font);
            float txH = FontRenderer.GetHeight(TextSize, Font);
            float txX = Rect.X + Rect.Width / 2f - txW / 2f;
            float txY = Rect.Y + Rect.Height / 2f - txH / 2f;

            FontVertices = FontRenderer.Print(txX, txY, Text, TextSize, Font);

            return new(vertices.ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right)
        {
            if (Hovering)
            {
                SoundPlayer.Play(Settings.clickSound.Value);
                OnButtonClicked(ID);
            }
        }
    }
}
