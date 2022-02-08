using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;
using Sound_Space_Editor.Properties;
using System;
using OpenTK.Input;

namespace Sound_Space_Editor.Gui
{
	class GuiScreenSettings : GuiScreen
	{
		private readonly int _textureId;
		private bool bgImg = false;

		private GuiButton _backButton = new GuiButton(0, 0, 0, 600, 100, "SAVE AND RETURN", "square", 100);
		private GuiButton _resetButton = new GuiButton(1, 0, 0, 500, 50, "RESET TO DEFAULT", "square", 100);
		private GuiButton _keybindsButton = new GuiButton(8, 0, 0, 500, 50, "CHANGE KEYBINDS", "square", 100);
		private GuiButton _openFolderButton = new GuiButton(2, 0, 0, 500, 50, "OPEN EDITOR FOLDER", "square", 100);

		private GuiButton Color1Picker = new GuiButton(3, 0, 0, 200, 50, "PICK COLOR", "square", 100);
		private GuiButton Color2Picker = new GuiButton(4, 0, 0, 200, 50, "PICK COLOR", "square", 100);
		private GuiButton Color3Picker = new GuiButton(7, 0, 0, 200, 50, "PICK COLOR", "square", 100);
		private GuiButton NoteColor1Picker = new GuiButton(5, 0, 0, 200, 50, "PICK COLOR", "square", 100);
		private GuiButton NoteColor2Picker = new GuiButton(6, 0, 0, 200, 50, "PICK COLOR", "square", 100);

		private Color color1;
		private Color color2;
		private Color color3;
		private Color notecolor1;
		private Color notecolor2;

		private GuiCheckBox WaveformCheckbox;
		//private GuiCheckBox BPMFormCheckbox;

		private GuiTextBox EditorBGOpacityTextBox;
		private GuiTextBox GridOpacityTextBox;
		private GuiTextBox TrackOpacityTextBox;

		private GuiCheckBox AutosaveCheckbox;
		private GuiTextBox AutosaveInterval;

		private GuiCheckBox CorrectOnCopy;

