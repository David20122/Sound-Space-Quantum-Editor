using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;

namespace Sound_Space_Editor.Gui
{
	class GuiScreenMenu : GuiScreen
	{
		private readonly int _textureId;
		private bool bgImg = false;
		private string ChangelogText;

		private readonly GuiLabel CHANGELOGlabel = new GuiLabel(0, 0, "CHANGELOG", "square", 40) { Centered = true };
		private readonly GuiLabel CHANGELOGlabelOutline = new GuiLabel(0, 0, "CHANGELOG", "squareo", 41) { Centered = true };

		private readonly GuiLabel ssLabel = new GuiLabel(0, 0, "SOUND SPACE", "square", 150);
		private readonly GuiLabel ssLabelOutline = new GuiLabel(0, 0, "SOUND SPACE", "squareo", 151);

		private readonly GuiLabel qeLabel = new GuiLabel(0, 0, "QUANTUM EDITOR", "square", 36);
		private readonly GuiLabel qeLabelOutline = new GuiLabel(0, 0, "QUANTUM EDITOR", "squareo", 36);

		private readonly GuiLabel Changelog;

		private GuiButton _createMapButton = new GuiButton(0, 0, 0, 600, 100, "CREATE NEW MAP", "square", 100);
		private GuiButton _loadMapButton = new GuiButton(1, 0, 0, 600, 100, "LOAD MAP", "square", 100);
		private GuiButton _importButton = new GuiButton(2, 0, 0, 600, 100, "IMPORT MAP", "square", 100);
        private GuiButton _SettingsButton = new GuiButton(3, 0, 0, 600, 100, "SETTINGS", "square", 100);

		public GuiSlider ScrollBar;

