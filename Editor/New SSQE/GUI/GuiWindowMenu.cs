using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace New_SSQE.GUI
{
    internal class GuiWindowMenu : GuiWindow
    {
        private readonly GuiLabel clLabel = new(60, 195, 200, 40, "CHANGELOG", 40, false, false, "square", false);
        private readonly GuiLabel ssLabel = new(35, 0, 750, 100, "SOUND SPACE", 150, false, false, "square", false);
        private readonly GuiLabel qeLabel = new(615, 140, 150, 40, "QUANTUM EDITOR", 36, false, false, "square", false);
        private readonly GuiLabel ChangelogLabel = new(60, 230, 890, 715, "", 18, false, false, "main", false);

        private readonly GuiButton CreateButton = new(1190, 180, 600, 100, 0, "CREATE NEW MAP", 52, false, false, "square");
        private readonly GuiButton LoadButton = new(1190, 295, 600, 100, 1, "LOAD MAP", 52, false, false, "square");
        private readonly GuiButton ImportButton = new(1190, 410, 600, 100, 2, "IMPORT MAP", 52, false, false, "square");
        private readonly GuiButton SettingsButton = new(1190, 525, 600, 100, 3, "SETTINGS", 52, false, false, "square");

        private readonly GuiButton AutosavedButton = new(1190, 640, 600, 100, 4, "AUTOSAVED MAP", 52, false, false, "square");
        private readonly GuiButton LastMapButton = new(1190, 755, 600, 100, 5, "EDIT LAST MAP", 52, false, false, "square");

        private readonly GuiSlider ChangelogSlider = new(950, 230, 20, 720, "changelogPosition", true);

        private readonly GuiSquare ChangelogBackdrop1 = new(35, 180, 950, 790, Color.FromArgb(40, 0, 0, 0));
        private readonly GuiSquare ChangelogBackdrop2 = new(55, 230, 900, 715, Color.FromArgb(50, 0, 0, 0));

        private readonly GuiSquare MapSelectBackdrop = new(0, 1040, 1920, 40, Color.FromArgb(50, 0, 0, 0));
        private readonly GuiButton NavLeft = new(0, 1040, 40, 40, 6, "<", 32);
        private readonly GuiButton NavRight = new(1880, 1040, 40, 40, 7, ">", 32);

        private readonly GuiButton MapSelect0 = new(40, 1040, 368, 40, 80, "", 18);
        private readonly GuiButton MapSelect1 = new(408, 1040, 368, 40, 81, "", 18);
        private readonly GuiButton MapSelect2 = new(776, 1040, 368, 40, 82, "", 18);
        private readonly GuiButton MapSelect3 = new(1144, 1040, 368, 40, 83, "", 18);
        private readonly GuiButton MapSelect4 = new(1512, 1040, 368, 40, 84, "", 18);

        private readonly GuiButton MapClose0 = new(40, 1040, 40, 40, 90, "X", 24);
        private readonly GuiButton MapClose1 = new(408, 1040, 40, 40, 91, "X", 24);
        private readonly GuiButton MapClose2 = new(776, 1040, 40, 40, 92, "X", 24);
        private readonly GuiButton MapClose3 = new(1144, 1040, 40, 40, 93, "X", 24);
        private readonly GuiButton MapClose4 = new(1512, 1040, 40, 40, 94, "X", 24);

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
                MapSelect0, MapSelect1, MapSelect2, MapSelect3, MapSelect4,
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
            
            BackgroundSquare = new(0, 0, 1920, 1080, Color.FromArgb(255, 30, 30, 30), false, "background_menu.png", "menubg");
            Init();

            if (File.Exists("background_menu.png"))
            {
                ChangelogBackdrop1.Color = Color.FromArgb(120, 57, 56, 47);
                ChangelogBackdrop2.Color = Color.FromArgb(100, 36, 35, 33);
                MapSelectBackdrop.Color = Color.FromArgb(100, 36, 35, 33);
            }

            try
            {
                changelogText = WebClient.DownloadString("https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/changelog");
            }
            catch { changelogText = "Failed to load changelog"; }

            OnResize(MainWindow.Instance.ClientSize);
            AssembleMapList();
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if ((int)Settings.settings["changelogPosition"].Value != lastAssembled)
            {
                AssembleChangelog();

                lastAssembled = (int)Settings.settings["changelogPosition"].Value;
            }

            AutosavedButton.Visible = Settings.settings["autosavedFile"] != "";
            LastMapButton.Visible = Settings.settings["lastFile"] != "" && File.Exists(Settings.settings["lastFile"]);

            for (int i = 0; i < mapSelects.Count; i++)
            {
                var select = mapSelects[i].Item1;
                var close = mapSelects[i].Item2;

                close.Visible = select.Visible && select.Rect.Contains(mousex, mousey);
                select.Text = close.Visible ? "Open Map" : prevTexts[i] ?? "";
            }

            base.Render(mousex, mousey, frametime);
        }

        public override void OnResize(Vector2i size)
        {
            Rect = new RectangleF(0, 0, size.X, size.Y);

            base.OnResize(size);

            LastMapButton.Rect.Y = Settings.settings["autosavedFile"] == "" ? AutosavedButton.Rect.Y : LastMapButton.Rect.Y;
            LastMapButton.Update();

            AssembleChangelog();
            Settings.settings["changelogPosition"].Value = Settings.settings["changelogPosition"].Max;
        }

        private void AssembleChangelog()
        {
            var widthdiff = Rect.Width / 1920f;
            var heightdiff = Rect.Height / 1080f;

            var result = "";
            var lines = new List<string>();

            foreach (var line in changelogText.Split('\n'))
            {
                var lineedit = line;

                while (FontRenderer.GetWidth(lineedit, ChangelogLabel.TextSize, "main") > 890 * widthdiff && lineedit.Contains(' '))
                {
                    var index = lineedit.LastIndexOf(' ');

                    if (FontRenderer.GetWidth(lineedit[..index], ChangelogLabel.TextSize, "main") <= 890 * widthdiff)
                        lineedit = lineedit.Remove(index, 1).Insert(index, "\n");
                    else
                        lineedit = lineedit.Remove(index, 1).Insert(index, "\\");
                }

                lineedit = lineedit.Replace("\\", " ");

                foreach (var newline in lineedit.Split('\n'))
                    lines.Add(newline);
            }

            var setting = Settings.settings["changelogPosition"];

            setting.Max = lines.Count - (int)(715f * heightdiff / ChangelogLabel.TextSize);

            for (int i = 0; i < lines.Count; i++)
                if (i >= setting.Max - setting.Value && i < setting.Max - setting.Value + 715f * heightdiff / ChangelogLabel.TextSize - 1)
                    result += $"{lines[i]}\n";

            ChangelogLabel.Text = result;
        }

        public void AssembleMapList()
        {
            var editor = MainWindow.Instance;

            mapOffset = MathHelper.Clamp(mapOffset, 0, editor.Maps.Count - mapSelects.Count);

            NavLeft.Visible = mapOffset > 0;
            NavRight.Visible = mapOffset < editor.Maps.Count - mapSelects.Count;

            for (int i = 0; i < mapSelects.Count; i++)
            {
                var button = mapSelects[i].Item1;
                button.Visible = i + mapOffset < editor.Maps.Count;

                if (button.Visible)
                {
                    var map = editor.Maps[i + mapOffset];
                    var fileName = (!map.IsSaved() ? "[!] " : "") + map.FileName;

                    button.Text = FontRenderer.TrimText(fileName, button.TextSize, (int)button.Rect.Width - 10, button.Font);
                    prevTexts[i] = button.Text;
                }
            }
        }

        public override void OnButtonClicked(int id)
        {
            var editor = MainWindow.Instance;

            switch (id)
            {
                case 0:
                    editor.SwitchWindow(new GuiWindowCreate());

                    break;

                case 1:
                    var dialog = new OpenFileDialog()
                    {
                        Title = "Select Map File",
                        Filter = "Text Documents (*.txt;*.sspm)|*.txt;*.sspm",
                    };

                    if (Settings.settings["defaultPath"] != "")
                        dialog.InitialDirectory = Settings.settings["defaultPath"];

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        Settings.settings["defaultPath"] = Path.GetDirectoryName(dialog.FileName) ?? "";

                        editor.LoadMap(dialog.FileName, true);
                    }

                    break;

                case 2:
                    try
                    {
                        var clipboard = Clipboard.GetText();

                        if (!string.IsNullOrWhiteSpace(clipboard))
                            editor.LoadMap(clipboard);
                    }
                    catch { MessageBox.Show("Failed to load map data - is it valid?", "Warning", "OK"); }

                    break;

                case 3:
                    editor.SwitchWindow(new GuiWindowSettings());

                    break;

                case 4:
                    var autosavedFile = Settings.settings["autosavedFile"];

                    if (autosavedFile != "")
                        editor.LoadMap(autosavedFile, false, true);

                    break;

                case 5:
                    var lastFile = Settings.settings["lastFile"];

                    if (lastFile != "" && File.Exists(lastFile))
                        editor.LoadMap(lastFile, true);

                    break;

                case 6:
                    if (mapOffset > 0)
                    {
                        mapOffset--;
                        AssembleMapList();
                    }

                    break;

                case 7:
                    if (mapOffset < editor.Maps.Count - mapSelects.Count)
                    {
                        mapOffset++;
                        AssembleMapList();
                    }

                    break;

                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                    int indexM = id % 80 + mapOffset;

                    if (indexM >= 0 && indexM < editor.Maps.Count)
                    {
                        editor.Maps[indexM].MakeCurrent();
                        editor.SwitchWindow(new GuiWindowEditor());
                    }

                    break;

                case 90:
                case 91:
                case 92:
                case 93:
                case 94:
                    int indexC = id % 90 + mapOffset;

                    if (indexC >= 0 && indexC < editor.Maps.Count)
                    {
                        editor.Maps[indexC].Close(false);
                        AssembleMapList();
                    }

                    break;
            }

            base.OnButtonClicked(id);
        }
    }
}
