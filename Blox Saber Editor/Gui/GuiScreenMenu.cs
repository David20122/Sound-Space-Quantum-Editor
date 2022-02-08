using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;
using Sound_Space_Editor.Properties;

namespace Sound_Space_Editor.Gui
{
	class GuiScreenMenu : GuiScreen
	{
		private readonly int _textureId;
		private bool bgImg = false;
		private string ChangelogText;

		private readonly GuiLabel CHANGELOGlabel = new GuiLabel(0, 0, "CHANGELOG", "square", 40) { Centered = false };
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

		private GuiButton _AutosavedButton = new GuiButton(4, 0, 0, 0, 0, "AUTOSAVED MAP", "square", 100);
		private GuiButton _LastButton = new GuiButton(5, 0, 0, 0, 0, "EDIT LAST MAP", "square", 100);

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

			ScrollBar = new GuiSlider(0, 0, 20, 720)
			{
				MaxValue = 0,
				Value = 0,
			};

			try
            {
				SecureWebClient wc = new SecureWebClient();
				ChangelogText = wc.DownloadString("https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/master/changelog");
				Changelog = new GuiLabel(0, 0, ChangelogText, "main", 16);

				ScrollBar.MaxValue = ChangelogText.Split('\n').Length;
				ScrollBar.Value = ChangelogText.Split('\n').Length;

			} catch {
				Changelog = new GuiLabel(0, 0, "Failed to load changelog", "main", 16);
			}

			Buttons.Add(_createMapButton);
			Buttons.Add(_loadMapButton);
			Buttons.Add(_importButton);
			Buttons.Add(_SettingsButton);
			Buttons.Add(_AutosavedButton);
			Buttons.Add(_LastButton);
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

			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;

			if (bgImg)
            {
				GL.Color4(Color.FromArgb(255, 255, 255, 255));
				Glu.RenderTexturedQuad(0, 0, size.Width, size.Height, 0, 0, 1, 1, _textureId);

				GL.Color4(Color.FromArgb(120, 57, 56, 47));
				Glu.RenderQuad(35 * widthdiff, 180 * heightdiff, 950 * widthdiff, 790 * heightdiff);
				GL.Color4(Color.FromArgb(100, 36, 35, 33));
				Glu.RenderQuad(55 * widthdiff, 230 * heightdiff, 900 * widthdiff, 715 * heightdiff);
			}
			else
            {
				GL.Color4(Color.FromArgb(255, 30, 30, 30));
				Glu.RenderQuad(0, 0, size.Width, size.Height);

				GL.Color4(Color.FromArgb(40, 0, 0, 0));
				Glu.RenderQuad(35 * widthdiff, 180 * heightdiff, 950 * widthdiff, 790 * heightdiff);
				GL.Color4(Color.FromArgb(50, 0, 0, 0));
				Glu.RenderQuad(55 * widthdiff, 230 * heightdiff, 900 * widthdiff, 715 * heightdiff);
			}

			CHANGELOGlabel.Render(delta, mouseX, mouseY);
			ssLabel.Render(delta, mouseX, mouseY);
			qeLabel.Render(delta, mouseX, mouseY);

			Changelog.Render(delta, mouseX, mouseY);

			base.Render(delta, mouseX, mouseY);
		}

		public override void OnResize(Size size)
		{
			Console.WriteLine($"Resolution: {size.Width}, {size.Height}");

			ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);

			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;

			var CHANGELOGLabelFontSize = 40f * heightdiff;
			var SSLabelFontSize = 150f * heightdiff;
			var QELabelFontSize = 36f * heightdiff;
			var ChangelogFontSize = 16f;

			CHANGELOGlabel.ClientRectangle.Location = new PointF(60 * widthdiff, 200 * heightdiff);
			CHANGELOGlabel.FontSize = (int)CHANGELOGLabelFontSize;
			//CHANGELOGlabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 157, ClientRectangle.Top + 210);
			ssLabel.ClientRectangle.Location = new PointF(35 * widthdiff, 35 * heightdiff);
			ssLabel.FontSize = (int)SSLabelFontSize;
			//ssLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 37, ClientRectangle.Top + 35);
			qeLabel.ClientRectangle.Location = new PointF(615 * widthdiff, 140 * heightdiff);
			qeLabel.FontSize = (int)QELabelFontSize;
			//qeLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Left + 617, ClientRectangle.Top + 140);
			Changelog.ClientRectangle.Location = new PointF(60 * widthdiff, 230 * heightdiff);
			Changelog.FontSize = (int)ChangelogFontSize;

