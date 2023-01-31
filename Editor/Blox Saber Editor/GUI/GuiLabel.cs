using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.GUI
{
	class GuiLabel : Gui
	{
        public string text;
        public int textSize;
        public string font;
        public Color color = Color.White;

        public bool Visible = true;
        public bool lockSize;
        public bool moveWithOffset;

        public bool centered;

        public RectangleF originRect;
        public int originTextSize;

        public GuiLabel(float posx, float posy, float sizex, float sizey, string Text, int TextSize, bool LockSize = false, bool MoveWithOffset = false, string Font = "main", bool Centered = true) : base(posx, posy, sizex, sizey)
        {
            text = Text;
            textSize = TextSize;
            font = Font;

            lockSize = LockSize;
            moveWithOffset = MoveWithOffset;

            centered = Centered;

            originRect = new RectangleF(posx, posy, sizex, sizey);
            originTextSize = textSize;
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (Visible)
            {
                GL.Color4(color);

                if (centered)
                {
                    var width = TextWidth(text, textSize);
                    var height = TextHeight(textSize);

                    RenderText(text, rect.X + rect.Width / 2f - width / 2f, rect.Y + rect.Height / 2f - height / 2f, textSize, font);
                }
                else
                    RenderText(text, rect.X, rect.Y, textSize, font);
            }
        }
    }
}
