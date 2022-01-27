using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sound_Space_Editor.Properties;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;

namespace Sound_Space_Editor.Gui
{
	class GuiScreenEditor : GuiScreen
	{
		public GuiScreen GuiScreen { get; private set; }

		private List<GuiTextBox> Boxes = new List<GuiTextBox>();

		public readonly GuiGrid Grid = new GuiGrid(300, 300);
		public readonly GuiTrack Track = new GuiTrack(0, 80);
		public readonly GuiSlider Tempo;
		public readonly GuiSlider MasterVolume;
		public readonly GuiSlider SfxVolume;
		public readonly GuiSlider BeatSnapDivisor;
		public readonly GuiSlider Timeline;
		public readonly GuiSlider NoteAlign;
		public readonly GuiTextBox Offset;
		public readonly GuiTextBox SfxOffset;
		public readonly GuiTextBox JumpMSBox;
		public readonly GuiTextBox RotateBox;
		public readonly GuiTextBox BezierBox;
		public readonly GuiCheckBox Autoplay;
		public readonly GuiCheckBox ApproachSquares;
		public readonly GuiCheckBox GridNumbers;
		public readonly GuiCheckBox Quantum;
		public readonly GuiCheckBox Numpad;
		public readonly GuiCheckBox AutoAdvance;
		public readonly GuiCheckBox QuantumGridLines;
		public readonly GuiCheckBox QuantumGridSnap;
		public readonly GuiCheckBox Metronome;
		public readonly GuiCheckBox DynamicBezier;
		//public readonly GuiCheckBox LegacyBPM;
		public readonly GuiButton BackButton;
		public readonly GuiButton CopyButton;
		public readonly GuiButton SetOffset;
		public readonly GuiButton JumpMSButton;
		public readonly GuiButton RotateButton;
		public readonly GuiButton BezierButton;
		public readonly GuiButton BezierStoreButton;
		public readonly GuiButton BezierClearButton;

		public readonly GuiButton OpenTimings;
		public readonly GuiButton UseCurrentMs;

		public readonly GuiButton HFlip;
		public readonly GuiButton VFlip;

		public readonly GuiButton OptionsNav;
		public readonly GuiButton TimingNav;
		public readonly GuiButton PatternsNav;

		public readonly GuiTextBox ScaleBox;
		public readonly GuiButton ScaleButton;

		public readonly GuiSlider TrackHeight;

		public readonly GuiTextBox MSBoundLower;
		public readonly GuiTextBox MSBoundHigher;
		public readonly GuiButton SelectBound;

		private bool OptionsNavEnabled = false;
		private bool TimingNavEnabled = false;
		private bool PatternsNavEnabled = false;

        public float AutoSaveTimer { get; private set; } = 0f;

		private readonly GuiLabel _toast;
		private float _toastTime;
		private readonly int _textureId;
		private bool bgImg = false;

		private List<Note> beziernodes;

		//private object TimingPanel;
		//TimingPoints TimingPoints;

		public BigInteger FactorialApprox(int k)
        {
			var result = new BigInteger(1);
			if (k < 10)
            {
				for (int i = 1; i <= k; i++)
                {
					result *= i;
                }
            }
			else
            {
				result = (BigInteger)(Math.Sqrt(2 * Math.PI * k) * Math.Pow(k / Math.E, k));
			}
			return result;
		}

		public BigInteger BinomialCoefficient(int k, int v)
        {
			var bic = FactorialApprox(k) / (FactorialApprox(v) * FactorialApprox(k - v));
			return bic;
		}

		public GuiScreenEditor() : base(0, EditorWindow.Instance.ClientSize.Height - 64, EditorWindow.Instance.ClientSize.Width - 512 - 64, 64)
		{
			if (File.Exists(Path.Combine(EditorWindow.Instance.LauncherDir, "background_editor.png")))
			{
				this.bgImg = true;
				using (Bitmap img = new Bitmap(Path.Combine(EditorWindow.Instance.LauncherDir, "background_editor.png")))
				{
					this._textureId = TextureManager.GetOrRegister("bg", img, true);
				}
			}

			_toast = new GuiLabel(0, 0, "", false)
			{
				Centered = true,
				FontSize = 36
			};

			var playPause = new GuiButtonPlayPause(0, EditorWindow.Instance.ClientSize.Width - 512 - 64, EditorWindow.Instance.ClientSize.Height - 64, 64, 64);
			Offset = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false
			};
			JumpMSBox = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			MSBoundLower = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			MSBoundHigher = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			RotateBox = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "90",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			BezierBox = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "4",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			SfxOffset = new GuiTextBox(EditorWindow.Instance.ClientSize.Width - 128, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = true
			};
			NoteAlign = new GuiSlider(0, 0, 256, 40)
			{
				MaxValue = 59,
                Value = 2
			};
			BeatSnapDivisor = new GuiSlider(0, 0, 256, 40);
			Timeline = new GuiSliderTimeline(0, 0, EditorWindow.Instance.ClientSize.Width, 64);
			Tempo = new GuiSlider(0, 0, 512, 64)
			{
				MaxValue = 26,
				Value = 16
			};

			Timeline.Snap = false;
			BeatSnapDivisor.Value = GuiTrack.BeatDivisor - 1;
			BeatSnapDivisor.MaxValue = 31;

			MasterVolume = new GuiSlider(0, 0, 40, 256)
			{
				MaxValue = 50
			};
			SfxVolume = new GuiSlider(0, 0, 40, 256)
			{
				MaxValue = 50
			};

			TrackHeight = new GuiSlider(0, 0, 32, 128)
			{
				MaxValue = 32,
				Value = 16,
			};

			SetOffset = new GuiButton(2, 0, 0, 64, 32, "SET", false);
			BackButton = new GuiButton(3, 0, 0, Grid.ClientRectangle.Width + 1, 42, "BACK TO MENU", false);
			CopyButton = new GuiButton(4, 0, 0, Grid.ClientRectangle.Width + 1, 42, "COPY MAP DATA", false);

			JumpMSButton = new GuiButton(6, 0, 0, 64, 32, "JUMP", false);
			RotateButton = new GuiButton(7, 0, 0, 64, 32, "ROTATE", false);
			BezierButton = new GuiButton(10, 0, 0, 64, 32, "DRAW", false);
			BezierStoreButton = new GuiButton(13, 0, 0, 128, 32, "STORE NODES", false);
			BezierClearButton = new GuiButton(14, 0, 0, 128, 32, "CLEAR NODES", false);

