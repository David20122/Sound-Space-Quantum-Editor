using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SkiaSharp;
using System.IO;
using StbTrueTypeSharp;
using System.Linq;

namespace SSQE_Player
{
    internal unsafe class StbFont
    {
        // Standard ASCII range: 128 (why was the old editor's range 400)
        public static readonly int CharRange = 128;
        // Character size initially rendered to be scaled later
        // Greater values are smoother but take more memory
        private static readonly int OriginSize = 128;
        // Pixels between each character in rendered layout
        // Needs to be above 0 to ensure no ghost pixels appear while rendering
        // Recommended: 4
        private static readonly int CharSpacing = 4;

        private readonly int[] Extents;
        private readonly int[] Bearings;
        private readonly int[] YOffsets;
        private readonly SKBitmap[] Bitmaps;
        private readonly SKBitmap Bitmap;

        public Vector2 CharSize;
        public Vector4[] AtlasMetrics;
        public VertexArrayHandle VaO;
        public BufferHandle[] VbOs;
        public BufferHandle StaticVbO;

        private readonly int _baseline;
        private readonly TextureHandle _handle;

        public TextureHandle Handle => _handle;

        // Change unit to store multiple fonts without having to switch between handles while rendering
        // Otherwise extract the handle via StbFont.Handle and manage switching elsewhere
        public unsafe StbFont(string font, TextureUnit unit = TextureUnit.Texture15)
        {
            // Some font size discrepancies exist between FreeType and stb_truetype but the majority of the loader is the same

            YOffsets = new int[CharRange];
            Extents = new int[CharRange];
            Bearings = new int[CharRange];
            Bitmaps = new SKBitmap[CharRange];

            AtlasMetrics = new Vector4[CharRange];

            StbTrueType.stbtt_fontinfo fontInfo = StbTrueType.CreateFont(File.ReadAllBytes($"assets/fonts/{font}.ttf"), 0);
            float scale = StbTrueType.stbtt_ScaleForPixelHeight(fontInfo, OriginSize);

            int ascent, descent, lineGap;
            StbTrueType.stbtt_GetFontVMetrics(fontInfo, &ascent, &descent, &lineGap);

            // Render each character in the given range individually
            for (int i = 0; i < CharRange; i++)
            {
                int width, height, xoffset, yoffset;
                byte* glyph = StbTrueType.stbtt_GetCodepointBitmap(fontInfo, scale, scale, i, &width, &height, &xoffset, &yoffset);

                Bitmaps[i] = ConvertToSKBitmap(glyph, width, height);
                Extents[i] = width + xoffset;
                Bearings[i] = xoffset;

                if (width * height <= 0 && i == 32)
                    Extents[i] = OriginSize / 4;
                else
                    YOffsets[i] = yoffset;
            }

            _baseline = (int)(scale * ascent);

            int maxCharX = Extents.Max();
            int maxCharY = (int)(scale * (ascent - descent)) + YOffsets.Max();
            int px = maxCharX * maxCharY;
            CharSize = new(maxCharX, maxCharY);

            double texSize = Math.Sqrt(px * CharRange);
            int texX = (int)(texSize / maxCharX + 1) * (maxCharX + CharSpacing);
            int texY = (int)(texSize / maxCharY + 1) * (maxCharY + CharSpacing);

            SKImageInfo info = new(texX + 1, texY);
            SKSurface surface = SKSurface.Create(info);
            SKCanvas canvas = surface.Canvas;

            float currentX = 0;
            float currentY = 0;

            // Combine each character's bitmap on a main canvas to later store into memory
            int charsPerLine = (int)(info.Width / (CharSize.X + CharSpacing));
            float txW = CharSize.X / info.Width;
            float txH = CharSize.Y / info.Height;

            for (uint c = 0; c < CharRange; c++)
            {
                if (currentX + maxCharX > texX)
                {
                    currentX = 0;
                    currentY += maxCharY + CharSpacing;
                }

                if (Bitmaps[c].ByteCount > 0)
                    canvas.DrawBitmap(Bitmaps[c], currentX, currentY + _baseline + YOffsets[c]);
                currentX += maxCharX + CharSpacing;

                Bitmaps[c].Dispose();

                float txX = c % charsPerLine * (CharSize.X + CharSpacing);
                float txY = c / charsPerLine * (CharSize.Y + CharSpacing);

                AtlasMetrics[c] = (txX / info.Width, txY / info.Height, txW, txH);
            }

            _baseline -= (int)(scale * descent);

            Bitmap = SKBitmap.FromImage(surface.Snapshot());
            GC.KeepAlive(Bitmap);

            fontInfo.Dispose();

            // Store the font texture as a png in the current directory - for debugging
            /*
            using (SKImage image = surface.Snapshot())
            using (SKData imgData = image.Encode(SKEncodedImageFormat.Png, 80))
            using (FileStream stream = File.OpenWrite("font_texture.png"))
                imgData.SaveTo(stream);
            */

            canvas.Dispose();
            surface.Dispose();

            // Prep instance data in shader
            VbOs = new BufferHandle[2];

            VaO = GL.GenVertexArray();
            VbOs[0] = GL.GenBuffer();
            VbOs[1] = GL.GenBuffer();
            StaticVbO = GL.GenBuffer();

            float[] data = new float[12] {
                0, 0,
                CharSize.X, 0,
                0, CharSize.Y,

                CharSize.X, CharSize.Y,
                0, CharSize.Y,
                CharSize.X, 0
            };

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, StaticVbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.StaticDraw);

