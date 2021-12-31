﻿using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.Gui
{
	class GuiSlider : GuiButton
	{
		public int MaxValue = 7;

		public int Value = 0;

		public float Progress;

		public bool Dragging;

		public bool Snap = true;

		private readonly bool _vertical;

		private float _alpha;

		public int[] Color1;
		public int[] Color2;

		public GuiSlider(float x, float y, float sx, float sy) : base(int.MinValue, x, y, sx, sy, "", false)
		{
			_vertical = sx < sy;
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			if (EditorWindow.Instance.GuiScreen is GuiScreenMenu menu)
			{
				Color1 = new int[] { 255, 255, 255 };
				Color2 = new int[] { 50, 50, 50 };

			}
			else
			{

				Color1 = EditorWindow.Instance.Color1;
				Color2 = EditorWindow.Instance.Color2;
			}

			if (MaxValue > 0)
            {
				var rect = ClientRectangle;

				IsMouseOver = rect.Contains(mouseX, mouseY);

				var lineSize = _vertical ? rect.Height - rect.Width : rect.Width - rect.Height;

				var step = lineSize / MaxValue;
				var pos = Snap ? step * Value : Progress * lineSize;
				var lineRect = new RectangleF(rect.X + (_vertical ? rect.Width / 2 - 1 : rect.Height / 2), rect.Y + (_vertical ? rect.Width / 2 : rect.Height / 2 - 1), _vertical ? 2 : lineSize, _vertical ? lineSize : 2);
				var cursorPos = new PointF(rect.X + (_vertical ? rect.Width / 2 : rect.Height / 2 + pos), _vertical ? rect.Bottom - rect.Width / 2 - pos : rect.Y + rect.Height / 2);

				var mouseClose = Dragging || Math.Sqrt(Math.Pow(mouseX - cursorPos.X, 2) + Math.Pow(mouseY - cursorPos.Y, 2)) <= 12;

				_alpha = MathHelper.Clamp(_alpha + (mouseClose ? 10 : -10) * delta, 0, 1);

				RenderTimeline(lineRect);

				//cursor
				GL.Translate(cursorPos.X, cursorPos.Y, 0);
				GL.Rotate(_alpha * 90, 0, 0, 1);

				if (_alpha > 0)
				{
					GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
					GL.Color4(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
					Glu.RenderCircle(0, 0, 12 * _alpha);
				}

				GL.Rotate(-_alpha * 90, 0, 0, 1);
				GL.Translate(-cursorPos.X, -cursorPos.Y, 0);

				GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
				GL.Color4(Color.FromArgb(Color1[0], Color1[1], Color1[2]));
				Glu.RenderCircle(cursorPos.X, cursorPos.Y, 4, 16);
				//GL.LineWidth(1);
			}

		}
		protected virtual void RenderTimeline(RectangleF rect)
		{

			GL.Color4(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
			Glu.RenderQuad(rect);
			GL.Color4(Color.FromArgb(Color2[0], Color2[1], Color2[2]));
			Glu.RenderOutline(rect);
		}

		public override void OnResize(Size size)
		{
			ClientRectangle = new RectangleF(size.Width - ClientRectangle.Size.Width, size.Height - ClientRectangle.Size.Height - 64, ClientRectangle.Size.Width, ClientRectangle.Size.Height);
		}
	}
}