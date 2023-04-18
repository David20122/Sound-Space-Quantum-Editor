using SSQE_Player.GUI;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SSQE_Player
{
    internal class FontRenderer
    {
        private static readonly FtFont main = new("main");

        public static Vector4[] Print(float x, float y, string text, int fontSize)
        {
            return main.Print(x, y, text, fontSize);
        }

        public static int GetWidth(string text, int fontSize)
        {
            return main.Extent(text, fontSize);
        }

        public static int GetHeight(int fontSize)
        {
            return main.Baseline(fontSize);
        }

        public static void Init()
        {
            GL.UseProgram(Shader.FontTexProgram);

            var location = GL.GetUniformLocation(Shader.FontTexProgram, "texture0");
            GL.Uniform1(location, 15);

            location = GL.GetUniformLocation(Shader.FontTexProgram, "TexLookup");
            GL.Uniform4(location, FtFont.CharRange, main.AtlasMetrics);

            location = GL.GetUniformLocation(Shader.FontTexProgram, "CharSize");
            GL.Uniform2(location, main.CharSize);
        }

        public static void SetActive()
        {
            GL.BindVertexArray(main.VaO);
        }

        public static void RenderData(Vector4[] data, float[]? alpha = null)
        {
            alpha ??= new float[data.Length];

            GL.BindBuffer(BufferTarget.ArrayBuffer, main.VbOs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, 4 * sizeof(float) * data.Length, data, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, main.VbOs[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, 1 * sizeof(float) * alpha.Length, alpha, BufferUsageHint.DynamicDraw);

            GL.BindVertexBuffer(0, main.StaticVbO, IntPtr.Zero, 2 * sizeof(float));
            GL.BindVertexBuffer(1, main.VbOs[0], IntPtr.Zero, 4 * sizeof(float));
            GL.BindVertexBuffer(2, main.VbOs[1], IntPtr.Zero, 1 * sizeof(float));

            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, data.Length);
        }
    }
}
