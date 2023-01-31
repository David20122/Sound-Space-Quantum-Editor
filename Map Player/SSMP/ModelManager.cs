using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace SSQE_Player
{
    class ModelManager : IDisposable
    {
        private static readonly List<int> VaOs = new List<int>();
        private static readonly List<int> VbOs = new List<int>();

        private readonly Dictionary<string, Model> models = new Dictionary<string, Model>();

        public void RegisterModel(string name, float[] vertexes, bool centered = false)
        {
            var model = LoadModelToVao(vertexes, centered);

            models.Add(name, model);
        }

        public Model GetModel(string name)
        {
            models.TryGetValue(name, out var model);

            return model;
        }

        public static Model LoadModelToVao(float[] vertexes, bool centered = false)
        {
            int vaoId = CreateVao();
            int buffer = StoreDataInAttribList(0, 3, vertexes);

            UnbindVao();

            return new Model(buffer, vertexes, vaoId, centered);
        }

        private static int CreateVao()
        {
            int vaoId = GL.GenVertexArray();

            VaOs.Add(vaoId);

            GL.BindVertexArray(vaoId);

            return vaoId;
        }

        private static void UnbindVao()
        {
            GL.BindVertexArray(0);
        }

        private static int StoreDataInAttribList(int attrib, int coordSize, float[] data)
        {
            int vboId = GL.GenBuffer();

            VbOs.Add(vboId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attrib, coordSize, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return vboId;
        }

        public void Dispose()
        {
            foreach (var vao in VaOs)
                GL.DeleteVertexArray(vao);
            foreach (var vbo in VbOs)
                GL.DeleteBuffer(vbo);
            foreach (var model in models.Values)
                model.Dispose();
        }
    }
}