		public GuiScreenMenu() : base(0, 0, EditorWindow.Instance.ClientSize.Width, EditorWindow.Instance.ClientSize.Height)
		{

			if (File.Exists(Path.Combine(EditorWindow.Instance.LauncherDir, "background_menu.png")))
			{
				bgImg = true;
				using (Bitmap img = new Bitmap(Path.Combine(EditorWindow.Instance.LauncherDir, "background_menu.png")))
				{
					_textureId = TextureManager.GetOrRegister("menubg", img, true);
				}
			}

			try
            {
				SecureWebClient wc = new SecureWebClient();
				ChangelogText = wc.DownloadString("https://raw.githubusercontent.com/haawwkeye/Sound-Space-Quantum-Editor/master/changelog");
				Changelog = new GuiLabel(0, 0, ChangelogText, "main", 16);

				ScrollBar = new GuiSlider(0, 0, 20, 720)
				{
					MaxValue = ChangelogText.Split('\n').Length,
					Value = ChangelogText.Split('\n').Length,
				};

			} catch {
				Changelog = new GuiLabel(0, 0, "Failed to load changelog", "main", 16);
			}

			Buttons.Add(_createMapButton);
			Buttons.Add(_loadMapButton);
			Buttons.Add(_importButton);
			Buttons.Add(_SettingsButton);
			Buttons.Add(ScrollBar);

			CHANGELOGlabel.Color = Color.FromArgb(255, 255, 255);
			CHANGELOGlabelOutline.Color = Color.FromArgb(0, 0, 0);
			ssLabel.Color = Color.FromArgb(255, 255, 255);
			ssLabelOutline.Color = Color.FromArgb(0, 0, 0);
			qeLabel.Color = Color.FromArgb(255, 255, 255);
			qeLabelOutline.Color = Color.FromArgb(0, 0, 0);

			Changelog.Color = Color.FromArgb(255,255,255);

			OnResize(EditorWindow.Instance.ClientSize);
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			var size = EditorWindow.Instance.ClientSize;

			if (bgImg) {

				// background
				GL.Color4(Color.FromArgb(255, 255, 255, 255));
				Glu.RenderTexturedQuad(0, 0, size.Width, size.Height, 0, 0, 1, 1, _textureId);

				if (size.Width == 1920 && size.Height > 1000 && size.Height <= 1080)
				{
					GL.Color4(Color.FromArgb(120, 57, 56, 47));
					Glu.RenderQuad(ClientRectangle.Left + 35, ClientRectangle.Top + 180, 950, 790);
					GL.Color4(Color.FromArgb(100, 36, 35, 33));
					Glu.RenderQuad(ClientRectangle.Left + 55, ClientRectangle.Top + 230, 900, 715);
				}
				//else if (size.Width >= 1280 && size.Width <= 1700  && size.Height > 690 && size.Height <= 768 || size.Width == 1280 && size.Height > 640 && size.Height <= 720)
				else if (size.Width >= 10 && size.Width <= 1700 && size.Height > 600 && size.Height <= 1300)
				{
					GL.Color4(Color.FromArgb(120, 57, 56, 47));
					Glu.RenderQuad(ClientRectangle.Left + 35, ClientRectangle.Top + 180, 650, 525);
					GL.Color4(Color.FromArgb(100, 36, 35, 33));
					Glu.RenderQuad(ClientRectangle.Left + 55, ClientRectangle.Top + 230, 600, 450);
				}
				else
                {
					GL.Color4(Color.FromArgb(120, 57, 56, 47));
					Glu.RenderQuad(ClientRectangle.Left + 35, ClientRectangle.Top + 180, 950, 790);
					GL.Color4(Color.FromArgb(100, 36, 35, 33));
					Glu.RenderQuad(ClientRectangle.Left + 55, ClientRectangle.Top + 230, 900, 715);
				}

				CHANGELOGlabel.Render(delta, mouseX, mouseY);
				//CHANGELOGlabelOutline.Render(delta, mouseX, mouseY);
				ssLabel.Render(delta, mouseX, mouseY);
				//ssLabelOutline.Render(delta, mouseX, mouseY);
				qeLabel.Render(delta, mouseX, mouseY);

				Changelog.Render(delta, mouseX, mouseY);

				ScrollBar.Render(delta, mouseX, mouseY);

			} else {

				// background
				GL.Color4(Color.FromArgb(255, 30, 30, 30));
				Glu.RenderQuad(0, 0, size.Width, size.Height);


				if (size.Width == 1920 && size.Height > 1000 && size.Height <= 1080)
				{
					GL.Color4(Color.FromArgb(40, 0, 0, 0));
					Glu.RenderQuad(ClientRectangle.Left + 35, ClientRectangle.Top + 180, 950, 790);
					GL.Color4(Color.FromArgb(50, 0, 0, 0));
					Glu.RenderQuad(ClientRectangle.Left + 55, ClientRectangle.Top + 230, 900, 715);
				}
				else if (size.Width >= 10 && size.Width <= 1700 && size.Height >= 100 && size.Height <= 1300)
				{
					GL.Color4(Color.FromArgb(40, 0, 0, 0));
					Glu.RenderQuad(ClientRectangle.Left + 35, ClientRectangle.Top + 180, 650, 525);
					GL.Color4(Color.FromArgb(50, 0, 0, 0));
					Glu.RenderQuad(ClientRectangle.Left + 55, ClientRectangle.Top + 230, 600, 450);
				} 
				else
                {
					GL.Color4(Color.FromArgb(40, 0, 0, 0));
					Glu.RenderQuad(ClientRectangle.Left + 35, ClientRectangle.Top + 180, 950, 790);
					GL.Color4(Color.FromArgb(50, 0, 0, 0));
					Glu.RenderQuad(ClientRectangle.Left + 55, ClientRectangle.Top + 230, 900, 715);
				}


				CHANGELOGlabel.Render(delta, mouseX, mouseY);
				ssLabel.Render(delta, mouseX, mouseY);
				qeLabel.Render(delta, mouseX, mouseY);

				Changelog.Render(delta, mouseX, mouseY);
			}

			base.Render(delta, mouseX, mouseY);
		}

