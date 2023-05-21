using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Collections.Generic;

namespace New_SSQE.GUI
{
    internal class GuiCheckbox : WindowControl
    {
        public string Text;

        private readonly string setting;
        public bool? Toggle = null;

        private float alpha;
        private float prevAlpha;
        private Color textColor;
        private Color prevColor = Color.White;

        public GuiCheckbox(float posx, float posy, float sizex, float sizey, string Setting, string text, int textSize, bool lockSize = false, bool moveWithOffset = false, string font = "main") : base(posx, posy, sizex, sizey)
        {
            setting = Setting;

            Text = text;
            Font = font;

            TextSize = textSize;
            OriginTextSize = textSize;

            LockSize = lockSize;
            MoveWithOffset = moveWithOffset;

            Init();
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            alpha = (Toggle ?? Settings.settings[setting]) ? Math.Min(1, alpha + frametime * 8) : Math.Max(0, alpha - frametime * 8);

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
            var colored = MainWindow.Instance.CurrentWindow is GuiWindowEditor || MainWindow.Instance.CurrentWindow is GuiWindowKeybinds;

            var color1 = colored ? Settings.settings["color1"] : Color.FromArgb(255, 255, 255);
            var color2 = colored ? Settings.settings["color2"] : Color.FromArgb(50, 50, 50);

            float[] fill = GLU.Rect(Rect, 0.05f, 0.05f, 0.05f);
            float[] outline = GLU.Outline(Rect, 2, 0.2f, 0.2f, 0.2f);

            var checkSizeX = Rect.Width * 0.75f * alpha;
            var checkSizeY = Rect.Height * 0.75f * alpha;
            var gapX = (Rect.Width - checkSizeX) / 2;
            var gapY = (Rect.Height - checkSizeY) / 2;

            float[] check = GLU.Rect(Rect.X + gapX, Rect.Y + gapY, checkSizeX, checkSizeY, color2.R / 255f, color2.G / 255f, color2.B / 255f);

            List<float> vertices = new(fill);
            vertices.AddRange(outline);
            vertices.AddRange(check);

            float txH = FontRenderer.GetHeight(TextSize, Font);
            float txX = Rect.Right + Rect.Height / 4f;
            float txY = Rect.Y + Rect.Height / 2f - txH / 2f;

            FontVertices = FontRenderer.Print(txX, txY, Text, TextSize, Font);
            textColor = color1;

            return new Tuple<float[], float[]>(vertices.ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right)
        {
            if (setting != "")
            {
                Settings.settings[setting] ^= true;

                switch (setting)
                {
                    case "numpad":
                        Settings.RefreshKeyMapping();
                        break;
                }

                MainWindow.Instance.SoundPlayer.Play(Settings.settings["clickSound"]);
            }
        }
    }
}
