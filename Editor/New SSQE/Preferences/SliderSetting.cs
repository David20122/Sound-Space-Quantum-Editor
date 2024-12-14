namespace New_SSQE.Preferences
{
    [Serializable]
    internal class SliderSetting
    {
        public float Value;
        public float Max;
        public float Step;
        public int Decimals;
        public float Default;

        public SliderSetting(float value, float max, float step, int decimals = 0)
        {
            Value = value;
            Max = max;
            Step = step;
            Decimals = decimals;
            Default = value;
        }

        public float Get()
        {
            return (float)Math.Round(Value, Decimals);
        }
    }
}
