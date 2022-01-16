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
		private GuiButton _openFolderButton = new GuiButton(2, 0, 0, 500, 50, "OPEN EDITOR FOLDER", "square", 100);

		private GuiTextBox color1TextBox;
		private GuiTextBox color2TextBox;
		private GuiTextBox NoteColor1TextBox;
		private GuiTextBox NoteColor2TextBox;

		private GuiCheckBox WaveformCheckbox;
		//private GuiCheckBox BPMFormCheckbox;

		private GuiTextBox EditorBGOpacityTextBox;
		private GuiTextBox GridOpacityTextBox;
		private GuiTextBox TrackOpacityTextBox;

		private GuiCheckBox AutosaveCheckbox;
		private GuiTextBox AutosaveInterval;

		public GuiScreenSettings() : base(0, 0, EditorWindow.Instance.ClientSize.Width, EditorWindow.Instance.ClientSize.Height)
		{
			color1TextBox = new GuiTextBox(0, 0, 200, 50)
			{
				Text = EditorSettings.Color1,
				Centered = true,
				Numeric = true,
				CanBeNegative = false
			};

			color2TextBox = new GuiTextBox(0, 0, 200, 50)
			{
				Text = EditorSettings.Color2,
				Centered = true,
				Numeric = true,
				CanBeNegative = false
			};

			NoteColor1TextBox = new GuiTextBox(0, 0, 200, 50)
			{
				Text = EditorSettings.NoteColor1,
				Centered = true,
				Numeric = true,
				CanBeNegative = false
			};

			NoteColor2TextBox = new GuiTextBox(0, 0, 200, 50)
			{
				Text = EditorSettings.NoteColor2,
				Centered = true,
				Numeric = true,
				CanBeNegative = false
			};

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
			AutosaveCheckbox = new GuiCheckBox(3, "Enable Autosave", 0, 0, 72, 72, 32, EditorSettings.EnableAutosave);

			Buttons.Add(_openFolderButton);
			Buttons.Add(_resetButton);
			Buttons.Add(_backButton);
			Buttons.Add(WaveformCheckbox);
			//Buttons.Add(BPMFormCheckbox);
			Buttons.Add(AutosaveCheckbox);

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

			} else {

				GL.Color4(Color.FromArgb(255, 30, 30, 30));
				Glu.RenderQuad(0, 0, size.Width, size.Height);
			}
			var fr = EditorWindow.Instance.FontRenderer;

			GL.Color4(Color.FromArgb(255, 255, 255, 255));
			fr.Render("Color 1:", (int)color1TextBox.ClientRectangle.X, (int)color1TextBox.ClientRectangle.Y - 26, 24);
			fr.Render("Color 2:", (int)color2TextBox.ClientRectangle.X, (int)color2TextBox.ClientRectangle.Y - 26, 24);

			fr.Render("Note Color 1:", (int)NoteColor1TextBox.ClientRectangle.X, (int)NoteColor1TextBox.ClientRectangle.Y - 26, 24);
			fr.Render("Note Color 2:", (int)NoteColor2TextBox.ClientRectangle.X, (int)NoteColor2TextBox.ClientRectangle.Y - 26, 24);

			fr.Render("Editor BG Opacity:", (int)EditorBGOpacityTextBox.ClientRectangle.X, (int)EditorBGOpacityTextBox.ClientRectangle.Y - 26, 24);
			fr.Render("Grid Opacity:", (int)GridOpacityTextBox.ClientRectangle.X, (int)GridOpacityTextBox.ClientRectangle.Y - 26, 24);
			fr.Render("Track Opacity:", (int)TrackOpacityTextBox.ClientRectangle.X, (int)TrackOpacityTextBox.ClientRectangle.Y - 26, 24);

			fr.Render("Autosave Interval (min):", (int)AutosaveInterval.ClientRectangle.X, (int)AutosaveInterval.ClientRectangle.Y - 26, 24);

			color1TextBox.Render(delta, mouseX, mouseY);
			color2TextBox.Render(delta, mouseX, mouseY);

			NoteColor1TextBox.Render(delta, mouseX, mouseY);
			NoteColor2TextBox.Render(delta, mouseX, mouseY);

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
			_openFolderButton.ClientRectangle.Location = new PointF(700 * widthdiff, 810 * heightdiff);

			_backButton.ClientRectangle.Size = new SizeF(600 * widthdiff, 100 * heightdiff);
			_resetButton.ClientRectangle.Size = new SizeF(500 * widthdiff, 50 * heightdiff);
			_openFolderButton.ClientRectangle.Size = new SizeF(500 * widthdiff, 50 * heightdiff);

			color1TextBox.ClientRectangle.Location = new PointF(160 * widthdiff, 210 * heightdiff);
			color2TextBox.ClientRectangle.Location = new PointF(160 * widthdiff, 360 * heightdiff);

			color1TextBox.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);
			color2TextBox.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);

			NoteColor1TextBox.ClientRectangle.Location = new PointF(160 * widthdiff, 510 * heightdiff);
			NoteColor2TextBox.ClientRectangle.Location = new PointF(160 * widthdiff, 660 * heightdiff);

			NoteColor1TextBox.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);
			NoteColor2TextBox.ClientRectangle.Size = new SizeF(200 * widthdiff, 50 * heightdiff);

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

			//BPMFormCheckbox.ClientRectangle.Location = new PointF(760 * widthdiff, 240 * heightdiff);

			base.OnResize(size);
		}

		void ShowColor()
        {
			var fr = EditorWindow.Instance.FontRenderer;

			var size = EditorWindow.Instance.ClientSize;

			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;

			try
            {
				string c1 = color1TextBox.Text;
				string[] c1values = c1.Split(',');
				int[] Color1 = Array.ConvertAll<string, int>(c1values, int.Parse);

				GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
				Glu.RenderQuad(color1TextBox.ClientRectangle.X + 210 * widthdiff, color1TextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}
			catch
            {
				GL.Color3(Color.FromArgb(255, 0, 0));
				fr.Render("invalid", (int)(color1TextBox.ClientRectangle.X + 300 * widthdiff), (int)(color1TextBox.ClientRectangle.Y + 15 * heightdiff), 20);
				GL.Color3(Color.FromArgb(0, 0, 0));
				Glu.RenderQuad(color1TextBox.ClientRectangle.X + 210 * widthdiff, color1TextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}

			// color 2

            try
            {
				string c2 = color2TextBox.Text;
				string[] c2values = c2.Split(',');
				int[] Color2 = Array.ConvertAll<string, int>(c2values, int.Parse);

				GL.Color3(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
				Glu.RenderQuad(color2TextBox.ClientRectangle.X + 210 * widthdiff, color2TextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}
            catch
            {
				GL.Color3(Color.FromArgb(255, 0, 0));
				fr.Render("invalid", (int)(color2TextBox.ClientRectangle.X + 300 * widthdiff), (int)(color2TextBox.ClientRectangle.Y + 15 * heightdiff), 20);
				GL.Color3(Color.FromArgb(0, 0, 0));
				Glu.RenderQuad(color2TextBox.ClientRectangle.X + 210 * widthdiff, color2TextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}
		}

		void ShowNoteColor()
        {
			var fr = EditorWindow.Instance.FontRenderer;

			var size = EditorWindow.Instance.ClientSize;

			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;

			try
            {
				string c1 = NoteColor1TextBox.Text;
				string[] c1values = c1.Split(',');
				int[] Color1 = Array.ConvertAll<string, int>(c1values, int.Parse);

				GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
				Glu.RenderQuad(NoteColor1TextBox.ClientRectangle.X + 210 * widthdiff, NoteColor1TextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			} 
			catch
            {
				GL.Color3(Color.FromArgb(255, 0, 0));
				fr.Render("invalid", (int)(NoteColor1TextBox.ClientRectangle.X + 300 * widthdiff), (int)(NoteColor1TextBox.ClientRectangle.Y + 15 * heightdiff), 20);
				GL.Color3(Color.FromArgb(0, 0, 0));
				Glu.RenderQuad(NoteColor1TextBox.ClientRectangle.X + 210 * widthdiff, NoteColor1TextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}

			// color 2

			try
            {
				string c2 = NoteColor2TextBox.Text;
				string[] c2values = c2.Split(',');
				int[] Color2 = Array.ConvertAll<string, int>(c2values, int.Parse);

				GL.Color3(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
				Glu.RenderQuad(NoteColor2TextBox.ClientRectangle.X + 210 * widthdiff, NoteColor2TextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}
			catch
            {
				GL.Color3(Color.FromArgb(255, 0, 0));
				fr.Render("invalid", (int)(NoteColor2TextBox.ClientRectangle.X + 300 * widthdiff), (int)(NoteColor2TextBox.ClientRectangle.Y + 15 * heightdiff), 20);
				GL.Color3(Color.FromArgb(0, 0, 0));
				Glu.RenderQuad(NoteColor2TextBox.ClientRectangle.X + 210 * widthdiff, NoteColor2TextBox.ClientRectangle.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
			}
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
			color1TextBox.OnKeyTyped(key);
			color2TextBox.OnKeyTyped(key);
			NoteColor1TextBox.OnKeyTyped(key);
			NoteColor2TextBox.OnKeyTyped(key);

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
			color1TextBox.OnKeyDown(key, control);
			color2TextBox.OnKeyDown(key, control);
			NoteColor1TextBox.OnKeyDown(key, control);
			NoteColor2TextBox.OnKeyDown(key, control);

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
			color1TextBox.OnMouseClick(x, y); 
			color2TextBox.OnMouseClick(x, y);
			NoteColor1TextBox.OnMouseClick(x, y);
			NoteColor2TextBox.OnMouseClick(x, y);

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
					int autosaveInterval = int.TryParse(AutosaveInterval.Text, out var val4) && val4 > 0? val4 : EditorSettings.AutosaveInterval;
					EditorSettings.Color1 = color1TextBox.Text;
					EditorSettings.Color2 = color2TextBox.Text;
					EditorSettings.NoteColor1 = NoteColor1TextBox.Text;
					EditorSettings.NoteColor2 = NoteColor2TextBox.Text;
					EditorSettings.Waveform = WaveformCheckbox.Toggle;
					//EditorSettings.BPMForm = BPMFormCheckbox.Toggle;
					EditorSettings.EnableAutosave = AutosaveCheckbox.Toggle;
					EditorSettings.EditorBGOpacity = editorbgOpacity;
					EditorSettings.GridOpacity = gridOpacity;
					EditorSettings.TrackOpacity = trackOpacity;
					EditorSettings.AutosaveInterval = autosaveInterval;
					EditorSettings.Save();
					EditorSettings.Load();
					EditorWindow.Instance.UpdateColors();
					EditorSettings.Save();
					EditorWindow.Instance.OpenGuiScreen(new GuiScreenMenu());
					break;
				case 1:
					EditorSettings.Waveform = true;
					//EditorSettings.BPMForm = false;
					EditorBGOpacityTextBox.Text = "255";
					GridOpacityTextBox.Text = "255";
					TrackOpacityTextBox.Text = "255";
					color1TextBox.Text = "0,255,200";
                    color2TextBox.Text = "255,0,255";
					NoteColor1TextBox.Text = "255,0,255";
					NoteColor2TextBox.Text = "0,255,200";
					AutosaveInterval.Text = "5";
					break;
				case 2:
					System.Diagnostics.Process.Start(Environment.CurrentDirectory);
					break;
				case 3:

					break;
			}
			base.OnButtonClicked(id);
		}
	}
}
