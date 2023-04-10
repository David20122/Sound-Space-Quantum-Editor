using OpenTK.Graphics.OpenGL;
using System;

namespace New_SSQE.GUI
{
    internal class GuiButtonPlayPause : GuiButton
    {
        private bool wasPlaying = false;

        public GuiButtonPlayPause(float posx, float posy, float sizex, float sizey, int id) : base(posx, posy, sizex, sizey, id, "", 0, true)
        {
            HasSubTexture = true;
            tHandle = TextureManager.GetOrRegister("widgets", null, false, TextureUnit.Texture1);
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            var player = MainWindow.Instance.MusicPlayer;

            if (player.IsPlaying != wasPlaying)
            {
                Update();

                wasPlaying = player.IsPlaying;
            }

            base.Render(mousex, mousey, frametime);
        }

        public override void RenderTexture()
        {
            GL.UseProgram(Shader.TexProgram);
            TextureManager.SetActive(1);

            GL.BindVertexArray(tVaO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.UseProgram(Shader.FontTexProgram);
            FontRenderer.SetActive("main");
        }

        public override Tuple<float[], float[]> GetVertices()
        {
            float[] vertices = GLU.TexturedRect(Rect, 1f, MainWindow.Instance.MusicPlayer.IsPlaying ? 0.5f : 0f, 0f, 0.5f, 0.5f);

            return new Tuple<float[], float[]>(Array.Empty<float>(), vertices);
        }
    }
}