            GL.BindVertexArray(VaO);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbOs[0]);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribDivisor(1, 1);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbOs[1]);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 1 * sizeof(float), 0);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribDivisor(2, 1);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
            GL.BindVertexArray(VertexArrayHandle.Zero);

            // Load the texture into memory
            _handle = GL.GenTexture();

            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2d, _handle);

            GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, Bitmap.Width, Bitmap.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, Bitmap.GetPixels());

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        // Converts alpha bitmap to RGBa
        private static SKBitmap ConvertToSKBitmap(byte* bytes, int width, int height)
        {
            SKColor[] pixels = new SKColor[width * height];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int index = width * row + col;
                    byte currentByte = bytes[index];

                    SKColor color = new(255, 255, 255, currentByte);

                    pixels[index] = color;
                }
            }

            return new SKBitmap(width, height) { Pixels = pixels };
        }

        // Returns baseline of font scaled depending on font size
        public int Baseline(int fontSize)
        {
            float scale = fontSize / (float)OriginSize;

            return (int)(_baseline * scale);
        }

        // Returns width of string scaled depending on the font size
        public int Extent(string text, int fontSize)
        {
            float scale = fontSize / (float)OriginSize;
            string[] split = text.Split('\n');

            float maxX = 0;

            foreach (string line in split)
            {
                float currentX = 0;

                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if (c < 0 || c > CharRange)
                        continue;

                    if (currentX != 0)
                        currentX += Bearings[c];

                    currentX += Extents[c];
                }

                maxX = Math.Max(maxX, currentX);
            }

            return (int)(maxX * scale);
        }

        // Returns one vector4 per character with the necessary data to be passed to a corresponding shader for rendering
        // Formatted as x/y/scale/char
        public Vector4[] Print(float x, float y, string text, int fontSize)
        {
            Vector4[] verts = new Vector4[text.Replace("\n", "").Length];

            float scale = fontSize / (float)OriginSize;
            float cx = x;
            int vi = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    cx = x;
                    y += fontSize;
                    vi++;
                }
                else
                {
                    char c = text[i];
                    if (c < 0 || c > CharRange)
                        continue;

                    if (cx != x)
                        cx += Bearings[c] * scale;

                    verts[i - vi] = (cx, y, scale, c);

                    cx += Extents[c] * scale;
                }
            }

            return verts;
        }
    }
}
