using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sound_Space_Editor.Gui;

namespace Sound_Space_Editor
{
	public class UndoRedo
	{
		private readonly List<UndoRedoAction> _actions = new List<UndoRedoAction>();

		public bool CanUndo => _actions.LastOrDefault(a => !a.Undone) != null;
		public bool CanRedo => _actions.FirstOrDefault(a => a.Undone) != null;

		public void AddUndoRedo(string label, Action undo, Action redo)
		{
			_actions.RemoveAll(a => a.Undone);
			_actions.Add(new UndoRedoAction(label, undo, redo));
		}

		public void Undo()
		{
			var action = _actions.LastOrDefault(a => !a.Undone);

			if (action == null)
				return;

			if (EditorWindow.Instance.GuiScreen is GuiScreenEditor editor)
				editor.ShowToast("UNDONE: " + action.Label, Color.FromArgb(255, 109, 0));

			action.Undo?.Invoke();
			action.Undone = true;
		}

		public void Redo()
		{
			var action = _actions.FirstOrDefault(a => a.Undone);

			if (action == null)
				return;

			if (EditorWindow.Instance.GuiScreen is GuiScreenEditor editor)
				editor.ShowToast("REDONE: " + action.Label, Color.FromArgb(255, 109, 0));

			action.Redo?.Invoke();
			action.Undone = false;
		}

		public void Clear()
		{
			_actions.Clear();
		}
	}
}