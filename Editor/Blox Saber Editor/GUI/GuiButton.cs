using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.GUI
{
	class GuiButton : Gui
	{
        public int ID;

        public string text;
        public int textSize;
        public string font;

        public bool Visible = true;
        public bool lockSize;
        public bool moveWithOffset;

        public bool hovering;

        public RectangleF originRect;
        public int originTextSize;

        protected int texture = -1;
        private float alpha;

        public GuiButton(float posx, float posy, float sizex, float sizey, int id, string Text, int TextSize, bool LockSize = false, bool MoveWithOffset = false, string Font = "main") : base(posx, posy, sizex, sizey)
        {
            ID = id;

            text = Text;
            textSize = TextSize;
            font = Font;

            lockSize = LockSize;
            moveWithOffset = MoveWithOffset;

            originRect = new RectangleF(posx, posy, sizex, sizey);
            originTextSize = textSize;
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (Visible && texture <= 0)
            {
                hovering = rect.Contains(mousex, mousey);

                alpha = MathHelper.Clamp(alpha + (hovering ? 10 : -10) * frametime, 0, 1) * 0.075f;

                GL.Color3(0.1f + alpha, 0.1f + alpha, 0.1f + alpha);
                GLSpecial.Rect(rect);

                GL.LineWidth(2f);

                GL.Color3(0.2f + alpha, 0.2f + alpha, 0.2f + alpha);
                GLSpecial.Outline(rect);

                var width = TextWidth(text, textSize, font);
                var height = TextHeight(textSize, font);

                GL.Color3(1f, 1f, 1f);
                RenderText(text, rect.X + rect.Width / 2f - width / 2f, rect.Y + rect.Height / 2f - height / 2f, textSize, font);
            }
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            MainWindow.Instance.SoundPlayer.Play(Settings.settings["clickSound"]);
        }
    }
}