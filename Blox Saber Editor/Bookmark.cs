using System;

namespace Sound_Space_Editor
{
	[Serializable]
	class Bookmark
	{
		public string Name;
		public long MS;

		public Bookmark(string name, long ms)
		{
			this.Name = name;
			this.MS = ms;
		}
	}
}