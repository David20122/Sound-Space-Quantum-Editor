using System;
using System.ComponentModel;

namespace New_SSQE
{
    [Serializable]
    internal class TimingPoint
    {
        public float BPM { get; set; }
        public long Ms { get; set; }

        public long DragStartMs;

        public TimingPoint(float bpm, long ms)
        {
            BPM = bpm;
            Ms = ms;
        }
    }
}
