using New_SSQE.Audio;
using New_SSQE.GUI.Font;
using New_SSQE.GUI.Shaders;
using OpenTK.Graphics.OpenGL;

namespace New_SSQE.GUI
{
    internal class GuiButtonPlayPause : GuiButton
    {
        private bool wasPlaying = false;

        public GuiButtonPlayPause(int id) : base(0, 0, 0, 0, id, "", 0, true)
        {
            HasSubTexture = true;
            tHandle = TextureManager.GetOrRegister("widgets", null, false, TextureUnit.Texture1);
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (MusicPlayer.IsPlaying != wasPlaying)
            {
                Update();

                wasPlaying = MusicPlayer.IsPlaying;
            }

            base.Render(mousex, mousey, frametime);
        }

        public override void RenderTexture()
        {
            TextureManager.SetActive(1);

            GL.BindTexture(TextureTarget.Texture2d, tHandle);
            GL.BindVertexArray(tVaO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.UseProgram(FontRenderer.unicode ? Shader.UnicodeProgram : Shader.FontProgram);
            FontRenderer.SetActive("main");
        }

        public override Tuple<float[], float[]> GetVertices()
        {
            float[] vertices = GLU.TexturedRect(Rect, 1f, MusicPlayer.IsPlaying ? 0.5f : 0f, 0f, 0.5f, 0.5f);

            return new(Array.Empty<float>(), vertices);
        }
    }
}
