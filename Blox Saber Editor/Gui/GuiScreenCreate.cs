using System.Drawing;
using System.Windows.Forms;
using OpenTK.Input;

namespace Sound_Space_Editor.Gui
{
	class GuiScreenCreate : GuiScreen
	{
		private readonly GuiTextBox _tb;
		private readonly GuiButton _btnCreate;
		private readonly GuiButton _btnBack;
		private readonly GuiLabel _lbl = new GuiLabel(0, 0, "i need audio id") { Centered = true };

		public GuiScreenCreate() : base(0, 0, 0, 0)
		{
			_tb = new GuiTextBox(0, 0, 256, 64) { Centered = true, Focused = true };
			_btnCreate = new GuiButton(0, 0, 0, 256, 64, "CREATE");
			_btnBack = new GuiButton(1, 0, 0, 256, 64, "BACK");

			_lbl.Color = Color.FromArgb(255, 255, 255);

			OnResize(EditorWindow.Instance.ClientSize);

			Buttons.Add(_btnCreate);
			Buttons.Add(_btnBack);
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			_tb.Render(delta, mouseX, mouseY);

			foreach (var button in Buttons)
			{
				button.Render(delta, mouseX, mouseY);
			}

			_lbl.Render(delta, mouseX, mouseY);
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

		protected override void OnButtonClicked(int id)
		{
			switch (id)
			{
				case 0:
					var text = _tb.Text.Trim();

					if (long.TryParse(text, out var parsed))
					{
						EditorWindow.Instance.CreateMap(parsed);
					}
					else
					{
						MessageBox.Show("this isnt a number smhHhH.", "Error", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}

					break;
				case 1:
					EditorWindow.Instance.OpenGuiScreen(new GuiScreenSelectMap());
					break;
			}
		}

		public override void OnResize(Size size)
		{
			var middle = new PointF(size.Width / 2f, size.Height / 2f);

			var rect = _tb.ClientRectangle;
			_lbl.ClientRectangle.Location = new PointF(middle.X, middle.Y - rect.Height / 2 - 20);
			_tb.ClientRectangle.Location = new PointF(middle.X - rect.Width / 2, middle.Y - rect.Height / 2);
			_btnCreate.ClientRectangle.Location = new PointF(middle.X - _btnCreate.ClientRectangle.Width / 2, middle.Y + rect.Height / 2 + 20);
			_btnBack.ClientRectangle.Location = new PointF(_btnCreate.ClientRectangle.X, _btnCreate.ClientRectangle.Bottom + 10);
		}
	}
}
