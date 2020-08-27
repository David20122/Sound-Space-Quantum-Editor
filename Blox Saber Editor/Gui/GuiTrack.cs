using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.Gui
{
	class GuiTrack : Gui
	{
		private readonly ColorSequence _cs = new ColorSequence();

		public Note MouseOverNote;

		public float ScreenX = 300;

		public static float Bpm = 0;//150;
		public static long BpmOffset = 0; //in ms
		public int BeatDivisor = 8;

		public GuiTrack(float y, float sy) : base(0, y, EditorWindow.Instance.ClientSize.Width, sy)
		{

		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			// color 1

			string rc1 = EditorWindow.Instance.ReadLine("settings.ini", 12);
			string[] c1values = rc1.Split(',');
			int[] Color1 = Array.ConvertAll<string, int>(c1values, int.Parse);

			//color 2

			string rc2 = EditorWindow.Instance.ReadLine("settings.ini", 17);
			string[] c2values = rc2.Split(',');
			int[] Color2 = Array.ConvertAll<string, int>(c2values, int.Parse);

			GL.Color3(0.1f, 0.1f, 0.1f);

			var rect = ClientRectangle;

			Glu.RenderQuad(rect);
			GL.Color3(0.2f, 0.2f, 0.2f);
			Glu.RenderQuad((int)rect.X, (int)rect.Y + rect.Height, (int)rect.Width, 1);

			var fr = EditorWindow.Instance.FontRenderer;

			float cellSize = rect.Height;
			float noteSize = cellSize * 0.65f;

			var gap = cellSize - noteSize;

			float audioTime = (float)EditorWindow.Instance.MusicPlayer.CurrentTime.TotalMilliseconds;

			float cubeStep = EditorWindow.Instance.CubeStep;
			float posX = audioTime / 1000 * cubeStep;
			float maxX = (float)EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds / 1000f * cubeStep;

			var zoomLvl = EditorWindow.Instance.Zoom;
			float lineSpace = cubeStep / zoomLvl;

			float lineX = ScreenX - posX;
			if (lineX < 0)
				lineX %= lineSpace;

			//render quarters of a second depending on zoom level
			while (lineSpace > 0 && lineX < rect.Width)
			{
				GL.Color3(0.85f, 0.85f, 0.85f);
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex2((int)lineX + 0.5f, rect.Y);
				GL.Vertex2((int)lineX + 0.5f, rect.Y + 5);
				GL.End();

				lineX += lineSpace;
			}

			var mouseOver = false;

			//draw start line
			GL.LineWidth(2);
			GL.Color4(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2((int)(ScreenX - posX), rect.Y);
			GL.Vertex2((int)(ScreenX - posX), rect.Y + rect.Height);
			GL.End();

			var endLineX = ScreenX - posX + maxX + 1;

			//draw end line
			GL.Color4(1f, 0f, 0f, 1);
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2((int)endLineX, rect.Y);
			GL.Vertex2((int)endLineX, rect.Y + rect.Height);
			GL.End();
			GL.LineWidth(1);

			MouseOverNote = null;

			_cs.Reset();
			for (int i = 0; i < EditorWindow.Instance.Notes.Count; i++)
			{
				Note note = EditorWindow.Instance.Notes[i];

				note.Color = _cs.Next();

				var x = Math.Round(ScreenX - posX + note.Ms / 1000f * cubeStep);

				if (x > rect.Width)
					break;

				if (x < rect.X - noteSize)
					continue;

				var alphaMult = 1f;

				if (audioTime - 1 > note.Ms)//(x <= ScreenX)
				{
					alphaMult = 0.35f;
				}

				var y = rect.Y + gap / 2;

				var noteRect = new RectangleF((int)x, (int)y, noteSize, noteSize);

				var b = MouseOverNote == null && !mouseOver && noteRect.Contains(mouseX, mouseY);

				if ((b || EditorWindow.Instance.SelectedNotes.Contains(note)) &&
					!EditorWindow.Instance.IsDraggingNoteOnTimeLine)
				{
					if (b)
					{
						MouseOverNote = note;
						mouseOver = true;
						GL.Color3(0, 1, 0.25f);
					}
					else
					{
						GL.Color3(0, 0.5f, 1);
					}

					Glu.RenderOutline((int)(x - 4), (int)(y - 4), (int)(noteSize + 8), (int)(noteSize + 8));
				}

				var c = Color.FromArgb((int)(15 * alphaMult), note.Color);

				GL.Color4(c);
				Glu.RenderQuad((int)x, (int)y, (int)noteSize, (int)noteSize);
				GL.Color4(note.Color.R, note.Color.G, note.Color.B, alphaMult);
				Glu.RenderOutline((int)x, (int)y, (int)noteSize, (int)noteSize);

				var gridGap = 2;
				for (int j = 0; j < 9; j++)
				{
					var indexX = 2 - j % 3;
					var indexY = 2 - j / 3;

					var gridX = (int)x + indexX * (9 + gridGap) + 5;
					var gridY = (int)y + indexY * (9 + gridGap) + 5;

					if (note.X == indexX && note.Y == indexY)
					{
						GL.Color4(note.Color.R, note.Color.G, note.Color.B, alphaMult);
						Glu.RenderQuad(gridX, gridY, 9, 9);
					}
					else
					{
						GL.Color4(note.Color.R, note.Color.G, note.Color.B, alphaMult * 0.45);
						Glu.RenderOutline(gridX, gridY, 9, 9);
					}
				}

				var numText = $"{(i + 1):##,###}";

				GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
				fr.Render(numText, (int)x + 3, (int)(rect.Y + rect.Height) + 3, 16);

				GL.Color3(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
				fr.Render($"{note.Ms:##,###}ms", (int)x + 3,
					(int)(rect.Y + rect.Height + fr.GetHeight(16)) + 3 + 2, 16);

				//draw line
				GL.Color4(1f, 1f, 1f, alphaMult);
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex2((int)x + 0.5f, rect.Y + rect.Height + 3);
				GL.Vertex2((int)x + 0.5f, rect.Y + rect.Height + 28);
				GL.End();
			}

			if (Bpm > 33)
			{
				lineSpace = 60 / Bpm * cubeStep;
				var stepSmall = lineSpace / BeatDivisor;

				lineX = ScreenX - posX + BpmOffset / 1000f * cubeStep;
				if (lineX < 0)
					lineX %= lineSpace;

				if (lineSpace > 0 && lineX < rect.Width)
				{
					//draw offset start line
					GL.Color4(Color.FromArgb(255, 0, 0));
					GL.Begin(PrimitiveType.Lines);
					GL.Vertex2((int)lineX + 0.5f, 0);
					GL.Vertex2((int)lineX + 0.5f, rect.Bottom);
					GL.End();
				}

				//render BPM lines
				while (lineSpace > 0 && lineX < rect.Width && lineX < endLineX)
				{
					GL.Color3(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
					GL.Begin(PrimitiveType.Lines);
					GL.Vertex2((int)lineX + 0.5f, rect.Bottom);
					GL.Vertex2((int)lineX + 0.5f, rect.Bottom - 11);
					GL.End();

					for (int j = 1; j <= BeatDivisor; j++)
					{
						var xo = lineX + j * stepSmall;

						if (j < BeatDivisor && xo < endLineX)
						{
							var half = j == BeatDivisor / 2 && BeatDivisor % 2 == 0;

							if (half)
								GL.Color3(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
							else
								GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));

							GL.Begin(PrimitiveType.Lines);
							GL.Vertex2((int)xo + 0.5f, rect.Bottom - (half ? 7 : 4));
							GL.Vertex2((int)xo + 0.5f, rect.Bottom);
							GL.End();
						}
					}

					lineX += lineSpace;
				}
			}

			//draw screen line
			GL.Color4(1f, 1, 1, 0.75);
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2((int)(rect.X + ScreenX) + 0.5, rect.Y + 4);
			GL.Vertex2((int)(rect.X + ScreenX) + 0.5, rect.Y + rect.Height - 4);
			GL.End();

			//GL.Color3(1, 1, 1f);
			//FontRenderer.Print("HELLO", 0, rect.Y + rect.Height + 8);
		}

		public override void OnResize(Size size)
		{
			ClientRectangle = new RectangleF(0, ClientRectangle.Y, size.Width, ClientRectangle.Height);

			ScreenX = ClientRectangle.Width / 2.5f;
		}

		public List<Note> GetNotesInRect(RectangleF selectionRect)
		{
			var notes = new List<Note>();

			var rect = ClientRectangle;

			float cellSize = rect.Height;
			float noteSize = cellSize * 0.65f;

			float gap = cellSize - noteSize;

			float audioTime = (float)EditorWindow.Instance.MusicPlayer.CurrentTime.TotalMilliseconds;

			float cubeStep = EditorWindow.Instance.CubeStep;
			float posX = audioTime / 1000 * cubeStep;

			for (int i = 0; i < EditorWindow.Instance.Notes.Count; i++)
			{
				Note note = EditorWindow.Instance.Notes[i];
				note.Color = _cs.Next();

				float x = ScreenX - posX + note.Ms / 1000f * cubeStep;

				if (x < rect.X - noteSize || x > rect.Width)
					continue;

				float y = rect.Y + gap / 2;

				var noteRect = new RectangleF(x, y, noteSize, noteSize);

				if (selectionRect.IntersectsWith(noteRect))
					notes.Add(note);
			}

			return notes;
		}
	}
}