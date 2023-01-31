using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;

namespace Sound_Space_Editor.Gui
{
	class GuiScreenLoadCreate : GuiScreen
	{
		private readonly Random _r = new Random();

		private readonly GuiButton _createButton;
		private readonly GuiButton _loadButton;
		private readonly GuiButton _pasteButton;
		private readonly GuiButton _githubButton;
		private readonly GuiLabel _lbl = new GuiLabel(0, 0, "Source Code: github.com/TominoCZ");
		private readonly GuiLabel _lbl2 = new GuiLabel(0, 0, "New features: David20122");

		private readonly List<Particle> _particles = new List<Particle>();

		private readonly int _textureId;

		private float _timer;

		public GuiScreenLoadCreate() : base(0, 0, EditorWindow.Instance.ClientSize.Width, EditorWindow.Instance.ClientSize.Height)
		{
			using (var img = Properties.Resources.logo)
			{
				_textureId = TextureManager.GetOrRegister("logo", img, true);
			}

			_createButton = new GuiButton(0, 0, 0, 192, 64, "CREATE MAP");
			_loadButton = new GuiButton(1, 0, 0, 192, 64, "LOAD MAP");
			_pasteButton = new GuiButton(2, 0, 0, 192, 64, "PASTE DATA");
			_githubButton = new GuiButton(3, 0, 0, 192, 64, "LOAD GITHUB LINK");

			_lbl.Color = Color.FromArgb(169, 169, 169);
			_lbl2.Color = Color.FromArgb(169, 169, 169);

			Buttons.Add(_createButton);
			Buttons.Add(_loadButton);
			Buttons.Add(_pasteButton);
			Buttons.Add(_githubButton);

			OnResize(EditorWindow.Instance.ClientSize);
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			_timer += delta;

			if (_timer >= 0.08)
			{
				_timer = 0;

				for (int i = 0; i < 3; i++)
				{
					var s = 15 + (float)_r.NextDouble() * 20;
					var x = (float)_r.NextDouble() * ClientRectangle.Width;
					var y = ClientRectangle.Height + s;

					var mx = -0.5f + (float)_r.NextDouble();
					var my = -(2 + (float)_r.NextDouble() * 2);

					_particles.Add(new Particle(x, y, mx, my, s));
				}
			}

			var rect = ClientRectangle;

			GL.Color3(1, 1, 1f);
			Glu.RenderTexturedQuad(rect.X + rect.Width / 2 - 256, 0, 512, 512, 0, 0, 1, 1, _textureId);

			for (var index = _particles.Count - 1; index >= 0; index--)
			{
				var particle = _particles[index];

				particle.Render(delta);

				if (particle.IsDead)
					_particles.Remove(particle);
			}

			base.Render(delta, mouseX, mouseY);
			_lbl.Render(delta, mouseX, mouseY);
			_lbl2.Render(delta, mouseX, mouseY);
		}


		protected override void OnButtonClicked(int id)
		{
			switch (id)
			{
				case 0:
					EditorWindow.Instance.OpenGuiScreen(new GuiScreenSelectMap());
					break;
				case 1:
					var ofd = new OpenFileDialog
					{
						Title = "Load map",
						Filter = "Text Documents (*.txt)|*.txt"
					};

					var wasFullscreen = EditorWindow.Instance.IsFullscreen;

					if (EditorWindow.Instance.IsFullscreen)
					{
						EditorWindow.Instance.ToggleFullscreen();
					}

					var result = ofd.ShowDialog();

					if (wasFullscreen)
					{
						EditorWindow.Instance.ToggleFullscreen();
					}

					if (result == DialogResult.OK)
					{
						EditorWindow.Instance.LoadFile(ofd.FileName);
					}
					break;
				case 2:
					var clipboard = Clipboard.GetText();

					if (!string.IsNullOrWhiteSpace(clipboard))
					{
						EditorWindow.Instance.LoadMap(clipboard, false);
					}
					break;
				case 3:
					var gclipboard = Clipboard.GetText();
					WebClient wc = new WebClient();
					string reply = wc.DownloadString(gclipboard);
					MessageBox.Show(reply, "reply");
					EditorWindow.Instance.LoadMap(reply, false);
					break;
			}
		}

		public override bool AllowInput()
		{
			return false;
		}

		public sealed override void OnResize(Size size)
		{
			ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);
			_lbl.ClientRectangle.Location = new PointF(0, 0);
			_lbl2.ClientRectangle.Location = new PointF(0, 20);
			_createButton.ClientRectangle.Location =
				new PointF((int)(size.Width / 2f - _createButton.ClientRectangle.Width - 10), 512 - 64 - 20);
			_loadButton.ClientRectangle.Location = new PointF((int)(size.Width / 2f + 10), 512 - 64 - 20);
			//old, incase; _createButton.ClientRectangle.Location = new PointF((int)(size.Width / 2f - _createButton.ClientRectangle.Width - 10), 512);
			//old. incase; _loadButton.ClientRectangle.Location = new PointF((int)(size.Width / 2f + 10), 512);
			_pasteButton.ClientRectangle.Location = new PointF((int)(size.Width / 2f - 192 - 10), 512);
			_githubButton.ClientRectangle.Location = new PointF((int)(size.Width / 2f + 10), 512);
		}
	}

	class Particle
	{
		public float X, Y, Mx, My, Size, Age, MaxAge, Angle, AngleStep;

		public int RotationOrientation = 1;

		public bool IsDead;

		private static readonly Random _r = new Random();

		private readonly int _vertices = _r.Next(3, 6);

		public Particle(float x, float y, float mx, float my, float size)
		{
			X = x;
			Y = y;

			Mx = mx;
			My = my;

			Size = size;

			MaxAge = 0.75f + (float)_r.NextDouble() * 2f;
			Angle = (float)_r.NextDouble() * 45;
			AngleStep = 10 + (float)_r.NextDouble() * 50;

			if (_r.NextDouble() >= 0.45)
				RotationOrientation = -1;
		}

		public void Render(float delta)
		{
			if (IsDead)
				return;

			var speedMult = Math.Max(0.001f, (MaxAge - Age) / MaxAge);

			if (speedMult <= 0.001f)
				IsDead = true;

			var squareMult = (float)Math.Pow(speedMult, 2);

			X += Mx * squareMult;
			Y += My * squareMult;

			Age += delta;
			Angle += delta * RotationOrientation * 360 * speedMult;

			var size = Size * speedMult;

			var alpha = (int)(255 * squareMult);

			GL.Color4(RotationOrientation == 1 ? Color.FromArgb((int)(alpha * 0.2f), 255, 0, 255) : Color.FromArgb((int)(alpha * 0.2f), 0, 255, 200));
			GL.Translate(X, Y, 0);
			GL.Rotate(Angle, 0, 0, 1);
			GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
			//Glu.RenderQuad(-size / 2, -size / 2, size, size);
			Glu.RenderCircle(0, 0, size / 2, _vertices);
			GL.Color4(RotationOrientation == 1 ? Color.FromArgb(alpha, 255, 0, 255) : Color.FromArgb(alpha, 0, 255, 200));
			GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
			Glu.RenderCircle(0, 0, size / 2, _vertices);
			//Glu.RenderOutline(-size / 2, -size / 2, size, size);
			GL.Rotate(-Angle, 0, 0, 1);
			GL.Translate(-X, -Y, 0);

			GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
		}
	}
}
