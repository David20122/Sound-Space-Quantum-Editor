using System.Globalization;

namespace SSQE_Player
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // uncomment if debugging
            //args = new string[1] { "true" };


            if (args.Length == 0) { return; }
            if (!File.Exists("assets/temp/tempmap.txt")) { return; }

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            try
            {
                MainWindow window = new(bool.Parse(args[0]));

                using (window)
                    window.Run();
            }
            catch { }
        }
    }
}