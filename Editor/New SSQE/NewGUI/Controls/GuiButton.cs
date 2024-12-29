using New_SSQE.Audio;
using New_SSQE.Preferences;
using OpenTK.Mathematics;

namespace New_SSQE.NewGUI.Controls
{
    internal class GuiButton : InteractiveControl
    {
        private float hoverTime = 0f;
        protected bool RightResponsive = false;

        private float Fill => 0.1f + hoverTime;
        private float Outline => 0.1f + Fill;
        
        public GuiButton(float x, float y, float w, float h, string text = "", int textSize = 0, string font = "main", bool centered = true) : base(x, y, w, h, text, textSize, font, centered)
        {

        }

        public override float[] Draw()
        {
            float[] fill = GLVerts.Rect(rect, Fill, Fill, Fill);
            float[] outline = GLVerts.Outline(rect, 2f, Outline, Outline, Outline);

            return fill.Concat(outline).ToArray();
        }

        public override void PreRender(float mousex, float mousey, float frametime)
        {
            base.PreRender(mousex, mousey, frametime);

            float prevTime = hoverTime;
            hoverTime = MathHelper.Clamp((hoverTime / 0.025f) + (Hovering ? 10 : -10) * frametime, 0, 1) * 0.025f;

            if (hoverTime != prevTime)
                Update();
        }

        public override void MouseClickLeft(float x, float y)
        {
            base.MouseClickLeft(x, y);

            if (Hovering)
                SoundPlayer.Play(Settings.clickSound.Value);
        }

        public override void MouseClickRight(float x, float y)
        {
            base.MouseClickRight(x, y);

            if (Hovering && RightResponsive)
                SoundPlayer.Play(Settings.clickSound.Value);
        }
    }
}
