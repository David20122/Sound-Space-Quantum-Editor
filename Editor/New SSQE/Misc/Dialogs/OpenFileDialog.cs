using Avalonia.Controls;
using Avalonia.Threading;
using New_SSQE.Preferences;

namespace New_SSQE.Misc.Dialogs
{
    // turns forms OpenFileDialog info into Avalonia method
    internal class OpenFileDialog
    {
        public string? Title;
        public string? Filter;
        public string? InitialDirectory;

        public string FileName = "";

        public DialogResult ShowDialog()
        {
            MainWindow.DefaultWindow.Topmost = true;
            MainWindow.Instance.LockClick();

            string[] filters = (Filter ?? "").Split('|');
            string name = filters[0];
            List<string> extensions = filters[1].Replace("*.", "").Split(';').ToList();

            FileDialogFilter filter = new() { Name = name, Extensions = extensions };

            Avalonia.Controls.OpenFileDialog dialog = new()
            {
                Title = Title,
                Filters = new List<FileDialogFilter>() { filter },
                Directory = InitialDirectory,
            };

            using CancellationTokenSource source = new();
            Task<string[]?> task = dialog.ShowAsync(MainWindow.DefaultWindow);
            task.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(source.Token);
            string result = task.Result?.FirstOrDefault() ?? "";

            MainWindow.DefaultWindow.Topmost = false;
            MainWindow.Instance.UnlockClick();

            if (result != "")
            {
                FileName = result;
                return DialogResult.OK;
            }
            else
                return DialogResult.Cancel;
        }

        public DialogResult RunWithSetting(Setting<string> directory, out string fileName)
        {
            if (directory.Value != "")
                InitialDirectory = directory.Value;

            DialogResult result = ShowDialog();

            if (result == DialogResult.OK)
                directory.Value = Path.GetDirectoryName(FileName) ?? "";

            fileName = FileName;
            return result;
        }
    }
}
