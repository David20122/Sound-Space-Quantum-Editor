using System;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor
{

    /// <summary>The font class for 2D text printing in a OpenGL scene.</summary>
    /// <remarks>Based on open-source Font3D class, e. g. presented in: http://www.gamedev.ru/code/forum/?id=192978 and adopted to C# and
    /// AltSketch.AltNETType library in: http://www.codeproject.com/Tips/668161/Rendering-AltNETType-equals-NET-FreeType-port-with.</remarks>
    public class FreeTypeFont
    {

        // The number of precompiled character.
        private static readonly int CharacterRange = 400;

        // ###############################################################################
        // ### A T T R I B U T E S
        // ###############################################################################

        #region Attributes

        private readonly int _listBase;
        private readonly int _fontSize;
        private readonly float _baseLine;
        public float BaseLine => _baseLine;

        /// <summary>The flaf, indicating a monospace font.</summary>
        private readonly bool _monospace;

        /// <summary>The glyph textures.</summary>
        private int[] _textures;

        /// <summary>The horizontal glyph extents.</summary>
        private int[] _extentsX;

        public bool IsCorrectlyLoaded
        {
            get
            {
                if (_extentsX == null) return false;
                if (_extentsX[0] == 0) return false;
                return true;
            }
        }

        public int FontSize => _fontSize;

        #endregion Attributes

        // ###############################################################################
        // ### C O N S T R U C T I O N   A N D   I N I T I A L I Z A T I O N
        // ###############################################################################

        #region Construction

        /// <summary>Initialize a new instance of the <see cref="FreeType.Font3D"/> class with font and size.</summary>
        /// <param name="font">The font file path.</param>
        /// <param name="size">The requested font size.</param>
        public FreeTypeFont(string font, int size)
        {
            if (sizeof(FtLong) != IntPtr.Size)
            {
                Console.WriteLine("Wrong compilation options!");
                if (IntPtr.Size == 4)
                    Console.WriteLine("Do not use 'X11_64' on 32 bit OS!");
                else
                    Console.WriteLine("Use 'X11_64' on 64 bit OS!");

                return;
            }

            // Save the size we need it later on when printing
            _fontSize = size;

            // We begin by creating a library pointer
            int ret = FtLibrary.FT_Init_FreeType(out IntPtr libptr);
            if (ret != 0)
                return;

            //Once we have the library we create and load the font face
            int retb = FtFace.FT_New_Face(libptr, font, 0, out IntPtr faceptr);
            if (retb != 0)
                return;

            var face = (FtFace)Marshal.PtrToStructure(faceptr, typeof(FtFace));
            if ((((int)face.face_flags) & ((int)((FtLong)FtFaceFlags.FtFaceFlagHorizontal))) !=
                (int)((FtLong)FtFaceFlags.FtFaceFlagHorizontal))
                Console.WriteLine("WARNING! The font '" + font + "' is not suitable for horizontal left-to-right text output.");

            if ((((int)face.face_flags) & ((int)((FtLong)FtFaceFlags.FtFaceFlagFixedWidth))) !=
                (int)((FtLong)FtFaceFlags.FtFaceFlagFixedWidth))
                _monospace = false;
            else
                _monospace = true;
            uint tmp = (((('u' << 8) | 'n') << 8 | 'i') << 8 | 'c');
            if (FtFace.FT_Select_CharMap(faceptr, tmp) != 0)
            {
                throw new Exception("Cannot set unicode");
            }

            // Freetype measures the font size in 1/64th of pixels for accuracy 
            // so we need to request characters in size*64
            FtFace.FT_Set_Char_Size(faceptr, size << 6, size << 6, 96, 96);

            // Provide a reasonably accurate estimate for expected pixel sizes
            // when we later on create the bitmaps for the font.
            FtFace.FT_Set_Pixel_Sizes(faceptr, size, size);



            FtSize ftSize = Marshal.PtrToStructure<FtSize>(face.size);
            _baseLine = ((int)ftSize.metrics.ascender + (int)ftSize.metrics.descender) / 64;// (int)ft_size.metrics.ascender/64;


            // Once we have the face loaded and sized, we generate opengl textures 
            // from the glyphs for each printable character.
            _textures = new int[CharacterRange];
            _extentsX = new int[CharacterRange];
            _listBase = GL.GenLists(CharacterRange);
            GL.GenTextures(CharacterRange, _textures);
            Console.WriteLine($"{font} {size}");
            for (int c = 0; c < CharacterRange; c++)
            {
                CompileCharacter(face, faceptr, c);
            }

            // Dispose of these as we don't need
            FtFace.FT_Done_Face(faceptr);
            FtLibrary.FT_Done_FreeType(libptr);
        }

        #endregion Construction	

        // ###############################################################################
        // ### D E S T R U C T I O N
        // ###############################################################################

        #region Destruction

        #endregion Destruction

        // ###############################################################################
        // ### P R O P E R T I E S
        // ###############################################################################

        #region Properties

        public void Dispose()
        {
            GL.DeleteLists(_listBase, CharacterRange);
            GL.DeleteTextures(CharacterRange, _textures);
            _textures = null;
            _extentsX = null;
        }

        #endregion Properties

        // ###############################################################################
        // ### M E T H O D S
        // ###############################################################################

        #region Methods

        /// <summary>Compile a single character to a list of GL commands, that draw the character's glyph bitmap.</summary>
        /// <param name="face">The font face, associated with the character to compile.</param>
        /// <param name="faceptr">The font face pointer, associated with the character to compile.</param>
        /// <param name="c">The character to compile to a list of GL commands, that draw the character's glyph bitmap.</param>
        /// <remarks>For details see: http://www.freetype.org/freetype2/docs/tutorial/step2.html</remarks>
        public void CompileCharacter(FtFace face, IntPtr faceptr, int c)
        {
            int result;
            // Convert the number index to a character index.
            int index = FtFace.FT_Get_Char_Index(faceptr, Convert.ToChar(c));

            // Load a single glyph (indicated by the index of the glyph in the font file) into the glyph slot of a face object.
            // The FT_LOAD_TYPES.FT_LOAD_DEFAULT controls:
            // - PRINARILY: Look for a bitmap of the glyph, corresponding to the face's current size (if the glyph of the current size
            //              has already been loaded and provided as bitmap), provide bitmap data to the glyph slot and return integer 0.
            // - ALTERNATIVELY: Look for a scalable outline, load it from the font file, scale it to device pixels, ‘hint’ it to the
            //                  pixel grid (in order to optimize it), provide outline data to the glyph slot and return integer 0.
            // - FALLBACK: Return integer != 0.
            result = FtFace.FT_Load_Glyph(faceptr, index, /*get metrics in 1/64th of pixels*/ FtLoadTypes.FtLoadDefault);
            if (result != 0)
                return;

            // Extract a glyph from the glyph slot. The 'face' is a managed mirror of the 'faceptr' and therefore it shares the pointer.
            // The FT_Load_Glyph() has already provided the glyph to 'faceptr.glyphrec', that can also be accessed via 'face.glyphrec'.
            // The created glyph object must be released with FT_Done_Glyph().
            result = Glyph.FT_Get_Glyph(face.glyphrec, out IntPtr glyphPointer);
            if (result != 0)
                return;

            // Convert a given glyph object to a bitmap glyph object.
            // The bitmap glyph object must be released with FT_Done_Glyph().
            result = Glyph.FT_Glyph_To_Bitmap(out glyphPointer,
                                               /*8-bit anti-aliased pixmap*/ FtRenderModes.FtRenderModeNormal,
                                               /*origin*/ new FtVector(0, 0),
                                               /*destroy non-bitmap glyph before replacing*/ (FtBool)1);
            if (result != 0)
                return;

            // Incorporate glyph bitmap structure into managed code.
            FtBitmapGlyph bitmapglyphStructure = (FtBitmapGlyph)Marshal.PtrToStructure(glyphPointer, typeof(FtBitmapGlyph));
            uint size = (bitmapglyphStructure.bitmap.width * bitmapglyphStructure.bitmap.rows);
            if (size <= 0)
            {
                _extentsX[c] = 0;
                // Space is a special `blank` character, that must be supported by a glyph bitmap.
                if (c == 32)
                {
                    GL.NewList((uint)(_listBase + c), ListMode.Compile);
                    GL.Translate(_fontSize >> 2, 0, 0);
                    _extentsX[c] = _fontSize >> 2;
                    GL.EndList();
                }
                return;
            }

            // Incorporate glyph bitmap bytes into managed code.
            byte[] currentGlyphBitmapBytes = new byte[size];
            uint glyphWidth = (uint)(((long)bitmapglyphStructure.root.advance.x) / 65536);
            Marshal.Copy(bitmapglyphStructure.bitmap.buffer, currentGlyphBitmapBytes, 0, currentGlyphBitmapBytes.Length);

            // Expand the 8bpp anti-aliased pixmap to 16bpp, because target texture format shall be PixelInternalFormat.Rgba
            // (which is 4bpp R, 4bpp G, 4bpp B and 4bpp A) and source pixel data should have the same bpp.
            // Since source pixel data are 8bpp gray-scale (interpreted as PixelFormat.Luminance) just double them to be
            // interpreted as PixelFormat.LuminanceAlpha, which is loassless gray-scale 16bpp (8bpp luminance/8bppalpha).
            uint expandedBitmapWidth = RoundUpToNextPowerOf2(bitmapglyphStructure.bitmap.width);
            uint expandedBitmapHeight = RoundUpToNextPowerOf2(bitmapglyphStructure.bitmap.rows);
            byte[] expandedBitmapBytes = new byte[2 * expandedBitmapWidth * expandedBitmapHeight];
            for (int countY = 0; countY < expandedBitmapHeight; countY++)
            {
                for (int countX = 0; countX < expandedBitmapWidth; countX++)
                {
                    // Since a glyph bitmap is 8bpp anti-aliased (gray-scale) pixmap, the new bitmap can assume luminance == alpha.
                    expandedBitmapBytes[2 * (countX + countY * expandedBitmapWidth)] =
                        expandedBitmapBytes[2 * (countX + countY * expandedBitmapWidth) + 1] =
                            (countX >= bitmapglyphStructure.bitmap.width || countY >= bitmapglyphStructure.bitmap.rows) ?
                            (byte)0 : currentGlyphBitmapBytes[countX + bitmapglyphStructure.bitmap.width * countY];
                }
            }

            // Set up some texture parameters for opengl.
            GL.BindTexture(TextureTarget.Texture2D, _textures[c]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            // Create the texture.
            GL.TexImage2D(TextureTarget.Texture2D, /*level-of-detail*/0, /*texture-format 32bit*/PixelInternalFormat.Rgba,
                           /*texture-width*/(int)expandedBitmapWidth, /*texture-height*/(int)expandedBitmapHeight, /*border*/0,
                           /*pixel-data-format*/PixelFormat.LuminanceAlpha, /*pixel-data-type*/PixelType.UnsignedByte, expandedBitmapBytes);
            expandedBitmapBytes = null;
            currentGlyphBitmapBytes = null;

            // ---------------------------------------------------------------------------
            //Create a display list (of precompiled GL commands) and bind a texture to it.
            GL.NewList((uint)(_listBase + c), ListMode.Compile);
            GL.BindTexture(TextureTarget.Texture2D, _textures[c]);

            // Account for freetype spacing rules.
            GL.Translate((int)bitmapglyphStructure.left, 0, 0);
            GL.PushMatrix();
            //var top = (int)bitmapglyphStructure.bitmap.rows - (int)bitmapglyphStructure.top;
            var height = bitmapglyphStructure.bitmap.rows;
            var top = (int)bitmapglyphStructure.top;
            GL.Translate(0, -top, 0);
            //GL.Translate (Math.Max (0.0, (glyphWidth - bitmapglyphStructure.bitmap.width) / 2.0), (int)bitmapglyphStructure.top - (int)bitmapglyphStructure.bitmap.rows, 0);
            float x = bitmapglyphStructure.bitmap.width / (float)expandedBitmapWidth;
            float y = bitmapglyphStructure.bitmap.rows / (float)expandedBitmapHeight;

            // Draw the quad.
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
            GL.TexCoord2(x, 0); GL.Vertex2(bitmapglyphStructure.bitmap.width, 0);
            GL.TexCoord2(x, y); GL.Vertex2(bitmapglyphStructure.bitmap.width, bitmapglyphStructure.bitmap.rows);
            GL.TexCoord2(0, y); GL.Vertex2(0, bitmapglyphStructure.bitmap.rows);
            GL.End();

            GL.PopMatrix();


            // Advance for the next character.
            if (!_monospace)
                GL.Translate(bitmapglyphStructure.bitmap.width, 0, 0);
            else
                GL.Translate(glyphWidth, 0, 0);
            GL.EndList();
            // ---------------------------------------------------------------------------
            _extentsX[c] = (int)bitmapglyphStructure.left + (int)bitmapglyphStructure.bitmap.width;

            // Clean up the bitmap glyph memory.
            Glyph.FT_Done_Glyph(glyphPointer);
        }

        /// <summary>Get the next equal or greater number, that is a power of 2.</summary>
        /// <param name="a">The number to get the next equal or greater number for, that is a power of 2.</param>
        /// <returns>The next equal or greater number, that is a power of 2.</returns>
        internal uint RoundUpToNextPowerOf2(uint a)
        {
            uint rval = 1;
            while (rval < a)
                rval <<= 1;
            return rval;
        }

        public int Extent(string text)
        {
            try
            {
                int ret = 0;
                for (int charIndex = 0; charIndex < text.Length; charIndex++)
                    ret += _extentsX[text[charIndex]];
                return ret;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>Push the projection matrix to the matrix stack and set parallel projection based on current
        /// viewport dimension, using an orthographic matrix.</summary>
        /// <returns>The viewport dimension.<see cref="System.Drawing.Size"/> </returns>
        internal Size PushProjectionMatrixAndSetParallelProjectionForViewport()
        {
            int[] viewport = new int[4];

            // Save the coefficients of the six user-definable clipping planes.
            GL.PushAttrib(AttribMask.TransformBit);

            // Determine the viewport coordinates.
            GL.GetInteger(GetPName.Viewport, viewport);

            // Save the projection matrix.
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();

            // Reset/initialize the projection matrix.
            GL.LoadIdentity();

            // Set the GDI-text-mode alike coordinate system based on the viewport coordinates.
            GL.Ortho(viewport[0], viewport[2], viewport[1], viewport[3], 0, 1);

            // Recall the coefficients of the six user-definable clipping planes.
            GL.PopAttrib();

            return new Size(viewport[2] - viewport[0], viewport[3] - viewport[1]);
        }

        internal void PopProjectionMatrix()
        {
            // Save the coefficients of the six user-definable clipping planes.
            GL.PushAttrib(AttribMask.TransformBit);

            // Recall the projection matrix.
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            // Recall the coefficients of the six user-definable clipping planes.
            GL.PopAttrib();
        }

        /// <summary>Print the indicated text (left to right) without any formatting at the indicated x and y coordinate.</summary>
        /// <param name="text">The text to print.</param>
        /// <param name="x">The x coordinate of the first character, given in a mathematical coordinate system.</param>
        /// <param name="y">The y coordinate of the bottom line, given in a mathematical coordinate system.</param>
        /// <remarks>Assume a mathematical coordinate system for x and y coordinate (origin at left bottom corner) with positive
        /// x-axis from left to right and positive y-axis from bottom to top.</remarks>
        public void PrintMathMap(string text, float x, float y)
        {
            if (string.IsNullOrEmpty(text))
                return;
            string[] lines = text.Split('\n');

            Size viewporSize = PushProjectionMatrixAndSetParallelProjectionForViewport();
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
                PrintWithCurrentProjection(lines[lineIndex], x, viewporSize.Height - y - _fontSize - _fontSize * lineIndex);
            PopProjectionMatrix();
        }

        /// <summary>Print the indicated text (left to right) without any formatting at the indicated x and y coordinate.</summary>
        /// <param name="text">The text to print.</param>
        /// <param name="x">The x coordinate of the first character, given in a GDI-text-mode alike coordinate system.</param>
        /// <param name="y">The y coordinate of the bottom line, given in a GDI-text-mode alike coordinate system.</param>
        /// <remarks>Assume a GDI-text-mode alike coordinate system for x and y coordinate (origin at left top corner) with positive
        /// x-axis from left to right and positive y-axis from top to bottom.</remarks>
        public void Print(string text, float x, float y)
        {
            if (string.IsNullOrEmpty(text))
                return;
            string[] lines = text.Split('\n');

            //PushProjectionMatrixAndSetParallelProjectionForViewport();
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
                PrintWithCurrentProjection(lines[lineIndex], x, y + _fontSize * lineIndex);
            //PopProjectionMatrix();
        }

        public void PrintWithCurrentProjection(string text, float x, float y)
        {
            int font = _listBase;

            GL.PushAttrib(AttribMask.ListBit | AttribMask.CurrentBit | AttribMask.EnableBit | AttribMask.TransformBit);
            GL.PushMatrix();
            // GL.LoadIdentity();
            GL.Translate(x, y + _baseLine, 0);
            //GL.Scale(-1, 1, 1);

            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);
            //GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ListBase(font);
            //float[] modelviewMatrix = new float[16];
            //GL.GetFloat(GetPName.ModelviewMatrix, modelviewMatrix);
            //GL.MultMatrix(modelviewMatrix);

            //Render
            int[] textbytes = new int[text.Length];
            for (int i = 0; i < text.Length; i++)
                textbytes[i] = text[i];
            GL.CallLists(text.Length, ListNameType.Int, textbytes);
            textbytes = null;

            //GL.End();
            //Restore openGL state
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.PopMatrix();
            GL.PopAttrib();
        }

        #endregion Methods

    }
}