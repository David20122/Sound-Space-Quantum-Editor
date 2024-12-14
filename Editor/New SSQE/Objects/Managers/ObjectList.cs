using New_SSQE.GUI;

namespace New_SSQE.Objects.Managers
{
    internal class SelectionList<T> : List<T>
    {
        public SelectionList() { }
        public SelectionList(int capacity) : base(capacity) { }
        public SelectionList(IEnumerable<T> collection) : base(collection) { }

        [Obsolete("Use ObjectList<T>.ClearSelection() instead.", true)]
        public new void Clear()
        {
            base.Clear();
        }
    }



    internal class ObjectList<T> : List<T> where T : MapObject
    {
        public ObjectList() : base() { }
        public ObjectList(int capacity) : base(capacity) { }
        public ObjectList(IEnumerable<T> collection) : base(collection) { }



        private SelectionList<T> _selected = new();
        public SelectionList<T> Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                UpdateSelection();
            }
        }

        public void ClearSelection()
        {
            Selected = new();
        }



        public long GetClosest(float ms)
        {
            long closest = -1;

            for (int i = 0; i < Count; i++)
            {
                long cur = this[i].Ms;

                if (Math.Abs(cur - ms) < Math.Abs(closest - ms))
                    closest = cur;
            }

            return closest;
        }

        public void UpdateSelection()
        {
            for (int i = 0; i < Count; i++)
                this[i].Selected = false;
            for (int i = 0; i < Selected.Count; i++)
                Selected[i].Selected = true;
        }



        public (int, int) SearchRange(long start, long end)
        {
            int first = SearchFirst(start);
            int last = SearchLast(end);

            return (first, last);
        }

        public (int, int) SearchRange(float start, float end) => SearchRange((long)start, (long)end);

        public int SearchFirst(long key)
        {
            int first = 0;
            int count = Count - 1;

            int iter, step;

            while (count > 0)
            {
                step = count / 2;
                iter = first + step;

                if (this[iter].Ms < key)
                {
                    first = iter + 1;
                    count -= step + 1;
                }
                else
                    count = step;
            }

            return first;
        }

        public int SearchLast(long key)
        {
            int last = 0;
            int count = Count - 1;

            int iter, step;

            while (count > 0)
            {
                step = count / 2;
                iter = last + step;

                if (this[iter].Ms <= key)
                {
                    last = iter + 1;
                    count -= step + 1;
                }
                else
                    count = step;
            }

            return last;
        }



        public new void Clear()
        {
            base.Clear();
            Selected = new();
        }

        public new void Sort()
        {
            Sort((x, y) => (int)(x.Ms - y.Ms));

            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                editor.Timeline.GenerateOffsets();
        }
    }
}
