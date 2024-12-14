using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Interactivity;
using New_SSQE.Misc.Static;
using New_SSQE.Misc.Dialogs;

namespace New_SSQE
{
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

        public static DialogResult Show(string message, string icon, params string[] buttons)
        {
            Instance?.Close();
            Result = DialogResult.Cancel;

            iconPath = icon;
            MessageBox box = new();

            box.Text.Text = message;

            for (int i = 0; i < buttons.Length; i++)
                box.GetControl<Button>($"{buttons[i]}{buttons.Length - i}").IsVisible = true;

            box.Show();
            BackgroundWindow.YieldWindow(box);

            return Result;
        }
    }
}
