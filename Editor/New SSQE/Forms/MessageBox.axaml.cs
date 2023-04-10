using Avalonia.Controls;
using System.Runtime.InteropServices;
using System;
using Avalonia.Platform;
using System.Threading;
using Avalonia.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Interactivity;

namespace New_SSQE
{
    public partial class MessageBox : Window
    {
        private static MessageBox Instance;
        private static string iconPath;

        public MessageBox()
        {
            Instance = this;
            Result = DialogResult.Cancel;

            Icon = new WindowIcon(new Bitmap("assets/textures/Empty.png"));
            Resources["iconPath"] = $"assets/textures/{iconPath}.png";
            
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.Yes;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.No;
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }

        private static DialogResult Result;
        private static TaskCompletionSource<bool>? tcs = new();

        public static DialogResult Show(string message, string icon, params string[] buttons)
        {
            Instance?.Close();
            Result = DialogResult.Cancel;

            iconPath = icon;
            var box = new MessageBox();

            box.Text.Text = message;

            for (int i = 0; i < buttons.Length; i++)
                box.GetControl<Button>($"{buttons[i]}{buttons.Length - i}").IsVisible = true;

            if (MainWindow.DefaultWindow.PlatformImpl.Handle is IMacOSTopLevelPlatformHandle handle)
            {
                var finished = ShowMessageBoxOnMac.Show<bool>(box, handle);

                return Result;
            }
            else
            {
                box.Show();

                using var source = new CancellationTokenSource();

                var task = Task.Run(async () =>
                {
                    tcs = new();
                    box.Closed += (s, e) => tcs.TrySetResult(true);

                    return await tcs.Task;
                }).ContinueWith(t =>
                {
                    source.Cancel();

                    return true;
                });

                Dispatcher.UIThread.MainLoop(source.Token);
                
                var final = task.Result;
                return Result;
            }
        }
    }

    internal static class ShowMessageBoxOnMac
    {
        [DllImport("/usr/lib/libobjc.dylib")]
        private static extern IntPtr objc_getClass(string name);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
        private static extern IntPtr GetHandle(string name);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern long Int64_objc_msgSend_IntPtr(
            IntPtr receiver,
            IntPtr selector,
            IntPtr arg1);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern void Void_objc_msgSend(
            IntPtr receiver,
            IntPtr selector);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

        public static T Show<T>(this Window window, IMacOSTopLevelPlatformHandle handle)
        {
            var nsAppStaticClass = objc_getClass("NSApplication");
            var sharedApplicationSelector = GetHandle("sharedApplication");
            var sharedApplication = IntPtr_objc_msgSend(nsAppStaticClass, sharedApplicationSelector);
            var runModalForSelector = GetHandle("runModalForWindow:");
            var stopModalSelector = GetHandle("stopModal");

            var task = window.ShowDialog<T>(MainWindow.DefaultWindow);
            Int64_objc_msgSend_IntPtr(sharedApplication, runModalForSelector, handle.NSWindow);
            var final = task.Result;
            Void_objc_msgSend(sharedApplication, stopModalSelector);

            return final;
        }
    }
}
