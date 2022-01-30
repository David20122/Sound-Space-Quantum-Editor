using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.Gui
{
	class GuiCheckBox : GuiButton
	{
		public bool Toggle { get; private set; }

		private float _alpha;
		public int FontSize;
		public Color Color1;
		public Color Color2;

		public GuiCheckBox(int id, float x, float y, float sx, float sy, bool toggle) : base(id, x, y, sx, sy)
		{
			Id = id;

			Toggle = toggle;
		}

		public GuiCheckBox(int id, string text, float x, float y, float sx, float sy, bool toggle) : this(id, x, y, sx, sy, toggle)
		{
			Text = text;
			FontSize = 24;
		}

		public GuiCheckBox(int id, string text, float x, float y, float sx, float sy, int fontsize, bool toggle) : this(id, x, y, sx, sy, toggle)
		{
			Text = text;
			FontSize = fontsize;
		}


		public override void Render(float delta, float mouseX, float mouseY)
		{

			if (EditorWindow.Instance.GuiScreen is GuiScreenSettings settings)
			{

				Color1 = Color.FromArgb(255, 255, 255);
				Color2 = Color.FromArgb(50, 50, 50);

			}
			else
			{

				Color1 = EditorWindow.Instance.Color1;
				Color2 = EditorWindow.Instance.Color2;

			}

			var rect = ClientRectangle;

			IsMouseOver = rect.Contains(mouseX, mouseY);

			GL.Color3(0.05f, 0.05f, 0.05f);
			Glu.RenderQuad(rect);
			GL.Color3(0.2f, 0.2f, 0.2f);
			Glu.RenderOutline(rect);

			_alpha = Toggle ? Math.Min(1, _alpha + delta * 8) : Math.Max(0, _alpha - delta * 8);

			var checkSizeX = rect.Width * 0.75f * _alpha;
			var checkSizeY = rect.Height * 0.75f * _alpha;
			var gapX = (rect.Width - checkSizeX) / 2;
			var gapY = (rect.Height - checkSizeY) / 2;

			if (checkSizeX > 0 && checkSizeY > 0)
			{
				GL.Color4(Color2);
				Glu.RenderQuad(rect.X + gapX, rect.Y + gapY, checkSizeX, checkSizeY);
			}

			var fr = EditorWindow.Instance.FontRenderer;
			var height = fr.GetHeight(FontSize);

			var finaltext = Text;

			if (EditorWindow.Instance.inconspicuousvar)
			{
				finaltext = finaltext.Replace('r', 'w');
				finaltext = finaltext.Replace('R', 'W');
				finaltext = finaltext.Replace('l', 'w');
				finaltext = finaltext.Replace('L', 'W');
				finaltext = finaltext.Replace(':', '~');
			}

			GL.Color3(Color1);
			fr.Render(finaltext, (int)(rect.Right + rect.Height / 4), (int)(rect.Y + rect.Height / 2 - height / 2f), FontSize);
		}

		public override void OnMouseClick(float x, float y)
		{
			Toggle = !Toggle;
		}
	}
}