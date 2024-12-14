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
                if (args.Length == 0) { args = ["true", "false", "false"]; }
                if (args.Length == 1) { args = [args[0], "false", "false"]; }
                if (args.Length == 2) { args = [args[0], args[1], "false"]; }
                if (!File.Exists("assets/temp/tempmap.txt")) { return; }
                
                if (bool.Parse(args[1]) && File.Exists("assets/temp/tempreplay.qer"))
                    MainWindow.Replay = true;
                MainWindow.Autoplay = bool.Parse(args[2]) && !MainWindow.Replay;

                CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                culture.NumberFormat.NumberDecimalSeparator = ".";

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Settings.Load();
                MainWindow window = new(bool.Parse(args[0]), Settings.msaa.Value ? 32 : 0);

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