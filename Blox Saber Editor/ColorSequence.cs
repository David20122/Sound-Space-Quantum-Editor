using System.Drawing;

namespace Sound_Space_Editor
{
	class ColorSequence
	{
		private readonly Color[] _colors;
		private int _index;

		public ColorSequence()
		{
			_colors = new[] { Color.FromArgb(255, 0, 255), Color.FromArgb(0, 255, 200) };
		}

		public Color Next()
		{
			var color = _colors[_index];

			_index = (_index + 1) % _colors.Length;

			return color;
		}

		public void Reset()
		{
			_index = 0;
		}
	}
}