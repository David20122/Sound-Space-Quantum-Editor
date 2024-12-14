using New_SSQE.Misc.Easing;

namespace New_SSQE.Objects
{
    internal class Brightness : MapObject
    {
        public EasingStyle Style;
        public EasingDirection Direction;

        public float Intensity;

        public Brightness(long ms, long duration, EasingStyle style, EasingDirection direction, float intensity) : base(2, ms, "Brightness")
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

        public override Brightness Clone()
        {
            return new(Ms, Duration, Style, Direction, Intensity);
        }
    }
}
