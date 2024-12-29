using New_SSQE.GUI.Shaders;
using New_SSQE.Misc.Static;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;

namespace New_SSQE.GUI.Font
{
    internal class FontRenderer
    {
        public static bool unicode = true;

        public static readonly Dictionary<string, Tuple<TextureUnit, int>> FontIndex = new()
        {
            {"main", new Tuple<TextureUnit, int>(TextureUnit.Texture15, 15) },
            {"square", new Tuple<TextureUnit, int>(TextureUnit.Texture14, 14) },
            {"squareo", new Tuple<TextureUnit, int>(TextureUnit.Texture13, 13) },
        };
        private static readonly Dictionary<string, StbFont> fonts = new()
        {
            {"main", new StbFont("main", FontIndex["main"].Item1) },
            {"square", new StbFont("Square", FontIndex["square"].Item1) },
            {"squareo", new StbFont("Squareo", FontIndex["squareo"].Item1) },
        };

        public static VertexArrayHandle UnicodeVaO;
        public static BufferHandle UnicodeVbO0;
        public static BufferHandle UnicodeVbO1;
        public static BufferHandle UnicodeStaticVbO;

        private static readonly bool unicodeFont = StbFont.InitUnicode($"{Assets.FONTS}\\Unifont-P0.png", TextureUnit.Texture12);
        private static readonly int unicodeUnit = 12;

        public static Vector4[] Print(float x, float y, string text, int fontSize, string font)
        {
            return fonts[font].Print(x, y, text, fontSize, unicode);
        }

        public static void PrintInto(Vector4[] arr, int offset, float x, float y, string text, int fontSize, string font)
        {
            if (unicode)
                text = Translator.Translate(text);
            fonts[font].PrintInto(arr, offset, x, y, text, fontSize, unicode);
        }

        public static int GetWidth(string text, int fontSize, string font)
        {
            return fonts[font].Extent(text, fontSize, unicode);
        }

        public static int GetHeight(int fontSize, string font)
        {
            return fonts[font].Baseline(fontSize, unicode);
        }

        private static string _activeFont = "";

        public static void SetActive(string font)
        {
            //if (font == _activeFont)
                //return;
            _activeFont = font;

            if (unicode)
            {
                int location = GL.GetUniformLocation(Shader.UnicodeProgram, "texture0");
                GL.Uniform1i(location, unicodeUnit);

                location = GL.GetUniformLocation(Shader.UnicodeProgram, "CharSize");
                GL.Uniform2f(location, ((1 - 0.04f) / 256f, (1 - 0.04f) / 256f));

                GL.BindVertexArray(UnicodeVaO);
            }
            else
            {
                int location = GL.GetUniformLocation(Shader.FontProgram, "texture0");
                GL.Uniform1i(location, FontIndex[font].Item2);

                location = GL.GetUniformLocation(Shader.FontProgram, "TexLookup");
                GL.Uniform4f(location, StbFont.CharRange, fonts[font].AtlasMetrics);

                location = GL.GetUniformLocation(Shader.FontProgram, "CharSize");
                GL.Uniform2f(location, fonts[font].CharSize);

                GL.BindVertexArray(fonts[font].VaO);
            }
        }

        private static Color _activeColor = Color.White;

        public static void SetColor(Color color)
        {
            if (color == _activeColor)
                return;
            _activeColor = color;

            int location = GL.GetUniformLocation(unicode ? Shader.FontProgram : Shader.UnicodeProgram, "TexColor");
            GL.Uniform4f(location, color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public static void RenderData(string font, Vector4[] data, float[]? alpha = null, int? count = null)
        {
            if (data.Length > 0)
            {
                alpha ??= new float[data.Length];

                GL.BindBuffer(BufferTargetARB.ArrayBuffer, unicode ? UnicodeVbO0 : fonts[font].VbOs[0]);
                GL.BufferData(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.DynamicDraw);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, unicode ? UnicodeVbO1 : fonts[font].VbOs[1]);
                GL.BufferData(BufferTargetARB.ArrayBuffer, alpha, BufferUsageARB.DynamicDraw);

                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, count ?? data.Length);
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
