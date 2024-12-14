using DiscordRPC;
using DiscordRPC.Logging;
using New_SSQE.ExternalUtils;

namespace New_SSQE.Misc.Network
{
    internal class DiscordManager
    {
        private static DiscordRpcClient? client;
        private static bool enabled = true;

        public static void Init()
        {
            try
            {
                client = new("1067849747710345346", -1)
                {
                    Logger = new ConsoleLogger() { Level = LogLevel.Warning }
                };

                client.OnReady += (sender, e) =>
                {
                    Logging.Register($"Discord integration ready");
                };

                client.OnPresenceUpdate += (sender, e) =>
                {
                    Logging.Register($"Discord integration updated with activity '{e.Presence.State}'");
                };

                client.Initialize();
            }
            catch { enabled = false; }
        }

        public static void SetActivity(string status)
        {
            if (!enabled)
                return;

            client?.SetPresence(new RichPresence
            {
                State = status,
                Details = $"Version {Program.Version}{(MainWindow.DebugVersion ? "-pre" : "")}",
                Timestamps = new() { Start = DateTime.UtcNow },
                Assets = new() { LargeImageKey = "logo" }
            });
        }

        public static void Dispose()
        {
            if (enabled)
                try { client?.Dispose(); } catch { }
        }
    }
}
