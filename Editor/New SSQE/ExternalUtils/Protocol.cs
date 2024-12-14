using Microsoft.Win32;
using New_SSQE.Misc.Static;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace New_SSQE.ExternalUtils
{
    internal static class Protocol
    {
        private static readonly string protocol = "ssqe";
        private static readonly string protocolUrl = $"\"{Assets.EXE}\" \"%1\"";

        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
        public static void Register()
        {
            if (Platform.IsLinux)
                return;

            RegistryKey? key = Registry.ClassesRoot.OpenSubKey(protocol);

            if (key != null)
                key = key.OpenSubKey(@"shell\open\command");

            if (key == null || key.GetValue(string.Empty)?.ToString() != protocolUrl)
            {
                ProcessStartInfo proc = new()
                {
                    UseShellExecute = true,
                    WorkingDirectory = Assets.THIS,
                    FileName = Assets.EXE,
                    Verb = "runas",
                    Arguments = "registerProtocol"
                };

                Process.Start(proc);
            }
        }

        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
        public static void Finish()
        {
            if (Platform.IsLinux)
                return;

            RegistryKey key = Registry.ClassesRoot.CreateSubKey(protocol);
            key.SetValue(string.Empty, $"URL: {protocol}");
            key.SetValue("URL Protocol", string.Empty);

            RegistryKey defaultIcon = key.CreateSubKey(@"DefaultIcon");
            defaultIcon.SetValue(string.Empty, $"\"{Assets.FILE_NAME},1\"");
            defaultIcon.Close();

            key = key.CreateSubKey(@"shell\open\command");
            key.SetValue(string.Empty, protocolUrl);
            key.Close();
        }
    }
}
