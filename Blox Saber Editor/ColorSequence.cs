using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Linq;
using Sound_Space_Editor.Properties;

namespace Sound_Space_Editor
{
	class ColorSequence
	{
		private readonly Color[] _colors;
		private int _index;


		public ColorSequence()
		{
			int bgdim = EditorSettings.EditorBGOpacity;

			int[] NoteColor1 = EditorWindow.Instance.NoteColor1;
			int[] NoteColor2 = EditorWindow.Instance.NoteColor2;

			_colors = new Color[] { Color.FromArgb(NoteColor1[0], NoteColor1[1], NoteColor1[2]), Color.FromArgb(NoteColor2[0], NoteColor2[1], NoteColor2[2]) };
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