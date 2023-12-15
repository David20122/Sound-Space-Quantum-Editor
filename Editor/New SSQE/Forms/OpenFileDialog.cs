using Avalonia.Controls;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace New_SSQE
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

            var filters = (Filter ?? "").Split('|');
            var name = filters[0];
            var extensions = filters[1].Replace("*.", "").Split(';').ToList();

            var filter = new FileDialogFilter() { Name = name, Extensions = extensions };

            var dialog = new Avalonia.Controls.OpenFileDialog()
            {
                Title = Title,
                Filters = new List<FileDialogFilter>() { filter },
                Directory = InitialDirectory,
            };

            using var source = new CancellationTokenSource();
            var task = dialog.ShowAsync(MainWindow.DefaultWindow);
            task.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(source.Token);
            var result = task.Result?.FirstOrDefault() ?? "";

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
    }
}
