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
	public class GuiScreenSettings : GuiScreen
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

			WaveformCheckbox = new GuiCheckBox(1, "Waveform", 0, 0, 72, 72, 32, EditorSettings.Waveform);
			//BPMFormCheckbox = new GuiCheckBox(1, "Use Timings Form", 0, 0, 72, 72, 32, EditorSettings.BPMForm);

			Buttons.Add(_openFolderButton);
			Buttons.Add(_resetButton);
			Buttons.Add(_backButton);
			Buttons.Add(WaveformCheckbox);
			//Buttons.Add(BPMFormCheckbox);

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

			color1TextBox.Render(delta, mouseX, mouseY);
			color2TextBox.Render(delta, mouseX, mouseY);

			NoteColor1TextBox.Render(delta, mouseX, mouseY);
			NoteColor2TextBox.Render(delta, mouseX, mouseY);

			EditorBGOpacityTextBox.Render(delta, mouseX, mouseY);
			TrackOpacityTextBox.Render(delta, mouseX, mouseY);
			GridOpacityTextBox.Render(delta, mouseX, mouseY);

			ShowColor();
			ShowNoteColor();
			ShowOpacities();

			base.Render(delta, mouseX, mouseY);
		}

		public override void OnResize(Size size)
		{
			if (size.Width == 1920 && size.Height > 1000 && size.Height <= 1080)
			{
				ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);
				var middle = new PointF(size.Width / 2f, size.Height / 2f);

				_backButton.ClientRectangle.Location = new PointF(ClientRectangle.Left + 655, ClientRectangle.Bottom - 150);
				_resetButton.ClientRectangle.Location = new PointF(ClientRectangle.Left + 700, ClientRectangle.Bottom - 215);
				_openFolderButton.ClientRectangle.Location = new PointF(ClientRectangle.Left + 700, ClientRectangle.Bottom - 270);

				_backButton.ClientRectangle.Size = new SizeF(600, 100);
				_resetButton.ClientRectangle.Size = new SizeF(500, 50);
				_openFolderButton.ClientRectangle.Size = new SizeF(500, 50);

				color1TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 160, ClientRectangle.Top + 210);
				color2TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 160, ClientRectangle.Top + 360);

				NoteColor1TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 160, ClientRectangle.Top + 510);
				NoteColor2TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 160, ClientRectangle.Top + 660);

				WaveformCheckbox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 485, ClientRectangle.Top + 195);
				EditorBGOpacityTextBox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 485, ClientRectangle.Top + 360);

				GridOpacityTextBox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 485, ClientRectangle.Top + 510);
				TrackOpacityTextBox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 485, ClientRectangle.Top + 660);

				//BPMFormCheckbox.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 200, ClientRectangle.Height / 2 - 300);
			}
			//else if (size.Width == 1366 && size.Height > 690 && size.Height <= 768 || size.Width == 1280 && size.Height > 640 && size.Height <= 720)
			else if (size.Width >= 10 && size.Width <= 1700 && size.Height > 600 && size.Height <= 1300)
			{
				ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);
				var middle = new PointF(size.Width / 2f, size.Height / 2f);

				_backButton.ClientRectangle.Location = new PointF(ClientRectangle.Left + 450, ClientRectangle.Bottom - 100);
				_resetButton.ClientRectangle.Location = new PointF(ClientRectangle.Left + 480, ClientRectangle.Bottom - 150);
				_openFolderButton.ClientRectangle.Location = new PointF(ClientRectangle.Left + 480, ClientRectangle.Bottom - 190);

				_backButton.ClientRectangle.Size = new SizeF(350, 55);
				_resetButton.ClientRectangle.Size = new SizeF(300, 35);
				_openFolderButton.ClientRectangle.Size = new SizeF(300, 35);

				color1TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 100, ClientRectangle.Top + 150);
				color2TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 100, ClientRectangle.Top + 300);

				NoteColor1TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 100, ClientRectangle.Top + 450);
				NoteColor2TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 100, ClientRectangle.Top + 600);

				WaveformCheckbox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 425, ClientRectangle.Top + 135);
				EditorBGOpacityTextBox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 425, ClientRectangle.Top + 300);

				GridOpacityTextBox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 425, ClientRectangle.Top + 450);
				TrackOpacityTextBox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 425, ClientRectangle.Top + 600);

				//BPMFormCheckbox.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 200, ClientRectangle.Height / 2 - 300);
			} 
			else
            {
				ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);
				var middle = new PointF(size.Width / 2f, size.Height / 2f);

				_backButton.ClientRectangle.Location = new PointF(ClientRectangle.Left + 655, ClientRectangle.Bottom - 150);
				_resetButton.ClientRectangle.Location = new PointF(ClientRectangle.Left + 700, ClientRectangle.Bottom - 215);
				_openFolderButton.ClientRectangle.Location = new PointF(ClientRectangle.Left + 700, ClientRectangle.Bottom - 270);

				_backButton.ClientRectangle.Size = new SizeF(600, 100);
				_resetButton.ClientRectangle.Size = new SizeF(500, 50);
				_openFolderButton.ClientRectangle.Size = new SizeF(500, 50);

				color1TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 160, ClientRectangle.Top + 210);
				color2TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 160, ClientRectangle.Top + 360);

				NoteColor1TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 160, ClientRectangle.Top + 510);
				NoteColor2TextBox.ClientRectangle.Location = new PointF(ClientRectangle.Left + 160, ClientRectangle.Top + 660);

				WaveformCheckbox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 485, ClientRectangle.Top + 195);
				EditorBGOpacityTextBox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 485, ClientRectangle.Top + 360);

				GridOpacityTextBox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 485, ClientRectangle.Top + 510);
				TrackOpacityTextBox.ClientRectangle.Location = new PointF(ClientRectangle.Right - 485, ClientRectangle.Top + 660);

				//BPMFormCheckbox.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 200, ClientRectangle.Height / 2 - 300);
			}

			base.OnResize(size);
		}

		void ShowColor()
        {
			var fr = EditorWindow.Instance.FontRenderer;

			var size = EditorWindow.Instance.ClientSize;

			try
            {
				string c1 = color1TextBox.Text;
				string[] c1values = c1.Split(',');
				int[] Color1 = Array.ConvertAll<string, int>(c1values, int.Parse);

				GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
				Glu.RenderQuad(color1TextBox.ClientRectangle.X + 210, color1TextBox.ClientRectangle.Y - 15, 75, 75);
			}
			catch
            {
				GL.Color3(Color.FromArgb(255, 0, 0));
				fr.Render("invalid", (int)color1TextBox.ClientRectangle.X + 300, (int)color1TextBox.ClientRectangle.Y + 15, 20);
				GL.Color3(Color.FromArgb(0, 0, 0));
				Glu.RenderQuad(color1TextBox.ClientRectangle.X + 210, color1TextBox.ClientRectangle.Y - 15, 75, 75);
			}

			// color 2

            try
            {
				string c2 = color2TextBox.Text;
				string[] c2values = c2.Split(',');
				int[] Color2 = Array.ConvertAll<string, int>(c2values, int.Parse);

				GL.Color3(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
				Glu.RenderQuad(color2TextBox.ClientRectangle.X + 210, color2TextBox.ClientRectangle.Y - 15, 75, 75);
			}
            catch
            {
				GL.Color3(Color.FromArgb(255, 0, 0));
				fr.Render("invalid", (int)color2TextBox.ClientRectangle.X + 300, (int)color2TextBox.ClientRectangle.Y + 15, 20);
				GL.Color3(Color.FromArgb(0, 0, 0));
				Glu.RenderQuad(color2TextBox.ClientRectangle.X + 210, color2TextBox.ClientRectangle.Y - 15, 75, 75);
			}
		}

		void ShowNoteColor()
        {
			var fr = EditorWindow.Instance.FontRenderer;

			var size = EditorWindow.Instance.ClientSize;

			try
            {
				string c1 = NoteColor1TextBox.Text;
				string[] c1values = c1.Split(',');
				int[] Color1 = Array.ConvertAll<string, int>(c1values, int.Parse);

				GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
				Glu.RenderQuad(NoteColor1TextBox.ClientRectangle.X + 210, NoteColor1TextBox.ClientRectangle.Y - 15, 75, 75);
			} 
			catch
            {
				GL.Color3(Color.FromArgb(255, 0, 0));
				fr.Render("invalid", (int)NoteColor1TextBox.ClientRectangle.X + 300, (int)NoteColor1TextBox.ClientRectangle.Y + 15, 20);
				GL.Color3(Color.FromArgb(0, 0, 0));
				Glu.RenderQuad(NoteColor1TextBox.ClientRectangle.X + 210, NoteColor1TextBox.ClientRectangle.Y - 15, 75, 75);
			}

			// color 2

			try
            {
				string c2 = NoteColor2TextBox.Text;
				string[] c2values = c2.Split(',');
				int[] Color2 = Array.ConvertAll<string, int>(c2values, int.Parse);

				GL.Color3(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
				Glu.RenderQuad(NoteColor2TextBox.ClientRectangle.X + 210, NoteColor2TextBox.ClientRectangle.Y - 15, 75, 75);
			}
			catch
            {
				GL.Color3(Color.FromArgb(255, 0, 0));
				fr.Render("invalid", (int)NoteColor2TextBox.ClientRectangle.X + 300, (int)NoteColor2TextBox.ClientRectangle.Y + 15, 20);
				GL.Color3(Color.FromArgb(0, 0, 0));
				Glu.RenderQuad(NoteColor2TextBox.ClientRectangle.X + 210, NoteColor2TextBox.ClientRectangle.Y - 15, 75, 75);
			}
        }

		void ShowOpacities()
        {
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
				Glu.RenderQuad(EditorBGOpacityTextBox.ClientRectangle.X + 210, EditorBGOpacityTextBox.ClientRectangle.Y - 15, 75, 75);
			}
			catch
            {
				GL.Color4(Color.FromArgb(255, 255, 255, 255));
				Glu.RenderQuad(EditorBGOpacityTextBox.ClientRectangle.X + 210, EditorBGOpacityTextBox.ClientRectangle.Y - 15, 75, 75);
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
				Glu.RenderQuad(GridOpacityTextBox.ClientRectangle.X + 210, GridOpacityTextBox.ClientRectangle.Y - 15, 75, 75);
			}
			catch
            {
				GL.Color4(Color.FromArgb(255, 255, 255, 255));
				Glu.RenderQuad(GridOpacityTextBox.ClientRectangle.X + 210, GridOpacityTextBox.ClientRectangle.Y - 15, 75, 75);
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
				Glu.RenderQuad(TrackOpacityTextBox.ClientRectangle.X + 210, TrackOpacityTextBox.ClientRectangle.Y - 15, 75, 75);
			}
			catch
            {
				GL.Color4(Color.FromArgb(255, 255, 255, 255));
				Glu.RenderQuad(TrackOpacityTextBox.ClientRectangle.X + 210, TrackOpacityTextBox.ClientRectangle.Y - 15, 75, 75);
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

			base.OnMouseClick(x, y);
		}

		protected override void OnButtonClicked(int id)
		{
			switch (id)
			{
				case 0:
					int editorbgOpacity = int.Parse(EditorBGOpacityTextBox.Text);
					int gridOpacity = int.Parse(GridOpacityTextBox.Text);
					int trackOpacity = int.Parse(TrackOpacityTextBox.Text);
					EditorSettings.Color1 = color1TextBox.Text;
					EditorSettings.Color2 = color2TextBox.Text;
					EditorSettings.NoteColor1 = NoteColor1TextBox.Text;
					EditorSettings.NoteColor2 = NoteColor2TextBox.Text;
					EditorSettings.NoteColors = EditorSettings.NoteColors;
					EditorSettings.Waveform = WaveformCheckbox.Toggle;
					//EditorSettings.BPMForm = BPMFormCheckbox.Toggle;
					EditorSettings.EditorBGOpacity = editorbgOpacity;
					EditorSettings.GridOpacity = gridOpacity;
					EditorSettings.TrackOpacity = trackOpacity;
					EditorSettings.Save();
					EditorSettings.Load();
					EditorWindow.Instance.UpdateColors();
					EditorSettings.Save();
					EditorWindow.Instance.OpenGuiScreen(new GuiScreenMenu());
					break;
				case 1:
					EditorSettings.NoteColors = "255,0,255|0,255,200";
					EditorSettings.Waveform = true;
					//EditorSettings.BPMForm = false;
					EditorBGOpacityTextBox.Text = "255";
					GridOpacityTextBox.Text = "255";
					TrackOpacityTextBox.Text = "255";
					color1TextBox.Text = "0,255,200";
                    color2TextBox.Text = "255,0,255";
					NoteColor1TextBox.Text = "255,0,255";
					NoteColor2TextBox.Text = "0,255,200";
					break;
				case 2:
					System.Diagnostics.Process.Start(Environment.CurrentDirectory);
					break;
			}
			base.OnButtonClicked(id);
		}
	}
}
