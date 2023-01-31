using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;
using System.Net;

namespace Sound_Space_Editor.GUI
{
	class GuiWindowMenu : Gui
	{
        private readonly GuiLabel clLabel = new GuiLabel(60, 195, 200, 40, "CHANGELOG", 40, false, false, "square", false);
        private readonly GuiLabel ssLabel = new GuiLabel(35, 35, 750, 100, "SOUND SPACE", 150, false, false, "square", false);
        private readonly GuiLabel qeLabel = new GuiLabel(615, 140, 150, 40, "QUANTUM EDITOR", 36, false, false, "square", false);
        private readonly GuiLabel ChangelogLabel = new GuiLabel(60, 230, 890, 715, "", 18, false, false, "main", false);

        private readonly GuiButton CreateButton = new GuiButton(1190, 180, 600, 100, 0, "CREATE NEW MAP", 52, false, false, "square");
        private readonly GuiButton LoadButton = new GuiButton(1190, 295, 600, 100, 1, "LOAD MAP", 52, false, false, "square");
        private readonly GuiButton ImportButton = new GuiButton(1190, 410, 600, 100, 2, "IMPORT MAP", 52, false, false, "square");
        private readonly GuiButton SettingsButton = new GuiButton(1190, 525, 600, 100, 3, "SETTINGS", 52, false, false, "square");

        private readonly GuiButton AutosavedButton = new GuiButton(1190, 640, 600, 100, 4, "AUTOSAVED MAP", 52, false, false, "square");
        private readonly GuiButton LastMapButton = new GuiButton(1190, 755, 600, 100, 5, "EDIT LAST MAP", 52, false, false, "square");

        private readonly GuiSlider ChangelogSlider = new GuiSlider(950, 230, 20, 720, "changelogPosition", true);

        private int txid;
        private bool bgImg;

        private int lastAssembled = 0;
        private string changelogText;

        public GuiWindowMenu() : base(0, 0, MainWindow.Instance.ClientSize.Width, MainWindow.Instance.ClientSize.Height)
        {
            buttons = new List<GuiButton> { CreateButton, LoadButton, ImportButton, SettingsButton, AutosavedButton, LastMapButton };
            sliders = new List<GuiSlider> { ChangelogSlider };
            labels = new List<GuiLabel> { clLabel, ssLabel, qeLabel, ChangelogLabel };

            try
            {
                var wc = new WebClient();
                changelogText = wc.DownloadString("https://raw.githubusercontent.com/Avibah/Sound-Space-Quantum-Editor/master/changelog");
            }
            catch { changelogText = "Failed to load changelog"; }

            OnResize(MainWindow.Instance.ClientSize);

            if (File.Exists("background_menu.png"))
            {
                bgImg = true;

                using (Bitmap img = new Bitmap("background_menu.png"))
                    txid = TextureManager.GetOrRegister("menubg", img, true);
            }
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

            var widthdiff = rect.Width / 1920f;
            var heightdiff = rect.Height / 1080f;

            GL.Color4(bgImg ? Color.FromArgb(255, 255, 255, 255) : Color.FromArgb(255, 30, 30, 30));

            if (bgImg)
                GLSpecial.TexturedQuad(rect, 0, 0, 1, 1, txid);
            else
                GLSpecial.Rect(rect);

            GL.Color4(bgImg ? Color.FromArgb(120, 57, 56, 47) : Color.FromArgb(40, 0, 0, 0));
            GLSpecial.Rect(35 * widthdiff, 180 * heightdiff, 950 * widthdiff, 790 * heightdiff);
            GL.Color4(bgImg ? Color.FromArgb(100, 36, 35, 33) : Color.FromArgb(50, 0, 0, 0));
            GLSpecial.Rect(55 * widthdiff, 230 * heightdiff, 900 * widthdiff, 715 * heightdiff);

            base.Render(mousex, mousey, frametime);
        }

        public override void OnResize(Size size)
        {
            rect = new RectangleF(0, 0, size.Width, size.Height);

            base.OnResize(size);

            LastMapButton.rect.Y = Settings.settings["autosavedFile"] == "" ? AutosavedButton.rect.Y : LastMapButton.rect.Y;

            AssembleChangelog();
            Settings.settings["changelogPosition"].Value = Settings.settings["changelogPosition"].Max;
        }

        public void AssembleChangelog()
        {
            var widthdiff = rect.Width / 1920f;
            var heightdiff = rect.Height / 1080f;

            var result = "";
            var lines = new List<string>();

            foreach (var line in changelogText.Split('\n'))
            {
                var lineedit = line;

                while (TextWidth(lineedit, ChangelogLabel.textSize) > 890 * widthdiff && lineedit.Contains(" "))
                {
                    var index = lineedit.LastIndexOf(' ');

                    if (TextWidth(lineedit.Substring(0, index), ChangelogLabel.textSize) <= 890 * widthdiff)
                        lineedit = lineedit.Remove(index, 1).Insert(index, "\n");
                    else
                        lineedit = lineedit.Remove(index, 1).Insert(index, "\\");
                }

                lineedit = lineedit.Replace("\\", " ");

                foreach (var newline in lineedit.Split('\n'))
                    lines.Add(newline);
            }

            var setting = Settings.settings["changelogPosition"];

            setting.Max = lines.Count - (int)(715f * heightdiff / ChangelogLabel.textSize);

            for (int i = 0; i < lines.Count; i++)
                if (i >= setting.Max - setting.Value && i < setting.Max - setting.Value + 715f * heightdiff / ChangelogLabel.textSize - 1)
                    result += $"{lines[i]}\n";

            ChangelogLabel.text = result;
        }

        protected override void OnButtonClicked(int id)
        {
            switch (id)
            {
                case 0:
                    MainWindow.Instance.SwitchWindow(new GuiWindowCreate());

                    break;

                case 1:
                    using (var dialog = new OpenFileDialog
                    {
                        Title = "Select Map File",
                        Filter = "Text Documents (*.txt)|*.txt",
                    })
                    {
                        if (Settings.settings["defaultPath"] != "")
                            dialog.InitialDirectory = Settings.settings["defaultPath"];

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            Settings.settings["defaultPath"] = Path.GetDirectoryName(dialog.FileName);

                            MainWindow.Instance.LoadMap(dialog.FileName, true);
                        }
                    }

                    break;

                case 2:
                    try
                    {
                        var clipboard = Clipboard.GetText();

                        if (!string.IsNullOrWhiteSpace(clipboard))
                        {
                            var wc = new WebClient();

                            if (clipboard.Contains("github"))
                                clipboard = wc.DownloadString(clipboard);

                            MainWindow.Instance.LoadMap(clipboard);
                        }
                    }
                    catch { MainWindow.Instance.ShowMessageBox("Failed to load map data - is it valid?"); }

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
        }
    }
}
