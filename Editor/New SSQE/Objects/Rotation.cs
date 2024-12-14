using New_SSQE.Misc.Easing;

namespace New_SSQE.Objects
{
    internal class Rotation : MapObject
    {
        public EasingStyle Style;
        public EasingDirection Direction;

        public float Degrees;

        public Rotation(long ms, long duration, EasingStyle style, EasingDirection direction, float degrees) : base(9, ms, "Rotation")
        {
            Duration = duration;

            Style = style;
            Direction = direction;

            Degrees = degrees;
        }

        public override string ToString(params object[] data)
        {
            return base.ToString(
                Duration,
                (int)Style,
                (int)Direction,
                Math.Round(Degrees, 2).ToString(Program.Culture)
            );
        }

        public override Rotation Clone()
        {
            return new(Ms, Duration, Style, Direction, Degrees);
        }
    }
}
