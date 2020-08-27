using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Linq;

namespace Sound_Space_Editor
{
	class ColorSequence
	{
		private readonly Color[] _colors;
		private int _index;


		public ColorSequence()
		{
			// color 1

			string rc1 = EditorWindow.Instance.ReadLine("settings.ini", 12);
			string[] c1values = rc1.Split(',');
			int[] Color1 = Array.ConvertAll<string, int>(c1values, int.Parse);

			//color 2

			string rc2 = EditorWindow.Instance.ReadLine("settings.ini", 17);
			string[] c2values = rc2.Split(',');
			int[] Color2 = Array.ConvertAll<string, int>(c2values, int.Parse);

			var colors = new[] { GL.Color3(Color2[0] / 255, Color2[1] / 255, Color2[2] / 255), GL.Color3(Color1[0] / 255, Color1[1] / 255, Color1[2] / 255) };
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