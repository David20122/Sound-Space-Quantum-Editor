using New_SSQE.Preferences;

namespace New_SSQE.ExternalUtils
{
    // for debugging purposes
    internal class Logging
    {
        public static List<string> Logs = new();

        public static void Register(string log, LogSeverity severity = LogSeverity.INFO, Exception? ex = null)
        {
            if (ex != null)
                log += $"\n{ex}";

            DateTime timestamp = DateTime.Now;
            string logF = $"[{timestamp} - {severity}] {log}";

            Logs.Add(logF);

            if (Settings.debugMode.Value || MainWindow.DebugVersion)
            {
                try
                {
                    string logs = string.Join('\n', Logs);
                    File.WriteAllText("logs-debug.txt", logs);
                }
                catch { }
            }
        }
    }
}
