using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.GUI
{
	class GuiSlider : Gui
	{
        public bool Visible = true;
        public bool lockSize;
        public bool moveWithOffset;

        public bool hovering;
        public bool dragging;

        public RectangleF originRect;

        public string settingName;
        public bool reverse;

        public float alpha;

        public GuiSlider(float posx, float posy, float sizex, float sizey, string Setting, bool Reverse, bool LockSize = false, bool MoveWithOffset = false) : base(posx, posy, sizex, sizey)
        {
            settingName = Setting;

            reverse = Reverse;
            lockSize = LockSize;
            moveWithOffset = MoveWithOffset;

            originRect = new RectangleF(posx, posy, sizex, sizey);
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (Visible)
            {
                var editor = MainWindow.Instance.CurrentWindow;
                var colored = editor is GuiWindowEditor;

                var color1 = colored ? Settings.settings["color1"] : Color.FromArgb(255, 255, 255);
                var color2 = colored ? Settings.settings["color2"] : Color.FromArgb(50, 50, 50);

                var setting = Settings.settings[settingName];

                var horizontal = rect.Width > rect.Height;
                var width = horizontal ? rect.Width - rect.Height : rect.Height - rect.Width;

                if (dragging)
                {
                    float stepf = setting.Step / setting.Max;
                    if (settingName == "beatDivisor" && !MainWindow.Instance.shiftHeld)
                        stepf *= 2;

                    var pos = horizontal ? rect.X + rect.Height / 2f : rect.Y + rect.Width / 2f;
                    var mouse = horizontal ? mousex : mousey;

                    var prog = (float)Math.Round((horizontal ? mouse - pos : reverse ? (width - (mouse - pos)) : (mouse - pos)) / width / stepf) * stepf;

                    setting.Value = MathHelper.Clamp(setting.Max * prog, 0, setting.Max);

                    switch (settingName)
                    {
                        case "trackHeight":
                            editor.yoffset = 64 + setting.Value;
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

                var pos1 = new Vector2(horizontal ? rect.X + rect.Height / 2f + width * progress : rect.X + rect.Width / 2f, horizontal ? rect.Y + rect.Height / 2f : rect.Y + rect.Width / 2f + width * (reverse ? (1f - progress) : progress));
                var pos2 = new Vector2(mousex, mousey);

                var hover = dragging || (pos1 - pos2).Length <= 12f;

                alpha = MathHelper.Clamp(alpha + (hover ? 10 : -10) * frametime, 0, 1);

                GL.LineWidth(3f);
                GL.Color3(color2);

                if (horizontal)
                    GLSpecial.Line(rect.X + rect.Height / 2f, rect.Y + rect.Height / 2f, rect.Right - rect.Height / 2f, rect.Y + rect.Height / 2f);
                else
                    GLSpecial.Line(rect.X + rect.Width / 2f, rect.Y + rect.Width / 2f, rect.X + rect.Width / 2f, rect.Bottom - rect.Width / 2f);

                //visible portion on timeline
                if (settingName == "currentTime")
                {
                    var Track = editor.track;
                    var start = Track.startPos;
                    var end = Track.endPos;

                    var lineRect = new RectangleF(rect.X + rect.Height / 2f, rect.Y + rect.Height / 2f - 1.5f, rect.Width - rect.Height, 3f);
                    var spRect = new RectangleF(lineRect.X + lineRect.Width * start, lineRect.Y, lineRect.Width * (end - start), lineRect.Height);

                    GL.Color3(Settings.settings["color3"]);
                    GLSpecial.Rect(spRect);
                }

                GL.LineWidth(2f);
                GL.Color3(color1);
                GLSpecial.Circle(pos1.X, pos1.Y, 4f, 16);

                if (hover)
                {
                    var rotate = alpha * 90f;

                    GL.Translate(pos1.X, pos1.Y, 0);
                    GL.Rotate(rotate, 0, 0, 1);
                    GLSpecial.Circle(0, 0, 12f * alpha, 6, true);
                    GL.Rotate(-rotate, 0, 0, 1);
                    GL.Translate(-pos1.X, -pos1.Y, 0);
                }
            }
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            MainWindow.Instance.SoundPlayer.Play(Settings.settings["clickSound"]);
        }
    }
}