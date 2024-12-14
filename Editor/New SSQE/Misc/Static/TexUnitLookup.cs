using OpenTK.Graphics.OpenGL;

namespace New_SSQE.Misc.Static
{
    internal class TexUnitLookup
    {
        private static readonly TextureUnit[] units =
        {
            TextureUnit.Texture0,
            TextureUnit.Texture1,
            TextureUnit.Texture2,
            TextureUnit.Texture3,
            TextureUnit.Texture4,
            TextureUnit.Texture5,
            TextureUnit.Texture6,
            TextureUnit.Texture7,
            TextureUnit.Texture8,
            TextureUnit.Texture9,
            TextureUnit.Texture10,
            TextureUnit.Texture11,
            TextureUnit.Texture12,
            TextureUnit.Texture13,
            TextureUnit.Texture14,
            TextureUnit.Texture15,
        };

        public static TextureUnit Get(int index)
        {
            return units[index];
        }
    }
}
