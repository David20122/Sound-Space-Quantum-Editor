using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.GUI
{
	class GuiCheckbox : Gui
	{
        public string text;
        public int textSize;
        public string font;

        public bool Visible = true;
        public bool lockSize;
        public bool moveWithOffset;

        private string setting;
        public bool? Toggle = null;

        public RectangleF originRect;
        public int originTextSize;

        private float alpha;

        public GuiCheckbox(float posx, float posy, float sizex, float sizey, string Setting, string Text, int TextSize, bool LockSize = false, bool MoveWithOffset = false, string Font = "main") : base(posx, posy, sizex, sizey)
        {
            setting = Setting;

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
            if (Visible)
            {
                var colored = MainWindow.Instance.CurrentWindow is GuiWindowEditor || MainWindow.Instance.CurrentWindow is GuiWindowKeybinds;

                var color1 = colored ? Settings.settings["color1"] : Color.FromArgb(255, 255, 255);
                var color2 = colored ? Settings.settings["color2"] : Color.FromArgb(50, 50, 50);

                GL.Color3(0.05f, 0.05f, 0.05f);
                GLSpecial.Rect(rect);
                GL.Color3(0.2f, 0.2f, 0.2f);
                GLSpecial.Outline(rect);

                alpha = (Toggle ?? Settings.settings[setting]) ? Math.Min(1, alpha + frametime * 8) : Math.Max(0, alpha - frametime * 8);

                var checkSizeX = rect.Width * 0.75f * alpha;
                var checkSizeY = rect.Height * 0.75f * alpha;
                var gapX = (rect.Width - checkSizeX) / 2;
                var gapY = (rect.Height - checkSizeY) / 2;

                if (checkSizeX > 0)
                {
                    GL.Color3(color2);
                    GLSpecial.Rect(rect.X + gapX, rect.Y + gapY, checkSizeX, checkSizeY);
                }

                GL.Color3(color1);
                RenderText(text, (int)(rect.Right + rect.Height / 4), (int)(rect.Y + rect.Height / 2 - TextHeight(textSize) / 2f), textSize);
            }
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            if (setting != "")
            {
                var current = Settings.settings[setting];

                Settings.settings[setting] = !current;

                switch (setting)
                {
                    case "numpad":
                        Settings.RefreshKeymapping();
                        break;
                }

                MainWindow.Instance.SoundPlayer.Play(Settings.settings["clickSound"]);
            }
        }
    }
}