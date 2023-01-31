using System;

namespace Sound_Space_Editor.Misc
{
    [Serializable]
    class TimingPoint
    {
        public float bpm;
        public long Ms;
        public long DragStartMs;

        public TimingPoint(float BPM, long ms)
        {
            bpm = BPM;
            Ms = ms;
        }
    }
}