			OpenTimings = new GuiButton(8, 0, 0, 200, 32, "OPEN BPM SETUP", false);
			UseCurrentMs = new GuiButton(9, 0, 0, 200, 32, "USE CURRENT MS", false);

			HFlip = new GuiButton(11, 0, 0, 128, 32, "HORIZONTAL FLIP", false);
			VFlip = new GuiButton(12, 0, 0, 128, 32, "VERTICAL FLIP", false);

			OptionsNav = new GuiButton(15, 0, 0, 200, 50, "OPTIONS >", false);
			TimingNav = new GuiButton(16, 0, 0, 200, 50, "TIMING >", false);
			PatternsNav = new GuiButton(17, 0, 0, 200, 50, "PATTERNS >", false);

			SelectBound = new GuiButton(19, 0, 0, 64, 32, "SELECT", false);

			Autoplay = new GuiCheckBox(5, "Autoplay", 0, 0, 32, 32, Settings.Default.Autoplay);
			ApproachSquares = new GuiCheckBox(5, "Approach Squares", 0, 0, 32, 32, Settings.Default.ApproachSquares);
			GridNumbers = new GuiCheckBox(5, "Grid Numbers", 0, 0, 32, 32, Settings.Default.GridNumbers);
			Quantum = new GuiCheckBox(5, "Quantum", 0, 0, 32, 32, Settings.Default.Quantum);
			AutoAdvance = new GuiCheckBox(5, "Auto-Advance", 0, 0, 32, 32, Settings.Default.AutoAdvance);
			Numpad = new GuiCheckBox(5, "Use Numpad", 0, 0, 32, 32, Settings.Default.Numpad);
			QuantumGridLines = new GuiCheckBox(5, "Quantum Grid Lines", 0, 0, 32, 32, Settings.Default.QuantumGridLines);
			QuantumGridSnap = new GuiCheckBox(5, "Snap to Grid", 0, 0, 32, 32, Settings.Default.QuantumGridSnap);
			Metronome = new GuiCheckBox(5, "Metronome", 0, 0, 32, 32, Settings.Default.Metronome);
			DynamicBezier = new GuiCheckBox(5, "Show Bezier Preview", 0, 0, 32, 32, Settings.Default.DynamicBezier);
			//LegacyBPM = new GuiCheckBox(5, "Use Legacy Panel", 0, 0, 24, 24, Settings.Default.LegacyBPM);

			ScaleBox = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "150",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			ScaleButton = new GuiButton(18, 0, 0, 200, 50, "SCALE", false);

			Offset.Focused = true;
			SfxOffset.Focused = true;
			JumpMSBox.Focused = true;
			RotateBox.Focused = true;
			BezierBox.Focused = true;
			ScaleBox.Focused = true;
			MSBoundLower.Focused = true;
			MSBoundHigher.Focused = true;

			Offset.OnKeyDown(Key.Right, false);
			SfxOffset.OnKeyDown(Key.Right, false);
			JumpMSBox.OnKeyDown(Key.Right, false);
			RotateBox.OnKeyDown(Key.Right, false);
			BezierBox.OnKeyDown(Key.Right, false);
			ScaleBox.OnKeyDown(Key.Right, false);
			MSBoundLower.OnKeyDown(Key.Right, false);
			MSBoundHigher.OnKeyDown(Key.Right, false);

			Offset.Focused = false;
			SfxOffset.Focused = false;
			JumpMSBox.Focused = false;
			RotateBox.Focused = false;
			BezierBox.Focused = false;
			ScaleBox.Focused = false;
			MSBoundLower.Focused = false;
			MSBoundHigher.Focused = false;

			Buttons.Add(playPause);
			Buttons.Add(Timeline);
			Buttons.Add(Tempo);
			Buttons.Add(NoteAlign);
			Buttons.Add(MasterVolume);
			Buttons.Add(SfxVolume);
			Buttons.Add(BeatSnapDivisor);
			Buttons.Add(Autoplay);
			Buttons.Add(ApproachSquares);
			Buttons.Add(GridNumbers);
			Buttons.Add(Quantum);
			Buttons.Add(AutoAdvance);
			Buttons.Add(Numpad);
			Buttons.Add(QuantumGridLines);
			Buttons.Add(QuantumGridSnap);
			Buttons.Add(Metronome);
			Buttons.Add(DynamicBezier);
			//Buttons.Add(LegacyBPM);
			Buttons.Add(SetOffset);
			Buttons.Add(BackButton);
			Buttons.Add(CopyButton);
			Buttons.Add(JumpMSButton);
			Buttons.Add(RotateButton);
			Buttons.Add(OpenTimings);
			Buttons.Add(UseCurrentMs);
			Buttons.Add(BezierButton);
			Buttons.Add(BezierClearButton);
			Buttons.Add(BezierStoreButton);
			Buttons.Add(HFlip);
			Buttons.Add(VFlip);
			Buttons.Add(OptionsNav);
			Buttons.Add(TimingNav);
			Buttons.Add(PatternsNav);
			Buttons.Add(ScaleButton);
			Buttons.Add(TrackHeight);
			Buttons.Add(SelectBound);

			Boxes.Add(Offset);
			Boxes.Add(SfxOffset);
			Boxes.Add(JumpMSBox);
			Boxes.Add(RotateBox);
			Boxes.Add(BezierBox);
			Boxes.Add(ScaleBox);
			Boxes.Add(MSBoundLower);
			Boxes.Add(MSBoundHigher);

			HideShowElements();

			EditorWindow.Instance.MusicPlayer.Volume = (float)Settings.Default.MasterVolume;

			SfxOffset.Text = Settings.Default.SfxOffset;
			MasterVolume.Value = (int)(Settings.Default.MasterVolume * MasterVolume.MaxValue);
			SfxVolume.Value = (int)(Settings.Default.SFXVolume * SfxVolume.MaxValue);
			TrackHeight.Value = Settings.Default.TrackHeight;
			// NoteAlign.Value = (int)(Settings.Default.NoteAlign * NoteAlign.MaxValue);

			OnResize(EditorWindow.Instance.ClientSize);

