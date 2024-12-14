namespace New_SSQE.Objects
{
    [Serializable]
    internal class TimingPoint : MapObject
    {
        public float BPM { get; set; }

        public TimingPoint(float bpm, long ms) : base(1, ms)
        {
            BPM = bpm;
            Ms = ms;
        }

        public TimingPoint(string data) : base(1, 0)
        {
            string[] split = data.Split('|');

            BPM = float.Parse(split[1], Program.Culture);
            Ms = long.Parse(split[0]);
        }

        public override string ToString(params object[] data)
        {
            double bpm = Math.Round(BPM, 2);

            return base.ToString(bpm.ToString(Program.Culture));
        }

        public override TimingPoint Clone()
        {
            return new(BPM, Ms);
        }
    }
}
