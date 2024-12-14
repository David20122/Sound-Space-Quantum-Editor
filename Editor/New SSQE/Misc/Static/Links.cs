using New_SSQE.ExternalUtils;

namespace New_SSQE.Misc.Static
{
    internal class Links
    {
        private static readonly Dictionary<string, string> windowsLinks = new()
        {
            {"SSQE Player Version", "https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/player_version" },
            {"SSQE Player Zip", "https://github.com/David20122/Sound-Space-Quantum-Editor/raw/2.0%2B_rewrite/SSQE%20Player.zip" },
            {"SSQE Updater Version", "https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/updater_version" },
            {"SSQE Updater Zip", "https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/SSQE%20Updater.zip" }
        };

        private static readonly Dictionary<string, string> linuxLinks = new()
        {
            {"SSQE Player Version", "https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/player_version" },
            {"SSQE Player Zip", "https://github.com/David20122/Sound-Space-Quantum-Editor/raw/2.0%2B_rewrite/SSQE%20Player-linux.zip" },
            {"SSQE Updater Version", "https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/updater_version" },
            {"SSQE Updater Zip", "https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/SSQE%20Updater-linux.zip" }
        };

        public static readonly string SSQE_PLAYER_VERSION = (Platform.IsLinux ? linuxLinks : windowsLinks)["SSQE Player Version"];
        public static readonly string SSQE_PLAYER_ZIP = (Platform.IsLinux ? linuxLinks : windowsLinks)["SSQE Player Zip"];
        public static readonly string SSQE_UPDATER_VERSION = (Platform.IsLinux ? linuxLinks : windowsLinks)["SSQE Updater Version"];
        public static readonly string SSQE_UPDATER_ZIP = (Platform.IsLinux ? linuxLinks : windowsLinks)["SSQE Updater Zip"];

        public static readonly string EDITOR_REDIRECT = "https://github.com/David20122/Sound-Space-Quantum-Editor/releases/latest";
        public static readonly string CHANGELOG = "https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/changelog";

        public static readonly string FEEDBACK_FORM = "https://forms.gle/Rh4RXKT9KyttJ9Dc6";

        public static readonly Dictionary<string, string> ALL = new()
        {
            {"SSQE Player Version", SSQE_PLAYER_VERSION },
            {"SSQE Player Zip", SSQE_PLAYER_ZIP },
            {"SSQE Updater Version", SSQE_UPDATER_VERSION },
            {"SSQE Updater Zip", SSQE_UPDATER_ZIP }
        };

        public static readonly string NEW_GITHUB_ISSUE = $"https://github.com/David20122/Sound-Space-Quantum-Editor/issues/new?title=SSQE+Crash+-+{Program.Version}&body=" +
            $"Upload+your+crash+report+and+describe+how+it+happened+here&labels=bug";
    }
}
