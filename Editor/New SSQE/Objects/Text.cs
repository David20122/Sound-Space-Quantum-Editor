namespace New_SSQE.Objects
{
    internal class Text : MapObject
    {
        public string String;
        public int Visibility;

        public Text(long ms, long duration, string str, int visibility) : base(11, ms, "Text")
        {
            Duration = duration;
            String = str;
            Visibility = visibility;
        }

        public override string ToString(params object[] data)
        {
            return base.ToString(
                Duration,
                GetFixedString(),
                Visibility
            );
        }

        public override Text Clone()
        {
            return new(Ms, Duration, String, Visibility);
        }

        private string GetFixedString()
        {
            return String.Replace('/', '_')
                .Replace('`', '_')
                .Replace('|', '_')
                .Replace(',', '_');
        }
    }
}
