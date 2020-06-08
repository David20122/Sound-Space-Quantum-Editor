using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sound_Space_Editor.Properties;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
namespace Sound_Space_Editor.Gui
{
	class GuiScreenEditor : GuiScreen
	{
		public GuiScreen GuiScreen { get; private set; }
        public bool AutoSave = false;
		public readonly GuiGrid Grid = new GuiGrid(300, 300);
		public readonly GuiTrack Track = new GuiTrack(0, 64);
		public readonly GuiSlider Tempo;
		public readonly GuiSlider MasterVolume;
		public readonly GuiSlider SfxVolume;
		public readonly GuiSlider BeatSnapDivisor;
		public readonly GuiSlider Timeline;
        public readonly GuiTextBox Bpm;
		public readonly GuiTextBox Offset;
		public readonly GuiTextBox NoteAlign;
		public readonly GuiCheckBox Reposition;
		public readonly GuiCheckBox Autoplay;
		public readonly GuiCheckBox ApproachSquares;
		public readonly GuiCheckBox GridNumbers;
		public readonly GuiCheckBox Quantum;
		public readonly GuiCheckBox AutoAdvance;
		public readonly GuiButton BackButton;
		public readonly GuiButton CopyButton;
		public readonly GuiButton SetOffset;

		private readonly GuiLabel _toast;
		private float _toastTime;

