namespace SSQE_Player.Types
{
    [Serializable]
    internal class ListSetting
    {
        public string Current;
        public List<string> Possible;

        public ListSetting(string current, params string[] possible)
        {
            Current = current;
            Possible = possible.ToList();
        }
    }
}
