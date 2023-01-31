using System;

namespace Sound_Space_Editor.Misc
{
    [Serializable]
    class UndoRedoAction
    {
        public string Label;
        public Action Undo;
        public Action Redo;

        public UndoRedoAction(string label, Action undo, Action redo)
        {
            Label = label;
            Undo = undo;
            Redo = redo;
        }
    }
}
