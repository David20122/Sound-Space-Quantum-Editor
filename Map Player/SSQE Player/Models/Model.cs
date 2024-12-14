using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SSQE_Player.Models
{
    internal class Model
    {
        public readonly int vertexCount;
        private readonly VertexArrayHandle VaO;

        public Vector3 Size;

        public Model(float[] vertices, float scale, VertexArrayHandle vao)
        {
            VaO = vao;

            vertexCount = vertices.Length / 3;

            Init(vertices, scale);
        }

        private void Init(float[] vertices, float scale)
        {
            Vector3 min = (float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = (float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 vec = (vertices[i * 3 + 0], vertices[i * 3 + 1], vertices[i * 3 + 2]);

                min = Vector3.ComponentMin(min, vec);
                max = Vector3.ComponentMax(max, vec);
            }

            Size = (max - min) / scale;

            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i * 3 + 0] = min.X + Size.X / 2;
                vertices[i * 3 + 1] = min.Y + Size.Y / 2;
                vertices[i * 3 + 2] = min.Z + Size.Z / 2;
            }
        }

        public void Bind()
        {
            GL.BindVertexArray(VaO);
        }
    }
}
