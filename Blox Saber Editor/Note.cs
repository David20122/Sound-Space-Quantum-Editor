using System;
using System.Drawing;

namespace Sound_Space_Editor
{
	[Serializable]
	class Note
	{
		public float X;
		public float Y;
		public long Ms;
		public long DragStartMs;

		public Color Color;

		public Note(float x, float y, long ms)
		{
			X = x;
			Y = y;

			Ms = ms;
		}

		public Note Clone()
		{
			return new Note(X, Y, Ms) { Color = Color };
		}
	}
}