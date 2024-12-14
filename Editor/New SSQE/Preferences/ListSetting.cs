namespace New_SSQE.Preferences
{
    [Serializable]
    internal class ListSetting
    {
        public string Current;
        public string[] Possible;

        public ListSetting(int current, params string[] possible)
        {
            Current = possible[current];
            Possible = possible;
        }
    }
}
