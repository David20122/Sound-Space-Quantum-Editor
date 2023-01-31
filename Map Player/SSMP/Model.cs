using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SSQE_Player
{
    class Model : IDisposable
    {
        private readonly int bufferSize;
        private readonly int buffer;
        private readonly int vao;

        public Vector3 Size;

        public Model(int Buffer, float[] vertexes, int Vao, bool centered = false)
        {
            buffer = Buffer;
            vao = Vao;

            bufferSize = vertexes.Length / 3;

            if (centered)
                Init(vertexes);
        }

        private void Init(float[] vertexes)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (var i = 0; i < vertexes.Length; i += 3)
            {
                var vector = new Vector3(vertexes[i], vertexes[i + 1], vertexes[i + 2]);

                min = Vector3.ComponentMin(min, vector);
                max = Vector3.ComponentMax(max, vector);
            }

            Size = max - min;

            for (var i = 0; i < vertexes.Length; i += 3)
            {
                vertexes[i] -= min.X + Size.X / 2;
                vertexes[i + 1] -= min.Y + Size.Y / 2;
                vertexes[i + 2] -= min.Z + Size.Z / 2;
            }
        }

        public void Bind()
        {
            GL.BindVertexArray(vao);
            GL.EnableVertexAttribArray(0);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
            GL.DisableVertexAttribArray(0);
        }

        public void Render()
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, bufferSize);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(buffer);
            GL.DeleteVertexArray(vao);
        }
    }
}
