using New_SSQE.ExternalUtils;
using New_SSQE.GUI.Font;
using New_SSQE.Maps;
using New_SSQE.Misc.Dialogs;
using New_SSQE.Misc.Network;
using New_SSQE.Misc.Static;
using New_SSQE.Preferences;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiWindowMenu : GuiWindow
    {
        private readonly GuiLabel clLabel = new(60, 195, 200, 40, "CHANGELOG", 42, "square", false);
        private readonly GuiLabel ssLabel = new(35, 0, 750, 100, "SOUND SPACE", 156, "square", false);
        private readonly GuiLabel qeLabel = new(615, 140, 150, 40, "QUANTUM EDITOR", 38, "square", false);
        private readonly GuiLabel ChangelogLabel = new(60, 230, 890, 715, "", 20, "main", false);

        private readonly GuiButton CreateButton = new(1190, 180, 600, 100, 0, "CREATE NEW MAP", 54, "square");
        private readonly GuiButton LoadButton = new(1190, 295, 600, 100, 1, "LOAD MAP", 54, "square");
        private readonly GuiButton ImportButton = new(1190, 410, 600, 100, 2, "PASTE MAP", 54, "square");
        private readonly GuiButton SettingsButton = new(1190, 525, 600, 100, 3, "SETTINGS", 54, "square");

        private readonly GuiButton FeedbackButton = new(35, 140, 100, 40, 8, "Feedback?", 20);

        private readonly GuiButton AutosavedButton = new(1190, 640, 600, 100, 4, "AUTOSAVED MAP", 54, "square");
        private readonly GuiButton LastMapButton = new(1190, 755, 600, 100, 5, "EDIT LAST MAP", 54, "square");

        private readonly GuiSlider ChangelogSlider = new(950, 230, 20, 720, Settings.changelogPosition, true);

        private readonly GuiSquare ChangelogBackdrop1 = new(35, 180, 950, 790, Color.FromArgb(40, 0, 0, 0));
        private readonly GuiSquare ChangelogBackdrop2 = new(55, 230, 900, 715, Color.FromArgb(50, 0, 0, 0));

        public readonly GuiSquare MapSelectBackdrop = new(0, 1040, 1920, 40, Color.FromArgb(50, 0, 0, 0));
        private readonly GuiButton NavLeft = new(0, 1040, 40, 40, 6, "<", 34);
        private readonly GuiButton NavRight = new(1880, 1040, 40, 40, 7, ">", 34);

        private readonly GuiButton MapSelect0 = new(40, 1040, 368, 40, 80, "", 20);
        private readonly GuiButton MapSelect1 = new(408, 1040, 368, 40, 81, "", 20);
        private readonly GuiButton MapSelect2 = new(776, 1040, 368, 40, 82, "", 20);
        private readonly GuiButton MapSelect3 = new(1144, 1040, 368, 40, 83, "", 20);
        private readonly GuiButton MapSelect4 = new(1512, 1040, 368, 40, 84, "", 20);

        private readonly GuiButton MapClose0 = new(40, 1040, 40, 40, 90, "X", 26);
        private readonly GuiButton MapClose1 = new(408, 1040, 40, 40, 91, "X", 26);
        private readonly GuiButton MapClose2 = new(776, 1040, 40, 40, 92, "X", 26);
        private readonly GuiButton MapClose3 = new(1144, 1040, 40, 40, 93, "X", 26);
        private readonly GuiButton MapClose4 = new(1512, 1040, 40, 40, 94, "X", 26);

        private readonly List<(GuiButton, GuiButton)> mapSelects;
        private readonly string?[] prevTexts = new string?[8];

        private int lastAssembled = 0;
        private int mapOffset = 0;
        private readonly string changelogText;

        public GuiWindowMenu() : base(0, 0, MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y)
        {
            Controls = new List<WindowControl>
            {
                // Squares
                ChangelogBackdrop1, ChangelogBackdrop2, MapSelectBackdrop,
                // Buttons
                CreateButton, LoadButton, ImportButton, SettingsButton, AutosavedButton, LastMapButton, NavLeft, NavRight,
                MapSelect0, MapSelect1, MapSelect2, MapSelect3, MapSelect4, FeedbackButton,
                MapClose0, MapClose1, MapClose2, MapClose3, MapClose4,
                // Sliders
                ChangelogSlider,
                // Labels
                clLabel, ssLabel, qeLabel, ChangelogLabel
            };

            mapSelects = new List<(GuiButton, GuiButton)>
            {
                (MapSelect0, MapClose0),
                (MapSelect1, MapClose1),
                (MapSelect2, MapClose2),
                (MapSelect3, MapClose3),
                (MapSelect4, MapClose4),
            };
            
            BackgroundSquare = new(Color.FromArgb(255, 30, 30, 30), "background_menu.png", "menubg");
            Init();

            if (File.Exists("background_menu.png"))
            {
                ChangelogBackdrop1.Color = Color.FromArgb(120, 57, 56, 47);
                ChangelogBackdrop2.Color = Color.FromArgb(100, 36, 35, 33);
                MapSelectBackdrop.Color = Color.FromArgb(100, 36, 35, 33);
            }

            try
            {
                changelogText = WebClient.DownloadString(Links.CHANGELOG);
            }
            catch { changelogText = "Failed to load changelog"; }

            OnResize(MainWindow.Instance.ClientSize);
            AssembleMapList();
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if ((int)Settings.changelogPosition.Value.Value != lastAssembled)
            {
                AssembleChangelog();

                lastAssembled = (int)Settings.changelogPosition.Value.Value;
            }

            AutosavedButton.Visible = Settings.autosavedFile.Value != "";
            LastMapButton.Visible = Settings.lastFile.Value != "" && File.Exists(Settings.lastFile.Value);

            for (int i = 0; i < mapSelects.Count; i++)
            {
                GuiButton select = mapSelects[i].Item1;
                GuiButton close = mapSelects[i].Item2;

                close.Visible = select.Visible && select.Rect.Contains(mousex, mousey);
                select.Text = close.Visible ? "Open Map" : prevTexts[i] ?? "";
            }

            base.Render(mousex, mousey, frametime);
        }

        public override void OnResize(Vector2i size)
        {
            base.OnResize(size);

            LastMapButton.Rect.Y = Settings.autosavedFile.Value == "" ? AutosavedButton.Rect.Y : LastMapButton.Rect.Y;
            LastMapButton.Update();

            AssembleChangelog();
            Settings.changelogPosition.Value.Value = Settings.changelogPosition.Value.Max;
        }

        private void AssembleChangelog()
        {
            float widthdiff = Rect.Width / 1920f;
            float heightdiff = Rect.Height / 1080f;

            string result = "";
            List<string> lines = new();

            foreach (string line in changelogText.Split('\n'))
            {
                string lineedit = line;

                while (FontRenderer.GetWidth(lineedit, ChangelogLabel.TextSize, "main") > 890 * widthdiff && lineedit.Contains(' '))
                {
                    int index = lineedit.LastIndexOf(' ');

                    if (FontRenderer.GetWidth(lineedit[..index], ChangelogLabel.TextSize, "main") <= 890 * widthdiff)
                        lineedit = lineedit.Remove(index, 1).Insert(index, "\n");
                    else
                        lineedit = lineedit.Remove(index, 1).Insert(index, "\\");
                }

                lineedit = lineedit.Replace("\\", " ");

                foreach (string newline in lineedit.Split('\n'))
                    lines.Add(newline);
            }

            SliderSetting setting = Settings.changelogPosition.Value;

            setting.Max = lines.Count - (int)(715f * heightdiff / ChangelogLabel.TextSize);
            ChangelogSlider.Visible = setting.Max > 0;

            for (int i = 0; i < lines.Count; i++)
                if (i >= setting.Max - setting.Value && i < setting.Max - setting.Value + 715f * heightdiff / ChangelogLabel.TextSize / (FontRenderer.unicode ? StbFont.UnicodeMult : 1) - 1)
                    result += $"{lines[i]}\n";

            ChangelogLabel.Text = result;
        }

        public void AssembleMapList()
        {
            mapOffset = MathHelper.Clamp(mapOffset, 0, MapManager.Cache.Count - mapSelects.Count);

            NavLeft.Visible = mapOffset > 0;
            NavRight.Visible = mapOffset < MapManager.Cache.Count - mapSelects.Count;

            for (int i = 0; i < mapSelects.Count; i++)
            {
                GuiButton button = mapSelects[i].Item1;
                button.Visible = i + mapOffset < MapManager.Cache.Count;

                if (button.Visible)
                {
                    Map map = MapManager.Cache[i + mapOffset];
                    string fileName = (!map.IsSaved ? "[!] " : "") + map.FileID;

                    button.Text = FontRenderer.TrimText(fileName, button.TextSize, (int)button.Rect.Width - 10, button.Font);
                    prevTexts[i] = button.Text;
                }
            }
        }

        public override void OnButtonClicked(int id)
        {
            MainWindow editor = MainWindow.Instance;

            switch (id)
            {
                case 0:
                    editor.SwitchWindow(new GuiWindowCreate());

                    break;

                case 1:
                    DialogResult result = new OpenFileDialog()
                    {
                        Title = "Select Map File",
                        Filter = "Map Files (*.txt;*.sspm;*.osu;*.nch;*.npk;*.phxm;*.phz;*.json)|*.txt;*.sspm;*.osu;*.nch;*.npk;*.phxm;*.phz;*.json"
                    }.RunWithSetting(Settings.defaultPath, out string fileName);

                    if (result == DialogResult.OK)
                        MapManager.Load(fileName, true);

                    break;

                case 2:
                    string clipboard = Clipboard.GetText();

                    if (!string.IsNullOrWhiteSpace(clipboard))
                        MapManager.Load(clipboard, false, false, MainWindow.DebugVersion && MainWindow.Instance.AltHeld);

                    break;

                case 3:
                    editor.SwitchWindow(new GuiWindowSettings());

                    break;

                case 4:
                    string autosavedFile = Settings.autosavedFile.Value;

                    if (autosavedFile != "")
                        MapManager.Load(autosavedFile, false, true);

                    break;

                case 5:
                    string lastFile = Settings.lastFile.Value;

                    if (lastFile != "" && File.Exists(lastFile))
                        MapManager.Load(lastFile, true);

                    break;

                case 6:
                    if (mapOffset > 0)
                    {
                        mapOffset--;
                        AssembleMapList();
                    }

                    break;

                case 7:
                    if (mapOffset < MapManager.Cache.Count - mapSelects.Count)
                    {
                        mapOffset++;
                        AssembleMapList();
                    }

                    break;

                case 8:
                    Platform.OpenLink(Links.FEEDBACK_FORM);

                    break;

                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                    int indexM = id % 80 + mapOffset;

                    if (indexM >= 0 && indexM < MapManager.Cache.Count)
                        MapManager.Load(MapManager.Cache[indexM]);

                    break;

                case 90:
                case 91:
                case 92:
                case 93:
                case 94:
                    int indexC = id % 90 + mapOffset;

                    if (indexC >= 0 && indexC < MapManager.Cache.Count)
                    {
                        MapManager.Cache[indexC].Close(false);
                        AssembleMapList();
                    }

                    break;
            }

            base.OnButtonClicked(id);
        }

        public void ScrollMaps(bool up)
        {
            if (up && mapOffset < MapManager.Cache.Count - mapSelects.Count)
                mapOffset++;
            else if (!up && mapOffset > 0)
                mapOffset--;

            AssembleMapList();
        }
    }
}