		public GuiScreenSettings() : base(0, 0, EditorWindow.Instance.ClientSize.Width, EditorWindow.Instance.ClientSize.Height)
		{
			color1 = EditorSettings.Color1;
			color2 = EditorSettings.Color2;
			color3 = EditorSettings.Color3;
			notecolor1 = EditorSettings.NoteColor1;
			notecolor2 = EditorSettings.NoteColor2;

			EditorBGOpacityTextBox = new GuiTextBox(0, 0, 200, 50)
			{
				Text = EditorSettings.EditorBGOpacity.ToString(),
				Centered = true,
				Numeric = true,
				CanBeNegative = false
			};

			GridOpacityTextBox = new GuiTextBox(0, 0, 200, 50)
			{
				Text = EditorSettings.GridOpacity.ToString(),
				Centered = true,
				Numeric = true,
				CanBeNegative = false
			};

			TrackOpacityTextBox = new GuiTextBox(0, 0, 200, 50)
			{
				Text = EditorSettings.TrackOpacity.ToString(),
				Centered = true,
				Numeric = true,
				CanBeNegative = false
			};

			AutosaveInterval = new GuiTextBox(0, 0, 200, 50)
			{
				Text = EditorSettings.AutosaveInterval.ToString(),
				Centered = true,
				Numeric = true,
				CanBeNegative = false
			};

			WaveformCheckbox = new GuiCheckBox(1, "Waveform", 0, 0, 72, 72, 32, EditorSettings.Waveform);
			//BPMFormCheckbox = new GuiCheckBox(1, "Use Timings Form", 0, 0, 72, 72, 32, EditorSettings.BPMForm);
			AutosaveCheckbox = new GuiCheckBox(1, "Enable Autosave", 0, 0, 72, 72, 32, EditorSettings.EnableAutosave);

			CorrectOnCopy = new GuiCheckBox(1, "Correct Errors on Copy", 0, 0, 72, 72, 32, EditorSettings.CorrectOnCopy);

			Buttons.Add(_openFolderButton);
			Buttons.Add(_resetButton);
			Buttons.Add(_backButton);
			Buttons.Add(_keybindsButton);
			Buttons.Add(WaveformCheckbox);
			//Buttons.Add(BPMFormCheckbox);
			Buttons.Add(AutosaveCheckbox);
			Buttons.Add(CorrectOnCopy);
			Buttons.Add(Color1Picker);
			Buttons.Add(Color2Picker);
			Buttons.Add(Color3Picker);
			Buttons.Add(NoteColor1Picker);
			Buttons.Add(NoteColor2Picker);

			if (File.Exists(Path.Combine(EditorWindow.Instance.LauncherDir, "background_menu.png")))
			{
				bgImg = true;
				using (Bitmap img = new Bitmap(Path.Combine(EditorWindow.Instance.LauncherDir, "background_menu.png")))
				{
					_textureId = TextureManager.GetOrRegister("settingsbg", img, true);
				}
			}

			OnResize(EditorWindow.Instance.ClientSize);
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			var size = EditorWindow.Instance.ClientSize;

			if (bgImg)
			{
				GL.Color4(Color.FromArgb(255, 255, 255, 255));
				Glu.RenderTexturedQuad(0, 0, size.Width, size.Height, 0, 0, 1, 1, _textureId);

			}
			else
			{

				GL.Color4(Color.FromArgb(255, 30, 30, 30));
				Glu.RenderQuad(0, 0, size.Width, size.Height);
			}
			var fr = EditorWindow.Instance.FontRenderer;

			GL.Color4(Color.FromArgb(255, 255, 255, 255));

			if (EditorWindow.Instance.inconspicuousvar)
            {
				fr.Render("Cowow 1~", (int)Color1Picker.ClientRectangle.X, (int)Color1Picker.ClientRectangle.Y - 26, 24);
				fr.Render("Cowow 2~", (int)Color2Picker.ClientRectangle.X, (int)Color2Picker.ClientRectangle.Y - 26, 24);
				fr.Render("Cowow 3~", (int)Color3Picker.ClientRectangle.X, (int)Color3Picker.ClientRectangle.Y - 26, 24);

				fr.Render("Note Cowow 1~", (int)NoteColor1Picker.ClientRectangle.X, (int)NoteColor1Picker.ClientRectangle.Y - 26, 24);
				fr.Render("Note Cowow 2~", (int)NoteColor2Picker.ClientRectangle.X, (int)NoteColor2Picker.ClientRectangle.Y - 26, 24);

				fr.Render("Editow BG Opacity~", (int)EditorBGOpacityTextBox.ClientRectangle.X, (int)EditorBGOpacityTextBox.ClientRectangle.Y - 26, 24);
				fr.Render("Gwid Opacity~", (int)GridOpacityTextBox.ClientRectangle.X, (int)GridOpacityTextBox.ClientRectangle.Y - 26, 24);
				fr.Render("Twack Opacity~", (int)TrackOpacityTextBox.ClientRectangle.X, (int)TrackOpacityTextBox.ClientRectangle.Y - 26, 24);

				fr.Render("Autosave Intewvaw (min)~", (int)AutosaveInterval.ClientRectangle.X, (int)AutosaveInterval.ClientRectangle.Y - 26, 24);
			}
			else
            {
				fr.Render("Color 1:", (int)Color1Picker.ClientRectangle.X, (int)Color1Picker.ClientRectangle.Y - 26, 24);
				fr.Render("Color 2:", (int)Color2Picker.ClientRectangle.X, (int)Color2Picker.ClientRectangle.Y - 26, 24);
				fr.Render("Color 3:", (int)Color3Picker.ClientRectangle.X, (int)Color3Picker.ClientRectangle.Y - 26, 24);

				fr.Render("Note Color 1:", (int)NoteColor1Picker.ClientRectangle.X, (int)NoteColor1Picker.ClientRectangle.Y - 26, 24);
				fr.Render("Note Color 2:", (int)NoteColor2Picker.ClientRectangle.X, (int)NoteColor2Picker.ClientRectangle.Y - 26, 24);

				fr.Render("Editor BG Opacity:", (int)EditorBGOpacityTextBox.ClientRectangle.X, (int)EditorBGOpacityTextBox.ClientRectangle.Y - 26, 24);
				fr.Render("Grid Opacity:", (int)GridOpacityTextBox.ClientRectangle.X, (int)GridOpacityTextBox.ClientRectangle.Y - 26, 24);
				fr.Render("Track Opacity:", (int)TrackOpacityTextBox.ClientRectangle.X, (int)TrackOpacityTextBox.ClientRectangle.Y - 26, 24);

				fr.Render("Autosave Interval (min):", (int)AutosaveInterval.ClientRectangle.X, (int)AutosaveInterval.ClientRectangle.Y - 26, 24);
			}

			EditorBGOpacityTextBox.Render(delta, mouseX, mouseY);
			TrackOpacityTextBox.Render(delta, mouseX, mouseY);
			GridOpacityTextBox.Render(delta, mouseX, mouseY);

			AutosaveInterval.Render(delta, mouseX, mouseY);

			ShowColor();
			ShowNoteColor();
			ShowOpacities();

			base.Render(delta, mouseX, mouseY);
		}

