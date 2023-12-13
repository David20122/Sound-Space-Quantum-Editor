using System;
using System.Linq;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpFont;
using SkiaSharp;

namespace New_SSQE
{
    internal class FtFont
    {
        // Standard ASCII range: 128 (why was the old editor's range 400)
        public static readonly int CharRange = 128;
        // Character size initially rendered to be scaled later
        // Greater values are smoother but take more memory
        private static readonly int OriginSize = 128;
        // Pixels between each character in rendered layout, needs to be above 0 to ensure no ghost pixels appear while rendering
        // Recommended: 4
        private static readonly int CharSpacing = 4;

        private readonly int[] Extents;
        private readonly int[] Bearings;
        private readonly int[] YOffsets;
        private readonly SKBitmap Bitmap;
        
        public Vector2 CharSize;
        public Vector4[] AtlasMetrics;
        public VertexArrayHandle VaO;
        public BufferHandle[] VbOs;
        public BufferHandle StaticVbO;

        private readonly int BmpWidth;
        private readonly int BmpHeight;

        private readonly int _baseline;
        private readonly TextureHandle _handle;

        public TextureHandle Handle => _handle;

        // Change unit to store multiple fonts without having to switch between handles while rendering
        // Otherwise extract the handle via FTFont.Handle and manage switching elsewhere
        public FtFont(string font, TextureUnit unit = TextureUnit.Texture15)
        {
            Library library = new();
            Face face = new(library, $"assets/fonts/{font}.ttf");

            SKBitmap[] Bitmaps;

            face.SetCharSize(OriginSize, OriginSize, 96, 96);
            face.SetPixelSizes((uint)OriginSize, (uint)OriginSize);

            Bitmaps = new SKBitmap[CharRange];
            Extents = new int[CharRange];
            Bearings = new int[CharRange];
            YOffsets = new int[CharRange];

            // Render each character in the given range individually and store its metrics in various arrays
            for (uint c = 0; c < CharRange; c++)
            {
                var index = face.GetCharIndex(c);

                face.LoadGlyph(index, LoadFlags.Render | LoadFlags.Default, LoadTarget.Normal);

                FTBitmap glyph = face.Glyph.Bitmap;
                int size = glyph.Width * glyph.Rows;

                Bitmaps[c] = ConvertToSKBitmap(glyph);
                Extents[c] = face.Glyph.Bitmap.Width + face.Glyph.BitmapLeft;
                Bearings[c] = face.Glyph.BitmapLeft;

                if (size <= 0 && c == 32)
                    Extents[c] = OriginSize / 4;
                else
                    YOffsets[c] = face.Glyph.BitmapTop;
            }

            _baseline = (int)(face.Size.Metrics.Ascender - face.Size.Metrics.Descender / 2);

            var maxCharX = Extents.Max();
            var maxCharY = face.Glyph.Metrics.VerticalAdvance + YOffsets.Max();
            int px = (int)(maxCharX * maxCharY);

            var texSize = Math.Sqrt(px * CharRange);
            var texX = (int)(texSize / maxCharX + 1) * (maxCharX + CharSpacing);
            var texY = (int)(texSize / maxCharY + 1) * ((int)maxCharY + CharSpacing);

            var info = new SKImageInfo(texX + 1, texY);
            var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            float currentX = 0;
            float currentY = YOffsets.Max();

            // Combine each character's bitmap on a main canvas to later store into memory
            for (uint c = 0; c < CharRange; c++)
            {
                if (currentX + maxCharX > texX)
                {
                    currentX = 0;
                    currentY += (int)maxCharY + CharSpacing;
                }

                if (Bitmaps[c].ByteCount > 0)
                    canvas.DrawBitmap(Bitmaps[c], currentX, currentY - YOffsets[c]);
                currentX += maxCharX + CharSpacing;

                Bitmaps[c].Dispose();
            }

            Bitmap = SKBitmap.FromImage(surface.Snapshot());
            GC.KeepAlive(Bitmap);
            CharSize = new(maxCharX, (int)maxCharY);

            BmpWidth = Bitmap.Width;
            BmpHeight = Bitmap.Height;

            // put font texture metrics into array for uploading to a font shader
            AtlasMetrics = new Vector4[CharRange];

            int charsPerLine = (int)(BmpWidth / (CharSize.X + CharSpacing));
            float txW = CharSize.X / BmpWidth;
            float txH = CharSize.Y / BmpHeight;

            for (int i = 0; i < CharRange; i++)
            {
                float txX = (i % charsPerLine) * (CharSize.X + CharSpacing);
                float txY = (i / charsPerLine) * (CharSize.Y + CharSpacing);

                AtlasMetrics[i] = (txX / BmpWidth, txY / BmpHeight, txW, txH);
            }

            // prep instance data in shader
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

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribFormat(0, 2, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(0, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribFormat(1, 4, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(1, 1);
            GL.VertexBindingDivisor(1, 1);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribFormat(2, 1, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(2, 2);
            GL.VertexBindingDivisor(2, 1);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
            GL.BindVertexArray(VertexArrayHandle.Zero);


            // Store the font texture as a png in the current directory - for debugging
            /*
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
            using (var stream = File.OpenWrite("font_texture.png"))
                data.SaveTo(stream);
            */


            face.Dispose();
            library.Dispose();


            // Load the texture into memory
            _handle = GL.GenTexture();

            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2d, _handle);

            GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, BmpWidth, BmpHeight, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, Bitmap.GetPixels());

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            canvas.Dispose();
            surface.Dispose();
        }

        // Converts alpha map to RGBa
        private static SKBitmap ConvertToSKBitmap(FTBitmap bitmap)
        {
            byte[] bytes = bitmap.Buffer != IntPtr.Zero ? bitmap.BufferData : Array.Empty<byte>();
            int width = bitmap.Pitch, height = bitmap.Rows;

            var pixels = new SKColor[width * height];

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

            return new SKBitmap(bitmap.Pitch, bitmap.Rows) { Pixels = pixels };
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

            foreach (var line in split)
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
