using System;
using System.Windows.Forms;
using System.Globalization;

namespace SSQE_Player
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // debug line
            //args = new string[] { "true" };

            if (args.Length == 0)
                return;

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            try
            {
                Application.SetCompatibleTextRenderingDefault(false);

                MainWindow window = new MainWindow(bool.Parse(args[0]));

                using (window)
                    window.Run();
            }
            catch (Exception e)
            {
                if (MainWindow.Instance != null)
                    MainWindow.Instance.CursorVisible = true;

                MessageBox.Show($"An exception has occurred while trying to run player\n\n{e.Message}\n\n{e.StackTrace}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
