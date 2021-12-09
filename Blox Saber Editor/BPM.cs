using System;

namespace Sound_Space_Editor
{
	[Serializable]
	class BPM
	{
		public float bpm;
		public long Ms;

		public BPM(float beatspermin, long ms)
		{
			bpm = beatspermin;
			Ms = ms;
		}
	}
}