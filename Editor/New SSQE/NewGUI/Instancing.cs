using OpenTK.Graphics;
using OpenTK.Mathematics;

namespace New_SSQE.NewGUI
{
    internal class Instance : IDisposable
    {
        private ProgramHandle program;
        private BufferHandle staticVBO;
        private VertexArrayHandle staticVAO;
        private int vertexCount;

        private BufferHandle vbo;
        private BufferHandle vbo_2;
        private int instanceCount;

        private bool hasSecondary;

        public Instance(ProgramHandle program, bool hasSecondary = false)
        {
            this.program = program;
            this.hasSecondary = hasSecondary;

            (staticVAO, staticVBO) = GLState.NewVAO_VBO(2, 4);

            vbo = GLState.ExtendInstancingVAO(staticVAO, 2, 4);
            if (hasSecondary)
                vbo_2 = GLState.ExtendInstancingVAO(staticVAO, 3, 3);
        }

        public void UploadStaticData(float[] verts)
        {
            GLState.BufferData(staticVBO, verts);
            vertexCount = verts.Length / 6;
        }

        public void UploadData(Vector4[] primary, Vector3[]? secondary = null)
        {
            GLState.BufferData(vbo, primary);

            if (hasSecondary && secondary != null)
                GLState.BufferData(vbo_2, secondary);

            instanceCount = primary.Length;
        }

        public void Render()
        {
            GLState.EnableProgram(program);
            GLState.DrawInstances(staticVAO, 0, vertexCount, instanceCount);
        }

        public void Dispose()
        {
            GLState.Clean(staticVBO);
            GLState.Clean(staticVAO);

            GLState.Clean(vbo);
            GLState.Clean(vbo_2);
        }
    }

    internal static class Instancing
    {
        private static readonly Dictionary<string, Instance> instances = new();

        public static Instance Generate(string key, ProgramHandle program)
        {
            if (!instances.TryGetValue(key, out Instance? instance))
                instances.Add(key, new(program));

            return instance ?? instances[key];
        }

        public static void UploadStaticData(string key, float[] verts)
        {
            if (instances.TryGetValue(key, out Instance? instance))
                instance.UploadStaticData(verts);
        }

        public static void UploadData(string key, Vector4[] primary, Vector3[]? secondary = null)
        {
            if (instances.TryGetValue(key, out Instance? instance))
                instance.UploadData(primary, secondary);
        }

        public static void Render(params string[] keys)
        {
            foreach (string key in keys)
            {
                if (instances.TryGetValue(key, out Instance? instance))
                    instance.Render();
            }
        }

        public static void Dispose()
        {
            foreach (Instance value in instances.Values)
                value.Dispose();
            instances.Clear();
        }
    }
}
