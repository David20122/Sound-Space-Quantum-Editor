using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.Gui
{
	class GuiButtonPlayPause : GuiButton
	{
		public GuiButtonPlayPause(int id, float x, float y, float sx, float sy) : base(id, x, y, sx, sy)
		{
			Texture = TextureManager.GetOrRegister("widgets");
		}

		public override void Render(float delta, float mouseX, float mouseY)
		{
			var rect = ClientRectangle;

			IsMouseOver = rect.Contains(mouseX, mouseY);
			var b = EditorWindow.Instance.MusicPlayer.IsPlaying;

			float us = b ? 0.5f : 0;
			
			GL.Color3(1, 1, 1f);
			Glu.RenderTexturedQuad(rect.X, rect.Y, rect.Width, rect.Height, us, 0, us + 0.5f, 0.5f, Texture);
		}
	}
}