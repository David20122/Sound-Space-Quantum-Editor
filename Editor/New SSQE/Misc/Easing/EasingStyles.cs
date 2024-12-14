namespace New_SSQE.Misc.Easing
{
    internal class EasingStyles
    {
        // https://easings.net/

        private static readonly double C1 = 1.70158;
        private static readonly double C2 = C1 * 1.525;
        private static readonly double C3 = C1 + 1;
        private static readonly double C4 = 2 * Math.PI / 3;
        private static readonly double C5 = 2 * Math.PI / 4.5;

        private static readonly double N1 = 7.5625;

        private static readonly double D1 = 2.75;

        public static double Process(double min, double max, double t, EasingStyle style, EasingDirection direction)
        {
            t = Math.Clamp(t, 0, 1);

            return direction switch
            {
                EasingDirection.In => In(min, max, t, style),
                EasingDirection.Out => Out(min, max, t, style),
                EasingDirection.InOut => InOut(min, max, t, style),
                _ => min + (min - max) * t,
            };
        }

        private static double In(double min, double max, double t, EasingStyle style)
        {
            double x = 0;

            switch (style)
            {
                case EasingStyle.Linear:
                    x = t;
                    break;

                case EasingStyle.Sine:
                    x = 1 - Math.Cos(t * Math.PI / 2);
                    break;

                case EasingStyle.Back:
                    x = C3 * t * t * t - C1 * t * t;
                    break;

                case EasingStyle.Quad:
                    x = t * t;
                    break;

                case EasingStyle.Quart:
                    x = t * t * t * t;
                    break;

                case EasingStyle.Quint:
                    x = t * t * t * t * t;
                    break;

                case EasingStyle.Bounce:
                    x = In(0, 1, 1 - t, style);
                    break;

                case EasingStyle.Elastic:
                    x = t == 0 ? 0 : t == 1 ? 1 : -Math.Pow(2, 10 * t - 10) * Math.Sin((t * 10 - 10.75) * C4);
                    break;

                case EasingStyle.Exponential:
                    x = t == 0 ? 0 : Math.Pow(2, 10 * t - 10);
                    break;

                case EasingStyle.Circular:
                    x = 1 - Math.Sqrt(1 - Math.Pow(t, 2));
                    break;

                case EasingStyle.Cubic:
                    x = t * t * t;
                    break;
            }

            return min + (max - min) * x;
        }

        private static double Out(double min, double max, double t, EasingStyle style)
        {
            double x = 0;

            switch (style)
            {
                case EasingStyle.Linear:
                    x = t;
                    break;

                case EasingStyle.Sine:
                    x = Math.Sin(t * Math.PI / 2);
                    break;

                case EasingStyle.Back:
                    x = 1 + C3 * Math.Pow(t - 1, 3) + C1 * Math.Pow(t - 1, 2);
                    break;

                case EasingStyle.Quad:
                    x = 1 - (1 - t) * (1 - t);
                    break;

                case EasingStyle.Quart:
                    x = 1 - Math.Pow(1 - t, 4);
                    break;

                case EasingStyle.Quint:
                    x = 1 - Math.Pow(1 - t, 5);
                    break;

                case EasingStyle.Bounce:
                    if (t < 1 / D1)
                        x = N1 * t * t;
                    else if (t < 2 / D1)
                        x = N1 * (t -= 1.5 / D1) * t + 0.75;
                    else if (t < 2.5 / D1)
                        x = N1 * (t -= 2.25 / D1) * t + 0.9375;
                    else
                        x = N1 * (t -= 2.625 / D1) * t + 0.984375;
                    break;

                case EasingStyle.Elastic:
                    x = t == 0 ? 0 : t == 1 ? 1 : Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * C4) + 1;
                    break;

                case EasingStyle.Exponential:
                    x = x == 1 ? 1 : 1 - Math.Pow(2, -10 * t);
                    break;

                case EasingStyle.Circular:
                    x = Math.Sqrt(1 - Math.Pow(t - 1, 2));
                    break;

                case EasingStyle.Cubic:
                    x = 1 - Math.Pow(1 - t, 3);
                    break;
            }

            return min + (max - min) * x;
        }

        private static double InOut(double min, double max, double t, EasingStyle style)
        {
            double x = 0;

            switch (style)
            {
                case EasingStyle.Linear:
                    x = t;
                    break;

                case EasingStyle.Sine:
                    x = -(Math.Cos(t * Math.PI) - 1) / 2;
                    break;

                case EasingStyle.Back:
                    x = t < 0.5 ? Math.Pow(2 * t, 2) * ((C2 + 1) * 2 * t - C2) / 2 : (Math.Pow(2 * t - 2, 2) * ((C2 + 1) * (t * 2 - 2) + C2) + 2) / 2;
                    break;

                case EasingStyle.Quad:
                    x = t < 0.5 ? 2 * t * t : 1 - Math.Pow(-2 * t + 2, 2) / 2;
                    break;

                case EasingStyle.Quart:
                    x = t < 0.5 ? 8 * t * t * t * t : 1 - Math.Pow(-2 * t + 2, 4) / 2;
                    break;

                case EasingStyle.Quint:
                    x = t < 0.5 ? 16 * t * t * t * t * t : 1 - Math.Pow(-2 * t + 2, 5) / 2;
                    break;

                case EasingStyle.Bounce:
                    x = t < 0.5 ? (1 - Out(0, 1, 1 - 2 * t, style)) / 2 : (1 + Out(0, 1, 2 * t - 1, style)) / 2;
                    break;

                case EasingStyle.Elastic:
                    x = t == 0 ? 0 : t == 1 ? 1 : t < 0.5 ? -(Math.Pow(2, 20 * t - 10) * Math.Sin((20 * t - 11.125) * C5)) / 2 : Math.Pow(2, -20 * t + 10) * Math.Sin((20 * t - 11.125) * C5) / 2 + 1;
                    break;

                case EasingStyle.Exponential:
                    x = t == 0 ? 0 : t == 1 ? 1 : t < 0.5 ? Math.Pow(2, 20 * t - 0) / 2 : (2 - Math.Pow(2, -20 * t + 10)) / 2;
                    break;

                case EasingStyle.Circular:
                    x = t < 0.5 ? (1 - Math.Sqrt(1 - Math.Pow(2 * t, 2))) / 2 : (Math.Sqrt(1 - Math.Pow(-2 * t + 2, 2)) + 1) / 2;
                    break;

                case EasingStyle.Cubic:
                    x = t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
                    break;
            }

            return min + (max - min) * x;
        }
    }
}
