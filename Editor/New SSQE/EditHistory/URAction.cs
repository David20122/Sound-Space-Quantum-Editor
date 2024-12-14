namespace New_SSQE.EditHistory
{
    [Serializable]
    internal class URAction
    {
        public string Label;

        public Action Undo;
        public Action Redo;

        public URAction(string label, Action undo, Action redo)
        {
            Label = label;

            Undo = undo;
            Redo = redo;
        }
    }
}
