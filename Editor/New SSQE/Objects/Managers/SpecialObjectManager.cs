using New_SSQE.EditHistory;
using New_SSQE.Maps;

namespace New_SSQE.Objects.Managers
{
    internal class SpecialObjectManager
    {
        private static ObjectList<MapObject> Objects => CurrentMap.SpecialObjects;

        public static void Replace(string label, List<MapObject> oldObjects, List<MapObject> newObjects)
        {
            label = label.Replace("[S]", Math.Max(oldObjects.Count, newObjects.Count) > 1 ? "S" : "");

            oldObjects = oldObjects.ToList();
            newObjects = newObjects.ToList();

            UndoRedoManager.Add(label, () =>
            {
                Objects.RemoveAll(newObjects);
                Objects.AddRange(oldObjects);

                Objects.Sort();
                Objects.Selected = new(oldObjects);
            }, () =>
            {
                Objects.RemoveAll(oldObjects);
                Objects.AddRange(newObjects);

                Objects.Sort();
                Objects.Selected = new(newObjects);
            });
        }

        public static void Edit(string label, List<MapObject> toModify, Action<MapObject> action)
        {
            if (toModify.Count == 0)
                return;

            List<MapObject> completed = toModify.Select(n => n.Clone()).ToList();
            completed.ForEach(action);

            Replace(label, toModify, completed);
        }
        public static void Edit(string label, Action<MapObject> action) => Edit(label, Objects.Selected, action);

        public static void Add(string label, List<MapObject> toAdd) => Replace(label, [], toAdd);
        public static void Add(string label, MapObject toAdd) => Replace(label, [], [toAdd]);

        public static void Remove(string label, List<MapObject> toRemove) => Replace(label, toRemove, []);
        public static void Remove(string label, MapObject toRemove) => Replace(label, [toRemove], []);
        public static void Remove(string label) => Replace(label, Objects.Selected, []);
    }
}
