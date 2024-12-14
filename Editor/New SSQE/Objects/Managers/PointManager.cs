using New_SSQE.EditHistory;
using New_SSQE.Maps;

namespace New_SSQE.Objects.Managers
{
    internal class PointManager
    {
        private static List<TimingPoint> Points => CurrentMap.TimingPoints;
        private static TimingPoint? Selected => CurrentMap.SelectedPoint;

        public static void Replace(string label, TimingPoint? oldPoint, TimingPoint? newPoint)
        {
            if (oldPoint == null && newPoint == null)
                return;

            UndoRedoManager.Add(label, () =>
            {
                if (newPoint != null)
                    Points.Remove(newPoint);
                if (oldPoint != null)
                    Points.Add(oldPoint);

                CurrentMap.SortTimings();
                TimingsWindow.Instance?.ResetList();
            }, () =>
            {
                if (oldPoint != null)
                    Points.Remove(oldPoint);
                if (newPoint != null)
                    Points.Add(newPoint);

                CurrentMap.SortTimings();
                TimingsWindow.Instance?.ResetList();
            });
        }

        public static void Edit(string label, TimingPoint? toModify, Action<TimingPoint> action)
        {
            if (toModify == null)
                return;

            TimingPoint completed = toModify.Clone();
            action(completed);

            Replace(label, toModify, completed);
        }
        public static void Edit(string label, Action<TimingPoint> action) => Edit(label, Selected, action);

        public static void Add(string label, TimingPoint toAdd) => Replace(label, null, toAdd);

        public static void Remove(string label, TimingPoint toRemove) => Replace(label, toRemove, null);
        public static void Remove(string label) => Replace(label, Selected, null);
    }
}
