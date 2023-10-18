using System;
using System.Collections.Generic;

namespace New_SSQE
{
    // for debugging purposes
    internal class ActionLogging
    {
        public static List<string> Logs = new();

        public static void Register(string log, string tag = "INFO")
        {
            var timestamp = DateTime.Now;
            var logF = $"[{timestamp} - {tag.ToUpper()}] {log}";

            Logs.Add(logF);

            if (Settings.settings["debugMode"])
            {
                var logs = string.Join('\n', Logs);
                File.WriteAllText("logs-debug.txt", logs);
            }
        }
    }
}
