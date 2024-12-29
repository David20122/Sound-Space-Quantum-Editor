using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Interactivity;
using New_SSQE.Misc.Static;
using New_SSQE.Misc.Dialogs;

namespace New_SSQE
{
    public enum MBoxButtons
    {
        OK,
        Yes_No,
        Yes_No_Cancel,
        OK_Cancel
    }

    public enum MBoxIcon
    {
        Info,
        Warning,
        Error
    }

    public partial class MessageBox : Window
    {
        public static MessageBox Instance;
        private static string iconPath;

        public MessageBox()
        {
            Instance = this;
            Result = DialogResult.Cancel;

            Icon = new WindowIcon(new Bitmap($"{Assets.TEXTURES}\\Empty.png"));
            Resources["iconPath"] = $"{Assets.TEXTURES}\\{iconPath}.png";

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

        public static DialogResult Show(string message, MBoxIcon icon, MBoxButtons buttons)
        {
            Instance?.Close();
            Result = DialogResult.Cancel;

            iconPath = icon.ToString();
            MessageBox box = new();

            box.Text.Text = message;

            string[] buttonStr = buttons.ToString().Split('_');
            for (int i = 0; i < buttonStr.Length; i++)
                box.GetControl<Button>($"{buttonStr[i]}{buttonStr.Length - i}").IsVisible = true;

            box.Show();
            BackgroundWindow.YieldWindow(box);

            return Result;
        }
    }
}
