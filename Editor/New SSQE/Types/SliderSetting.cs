using System;

namespace New_SSQE
{
    [Serializable]
    internal class SliderSetting
    {
        public float Value;
        public float Max;
        public float Step;
        public float Default;

        public SliderSetting(float value, float max, float step)
        {
            Value = value;
            Max = max;
            Step = step;
            Default = value;
        }
    }
}
