﻿using New_SSQE.GUI;
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
        private static readonly Dictionary<string, FtFont> fonts = new()
        {
            {"main", new FtFont("main", FontIndex["main"].Item1) },
            {"square", new FtFont("square", FontIndex["square"].Item1) },
            {"squareo", new FtFont("squareo", FontIndex["squareo"].Item1) }
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
            GL.Uniform1(location, FontIndex[font].Item2);

            location = GL.GetUniformLocation(Shader.FontTexProgram, "TexLookup");
            GL.Uniform4(location, FtFont.CharRange, fonts[font].AtlasMetrics);

            location = GL.GetUniformLocation(Shader.FontTexProgram, "CharSize");
            GL.Uniform2(location, fonts[font].CharSize);

            GL.BindVertexArray(fonts[font].VaO);
        }

        public static void RenderData(string font, Vector4[] data, float[]? alpha = null)
        {
            alpha ??= new float[data.Length];

            GL.BindBuffer(BufferTarget.ArrayBuffer, fonts[font].VbOs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, 4 * sizeof(float) * data.Length, data, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, fonts[font].VbOs[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, 1 * sizeof(float) * alpha.Length, alpha, BufferUsageHint.DynamicDraw);

            GL.BindVertexBuffer(0, fonts[font].StaticVbO, IntPtr.Zero, 2 * sizeof(float));
            GL.BindVertexBuffer(1, fonts[font].VbOs[0], IntPtr.Zero, 4 * sizeof(float));
            GL.BindVertexBuffer(2, fonts[font].VbOs[1], IntPtr.Zero, 1 * sizeof(float));

            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, data.Length);
        }
    }
}
