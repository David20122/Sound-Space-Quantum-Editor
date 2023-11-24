using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SSQE_Player.Models
{
    internal class ModelManager
    {
        private static readonly List<VertexArrayHandle> VaOs = new();
        private static readonly List<BufferHandle> VbOs = new();

        private readonly Dictionary<string, Model> models = new();

        public void RegisterModel(string name, float[] vertices, float scale)
        {
            var model = LoadModelToVao(vertices, scale);

            models.Add(name, model);
        }

        public Model GetModel(string name)
        {
            models.TryGetValue(name, out var model);

            return model;
        }

        public static Model LoadModelToVao(float[] vertices, float scale)
        {
            VertexArrayHandle vao = CreateVao();
            StoreDataInAttribList(vertices);

            return new Model(vertices, scale, vao);
        }

        private static VertexArrayHandle CreateVao()
        {
            VertexArrayHandle vao = GL.GenVertexArray();
            VaOs.Add(vao);

            GL.BindVertexArray(vao);

            return vao;
        }

        private static void StoreDataInAttribList(float[] data)
        {
            BufferHandle vbo = GL.GenBuffer();
            VbOs.Add(vbo);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            GL.BufferData(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
    }
}
