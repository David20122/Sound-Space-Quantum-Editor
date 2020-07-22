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
		public bool Decimal;
		public bool Centered;
		public bool CanBeNegative;

		private bool _focused;
		private string _text = "";
		private int _cursorPos;

		private float _timer;

		public string Text
		{
			get => _text;

			set => _cursorPos = Math.Min(_cursorPos, (_text = value).Length);
		}

		public bool Focused
		{
			get => _focused;
			set
			{
				_focused = value;

				OnFocus(value);
			}
		}

		public GuiTextBox(float x, float y, float sx, float sy) : base(x, y, sx, sy)
		{
		}

		private void OnFocus(bool flag)
		{
			if (flag)
				return;
				var hasdecimalpoint = _text.Contains(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);

			if (Numeric)
			{
				if (string.IsNullOrWhiteSpace(_text))
				{
					Text = "0";
					return;
				}
				if (Decimal)	//fixed 0 bug
				{
					if (hasdecimalpoint)
					{
						var text = Text.Trim('0');

						if (text.Length > 0 && text[text.Length - 1].ToString() == CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator)
						{
							text += 0;
						}
						if (decimal.TryParse(text, NumberStyles.AllowDecimalPoint, null, out var parsed))
						{
							Text = parsed.ToString();
						}

					}
                }
                //else
                //{
                //    var text = Text.Trim('0');z
				//
                //    if (text.Length > 0 && text[text.Length - 1].ToString() == CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator)
                //    {
                //        text += 0;
                //    }
                //    if (decimal.TryParse(text, NumberStyles.Integer, null, out var parsed))
                //    {
                //        Text = parsed.ToString();
                //    }
               //}
                if (decimal.TryParse(_text, out var num) && num == (long)num)
                {
                    Text = ((long)num).ToString();
                }
            }
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			var rect = ClientRectangle;

			var x = rect.X + rect.Height / 4;
			var y = rect.Y + rect.Height / 2;

			GL.Color3(0.1f, 0.1f, 0.1f);
			Glu.RenderQuad(rect);
			GL.Color3(0.5f, 0.5f, 0.5f);
			Glu.RenderOutline(rect);

			var fr = EditorWindow.Instance.FontRenderer;

			var renderedText = _text;

			while (fr.GetWidth(renderedText, 24) > rect.Width - rect.Height / 2)
			{
				renderedText = renderedText.Substring(1, renderedText.Length - 1);
			}

			var offX = (int)(ClientRectangle.Width / 2 - fr.GetWidth(renderedText, 24) / 2f - rect.Height / 4);

			if (Centered)
				GL.Translate(offX, 0, 0);

			GL.Color3(Color.FromArgb(255, 0, 255));
			fr.Render(renderedText, (int)x, (int)(y - fr.GetHeight(24) / 2f), 24);

			if (Focused)
			{
				var textToCursor = renderedText.Substring(0,
					Math.Max(0, Math.Min(renderedText.Length, renderedText.Length - (_text.Length - _cursorPos))));
				var textToCursorSize = fr.GetWidth(textToCursor, 24);

				var cursorHeight = fr.GetHeight(24) * 1.4f;

				var alpha = (float)(Math.Sin(_timer * MathHelper.TwoPi) + 1) / 2;

				GL.Color4(Color.FromArgb(0, 255, 200));
				Glu.RenderQuad(x + textToCursorSize, y - cursorHeight / 2, 1, cursorHeight);

				_timer += delta * 1.25f;
			}
			else
			{
				_timer = 0;
			}

			if (Centered)
				GL.Translate(-offX, 0, 0);
		}

		public void OnMouseClick(float x, float y)
		{
			if (!ClientRectangle.Contains(x, y))
			{
				Focused = false;
				return;
			}

			Focused = true;
		}

		public void OnKeyTyped(char key)
		{
			if (!Focused)
				return;

			var keyChar = key.ToString();

			try
			{
				_text = _text.Insert(_cursorPos, keyChar);
			}
			catch
			{

			}

			_cursorPos++;
		}

		public void OnKeyDown(Key key, bool control)
		{
			if (!Focused)
				return;

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
			}
		}
	}
}