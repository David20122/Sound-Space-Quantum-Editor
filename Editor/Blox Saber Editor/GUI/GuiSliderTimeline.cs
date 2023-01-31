using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Sound_Space_Editor.Misc;

namespace Sound_Space_Editor.GUI
{
	class GuiSliderTimeline : GuiSlider
	{
        public Bookmark hoveringBookmark;

        public GuiSliderTimeline(float posx, float posy, float sizex, float sizey, bool Reverse, bool LockSize = false) : base(posx, posy, sizex, sizey, "currentTime", Reverse, LockSize)
        {

        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            base.Render(mousex, mousey, frametime);

            var setting = Settings.settings[settingName];

            var lineRect = new RectangleF(rect.X + rect.Height / 2f, rect.Y + rect.Height / 2f - 1.5f, rect.Width - rect.Height, 3f);

            GL.Color3(Settings.settings["color1"]);
            GL.LineWidth(1f);

            //notes
            for (int i = 0; i < MainWindow.Instance.Notes.Count; i++)
            {
                var note = MainWindow.Instance.Notes[i];

                var progress = note.Ms / setting.Max;
                var x = lineRect.X + progress * lineRect.Width;
                var y = lineRect.Y + lineRect.Height / 2f;

                GLSpecial.Line(x, y + 5f, x, y + lineRect.Height);
            }

            GL.LineWidth(2f);

            //timing points
            for (int i = 0; i < MainWindow.Instance.TimingPoints.Count; i++)
            {
                var point = MainWindow.Instance.TimingPoints[i];

                var progress = point.Ms / setting.Max;
                var x = lineRect.X + progress * lineRect.Width - 1f;
                var y = lineRect.Y + lineRect.Height / 2f;

                GLSpecial.Line(x, y - 10f, x, y - lineRect.Height * 2f);
            }

            //bookmarks
            for (int i = 0; i < MainWindow.Instance.Bookmarks.Count; i++)
            {
                var bookmark = MainWindow.Instance.Bookmarks[i];

                var progress = bookmark.Ms / setting.Max;
                var x = lineRect.X + progress * lineRect.Width;
                var y = lineRect.Y + lineRect.Height;

                var brect = new RectangleF(x - 4f, y - 40f, 8f, 8f);
                var hovering = brect.Contains(mousex, mousey);

                hoveringBookmark = hovering ? bookmark : null;

                GL.Color3(hovering ? Settings.settings["color2"] : Settings.settings["color3"]);
                GLSpecial.Rect(brect);

                if (hovering)
                {
                    var height = TextHeight(16);

                    RenderText(bookmark.Text, x - 4f, y - 48f - height, 16);
                }
            }
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            if (MainWindow.Instance.MusicPlayer.IsPlaying)
                MainWindow.Instance.MusicPlayer.Pause();
        }
    }
}