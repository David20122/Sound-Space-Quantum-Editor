using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.Gui
{
	class GuiTrack : Gui
	{
		private readonly ColorSequence _cs = new ColorSequence();

		private Note _lastPlayedNote;
		public Note MouseOverNote;

		private double _lastPlayedMS;

		public float ScreenX = 300;

		//public static float Bpm = 0;//150;
		public static List<BPM> BPMs = new List<BPM>();
		public static long BpmOffset = 0; //in ms
		public static float TextBpm = 0;
		public static string wv = null;
		public static int BeatDivisor = 4;

		public GuiTrack(float y, float sy) : base(0, y, EditorWindow.Instance.ClientSize.Width, sy)
		{

		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			// track transparency

			var editor = EditorWindow.Instance;

			int res;
			string trackdim = editor.ReadLine("settings.ini", 8);
			int.TryParse(trackdim, out res);

			// color 1

			string rc1 = editor.ReadLine("settings.ini", 17);
			string[] c1values = rc1.Split(',');
			int[] Color1 = Array.ConvertAll<string, int>(c1values, int.Parse);

			//color 2

			string rc2 = editor.ReadLine("settings.ini", 21);
			string[] c2values = rc2.Split(',');
			int[] Color2 = Array.ConvertAll<string, int>(c2values, int.Parse);

			// waveform

			try
			{
				wv = editor.ReadLine("settings.ini", 30);

			} catch
            {
				
            }

			GL.Color4(Color.FromArgb(res, 36, 35, 33));

			var rect = ClientRectangle;

			Glu.RenderQuad(rect);
			GL.Color3(0.2f, 0.2f, 0.2f);
			Glu.RenderQuad((int)rect.X, (int)rect.Y + rect.Height, (int)rect.Width, 1);

			var fr = editor.FontRenderer;

			float cellSize = rect.Height;
			float noteSize = cellSize * 0.65f;

			var gap = cellSize - noteSize;

			double audioTime = editor.MusicPlayer.CurrentTime.TotalMilliseconds;

			float cubeStep = editor.CubeStep;
			float posX = (float)audioTime / 1000 * cubeStep;
			float maxX = (float)editor.MusicPlayer.TotalTime.TotalMilliseconds / 1000f * cubeStep;

			var zoomLvl = editor.Zoom;
			float lineSpace = cubeStep * zoomLvl;

			float lineX = ScreenX - posX;
			if (lineX < 0)
				lineX %= lineSpace;

			if (wv == "true")
            {
				GL.Color3(0.35f, 0.35f, 0.35f);
				GL.PushMatrix();
				GL.BindVertexArray(editor.MusicPlayer.WaveModel.VaoID);
				GL.EnableVertexAttribArray(0);

				var p = posX / maxX;
				var total = zoomLvl * maxX;

				var waveX = -posX + ScreenX + maxX / 2;
				var scale = maxX;//;total;

				GL.Translate(waveX, rect.Height * 0.5, 0);
				GL.Scale(scale / 100000.0, -rect.Height, 1);
				GL.Translate(-50000, -0.5, 0);
				GL.LineWidth(2);
				editor.MusicPlayer.WaveModel.Render(PrimitiveType.LineStrip);
				GL.LineWidth(1);
				GL.Translate(50000 * scale, 0.5, 0);
				GL.Scale(1 / scale * 100000.0, -1.0 / rect.Height, 1);
				GL.Translate(-waveX, -rect.Height * 0.5, 0);

				GL.DisableVertexAttribArray(0);
				GL.BindVertexArray(0);
				GL.PopMatrix();
			}
			/*
			GL.Begin(PrimitiveType.LineStrip);

			for (double x = 0; x < rect.Width + 4; x += 4)
			{
				var peak = editor.MusicPlayer.GetPeak(audioTime + (x - ScreenX) / cubeStep * 1000) * rect.Height;

				GL.Vertex2(x + 0.5f, rect.Height - peak);
			}
			GL.End();*/

			//render quarters of a second depending on zoom level
			/*
			while (lineSpace > 0 && lineX < rect.Width)
			{
				GL.Color3(0.85f, 0.85f, 0.85f);
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex2((int)lineX + 0.5f, rect.Y);
				GL.Vertex2((int)lineX + 0.5f, rect.Y + 5);
				GL.End();

				lineX += lineSpace;
			}*/

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
			Note closest = null;

			_cs.Reset();
			for (int i = 0; i < editor.Notes.Count; i++)
			{
				Note note = EditorWindow.Instance.Notes[i];

				if (editor.GuiScreen is GuiScreenEditor gse)
				{
					var offset = 0L;
					long.TryParse(gse.SfxOffset.Text, out offset);

					if (note.Ms <= (long)(EditorWindow.Instance.MusicPlayer.CurrentTime.TotalMilliseconds - offset))
					{
						closest = note;
					}
				}

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
				var c = Color.FromArgb((int)(15 * alphaMult), (int)note.Color.R, (int)note.Color.G, (int)note.Color.B);

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

					if (Math.Round(note.X, 3) == indexX && Math.Round(note.Y, 3) == indexY)
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

			if (_lastPlayedNote != closest)
			{
				_lastPlayedNote = closest;

				if (closest != null && editor.MusicPlayer.IsPlaying && editor.GuiScreen is GuiScreenEditor gse)
				{
					editor.SoundPlayer.Play("hit", gse.SfxVolume.Value / (float)gse.SfxVolume.MaxValue, editor.MusicPlayer.Tempo);
				}
			}

			for (var i = 0; i < BPMs.Count; i++ )
            {
				var Bpm = BPMs[i];
				double nextoffset = 0;
				double nextLineX = endLineX;
				double currentoffset = Bpm.Ms;
				double offsetint = 60000 / Bpm.bpm / BeatDivisor;

				if (i + 1 < BPMs.Count)
                {
					nextoffset = BPMs[i + 1].Ms;
                }
				else
                {
					nextoffset = editor.MusicPlayer.TotalTime.TotalMilliseconds * 2;
                }

				if (Bpm.bpm > 33)
				{
					lineSpace = 60 / Bpm.bpm * cubeStep;
					var stepSmall = lineSpace / BeatDivisor;

					lineX = ScreenX - posX + Bpm.Ms / 1000f * cubeStep;
					if (lineX < 0)
						lineX %= lineSpace;

					if (i + 1 < BPMs.Count)
						nextLineX = ScreenX - posX + nextoffset / 1000f * cubeStep;
					if (nextLineX < 0)
						nextLineX %= lineSpace;

					if (lineSpace > 0 && lineX < rect.Width && lineX > 0)
					{
						//draw offset start line
						GL.Color4(Color.FromArgb(255, 0, 0));
						GL.Begin(PrimitiveType.Lines);
						GL.Vertex2((int)lineX + 0.5f, 0);
						GL.Vertex2((int)lineX + 0.5f, rect.Bottom + 56);
						GL.End();
					}

					//draw timing point info
					var x = Math.Round(ScreenX - posX + Bpm.Ms / 1000f * cubeStep);

					var numText = $"{Bpm.bpm:##,###}";

					GL.Color3(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
					fr.Render(numText, (int)x + 3, (int)(rect.Y + rect.Height) + 3 + 28, 16);

					GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
					fr.Render($"{Bpm.Ms:##,###}ms", (int)x + 3,
						(int)(rect.Y + rect.Height + fr.GetHeight(16)) + 3 + 2 + 28, 16);

					//render BPM lines
					while (lineSpace > 0 && lineX < rect.Width && lineX < endLineX && lineX < nextLineX)
					{
						GL.Color3(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
						GL.Begin(PrimitiveType.Lines);
						GL.Vertex2((int)lineX + 0.5f, rect.Bottom);
						GL.Vertex2((int)lineX + 0.5f, rect.Bottom - 11);
						GL.End();

						for (int j = 1; j <= BeatDivisor; j++)
						{
							var xo = lineX + j * stepSmall;

							if (j < BeatDivisor && xo < endLineX && xo < nextLineX)
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
			}

			if (editor.GuiScreen is GuiScreenEditor gse1 && gse1.Metronome.Toggle)
			{
				double.TryParse(gse1.SfxOffset.Text, out var offset);

				var ms = editor.MusicPlayer.CurrentTime.TotalMilliseconds - offset;
				var bpm = editor.GetCurrentBpm(editor.MusicPlayer.CurrentTime.TotalMilliseconds);
				double interval = 60000 / bpm.bpm / BeatDivisor;
				double remainder = (ms - bpm.Ms) % interval;
				double closestMS = ms - remainder;

				if (_lastPlayedMS != closestMS && remainder >= 0 && editor.MusicPlayer.IsPlaying)
				{
					_lastPlayedMS = closestMS;

					editor.SoundPlayer.Play("metronome", gse1.SfxVolume.Value / (float)gse1.SfxVolume.MaxValue, editor.MusicPlayer.Tempo);
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
				Note closest = null;

				if (EditorWindow.Instance.GuiScreen is GuiScreenEditor gse)
				{
					var offset = 0L;
					long.TryParse(gse.SfxOffset.Text, out offset);

					if (note.Ms <= (long)(EditorWindow.Instance.MusicPlayer.CurrentTime.TotalMilliseconds - offset))
					{
						closest = note;
					}
				}

				//caused flashing
				//note.Color = _cs.Next();

				float x = ScreenX - posX + note.Ms / 1000f * cubeStep;

				/*
				if (x < rect.X - noteSize || x > rect.Width)
					continue;
				*/

				float y = rect.Y + gap / 2;

				var noteRect = new RectangleF(x, y, noteSize, noteSize);

				if (selectionRect.IntersectsWith(noteRect))
					notes.Add(note);
			}

			return notes;
		}
	}
}