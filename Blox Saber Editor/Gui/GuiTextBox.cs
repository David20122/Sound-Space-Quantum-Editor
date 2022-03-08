using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Sound_Space_Editor.Gui
{
	class GuiTextBox : Gui
	{
		public bool Numeric;
		public bool Centered;
		public bool CanBeNegative;
		public bool Timings;
		public Color Color1;
		public Color Color2;
		private bool _focused;
		private string _text = "";
		private int _cursorPos;

		private float _timer;

		public event EventHandler<string> OnChanged;

		public string Text
		{
			get => _text;

			set
			{
				var last = _text;

				if (last != value)
					OnChanged?.Invoke(null, Text);

				_cursorPos = Math.Min(_cursorPos, (_text = value).Length);
				
			}
		}

		public bool Focused
		{
			get => _focused;
			set
			{
				var last = _focused;
				_focused = value;

				OnFocus(value);

				if (last != value)
					OnChanged?.Invoke(null, Text);
			}
		}

		public GuiTextBox(float x, float y, float sx, float sy) : base(x, y, sx, sy)
		{
		}

		private void OnFocus(bool flag)
		{
			if (flag)
				return;

			var hasDecimalPoint = _text.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

			if (Numeric)
			{
				if (string.IsNullOrWhiteSpace(_text))
				{
					Text = "0";
					return;
				}
				if (hasDecimalPoint)
				{
					var text = Text;

					if (text.Length > 0 && text[text.Length - 1].ToString() == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
					{
						text += 0;
					}

					if (decimal.TryParse(text, out var parsed))
					{
						var old = Text;

						Text = parsed.ToString();
					}

				}
				if (decimal.TryParse(_text, out var num) && num == (long)num)
				{
					var old = Text;

					Text = ((long)num).ToString();
				}
			}
		}


		public override void Render(float delta, float mouseX, float mouseY)
		{
			if (Visible)
            {
				if (EditorWindow.Instance.GuiScreen is GuiScreenSettings settings)
				{
					Color1 = Color.FromArgb(50, 50, 50);
					Color2 = Color.FromArgb(255, 255, 255);

				}
				else
				{

					Color1 = EditorWindow.Instance.Color1;
					Color2 = EditorWindow.Instance.Color2;
				}

				var rect = ClientRectangle;

				var x = rect.X + rect.Height / 4;
				var y = rect.Y + rect.Height / 2;

				GL.Color3(0.1f, 0.1f, 0.1f);
				Glu.RenderQuad(rect);
				GL.Color3(0.5f, 0.5f, 0.5f);
				Glu.RenderOutline(rect);

				var fr = EditorWindow.Instance.FontRenderer;
				if (Timings)
					fr = TimingPoints.Instance.FontRenderer;

				var renderedText = _text;

				var txwidthratio = fr.GetWidth(renderedText, 24) / (rect.Width - 4);
				var txheight = (int)Math.Min(24, Math.Min(rect.Height, 24 / txwidthratio));

				/*while (fr.GetWidth(renderedText, 24) != null && fr.GetWidth(renderedText, 24) > rect.Width - rect.Height / 2)
				{
					renderedText = renderedText.Substring(1, renderedText.Length - 1);
				}*/

				var offX = (int)(ClientRectangle.Width / 2 - fr.GetWidth(renderedText, txheight) / 2f - rect.Height / 4);

				if (Centered)
					GL.Translate(offX, 0, 0);

				GL.Color3(Color2);
				fr.Render(renderedText, (int)x, (int)(y - fr.GetHeight(txheight) / 2f), txheight);

				if (Focused)
				{
					var textToCursor = renderedText.Substring(0,
						Math.Max(0, Math.Min(renderedText.Length, renderedText.Length - (_text.Length - _cursorPos))));
					var textToCursorSize = fr.GetWidth(textToCursor, txheight);

					var cursorHeight = fr.GetHeight(txheight) * 1.4f;

					var alpha = (float)(Math.Sin(_timer * MathHelper.TwoPi) + 1) / 2;

					GL.Color4(Color2);

					if ((int)_timer % 2 == 0)
						Glu.RenderQuad(x + textToCursorSize, y - cursorHeight / 2, 1, cursorHeight);

					_timer += delta * 2f;
				}
				else
				{
					_timer = 0;
				}
				if (Centered)
					GL.Translate(-offX, 0, 0);
			}
		}

		public void OnMouseClick(float x, float y)
		{
			if (!ClientRectangle.Contains(x, y))
			{
				Focused = false;
				return;
			}

			if (_text.Length > 0)
            {
				var txwidthratio = EditorWindow.Instance.FontRenderer.GetWidth(_text, 24) / (ClientRectangle.Width - 4);
				var txheight = (int)Math.Min(24, Math.Min(ClientRectangle.Height, 24 / txwidthratio));

				var textwidth = EditorWindow.Instance.FontRenderer.GetWidth(_text, txheight);
				var posX = x - ClientRectangle.X - (ClientRectangle.Width - textwidth) / 2;
				var letterwidth = textwidth / _text.Length;

				posX = Math.Max(0, posX);
				posX = Math.Min(textwidth, posX);
				posX = (float)Math.Floor(posX / letterwidth + 0.3);

				_cursorPos = (int)posX;
			}

			_timer = 0;
			Focused = true;
		}

		public void OnKeyTyped(char key)
		{
			if (!Focused)
				return;

			var keyChar = key.ToString();

			try
			{
				if (Numeric)
                {
					if (int.TryParse(key.ToString(), out var num) || (key.ToString() == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator && !Text.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)) || (key == '-' && !Text.Contains("-") && _cursorPos == 0))
						_text = _text.Insert(_cursorPos, keyChar);
				}
				else
					_text = _text.Insert(_cursorPos, keyChar);
			}
			catch { }

			_cursorPos++;
		}

		public void OnKeyDown(Key key, bool control)
		{
			if (!Focused)
				return;

			_timer = 0;

			switch (key)
			{
				case Key.C when control:
					//TODO maybe? edit: NO
					break;
				case Key.V when control:
					var clipboard = Clipboard.GetText();

					if (!string.IsNullOrWhiteSpace(clipboard))
					{
						_text += clipboard;
						_cursorPos += clipboard.Length;
					}
					break;
				case Key.Left:
					_cursorPos = Math.Max(0, _cursorPos - 1);
					break;
				case Key.Right:
					try
					{
						_cursorPos = Math.Min(_text.Length, _cursorPos + 1);
					}
					catch //was too lazy
					{

					}
					break;
				case Key.BackSpace:
					if (control)
					{
						_text = "";
					}
					else if (_text.Length > 0 && _cursorPos > 0)
					{
						try
						{
							_cursorPos = Math.Max(0, _cursorPos - 1);
							_text = _text.Remove(_cursorPos, 1);
						}
						catch //was too lazy
						{

						}
					}
					break;
				case Key.Delete:
					if (_text.Length > 0 && _cursorPos < _text.Length)
					{
						try
						{
							_text = _text.Remove(Math.Min(_cursorPos, _text.Length - 1), 1);
						}
						catch //was too lazy
						{

						}
					}
					break;
				case Key.Enter:
				case Key.KeypadEnter:
					if (!Focused)
						OnChanged?.Invoke(null, Text);
					else
						Focused = false;

					break;
			}
		}
	}
}