using New_SSQE.Misc.Easing;

namespace New_SSQE.Objects
{
    internal class FOV : MapObject
    {
        public EasingStyle Style;
        public EasingDirection Direction;

        public float Intensity;

        public FOV(long ms, long duration, EasingStyle style, EasingDirection direction, float intensity) : base(6, ms, "FOV")
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

        public override FOV Clone()
        {
            return new(Ms, Duration, Style, Direction, Intensity);
        }
    }
}
