using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace New_SSQE.GUI
{
    internal class GuiSlider : WindowControl
    {
        public bool Hovering;
        public bool Dragging;

        public string Setting;
        public bool Reverse;

        private float alpha;
        private float prevAlpha;
        private float prevValue;

        private readonly float defaultValue;

        public GuiSlider(float posx, float posy, float sizex, float sizey, string setting, bool reverse, bool lockSize = false, bool moveWithOffset = false) : base(posx, posy, sizex, sizey)
        {
            Setting = setting;

            Reverse = reverse;
            LockSize = lockSize;
            MoveWithOffset = moveWithOffset;

            defaultValue = Settings.settings[setting].Default;

            Init();
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            var editor = MainWindow.Instance.CurrentWindow;

            var setting = Settings.settings[Setting];

            var horizontal = Rect.Width > Rect.Height;
            var width = horizontal ? Rect.Width - Rect.Height : Rect.Height - Rect.Width;

            if (Dragging)
            {
                float stepf = setting.Step / setting.Max;
                if (Setting == "beatDivisor" && !MainWindow.Instance.ShiftHeld)
                    stepf *= 2;

                var pos = horizontal ? Rect.X + Rect.Height / 2f : Rect.Y + Rect.Width / 2f;
                var mouse = horizontal ? mousex : mousey;

                var prog = (float)Math.Round((horizontal ? mouse - pos : Reverse ? (width - mouse + pos) : mouse - pos) / width / stepf) * stepf;

                setting.Value = MathHelper.Clamp(setting.Max * prog, 0, setting.Max);

                switch (Setting)
                {
                    case "trackHeight":
                        editor.YOffset = 64 + setting.Value;
                        editor.OnResize(MainWindow.Instance.ClientSize);

                        break;

                    case "sfxVolume":
                        MainWindow.Instance.SoundPlayer.Volume = setting.Value;

                        break;

                    case "masterVolume":
                        MainWindow.Instance.MusicPlayer.Volume = setting.Value;

                        break;

                    case "tempo":
                        MainWindow.Instance.SetTempo(setting.Value);

                        break;
                }
            }

            var progress = setting.Value / setting.Max;

            var pos1 = new Vector2(horizontal ? Rect.X + Rect.Height / 2f + width * progress : Rect.X + Rect.Width / 2f, horizontal ? Rect.Y + Rect.Height / 2f : Rect.Y + Rect.Width / 2f + width * (Reverse ? (1f - progress) : progress));
            var pos2 = new Vector2(mousex, mousey);

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
                GL.DrawArrays(PrimitiveType.Triangles, 6, 6);
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
            var editor = MainWindow.Instance.CurrentWindow;
            var colored = editor is GuiWindowEditor;
            var setting = Settings.settings[Setting];

            var sc1 = colored ? Settings.settings["color1"] : Color.FromArgb(255, 255, 255);
            var color1 = new float[] { sc1.R / 255f, sc1.G / 255f, sc1.B / 255f };

            var sc2 = colored ? Settings.settings["color2"] : Color.FromArgb(50, 50, 50);
            var color2 = new float[] { sc2.R / 255f, sc2.G / 255f, sc2.B / 255f };

            var sc3 = Settings.settings["color3"];
            var color3 = new float[] { sc3.R / 255f, sc3.G / 255f, sc3.B / 255f };

            bool horizontal = Rect.Width > Rect.Height;
            float progress = setting.Value / setting.Max;
            if (Reverse)
                progress = 1f - progress;

            var lineRect = horizontal ? new RectangleF(Rect.X + Rect.Height / 2f, Rect.Y + Rect.Height / 2f - 1.5f, Rect.Width - Rect.Height, 3f)
                : new RectangleF(Rect.X + Rect.Width / 2f - 1.5f, Rect.Y + Rect.Width / 2f, 3f, Rect.Height - Rect.Width);
            var circlePos = new PointF(lineRect.X + lineRect.Width * (horizontal ? progress : 0.5f), lineRect.Y + lineRect.Height * (horizontal ? 0.5f : progress));

            List<float> line = new(GLU.Rect(lineRect, color2));

            if (Setting == "currentTime" && editor.Track != null)
            {
                var track = editor.Track;
                var start = track.StartPos;
                var end = track.EndPos;

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

            return new Tuple<float[], float[]>(final.ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right)
        {
            MainWindow.Instance.SoundPlayer.Play(Settings.settings["clickSound"]);

            if (right)
                Settings.settings[Setting].Value = defaultValue;

                switch (Setting)
                {
                    case "trackHeight":
                        editor.YOffset = 64 + defaultValue;
                        editor.OnResize(MainWindow.Instance.ClientSize);

                        break;

                    case "sfxVolume":
                        MainWindow.Instance.SoundPlayer.Volume = defaultValue;

                        break;

                    case "masterVolume":
                        MainWindow.Instance.MusicPlayer.Volume = defaultValue;

                        break;

                    case "tempo":
                        MainWindow.Instance.SetTempo(defaultValue);

                        break;
                }
            }
            else
                Dragging = true;

            MainWindow.Instance.CurrentWindow?.OnButtonClicked(-1);
        }

        public override void OnMouseUp(Point pos)
        {
            Dragging = false;
        }

        public override void OnMouseLeave(Point pos)
        {
            Dragging = false;
        }
    }
}
