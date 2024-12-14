using SkiaSharp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using New_SSQE.GUI.Shaders;
using New_SSQE.Misc.Static;
using New_SSQE.ExternalUtils;

namespace New_SSQE.GUI
{
    internal class TextureManager
    {
        private static readonly Dictionary<string, Tuple<TextureHandle, SKBitmap>> Textures = new();

        public static TextureHandle GetOrRegister(string textureName, SKBitmap? img = null, bool smooth = false, TextureUnit unit = TextureUnit.Texture0)
        {
            TextureHandle id;

            if (!Textures.TryGetValue(textureName, out Tuple<TextureHandle, SKBitmap>? value))
            {
                if (img == null)
                {
                    string file = $"{Assets.TEXTURES}\\{textureName}.png";

                    if (!File.Exists(file))
                    {
                        Logging.Register($"Failed to register texture: [{textureName}] - File not found", LogSeverity.WARN);

                        Console.WriteLine($"Could not find file {file}");
                        return TextureHandle.Zero;
                    }

                    using FileStream fs = File.OpenRead(file);
                    img = SKBitmap.Decode(fs);
                }

                id = LoadTexture(img, smooth, unit);
                Textures.Add(textureName, new Tuple<TextureHandle, SKBitmap>(id, img));

                Logging.Register($"Registered texture: [{textureName}]");
            }
            else
                id = value.Item1;


            return id;
        }

        private static TextureHandle LoadTexture(SKBitmap img, bool smooth = false, TextureUnit unit = TextureUnit.Texture0)
        {
            TextureHandle id = GL.GenTexture();

            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2d, id);

            GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, img.Width, img.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, img.GetPixels());

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)(smooth ? TextureMinFilter.Linear : TextureMinFilter.Nearest));
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)(smooth ? TextureMagFilter.Linear : TextureMagFilter.Nearest));

            GC.KeepAlive(img);
            return id;
        }

        public static void SetActive(int index)
        {
            GL.ActiveTexture(TexUnitLookup.Get(index));

            GL.UseProgram(Shader.TextureProgram);
            int location = GL.GetUniformLocation(Shader.TextureProgram, "texture0");
            GL.Uniform1i(location, index);
        }

        public static bool IsInUse(TextureHandle id)
        {
            foreach (KeyValuePair<string, Tuple<TextureHandle, SKBitmap>> value in Textures)
            {
                if (value.Value.Item1 == id)
                    return true;
            }

            return false;
        }
    }
}
