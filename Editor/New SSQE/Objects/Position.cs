using New_SSQE.Misc.Easing;
using OpenTK.Mathematics;

namespace New_SSQE.Objects
{
    internal class Position : MapObject
    {
        public EasingStyle Style;
        public EasingDirection Direction;

        public Vector3 Pos;

        public Position(long ms, long duration, EasingStyle style, EasingDirection direction, Vector3 pos) : base(8, ms, "Position")
        {
            Duration = duration;

            Style = style;
            Direction = direction;

            Pos = pos;
        }

        public override string ToString(params object[] data)
        {
            return base.ToString(
                Duration,
                (int)Style,
                (int)Direction,

                Math.Round(Pos.X, 2).ToString(Program.Culture),
                Math.Round(Pos.Y, 2).ToString(Program.Culture),
                Math.Round(Pos.Z, 2).ToString(Program.Culture)
            );
        }

        public override Position Clone()
        {
            return new(Ms, Duration, Style, Direction, Pos);
        }
    }
}
