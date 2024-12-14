using New_SSQE.Misc.Easing;
using System.Drawing;

namespace New_SSQE.Objects
{
    internal class Tint : MapObject
    {
        public EasingStyle Style;
        public EasingDirection Direction;

        public Color Color;

        public Tint(long ms, long duration, EasingStyle style, EasingDirection direction, Color color) : base(7, ms, "Tint")
        {
            Duration = duration;

            Style = style;
            Direction = direction;

            Color = color;
        }

        public override string ToString(params object[] data)
        {
            return base.ToString(
                Duration,
                (int)Style,
                (int)Direction,

                Color.R,
                Color.G,
                Color.B
            );
        }

        public override Tint Clone()
        {
            return new(Ms, Duration, Style, Direction, Color);
        }
    }
}
