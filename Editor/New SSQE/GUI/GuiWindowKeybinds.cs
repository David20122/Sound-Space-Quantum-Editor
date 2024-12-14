using System.Drawing;
using New_SSQE.Preferences;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New_SSQE.GUI
{
    internal class GuiWindowKeybinds : GuiWindow
    {
        private readonly GuiButton BackButton = new(655, 930, 600, 100, 0, "RETURN TO SETTINGS", 52, "square");

        private readonly GuiLabel HFlipLabel = new(1366, 100, 128, 26, "Horizontal Flip", 28, "main", false);
        private readonly GuiLabel HFlipCAS = new(1642, 130, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox HFlipBox = new(1366, 130, 128, 40, 28, Settings.hFlip);
        private readonly GuiButton HFlipReset = new(1504, 130, 128, 40, 1, "RESET", 28);

        private readonly GuiLabel VFlipLabel = new(1366, 180, 128, 26, "Vertical Flip", 28, "main", false);
        private readonly GuiLabel VFlipCAS = new(1642, 210, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox VFlipBox = new(1366, 210, 128, 40, 28, Settings.vFlip);
        private readonly GuiButton VFlipReset = new(1504, 210, 128, 40, 2, "RESET", 28);

        private readonly GuiLabel StoreNodesLabel = new(1366, 260, 128, 26, "Store Bezier Nodes", 28, "main", false);
        private readonly GuiLabel StoreNodesCAS = new(1642, 290, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox StoreNodesBox = new(1366, 290, 128, 40, 28, Settings.storeNodes);
        private readonly GuiButton StoreNodesReset = new(1504, 290, 128, 40, 7, "RESET", 28);

        private readonly GuiLabel AnchorNodeLabel = new(1366, 340, 128, 26, "Anchor Bezier Node", 28, "main", false);
        private readonly GuiLabel AnchorNodeCAS = new(1642, 370, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox AnchorNodeBox = new(1366, 370, 128, 40, 28, Settings.anchorNode);
        private readonly GuiButton AnchorNodeReset = new(1504, 370, 128, 40, 9, "RESET", 28);

        private readonly GuiLabel DrawBezierLabel = new(1366, 420, 128, 26, "Draw Bezier Curve", 28, "main", false);
        private readonly GuiLabel DrawBezierCAS = new(1642, 450, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox DrawBezierBox = new(1366, 450, 128, 40, 28, Settings.drawBezier);
        private readonly GuiButton DrawBezierReset = new(1504, 450, 128, 40, 8, "RESET", 28);


        private readonly GuiLabel SwitchClickLabel = new(150, 100, 128, 26, "Switch Click Function", 28, "main", false);
        private readonly GuiLabel SwitchClickCAS = new(426, 130, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox SwitchClickBox = new(150, 130, 128, 40, 28, Settings.switchClickTool);
        private readonly GuiButton SwitchClickReset = new(288, 130, 128, 40, 3, "RESET", 28);

        private readonly GuiLabel ToggleQuantumLabel = new(150, 180, 128, 26, "Toggle Quantum", 28, "main", false);
        private readonly GuiLabel ToggleQuantumCAS = new(426, 210, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox ToggleQuantumBox = new(150, 210, 128, 40, 28, Settings.quantum);
        private readonly GuiButton ToggleQuantumReset = new(288, 210, 128, 40, 4, "RESET", 28);

        
        private readonly GuiLabel OpenTimingsLabel = new(150, 300, 128, 26, "Open Timings", 28, "main", false);
        private readonly GuiLabel OpenTimingsCAS = new(426, 330, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox OpenTimingsBox = new(150, 330, 128, 40, 28, Settings.openTimings);
        private readonly GuiButton OpenTimingsReset = new(288, 330, 128, 40, 5, "RESET", 28);

        private readonly GuiLabel OpenBookmarksLabel = new(150, 380, 128, 26, "Open Bookmarks", 28, "main", false);
        private readonly GuiLabel OpenBookmarksCAS = new(426, 410, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox OpenBookmarksBox = new(150, 410, 128, 40, 28, Settings.openBookmarks);
        private readonly GuiButton OpenBookmarksReset = new(288, 410, 128, 40, 6, "RESET", 28);

        private readonly GuiLabel OpenDirectoryLabel = new(150, 460, 128, 26, "Open Directory", 28, "main", false);
        private readonly GuiLabel OpenDirectoryCAS = new(426, 490, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox OpenDirectoryBox = new(150, 490, 128, 40, 28, Settings.openDirectory);
        private readonly GuiButton OpenDirectoryReset = new(288, 490, 128, 40, 10, "RESET", 28);

        private readonly GuiLabel ExportSSPMLabel = new(150, 540, 128, 26, "Export SSPM", 28, "main", false);
        private readonly GuiLabel ExportSSPMCAS = new(426, 570, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox ExportSSPMBox = new(150, 570, 128, 40, 28, Settings.exportSSPM);
        private readonly GuiButton ExportSSPMReset = new(288, 570, 128, 40, 11, "RESET", 28);

        private readonly GuiLabel CreateBPMLabel = new(150, 620, 128, 26, "Create Timing Point", 28, "main", false);
        private readonly GuiLabel CreateBPMCAS = new(426, 650, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox CreateBPMBox = new(150, 650, 128, 40, 28, Settings.createBPM);
        private readonly GuiButton CreateBPMReset = new(288, 650, 128, 40, 12, "RESET", 28);


        private readonly GuiLabel GridLabel = new(778, 49, 128, 26, "Grid", 28, "main", false);
        private readonly GuiTextbox GridTLBox = new(778, 75, 128, 40, 28, 0);
        private readonly GuiButton GridTLReset = new(778, 119, 128, 40, 90, "RESET", 36);
        private readonly GuiTextbox GridTCBox = new(916, 75, 128, 40, 28, 1);
        private readonly GuiButton GridTCReset = new(916, 119, 128, 40, 91, "RESET", 36);
        private readonly GuiTextbox GridTRBox = new(1054, 75, 128, 40, 28, 2);
        private readonly GuiButton GridTRReset = new(1054, 119, 128, 40, 92, "RESET", 36);
        private readonly GuiTextbox GridMLBox = new(778, 169, 128, 40, 28, 3);
        private readonly GuiButton GridMLReset = new(778, 213, 128, 40, 93, "RESET", 36);
        private readonly GuiTextbox GridMCBox = new(916, 169, 128, 40, 28, 4);
        private readonly GuiButton GridMCReset = new(916, 213, 128, 40, 94, "RESET", 36);
        private readonly GuiTextbox GridMRBox = new(1054, 169, 128, 40, 28, 5);
        private readonly GuiButton GridMRReset = new(1054, 213, 128, 40, 95, "RESET", 36);
        private readonly GuiTextbox GridBLBox = new(778, 263, 128, 40, 28, 6);
        private readonly GuiButton GridBLReset = new(778, 307, 128, 40, 96, "RESET", 36);
        private readonly GuiTextbox GridBCBox = new(916, 263, 128, 40, 28, 7);
        private readonly GuiButton GridBCReset = new(916, 307, 128, 40, 97, "RESET", 36);
        private readonly GuiTextbox GridBRBox = new(1054, 263, 128, 40, 28, 8);
        private readonly GuiButton GridBRReset = new(1054, 307, 128, 40, 98, "RESET", 36);


        private readonly GuiLabel ExtrasLabel = new(1366, 540, 128, 26, "Extra Objects:", 28, "main", false);
        private readonly GuiLabel BeatLabel = new(1366, 580, 128, 26, "Beat", 28, "main", false);
        private readonly GuiLabel BeatCAS = new(1642, 610, 256, 40, "", 28, "main", false);
        private readonly GuiTextbox BeatBox = new(1366, 610, 128, 40, 28, Settings.extrasBeat);
        private readonly GuiButton BeatReset = new(1504, 610, 128, 40, 100, "RESET", 28);


        private readonly GuiCheckbox CtrlIndicator = new(64, 828, 64, 64, "CTRL Held", 36);
        private readonly GuiCheckbox AltIndicator = new(64, 912, 64, 64, "ALT Held", 36);
        private readonly GuiCheckbox ShiftIndicator = new(64, 996, 64, 64, "SHIFT Held", 36);

        private readonly GuiLabel StaticKeysLabel = new(480, 375, 960, 40, 20);

        public GuiWindowKeybinds() : base(0, 0, MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y)
        {
            Controls = new List<WindowControl>
            {
                // Buttons
                BackButton, HFlipReset, VFlipReset, SwitchClickReset, ToggleQuantumReset, OpenTimingsReset, OpenBookmarksReset, StoreNodesReset, DrawBezierReset, AnchorNodeReset,
                OpenDirectoryReset, ExportSSPMReset, CreateBPMReset, GridTLReset, GridTCReset, GridTRReset, GridMLReset, GridMCReset, GridMRReset, GridBLReset, GridBCReset, GridBRReset,
                /*BeatReset,*/
                // Checkboxes
                CtrlIndicator, AltIndicator, ShiftIndicator,
                // Boxes
                HFlipBox, VFlipBox, SwitchClickBox, ToggleQuantumBox, OpenTimingsBox, OpenBookmarksBox, StoreNodesBox, DrawBezierBox, AnchorNodeBox, OpenDirectoryBox, ExportSSPMBox,
                CreateBPMBox, GridTLBox, GridTCBox, GridTRBox, GridMLBox, GridMCBox, GridMRBox, GridBLBox, GridBCBox, GridBRBox, /*BeatBox,*/
                // Labels
                HFlipLabel, VFlipLabel, SwitchClickLabel, ToggleQuantumLabel, OpenTimingsLabel, OpenBookmarksLabel, StoreNodesLabel, DrawBezierLabel, AnchorNodeLabel, OpenDirectoryLabel,
                ExportSSPMLabel, CreateBPMLabel, HFlipCAS, VFlipCAS, SwitchClickCAS, ToggleQuantumCAS, OpenTimingsCAS, OpenBookmarksCAS, StoreNodesCAS, DrawBezierCAS, AnchorNodeCAS,
                OpenDirectoryCAS, ExportSSPMCAS, CreateBPMCAS, GridLabel, StaticKeysLabel, /*ExtrasLabel, BeatLabel, BeatCAS*/
            };

            BackgroundSquare = new(Color.FromArgb(255, 30, 30, 30), "background_menu.png", "menubg");
            Init();

            string[] staticList =
            {
                "Static keybinds:",
                "",
                "> Zoom: CTRL + SCROLL",
                "",
                "> Beat Divisor: SHIFT + SCROLL",
                ">> Hold CTRL to increment by 0.5",
                "",
                "> Seek: SCROLL/LEFT/RIGHT",
                "> Play/Pause: SPACE",
                "",
                "> Select all: CTRL + A",
                "> Deselect all: ESCAPE",
                "",
                "> Delete: DELETE/BACKSPACE",
                "> Copy: CTRL + C",
                "> Paste: CTRL + V",
                "> Cut: CTRL + X",
                "> Undo: CTRL + Z",
                "> Redo: CTRL + Y",
                "",
                "> Fullscreen: F11",
                "> Save: CTRL + S",
                "> Save as: CTRL + SHIFT + S",
                "",
                "> Place stored patterns: 0-9",
                ">> Hold SHIFT to store selected notes as the key's pattern",
                ">> Hold CTRL to clear the key's pattern",
            };
            StaticKeysLabel.Text = string.Join("\n", staticList);

            OnResize(MainWindow.Instance.ClientSize);
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            MainWindow editor = MainWindow.Instance;

            CtrlIndicator.Toggle = editor.CtrlHeld;
            AltIndicator.Toggle = editor.AltHeld;
            ShiftIndicator.Toggle = editor.ShiftHeld;

            HFlipCAS.Text = CAS(Settings.hFlip.Value);
            VFlipCAS.Text = CAS(Settings.vFlip.Value);
            SwitchClickCAS.Text = CAS(Settings.switchClickTool.Value);
            ToggleQuantumCAS.Text = CAS(Settings.quantum.Value);
            OpenTimingsCAS.Text = CAS(Settings.openTimings.Value);
            OpenBookmarksCAS.Text = CAS(Settings.openBookmarks.Value);
            StoreNodesCAS.Text = CAS(Settings.storeNodes.Value);
            DrawBezierCAS.Text = CAS(Settings.drawBezier.Value);
            AnchorNodeCAS.Text = CAS(Settings.anchorNode.Value);
            OpenDirectoryCAS.Text = CAS(Settings.openDirectory.Value);
            ExportSSPMCAS.Text = CAS(Settings.exportSSPM.Value);
            CreateBPMCAS.Text = CAS(Settings.createBPM.Value);
            BeatCAS.Text = CAS(Settings.extrasBeat.Value);

            base.Render(mousex, mousey, frametime);
        }

        public override void OnButtonClicked(int id)
        {
            switch (id)
            {
                case 0:
                    MainWindow.Instance.SwitchWindow(new GuiWindowSettings());

                    break;

                case 1:
                    Settings.hFlip.Value = new Keybind(Keys.H, false, false, true);
                    HFlipBox.Text = "H";
                    break;
                case 2:
                    Settings.vFlip.Value = new Keybind(Keys.V, false, false, true);
                    VFlipBox.Text = "V";
                    break;
                case 3:
                    Settings.switchClickTool.Value = new Keybind(Keys.Tab, false, false, false);
                    SwitchClickBox.Text = "TAB";
                    break;
                case 4:
                    Settings.quantum.Value = new Keybind(Keys.Q, true, false, false);
                    ToggleQuantumBox.Text = "Q";
                    break;
                case 5:
                    Settings.openTimings.Value = new Keybind(Keys.T, true, false, false);
                    OpenTimingsBox.Text = "T";
                    break;
                case 6:
                    Settings.openBookmarks.Value = new Keybind(Keys.B, true, false, false);
                    OpenBookmarksBox.Text = "B";
                    break;
                case 7:
                    Settings.storeNodes.Value = new Keybind(Keys.S, false, false, true);
                    StoreNodesBox.Text = "S";
                    break;
                case 8:
                    Settings.drawBezier.Value = new Keybind(Keys.D, false, false, true);
                    DrawBezierBox.Text = "D";
                    break;
                case 9:
                    Settings.anchorNode.Value = new Keybind(Keys.A, false, false, true);
                    AnchorNodeBox.Text = "A";
                    break;
                case 10:
                    Settings.openDirectory.Value = new Keybind(Keys.D, true, false, true);
                    OpenDirectoryBox.Text = "D";
                    break;
                case 11:
                    Settings.exportSSPM.Value = new Keybind(Keys.E, true, true, false);
                    ExportSSPMBox.Text = "E";
                    break;
                case 12:
                    Settings.createBPM.Value = new Keybind(Keys.B, true, false, true);
                    CreateBPMBox.Text = "B";
                    break;

                case 90:
                    Settings.gridKeys.Value[0] = Keys.Q;
                    GridTLBox.Text = "Q";
                    break;
                case 91:
                    Settings.gridKeys.Value[1] = Keys.W;
                    GridTCBox.Text = "W";
                    break;
                case 92:
                    Settings.gridKeys.Value[2] = Keys.E;
                    GridTRBox.Text = "E";
                    break;
                case 93:
                    Settings.gridKeys.Value[3] = Keys.A;
                    GridMLBox.Text = "A";
                    break;
                case 94:
                    Settings.gridKeys.Value[4] = Keys.S;
                    GridMCBox.Text = "S";
                    break;
                case 95:
                    Settings.gridKeys.Value[5] = Keys.D;
                    GridMRBox.Text = "D";
                    break;
                case 96:
                    Settings.gridKeys.Value[6] = Keys.Z;
                    GridBLBox.Text = "Z";
                    break;
                case 97:
                    Settings.gridKeys.Value[7] = Keys.X;
                    GridBCBox.Text = "X";
                    break;
                case 98:
                    Settings.gridKeys.Value[8] = Keys.C;
                    GridBRBox.Text = "C";
                    break;

                case 100:
                    Settings.extrasBeat.Value = new Keybind(Keys.S, false, false, false);
                    BeatBox.Text = "S";
                    break;
            }

            base.OnButtonClicked(id);
        }

        private static string CAS(Keybind key)
        {
            List<string> cas = new();

            if (key.Ctrl)
                cas.Add("CTRL");
            if (key.Alt)
                cas.Add("ALT");
            if (key.Shift)
                cas.Add("SHIFT");

            return string.Join(" + ", cas);
        }
    }
}
