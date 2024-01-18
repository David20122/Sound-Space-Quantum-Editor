using System;
using System.Collections.Generic;
using New_SSQE.GUI;
using System.Drawing;

namespace New_SSQE
{
    internal class UndoRedoManager
    {
        public readonly List<URAction> actions = new();

        public int _index = -1;

        private DateTime? prevTime;

        public void Add(string label, Action undo, Action redo, bool runRedo = true, bool reload = false)
        {
            try
            {
                while (_index + 1 < actions.Count)
                    actions.RemoveAt(_index + 1);

                actions.Add(new URAction(label, undo, redo));
                _index++;

                if (runRedo && _index < actions.Count && _index >= 0)
                    actions[_index].Redo?.Invoke();


                // cause people hate bpm now?
                if (label == "ADD NOTE" && runRedo && MainWindow.Instance.TimingPoints.Count == 0 && MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                {
                    DateTime curTime = DateTime.Now;

                    if (prevTime == null || (curTime - prevTime)?.TotalMilliseconds >= 5000)
                    {
                        editor.ShowToast("Please use BPM!", Settings.settings["color2"]);
                        prevTime = curTime;
                    }
                }
            }
            catch (Exception ex)
            {
                ActionLogging.Register($"Failed to register action: {label} - {ex.Message}", "WARN");
            }
        }

        public void Undo()
        {
            if (_index >= 0)
            {
                var action = actions[_index];

                if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                    editor.ShowToast($"UNDONE: {action.Label}", Color.FromArgb(255, 109, 0));

                    action.Undo?.Invoke();
                _index--;
            }
        }

        public void Redo()
        {
            if (_index + 1 < actions.Count)
            {
                var action = actions[_index + 1];

                if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                    editor.ShowToast($"REDONE: {action.Label}", Color.FromArgb(255, 109, 0));

                    action.Redo?.Invoke();
                _index++;
            }
        }

        public void Clear()
        {
            actions.Clear();
            _index = -1;
        }
    }
}
