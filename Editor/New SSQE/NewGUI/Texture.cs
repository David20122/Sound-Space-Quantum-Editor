using New_SSQE.GUI;
using New_SSQE.GUI.Shaders;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SkiaSharp;
using System.Drawing;

namespace New_SSQE.NewGUI
{
    internal class Texture : IDisposable
    {
        // todo: new shader for coloring/sprite compatibility

        private static readonly RectangleF defaultSource = new(0, 0, 1, 1);

        private ProgramHandle shader;
        private TextureHandle texture;
        private TextureUnit texUnit;

        private VertexArrayHandle vao;
        private BufferHandle vbo;

        public Texture(string texture, SKBitmap? img = null, bool smooth = false, TextureUnit unit = TextureUnit.Texture0)
        {
            shader = Shader.TextureProgram;
            this.texture = TextureManager.GetOrRegister(texture, img, smooth, unit);
            texUnit = unit;

            (vao, vbo) = GLState.NewVAO_VBO(2, 2, 1);
        }

        public Texture(string texture) : this(texture, null, false, TextureUnit.Texture0) { }

        public void Draw(float x, float y, float w, float h,
            float tx = 0, float ty = 0, float tw = 1, float th = 1, float alpha = 1)
        {
            float[] vertices = GLVerts.Texture(x, y, w, h, tx, ty, tw, th, alpha);

            GLState.BufferData(vbo, vertices);
        }

        public void Draw(RectangleF dest, RectangleF? source = null, float alpha = 1)
        {
            RectangleF rect = source ?? defaultSource;
            Draw(dest.X, dest.Y, dest.Width, dest.Height, rect.X, rect.Y, rect.Width, rect.Height, alpha);
        }

        public void Render()
        {
            if (_disposed)
                return;

            GLState.EnableTextureUnit(shader, texUnit);
            GLState.EnableTexture(texture);

            GLState.DrawTriangles(vao, 0, 6);
        }



        private bool _disposed = false;

        public virtual void Dispose()
        {
            if (_disposed)
                return;

            GLState.Clean(vbo);
            GLState.Clean(texture);

            _disposed = true;
        }
    }
}
