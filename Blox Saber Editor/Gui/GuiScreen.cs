using System.Collections.Generic;
using OpenTK.Input;

namespace Sound_Space_Editor.Gui
{
	class GuiScreen : Gui
	{
		protected List<GuiButton> Buttons = new List<GuiButton>();

		public bool Pauses { get; }

		protected GuiScreen(float x, float y, float sx, float sy) : base(x, y, sx, sy)
		{
			Pauses = true;
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			foreach (var button in Buttons)
			{
                if (button.Visible)
                {
                    button.Render(delta, mouseX, mouseY);
                }
			}
		}

		public virtual bool AllowInput()
		{
			return true;
		}

		public virtual void OnKeyTyped(char key)
		{

		}

		public virtual void OnKeyDown(Key key, bool control)
		{

		}

		public virtual void OnMouseMove(float x, float y)
		{

		}

		public virtual void OnMouseClick(float x, float y)
		{
			foreach (var button in Buttons)
			{
				if (button.IsMouseOver)
				{
					button.OnMouseClick(x, y);

					var id = EditorWindow.Instance.inconspicuousvar ? "1091083826" : "click";
					EditorWindow.Instance.SoundPlayer.Play(id, 0.035f);
					OnButtonClicked(button.Id);
					break;
				}
			}
		}

		protected virtual void OnButtonClicked(int id)
		{

		}

		public virtual void OnClosing()
		{
			
		}
	}
}