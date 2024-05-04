using OpenTK.Mathematics;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiWindowLanguage : GuiWindow
    {
        private readonly GuiButton BackButton = new(655, 930, 600, 100, 99, "RETURN TO SETTINGS", 52, false, false, "square");

        private readonly GuiLabel SelectLabel = new(860, 350, 200, 40, "Select Language", 40);

        private readonly List<string> languages = new()
        {
            "english",
            "日本語"
        };

        private readonly List<GuiCheckbox> languageCheckboxes = new();

        public GuiWindowLanguage() : base(0, 0, MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y)
        {
            Controls = new List<WindowControl>
            {
                // Buttons
                BackButton,
                // Labels
                SelectLabel
            };

            for (int i = 0; i < languages.Count; i++)
            {
                GuiCheckbox checkbox = new(770, 450 + i * 50 + 5, 30, 30, "", "", 0);
                GuiButton button = new(810, 450 + i * 50, 300, 40, i, languages[i].ToUpper(), 30);

                languageCheckboxes.Add(checkbox);

                Controls.Add(checkbox);
                Controls.Add(button);
            }

            BackgroundSquare = new(0, 0, 1920, 1080, Color.FromArgb(255, 30, 30, 30), false, "background_menu.png", "menubg");
            Init();

            OnResize(MainWindow.Instance.ClientSize);
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            for (int i = 0; i < languageCheckboxes.Count; i++)
                languageCheckboxes[i].Toggle = Settings.settings["language"] == languages[i];

            base.Render(mousex, mousey, frametime);
        }

        public override void OnResize(Vector2i size)
        {
            Rect = new RectangleF(0, 0, size.X, size.Y);

            base.OnResize(size);
        }

        public override void OnButtonClicked(int id)
        {
            switch (id)
            {
                case 99:
                    MainWindow.Instance.SwitchWindow(new GuiWindowSettings());

                    break;

                default:
                    if (id >= 0 && id < languages.Count)
                        Settings.settings["language"] = languages[id];

                    break;
            }

            base.OnButtonClicked(id);
        }
    }
}
