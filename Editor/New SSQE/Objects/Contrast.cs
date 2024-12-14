using New_SSQE.Misc.Easing;

namespace New_SSQE.Objects
{
    internal class Contrast : MapObject
    {
        public EasingStyle Style;
        public EasingDirection Direction;

        public float Intensity;

        public Contrast(long ms, long duration, EasingStyle style, EasingDirection direction, float intensity) : base(3, ms, "Contrast")
        {
            Duration = duration;

            Style = style;
            Direction = direction;

            Intensity = intensity;
        }

        public override string ToString(params object[] data)
        {
            return base.ToString(
                Duration,
                (int)Style,
                (int)Direction,
                Math.Round(Intensity, 2).ToString(Program.Culture)
            );
        }

        public override Contrast Clone()
        {
            return new(Ms, Duration, Style, Direction, Intensity);
        }
    }
}
