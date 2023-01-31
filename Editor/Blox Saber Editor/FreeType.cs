using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Sound_Space_Editor
{

#if (false)
	public enum FT_String		: sbyte		{	};  // X11 64/32 Bit: 1 Byte:                  -127 to 127
	public enum FT_Char			: sbyte		{	};  // X11 64/32 Bit: 1 Byte:                  -127 to 127
	public enum FT_Uchar		: byte		{	};  // X11 64/32 Bit: 1 Byte:                     0 to 255
	public enum FT_Bool			: byte		{	};  // X11 64/32 Bit: 1 Byte:                     0 to 255

	public enum FT_Short		: short		{	};  // X11 64/32 Bit: 2 Byte:                -32767 to 32767
	public enum FT_UShort		: ushort	{	};  // X11 64/32 Bit: 2 Byte:                     0 to 65535
	public enum FT_Int			: int		{	};  // X11 64/32 Bit: 4 Bytes:       -2.147.483.648 to 2.147.483.647
	public enum FT_Uint			: uint		{	};  // X11 64/32 Bit: 4 Bytes:                    0 to 4294967295

	public enum FT_Long			: long		{	};  // X11 64    Bit: 8 Bytes: -9223372036854775807 to 9223372036854775807
	public enum FT_Ulong		: ulong		{	};  // X11 64    Bit: 8 Bytes:                    0 to 18446744073709551615

	public enum FT_Pos			: long		{	};  // X11 64    Bit: 8 Bytes: -9223372036854775807 to 9223372036854775807
#else
    public enum FtString : sbyte { }  // X11 64/32 Bit: 1 Byte:                  -127 to 127
    public enum FtChar : sbyte { }  // X11 64/32 Bit: 1 Byte:                  -127 to 127
    public enum FtUchar : byte { }  // X11 64/32 Bit: 1 Byte:                     0 to 255
    public enum FtBool : byte { }  // X11 64/32 Bit: 1 Byte:                     0 to 255

    public enum FtFixed
    { }
    public enum FtShort : short { }  // X11 64/32 Bit: 2 Byte:                -32767 to 32767
    public enum FtUShort : ushort { }  // X11 64/32 Bit: 2 Byte:                     0 to 65535
    public enum FtInt
    { }  // X11 64/32 Bit: 4 Bytes:       -2.147.483.648 to 2.147.483.647
    public enum FtUint : uint { }  // X11 64/32 Bit: 4 Bytes:                    0 to 4294967295

    public enum FtLong
    { }  // X11    32 Bit: 4 Bytes:       -2.147.483.648 to 2.147.483.647
    public enum FtUlong : uint { }  // X11    32 Bit: 4 Bytes:                    0 to 4294967295

    public enum FtPos
    { }  // X11    32 Bit: 4 Bytes:       -2.147.483.648 to 2.147.483.647
#endif

    public class Ft
    {
        public const string Lib = "freetype";//"libfreetype.so"; // freetype.dll

        public static FtInt FT_IMAGE_TAG(char b3, char b2, char b1, char b0)
        {
            return (FtInt)(((byte)b3) * 16777216 + ((byte)b2) * 65536 + ((byte)b1) * 256 + ((byte)b0));
        }

        public static FtInt FtGlyphFormatNone = FT_IMAGE_TAG((char)0, (char)0, (char)0, (char)0);
        public static FtInt FtGlyphFormatComposite = FT_IMAGE_TAG('c', 'o', 'm', 'p');
        public static FtInt FtGlyphFormatBitmap = FT_IMAGE_TAG('b', 'i', 't', 's');
        public static FtInt FtGlyphFormatOutline = FT_IMAGE_TAG('o', 'u', 't', 'l');
        public static FtInt FtGlyphFormatPlotter = FT_IMAGE_TAG('p', 'l', 'o', 't');
    }

    public enum FtLoadTypes
    {
        FtLoadDefault = 0,
        FtLoadNoScale = 1
    }

    public enum FtRenderModes
    {
        FtRenderModeNormal = 0,
        FtRenderModeLight = 1
    }

    public enum FtGlyphFormat // FT_Int
    {
        FtGlyphFormatNone = 0,               // must be equal to FT.FT_GLYPH_FORMAT_NONE.
        FtGlyphFormatComposite = 1668246896,     // must be equal to FT.FT_GLYPH_FORMAT_COMPOSITE.
        FtGlyphFormatBitmap = 1651078259,        // must be equal to FT.FT_GLYPH_FORMAT_BITMAP.
        FtGlyphFormatOutline = 1869968492,       // must be equal to FT.FT_GLYPH_FORMAT_OUTLINE.
        FtGlyphFormatPlotter = 1886154612        // must be equal to FT.FT_GLYPH_FORMAT_PLOTTER.
    }

    public enum FtFaceFlags
    {
        FtFaceFlagScalable = (1 << 0),   // Indicates that the face contains outline glyphs.
        FtFaceFlagFixedSizes = (1 << 1),    // Indicates that the face contains bitmap strikes.
        FtFaceFlagFixedWidth = (1 << 2),    // Indicates that the face contains fixed-width characters (monospace).
        FtFaceFlagSfnt = (1 << 3),   // Indicates that the face uses the ‘sfnt’ storage scheme.
        FtFaceFlagHorizontal = (1 << 4), // Indicates that the face contains horizontal glyph metrics.
        FtFaceFlagVertical = (1 << 5),   // Indicates that the face contains vertical glyph metrics.
        FtFaceFlagKerning = (1 << 6),    // Indicates that the face contains kerning information.
        FtFaceFlagFastGlyphs = (1 << 7),    // DEPRECATED.
        FtFaceFlagMultipleMasters = (1 << 8),
        FtFaceFlagGlyphNames = (1 << 9),
        FtFaceFlagExternalStream = (1 << 10),
        FtFaceFlagHinter = (1 << 11),
        FtFaceFlagCidKeyed = (1 << 12),
        FtFaceFlagTricky = (1 << 13),
        FtFaceFlagColor = (1 << 14)
    }

    public enum FtStyleFlags
    {
        FtStyleFlagItalic = (1 << 0),
        FtStyleFlagBold = (1 << 1)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtLibrary
    {
        public IntPtr memory;
        public FtGeneric generic;
        public int major;
        public int minor;
        public int patch;
        public uint modules;
        public IntPtr module0, module1, module2, module3, module4, module5, module6, module7, module8, module9, module10;
        public IntPtr module11, module12, module13, module14, module15, module16, module17, module18, module19, module20;
        public IntPtr module21, module22, module23, module24, module25, module26, module27, module28, module29, module30;
        public IntPtr module31;
        public FtListRec renderers;
        public IntPtr renderer;
        public IntPtr auto_hinter;
        public IntPtr raster_pool;
        public long raster_pool_size;
        public IntPtr debug0, debug1, debug2, debug3;

        [DllImport(Ft.Lib, EntryPoint = "FT_Init_FreeType", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern int FT_Init_FreeType(out IntPtr lib);

        [DllImport(Ft.Lib, EntryPoint = "FT_Done_FreeType", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void FT_Done_FreeType(IntPtr lib);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtGeneric
    {
        public IntPtr data;
        public IntPtr finalizer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtBBox
    {
        public FtPos xMin, yMin;
        public FtPos xMax, yMax;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtListRec
    {
        public IntPtr head;
        public IntPtr tail;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtVector
    {
        public FtPos x;
        public FtPos y;

        public FtVector(int vX, int vY)
        {
            x = (FtPos)vX;
            y = (FtPos)vY;
        }

        public FtVector(long vX, long vY)
        {
            x = (FtPos)vX;
            y = (FtPos)vY;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtFace
    {
        public FtLong num_faces;
        public FtLong face_index;

        public FtLong face_flags;          /* See: FT_FACE_FLAGS */
        public FtLong style_flags;     /* See: FT_STYLE_FLAGS */

        public FtLong num_glyphs;

        [MarshalAs(UnmanagedType.LPStr)] public string family_name;     /* FT_String[] */
        [MarshalAs(UnmanagedType.LPStr)] public string style_name;          /* FT_String[] */

        public FtInt num_fixed_sizes;
        public IntPtr available_sizes;  /* FT_Bitmap_Size* */

        public FtInt num_charmaps;
        public IntPtr charmaps;         /* FT_CharMap* */

        public FtGeneric generic;          /* FT_Generic */

        /*# The following are only relevant to scalable outlines. */
        public FtBBox box;
        public FtUShort units_per_EM;
        public FtShort ascender;           // The vertical distance from the horizontal baseline to the highest ‘character’ coordinate in a font face, measured in 26.6 format (* 64) before pixel size has been applied.
        public FtShort descender;          // The vertical distance from the horizontal baseline to the lowest ‘character’ coordinate in a font face, measured in 26.6 format (* 64) before pixel size has been applied.
        public FtShort height;             // The default line spacing (i.e., the baseline-to-baseline distance) when writing text with this font, measured in 26.6 format (* 64) before pixel size has been applied.

        public FtShort max_advance_width;
        public FtShort max_advance_height;

        public FtShort underline_position;
        public FtShort underline_tickness;

        public /* FT_GlyphSlot */		IntPtr glyphrec;
        public IntPtr size;
        //public FT_Size*	size;
        public /* FT_CharMap */			IntPtr charmap;

        /*@private begin */
        public /* FT_Driver */			IntPtr driver;
        public /* FT_Memory */			IntPtr memory;
        public /* FT_Stream */			IntPtr stream;

        public FtListRec sizes_list;

        public FtGeneric autohint;
        public /* void* */				IntPtr extensions;

        public /* FT_Face_Internal */	IntPtr internal_face;

        /*@private end */

        [DllImport(Ft.Lib, EntryPoint = "FT_New_Face", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern int FT_New_Face(IntPtr lib, string fname, int index, out IntPtr face);

        [DllImport(Ft.Lib, EntryPoint = "FT_Set_Char_Size", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void FT_Set_Char_Size(IntPtr face, int width, int height, int horzResolution, int vertResolution);

        [DllImport(Ft.Lib, EntryPoint = "FT_Set_Pixel_Sizes", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void FT_Set_Pixel_Sizes(IntPtr face, int pixelWidth, int pixelHeight);

        [DllImport(Ft.Lib, EntryPoint = "FT_Done_Face", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void FT_Done_Face(IntPtr face);

        [DllImport(Ft.Lib, EntryPoint = "FT_Get_Char_Index", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern int FT_Get_Char_Index(IntPtr face, uint c);

        [DllImport(Ft.Lib, EntryPoint = "FT_Load_Glyph", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern int FT_Load_Glyph(IntPtr face, int index, FtLoadTypes flags);
        [DllImport(Ft.Lib, EntryPoint = "FT_Select_Charmap", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern int FT_Select_CharMap(IntPtr face, uint encoding);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtGlyphRec
    {
        public IntPtr library;
        public IntPtr clazz;
        public FtInt format;
        public FtVector advance;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtGlyphClass
    {
        public FtLong size;
        public FtGlyphFormat format;
        public IntPtr init;
        public IntPtr done;
        public IntPtr copy;
        public IntPtr transform;
        public IntPtr bbox;
        public IntPtr prepare;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtBitmapGlyph
    {
        public FtGlyphRec root;
        public FtInt left;     // The left-side bearing, i.e., the horizontal distance from the current pen position to the left border of the glyph bitmap.
        public FtInt top;      // The top-side bearing, i.e., the vertical distance from the current pen position to the top border of the glyph bitmap. This distance is positive for upwards y! (Distance from first pixel to base-line.)
        public FtBitmap bitmap;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtBitmap
    {
        public uint rows;
        public uint width;
        public int pitch;
        public IntPtr buffer;
        public ushort num_grays;
        public byte pixel_mode;
        public byte palette_mode;
        public IntPtr palette;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FtSize
    {
        public IntPtr face;      /* parent face object              */
        public FtGeneric generic;   /* generic pointer for client uses */
        public FtSizeMetrics metrics;   /* size metrics                    */
        public IntPtr @internal;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FtSizeMetrics
    {
        public FtUShort x_ppem;      /* horizontal pixels per EM               */
        public FtUShort y_ppem;      /* vertical pixels per EM                 */

        public FtFixed x_scale;     /* scaling values used to convert font    */
        public FtFixed y_scale;     /* units to 26.6 fractional pixels        */
        
        public FtPos ascender;    /* ascender in 26.6 frac. pixels          */
        public FtPos descender;   /* descender in 26.6 frac. pixels         */
        public FtPos height;      /* text height in 26.6 frac. pixels       */
        public FtPos max_advance; /* max horizontal advance, in 26.6 pixels */
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Glyph
    {
        [DllImport(Ft.Lib, EntryPoint = "FT_Get_Glyph", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern int FT_Get_Glyph(IntPtr glyphrec, out IntPtr glyph);

        [DllImport(Ft.Lib, EntryPoint = "FT_Glyph_To_Bitmap", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern int FT_Glyph_To_Bitmap(out IntPtr glyph, FtRenderModes renderMode, FtVector origin, FtBool destroy);

        [DllImport(Ft.Lib, EntryPoint = "FT_Done_Glyph", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void FT_Done_Glyph(IntPtr glyph);
    }

}