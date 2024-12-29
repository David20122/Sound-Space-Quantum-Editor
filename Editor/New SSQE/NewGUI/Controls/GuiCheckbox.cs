using New_SSQE.Audio;
using New_SSQE.Preferences;
using System.Drawing;

namespace New_SSQE.NewGUI.Controls
{
    internal class GuiCheckbox : InteractiveControl
    {
        public bool Toggle = false;

        private Setting<bool>? setting;
        private float checkSize = 0f;

        public GuiCheckbox(float x, float y, float w, float h, Setting<bool>? setting = null, string text = "", int textSize = 0, string font = "main") : base(x, y, w, h, text, textSize, font, false)
        {
            if (setting != null)
                Toggle = setting.Value;

            this.setting = setting;
        }

        public override float[] Draw()
        {
            float width = Math.Min(rect.Width, rect.Height);
            float wGap = (rect.Width - width) / 2;
            float hGap = (rect.Height - width) / 2;
            RectangleF squareRect = new(rect.X + wGap, rect.Y + hGap, width, width);

            float[] fill = GLVerts.Rect(squareRect, 0.05f, 0.05f, 0.05f);
            float[] outline = GLVerts.Outline(squareRect, 0.2f, 0.2f, 0.2f);

            float cWidth = width * 0.75f * checkSize;
            float cwGap = (rect.Width - cWidth) / 2;
            float chGap = (rect.Height - cWidth) / 2;
            RectangleF checkRect = new(rect.X + cwGap, rect.Y + chGap, cWidth, cWidth);

            SetColor(Settings.color1.Value);
            float[] check = GLVerts.Rect(checkRect, Settings.color2.Value);

            return fill.Concat(outline).Concat(check).ToArray();
        }

        public override void PreRender(float mousex, float mousey, float frametime)
        {
            base.PreRender(mousex, mousey, frametime);

            float prevSize = checkSize;
            checkSize = Toggle ? Math.Min(1, checkSize + frametime * 8) : Math.Max(0, checkSize - frametime * 8);

            if (checkSize != prevSize)
                Update();
        }

        public override void MouseClickLeft(float x, float y)
        {
            base.MouseClickLeft(x, y);

            if (Hovering)
            {
                SoundPlayer.Play(Settings.clickSound.Value);
                Toggle ^= true;

                if (setting != null)
                    setting.Value = Toggle;
            }
        }
    }
}
