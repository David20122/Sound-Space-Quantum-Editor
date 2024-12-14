using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SSQE_Player.Models
{
    internal class ModelManager
    {
        private static readonly List<VertexArrayHandle> VaOs = new();
        private static readonly List<BufferHandle> VbOs = new();

        private readonly Dictionary<string, Model> models = new();
        private static readonly Dictionary<string, (BufferHandle, BufferHandle)> instancedHandles = new();

        public void RegisterModel(string name, float[] vertices, float scale)
        {
            Model model = LoadModelToVao(vertices, scale);

            models.Add(name, model);
            instancedHandles.Add(name, (VbOs[^2], VbOs[^1]));
        }

        public Model GetModel(string name)
        {
            return models[name];
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

            BufferHandle instancedvbo1 = GL.GenBuffer();
            VbOs.Add(instancedvbo1);
            BufferHandle instancedvbo2 = GL.GenBuffer();
            VbOs.Add(instancedvbo2);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, instancedvbo1);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribDivisor(1, 1);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, instancedvbo2);

            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribDivisor(2, 1);
        }

        public void Render(Vector3[] positions, Vector4[] colors, int count, string name)
        {
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, instancedHandles[name].Item1);
            GL.BufferData(BufferTargetARB.ArrayBuffer, positions, BufferUsageARB.DynamicDraw);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, instancedHandles[name].Item2);
            GL.BufferData(BufferTargetARB.ArrayBuffer, colors, BufferUsageARB.DynamicDraw);

            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, models[name].vertexCount, count);
        }
    }
}
