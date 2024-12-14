namespace New_SSQE.Misc.Dialogs
{
    // mimics old DialogResult from forms
    public enum DialogResult
    {
        None,
        OK,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No,
        // 8-9 apparently dont exist
        TryAgain = 10,
        Continue = 11,
    }
}
