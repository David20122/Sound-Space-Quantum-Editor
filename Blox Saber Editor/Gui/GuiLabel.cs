using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.Gui
{
	class GuiLabel : Gui
	{
		public string Text;
		public string Font;
		public int FontSize;
		public FontRenderer fr;
		public bool Centered = false;
		public bool Timings = false;

		public Color Color = Color.White;

		public GuiLabel(float x, float y, string text, bool timings) : base(x, y, 0, 0)
		{
			Text = text;
			Timings = timings;
			Font = "main";
			FontSize = 24;
		}

		public GuiLabel(float x, float y, string text) : base(x, y, 0, 0)
		{
			Text = text;
			Font = "main";
			FontSize = 24;
		}

		public GuiLabel(float x, float y, string text, int size) : base(x, y, 0, 0)
		{
			Text = text;
			Font = "main";
			FontSize = size;
		}

		public GuiLabel(float x, float y, string text, string font) : base(x, y, 0, 0)
		{
			Text = text;
			Font = font;
			FontSize = 24;
		}

		public GuiLabel(float x, float y, string text, string font, int size) : base(x, y, 0, 0)
		{
			Text = text;
			Font = font;
			FontSize = size;
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			GL.Color4(Color);

			if (Font == "squareo")
			{
				fr = EditorWindow.Instance.SquareOFontRenderer;
			}
			else if (Font == "square")
			{
				fr = EditorWindow.Instance.SquareFontRenderer;
			}
			else if (Font == "main")
			{
				fr = EditorWindow.Instance.FontRenderer;
			}

			if (Timings)
				fr = TimingPoints.Instance.FontRenderer;

			if (Centered)
			{
				var w = fr.GetWidth(Text, FontSize);
				var h = fr.GetHeight(FontSize);

				fr.Render(Text, (int)(ClientRectangle.X - w / 2f), (int)(ClientRectangle.Y - h / 2f), FontSize);
			}
			else
				fr.Render(Text, (int)ClientRectangle.X, (int)ClientRectangle.Y, FontSize);
		}
	}
}
