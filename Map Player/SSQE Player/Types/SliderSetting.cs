namespace SSQE_Player
{
    [Serializable]
    internal class SliderSetting
    {
        public float Value;
        public float Max;
        public float Step;

        public SliderSetting(float value, float max, float step)
        {
            Value = value;
            Max = max;
            Step = step;
        }
    }
}