		public GuiScreenEditor() : base(0, EditorWindow.Instance.ClientSize.Height - 64, EditorWindow.Instance.ClientSize.Width - 512 - 64, 64)
		{
			_toast = new GuiLabel(0, 0, "")
			{
				Centered = true,
				FontSize = 36
			};

			var playPause = new GuiButtonPlayPause(0, EditorWindow.Instance.ClientSize.Width - 512 - 64, EditorWindow.Instance.ClientSize.Height - 64, 64, 64);
			Bpm = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				Decimal = true
			};
			Offset = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = true
			};
			NoteAlign = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0.5",
				Centered = true,
				Numeric = true,
				Decimal = true
			};
			Reposition = new GuiCheckBox(1, "Offset Notes", 10, 0, 32, 32, false);
			BeatSnapDivisor = new GuiSlider(0, 0, 256, 40);
			Timeline = new GuiSliderTimeline(0, 0, EditorWindow.Instance.ClientSize.Width, 64);
			Tempo = new GuiSlider(0, 0, 512, 64)
			{
				MaxValue = 8,
				Value = 8
			};

			Timeline.Snap = false;
			BeatSnapDivisor.Value = Track.BeatDivisor - 1;
			BeatSnapDivisor.MaxValue = 23;

			MasterVolume = new GuiSlider(0, 0, 40, 256)
			{
				MaxValue = 50
			};
			SfxVolume = new GuiSlider(0, 0, 40, 256)
			{
				MaxValue = 50
			};

			SetOffset = new GuiButton(2, 0, 0, 64, 32, "SET");
			BackButton = new GuiButton(3, 0, 0, Grid.ClientRectangle.Width + 1, 42, "BACK TO MENU");
			CopyButton = new GuiButton(4, 0, 0, Grid.ClientRectangle.Width + 1, 42, "COPY MAP DATA");

			Autoplay = new GuiCheckBox(5, "Autoplay", 0, 0, 32, 32, Settings.Default.Autoplay);
			ApproachSquares = new GuiCheckBox(5, "Approach Squares", 0, 0, 32, 32, Settings.Default.ApproachSquares);
			GridNumbers = new GuiCheckBox(5, "Grid Numbers", 0, 0, 32, 32, Settings.Default.GridNumbers);
			Quantum = new GuiCheckBox(5, "Quantum", 0, 0, 32, 32, Settings.Default.Quantum);
			AutoAdvance = new GuiCheckBox(5, "Auto-Advance", 0, 0, 32, 32, Settings.Default.AutoAdvance);

			Bpm.Focused = true;
			Offset.Focused = true;
			NoteAlign.Focused = true;
			Bpm.OnKeyDown(Key.Right, false);
			Offset.OnKeyDown(Key.Right, false);
			NoteAlign.OnKeyDown(Key.Right, false);
			Bpm.Focused = false;
			Offset.Focused = false;
			NoteAlign.Focused = false;
			
			Buttons.Add(playPause);
			Buttons.Add(Timeline);
			Buttons.Add(Tempo);
			Buttons.Add(MasterVolume);
			Buttons.Add(SfxVolume);
			Buttons.Add(BeatSnapDivisor);
			Buttons.Add(Reposition);
			Buttons.Add(Autoplay);
			Buttons.Add(ApproachSquares);
			Buttons.Add(GridNumbers);
			Buttons.Add(Quantum);
			Buttons.Add(AutoAdvance);
			Buttons.Add(SetOffset);
			Buttons.Add(BackButton);
			Buttons.Add(CopyButton);

			OnResize(EditorWindow.Instance.ClientSize);

			EditorWindow.Instance.MusicPlayer.Volume = (float)Settings.Default.MasterVolume;

			MasterVolume.Value = (int)(Settings.Default.MasterVolume * MasterVolume.MaxValue);
			SfxVolume.Value = (int)(Settings.Default.SFXVolume * SfxVolume.MaxValue);

		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			_toastTime = Math.Min(2, _toastTime + delta);

			var toastOffY = 1f;

			if (_toastTime <= 0.5)
			{
				toastOffY = (float)Math.Sin(Math.Min(0.5, _toastTime) / 0.5 * MathHelper.PiOver2);
			}
			else if (_toastTime >= 1.75)
			{
				toastOffY = (float)Math.Cos(Math.Min(0.25, _toastTime - 1.75) / 0.25 * MathHelper.PiOver2);
			}

			var size = EditorWindow.Instance.ClientSize;
			var fr = EditorWindow.Instance.FontRenderer;
			var h = fr.GetHeight(_toast.FontSize);

			_toast.ClientRectangle.Y = size.Height - toastOffY * h * 3.25f + h / 2;
			_toast.Color = Color.FromArgb((int)(Math.Pow(toastOffY, 3) * 255), _toast.Color);

			GL.Color3(Color.FromArgb(0, 255, 200));
			var zoomW = fr.GetWidth("Zoom: ", 24);

			fr.Render("Zoom: ", (int)Bpm.ClientRectangle.X, (int)Bpm.ClientRectangle.Y - 60, 24);
			GL.Color3(Color.FromArgb(255, 0, 255));
			fr.Render($"{(int)(EditorWindow.Instance.Zoom * 100)}%", (int)Bpm.ClientRectangle.X + zoomW, (int)Bpm.ClientRectangle.Y - 60, 24);
			GL.Color3(Color.FromArgb(0, 255, 200));
			fr.Render("BPM:", (int)Bpm.ClientRectangle.X, (int)Bpm.ClientRectangle.Y - 24, 24);
			fr.Render("BPM Offset[ms]:", (int)Offset.ClientRectangle.X, (int)Offset.ClientRectangle.Y - 24, 24);
			fr.Render("Options:", (int)Autoplay.ClientRectangle.X, (int)Autoplay.ClientRectangle.Y - 26, 24);
			fr.Render("Snapping", (int)Grid.ClientRectangle.X + 405, (int)Grid.ClientRectangle.Y + 70, 24);
			var divisor = $"Beat Divisor: {BeatSnapDivisor.Value + 1}";
			var divisorW = fr.GetWidth(divisor, 24);

			fr.Render(divisor, (int)(BeatSnapDivisor.ClientRectangle.X + BeatSnapDivisor.ClientRectangle.Width / 2 - divisorW / 2f), (int)BeatSnapDivisor.ClientRectangle.Y - 20, 24);

			var tempo = $"TEMPO - {Tempo.Value * 10 + 20}%";
			var tempoW = fr.GetWidth(tempo, 24);

			fr.Render(tempo, (int)(Tempo.ClientRectangle.X + Tempo.ClientRectangle.Width / 2 - tempoW / 2f), (int)Tempo.ClientRectangle.Bottom - 24, 24);

			var masterW = fr.GetWidth("Master", 18);
			var sfxW = fr.GetWidth("SFX", 18);

			var masterP = $"{(int)(MasterVolume.Value * 100f) / MasterVolume.MaxValue}";
			var sfxP = $"{(int)(SfxVolume.Value * 100f) / SfxVolume.MaxValue}";

			var masterPw = fr.GetWidth(masterP, 18);
			var sfxPw = fr.GetWidth(sfxP, 18);

			fr.Render("Music", (int)(MasterVolume.ClientRectangle.X + SfxVolume.ClientRectangle.Width / 2 - masterW / 2f), (int)MasterVolume.ClientRectangle.Y - 2, 18);
			fr.Render("SFX", (int)(SfxVolume.ClientRectangle.X + SfxVolume.ClientRectangle.Width / 2 - sfxW / 2f), (int)SfxVolume.ClientRectangle.Y - 2, 18);

			fr.Render(masterP, (int)(MasterVolume.ClientRectangle.X + SfxVolume.ClientRectangle.Width / 2 - masterPw / 2f), (int)MasterVolume.ClientRectangle.Bottom - 16, 18);
			fr.Render(sfxP, (int)(SfxVolume.ClientRectangle.X + SfxVolume.ClientRectangle.Width / 2 - sfxPw / 2f), (int)SfxVolume.ClientRectangle.Bottom - 16, 18);

			var rect = Timeline.ClientRectangle;

			var timelinePos = new Vector2(rect.Height / 2f, rect.Height / 2f - 5);
			var time = EditorWindow.Instance.MusicPlayer.TotalTime;
			var currentTime = EditorWindow.Instance.MusicPlayer.CurrentTime;

			var timeString = $"{time.Minutes}:{time.Seconds:0#}";
			var currentTimeString = $"{currentTime.Minutes}:{currentTime.Seconds:0#}";
			var currentMsString = $"{(long) currentTime.TotalMilliseconds:##,###}ms";
			if ((long) currentTime.TotalMilliseconds == 0)
				currentMsString = "0ms";

			var notesString = $"{EditorWindow.Instance.Notes.Count} Notes";
			var notesW = fr.GetWidth(notesString, 24);

			var timeW = fr.GetWidth(timeString, 20);
			var currentTimeW = fr.GetWidth(currentTimeString, 20);
			var currentMsW = fr.GetWidth(currentMsString, 20);

			fr.Render(notesString, (int)(rect.X + rect.Width / 2 - notesW / 2f), (int)(rect.Y + timelinePos.Y + 12), 24);

			GL.Color3(Color.FromArgb(0, 255, 200));
			fr.Render(timeString, (int)(rect.X + timelinePos.X - timeW / 2f + rect.Width - rect.Height), (int)(rect.Y + timelinePos.Y + 12), 20);
			fr.Render(currentTimeString, (int)(rect.X + timelinePos.X - currentTimeW / 2f), (int)(rect.Y + timelinePos.Y + 12), 20);
			fr.Render(currentMsString, (int)(rect.X + rect.Height / 2 + (rect.Width - rect.Height) * Timeline.Progress - currentMsW / 2f), (int)rect.Y, 20);

			base.Render(delta, mouseX, mouseY);

			_toast.Render(delta, mouseX, mouseY);

			Grid.Render(delta, mouseX, mouseY);
			Track.Render(delta, mouseX, mouseY);
			Bpm.Render(delta, mouseX, mouseY);
			NoteAlign.Render(delta, mouseX, mouseY);
			Offset.Render(delta, mouseX, mouseY);

            if (AutoSave)
            {
                AutoSave = false;
                ShowToast("AUTOSAVING", Color.FromArgb(0, 255, 200));
            }
		}

		public override bool AllowInput()
		{
			return !Bpm.Focused && !Offset.Focused && !NoteAlign.Focused;
		}

		public override void OnKeyTyped(char key)
		{
			Bpm.OnKeyTyped(key);
			Offset.OnKeyTyped(key);
			NoteAlign.OnKeyTyped(key);

			UpdateTrack();
		}

		public override void OnKeyDown(Key key, bool control)
		{
			Bpm.OnKeyDown(key, control);
			Offset.OnKeyDown(key, control);
			NoteAlign.OnKeyDown(key, control);

			UpdateTrack();
		}

		public override void OnMouseClick(float x, float y)
		{
			Bpm.OnMouseClick(x, y);
			Offset.OnMouseClick(x, y);
			NoteAlign.OnMouseClick(x, y);

			base.OnMouseClick(x, y);
		}

		protected override void OnButtonClicked(int id)
		{
			switch (id)
			{
				case 0:
					if (EditorWindow.Instance.MusicPlayer.IsPlaying)
						EditorWindow.Instance.MusicPlayer.Pause();
					else
						EditorWindow.Instance.MusicPlayer.Play();
					break;
				case 2:
					long oldOffset = GuiTrack.BpmOffset;

					long.TryParse(Offset.Text, out var newOffset);

					var toggle = Reposition.Toggle;
					var change = newOffset - oldOffset;

					void Redo()
					{
						Offset.Focused = false;
						Offset.Text = newOffset.ToString();

						if (toggle)
						{
							var list = EditorWindow.Instance.Notes.ToList();

							foreach (var note in list)
							{
								note.Ms += change;
							}
						}

						GuiTrack.BpmOffset = newOffset;
					}

					Redo();

					EditorWindow.Instance.UndoRedo.AddUndoRedo("CHANGE OFFSET", () =>
					{
						Offset.Focused = false;
						Offset.Text = oldOffset.ToString();

						if (toggle)
						{
							var list = EditorWindow.Instance.Notes.ToList();

							foreach (var note in list)
							{
								note.Ms -= change;
							}
						}

						GuiTrack.BpmOffset = oldOffset;
					}, Redo);
					break;
				case 3:
					if (EditorWindow.Instance.WillClose())
					{
						EditorWindow.Instance.UndoRedo.Clear();
						EditorWindow.Instance.MusicPlayer.Reset();
						EditorWindow.Instance.OpenGuiScreen(new GuiScreenLoadCreate());
					}
					break;
				case 4:
					Clipboard.SetText(EditorWindow.Instance.ParseData());
					ShowToast("COPIED TO CLIPBOARD", Color.FromArgb(0, 255, 200));
					break;
				case 5:
					Settings.Default.Autoplay = Autoplay.Toggle;
					Settings.Default.ApproachSquares = ApproachSquares.Toggle;
					Settings.Default.GridNumbers = GridNumbers.Toggle;
					Settings.Default.Quantum = Quantum.Toggle;
					Settings.Default.AutoAdvance = AutoAdvance.Toggle;
					Settings.Default.Save();
					break;
			}
		}

		public override void OnResize(Size size)
		{
			Buttons[0].ClientRectangle = new RectangleF(size.Width - 512 - 64, size.Height - 64, 64, 64);

			ClientRectangle = new RectangleF(0, size.Height - 64, size.Width - 512 - 64, 64);

			Track.OnResize(size);
			Tempo.OnResize(size);
			MasterVolume.OnResize(size);

			MasterVolume.ClientRectangle.Location = new PointF(EditorWindow.Instance.ClientSize.Width - 64, EditorWindow.Instance.ClientSize.Height - MasterVolume.ClientRectangle.Height - 64);
			SfxVolume.ClientRectangle.Location = new PointF(MasterVolume.ClientRectangle.X - 64, EditorWindow.Instance.ClientSize.Height - SfxVolume.ClientRectangle.Height - 64);

			Grid.ClientRectangle = new RectangleF((int)(size.Width / 2f - Grid.ClientRectangle.Width / 2), (int)((size.Height + Track.ClientRectangle.Height - 64) / 2 - Grid.ClientRectangle.Height / 2), Grid.ClientRectangle.Width, Grid.ClientRectangle.Height);
			BackButton.ClientRectangle.Location = new PointF(Grid.ClientRectangle.X, Grid.ClientRectangle.Bottom + 5 + 1 + 23);
			CopyButton.ClientRectangle.Location = new PointF(Grid.ClientRectangle.X, Grid.ClientRectangle.Y - CopyButton.ClientRectangle.Height - 8);
			BeatSnapDivisor.ClientRectangle.Location = new PointF(EditorWindow.Instance.ClientSize.Width - BeatSnapDivisor.ClientRectangle.Width, Bpm.ClientRectangle.Y);
			Timeline.ClientRectangle = new RectangleF(0, EditorWindow.Instance.ClientSize.Height - 64, EditorWindow.Instance.ClientSize.Width - 512 - 64, 64);
			Tempo.ClientRectangle = new RectangleF(EditorWindow.Instance.ClientSize.Width - 512, EditorWindow.Instance.ClientSize.Height - 64, 512, 64);

			// sm
			Bpm.ClientRectangle.Y = Grid.ClientRectangle.Y + 28;
			Offset.ClientRectangle.Y = Bpm.ClientRectangle.Bottom + 5 + 24 + 10;
			NoteAlign.ClientRectangle.Y = Grid.ClientRectangle.Y + 100;
			SetOffset.ClientRectangle.Y = Offset.ClientRectangle.Y;
			Reposition.ClientRectangle.Y = Offset.ClientRectangle.Bottom + 10;
			BeatSnapDivisor.ClientRectangle.Y = Bpm.ClientRectangle.Y;

			Autoplay.ClientRectangle.Y = Reposition.ClientRectangle.Bottom + 32 + 20;
			ApproachSquares.ClientRectangle.Y = Autoplay.ClientRectangle.Bottom + 10;
			GridNumbers.ClientRectangle.Y = ApproachSquares.ClientRectangle.Bottom + 10;
			Quantum.ClientRectangle.Y = GridNumbers.ClientRectangle.Bottom + 10;
			AutoAdvance.ClientRectangle.Y = CopyButton.ClientRectangle.Y - 10;

			Bpm.ClientRectangle.X = 10;
			Offset.ClientRectangle.X = Bpm.ClientRectangle.X;
			SetOffset.ClientRectangle.X = Bpm.ClientRectangle.Right + 5;
			NoteAlign.ClientRectangle.X = Grid.ClientRectangle.X + 390;
			Reposition.ClientRectangle.X = Bpm.ClientRectangle.X;

			Autoplay.ClientRectangle.X = Bpm.ClientRectangle.X;
			ApproachSquares.ClientRectangle.X = Bpm.ClientRectangle.X;
			GridNumbers.ClientRectangle.X = Bpm.ClientRectangle.X;
			Quantum.ClientRectangle.X = Bpm.ClientRectangle.X;
			AutoAdvance.ClientRectangle.X = BeatSnapDivisor.ClientRectangle.X + 20;

			_toast.ClientRectangle.X = size.Width / 2f;
		}

		public void OnMouseLeave()
		{
			Timeline.Dragging = false;
			Tempo.Dragging = false;
			MasterVolume.Dragging = false;
			SfxVolume.Dragging = false;
			BeatSnapDivisor.Dragging = false;
		}

		private void UpdateTrack()
		{
			if (Bpm.Focused)
			{
				var text = Bpm.Text;
				var text2 = NoteAlign.Text;
				var decimalPont = false;

				if (text.Length > 0 && text[text.Length - 1].ToString() ==
				    CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
				{
					text = text + 0;

					decimalPont = true;
				}

				if (text2.Length > 0 && text2[text2.Length - 1].ToString() ==
					CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
				{
					text2 = text2 + 0;

					decimalPont = true;
				}

				decimal.TryParse(text, out var bpm);

				if (bpm < 0)
					bpm = 0;
				else if (bpm > 3000)
					bpm = 3000;

				GuiTrack.Bpm = (float)bpm;

				if (GuiTrack.Bpm > 0 && !decimalPont)
					Bpm.Text = GuiTrack.Bpm.ToString();

				// notealign
				decimal.TryParse(text2, out var notealignincrement);

				if (notealignincrement <= 0)
					notealignincrement = 0;
				else if (notealignincrement >= 1)
					notealignincrement = 1;

				if (notealignincrement > 0 && !decimalPont)
					NoteAlign.Text = notealignincrement.ToString();
			}
			if (Offset.Focused)
			{
				long.TryParse(Offset.Text, out var offset);

				offset = Math.Max(0, offset);

				if (offset > 0)
					Offset.Text = offset.ToString();
			}
		}

		public void ShowToast(string text, Color color)
		{
			_toastTime = 0;

			_toast.Text = text;
			_toast.Color = color;
		}
	}
}