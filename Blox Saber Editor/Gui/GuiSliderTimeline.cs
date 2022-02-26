using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.Gui
{
	class GuiSliderTimeline : GuiSlider
	{
		public static List<Bookmark> Bookmarks = new List<Bookmark>();
		public GuiSliderTimeline(float x, float y, float sx, float sy) : base(x, y, sx, sy)
		{

		}

		Color Color1 = EditorWindow.Instance.Color1;
		Color Color3 = EditorWindow.Instance.Color3;
		public Bookmark SelectedBookmark;
		private RectangleF renderRect;
		public void OnMouseMove(float x, float y)
		{
			Bookmark selection = null;
			for (int i = 0; i < Bookmarks.Count; i++)
			{
				var bookmark = Bookmarks[i];
				var progress = bookmark.MS / EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds;

				var rx = renderRect.X + progress * renderRect.Width;
				var ry = renderRect.Y + renderRect.Height;

				var bmrk = new RectangleF((float)rx - 4, (float)ry - 40, 8, 8);

				if (bmrk.Contains(x, y))
					selection = bookmark;
			}
			SelectedBookmark = selection;
		}
		protected override void RenderTimeline(RectangleF rect)
		{
			base.RenderTimeline(rect);

			renderRect = rect;

			if (EditorWindow.Instance.MusicPlayer.TotalTime == TimeSpan.Zero)
				return;

			for (int i = 0; i < EditorWindow.Instance.Notes.Count; i++)
			{
				var note = EditorWindow.Instance.Notes[i];

				var progress = note.Ms / EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds;

				var x = rect.X + progress * rect.Width;
				var y = rect.Y + rect.Height / 2f;

				GL.Color4(Color1);
				Glu.RenderQuad((int)x, y + rect.Height * 2, 1, rect.Height);
			}

			for (int i = 0; i < Bookmarks.Count; i++)
			{
				var bookmark = Bookmarks[i];
				var progress = bookmark.MS / EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds;

				var x = rect.X + progress * rect.Width;
				var y = rect.Y + rect.Height;

				var bmrk = new RectangleF((float)x - 4, (float)y - 40, (float)8, (float)8);
				if (SelectedBookmark == bookmark)
				{
					var h = (int)EditorWindow.Instance.FontRenderer.GetHeight(16);
					EditorWindow.Instance.FontRenderer.Render(bookmark.Name, (int)x - 4, (int)y - 48 - h, 16);
					bmrk.Width += 4;
					bmrk.Height += 4;
					bmrk.X -= 2;
					bmrk.Y -= 2;
				}

				GL.Color4(Color3);
				Glu.RenderQuad(bmrk);
			}

			for (int i = 0; i < GuiTrack.BPMs.Count; i++)
            {
				var bpm = GuiTrack.BPMs[i];

				var progress = bpm.Ms / EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds;

				var x = rect.X + progress * rect.Width - 1;
				var y = rect.Y - rect.Height / 2f;

				GL.Color4(Color1);
				Glu.RenderQuad((int)x, y - rect.Height * 4, 2, rect.Height * 2);
            }
		}
	}
}