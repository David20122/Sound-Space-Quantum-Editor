using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.GUI
{
	class GuiButtonPlayPause : GuiButton
	{
        public GuiButtonPlayPause(float posx, float posy, float sizex, float sizey, int id) : base(posx, posy, sizex, sizey, id, "", 0, true)
        {
            texture = TextureManager.GetOrRegister("widgets");
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            var x = MainWindow.Instance.MusicPlayer.IsPlaying ? 0.5f : 0;

            GL.Color3(1f, 1f, 1f);
            GLSpecial.TexturedQuad(rect, x, 0, x + 0.5f, 0.5f, texture);
        }
    }
}