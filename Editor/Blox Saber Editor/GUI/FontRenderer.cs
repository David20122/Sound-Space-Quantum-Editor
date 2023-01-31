using System.Collections.Generic;
using System.IO;
using System;

namespace Sound_Space_Editor.GUI
{
	class FontRenderer
	{
        private readonly Dictionary<int, FreeTypeFont> cached = new Dictionary<int, FreeTypeFont>();
        private readonly string Path;

        public FontRenderer(string font)
        {
            Path = $"assets/fonts/{font}.ttf";

            if (!File.Exists(Path))
                throw new FileNotFoundException($"Couldn't find file '{Path}'", Path);
        }

        private FreeTypeFont GetFont(int size)
        {
            if (!cached.ContainsKey(size))
                cached.Add(size, new FreeTypeFont(Path, size));
            return cached[size];
        }

        public void Render(string text, float posx, float posy, int size)
        {
            GetFont(size).Print(text, (int)(posx + 0.5f), (int)(posy + 0.5f));
        }

        public int GetWidth(string text, int size)
        {
            var font = GetFont(size);
            string[] lines = text.Split('\n');
            int max = 0;

            foreach (var line in lines)
                max = Math.Max(max, font.Extent(line));

            return max;
        }

        public int GetHeight(int size)
        {
            return (int)GetFont(size).BaseLine;
        }
    }
}
