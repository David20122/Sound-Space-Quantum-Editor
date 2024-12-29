using New_SSQE.EditHistory;
using New_SSQE.Maps;

namespace New_SSQE.Objects.Managers
{
    internal class NoteManager
    {
        private static ObjectList<Note> Notes => CurrentMap.Notes;

        public static void Replace(string label, List<Note> oldNotes, List<Note> newNotes)
        {
            label = label.Replace("[S]", Math.Max(oldNotes.Count, newNotes.Count) > 1 ? "S" : "");

            oldNotes = oldNotes.ToList();
            newNotes = newNotes.ToList();

            UndoRedoManager.Add(label, () =>
            {
                Notes.RemoveAll(newNotes);
                Notes.AddRange(oldNotes);

                Notes.Sort();
                Notes.Selected = new(oldNotes);
            }, () =>
            {
                Notes.RemoveAll(oldNotes);
                Notes.AddRange(newNotes);

                Notes.Sort();
                Notes.Selected = new(newNotes);
            });
        }

        public static void Edit(string label, List<Note> toModify, Action<Note> action)
        {
            if (toModify.Count == 0)
                return;

            List<Note> completed = toModify.Select(n => n.Clone()).ToList();
            completed.ForEach(action);

            Replace(label, toModify, completed);
        }
        public static void Edit(string label, Action<Note> action) => Edit(label, Notes.Selected, action);

        public static void Add(string label, List<Note> toAdd) => Replace(label, [], toAdd);
        public static void Add(string label, Note toAdd) => Replace(label, [], [toAdd]);

        public static void Remove(string label, List<Note> toRemove) => Replace(label, toRemove, []);
        public static void Remove(string label, Note toRemove) => Replace(label, [toRemove], []);
        public static void Remove(string label) => Replace(label, Notes.Selected, []);
    }
}
