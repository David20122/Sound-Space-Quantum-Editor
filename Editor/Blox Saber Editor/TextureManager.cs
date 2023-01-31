using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Sound_Space_Editor
{
	static class TextureManager
	{
		private static readonly Dictionary<string, int> Textures = new Dictionary<string, int>();

		public static int GetOrRegister(string textureName, Bitmap bmp = null, bool smooth = false)
		{
			if (Textures.TryGetValue(textureName, out var texId))
				return texId;

			Bitmap img = bmp;

			if (img == null)
			{
				var file = $"assets\\textures\\{textureName}.png";

				if (!File.Exists(file))
				{
					Console.WriteLine($"Could not find file {file}");
					return -1;
				}

				using (var fs = File.OpenRead(file))
				{
					img = (Bitmap)Image.FromStream(fs);
				}
			}

			var id = LoadTexture(img, smooth);

			Textures.Add(textureName, id);

			return id;
		}

		private static int LoadTexture(Bitmap img, bool smooth = false)
		{
			var id = GL.GenTexture();

			GL.BindTexture(TextureTarget.Texture2D, id);

			BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
				ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

			img.UnlockBits(data);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
				(int)(smooth ? TextureMinFilter.Linear : TextureMinFilter.Nearest));
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
				(int)(smooth ? TextureMagFilter.Linear : TextureMagFilter.Nearest));
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
				(int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
				(int)TextureWrapMode.ClampToEdge);

			return id;
		}
	}
}