			SfxOffset.OnChanged += (_, value) =>
			{
				Settings.Default.SfxOffset = SfxOffset.Text;
			};
		}

		public bool CanClick(Point pos)
        {
			foreach (var button in Buttons)
            {
				if (button.ClientRectangle.Contains(pos))
					return false;
            }
			foreach (var box in Boxes)
            {
				if (box.ClientRectangle.Contains(pos))
					return false;
            }
			return true;
        }

		public override void Render(float delta, float mouseX, float mouseY)
		{
			var rl = EditorWindow.Instance.inconspicuousvar;

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

			int bgdim = EditorSettings.EditorBGOpacity;

			if (bgImg)
			{
				GL.Color4(Color.FromArgb(bgdim, 255, 255, 255));
				Glu.RenderTexturedQuad(0, 0, size.Width, size.Height, 0, 0, 1, 1, _textureId);
			}

			_toast.ClientRectangle.Y = size.Height - toastOffY * h * 3.25f + h / 2;
			_toast.Color = Color.FromArgb((int)(Math.Pow(toastOffY, 3) * 255), _toast.Color);

			int[] Color1 = EditorWindow.Instance.Color1;
			int[] Color2 = EditorWindow.Instance.Color2;

			GL.Color3(Color.FromArgb(Color1[0],Color1[1],Color1[2]));
			var zoomW = fr.GetWidth("Zoom: ", 24);
			if (rl)
            {
				zoomW = fr.GetWidth("Zoom~ ", 24);
				fr.Render("Zoom~ ", (int)OptionsNav.ClientRectangle.Right + 10, (int)OptionsNav.ClientRectangle.Y, 24);
			}
			else
            {
				fr.Render("Zoom: ", (int)OptionsNav.ClientRectangle.Right + 10, (int)OptionsNav.ClientRectangle.Y, 24);
			}
			
			GL.Color3(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
			fr.Render($"{Math.Round(EditorWindow.Instance.Zoom, 2) * 100}%", (int)OptionsNav.ClientRectangle.Right + zoomW + 10, (int)OptionsNav.ClientRectangle.Y, 24);
			GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));

			var th = 64 + (32 - TrackHeight.Value);

			if (OptionsNavEnabled)
            {
				if (rl)
                {
					var thw = fr.GetWidth($"Twack Height~ 00", 24);
					fr.Render($"Twack Height~ {th}", (int)TrackHeight.ClientRectangle.Left - thw, (int)Metronome.ClientRectangle.Bottom + 10, 24);
				}
				else
                {
					var thw = fr.GetWidth($"Track Height: 00", 24);
					fr.Render($"Track Height: {th}", (int)TrackHeight.ClientRectangle.Left - thw, (int)Metronome.ClientRectangle.Bottom + 10, 24);
				}
			}
			if (rl)
            {
				if (TimingNavEnabled)
					fr.Render("Offset[ms]~", (int)Offset.ClientRectangle.X, (int)Offset.ClientRectangle.Y - 24, 24);
				fr.Render("SFX Offset[ms]~", (int)SfxOffset.ClientRectangle.X, (int)SfxOffset.ClientRectangle.Y - 24, 24);
				fr.Render("Jump to MS~", (int)JumpMSBox.ClientRectangle.X, (int)JumpMSBox.ClientRectangle.Y - 24, 24);
				fr.Render("Sewect between MS~", (int)MSBoundLower.ClientRectangle.X, (int)MSBoundLower.ClientRectangle.Y - 24, 24);
			}
			else
            {
				if (TimingNavEnabled)
					fr.Render("Offset[ms]:", (int)Offset.ClientRectangle.X, (int)Offset.ClientRectangle.Y - 24, 24);
				fr.Render("SFX Offset[ms]:", (int)SfxOffset.ClientRectangle.X, (int)SfxOffset.ClientRectangle.Y - 24, 24);
				fr.Render("Jump to MS:", (int)JumpMSBox.ClientRectangle.X, (int)JumpMSBox.ClientRectangle.Y - 24, 24);
				fr.Render("Select between MS:", (int)MSBoundLower.ClientRectangle.X, (int)MSBoundLower.ClientRectangle.Y - 24, 24);
			}
			if (PatternsNavEnabled)
            {
				if (rl)
                {
					fr.Render("Dwaw Beziew with Divisow~", (int)BezierBox.ClientRectangle.X, (int)BezierBox.ClientRectangle.Y - 24, 24);
					fr.Render("Wotate by Degwees~", (int)RotateBox.ClientRectangle.X, (int)RotateBox.ClientRectangle.Y - 24, 24);
					fr.Render("Scawe by Pewcent~", (int)ScaleBox.ClientRectangle.X, (int)ScaleBox.ClientRectangle.Y - 24, 24);
				}
				else
                {
					fr.Render("Draw Bezier with Divisor:", (int)BezierBox.ClientRectangle.X, (int)BezierBox.ClientRectangle.Y - 24, 24);
					fr.Render("Rotate by Degrees:", (int)RotateBox.ClientRectangle.X, (int)RotateBox.ClientRectangle.Y - 24, 24);
					fr.Render("Scale by Percent:", (int)ScaleBox.ClientRectangle.X, (int)ScaleBox.ClientRectangle.Y - 24, 24);
				}
			}
			var divisor = rl ? $"Beat Divisow~ {BeatSnapDivisor.Value + 1}" : $"Beat Divisor: {BeatSnapDivisor.Value + 1}";
			var divisorW = fr.GetWidth(divisor, 24);
            var align = rl ? $"Snapping~ 3/{(float)(NoteAlign.Value + 1)}" : $"Snapping: 3/{(float)(NoteAlign.Value + 1)}";
            var alignW = fr.GetWidth(align, 24);

            fr.Render(divisor, (int)(BeatSnapDivisor.ClientRectangle.X + BeatSnapDivisor.ClientRectangle.Width / 2 - divisorW / 2f), (int)BeatSnapDivisor.ClientRectangle.Y - 20, 24);
            fr.Render(align, (int)(NoteAlign.ClientRectangle.X + NoteAlign.ClientRectangle.Width / 2 - alignW / 2f), (int)NoteAlign.ClientRectangle.Y - 20, 24);

			var tempoval = Tempo.Value;
			if (tempoval > 15)
				tempoval = (tempoval - 16) * 2 + 16;
            var tempo = $"TEMPO - {tempoval * 5 + 20}%";
			var tempoW = fr.GetWidth(tempo, 24);

