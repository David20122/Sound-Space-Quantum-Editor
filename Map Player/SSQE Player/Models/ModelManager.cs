using OpenTK.Graphics.OpenGL;

namespace SSQE_Player.Models
{
    internal class ModelManager
    {
        private static readonly List<int> VaOs = new();
        private static readonly List<int> VbOs = new();

        private readonly Dictionary<string, Model> models = new();

        public void RegisterModel(string name, float[] vertices)
        {
            var model = LoadModelToVao(vertices);

            models.Add(name, model);
        }

        public Model GetModel(string name)
        {
            models.TryGetValue(name, out var model);

            return model;
        }

        public static Model LoadModelToVao(float[] vertices)
        {
            int vao = CreateVao();
            StoreDataInAttribList(vertices);

            return new Model(vertices, vao);
        }

        private static int CreateVao()
        {
            int vao = GL.GenVertexArray();
            VaOs.Add(vao);

            GL.BindVertexArray(vao);

            return vao;
        }

        private static void StoreDataInAttribList(float[] data)
        {
            int vbo = GL.GenBuffer();
            VbOs.Add(vbo);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
    }
}
