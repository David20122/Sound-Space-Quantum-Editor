using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.Gui
{
	class GuiCheckBox : GuiButton
	{
		public bool Toggle { get; private set; }

		private float _alpha;

		public GuiCheckBox(int id, float x, float y, float sx, float sy, bool toggle) : base(id, x, y, sx, sy)
		{
			Id = id;

			Toggle = toggle;
		}

		public GuiCheckBox(int id, string text, float x, float y, float sx, float sy, bool toggle) : this(id, x, y, sx, sy, toggle)
		{
			Text = text;
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{

			// color 1

			string rc1 = EditorWindow.Instance.ReadLine("settings.ini", 17);
			string[] c1values = rc1.Split(',');
			int[] Color1 = Array.ConvertAll<string, int>(c1values, int.Parse);

			//color 2

			string rc2 = EditorWindow.Instance.ReadLine("settings.ini", 21);
			string[] c2values = rc2.Split(',');
			int[] Color2 = Array.ConvertAll<string, int>(c2values, int.Parse);

			var rect = ClientRectangle;

			IsMouseOver = rect.Contains(mouseX, mouseY);

			GL.Color3(0.05f, 0.05f, 0.05f);
			Glu.RenderQuad(rect);
			GL.Color3(0.2f, 0.2f, 0.2f);
			Glu.RenderOutline(rect);

			_alpha = Toggle ? Math.Min(1, _alpha + delta * 8) : Math.Max(0, _alpha - delta * 8);

			var checkSize = rect.Height * 0.75f * _alpha;
			var gap = (rect.Height - checkSize) / 2;

			if (checkSize > 0)
			{
				GL.Color4(Color.FromArgb(255, Color2[0], Color2[1], Color2[2]));
				Glu.RenderQuad(rect.X + gap, rect.Y + gap, checkSize, checkSize);
			}

			var fr = EditorWindow.Instance.FontRenderer;
			var height = fr.GetHeight(24);

			GL.Color3(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
			fr.Render(Text, (int)(rect.Right + rect.Height / 4), (int)(rect.Y + rect.Height / 2 - height / 2f), 24);
		}

		public override void OnMouseClick(float x, float y)
		{
			Toggle = !Toggle;
		}
	}
}