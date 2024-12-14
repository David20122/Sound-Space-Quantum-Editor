using SSQE_Player.GUI;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SSQE_Player
{
    internal class FontRenderer
    {
        private static readonly StbFont main = new("main");

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

            int location = GL.GetUniformLocation(Shader.FontTexProgram, "texture0");
            GL.Uniform1i(location, 15);

            location = GL.GetUniformLocation(Shader.FontTexProgram, "TexLookup");
            GL.Uniform4f(location, StbFont.CharRange, main.AtlasMetrics);

            location = GL.GetUniformLocation(Shader.FontTexProgram, "CharSize");
            GL.Uniform2f(location, main.CharSize);
        }

        public static void SetActive()
        {
            // why does discord streaming unbind the texture
            GL.BindTexture(TextureTarget.Texture2d, main.Handle);

            GL.BindVertexArray(main.VaO);
        }

        public static void RenderData(Vector4[] data, float[]? alpha = null)
        {
            alpha ??= new float[data.Length];

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, main.VbOs[0]);
            GL.BufferData(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.DynamicDraw);
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, main.VbOs[1]);
            GL.BufferData(BufferTargetARB.ArrayBuffer, alpha, BufferUsageARB.DynamicDraw);

            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, data.Length);
        }
    }
}
