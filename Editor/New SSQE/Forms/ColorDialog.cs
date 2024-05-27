using System.Drawing;
using Egorozh.ColorPicker.Dialog;
using Avalonia.Threading;
using Avalonia.Controls;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using OpenTK.Windowing.GraphicsLibraryFramework;

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

            dialog.Show();
            BackgroundWindow.YieldWindow(dialog);

            Color = Color.FromArgb(dialog.Color.R, dialog.Color.G, dialog.Color.B);

            MainWindow.Instance.UnlockClick();
            return Result;
        }
    }
}
