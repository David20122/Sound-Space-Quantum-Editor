using New_SSQE.Audio;
using New_SSQE.GUI;
using New_SSQE.Maps;
using New_SSQE.Preferences;
using OpenTK.Mathematics;
using System.Drawing;

namespace New_SSQE.NewGUI.Controls
{
    internal class GuiSlider : InteractiveControl
    {
        public float ShiftIncrement = 1;

        protected Setting<SliderSetting> setting;
        
        private bool reverse;
        private float hoverTime = 0f;

        public GuiSlider(float x, float y, float w, float h, Setting<SliderSetting> setting, bool reverse = false) : base(x, y, w, h, "", 0, "main", true)
        {
            this.setting = setting;
            this.reverse = reverse;
        }

        public override float[] Draw()
        {
            bool horizontal = rect.Width > rect.Height;

            float progress = setting.Value.Max == 0 ? 0.5f : setting.Value.Value / setting.Value.Max;
            if (reverse)
                progress = 1 - progress;

            RectangleF lineRect = horizontal ? new(rect.X + rect.Height / 2, rect.Y + rect.Height / 2 - 1.5f, rect.Width - rect.Height, 3)
                : new(rect.X + rect.Width / 2 - 1.5f, rect.Y + rect.Width / 2, 3, rect.Height - rect.Width);
            Vector2 circlePos = (lineRect.X + lineRect.Width * (horizontal ? progress : 0.5f), lineRect.Y + lineRect.Height * (horizontal ? 0.5f : progress));

            float[] line = GLVerts.Rect(lineRect, Settings.color2.Value);
            float[] circle = GLVerts.Circle(circlePos.X, circlePos.Y, 4, 16, 0, Settings.color1.Value);

            if (hoverTime > 0)
            {
                float[] hoverCircle = GLVerts.CircleOutline(circlePos.X, circlePos.Y, 12 * hoverTime, 6, 2, 90 * hoverTime, Settings.color1.Value);
                circle = circle.Concat(hoverCircle).ToArray();
            }

            return line.Concat(circle).ToArray();
        }

        public override void PreRender(float mousex, float mousey, float frametime)
        {
            base.PreRender(mousex, mousey, frametime);

            float prevTime = hoverTime;
            hoverTime = MathHelper.Clamp(hoverTime + (Hovering || Dragging ? 10 : -10) * frametime, 0, 1);

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

            if (Hovering)
            {
                SoundPlayer.Play(Settings.clickSound.Value);
                setting.Value.Value = setting.Value.Default;
            }
        }

        public override void MouseMove(float x, float y)
        {
            base.MouseMove(x, y);

            if (Dragging)
            {
                bool horizontal = rect.Width > rect.Height;
                float width = rect.Height - rect.Width * (horizontal ? -1 : 1);

                float step = setting.Value.Step / setting.Value.Max;
                if (!MainWindow.Instance.ShiftHeld)
                    step /= ShiftIncrement;

                float pos = horizontal ? rect.X + rect.Height / 2 : rect.Y + rect.Width / 2;
                float mouse = horizontal ? x : y;

                float progress = (float)Math.Round((horizontal ? mouse - pos : reverse ? (width - mouse + pos) : mouse - pos) / width / step) * step;
                setting.Value.Value = MathHelper.Clamp(setting.Value.Max * progress, 0, setting.Value.Max);

                UpdateSlider();
            }
        }

        private void UpdateSlider()
        {
            if (MainWindow.Instance.CurrentWindow is not GuiWindowEditor editor)
                return;

            switch (setting.Name)
            {
                case "trackHeight":
                    editor.YOffset = 64 + setting.Value.Value;
                    editor.OnResize(MainWindow.Instance.ClientSize);
                    break;

                case "sfxVolume":
                    SoundPlayer.Volume = setting.Value.Value;
                    break;

                case "masterVolume":
                    MusicPlayer.Volume = setting.Value.Value;
                    break;

                case "tempo":
                    CurrentMap.SetTempo(setting.Value.Value);
                    break;
            }
        }
    }
}
