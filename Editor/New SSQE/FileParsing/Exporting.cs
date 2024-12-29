using New_SSQE.ExternalUtils;
using New_SSQE.GUI;
using New_SSQE.Misc.Dialogs;
using New_SSQE.Preferences;

namespace New_SSQE.FileParsing
{
    internal class Exporting
    {
        public static Dictionary<string, string> Info = new()
        {
            {"songId", "" },
            {"mapName", "" },
            {"mappers", "" },
            {"coverPath", "" },
            {"difficulty", "" },
            {"customDifficulty", "" }
        };

        public static Dictionary<string, byte> Difficulties = new()
        {
            {"N/A", 0x00 },
            {"Easy", 0x01 },
            {"Medium", 0x02 },
            {"Hard", 0x03 },
            {"Logic", 0x04 },
            {"Tasukete", 0x05 }
        };

        public static void ExportSSPM()
        {
            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
            {
                DialogResult result = new SaveFileDialog()
                {
                    Title = "Export SSPM",
                    Filter = "Rhythia Maps (*.sspm)|*.sspm",
                    InitialFileName = $"{Info["songId"]}.sspm"
                }.RunWithSetting(Settings.exportPath, out string fileName);

                if (result == DialogResult.OK)
                {
                    try
                    {
                        Parser.SaveSSPM(fileName);

                        editor.ShowToast("SUCCESSFULLY EXPORTED", Settings.color1.Value);
                    }
                    catch (Exception ex)
                    {
                        Logging.Register("Failed to export", LogSeverity.WARN, ex);
                        MessageBox.Show($"Failed to export SSPM:\n\n{ex.Message}", MBoxIcon.Warning, MBoxButtons.OK);
                    }
                }
            }
        }

        public static Dictionary<string, string> NovaInfo = new()
        {
            {"songOffset", "" },
            {"songTitle", "" },
            {"songArtist", "" },
            {"mapCreator", "" },
            {"mapCreatorPersonalLink", "" },
            {"previewStartTime", "" },
            {"previewDuration", "" },
            {"coverPath", "" },
            {"iconPath", "" }
        };

        public static void ExportNOVA()
        {
            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
            {
                string mapper = NovaInfo["mapCreator"].ToLower().Replace(" ", "_");
                string title = NovaInfo["songTitle"].ToLower().Replace(" ", "_");
                string artist = NovaInfo["songArtist"].ToLower().Replace(" ", "_");

                DialogResult result = new SaveFileDialog()
                {
                    Title = "Export NOVA",
                    Filter = "Nova Maps (*.npk)|*.npk",
                    InitialFileName = $"{mapper}_-_{artist}_-_{title}"
                }.RunWithSetting(Settings.exportPath, out string fileName);

                if (result == DialogResult.OK)
                {
                    try
                    {
                        Parser.SaveNOVA(fileName);

                        editor.ShowToast("SUCCESSFULLY EXPORTED", Settings.color1.Value);
                    }
                    catch (Exception ex)
                    {
                        Logging.Register("Failed to export", LogSeverity.WARN, ex);
                        MessageBox.Show($"Failed to export NPK:\n\n{ex.Message}", MBoxIcon.Warning, MBoxButtons.OK);
                    }
                }
            }
        }

        private static readonly char[] invalidChars = { '/', '\\', ':', '*', '?', '"', '<', '>', '|', ',', '`' };

        public static string FixID(string id)
        {
            for (int i = 0; i < id.Length; i++)
            {
                if (Array.IndexOf(invalidChars, id[i]) > -1)
                    id = id.Remove(i, 1).Insert(i, "_");
            }

            return id;
        }
    }
}
