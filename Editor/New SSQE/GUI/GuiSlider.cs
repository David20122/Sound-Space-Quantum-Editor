using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Mathematics;
using New_SSQE.Audio;
using New_SSQE.Preferences;
using New_SSQE.Maps;

namespace New_SSQE.GUI
{
    internal class GuiSlider : WindowControl
    {
        public bool Hovering;
        public bool Dragging;
        public bool Locked;

        public string? Setting;
        public Setting<SliderSetting> Slider;
        public bool Reverse;

        private float alpha;
        private float prevAlpha;
        private float prevValue;

        private readonly float defaultValue;

        public GuiSlider(float x, float y, float w, float h, Setting<SliderSetting> setting, bool reverse, bool lockSize = false, bool moveWithOffset = false) : base(x, y, w, h)
        {
            Setting = setting.Name ?? "";
            Slider = setting;

            Reverse = reverse;
            LockSize = lockSize;
            MoveWithOffset = moveWithOffset;

            defaultValue = Slider.Value.Default;

            Init();
        }

        public GuiSlider(Setting<SliderSetting> setting, bool reverse, bool lockSize = false, bool moveWithOffset = false) : this(0, 0, 0, 0, setting, reverse, lockSize, moveWithOffset) { }

        public override void Render(float mousex, float mousey, float frametime)
        {
            GuiWindow editor = MainWindow.Instance.CurrentWindow;

            SliderSetting setting = Slider.Value;

            bool horizontal = Rect.Width > Rect.Height;
            float width = horizontal ? Rect.Width - Rect.Height : Rect.Height - Rect.Width;

            if (Dragging && !Locked)
            {
                float stepf = setting.Step / setting.Max;
                if (Setting == "beatDivisor" && !MainWindow.Instance.ShiftHeld)
                    stepf *= 2;

                float pos = horizontal ? Rect.X + Rect.Height / 2f : Rect.Y + Rect.Width / 2f;
                float mouse = horizontal ? mousex : mousey;

                float prog = (float)Math.Round((horizontal ? mouse - pos : Reverse ? (width - mouse + pos) : mouse - pos) / width / stepf) * stepf;

                setting.Value = MathHelper.Clamp(setting.Max * prog, 0, setting.Max);

                switch (Setting)
                {
                    case "trackHeight":
                        editor.YOffset = 64 + setting.Value;
                        editor.OnResize(MainWindow.Instance.ClientSize);

                        break;

                    case "sfxVolume":
                        SoundPlayer.Volume = setting.Value;

                        break;

                    case "masterVolume":
                        MusicPlayer.Volume = setting.Value;

                        break;

                    case "tempo":
                        CurrentMap.SetTempo(setting.Value);

                        break;
                }
            }

            float progress = setting.Value / setting.Max;
            if (setting.Max == 0)
                progress = 0.5f;

            Vector2 pos1 = (horizontal ? Rect.X + Rect.Height / 2f + width * progress : Rect.X + Rect.Width / 2f, horizontal ? Rect.Y + Rect.Height / 2f : Rect.Y + Rect.Width / 2f + width * (Reverse ? (1f - progress) : progress));
            Vector2 pos2 = (mousex, mousey);

            Hovering = (pos1 - pos2).Length <= 12f;

            alpha = MathHelper.Clamp(alpha + ((Dragging || Hovering) ? 10 : -10) * frametime, 0, 1);

            if (prevAlpha != alpha || prevValue != setting.Value)
            {
                Update();

                prevAlpha = alpha;
                prevValue = setting.Value;
            }

            GL.BindVertexArray(VaO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            int current = 6;

            if (Setting == "currentTime")
            {
                GL.DrawArrays(PrimitiveType.Triangles, current, 6);
                current += 6;
            }

            GL.DrawArrays(PrimitiveType.TriangleFan, current, 16);
            current += 16;

            if (Dragging || Hovering)
                GL.DrawArrays(PrimitiveType.LineLoop, current, 6);
        }

        public override void RenderTexture() { }

        public override Tuple<float[], float[]> GetVertices()
        {
            GuiWindow editor = MainWindow.Instance.CurrentWindow;
            bool colored = editor is GuiWindowEditor;
            SliderSetting setting = Slider.Value;

            Color sc1 = colored ? Settings.color1.Value : Color.FromArgb(255, 255, 255);
            float[] color1 = new float[] { sc1.R / 255f, sc1.G / 255f, sc1.B / 255f };

            Color sc2 = colored ? Settings.color2.Value : Color.FromArgb(75, 75, 75);
            float[] color2 = new float[] { sc2.R / 255f, sc2.G / 255f, sc2.B / 255f };

            Color sc3 = Settings.color3.Value;
            float[] color3 = new float[] { sc3.R / 255f, sc3.G / 255f, sc3.B / 255f };

            bool horizontal = Rect.Width > Rect.Height;
            float progress = setting.Value / setting.Max;
            if (setting.Max == 0)
                progress = 0.5f;
            if (Reverse)
                progress = 1f - progress;

            RectangleF lineRect = horizontal ? new(Rect.X + Rect.Height / 2f, Rect.Y + Rect.Height / 2f - 1.5f, Rect.Width - Rect.Height, 3f)
                : new(Rect.X + Rect.Width / 2f - 1.5f, Rect.Y + Rect.Width / 2f, 3f, Rect.Height - Rect.Width);
            PointF circlePos = new(lineRect.X + lineRect.Width * (horizontal ? progress : 0.5f), lineRect.Y + lineRect.Height * (horizontal ? 0.5f : progress));

            List<float> line = new(GLU.Rect(lineRect, color2));

            if (Setting == "currentTime" && editor.Track != null)
            {
                GuiTrack track = editor.Track;
                float start = track.StartPos;
                float end = track.EndPos;

                float[] spLine = GLU.Rect(lineRect.X + lineRect.Width * start, lineRect.Y, lineRect.Width * (end - start), lineRect.Height, color3);

                line.AddRange(spLine);
            }

            float[] circle = GLU.Circle(circlePos.X, circlePos.Y, 4f, 16, 0, color1);

            List<float> final = new(line);
            final.AddRange(circle);

            if (Dragging || alpha > 0)
            {
                float[] hoverCircle = GLU.Circle(circlePos.X, circlePos.Y, 12f * alpha, 6, 90f * alpha, color1);

                final.AddRange(hoverCircle);
            }

            return new(final.ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right)
        {
            SoundPlayer.Play(Settings.clickSound.Value);
            GuiWindow window = MainWindow.Instance.CurrentWindow;

            if (right && window is GuiWindowEditor editor)
            {
                Slider.Value.Value = defaultValue;

                switch (Setting)
                {
                    case "trackHeight":
                        editor.YOffset = 64 + defaultValue;
                        editor.OnResize(MainWindow.Instance.ClientSize);

                        break;

                    case "sfxVolume":
                        SoundPlayer.Volume = defaultValue;

                        break;

                    case "masterVolume":
                        MusicPlayer.Volume = defaultValue;

                        break;

                    case "tempo":
                        CurrentMap.SetTempo(defaultValue);

                        break;
                }
            }
            else if (!right)
                Dragging = true;

            window?.OnButtonClicked(-1);
        }

        public override void OnMouseUp(Point pos)
        {
            if (Dragging && this is GuiSliderTimeline timeline && timeline.WasPlaying)
            {
                MusicPlayer.CurrentTime = TimeSpan.FromMilliseconds(Slider.Value.Value);
                MusicPlayer.Play();
            }

            Dragging = false;
        }

        public override void OnMouseLeave(Point pos)
        {
            Dragging = false;
        }
    }
}