			fr.Render(tempo, (int)(Tempo.ClientRectangle.X + Tempo.ClientRectangle.Width / 2 - tempoW / 2f), (int)Tempo.ClientRectangle.Bottom - 24, 24);

			var masterW = rl ? fr.GetWidth("Mastew", 18) : fr.GetWidth("Master", 18);
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

			GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
			fr.Render(timeString, (int)(rect.X + timelinePos.X - timeW / 2f + rect.Width - rect.Height), (int)(rect.Y + timelinePos.Y + 12), 20);
			fr.Render(currentTimeString, (int)(rect.X + timelinePos.X - currentTimeW / 2f), (int)(rect.Y + timelinePos.Y + 12), 20);
			fr.Render(currentMsString, (int)(rect.X + rect.Height / 2 + (rect.Width - rect.Height) * Timeline.Progress - currentMsW / 2f), (int)rect.Y, 20);

			base.Render(delta, mouseX, mouseY);

			_toast.Render(delta, mouseX, mouseY);
			Grid.Render(delta, mouseX, mouseY);
			Track.Render(delta, mouseX, mouseY);
			NoteAlign.Render(delta, mouseX, mouseY);

			foreach (var box in Boxes)
            {
				box.Render(delta, mouseX, mouseY);
            }

			//bezier lines
			GL.LineWidth(2);
			if (Settings.Default.DynamicBezier && int.TryParse(BezierBox.Text, out var bezdivisor) && bezdivisor > 0 && ((!Settings.Default.DynamicBezier && EditorWindow.Instance.SelectedNotes.Count > 1) || (Settings.Default.DynamicBezier && beziernodes != null && beziernodes.Count > 1)))
			{
				try
                {
					var k = beziernodes.Count - 1;
					double d = 1f / (bezdivisor * k);
					float xprev = beziernodes[0].X * Grid.ClientRectangle.Width / 3 + Grid.ClientRectangle.X + Grid.ClientRectangle.Width / 6;
					float yprev = beziernodes[0].Y * Grid.ClientRectangle.Width / 3 + Grid.ClientRectangle.Y + Grid.ClientRectangle.Width / 6;
					for (double t = 0; t <= 1; t += d)
					{
						float xf = 0;
						float yf = 0;
						for (int v = 0; v <= k; v++)
						{
							var note = beziernodes[v];
							var bez = (double)BinomialCoefficient(k, v) * (Math.Pow(1 - t, k - v) * Math.Pow(t, v));

							xf += (float)(bez * note.X);
							yf += (float)(bez * note.Y);
						}

						xf *= Grid.ClientRectangle.Width / 3;
						yf *= Grid.ClientRectangle.Width / 3;

						xf += Grid.ClientRectangle.X + Grid.ClientRectangle.Width / 6;
						yf += Grid.ClientRectangle.Y + Grid.ClientRectangle.Width / 6;

						GL.Color3(Color.FromArgb(255, 255, 255));
						GL.Begin(PrimitiveType.Lines);
						GL.Vertex2(xprev, yprev);
						GL.Vertex2(xf, yf);
						GL.End();
						GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
						Glu.RenderCircle(xf, yf, 4);

						xprev = xf;
						yprev = yf;
					}
				}
				catch
                {
					beziernodes.Clear();
                }
			}
		}

		public override bool AllowInput()
		{
			foreach (var box in Boxes)
            {
				if (box.Focused)
					return false;
            }
			return true;
		}

		public override void OnKeyTyped(char key)
		{
			Offset.OnKeyTyped(key);
			SfxOffset.OnKeyTyped(key);
			JumpMSBox.OnKeyTyped(key);
			RotateBox.OnKeyTyped(key);
			BezierBox.OnKeyTyped(key);
			ScaleBox.OnKeyTyped(key);
			MSBoundLower.OnKeyTyped(key);
			MSBoundHigher.OnKeyTyped(key);

			UpdateTrack();
		}

		public override void OnKeyDown(Key key, bool control)
		{
			Offset.OnKeyDown(key, control);
			SfxOffset.OnKeyDown(key, control);
			JumpMSBox.OnKeyDown(key, control);
			RotateBox.OnKeyDown(key, control);
			BezierBox.OnKeyDown(key, control);
			ScaleBox.OnKeyDown(key, control);
			MSBoundLower.OnKeyDown(key, control);
			MSBoundHigher.OnKeyDown(key, control);

			UpdateTrack();
		}

		public override void OnMouseClick(float x, float y)
		{
			Offset.OnMouseClick(x, y);
			SfxOffset.OnMouseClick(x, y);
			JumpMSBox.OnMouseClick(x, y);
			RotateBox.OnMouseClick(x, y);
			BezierBox.OnMouseClick(x, y);
			ScaleBox.OnMouseClick(x, y);
			MSBoundLower.OnMouseClick(x, y);
			MSBoundHigher.OnMouseClick(x, y);

			base.OnMouseClick(x, y);
		}

		private void HideShowElements()
        {
			//button placement
			var heightdiff = EditorWindow.Instance.ClientSize.Height / 1080f;
			TimingNav.ClientRectangle.Y = OptionsNav.ClientRectangle.Bottom + 10 * heightdiff;
			PatternsNav.ClientRectangle.Y = TimingNav.ClientRectangle.Bottom + 10 * heightdiff;

			//options
			Autoplay.Visible = false;
			ApproachSquares.Visible = false;
			GridNumbers.Visible = false;
			Quantum.Visible = false;
			Numpad.Visible = false;
			QuantumGridLines.Visible = false;
			QuantumGridSnap.Visible = false;
			Metronome.Visible = false;
			TrackHeight.Visible = false;

			//timing
			Offset.Visible = false;
			SetOffset.Visible = false;
			UseCurrentMs.Visible = false;
			OpenTimings.Visible = false;

			//patterns
			RotateBox.Visible = false;
			BezierBox.Visible = false;
			DynamicBezier.Visible = false;
			RotateButton.Visible = false;
			BezierButton.Visible = false;
			BezierStoreButton.Visible = false;
			BezierClearButton.Visible = false;
			HFlip.Visible = false;
			VFlip.Visible = false;
			ScaleBox.Visible = false;
			ScaleButton.Visible = false;

			//button text
			OptionsNav.Text = "OPTIONS >";
			TimingNav.Text = "TIMING >";
			PatternsNav.Text = "PATTERNS >";


			if (OptionsNavEnabled)
            {
				Autoplay.Visible = true;
				ApproachSquares.Visible = true;
				GridNumbers.Visible = true;
				Quantum.Visible = true;
				Numpad.Visible = true;
				QuantumGridLines.Visible = true;
				QuantumGridSnap.Visible = true;
				Metronome.Visible = true;
				TrackHeight.Visible = true;

				TimingNav.ClientRectangle.Y = TrackHeight.ClientRectangle.Bottom + 20 * heightdiff;
				PatternsNav.ClientRectangle.Y = TimingNav.ClientRectangle.Bottom + 10 * heightdiff;

				OptionsNav.Text = "OPTIONS <";
			}

			if (TimingNavEnabled)
            {
				Offset.Visible = true;
				SetOffset.Visible = true;
				UseCurrentMs.Visible = true;
				OpenTimings.Visible = true;

				PatternsNav.ClientRectangle.Y = OpenTimings.ClientRectangle.Bottom + 20 * heightdiff;

				TimingNav.Text = "TIMING <";
			}

			if (PatternsNavEnabled)
            {
				RotateBox.Visible = true;
				BezierBox.Visible = true;
				DynamicBezier.Visible = true;
				RotateButton.Visible = true;
				BezierButton.Visible = true;
				BezierStoreButton.Visible = true;
				BezierClearButton.Visible = true;
				HFlip.Visible = true;
				VFlip.Visible = true;
				ScaleBox.Visible = true;
				ScaleButton.Visible = true;

				PatternsNav.Text = "PATTERNS <";
			}
        }

		protected override void OnButtonClicked(int id)
		{
			switch (id)
			{
				case 0:
					if (EditorWindow.Instance.MusicPlayer.IsPlaying)
						EditorWindow.Instance.MusicPlayer.Pause();
					else
                    {
						if (EditorWindow.Instance.MusicPlayer.CurrentTime.TotalMilliseconds >= EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds - 1)
							EditorWindow.Instance.MusicPlayer.CurrentTime = TimeSpan.FromMilliseconds(0);
						EditorWindow.Instance.MusicPlayer.Play();
					}
					break;
				case 2:
					long oldOffset = GuiTrack.NoteOffset;

					long.TryParse(Offset.Text, out var newOffset);

					var change = newOffset - oldOffset;

					void Redo()
					{
						Offset.Focused = false;
						Offset.Text = newOffset.ToString();

						var list = EditorWindow.Instance.Notes.ToList();

						foreach (var note in list)
						{
							note.Ms += change;
						}

						foreach (var bpm in GuiTrack.BPMs)
                        {
							bpm.Ms += change;
                        }

						GuiTrack.NoteOffset = newOffset;
					}

					Redo();

					EditorWindow.Instance.UndoRedo.AddUndoRedo("CHANGE OFFSET", () =>
					{
						Offset.Focused = false;
						Offset.Text = oldOffset.ToString();

						var list = EditorWindow.Instance.Notes.ToList();

						foreach (var note in list)
						{
							note.Ms -= change;
						}

						foreach (var bpm in GuiTrack.BPMs)
						{
							bpm.Ms -= change;
						}

						GuiTrack.NoteOffset = oldOffset;
					}, Redo);
					break;
				case 3:
					if (EditorWindow.Instance.WillClose())
					{
						EditorWindow.Instance.UndoRedo.Clear();
						EditorWindow.Instance.Notes.Clear();
						EditorWindow.Instance.SelectedNotes.Clear();
						EditorWindow.Instance.MusicPlayer.Reset();
						EditorWindow.Instance.OpenGuiScreen(new GuiScreenMenu());
						EditorWindow.Instance.UpdateActivity("Sitting in the menu");
					}
					break;
				case 4:
                    try
                    {
                        Clipboard.SetText(EditorWindow.Instance.ParseData(true));
                        ShowToast("COPIED TO CLIPBOARD", Color.FromArgb(0, 255, 200));
                    }
                    catch
                    {
						ShowToast("FAILED TO COPY", Color.FromArgb(255, 200, 0));
                    }
					break;
				case 5:
					Settings.Default.Autoplay = Autoplay.Toggle;
					Settings.Default.ApproachSquares = ApproachSquares.Toggle;
					Settings.Default.GridNumbers = GridNumbers.Toggle;
					Settings.Default.Quantum = Quantum.Toggle;
					Settings.Default.AutoAdvance = AutoAdvance.Toggle;
					Settings.Default.Numpad = Numpad.Toggle;
					Settings.Default.QuantumGridLines = QuantumGridLines.Toggle;
					Settings.Default.QuantumGridSnap = QuantumGridSnap.Toggle;
					Settings.Default.Metronome = Metronome.Toggle;
					Settings.Default.SfxOffset = SfxOffset.Text;
					Settings.Default.DynamicBezier = DynamicBezier.Toggle;
					//Settings.Default.LegacyBPM = LegacyBPM.Toggle;
					Settings.Default.Save();
					break;
				case 6:
					var time = long.Parse(JumpMSBox.Text);
					if (time <= EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds)
						EditorWindow.Instance.MusicPlayer.CurrentTime = TimeSpan.FromMilliseconds(time);
					break;
				case 7:
					var degrees = float.Parse(RotateBox.Text);
					var nodes = EditorWindow.Instance.SelectedNotes.ToList();

					foreach (var node in nodes)
                    {
						var angle = MathHelper.RadiansToDegrees(Math.Atan2(node.Y - 1, node.X - 1));
						var distance = Math.Sqrt(Math.Pow(node.X - 1, 2) + Math.Pow(node.Y - 1, 2));
						var finalradians = MathHelper.DegreesToRadians(angle + degrees);

						node.X = (float)(Math.Cos(finalradians) * distance + 1);
						node.Y = (float)(Math.Sin(finalradians) * distance + 1);
                    }
					EditorWindow.Instance.UndoRedo.AddUndoRedo("ROTATE " + degrees.ToString(), () =>
					{
						var undodeg = 360 - degrees;

						foreach (var node in nodes)
						{
							var angle = MathHelper.RadiansToDegrees(Math.Atan2(node.Y - 1, node.X - 1));
							var distance = Math.Sqrt(Math.Pow(node.X - 1, 2) + Math.Pow(node.Y - 1, 2));
							var finalradians = MathHelper.DegreesToRadians(angle + undodeg);

							node.X = (float)(Math.Cos(finalradians) * distance + 1);
							node.Y = (float)(Math.Sin(finalradians) * distance + 1);
						}
					}, () =>
					{
						foreach (var node in nodes)
						{
							var angle = MathHelper.RadiansToDegrees(Math.Atan2(node.Y - 1, node.X - 1));
							var distance = Math.Sqrt(Math.Pow(node.X - 1, 2) + Math.Pow(node.Y - 1, 2));
							var finalradians = MathHelper.DegreesToRadians(angle + degrees);

							node.X = (float)(Math.Cos(finalradians) * distance + 1);
							node.Y = (float)(Math.Sin(finalradians) * distance + 1);
						}
					});
					break;
				case 8:
					/*
					void openGui()
					{
						if (TimingPoints != null)
						{
							TimingPoints.Close();
						}
					TimingPoints = new TimingPoints();
					TimingPoints.Run();
					}

					Thread t = new Thread(new ThreadStart(openGui));
					t.Start();
					*/
					if (TimingsWindow.inst != null)
						TimingsWindow.inst.Close();
					new TimingsWindow().Show();
					break;
				case 9:
					Offset.Text = ((long)EditorWindow.Instance.MusicPlayer.CurrentTime.TotalMilliseconds).ToString();
					break;
				case 10:
					if (int.TryParse(BezierBox.Text, out var divisor) && divisor > 0 && ((!Settings.Default.DynamicBezier && EditorWindow.Instance.SelectedNotes.Count > 1) || (Settings.Default.DynamicBezier && beziernodes != null && beziernodes.Count > 1)))
					{
						try
                        {
							var finalnodes = EditorWindow.Instance.SelectedNotes.ToList();
							if (Settings.Default.DynamicBezier)
								finalnodes = beziernodes;
							var finalnotes = new List<Note>();
							var k = finalnodes.Count - 1;
							float tdiff = finalnodes[k].Ms - finalnodes[0].Ms;
							double d = 1f / (divisor * k);
							for (double t = 0; t <= 1; t += d)
							{
								float xf = 0;
								float yf = 0;
								double tf = finalnodes[0].Ms + tdiff * t;
								for (int v = 0; v <= k; v++)
								{
									var note = finalnodes[v];
									var bez = (double)BinomialCoefficient(k, v) * (Math.Pow(1 - t, k - v) * Math.Pow(t, v));

									xf += (float)(bez * note.X);
									yf += (float)(bez * note.Y);
								}
								finalnotes.Add(new Note(xf, yf, (long)tf));
							}
							EditorWindow.Instance.SelectedNotes.Clear();
							EditorWindow.Instance.Notes.RemoveAll(finalnodes);
							EditorWindow.Instance.Notes.AddAll(finalnotes);
							EditorWindow.Instance.UndoRedo.AddUndoRedo("DRAW BEZIER", () =>
							{
								EditorWindow.Instance.Notes.AddAll(finalnodes);
								EditorWindow.Instance.Notes.RemoveAll(finalnotes);
							}, () =>
							{
								EditorWindow.Instance.Notes.RemoveAll(finalnodes);
								EditorWindow.Instance.Notes.AddAll(finalnotes);
							});
						}
						catch (OverflowException)
                        {
							ShowToast("TOO MANY NODES", Color.FromArgb(255, 200, 0));
						}
						catch
                        {
							ShowToast("FAILED TO DRAW CURVE", Color.FromArgb(255, 200, 0));
                        }
					}
					break;
				case 11:
					var selectedH = EditorWindow.Instance.SelectedNotes.ToList();
					foreach (var node in selectedH)
					{
						node.X = 2 - node.X;
					}

					EditorWindow.Instance.UndoRedo.AddUndoRedo("HORIZONTAL FLIP", () =>
					{
						foreach (var node in selectedH)
						{
							node.X = 2 - node.X;
						}

					}, () =>
					{
						foreach (var node in selectedH)
						{
							node.X = 2 - node.X;
						}

					});
					break;
				case 12:
					var selectedV = EditorWindow.Instance.SelectedNotes.ToList();
					foreach (var node in selectedV)
					{
						node.Y = 2 - node.Y;
					}

					EditorWindow.Instance.UndoRedo.AddUndoRedo("VERTICAL FLIP", () =>
					{
						foreach (var node in selectedV)
						{
							node.Y = 2 - node.Y;
						}

					}, () =>
					{
						foreach (var node in selectedV)
						{
							node.Y = 2 - node.Y;
						}

					});
					break;
				case 13:
					if (EditorWindow.Instance.SelectedNotes.Count > 1)
						beziernodes = EditorWindow.Instance.SelectedNotes.ToList();
					break;
				case 14:
					beziernodes.Clear();
					break;
				case 15:
					OptionsNavEnabled = !OptionsNavEnabled;
					TimingNavEnabled = false;
					PatternsNavEnabled = false;
					HideShowElements();
					break;
				case 16:
					OptionsNavEnabled = false;
					TimingNavEnabled = !TimingNavEnabled;
					PatternsNavEnabled = false;
					HideShowElements();
					break;
				case 17:
					OptionsNavEnabled = false;
					TimingNavEnabled = false;
					PatternsNavEnabled = !PatternsNavEnabled;
					HideShowElements();
					break;
				case 18:
					if (int.TryParse(ScaleBox.Text, out var scale))
                    {
						var scalef = scale / 100f;
						var selected = EditorWindow.Instance.SelectedNotes.ToList();
						foreach (var note in selected)
                        {
							note.X = (note.X - 1) * scalef + 1;
							note.Y = (note.Y - 1) * scalef + 1;
                        }

						EditorWindow.Instance.UndoRedo.AddUndoRedo($"SCALE {scale}%", () =>
						{
							foreach (var note in selected)
							{
								note.X = (note.X - 1) / scalef + 1;
								note.Y = (note.Y - 1) / scalef + 1;
							}

						}, () =>
						{
							foreach (var note in selected)
							{
								note.X = (note.X - 1) * scalef + 1;
								note.Y = (note.Y - 1) * scalef + 1;
							}

						});
                    }
					break;
				case 19:
					if (long.TryParse(MSBoundHigher.Text, out var mshigh) && long.TryParse(MSBoundLower.Text, out var mslow))
                    {
						var mstop = mshigh;
						var msbot = mslow;

						EditorWindow.Instance.SelectedNotes.Clear();

						foreach (var note in EditorWindow.Instance.Notes.ToList())
                        {
							if ((note.Ms > msbot && note.Ms < mstop) || (note.Ms < msbot && note.Ms > mstop))
                            {
								EditorWindow.Instance.SelectedNotes.Add(note);
                            }
                        }

						EditorWindow.Instance._draggedNotes = EditorWindow.Instance.SelectedNotes;
                    }
					break;
			}
		}

		public override void OnResize(Size size)
		{
			Buttons[0].ClientRectangle = new RectangleF(size.Width - 512 - 64, size.Height - 64, 64, 64);

			ClientRectangle = new RectangleF(0, size.Height - 64, size.Width - 512 - 64, 64);

			Track.ClientRectangle.Height = 64 + (32 - TrackHeight.Value);

			Track.OnResize(size);
			Tempo.OnResize(size);
			MasterVolume.OnResize(size);
			NoteAlign.OnResize(size);
			TrackHeight.OnResize(size);

			MasterVolume.ClientRectangle.Location = new PointF(EditorWindow.Instance.ClientSize.Width - 64, EditorWindow.Instance.ClientSize.Height - MasterVolume.ClientRectangle.Height - 64);
			SfxVolume.ClientRectangle.Location = new PointF(MasterVolume.ClientRectangle.X - 64, EditorWindow.Instance.ClientSize.Height - SfxVolume.ClientRectangle.Height - 64);

			Grid.ClientRectangle = new RectangleF((int)(size.Width / 2f - Grid.ClientRectangle.Width / 2), (int)((size.Height + Track.ClientRectangle.Height - 64) / 2 - Grid.ClientRectangle.Height / 2), Grid.ClientRectangle.Width, Grid.ClientRectangle.Height);
			BeatSnapDivisor.ClientRectangle.Location = new PointF(EditorWindow.Instance.ClientSize.Width - BeatSnapDivisor.ClientRectangle.Width, Grid.ClientRectangle.Y + 28);
			Timeline.ClientRectangle = new RectangleF(0, EditorWindow.Instance.ClientSize.Height - 64, EditorWindow.Instance.ClientSize.Width - 512 - 64, 64);
			Tempo.ClientRectangle = new RectangleF(EditorWindow.Instance.ClientSize.Width - 512, EditorWindow.Instance.ClientSize.Height - 64, 512, 64);

			BeatSnapDivisor.ClientRectangle.Y = Grid.ClientRectangle.Y + 28;
            NoteAlign.ClientRectangle.Y = BeatSnapDivisor.ClientRectangle.Bottom + 5 + 24;
			NoteAlign.ClientRectangle.X = BeatSnapDivisor.ClientRectangle.X;

			_toast.ClientRectangle.X = size.Width / 2f;


			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;

			OptionsNav.ClientRectangle.Size = new SizeF(400 * widthdiff, 50 * heightdiff);
			TimingNav.ClientRectangle.Size = OptionsNav.ClientRectangle.Size;
			PatternsNav.ClientRectangle.Size = OptionsNav.ClientRectangle.Size;

			//timing
			Offset.ClientRectangle.Size = new SizeF(128 * widthdiff, 40 * heightdiff);
			SetOffset.ClientRectangle.Size = Offset.ClientRectangle.Size;
			UseCurrentMs.ClientRectangle.Size = new SizeF(Offset.ClientRectangle.Width + SetOffset.ClientRectangle.Width + 5 * widthdiff, Offset.ClientRectangle.Height);
			OpenTimings.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;

			//options
			Autoplay.ClientRectangle.Size = new SizeF(40 * widthdiff, 40 * heightdiff);
			ApproachSquares.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			GridNumbers.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			Quantum.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			Numpad.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			QuantumGridLines.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			QuantumGridSnap.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			Metronome.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			TrackHeight.ClientRectangle.Size = new SizeF(32 * widthdiff, 256 * heightdiff);

			//patterns
			HFlip.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;
			VFlip.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;
			BezierStoreButton.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;
			BezierClearButton.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;
			DynamicBezier.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			BezierBox.ClientRectangle.Size = Offset.ClientRectangle.Size;
			BezierButton.ClientRectangle.Size = SetOffset.ClientRectangle.Size;
			RotateBox.ClientRectangle.Size = Offset.ClientRectangle.Size;
			RotateButton.ClientRectangle.Size = SetOffset.ClientRectangle.Size;
			ScaleBox.ClientRectangle.Size = Offset.ClientRectangle.Size;
			ScaleButton.ClientRectangle.Size = SetOffset.ClientRectangle.Size;

			//etc
			JumpMSButton.ClientRectangle.Size = new SizeF(192 * widthdiff, 40 * heightdiff);
			SfxOffset.ClientRectangle.Size = JumpMSButton.ClientRectangle.Size;
			JumpMSBox.ClientRectangle.Size = JumpMSButton.ClientRectangle.Size;
			AutoAdvance.ClientRectangle.Size = Autoplay.ClientRectangle.Size;

			MSBoundLower.ClientRectangle.Size = JumpMSButton.ClientRectangle.Size;
			MSBoundHigher.ClientRectangle.Size = JumpMSButton.ClientRectangle.Size;
			SelectBound.ClientRectangle.Size = JumpMSButton.ClientRectangle.Size;
			

			OptionsNav.ClientRectangle.Location = new PointF(10 * widthdiff, Track.ClientRectangle.Bottom + 60 * heightdiff);
			TimingNav.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, OptionsNav.ClientRectangle.Bottom + 10 * heightdiff);
			PatternsNav.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, TimingNav.ClientRectangle.Bottom + 10 * heightdiff);

			//timing
			Offset.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, TimingNav.ClientRectangle.Bottom + 40 * heightdiff);
			SetOffset.ClientRectangle.Location = new PointF(Offset.ClientRectangle.Right + 5 * widthdiff, Offset.ClientRectangle.Y);
			UseCurrentMs.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, Offset.ClientRectangle.Bottom + 10 * heightdiff);
			OpenTimings.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, UseCurrentMs.ClientRectangle.Bottom + 10 * heightdiff);

			//options
			Autoplay.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, OptionsNav.ClientRectangle.Bottom + 20 * heightdiff);
			ApproachSquares.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, Autoplay.ClientRectangle.Bottom + 10 * heightdiff);
			GridNumbers.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, ApproachSquares.ClientRectangle.Bottom + 10 * heightdiff);
			Quantum.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, GridNumbers.ClientRectangle.Bottom + 10 * heightdiff);
			Numpad.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, Quantum.ClientRectangle.Bottom + 10 * heightdiff);
			QuantumGridLines.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, Numpad.ClientRectangle.Bottom + 10 * heightdiff);
			QuantumGridSnap.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, QuantumGridLines.ClientRectangle.Bottom + 10 * heightdiff);
			Metronome.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, QuantumGridSnap.ClientRectangle.Bottom + 10 * heightdiff);
			TrackHeight.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.Right - TrackHeight.ClientRectangle.Width, Metronome.ClientRectangle.Bottom + 50 * heightdiff - TrackHeight.ClientRectangle.Height);

			//patterns
			HFlip.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, PatternsNav.ClientRectangle.Bottom + 20 * heightdiff);
			VFlip.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, HFlip.ClientRectangle.Bottom + 10 * heightdiff);
			BezierStoreButton.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, VFlip.ClientRectangle.Bottom + 20 * heightdiff);
			BezierClearButton.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, BezierStoreButton.ClientRectangle.Bottom + 10 * heightdiff);
			DynamicBezier.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, BezierClearButton.ClientRectangle.Bottom + 10 * heightdiff);
			BezierBox.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, DynamicBezier.ClientRectangle.Bottom + 32 * heightdiff);
			BezierButton.ClientRectangle.Location = new PointF(BezierBox.ClientRectangle.Right + 5 * widthdiff, BezierBox.ClientRectangle.Y);
			RotateBox.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, BezierBox.ClientRectangle.Bottom + 35 * heightdiff);
			RotateButton.ClientRectangle.Location = new PointF(RotateBox.ClientRectangle.Right + 5 * widthdiff, RotateBox.ClientRectangle.Y);
			ScaleBox.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, RotateBox.ClientRectangle.Bottom + 35 * heightdiff);
			ScaleButton.ClientRectangle.Location = new PointF(RotateBox.ClientRectangle.Right + 5 * widthdiff, ScaleBox.ClientRectangle.Y);

			//etc
			BackButton.ClientRectangle.Location = new PointF(Grid.ClientRectangle.X, Grid.ClientRectangle.Bottom + 84 * heightdiff);
			CopyButton.ClientRectangle.Location = new PointF(Grid.ClientRectangle.X, Grid.ClientRectangle.Y - CopyButton.ClientRectangle.Height - 75 * heightdiff);

			SfxOffset.ClientRectangle.Location = new PointF(SfxVolume.ClientRectangle.Left - SfxOffset.ClientRectangle.Width - 10 * widthdiff, Tempo.ClientRectangle.Top - SfxOffset.ClientRectangle.Height - 15 * heightdiff);
			JumpMSButton.ClientRectangle.Location = new PointF(SfxOffset.ClientRectangle.X, SfxOffset.ClientRectangle.Top - JumpMSButton.ClientRectangle.Height - 30);
			JumpMSBox.ClientRectangle.Location = new PointF(SfxOffset.ClientRectangle.X, JumpMSButton.ClientRectangle.Top - JumpMSBox.ClientRectangle.Height - 10 * heightdiff);
			AutoAdvance.ClientRectangle.Location = new PointF(BeatSnapDivisor.ClientRectangle.X + 20 * widthdiff, CopyButton.ClientRectangle.Y + 35 * heightdiff);
			SelectBound.ClientRectangle.Location = new PointF(SfxOffset.ClientRectangle.X, JumpMSBox.ClientRectangle.Top - SelectBound.ClientRectangle.Height - 30);
			MSBoundHigher.ClientRectangle.Location = new PointF(SfxOffset.ClientRectangle.X, SelectBound.ClientRectangle.Top - MSBoundHigher.ClientRectangle.Height - 10 * heightdiff);
			MSBoundLower.ClientRectangle.Location = new PointF(SfxOffset.ClientRectangle.X, MSBoundHigher.ClientRectangle.Top - MSBoundLower.ClientRectangle.Height - 10 * heightdiff);


			HideShowElements();
		}

		public void OnMouseLeave()
		{
			Timeline.Dragging = false;
			Tempo.Dragging = false;
			MasterVolume.Dragging = false;
			SfxVolume.Dragging = false;
			NoteAlign.Dragging = false;
			BeatSnapDivisor.Dragging = false;
			TrackHeight.Dragging = false;
		}

		private void UpdateTrack()
		{
			/*
			if (Bpm.Focused)
			{
				var text = Bpm.Text;
				var decimalPont = false;

				if (text.Length > 0 && text[text.Length - 1].ToString() ==
				    CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
				{
					text = text + 0;

					decimalPont = true;
				}

				if (text.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                {
					decimalPont = true;
                }

				decimal.TryParse(text, out var bpm);

				if (bpm < 0)
					bpm = 0;
				else if (bpm > 5000)
					bpm = 5000;
				if (!decimalPont && bpm > 0)
					Bpm.Text = bpm.ToString();
			}
			*/
			if (Offset.Focused)
			{
				long.TryParse(Offset.Text, out var offset);

				offset = Math.Max(0, offset);

				if (offset > 0)
					Offset.Text = offset.ToString();
			}
			if (SfxOffset.Focused)
			{
				if (long.TryParse(SfxOffset.Text, out var sfxOffset))
					SfxOffset.Text = sfxOffset.ToString();
			}
			if (JumpMSBox.Focused)
            {
				if (long.TryParse(JumpMSBox.Text, out var jumpMS))
					if (jumpMS > EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds)
						jumpMS = (long)EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds;
				if (jumpMS.ToString() != "0")
					JumpMSBox.Text = jumpMS.ToString();
			}
			if (RotateBox.Focused)
            {
				if (float.TryParse(RotateBox.Text, out var degrees))
					RotateBox.Text = degrees.ToString();
            }
			if (BezierBox.Focused)
            {
				if (long.TryParse(BezierBox.Text, out var ints))
					BezierBox.Text = ints.ToString();
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