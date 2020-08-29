using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.Gui
{
	class GuiButton : Gui
	{
		public bool IsMouseOver { get; protected set; }
		public int Id;
		public string Text = " ";
        public bool Visible = true;

		protected int Texture;

		private float _alpha;

		public GuiButton(int id, float x, float y, float sx, float sy) : base(x, y, sx, sy)
		{
			Id = id;
		}

		public GuiButton(int id, float x, float y, float sx, float sy, int texture) : this(id, x, y, sx, sy)
		{
			Texture = texture;
		}

		public GuiButton(int id, float x, float y, float sx, float sy, string text) : this(id, x, y, sx, sy)
		{
			Text = text;
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			var IsMouseOverr = ClientRectangle.Contains(mouseX, mouseY);
			if (IsMouseOver && !IsMouseOverr)
			{
				OnMouseLeave(mouseX, mouseY);
			}
			else if (!IsMouseOver && IsMouseOverr)
			{
				OnMouseEnter(mouseX, mouseY);
			}
			IsMouseOver = IsMouseOverr;

			_alpha = MathHelper.Clamp(_alpha + (IsMouseOver ? 10 : -10) * delta, 0, 1);

			if (Texture > 0)
			{
				if (IsMouseOver)
					GL.Color3(0.75f, 0.75f, 0.75f);
				else
					GL.Color3(1f, 1, 1);

				Glu.RenderTexturedQuad(ClientRectangle, 0, 0, 1, 1, Texture);
			}
			else
			{
				var d = 0.075f * _alpha;

				GL.Color3(0.1f + d, 0.1f + d, 0.1f + d);
				Glu.RenderQuad(ClientRectangle);

				GL.Color3(0.2f + d, 0.2f + d, 0.2f + d);
				Glu.RenderOutline(ClientRectangle);
			}

			var fr = EditorWindow.Instance.FontRenderer;
			var width = fr.GetWidth(Text, (int)ClientRectangle.Height / 2);
			var height = fr.GetHeight((int)ClientRectangle.Height / 2);

			GL.Color3(1f, 1, 1);
			fr.Render(Text, (int)(ClientRectangle.X + ClientRectangle.Width / 2 - width / 2f), (int)(ClientRectangle.Y + ClientRectangle.Height / 2 - height / 2f), (int)ClientRectangle.Height / 2);
		}


		public virtual void OnMouseClick(float x, float y) { }
		public virtual void OnMouseEnter(float x, float y) { }
		public virtual void OnMouseLeave(float x, float y) { }
	}
}