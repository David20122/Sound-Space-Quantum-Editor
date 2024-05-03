using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New_SSQE.GUI
{
    internal class GuiWindowKeybinds : GuiWindow
    {
        private readonly GuiButton BackButton = new(655, 930, 600, 100, 0, "RETURN TO SETTINGS", 52, false, false, "square");

        private readonly GuiLabel HFlipLabel = new(1366, 520, 128, 26, "Horizontal Flip", 28, false, false, "main", false);
        private readonly GuiLabel HFlipCAS = new(1642, 550, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox HFlipBox = new(1366, 550, 128, 40, "", 28, false, false, false, "hFlip", "main", true);
        private readonly GuiButton HFlipReset = new(1504, 550, 128, 40, 1, "RESET", 28);

        private readonly GuiLabel VFlipLabel = new(1366, 600, 128, 26, "Vertical Flip", 28, false, false, "main", false);
        private readonly GuiLabel VFlipCAS = new(1642, 630, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox VFlipBox = new(1366, 630, 128, 40, "", 28, false, false, false, "vFlip", "main", true);
        private readonly GuiButton VFlipReset = new(1504, 630, 128, 40, 2, "RESET", 28);

        private readonly GuiLabel StoreNodesLabel = new(1366, 680, 128, 26, "Store Bezier Nodes", 28, false, false, "main", false);
        private readonly GuiLabel StoreNodesCAS = new(1642, 710, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox StoreNodesBox = new(1366, 710, 128, 40, "", 28, false, false, false, "storeNodes", "main", true);
        private readonly GuiButton StoreNodesReset = new(1504, 710, 128, 40, 7, "RESET", 28);

        private readonly GuiLabel AnchorNodeLabel = new(1366, 760, 128, 26, "Anchor Bezier Node", 28, false, false, "main", false);
        private readonly GuiLabel AnchorNodeCAS = new(1642, 790, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox AnchorNodeBox = new(1366, 790, 128, 40, "", 28, false, false, false, "anchorNode", "main", true);
        private readonly GuiButton AnchorNodeReset = new(1504, 790, 128, 40, 9, "RESET", 28);

        private readonly GuiLabel DrawBezierLabel = new(1366, 840, 128, 26, "Draw Bezier Curve", 28, false, false, "main", false);
        private readonly GuiLabel DrawBezierCAS = new(1642, 870, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox DrawBezierBox = new(1366, 870, 128, 40, "", 28, false, false, false, "drawBezier", "main", true);
        private readonly GuiButton DrawBezierReset = new(1504, 870, 128, 40, 8, "RESET", 28);


        private readonly GuiLabel SwitchClickLabel = new(150, 100, 128, 26, "Switch Click Function", 28, false, false, "main", false);
        private readonly GuiLabel SwitchClickCAS = new(426, 130, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox SwitchClickBox = new(150, 130, 128, 40, "", 28, false, false, false, "switchClickTool", "main", true);
        private readonly GuiButton SwitchClickReset = new(288, 130, 128, 40, 3, "RESET", 28);

        private readonly GuiLabel ToggleQuantumLabel = new(150, 180, 128, 26, "Toggle Quantum", 28, false, false, "main", false);
        private readonly GuiLabel ToggleQuantumCAS = new(426, 210, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox ToggleQuantumBox = new(150, 210, 128, 40, "", 28, false, false, false, "quantum", "main", true);
        private readonly GuiButton ToggleQuantumReset = new(288, 210, 128, 40, 4, "RESET", 28);

        
        private readonly GuiLabel OpenTimingsLabel = new(150, 300, 128, 26, "Open Timings", 28, false, false, "main", false);
        private readonly GuiLabel OpenTimingsCAS = new(426, 330, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox OpenTimingsBox = new(150, 330, 128, 40, "", 28, false, false, false, "openTimings", "main", true);
        private readonly GuiButton OpenTimingsReset = new(288, 330, 128, 40, 5, "RESET", 28);

        private readonly GuiLabel OpenBookmarksLabel = new(150, 380, 128, 26, "Open Bookmarks", 28, false, false, "main", false);
        private readonly GuiLabel OpenBookmarksCAS = new(426, 410, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox OpenBookmarksBox = new(150, 410, 128, 40, "", 28, false, false, false, "openBookmarks", "main", true);
        private readonly GuiButton OpenBookmarksReset = new(288, 410, 128, 40, 6, "RESET", 28);

        private readonly GuiLabel OpenDirectoryLabel = new(150, 460, 128, 26, "Open Directory", 28, false, false, "main", false);
        private readonly GuiLabel OpenDirectoryCAS = new(426, 490, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox OpenDirectoryBox = new(150, 490, 128, 40, "", 28, false, false, false, "openDirectory", "main", true);
        private readonly GuiButton OpenDirectoryReset = new(288, 490, 128, 40, 10, "RESET", 28);

        private readonly GuiLabel ExportSSPMLabel = new(150, 540, 128, 26, "Export SSPM", 28, false, false, "main", false);
        private readonly GuiLabel ExportSSPMCAS = new(426, 570, 256, 40, "", 28, false, false, "main", false);
        private readonly GuiTextbox ExportSSPMBox = new(150, 570, 128, 40, "", 28, false, false, false, "exportSSPM", "main", true);
        private readonly GuiButton ExportSSPMReset = new(288, 570, 128, 40, 11, "RESET", 28);


        private readonly GuiLabel GridLabel = new(1366, 49, 128, 26, "Grid", 28, false, false, "main", false);
        private readonly GuiTextbox GridTLBox = new(1366, 75, 128, 62, "", 28, false, false, false, "gridKey0", "main", true);
        private readonly GuiButton GridTLReset = new(1366, 141, 128, 62, 90, "RESET", 36);
        private readonly GuiTextbox GridTCBox = new(1504, 75, 128, 62, "", 28, false, false, false, "gridKey1", "main", true);
        private readonly GuiButton GridTCReset = new(1504, 141, 128, 62, 91, "RESET", 36);
        private readonly GuiTextbox GridTRBox = new(1642, 75, 128, 62, "", 28, false, false, false, "gridKey2", "main", true);
        private readonly GuiButton GridTRReset = new(1642, 141, 128, 62, 92, "RESET", 36);
        private readonly GuiTextbox GridMLBox = new(1366, 213, 128, 62, "", 28, false, false, false, "gridKey3", "main", true);
        private readonly GuiButton GridMLReset = new(1366, 279, 128, 62, 93, "RESET", 36);
        private readonly GuiTextbox GridMCBox = new(1504, 213, 128, 62, "", 28, false, false, false, "gridKey4", "main", true);
        private readonly GuiButton GridMCReset = new(1504, 279, 128, 62, 94, "RESET", 36);
        private readonly GuiTextbox GridMRBox = new(1642, 213, 128, 62, "", 28, false, false, false, "gridKey5", "main", true);
        private readonly GuiButton GridMRReset = new(1642, 279, 128, 62, 95, "RESET", 36);
        private readonly GuiTextbox GridBLBox = new(1366, 351, 128, 62, "", 28, false, false, false, "gridKey6", "main", true);
        private readonly GuiButton GridBLReset = new(1366, 417, 128, 62, 96, "RESET", 36);
        private readonly GuiTextbox GridBCBox = new(1504, 351, 128, 62, "", 28, false, false, false, "gridKey7", "main", true);
        private readonly GuiButton GridBCReset = new(1504, 417, 128, 62, 97, "RESET", 36);
        private readonly GuiTextbox GridBRBox = new(1642, 351, 128, 62, "", 28, false, false, false, "gridKey8", "main", true);
        private readonly GuiButton GridBRReset = new(1642, 417, 128, 62, 98, "RESET", 36);


        private readonly GuiCheckbox CtrlIndicator = new(64, 828, 64, 64, "", "CTRL Held", 36);
        private readonly GuiCheckbox AltIndicator = new(64, 912, 64, 64, "", "ALT Held", 36);
        private readonly GuiCheckbox ShiftIndicator = new(64, 996, 64, 64, "", "SHIFT Held", 36);

        private readonly GuiLabel StaticKeysLabel = new(480, 100, 960, 40, "", 28);

        public GuiWindowKeybinds() : base(0, 0, MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y)
        {
            Controls = new List<WindowControl>
            {
                // Buttons
                BackButton, HFlipReset, VFlipReset, SwitchClickReset, ToggleQuantumReset, OpenTimingsReset, OpenBookmarksReset, StoreNodesReset, DrawBezierReset, AnchorNodeReset,
                OpenDirectoryReset, ExportSSPMReset, GridTLReset, GridTCReset, GridTRReset, GridMLReset, GridMCReset, GridMRReset, GridBLReset, GridBCReset, GridBRReset,
                // Checkboxes
                CtrlIndicator, AltIndicator, ShiftIndicator,
                // Boxes
                HFlipBox, VFlipBox, SwitchClickBox, ToggleQuantumBox, OpenTimingsBox, OpenBookmarksBox, StoreNodesBox, DrawBezierBox, AnchorNodeBox, OpenDirectoryBox, ExportSSPMBox,
                GridTLBox, GridTCBox, GridTRBox, GridMLBox, GridMCBox, GridMRBox, GridBLBox, GridBCBox, GridBRBox,
                // Labels
                HFlipLabel, VFlipLabel, SwitchClickLabel, ToggleQuantumLabel, OpenTimingsLabel, OpenBookmarksLabel, StoreNodesLabel, DrawBezierLabel, AnchorNodeLabel, OpenDirectoryLabel,
                ExportSSPMLabel, HFlipCAS, VFlipCAS, SwitchClickCAS, ToggleQuantumCAS, OpenTimingsCAS, OpenBookmarksCAS, StoreNodesCAS, DrawBezierCAS, AnchorNodeCAS, OpenDirectoryCAS,
                ExportSSPMCAS, GridLabel, StaticKeysLabel
            };

            BackgroundSquare = new(0, 0, 1920, 1080, Color.FromArgb(255, 30, 30, 30), false, "background_menu.png", "menubg");
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
            var editor = MainWindow.Instance;

            CtrlIndicator.Toggle = editor.CtrlHeld;
            AltIndicator.Toggle = editor.AltHeld;
            ShiftIndicator.Toggle = editor.ShiftHeld;

            HFlipCAS.Text = CAS(Settings.settings["hFlip"]);
            VFlipCAS.Text = CAS(Settings.settings["vFlip"]);
            SwitchClickCAS.Text = CAS(Settings.settings["switchClickTool"]);
            ToggleQuantumCAS.Text = CAS(Settings.settings["quantum"]);
            OpenTimingsCAS.Text = CAS(Settings.settings["openTimings"]);
            OpenBookmarksCAS.Text = CAS(Settings.settings["openBookmarks"]);
            StoreNodesCAS.Text = CAS(Settings.settings["storeNodes"]);
            DrawBezierCAS.Text = CAS(Settings.settings["drawBezier"]);
            AnchorNodeCAS.Text = CAS(Settings.settings["anchorNode"]);
            OpenDirectoryCAS.Text = CAS(Settings.settings["openDirectory"]);
            ExportSSPMCAS.Text = CAS(Settings.settings["exportSSPM"]);

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
                case 0:
                    MainWindow.Instance.SwitchWindow(new GuiWindowSettings());

                    break;

                case 1:
                    Settings.settings["hFlip"] = new Keybind(Keys.H, false, false, true);
                    HFlipBox.Text = "H";
                    break;
                case 2:
                    Settings.settings["vFlip"] = new Keybind(Keys.V, false, false, true);
                    VFlipBox.Text = "V";
                    break;
                case 3:
                    Settings.settings["switchClickTool"] = new Keybind(Keys.Tab, false, false, false);
                    SwitchClickBox.Text = "TAB";
                    break;
                case 4:
                    Settings.settings["quantum"] = new Keybind(Keys.Q, true, false, false);
                    ToggleQuantumBox.Text = "Q";
                    break;
                case 5:
                    Settings.settings["openTimings"] = new Keybind(Keys.T, true, false, false);
                    OpenTimingsBox.Text = "T";
                    break;
                case 6:
                    Settings.settings["openBookmarks"] = new Keybind(Keys.B, true, false, false);
                    OpenBookmarksBox.Text = "B";
                    break;
                case 7:
                    Settings.settings["storeNodes"] = new Keybind(Keys.S, false, false, true);
                    StoreNodesBox.Text = "S";
                    break;
                case 8:
                    Settings.settings["drawBezier"] = new Keybind(Keys.D, false, false, true);
                    DrawBezierBox.Text = "D";
                    break;
                case 9:
                    Settings.settings["anchorNode"] = new Keybind(Keys.A, false, false, true);
                    AnchorNodeBox.Text = "A";
                    break;
                case 10:
                    Settings.settings["openDirectory"] = new Keybind(Keys.D, true, false, true);
                    OpenDirectoryBox.Text = "D";
                    break;
                case 11:
                    Settings.settings["exportSSPM"] = new Keybind(Keys.E, true, true, false);
                    ExportSSPMBox.Text = "E";
                    break;

                case 90:
                    Settings.settings["gridKeys"][0] = Keys.Q;
                    GridTLBox.Text = "Q";
                    break;
                case 91:
                    Settings.settings["gridKeys"][1] = Keys.W;
                    GridTCBox.Text = "W";
                    break;
                case 92:
                    Settings.settings["gridKeys"][2] = Keys.E;
                    GridTRBox.Text = "E";
                    break;
                case 93:
                    Settings.settings["gridKeys"][3] = Keys.A;
                    GridMLBox.Text = "A";
                    break;
                case 94:
                    Settings.settings["gridKeys"][4] = Keys.S;
                    GridMCBox.Text = "S";
                    break;
                case 95:
                    Settings.settings["gridKeys"][5] = Keys.D;
                    GridMRBox.Text = "D";
                    break;
                case 96:
                    Settings.settings["gridKeys"][6] = Keys.Z;
                    GridBLBox.Text = "Z";
                    break;
                case 97:
                    Settings.settings["gridKeys"][7] = Keys.X;
                    GridBCBox.Text = "X";
                    break;
                case 98:
                    Settings.settings["gridKeys"][8] = Keys.C;
                    GridBRBox.Text = "C";
                    break;
            }

            base.OnButtonClicked(id);
        }

        private static string CAS(Keybind key)
        {
            var cas = new List<string>();

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
