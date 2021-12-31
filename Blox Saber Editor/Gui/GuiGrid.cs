using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Sound_Space_Editor.Gui
{
	class GuiGrid : Gui
	{
		public Note MouseOverNote;

		private readonly Note _startNote = new Note(1, 1, 0);

		public GuiGrid(float sx, float sy) : base(EditorWindow.Instance.ClientSize.Width / 2f - sx / 2, EditorWindow.Instance.ClientSize.Height / 2f - sy / 2, sx, sy)
		{

		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			var editor = (GuiScreenEditor)EditorWindow.Instance.GuiScreen;

			var rect = ClientRectangle;
			var mouseOver = false;
			// grid transparency
			int griddim = EditorSettings.GridOpacity;

			GL.Color4(Color.FromArgb(griddim, 36, 35, 33));
			Glu.RenderQuad(rect.X, rect.Y, rect.Width, rect.Height);

			var cellSize = rect.Width / 3f;
			var noteSize = cellSize * 0.75f;

			var gap = cellSize - noteSize;

			var audioTime = EditorWindow.Instance.MusicPlayer.CurrentTime.TotalMilliseconds;

			if (editor.Numpad.Toggle)
            {
				if (EditorWindow.Instance.inputState == "keyboard")
                {
					EditorWindow.Instance.ChangeKeyMapping("numpad");
                }
            } else
            {
				if (EditorWindow.Instance.inputState == "numpad")
				{
					EditorWindow.Instance.ChangeKeyMapping("keyboard");
				}
			}

			GL.Color3(0.2, 0.2, 0.2f);

			for (float y = 0; y <= 3; y++)
			{
				var ly = y * cellSize;

				Glu.RenderQuad((int)(rect.X), (int)(rect.Y + ly), rect.Width + 1, 1);
			}

			for (float x = 0; x <= 3; x++)
			{
				var lx = x * cellSize;

				Glu.RenderQuad((int)(rect.X + lx), (int)(rect.Y), 1, rect.Height + 1);
			}

			if (editor.NoteAlign.Value != 1 && editor.QuantumGridLines.Toggle)
			{
				GL.Begin(PrimitiveType.Lines);

				var div = editor.NoteAlign.Value + 1;

				for (int i = 1; i < div; i++)
				{
					//GL.Vertex2(rect.X + rect.Width / div * i, rect.Y);
					//GL.Vertex2(rect.X + rect.Width / div * i, rect.Y + rect.Height / div * i);

					GL.Vertex2(rect.X + rect.Width / div * i, rect.Y);
					GL.Vertex2(rect.X + rect.Width / div * i, rect.Y + rect.Height);

					GL.Vertex2(rect.X, rect.Y + rect.Height / div * i);
					GL.Vertex2(rect.X + rect.Width, rect.Y + rect.Height / div * i);
				}
				GL.End();
			}

			var fr = EditorWindow.Instance.FontRenderer;

			GL.Color3(0.2f, 0.2f, 0.2f);
			foreach (var pair in EditorWindow.Instance.KeyMapping)
			{
				if (pair.Key == Key.Y)
					continue;

				var letter = pair.Key == Key.Z ? "Y/Z" : pair.Key.ToString();

				if (letter.Length > 1) letter = letter.Replace("Keypad", "");

				var tuple = pair.Value;

				var x = rect.X + tuple.Item1 * cellSize + cellSize / 2;
				var y = rect.Y + tuple.Item2 * cellSize + cellSize / 2;

				var width = fr.GetWidth(letter, 38);
				var height = fr.GetHeight(38);

				fr.Render(letter, (int)(x - width / 2f), (int)(y - height / 2), 38);
			}

			Note last = null;
			Note next = null;

			for (var index = 0; index < EditorWindow.Instance.Notes.Count; index++)
			{
				var note = EditorWindow.Instance.Notes[index];
				var passed = audioTime > note.Ms + 1;
				var visible = !passed && note.Ms - audioTime <= 750;

				if (passed)
				{
					last = note;
				}
				else if (next == null)
				{
					next = note;
				}

				if (!visible)
				{
					if (passed && next != null)
					{
						break;
					}

					continue;
				}

				var x = rect.X + note.X * cellSize + gap / 2;
				var y = rect.Y + note.Y * cellSize + gap / 2;

				var progress = (float)Math.Pow(1 - Math.Min(1, (note.Ms - audioTime) / 750.0), 2);
				var noteRect = new RectangleF(x, y, noteSize, noteSize);
				OpenTK.Graphics.Color4 colora = new OpenTK.Graphics.Color4(note.Color.R, note.Color.G, note.Color.B, progress * 0.15f);
				GL.Color4(colora);
				Glu.RenderQuad(noteRect);
				OpenTK.Graphics.Color4 color = new OpenTK.Graphics.Color4(note.Color.R, note.Color.G, note.Color.B, progress);
				GL.Color4(color);
				Glu.RenderOutline(noteRect);
				if (editor.ApproachSquares.Toggle)
				{
					var outlineSize = 4 + noteSize + noteSize * (1 - progress) * 2;
					Glu.RenderOutline(x - outlineSize / 2 + noteSize / 2, y - outlineSize / 2 + noteSize / 2,
						outlineSize,
						outlineSize);
				}

				if (editor.GridNumbers.Toggle)
				{
					GL.Color4(1, 1, 1, progress);
					var s = $"{(index + 1):#,##}";
					var w = fr.GetWidth(s, 24);
					var h = fr.GetHeight(24);

					fr.Render(s, (int)(noteRect.X + noteRect.Width / 2 - w / 2f),
						(int)(noteRect.Y + noteRect.Height / 2 - h / 2f), 24);
				}

				if (!mouseOver)
				{
					MouseOverNote = null;
				}

				if (EditorWindow.Instance.SelectedNotes.Contains(note))
				{
					var outlineSize = noteSize + 8;

					GL.Color4(0, 0.5f, 1f, progress);
					Glu.RenderOutline(x - outlineSize / 2 + noteSize / 2, y - outlineSize / 2 + noteSize / 2,
						outlineSize, outlineSize);
				}

				if (!mouseOver && noteRect.Contains(mouseX, mouseY))
				{
					MouseOverNote = note;
					mouseOver = true;

					GL.Color3(0, 1, 0.25f);
					Glu.RenderOutline(x - 4, y - 4, noteSize + 8, noteSize + 8);
				}
			}

			//RENDER AUTOPLAY
			if (editor.Autoplay.Toggle)
			{
				RenderAutoPlay(last, next, cellSize, rect, audioTime);
			}
		}

		private void RenderAutoPlay(Note last, Note next, float cellSize, RectangleF rect, double audioTime)
		{
			if (last == null)
				last = _startNote;

			if (next == null)
				next = last;

			var timeDiff = next.Ms - last.Ms;
			var timePos = audioTime - last.Ms;

			var progress = timeDiff == 0 ? 1 : (float)timePos / timeDiff;

			progress = (float)Math.Sin(progress * MathHelper.PiOver2);

			var s = (float)Math.Sin(progress * MathHelper.Pi) * 8 + 16;

			var lx = rect.X + last.X * cellSize;
			var ly = rect.Y + last.Y * cellSize;

			var nx = rect.X + next.X * cellSize;
			var ny = rect.Y + next.Y * cellSize;

			var x = cellSize / 2 + lx + (nx - lx) * progress;
			var y = cellSize / 2 + ly + (ny - ly) * progress;

			var cx = x - s / 2;
			var cy = y - s / 2;

			//something here is the autoplay cursor
			GL.Color4(1, 1, 1, 0.25f);
			Glu.RenderQuad(cx, cy, s, s);
			GL.Color4(1, 1, 1, 1f);
			GL.LineWidth(2);
			Glu.RenderOutline(cx, cy, s, s);
			GL.LineWidth(1);
		}
	}
}