using System.Drawing;
using Egorozh.ColorPicker.Dialog;
using Avalonia.Threading;
using System;
using Avalonia.Platform;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls;
using System.Threading.Tasks;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace New_SSQE
{
    internal class ColorDialog
    {
        public Color Color;

        private static DialogResult Result;
        private static TaskCompletionSource<bool>? tcs = new();

        public DialogResult ShowDialog()
        {
            MainWindow.Instance.LockClick();

            var dialog = new ColorPickerDialog()
            {
                Color = Avalonia.Media.Color.FromArgb(255, Color.R, Color.G, Color.B),
                Icon = new WindowIcon(new Bitmap("assets/textures/Empty.png")),
                Topmost = true
            };

            Result = DialogResult.Cancel;
            tcs = new();

            var okControl = dialog.GetControl<Button>("btOk");
            okControl.Click += (s, e) =>
            {
                Result = DialogResult.OK;
                tcs.TrySetResult(true);
            };
            var cancelControl = dialog.GetControl<Button>("btCancel");
            cancelControl.Click += (s, e) =>
            {
                Result = DialogResult.Cancel;
                tcs.TrySetResult(true);
            };

            if (MainWindow.DefaultWindow.PlatformImpl.Handle is IMacOSTopLevelPlatformHandle handle)
            {
                var finished = ShowColorDialogOnMac.Show<bool>(dialog, handle);
                Color = Color.FromArgb(dialog.Color.R, dialog.Color.G, dialog.Color.B);

                MainWindow.Instance.UnlockClick();
                return Result;
            }
            else
            {
                dialog.Show();

                using var source = new CancellationTokenSource();

                var task = Task.Run(async () =>
                {
                    dialog.Closed += (s, e) => tcs.TrySetResult(true);

                    return await tcs.Task;
                }).ContinueWith(t =>
                {
                    source.Cancel();

                    return true;
                });

                Dispatcher.UIThread.MainLoop(source.Token);

                var final = task.Result;

                Color = Color.FromArgb(dialog.Color.R, dialog.Color.G, dialog.Color.B);

                MainWindow.Instance.UnlockClick();
                return Result;
            }
        }
    }

    internal static class ShowColorDialogOnMac
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

        public static T Show<T>(ColorPickerDialog window, IMacOSTopLevelPlatformHandle handle)
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
