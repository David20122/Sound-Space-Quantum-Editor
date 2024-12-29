using New_SSQE.Preferences;
using OpenTK.Mathematics;

namespace New_SSQE.NewGUI.Controls
{
    internal class GuiSliderInfinite : GuiSlider
    {
        private float min;
        private float max;

        public GuiSliderInfinite(float x, float y, float w, float h, float min, float max, Setting<SliderSetting> setting, bool reverse = false) : base(x, y, w, h, setting, reverse)
        {
            this.min = min;
            this.max = max;
        }

        public override void MouseMove(float x, float y)
        {
            base.MouseMove(x, y);

            if (Dragging)
            {
                bool horizontal = rect.Width > rect.Height;
                float width = horizontal ? MainWindow.Instance.ClientSize.X / 1920f : MainWindow.Instance.ClientSize.Y / 1080f;
                float value = setting.Value.Value + (horizontal ? MainWindow.Instance.Delta.X : MainWindow.Instance.Delta.Y) * setting.Value.Step / width;
                value = MathHelper.Clamp(value, min, max);

                setting.Value.Value = value;
                setting.Value.Max = 2 * value;
            }
        }
    }
}
