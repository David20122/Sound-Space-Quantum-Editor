using System;

namespace New_SSQE
{
    [Serializable]
    internal class ListSetting
    {
        public string Current;
        public string[] Possible;

        public ListSetting(string current, params string[] possible)
        {
            Current = current;
            Possible = possible;
        }
    }
}
