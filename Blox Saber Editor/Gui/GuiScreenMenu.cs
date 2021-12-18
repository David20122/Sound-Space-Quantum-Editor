using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;

namespace Sound_Space_Editor.Gui
{
	class GuiScreenMenu : GuiScreen
	{
		private int logoTxt;
		private readonly int _textureId;
		private bool bgImg = false;

		private readonly GuiLabel CHANGELOGlabel = new GuiLabel(0, 0, "CHANGELOG", "square", 40) { Centered = true };
		private readonly GuiLabel CHANGELOGlabelOutline = new GuiLabel(0, 0, "CHANGELOG", "squareo", 41) { Centered = true };

		private readonly GuiLabel ssLabel = new GuiLabel(0, 0, "SOUND SPACE", "square", 150);
		private readonly GuiLabel ssLabelOutline = new GuiLabel(0, 0, "SOUND SPACE", "squareo", 151);

		private readonly GuiLabel qeLabel = new GuiLabel(0, 0, "QUANTUM EDITOR", "square", 36);
		private readonly GuiLabel qeLabelOutline = new GuiLabel(0, 0, "QUANTUM EDITOR", "squareo", 36);

		public GuiScreenMenu() : base(0, 0, EditorWindow.Instance.ClientSize.Width, EditorWindow.Instance.ClientSize.Height)
		{

			if (File.Exists(Path.Combine(EditorWindow.Instance.LauncherDir, "gradient.png")))
			{
				bgImg = true;
				using (Bitmap img = new Bitmap(Path.Combine(EditorWindow.Instance.LauncherDir, "gradient.png")))
				{
					_textureId = TextureManager.GetOrRegister("menubg", img, true);
				}
			}

			CHANGELOGlabel.Color = Color.FromArgb(255, 255, 255);
			CHANGELOGlabelOutline.Color = Color.FromArgb(0, 0, 0);
			ssLabel.Color = Color.FromArgb(255, 255, 255);
			ssLabelOutline.Color = Color.FromArgb(0, 0, 0);
			qeLabel.Color = Color.FromArgb(255, 255, 255);
			qeLabelOutline.Color = Color.FromArgb(0, 0, 0);
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

			base.Render(delta, mouseX, mouseY);

			GL.Color4(Color.FromArgb(50, 57, 56, 47));
			Glu.RenderQuad(size.Width / 2 - 1850 / 2, size.Height / 2 - 325, 950, 790);
			GL.Color4(Color.FromArgb(30, 36, 35, 33));
			Glu.RenderQuad(size.Width / 2 - 1810 / 2, size.Height / 2 - 270, 900, 715);

			CHANGELOGlabel.Render(delta, mouseX, mouseY);
			CHANGELOGlabelOutline.Render(delta, mouseX, mouseY);
			ssLabel.Render(delta, mouseX, mouseY);
			ssLabelOutline.Render(delta, mouseX, mouseY);
			qeLabel.Render(delta, mouseX, mouseY);
			qeLabelOutline.Render(delta, mouseX, mouseY);
		}

		public override void OnResize(Size size)
		{
			ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);

			CHANGELOGlabel.ClientRectangle.Location = new PointF(size.Width / 2 - 1615 / 2, size.Height / 2 - 295);
			CHANGELOGlabelOutline.ClientRectangle.Location = new PointF(size.Width / 2 - 1617 / 2, size.Height / 2 - 295);

			ssLabel.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 1860 / 2, ClientRectangle.Height / 2 - 470);
			ssLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 1862 / 2, ClientRectangle.Height / 2 - 470);

			qeLabel.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 685 / 2, ClientRectangle.Height / 2 - 360);
			qeLabelOutline.ClientRectangle.Location = new PointF(ClientRectangle.Width / 2 - 687 / 2, ClientRectangle.Height / 2 - 360);

			base.OnResize(size);
		}

		protected override void OnButtonClicked(int id)
		{
			/*
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
					EditorWindow.Instance.LoadFile(Properties.Settings.Default.LastFile);
					break;
			}
			base.OnButtonClicked(id);
		*/
		}
	}
}
