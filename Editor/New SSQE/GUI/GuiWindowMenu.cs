using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
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

        private int lastAssembled = 0;
        private readonly string changelogText;

        public GuiWindowMenu() : base(0, 0, MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y)
        {
            Controls = new List<WindowControl>
            {
                // Squares
                ChangelogBackdrop1, ChangelogBackdrop2,
                // Buttons
                CreateButton, LoadButton, ImportButton, SettingsButton, AutosavedButton, LastMapButton,
                // Sliders
                ChangelogSlider,
                // Labels
                clLabel, ssLabel, qeLabel, ChangelogLabel
            };
            
            BackgroundSquare = new(0, 0, 1920, 1080, Color.FromArgb(255, 30, 30, 30), false, "background_menu.png", "menubg");
            Init();

            if (File.Exists("background_menu.png"))
            {
                ChangelogBackdrop1.Color = Color.FromArgb(120, 57, 56, 47);
                ChangelogBackdrop2.Color = Color.FromArgb(100, 36, 35, 33);
            }

            try
            {
                changelogText = WebClient.DownloadString("https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/changelog");
            }
            catch { changelogText = "Failed to load changelog"; }

            OnResize(MainWindow.Instance.ClientSize);
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

        public void AssembleChangelog()
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

        public override void OnButtonClicked(int id)
        {
            switch (id)
            {
                case 0:
                    MainWindow.Instance.SwitchWindow(new GuiWindowCreate());

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

                        MainWindow.Instance.LoadMap(dialog.FileName, true);
                    }

                    break;

                case 2:
                    try
                    {
                        var clipboard = Clipboard.GetText();

                        if (!string.IsNullOrWhiteSpace(clipboard))
                        {
                            if (clipboard.Contains("github"))
                                clipboard = WebClient.DownloadString(clipboard);

                            MainWindow.Instance.LoadMap(clipboard);
                        }
                    }
                    catch { MessageBox.Show("Failed to load map data - is it valid?", "Warning", "OK"); }

                    break;

                case 3:
                    MainWindow.Instance.SwitchWindow(new GuiWindowSettings());

                    break;

                case 4:
                    var autosavedFile = Settings.settings["autosavedFile"];

                    if (autosavedFile != "")
                        MainWindow.Instance.LoadMap(autosavedFile, false, true);

                    break;

                case 5:
                    var lastFile = Settings.settings["lastFile"];

                    if (lastFile != "" && File.Exists(lastFile))
                        MainWindow.Instance.LoadMap(lastFile, true);

                    break;
            }

            base.OnButtonClicked(id);
        }
    }
}
