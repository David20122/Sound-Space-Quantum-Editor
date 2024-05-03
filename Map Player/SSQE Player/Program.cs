using System.Globalization;

namespace SSQE_Player
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0) { args = new string[1] { "true" }; }
                if (!File.Exists("assets/temp/tempmap.txt")) { return; }

                var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                culture.NumberFormat.NumberDecimalSeparator = ".";

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                MainWindow window = new(bool.Parse(args[0]));

                using (window)
                    window.Run();
            }
            catch (Exception ex)
            {
                File.WriteAllText("player-crash-report.txt", ex.ToString());
            }
        }
    }
}