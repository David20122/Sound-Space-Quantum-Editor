using System.Collections.Generic;
using System.IO;

namespace Sound_Space_Editor.Gui
{
	class FontRenderer
	{
		private readonly Dictionary<int, FtFont> _cached = new Dictionary<int, FtFont>();

		private readonly string _fontPath = "";

		public FontRenderer(string fontName)
		{
			_fontPath = $"assets/fonts/{fontName}.ttf";

			if (!File.Exists(_fontPath))
				throw new FileNotFoundException($"Couldn't find file '{_fontPath}'", _fontPath);
		}

		public void Render(string text, int x, int y, int size)
		{
			try
            {
				if (!_cached.TryGetValue(size, out var font))
					_cached.Add(size, font = new FtFont(_fontPath, size));
				font.Print(text, x, y);
			}
			catch
            {

            }
		}

		public int GetWidth(string text, int size)
		{
			if (_cached.TryGetValue(size, out var font))
            {
				var max = 0;
				var lines = text.Split('\n');
				foreach (var line in lines)
                {
					if (font.Extent(line) > max)
						max = font.Extent(line);
                }
				return max;
			}

			return 0;
		}

		public float GetHeight(int size)
		{
			if (_cached.TryGetValue(size, out var font))
				return font.BaseLine;

			return 0;
		}
	}
}