		public override void OnResize(Size size)
		{
			ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);
			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;

			_backButton.ClientRectangle.Location = new PointF(655 * widthdiff, 930 * heightdiff);
			_resetButton.ClientRectangle.Location = new PointF(700 * widthdiff, 865 * heightdiff);
			_keybindsButton.ClientRectangle.Location = new PointF(700 * widthdiff, 755 * heightdiff);
			_openFolderButton.ClientRectangle.Location = new PointF(700 * widthdiff, 810 * heightdiff);

			_backButton.ClientRectangle.Size = new SizeF(600 * widthdiff, 100 * heightdiff);
			_resetButton.ClientRectangle.Size = new SizeF(500 * widthdiff, 50 * heightdiff);
			_keybindsButton.ClientRectangle.Size = new SizeF(500 * widthdiff, 50 * heightdiff);
			_openFolderButton.ClientRectangle.Size = new SizeF(500 * widthdiff, 50 * heightdiff);

			Color1Picker.ClientRectangle.Location = new PointF(160 * widthdiff, 210 * heightdiff);
			Color2Picker.ClientRectangle.Location = new PointF(160 * widthdiff, 360 * heightdiff);
			Color3Picker.ClientRectangle.Location = new PointF(160 * widthdiff, 510 * heightdiff);

			Color1Picker.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);
			Color2Picker.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);
			Color3Picker.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);

			NoteColor1Picker.ClientRectangle.Location = new PointF(160 * widthdiff, 660 * heightdiff);
			NoteColor2Picker.ClientRectangle.Location = new PointF(160 * widthdiff, 810 * heightdiff);

			NoteColor1Picker.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);
			NoteColor2Picker.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);

			WaveformCheckbox.ClientRectangle.Location = new PointF(1435 * widthdiff, 195 * heightdiff);
			EditorBGOpacityTextBox.ClientRectangle.Location = new PointF(1435 * widthdiff, 360 * heightdiff);

			WaveformCheckbox.ClientRectangle.Size = new SizeF(72 * widthdiff, 72 * heightdiff);
			EditorBGOpacityTextBox.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);

			GridOpacityTextBox.ClientRectangle.Location = new PointF(1435 * widthdiff, 510 * heightdiff);
			TrackOpacityTextBox.ClientRectangle.Location = new PointF(1435 * widthdiff, 660 * heightdiff);

			GridOpacityTextBox.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);
			TrackOpacityTextBox.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);

			AutosaveInterval.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);
			AutosaveCheckbox.ClientRectangle.Size = new SizeF(72 * widthdiff, 72 * heightdiff);

			AutosaveInterval.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - AutosaveInterval.ClientRectangle.Width / 2, 360 * heightdiff);
			AutosaveCheckbox.ClientRectangle.Location = new PointF(AutosaveInterval.ClientRectangle.X, 195 * heightdiff);

			CorrectOnCopy.ClientRectangle.Size = new SizeF(72 * widthdiff, 72 * heightdiff);

			CorrectOnCopy.ClientRectangle.Location = new PointF(AutosaveInterval.ClientRectangle.X, 510 * heightdiff);

			//BPMFormCheckbox.ClientRectangle.Location = new PointF(760 * widthdiff, 240 * heightdiff);

			base.OnResize(size);
		}

		void ShowColor()
		{
			var fr = EditorWindow.Instance.FontRenderer;

			var size = EditorWindow.Instance.ClientSize;

			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;

			GL.Color3(color1);
			Glu.RenderQuad(Color1Picker.ClientRectangle.X + 210 * widthdiff, Color1Picker.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);

			GL.Color3(color2);
			Glu.RenderQuad(Color2Picker.ClientRectangle.X + 210 * widthdiff, Color2Picker.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);

			GL.Color3(color3);
			Glu.RenderQuad(Color3Picker.ClientRectangle.X + 210 * widthdiff, Color3Picker.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
		}

		void ShowNoteColor()
		{
			var fr = EditorWindow.Instance.FontRenderer;

			var size = EditorWindow.Instance.ClientSize;

			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;

			GL.Color3(notecolor1);
			Glu.RenderQuad(NoteColor1Picker.ClientRectangle.X + 210 * widthdiff, NoteColor1Picker.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);

			GL.Color3(notecolor2);
			Glu.RenderQuad(NoteColor2Picker.ClientRectangle.X + 210 * widthdiff, NoteColor2Picker.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
		}

		void ShowOpacities()
		{
			var size = EditorWindow.Instance.ClientSize;

			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;

			try
			{
				int res;
				string editorBGOpacityText = EditorBGOpacityTextBox.Text;
				var editorBGOpacity = int.TryParse(editorBGOpacityText, out res);

				if (res > 255)
				{
					EditorBGOpacityTextBox.Text = "255";
					string changedText = EditorBGOpacityTextBox.Text;
					var editorBGOpacityNew = int.TryParse(changedText, out res);
				}
				else if (res < 0)
				{
					EditorBGOpacityTextBox.Text = "0";
					string changedText = EditorBGOpacityTextBox.Text;
					var editorBGOpacityNew = int.TryParse(changedText, out res);
				}

				GL.Color4(Color.FromArgb(res, 255, 255, 255));
				Glu.RenderQuad(EditorBGOpacityTextBox.ClientRectangle.X + 210 * widthdiff, EditorBGOpacityTextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}
			catch
			{
				GL.Color4(Color.FromArgb(255, 255, 255, 255));
				Glu.RenderQuad(EditorBGOpacityTextBox.ClientRectangle.X + 210 * widthdiff, EditorBGOpacityTextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}

			try
			{
				int res;
				string gridOpacityText = GridOpacityTextBox.Text;
				var gridOpacity = int.TryParse(gridOpacityText, out res);

				if (res > 255)
				{
					GridOpacityTextBox.Text = "255";
					string changedText = GridOpacityTextBox.Text;
					var GridOpacityNew = int.TryParse(changedText, out res);
				}
				else if (res < 0)
				{
					GridOpacityTextBox.Text = "0";
					string changedText = GridOpacityTextBox.Text;
					var GridOpacityNew = int.TryParse(changedText, out res);
				}

				GL.Color4(Color.FromArgb(res, 255, 255, 255));
				Glu.RenderQuad(GridOpacityTextBox.ClientRectangle.X + 210 * widthdiff, GridOpacityTextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}
			catch
			{
				GL.Color4(Color.FromArgb(255, 255, 255, 255));
				Glu.RenderQuad(GridOpacityTextBox.ClientRectangle.X + 210 * widthdiff, GridOpacityTextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}

			try
			{
				int res;
				string trackOpacityText = TrackOpacityTextBox.Text;
				var trackOpacity = int.TryParse(trackOpacityText, out res);

				if (res > 255)
				{
					TrackOpacityTextBox.Text = "255";
					string changedText = TrackOpacityTextBox.Text;
					var TrackOpacityNew = int.TryParse(changedText, out res);
				}
				else if (res < 0)
				{
					TrackOpacityTextBox.Text = "0";
					string changedText = TrackOpacityTextBox.Text;
					var TrackOpacityNew = int.TryParse(changedText, out res);
				}

				GL.Color4(Color.FromArgb(res, 255, 255, 255));
				Glu.RenderQuad(TrackOpacityTextBox.ClientRectangle.X + 210 * widthdiff, TrackOpacityTextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}
			catch
			{
				GL.Color4(Color.FromArgb(255, 255, 255, 255));
				Glu.RenderQuad(TrackOpacityTextBox.ClientRectangle.X + 210 * widthdiff, TrackOpacityTextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}
		}

		public override void OnKeyTyped(char key)
		{
			EditorBGOpacityTextBox.OnKeyTyped(key);
			GridOpacityTextBox.OnKeyTyped(key);
			TrackOpacityTextBox.OnKeyTyped(key);

			AutosaveInterval.OnKeyTyped(key);

			ShowColor();
			ShowNoteColor();
			ShowOpacities();
		}

		public override void OnKeyDown(Key key, bool control)
		{
			EditorBGOpacityTextBox.OnKeyDown(key, control);
			GridOpacityTextBox.OnKeyDown(key, control);
			TrackOpacityTextBox.OnKeyDown(key, control);

			AutosaveInterval.OnKeyDown(key, control);

			ShowColor();
			ShowNoteColor();
			ShowOpacities();
		}

		public override void OnMouseClick(float x, float y)
		{
			EditorBGOpacityTextBox.OnMouseClick(x, y);
			GridOpacityTextBox.OnMouseClick(x, y);
			TrackOpacityTextBox.OnMouseClick(x, y);

			AutosaveInterval.OnMouseClick(x, y);

			base.OnMouseClick(x, y);
		}

		protected override void OnButtonClicked(int id)
		{
			switch (id)
			{
				case 0:
					int editorbgOpacity = int.TryParse(EditorBGOpacityTextBox.Text, out var val1) ? val1 : EditorSettings.EditorBGOpacity;
					int gridOpacity = int.TryParse(GridOpacityTextBox.Text, out var val2) ? val2 : EditorSettings.GridOpacity;
					int trackOpacity = int.TryParse(TrackOpacityTextBox.Text, out var val3) ? val3 : EditorSettings.TrackOpacity;
					int autosaveInterval = int.TryParse(AutosaveInterval.Text, out var val4) && val4 > 0 ? val4 : EditorSettings.AutosaveInterval;
					EditorSettings.Color1 = color1;
					EditorSettings.Color2 = color2;
					EditorSettings.Color3 = color3;
					EditorSettings.NoteColor1 = notecolor1;
					EditorSettings.NoteColor2 = notecolor2;
					EditorSettings.Waveform = WaveformCheckbox.Toggle;
					//EditorSettings.BPMForm = BPMFormCheckbox.Toggle;
					EditorSettings.EnableAutosave = AutosaveCheckbox.Toggle;
					EditorSettings.EditorBGOpacity = editorbgOpacity;
					EditorSettings.GridOpacity = gridOpacity;
					EditorSettings.TrackOpacity = trackOpacity;
					EditorSettings.AutosaveInterval = autosaveInterval;
					EditorSettings.CorrectOnCopy = CorrectOnCopy.Toggle;
					EditorSettings.SaveSettings();
					EditorSettings.Load();
					EditorWindow.Instance.UpdateColors();
					EditorSettings.SaveSettings();
					EditorWindow.Instance.OpenGuiScreen(new GuiScreenMenu());
					break;
				case 1:
					color1 = Color.FromArgb(0, 255, 200);
					color2 = Color.FromArgb(255, 0, 255);
					color3 = Color.FromArgb(255, 0, 100);
					notecolor1 = Color.FromArgb(255, 0, 255);
					notecolor2 = Color.FromArgb(0, 255, 200);
					EditorSettings.Waveform = true;
					EditorSettings.EnableAutosave = true;
					//EditorSettings.BPMForm = false;
					EditorBGOpacityTextBox.Text = "255";
					GridOpacityTextBox.Text = "255";
					TrackOpacityTextBox.Text = "255";
					AutosaveInterval.Text = "5";
					EditorSettings.CorrectOnCopy = true;
					break;
				case 2:
					System.Diagnostics.Process.Start(Environment.CurrentDirectory);
					break;
				case 3:
					var ColorDialog1 = new ColorDialog();
					ColorDialog1.Color = color1;

					if (ColorDialog1.ShowDialog() == DialogResult.OK)
						color1 = ColorDialog1.Color;

					break;
				case 4:
					var ColorDialog2 = new ColorDialog();
					ColorDialog2.Color = color2;

					if (ColorDialog2.ShowDialog() == DialogResult.OK)
						color2 = ColorDialog2.Color;
					
					break;
				case 5:
					var ColorDialog3 = new ColorDialog();
					ColorDialog3.Color = notecolor1;

					if (ColorDialog3.ShowDialog() == DialogResult.OK)
						notecolor1 = ColorDialog3.Color;
					
					break;
				case 6:
					var ColorDialog4 = new ColorDialog();
					ColorDialog4.Color = notecolor2;

					if (ColorDialog4.ShowDialog() == DialogResult.OK)
						notecolor2 = ColorDialog4.Color;
					
					break;
				case 7:
					var ColorDialog5 = new ColorDialog();
					ColorDialog5.Color = color3;

					if (ColorDialog5.ShowDialog() == DialogResult.OK)
						color3 = ColorDialog5.Color;
                    
					break;
				case 8:
					EditorWindow.Instance.OpenGuiScreen(new GuiScreenKeybinds());
					break;
			}
			base.OnButtonClicked(id);
		}
	}
}