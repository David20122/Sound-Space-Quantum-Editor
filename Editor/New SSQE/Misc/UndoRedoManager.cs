using System;
using System.Collections.Generic;
using New_SSQE.GUI;
using System.Drawing;

namespace New_SSQE
{
    internal class UndoRedoManager
    {
        private List<URAction> actions = new();

        private int index = -1;

        public void Add(string label, Action undo, Action redo, bool runRedo = true)
        {
            try
            {
                while (index + 1 < actions.Count)
                    actions.RemoveAt(index + 1);

                actions.Add(new URAction(label, undo, redo));
                ActionLogging.Register($"Action registered: {label}");
                index++;

                if (runRedo && index < actions.Count && index >= 0)
                    actions[index].Redo?.Invoke();
            }
            catch (Exception ex)
            {
                ActionLogging.Register($"Failed to register action: {label} - {ex.Message}", "WARN");
            }
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
