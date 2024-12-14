using Avalonia.Controls;
using Avalonia.Threading;

namespace New_SSQE
{
    public partial class BackgroundWindow : Window
    {
        public BackgroundWindow()
        {
            InitializeComponent();
        }

        private static TaskCompletionSource<bool>? tcs = new();

        public static void YieldWindow(Window window)
        {
            using CancellationTokenSource source = new();

            Task<bool> task = Task.Run(async () =>
            {
                tcs = new();
                window.Closed += (s, e) => tcs.TrySetResult(true);

                return await tcs.Task;
            }).ContinueWith(t =>
            {
                source.Cancel();

                return true;
            });

            Dispatcher.UIThread.MainLoop(source.Token);

            bool final = task.Result;
            return;
        }
    }
}
