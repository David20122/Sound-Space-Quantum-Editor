using Avalonia.OpenGL;
using New_SSQE.GUI.Shaders;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace New_SSQE.NewGUI
{
    internal static class GLState
    {
        private static ProgramHandle? _program;
        private static TextureUnit? _texUnit;
        private static TextureHandle? _texture;
        private static VertexArrayHandle? _vao;
        private static BufferHandle? _vbo;

        public static void EnableProgram(ProgramHandle program)
        {
            if (program != _program)
                GL.UseProgram(program);
            _program = program;
        }

        public static void EnableTextureUnit(ProgramHandle program, TextureUnit texUnit)
        {
            if (texUnit != _texUnit)
            {
                EnableProgram(program);
                GL.ActiveTexture(texUnit);

                EnableProgram(Shader.TextureProgram);
                int location = GL.GetUniformLocation(Shader.TextureProgram, "texture0");
                GL.Uniform1i(location, (int)texUnit - (int)TextureUnit.Texture0);
            }

            _texUnit = texUnit;
        }

        public static void EnableTexture(TextureHandle texture)
        {
            if (texture != _texture)
                GL.BindTexture(TextureTarget.Texture2d, texture);
            _texture = texture;
        }
        public static void EnableVAO(VertexArrayHandle vao)
        {
            if (vao != _vao)
                GL.BindVertexArray(vao);
            _vao = vao;
        }

        public static void EnableVBO(BufferHandle vbo)
        {
            if (vbo != _vbo)
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            _vbo = vbo;
        }

        public static void BufferData(BufferHandle vbo, float[] data)
        {
            EnableVBO(vbo);
            GL.BufferData(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.StaticDraw);
        }

        public static void BufferData(BufferHandle vbo, Vector4[] data)
        {
            EnableVBO(vbo);
            GL.BufferData(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.StaticDraw);
        }

        public static void BufferData(BufferHandle vbo, Vector3[] data)
        {
            EnableVBO(vbo);
            GL.BufferData(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.StaticDraw);
        }

        public static (VertexArrayHandle, BufferHandle) NewVAO_VBO(params int[] fieldWidths)
        {
            VertexArrayHandle vao = GL.GenVertexArray();
            EnableVAO(vao);

            BufferHandle vbo = GL.GenBuffer();
            EnableVBO(vbo);

            int stride = fieldWidths.Sum();
            int offset = 0;

            for (int i = 0; i < fieldWidths.Length; i++)
            {
                GL.VertexAttribPointer((uint)i, fieldWidths[i], VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
                GL.EnableVertexAttribArray((uint)i);
                
                offset += i;
            }

            DisableVAO_VBO();

            return (vao, vbo);
        }

        public static BufferHandle ExtendInstancingVAO(VertexArrayHandle vao, int location, int width)
        {
            EnableVAO(vao);

            BufferHandle vbo = GL.GenBuffer();
            EnableVBO(vbo);

            GL.VertexAttribPointer((uint)location, width, VertexAttribPointerType.Float, false, width * sizeof(float), 0);
            GL.EnableVertexAttribArray((uint)location);
            GL.VertexAttribDivisor((uint)location, 1);

            DisableVAO_VBO();

            return vbo;
        }

        public static void DisableVAO_VBO()
        {
            EnableVAO(VertexArrayHandle.Zero);
            EnableVBO(BufferHandle.Zero);
        }

        public static void LoadTexture(TextureHandle texture, int width, int height, nint pixels, TextureUnit texUnit)
        {
            EnableTextureUnit(Shader.TextureProgram, texUnit);
            EnableTexture(texture);

            GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, width, height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
        }

        public static TextureHandle NewTexture(TextureUnit texUnit)
        {
            TextureHandle texture = GL.GenTexture();
            EnableTextureUnit(Shader.TextureProgram, texUnit);
            EnableTexture(texture);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            return texture;
        }

        public static TextureHandle NewTexture(int width, int height, nint pixels, TextureUnit texUnit)
        {
            TextureHandle texture = NewTexture(texUnit);
            LoadTexture(texture, width, height, pixels, texUnit);

            return texture;
        }

        public static void DrawTriangles(int first, int count) => GL.DrawArrays(PrimitiveType.Triangles, first, count);

        public static void DrawTriangles(VertexArrayHandle vao, int first, int count)
        {
            EnableVAO(vao);
            DrawTriangles(first, count);
        }

        public static void DrawInstances(int first, int count, int instances) => GL.DrawArraysInstanced(PrimitiveType.Triangles, first, count, instances);

        public static void DrawInstances(VertexArrayHandle vao, int first, int count, int instances)
        {
            EnableVAO(vao);
            DrawInstances(first, count, instances);
        }

        public static void Clean(VertexArrayHandle vao) => GL.DeleteVertexArray(vao);
        public static void Clean(BufferHandle vbo) => GL.DeleteBuffer(vbo);
        public static void Clean(TextureHandle texture) => GL.DeleteTexture(texture);
    }
}
