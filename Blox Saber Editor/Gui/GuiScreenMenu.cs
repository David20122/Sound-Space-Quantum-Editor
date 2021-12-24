using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;

namespace Sound_Space_Editor.Gui
{
	class GuiScreenMenu : GuiScreen
	{
		private readonly int _textureId;
		private bool bgImg = false;

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

		public GuiScreenMenu() : base(0, 0, EditorWindow.Instance.ClientSize.Width, EditorWindow.Instance.ClientSize.Height)
		{
			Buttons.Add(_createMapButton);
			Buttons.Add(_loadMapButton);
			Buttons.Add(_importButton);
			Buttons.Add(_SettingsButton);

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
				string ChangelogText = wc.DownloadString("https://gist.githubusercontent.com/David20122/899be190b55e63e5587922c428e2f6de/raw/ddaeb2ae5dbd5cb09e97b92f2c108b8dc15da927/changelog");
				Changelog = new GuiLabel(0, 0, ChangelogText, "main", 16);
			} catch {
				Changelog = new GuiLabel(0, 0, "Failed to load changelog", "main", 16);
			}

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


				GL.Color4(Color.FromArgb(120, 57, 56, 47));
				Glu.RenderQuad(size.Width / 2 - 1850 / 2, size.Height / 2 - 325, 950, 790);
				GL.Color4(Color.FromArgb(100, 36, 35, 33));
				Glu.RenderQuad(size.Width / 2 - 1810 / 2, size.Height / 2 - 270, 900, 715);

				CHANGELOGlabel.Render(delta, mouseX, mouseY);
				CHANGELOGlabelOutline.Render(delta, mouseX, mouseY);
				ssLabel.Render(delta, mouseX, mouseY);
				ssLabelOutline.Render(delta, mouseX, mouseY);
				qeLabel.Render(delta, mouseX, mouseY);

				Changelog.Render(delta, mouseX, mouseY);

			} else {

				// background
				GL.Color4(Color.FromArgb(255, 30, 30, 30));
				Glu.RenderQuad(0, 0, size.Width, size.Height);


				GL.Color4(Color.FromArgb(40, 0, 0, 0));
				Glu.RenderQuad(size.Width / 2 - 1850 / 2, size.Height / 2 - 325, 950, 790);
				GL.Color4(Color.FromArgb(50, 0, 0, 0));
				Glu.RenderQuad(size.Width / 2 - 1810 / 2, size.Height / 2 - 270, 900, 715);

				CHANGELOGlabel.Render(delta, mouseX, mouseY);
				ssLabel.Render(delta, mouseX, mouseY);
				qeLabel.Render(delta, mouseX, mouseY);

				Changelog.Render(delta, mouseX, mouseY);
			}

			base.Render(delta, mouseX, mouseY);
		}

		public override void OnResize(Size size)
		{
			ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);


			// labels
			CHANGELOGlabel.ClientRectangle.Location = new PointF(size.Width / 2 - 1615 / 2, size.Height / 2 - 295);
			CHANGELOGlabelOutline.ClientRectangle.Location = new PointF(size.Width / 2 - 1617 / 2, size.Height / 2 - 295);

			ssLabel.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 1860 / 2, ClientRectangle.Height / 2 - 470);
			ssLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 1862 / 2, ClientRectangle.Height / 2 - 470);

			qeLabel.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 700 / 2, ClientRectangle.Height / 2 - 360);
			qeLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 687 / 2, ClientRectangle.Height / 2 - 360);

			Changelog.ClientRectangle.Location = new PointF(size.Width / 2 - 1810 / 2, size.Height / 2 - 260);

			// buttons
			_createMapButton.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - -230, ClientRectangle.Height / 2 - 325);
			_loadMapButton.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - -230, ClientRectangle.Height / 2 - 210);
			_importButton.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - -230, ClientRectangle.Height / 2 - 95);
			_SettingsButton.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - -230, ClientRectangle.Height / 2 - -20);
			base.OnResize(size);
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
