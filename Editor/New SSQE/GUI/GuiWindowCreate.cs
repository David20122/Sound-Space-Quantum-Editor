using New_SSQE.FileParsing;
using New_SSQE.Maps;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiWindowCreate : GuiWindow
    {
        private readonly GuiLabel Label = new(832, 478, 256, 20, "Input Audio ID", 30);
        private readonly GuiTextbox IDBox = new(832, 508, 256, 64, 30);

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

            BackgroundSquare = new(Color.FromArgb(255, 30, 30, 30), "background_menu.png", "menubg");
            Init();

            OnResize(MainWindow.Instance.ClientSize);
        }

        public override void OnButtonClicked(int id)
        {
            string audioId = IDBox.Text.Trim();
            audioId = Exporting.FixID(audioId);
            MainWindow editor = MainWindow.Instance;

            switch (id)
            {
                case 0:
                    if (!string.IsNullOrWhiteSpace(audioId))
                        MapManager.Load(audioId);

                    break;

                case 1:
                    CurrentMap.LoadedMap?.Save();
                    CurrentMap.LoadedMap = null;

                    if (MapManager.ImportAudio(audioId, true))
                        MapManager.Load(CurrentMap.SoundID);

                    break;

                case 2:
                    editor.SwitchWindow(new GuiWindowMenu());

                    break;
            }

            base.OnButtonClicked(id);
        }
    }
}
