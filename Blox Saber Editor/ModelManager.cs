using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Sound_Space_Editor
{
    public class ModelManager
    {
        private static readonly List<int> VaOs = new List<int>();
        private static readonly List<int> VbOs = new List<int>();

        public static ModelRaw LoadModel2ToVao(float[] vertexes)
        {
            int vaoId = CreateVao();

            int buff0 = StoreDataInAttribList(0, 2, vertexes);

            UnbindVao();

            return new ModelRaw(vaoId, vertexes.Length / 2, buff0);
        }

        private static void OverrideDataInAttributeList(int id, int attrib, int coordSize, float[] data)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(attrib, coordSize, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(attrib, coordSize, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return vboId;
        }

        public static void Cleanup()
        {
            foreach (int vao in VaOs)
                GL.DeleteVertexArray(vao);

            foreach (int vbo in VbOs)
                GL.DeleteBuffer(vbo);
        }

        public static void DisposeOf(ModelRaw model)
        {
            VaOs.Remove(model.VaoID);

            GL.DeleteVertexArray(model.VaoID);

            foreach (int vbo in model.BufferIDs)
            {
                VbOs.Remove(vbo);

                GL.DeleteBuffer(vbo);
            }
        }
    }
}