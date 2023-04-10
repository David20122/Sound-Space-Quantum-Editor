using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace New_SSQE
{
    // turns forms SaveFileDialog info into Avalonia method
    internal class SaveFileDialog
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

            var dialog = new Avalonia.Controls.SaveFileDialog()
            {
                Title = Title,
                Filters = new List<FileDialogFilter>() { filter },
                Directory = InitialDirectory,
            };

            var result = "";

            if (MainWindow.DefaultWindow.PlatformImpl.Handle is IMacOSTopLevelPlatformHandle handle)
            {
                var nsAppStaticClass = objc_getClass("NSApplication");
                var sharedApplicationSelector = GetHandle("sharedApplication");
                var sharedApplication = IntPtr_objc_msgSend(nsAppStaticClass, sharedApplicationSelector);
                var runModalForSelector = GetHandle("runModalForWindow:");
                var stopModalSelector = GetHandle("stopModal");

                var task = dialog.ShowAsync(MainWindow.DefaultWindow);
                Int64_objc_msgSend_IntPtr(sharedApplication, runModalForSelector, handle.NSWindow);
                var final = task.Result;
                Void_objc_msgSend(sharedApplication, stopModalSelector);
                result = final ?? "";
            }
            else
            {
                using var source = new CancellationTokenSource();
                var task = Task.Run(async () =>
                {
                    var final = await dialog.ShowAsync(MainWindow.DefaultWindow);

                    return final ?? "";
                }).ContinueWith(final =>
                {
                    source.Cancel();

                    return final.Result;
                });
                Dispatcher.UIThread.MainLoop(source.Token);
                result = task.Result;
            }

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
