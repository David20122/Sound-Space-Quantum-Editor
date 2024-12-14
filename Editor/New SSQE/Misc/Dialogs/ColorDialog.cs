using System.Drawing;
using Egorozh.ColorPicker.Dialog;
using Avalonia.Controls;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using New_SSQE.Misc.Static;

namespace New_SSQE.Misc.Dialogs
{
    internal class ColorDialog
    {
        public Color Color;

        private static DialogResult Result;
        private static TaskCompletionSource<bool>? tcs = new();

        public DialogResult ShowDialog()
        {
            MainWindow.Instance.LockClick();

            ColorPickerDialog dialog = new()
            {
                Color = Avalonia.Media.Color.FromArgb(255, Color.R, Color.G, Color.B),
                Icon = new(new Bitmap($"{Assets.TEXTURES}\\Empty.png")),
                Topmost = true
            };

            Result = DialogResult.Cancel;
            tcs = new();

            Button okControl = dialog.GetControl<Button>("btOk");
            okControl.Click += (s, e) =>
            {
                Result = DialogResult.OK;
                tcs.TrySetResult(true);
            };

            Button cancelControl = dialog.GetControl<Button>("btCancel");
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