		public override void OnResize(Size size)
		{
			Console.WriteLine("Resolution: {0}, {1}", size.Width, size.Height);

			if (size.Width == 1920 && size.Height > 1000 && size.Height <= 1080)
            {
				var SSLabelFontSize = 150;
				var QELabelFontSize = 36;
				var ChangelogFontSize = 16;

				ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);

				CHANGELOGlabel.ClientRectangle.Location = new PointF(ClientRectangle.Left + 155, ClientRectangle.Top + 210);
				//CHANGELOGlabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 157, ClientRectangle.Top + 210);
				ssLabel.ClientRectangle.Location = new PointF(ClientRectangle.Left + 35, ClientRectangle.Top + 35);
				ssLabel.FontSize = SSLabelFontSize;
				//ssLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 37, ClientRectangle.Top + 35);
				qeLabel.ClientRectangle.Location = new PointF(ClientRectangle.Left + 615, ClientRectangle.Top + 140);
				qeLabel.FontSize = QELabelFontSize;
				//qeLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 617, ClientRectangle.Top + 140);
				Changelog.ClientRectangle.Location = new PointF(ClientRectangle.Left + 60, ClientRectangle.Top + 230);
				Changelog.FontSize = ChangelogFontSize;

				// buttons
				_createMapButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 730, ClientRectangle.Top + 180);
				_loadMapButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 730, ClientRectangle.Top + 295);
				_importButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 730, ClientRectangle.Top + 410);
				_SettingsButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 730, ClientRectangle.Top + 525);

				// resizing
				_createMapButton.ClientRectangle.Size = new SizeF(600, 100);
				_loadMapButton.ClientRectangle.Size = new SizeF(600, 100);
				_importButton.ClientRectangle.Size = new SizeF(600, 100);
				_SettingsButton.ClientRectangle.Size = new SizeF(600, 100);

				ScrollBar.ClientRectangle.Location = new PointF(ClientRectangle.Left + 950, ClientRectangle.Top + 230);
				ScrollBar.ClientRectangle.Size = new SizeF(20, 720);
				ScrollBar.MaxValue = ChangelogText.Split('\n').Length;

				AssembleChangelog();
			} 
			//else if (size.Width >= 1280 && size.Height > 690 && size.Height <= 768 || size.Width == 1280 && size.Height > 640 && size.Height <= 720)
			else if (size.Width >= 10 && size.Width <= 1700 && size.Height > 600 && size.Height <= 1300)
			{
				ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);
				var SSLabelFontSize = 110;
				var QELabelFontSize = 30;
				var ChangelogFontSize = 13;

				// labels
				CHANGELOGlabel.ClientRectangle.Location = new PointF(ClientRectangle.Left + 155, ClientRectangle.Top + 210);

				//CHANGELOGlabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 157, ClientRectangle.Top + 210);

				ssLabel.ClientRectangle.Location = new PointF(ClientRectangle.Left + 35, ClientRectangle.Top + 35);
				ssLabel.FontSize = SSLabelFontSize;

				ssLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 37, ClientRectangle.Top + 35);

				qeLabel.ClientRectangle.Location = new PointF(ClientRectangle.Left + 440, ClientRectangle.Top + 110);
				qeLabel.FontSize = QELabelFontSize;
				//qeLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 617, ClientRectangle.Top + 140);

				Changelog.ClientRectangle.Location = new PointF(ClientRectangle.Left + 60, ClientRectangle.Top + 230);
				Changelog.FontSize = ChangelogFontSize;

				// buttons
				_createMapButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 500, ClientRectangle.Top + 180);
				_loadMapButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 500, ClientRectangle.Top + 240);
				_importButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 500, ClientRectangle.Top + 300);
				_SettingsButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 500, ClientRectangle.Top + 360);

				// resizing
				_createMapButton.ClientRectangle.Size = new SizeF(400, 50);
				_loadMapButton.ClientRectangle.Size = new SizeF(400, 50);
				_importButton.ClientRectangle.Size = new SizeF(400, 50);
				_SettingsButton.ClientRectangle.Size = new SizeF(400, 50);

				ScrollBar.ClientRectangle.Location = new PointF(ClientRectangle.Left + 650, ClientRectangle.Top + 230);
				ScrollBar.ClientRectangle.Size = new SizeF(20, 460);
				ScrollBar.MaxValue = ChangelogText.Split('\n').Length;

				AssembleChangelog();
			} 
			else
            {
				var SSLabelFontSize = 150;
				var QELabelFontSize = 36;
				var ChangelogFontSize = 16;

				ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);

				CHANGELOGlabel.ClientRectangle.Location = new PointF(ClientRectangle.Left + 155, ClientRectangle.Top + 210);
				//CHANGELOGlabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 157, ClientRectangle.Top + 210);
				ssLabel.ClientRectangle.Location = new PointF(ClientRectangle.Left + 35, ClientRectangle.Top + 35);
				ssLabel.FontSize = SSLabelFontSize;
				//ssLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 37, ClientRectangle.Top + 35);
				qeLabel.ClientRectangle.Location = new PointF(ClientRectangle.Left + 615, ClientRectangle.Top + 140);
				qeLabel.FontSize = QELabelFontSize;
				//qeLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 617, ClientRectangle.Top + 140);
				Changelog.ClientRectangle.Location = new PointF(ClientRectangle.Left + 60, ClientRectangle.Top + 230);
				Changelog.FontSize = ChangelogFontSize;

				// buttons
				_createMapButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 730, ClientRectangle.Top + 180);
				_loadMapButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 730, ClientRectangle.Top + 295);
				_importButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 730, ClientRectangle.Top + 410);
				_SettingsButton.ClientRectangle.Location = new PointF(ClientRectangle.Right - 730, ClientRectangle.Top + 525);

				// resizing
				_createMapButton.ClientRectangle.Size = new SizeF(600, 100);
				_loadMapButton.ClientRectangle.Size = new SizeF(600, 100);
				_importButton.ClientRectangle.Size = new SizeF(600, 100);
				_SettingsButton.ClientRectangle.Size = new SizeF(600, 100);

				ScrollBar.ClientRectangle.Location = new PointF(ClientRectangle.Left + 950, ClientRectangle.Top + 230);
				ScrollBar.ClientRectangle.Size = new SizeF(20, 720);
				ScrollBar.MaxValue = ChangelogText.Split('\n').Length;
			}

			base.OnResize(size);
		}

		public void OnMouseLeave()
		{
			ScrollBar.Dragging = false;
		}

		public void AssembleChangelog()
        {
			string result = "";
			string[] lines = ChangelogText.Split('\n');
			for (int i = 0; i < lines.Length; i++)
            {
				if (i >= ScrollBar.MaxValue - ScrollBar.Value)
                {
					result += lines[i] + "\n";
                }
            }
			Changelog.Text = result;
        }

		protected override void OnButtonClicked(int id)
		{
			switch (id)
			{
				case 0:
					EditorWindow.Instance.OpenGuiScreen(new GuiScreenCreate());
					break;
				case 1:
					using (var dialog = new OpenFileDialog
					{
						Title = "Select Map File",
						Filter = "Text Documents (*.txt)|*.txt"
					})
					{
						if (dialog.ShowDialog() == DialogResult.OK)
						{
							EditorWindow.Instance.LoadFile(dialog.FileName);
						}
					}
					break;
				case 2:
					try
					{
						var clipboard = Clipboard.GetText();
						SecureWebClient wc = new SecureWebClient();
						if (clipboard.Contains("githubusercontent") || clipboard.Contains("raw") || clipboard.Contains("gist"))
						{
							try
							{
								var reply = wc.DownloadString(clipboard);
								EditorWindow.Instance.LoadMap(reply, false);
							}
							catch
							{
								MessageBox.Show("Error while loading map data from link.\nIs it valid?");
							}
						}
						else
						{
							try
							{
								EditorWindow.Instance.LoadMap(clipboard, false);
							}
							catch
							{
								MessageBox.Show("Error while loading map data.\nIs it valid?");
							}
						}
					}
					catch
					{

					}

					break;
				case 3:
					EditorWindow.Instance.OpenGuiScreen(new GuiScreenSettings());
					break;
			}
			base.OnButtonClicked(id);
		}
	}
}
