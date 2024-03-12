using New_SSQE.GUI;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace New_SSQE
{
    internal class FontRenderer
    {
        public static readonly Dictionary<string, Tuple<TextureUnit, int>> FontIndex = new()
        {
            {"main", new Tuple<TextureUnit, int>(TextureUnit.Texture15, 15) },
            {"square", new Tuple<TextureUnit, int>(TextureUnit.Texture14, 14) },
            {"squareo", new Tuple<TextureUnit, int>(TextureUnit.Texture13, 13) }
        };
        private static readonly Dictionary<string, StbFont> fonts = new()
        {
            {"main", new StbFont("main", FontIndex["main"].Item1) },
            {"square", new StbFont("square", FontIndex["square"].Item1) },
            {"squareo", new StbFont("squareo", FontIndex["squareo"].Item1) }
        };

        public static Vector4[] Print(float x, float y, string text, int fontSize, string font)
        {
            return fonts[font].Print(x, y, text, fontSize);
        }

        public static int GetWidth(string text, int fontSize, string font)
        {
            return fonts[font].Extent(text, fontSize);
        }

        public static int GetHeight(int fontSize, string font)
        {
            return fonts[font].Baseline(fontSize);
        }

        public static void SetActive(string font)
        {
            var location = GL.GetUniformLocation(Shader.FontTexProgram, "texture0");
            GL.Uniform1i(location, FontIndex[font].Item2);

            location = GL.GetUniformLocation(Shader.FontTexProgram, "TexLookup");
            GL.Uniform4f(location, StbFont.CharRange, fonts[font].AtlasMetrics);

            location = GL.GetUniformLocation(Shader.FontTexProgram, "CharSize");
            GL.Uniform2f(location, fonts[font].CharSize);

            GL.BindVertexArray(fonts[font].VaO);
        }

        public static void RenderData(string font, Vector4[] data, float[]? alpha = null)
        {
            if (data.Length > 0)
            {
                alpha ??= new float[data.Length];

                GL.BindBuffer(BufferTargetARB.ArrayBuffer, fonts[font].VbOs[0]);
                GL.BufferData(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.DynamicDraw);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, fonts[font].VbOs[1]);
                GL.BufferData(BufferTargetARB.ArrayBuffer, alpha, BufferUsageARB.DynamicDraw);

                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, data.Length);
            }
        }

        public static string TrimText(string text, int fontSize, int width, string font = "main")
        {
            string end = "...";
            int endWidth = GetWidth(end, fontSize, font);

            if (GetWidth(text, fontSize, font) < width)
                return text;

            while (GetWidth(text, fontSize, font) >= width - endWidth)
                text = text[..^1];

            return text + end;
        }
    }
}
