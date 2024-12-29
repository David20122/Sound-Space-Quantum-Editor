using DiscordRPC;
using DiscordRPC.Logging;
using New_SSQE.ExternalUtils;
using New_SSQE.GUI;
using New_SSQE.Maps;

namespace New_SSQE.Misc.Network
{
    internal enum DiscordStatus
    {
        None,
        Menu,
        Editor
    }

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

        private static double time = 0;

        public static void Process(double frametime)
        {
            time += frametime;

            if (time >= 5)
            {
                if (MainWindow.Instance.CurrentWindow is GuiWindowEditor)
                    SetActivity(DiscordStatus.Editor);
                time %= 5;
            }
        }

        private static DiscordStatus prevStatus = DiscordStatus.None;
        private static DateTime prevTimestamp = DateTime.UtcNow;

        public static void SetActivity(DiscordStatus status)
        {
            if (!enabled)
                return;

            string details = status switch
            {
                DiscordStatus.Menu => "Watching the sunset",
                DiscordStatus.Editor => $"Editing a map - {CurrentMap.Notes.Count} notes",
                _ => ""
            };

            string state = status switch
            {
                DiscordStatus.Editor => CurrentMap.FileID[..Math.Min(CurrentMap.FileID.Length, 128)],
                _ => ""
            };

            if (prevStatus != status)
            {
                prevTimestamp = DateTime.UtcNow;
                prevStatus = status;
            }

            client?.SetPresence(new RichPresence
            {
                Details = details,
                State = state,
                Timestamps = new() { Start = prevTimestamp },
                Assets = new() { LargeImageKey = "logo", LargeImageText = $"Version {Program.Version}{(MainWindow.DebugVersion ? "-pre" : "")}" }
            });
        }

        public static void Dispose()
        {
            if (enabled)
                try { client?.Dispose(); } catch { }
        }
    }
}
