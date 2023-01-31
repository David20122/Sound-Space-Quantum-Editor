using System;
using System.Collections.Generic;
using System.Drawing;
using Sound_Space_Editor.GUI;
using Sound_Space_Editor.Misc;

namespace Sound_Space_Editor
{
	class UndoRedoManager
	{
        private List<UndoRedoAction> actions = new List<UndoRedoAction>();

        private int index = -1;

        public void Add(string label, Action undo, Action redo, bool runRedo = true)
        {
            while (index + 1 < actions.Count)
                actions.RemoveAt(index + 1);

            actions.Add(new UndoRedoAction(label, undo, redo));
            index++;

            if (runRedo && index < actions.Count && index >= 0)
                actions[index].Redo?.Invoke();
        }

        public void Undo()
        {
            if (index >= 0)
            {
                var action = actions[index];

                if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                    editor.ShowToast($"UNDONE: {action.Label}", Color.FromArgb(255, 109, 0));

                action.Undo?.Invoke();
                index--;
            }
        }

        public void Redo()
        {
            if (index + 1 < actions.Count)
            {
                var action = actions[index + 1];

                if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                    editor.ShowToast($"REDONE: {action.Label}", Color.FromArgb(255, 109, 0));

                action.Redo?.Invoke();
                index++;
            }
        }

        public void Clear()
        {
            actions.Clear();
            index = -1;
        }
    }
}