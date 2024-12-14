namespace New_SSQE.Objects.Other
{
    [Serializable]
    internal class Bookmark
    {
        public string Text { get; set; }
        public long Ms { get; set; }
        public long EndMs { get; set; }

        public Bookmark(string text, long ms, long endMs)
        {
            Text = text;
            Ms = ms;
            EndMs = endMs;
        }

        public Bookmark(string data)
        {
            string[] split = data.Split('|');

            Text = split[0].Replace("\0\0", "|").Replace("\0", ",");
            Ms = long.Parse(split[1]);
            EndMs = long.Parse(split[2]);
        }

        public override string ToString()
        {
            string text = Text.Replace(",", "\0").Replace("|", "\0\0");

            return $",{text}|{Ms}|{EndMs}";
        }

        public Bookmark Clone()
        {
            return new(Text, Ms, EndMs);
        }
    }
}
