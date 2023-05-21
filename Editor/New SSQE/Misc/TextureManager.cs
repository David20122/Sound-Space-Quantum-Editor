using SkiaSharp;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.IO;
using New_SSQE.GUI;
using OpenTK.Graphics;

namespace New_SSQE
{
    internal class TextureManager
    {
        private static readonly Dictionary<string, Tuple<TextureHandle, SKBitmap>> Textures = new();

        public static TextureHandle GetOrRegister(string textureName, SKBitmap? img = null, bool smooth = false, TextureUnit unit = TextureUnit.Texture0)
        {
            //if (Textures.TryGetValue(textureName, out var texId))
                //return texId;

            if (img == null)
            {
                var file = $"assets/textures/{textureName}.png";

                if (!File.Exists(file))
                {
                    ActionLogging.Register($"Failed to register texture: [{textureName}] - File not found", "WARN");

                    Console.WriteLine($"Could not find file {file}");
                    return TextureHandle.Zero;
                }

                using var fs = File.OpenRead(file);
                img = SKBitmap.Decode(fs);
            }

            var id = LoadTexture(img, smooth, unit);

            if (Textures.ContainsKey(textureName))
                Textures[textureName] = new Tuple<TextureHandle, SKBitmap>(id, img);
            else
                Textures.Add(textureName, new Tuple<TextureHandle, SKBitmap>(id, img));

            ActionLogging.Register($"Registered texture: [{textureName}]");

            return id;
        }

        private static TextureHandle LoadTexture(SKBitmap img, bool smooth = false, TextureUnit unit = TextureUnit.Texture0)
        {
            var id = GL.GenTexture();
            GC.KeepAlive(img);

            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2d, id);

            GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, img.Width, img.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, img.GetPixels());

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)(smooth ? TextureMinFilter.Linear : TextureMinFilter.Nearest));
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)(smooth ? TextureMagFilter.Linear : TextureMagFilter.Nearest));

            return id;
        }

        public static void SetActive(int index)
        {
            var location = GL.GetUniformLocation(Shader.TexProgram, "texture0");
            GL.Uniform1i(location, index);
        }
    }
}
