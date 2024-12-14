using New_SSQE.Preferences;
using OpenTK.Mathematics;

namespace New_SSQE.GUI
{
    internal class GuiSliderInfinite : GuiSlider
    {
        public float Min = -1;
        public float Max = 1;

        public GuiSliderInfinite(float x, float y, float w, float h, Setting<SliderSetting> setting, bool reverse, bool lockSize = false, bool moveWithOffset = false) : base(x, y, w, h, setting, reverse, lockSize, moveWithOffset)
        {
            Locked = true;
            Slider.Value.Max = 2 * Slider.Value.Value;
        }

        public GuiSliderInfinite(Setting<SliderSetting> setting, bool reverse, bool lockSize = false, bool moveWithOffset = false) : this(0, 0, 0, 0, setting, reverse, lockSize, moveWithOffset) { }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (Dragging)
            {
                float widthdiff = MainWindow.Instance.ClientSize.X / 1920f;
                float value = Slider.Value.Value + MainWindow.Instance.Delta.X * Slider.Value.Step / widthdiff;
                value = MathHelper.Clamp(value, Min, Max);
                
                Slider.Value.Value = value;
                Slider.Value.Max = 2 * value;
            }

            base.Render(mousex, mousey, frametime);
        }
    }
}
