using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiWindowCreate : GuiWindow
    {
        private readonly GuiLabel Label = new(832, 478, 256, 20, "Input Audio ID", 30);
        private readonly GuiTextbox IDBox = new(832, 508, 256, 64, "", 30, false);

        private readonly GuiButton CreateButton = new(832, 592, 256, 64, 0, "CREATE", 38);
        private readonly GuiButton ImportButton = new(832, 666, 256, 64, 1, "IMPORT FILE", 38);
        private readonly GuiButton BackButton = new(832, 740, 256, 64, 2, "BACK", 38);

        public GuiWindowCreate() : base(0, 0, MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y)
        {
            Controls = new List<WindowControl>
            {
                // Buttons
                CreateButton, ImportButton, BackButton,
                // Boxes
                IDBox,
                // Labels
                Label
            };

            BackgroundSquare = new(0, 0, 1920, 1080, Color.FromArgb(255, 30, 30, 30), false, "background_menu.png", "menubg");
            Init();

            OnResize(MainWindow.Instance.ClientSize);
        }

        public override void OnResize(Vector2i size)
        {
            Rect = new RectangleF(0, 0, size.X, size.Y);

            base.OnResize(size);
        }

        public override void OnButtonClicked(int id)
        {
            var audioId = IDBox.Text.Trim();
            var editor = MainWindow.Instance;

            switch (id)
            {
                case 0:
                    if (!string.IsNullOrWhiteSpace(audioId))
                        editor.LoadMap(audioId);

                    break;

                case 1:
                    editor.CurrentMap?.Save();
                    editor.CurrentMap = null;

                    if (editor.PromptImport(audioId, true))
                        editor.LoadMap(MainWindow.Instance.SoundID);

                    break;

                case 2:
                    editor.SwitchWindow(new GuiWindowMenu());

                    break;
            }

            base.OnButtonClicked(id);
        }
    }
}
