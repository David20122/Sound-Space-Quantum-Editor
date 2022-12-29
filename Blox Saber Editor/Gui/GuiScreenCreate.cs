using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Sound_Space_Editor.Gui
{
	class GuiScreenCreate : GuiScreen
	{
		private readonly int _textureId;
		private bool bgImg = false;

		private readonly GuiTextBox _tb;
		private readonly GuiButton _btnCreate;
		private readonly GuiButton _btnBack;
		private readonly GuiButton _btnImport;
		private readonly GuiLabel _lbl = new GuiLabel(0, 0, "Input Audio ID", false) { Centered = true };

        private readonly GuiLabel ssLabel = new GuiLabel(0, 0, "SOUND SPACE", "square", 150);
		private readonly GuiLabel qeLabel = new GuiLabel(0, 0, "QUANTUM EDITOR", "square", 36);

        public GuiSlider ScrollBar;

        public GuiScreenCreate() : base(0, 0, 0, 0)
		{
            _tb = new GuiTextBox(0, 0, 256, 64) { Centered = true, Focused = false };
			_btnCreate = new GuiButton(0, 0, 0, 256, 64, "CREATE", false);
			_btnBack = new GuiButton(1, 0, 0, 256, 64, "BACK", false);
			_btnImport = new GuiButton(2, 0, 0, 256, 64, "IMPORT AUDIO", false);

            ScrollBar = new GuiSlider(0, 0, 20, 720)
            {
                MaxValue = 0,
                Value = 0,
            };

            OnResize(EditorWindow.Instance.ClientSize);

			Buttons.Add(_btnCreate);
			Buttons.Add(_btnBack);
			Buttons.Add(_btnImport);
			Buttons.Add(ScrollBar);

            _lbl.Color = Color.FromArgb(255, 255, 255);

            ssLabel.Color = Color.FromArgb(255, 255, 255);
            qeLabel.Color = Color.FromArgb(255, 255, 255);

            if (File.Exists(Path.Combine(EditorWindow.Instance.LauncherDir, "background_menu.png")))
			{
				bgImg = true;
				using (Bitmap img = new Bitmap(Path.Combine(EditorWindow.Instance.LauncherDir, "background_menu.png")))
				{
					_textureId = TextureManager.GetOrRegister("createbg", img, true);
				}
			}
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
            }
            else
            {
                GL.Color4(Color.FromArgb(255, 30, 30, 30));
                Glu.RenderQuad(0, 0, size.Width, size.Height);
            }

            foreach (var button in Buttons)
            {
                button.Render(delta, mouseX, mouseY);
            }

            _tb.Render(delta, mouseX, mouseY);
			_lbl.Render(delta, mouseX, mouseY);

            //ssLabel.Render(delta, mouseX, mouseY);
            //qeLabel.Render(delta, mouseX, mouseY);

            ScrollBar.Render(delta, mouseX, mouseY);
        }

		public override void OnKeyTyped(char key)
		{
			_tb.OnKeyTyped(key);
		}

		public override void OnKeyDown(Key key, bool control)
		{
			_tb.OnKeyDown(key, control);
		}

		public override void OnMouseClick(float x, float y)
		{
			_tb.OnMouseClick(x, y);

			base.OnMouseClick(x, y);
		}

        public void OnMouseLeave()
        {
            ScrollBar.Dragging = false;
        }

        protected override void OnButtonClicked(int id)
		{
			switch (id)
			{
				case 0:
					var text = _tb.Text.Trim();
					EditorWindow.Instance.CreateMap(text);

					break;
				case 1:
					EditorWindow.Instance.OpenGuiScreen(new GuiScreenMenu());

					break;
				case 2:
					using (var dialog = new OpenFileDialog
					{
						Title = "Select Audio File",
						Filter = "Audio Files (*.mp3;*.ogg;*.wav;*.flac;*.asset)|*.mp3;*.ogg;*.wav;*.flac;*.asset"
					})
					{
						if (dialog.ShowDialog() == DialogResult.OK)
						{
                            EditorWindow.Instance.ActualAudio = dialog.FileName;
                            FileInfo fi = new FileInfo(EditorWindow.Instance.ActualAudio);
							EditorWindow.Instance.ActualAudioPath = fi.FullName;
                            EditorWindow.Instance.ActualAudioSafe = Path.GetFileNameWithoutExtension(fi.Name);
							try
							{
                                File.Copy(dialog.FileName, EditorWindow.Instance.cacheFolder + EditorWindow.Instance.ActualAudioSafe + fi.Extension);
                                EditorWindow.Instance.CreateMap(fi.Name);
                            } catch (Exception e)
							{
								if (e.GetType().ToString() == "System.IO.IOException")
								{
                                    EditorWindow.Instance.CreateMap(fi.Name);
                                }
							}
						}
					}

					break;
			}
		}

        public override void OnResize(Size size)
		{
            Console.WriteLine($"Resolution: {size.Width}, {size.Height}");

            ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);

            var widthdiff = size.Width / 1920f;
            var heightdiff = size.Height / 1080f;

            var middle = new PointF(size.Width / 2f, size.Height / 2f);
            var SSLabelFontSize = 150f * heightdiff;
            var QELabelFontSize = 36f * heightdiff;

            var rect = _tb.ClientRectangle;

            ssLabel.ClientRectangle.Location = new PointF(35 * widthdiff, 35 * heightdiff);
            ssLabel.FontSize = (int)SSLabelFontSize;
            qeLabel.ClientRectangle.Location = new PointF(615 * widthdiff, 140 * heightdiff);
            qeLabel.FontSize = (int)QELabelFontSize;

            ScrollBar.ClientRectangle.Location = new PointF(950 * widthdiff, 230 * heightdiff);
            ScrollBar.ClientRectangle.Size = new SizeF(20 * widthdiff, 720 * heightdiff);

            _lbl.ClientRectangle.Location = new PointF(middle.X, middle.Y - rect.Height / 2 - 20);
			_tb.ClientRectangle.Location = new PointF(middle.X - rect.Width / 2, middle.Y - rect.Height / 2);
			_btnCreate.ClientRectangle.Location = new PointF(middle.X - _btnCreate.ClientRectangle.Width / 2, middle.Y + rect.Height / 2 + 20);
			_btnImport.ClientRectangle.Location = new PointF(_btnCreate.ClientRectangle.X, _btnCreate.ClientRectangle.Bottom + 10);
			_btnBack.ClientRectangle.Location = new PointF(_btnImport.ClientRectangle.X, _btnImport.ClientRectangle.Bottom + 10);
		}
	}
}
