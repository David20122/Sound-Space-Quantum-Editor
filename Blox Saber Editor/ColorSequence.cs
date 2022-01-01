using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Linq;
using Sound_Space_Editor.Properties;

namespace Sound_Space_Editor
{
	public class ColorSequence
	{
		private readonly Color[] _colors;
		private int _index;


		public ColorSequence()
		{
			int bgdim = EditorSettings.EditorBGOpacity;

			string[] NoteColors = EditorWindow.Instance.NoteColors;
			//int[] NoteColor1 = EditorWindow.Instance.NoteColor1;
			//int[] NoteColor2 = EditorWindow.Instance.NoteColor2;

			_colors = new Color[NoteColors.Length]; //{ Color.FromArgb(NoteColor1[0], NoteColor1[1], NoteColor1[2]), Color.FromArgb(NoteColor2[0], NoteColor2[1], NoteColor2[2]) };
			int i = 0;

			foreach (string color in NoteColors)
			{
				string[] c1values = color.Split(',');
				int[] cc = Array.ConvertAll(c1values, int.Parse);

				Color current;

				bool a = cc.Length > 3;

				if (a)
				{
					current = Color.FromArgb(cc[3], cc[0], cc[1], cc[2]);
				}
				else
				{
					current = Color.FromArgb(cc[0], cc[1], cc[2]);
				}

				_colors[i] = current;
				i++;
			}
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