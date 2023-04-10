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
        }
    }
}