			// buttons
			_createMapButton.ClientRectangle.Location = new PointF(1190 * widthdiff, 180 * heightdiff);
			_loadMapButton.ClientRectangle.Location = new PointF(1190 * widthdiff, 295 * heightdiff);
			_importButton.ClientRectangle.Location = new PointF(1190 * widthdiff, 410 * heightdiff);
			_SettingsButton.ClientRectangle.Location = new PointF(1190 * widthdiff, 525 * heightdiff);

			if (Settings.Default.AutosavedFile != "")
				_AutosavedButton.ClientRectangle.Location = new PointF(1190 * widthdiff, 640 * heightdiff);

			if (Settings.Default.LastFile != "")
				_LastButton.ClientRectangle.Location = new PointF(1190 * widthdiff, Settings.Default.AutosavedFile == "" ? 640 * heightdiff : 755 * heightdiff);

			// resizing
			_createMapButton.ClientRectangle.Size = new SizeF(600 * widthdiff, 100 * heightdiff);
			_loadMapButton.ClientRectangle.Size = new SizeF(600 * widthdiff, 100 * heightdiff);
			_importButton.ClientRectangle.Size = new SizeF(600 * widthdiff, 100 * heightdiff);
			_SettingsButton.ClientRectangle.Size = new SizeF(600 * widthdiff, 100 * heightdiff);

			if (Settings.Default.AutosavedFile != "")
				_AutosavedButton.ClientRectangle.Size = new SizeF(600 * widthdiff, 100 * heightdiff);

			if (Settings.Default.LastFile != "")
				_LastButton.ClientRectangle.Size = new SizeF(600 * widthdiff, 100 * heightdiff);

			ScrollBar.ClientRectangle.Location = new PointF(950 * widthdiff, 230 * heightdiff);
			ScrollBar.ClientRectangle.Size = new SizeF(20 * widthdiff, 720 * heightdiff);

			base.OnResize(size);

			AssembleChangelog();
		}

		public void OnMouseLeave()
		{
			ScrollBar.Dragging = false;
		}

		public void AssembleChangelog()
        {
			if (ChangelogText == null)
				return;
			var widthdiff = ClientRectangle.Width / 1920f;
			var heightdiff = ClientRectangle.Height / 1080f;
			string result = "";
			List<string> lines = new List<string>();
			foreach (var line in ChangelogText.Split('\n'))
            {
				var test = 1;
				var lineedit = line;
				while (EditorWindow.Instance.FontRenderer.GetWidth(lineedit, Changelog.FontSize) > 900 * widthdiff && test < 20)
                {
					var index = lineedit.LastIndexOf(' ');
					if (index >= 0)
                    {
						if (EditorWindow.Instance.FontRenderer.GetWidth(lineedit.Substring(0, index), Changelog.FontSize) < 900 * widthdiff)
						{
							lineedit = lineedit.Remove(index, 1);
							lineedit = lineedit.Insert(index, "\n");
						}
						else
						{
							lineedit = lineedit.Remove(index, 1);
							lineedit = lineedit.Insert(index, "_");
						}
					}
					test += 1;
                }
				lineedit = lineedit.Replace('_', ' ');
				foreach (var newline in lineedit.Split('\n'))
                {
					if (newline != "\n")
						lines.Add(newline);
				}
            }
			ScrollBar.MaxValue = lines.Count;
			for (int i = 0; i < lines.Count; i++)
            {
				if (i >= ScrollBar.MaxValue - ScrollBar.Value && i < ScrollBar.MaxValue - ScrollBar.Value + 715 * heightdiff / Changelog.FontSize - 1)
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
				case 4:
					if (Settings.Default.AutosavedFile != "")
						EditorWindow.Instance.LoadMap(Settings.Default.AutosavedFile, false);
					break;
				case 5:
					if (Settings.Default.LastFile != "")
						EditorWindow.Instance.LoadFile(Settings.Default.LastFile);
					break;
			}
			base.OnButtonClicked(id);
		}
	}
}
