using System.Collections.Generic;
using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New_SSQE.GUI
{
    internal class GuiWindowKeybinds : GuiWindow
    {
        private readonly GuiButton BackButton = new(655, 930, 600, 100, 0, "RETURN TO SETTINGS", 48, false, false, "square");

        private readonly GuiLabel HFlipLabel = new(150, 49, 128, 26, "Horizontal Flip", 24, false, false, "main", false);
        private readonly GuiLabel HFlipCAS = new(426, 83, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox HFlipBox = new(150, 75, 128, 40, "", 24, false, false, false, "hFlip", "main", true);
        private readonly GuiButton HFlipReset = new(288, 75, 128, 40, 1, "RESET", 24);

        private readonly GuiLabel VFlipLabel = new(150, 119, 128, 26, "Vertical Flip", 24, false, false, "main", false);
        private readonly GuiLabel VFlipCAS = new(426, 153, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox VFlipBox = new(150, 145, 128, 40, "", 24, false, false, false, "vFlip", "main", true);
        private readonly GuiButton VFlipReset = new(288, 145, 128, 40, 2, "RESET", 24);

        private readonly GuiLabel SwitchClickLabel = new(150, 189, 128, 26, "Switch Click Function", 24, false, false, "main", false);
        private readonly GuiLabel SwitchClickCAS = new(426, 223, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox SwitchClickBox = new(150, 215, 128, 40, "", 24, false, false, false, "switchClickTool", "main", true);
        private readonly GuiButton SwitchClickReset = new(288, 215, 128, 40, 3, "RESET", 24);

        private readonly GuiLabel ToggleQuantumLabel = new(150, 259, 128, 26, "Toggle Quantum", 24, false, false, "main", false);
        private readonly GuiLabel ToggleQuantumCAS = new(426, 293, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox ToggleQuantumBox = new(150, 285, 128, 40, "", 24, false, false, false, "quantum", "main", true);
        private readonly GuiButton ToggleQuantumReset = new(288, 285, 128, 40, 4, "RESET", 24);

        private readonly GuiLabel OpenTimingsLabel = new(150, 329, 128, 26, "Open Timings", 24, false, false, "main", false);
        private readonly GuiLabel OpenTimingsCAS = new(426, 363, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox OpenTimingsBox = new(150, 355, 128, 40, "", 24, false, false, false, "openTimings", "main", true);
        private readonly GuiButton OpenTimingsReset = new(288, 355, 128, 40, 5, "RESET", 24);

        private readonly GuiLabel OpenBookmarksLabel = new(150, 399, 128, 26, "Open Bookmarks", 24, false, false, "main", false);
        private readonly GuiLabel OpenBookmarksCAS = new(426, 433, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox OpenBookmarksBox = new(150, 425, 128, 40, "", 24, false, false, false, "openBookmarks", "main", true);
        private readonly GuiButton OpenBookmarksReset = new(288, 425, 128, 40, 6, "RESET", 24);

        private readonly GuiLabel StoreNodesLabel = new(150, 469, 128, 26, "Store Bezier Nodes", 24, false, false, "main", false);
        private readonly GuiLabel StoreNodesCAS = new(426, 503, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox StoreNodesBox = new(150, 495, 128, 40, "", 24, false, false, false, "storeNodes", "main", true);
        private readonly GuiButton StoreNodesReset = new(288, 495, 128, 40, 7, "RESET", 24);

        private readonly GuiLabel DrawBezierLabel = new(150, 539, 128, 26, "Draw Bezier Curve", 24, false, false, "main", false);
        private readonly GuiLabel DrawBezierCAS = new(426, 573, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox DrawBezierBox = new(150, 565, 128, 40, "", 24, false, false, false, "drawBezier", "main", true);
        private readonly GuiButton DrawBezierReset = new(288, 565, 128, 40, 8, "RESET", 24);

        private readonly GuiLabel AnchorNodeLabel = new(150, 609, 128, 26, "Anchor Bezier Node", 24, false, false, "main", false);
        private readonly GuiLabel AnchorNodeCAS = new(426, 643, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox AnchorNodeBox = new(150, 635, 128, 40, "", 24, false, false, false, "anchorNode", "main", true);
        private readonly GuiButton AnchorNodeReset = new(288, 635, 128, 40, 9, "RESET", 24);

        private readonly GuiLabel OpenDirectoryLabel = new(150, 679, 128, 26, "Open Directory", 24, false, false, "main", false);
        private readonly GuiLabel OpenDirectoryCAS = new(426, 713, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox OpenDirectoryBox = new(150, 705, 128, 40, "", 24, false, false, false, "openDirectory", "main", true);
        private readonly GuiButton OpenDirectoryReset = new(288, 705, 128, 40, 10, "RESET", 24);

        private readonly GuiLabel ExportSSPMLabel = new(150, 749, 128, 26, "Export SSPM", 24, false, false, "main", false);
        private readonly GuiLabel ExportSSPMCAS = new(426, 783, 256, 40, "", 24, false, false, "main", false);
        private readonly GuiTextbox ExportSSPMBox = new(150, 775, 128, 40, "", 24, false, false, false, "exportSSPM", "main", true);
        private readonly GuiButton ExportSSPMReset = new(288, 775, 128, 40, 11, "RESET", 24);

        private readonly GuiLabel GridLabel = new(1366, 49, 128, 26, "Grid", 24, false, false, "main", false);
        private readonly GuiTextbox GridTLBox = new(1366, 75, 128, 62, "", 24, false, false, false, "gridKey0", "main", true);
        private readonly GuiButton GridTLReset = new(1366, 141, 128, 62, 90, "RESET", 32);
        private readonly GuiTextbox GridTCBox = new(1504, 75, 128, 62, "", 24, false, false, false, "gridKey1", "main", true);
        private readonly GuiButton GridTCReset = new(1504, 141, 128, 62, 91, "RESET", 32);
        private readonly GuiTextbox GridTRBox = new(1642, 75, 128, 62, "", 24, false, false, false, "gridKey2", "main", true);
        private readonly GuiButton GridTRReset = new(1642, 141, 128, 62, 92, "RESET", 32);
        private readonly GuiTextbox GridMLBox = new(1366, 213, 128, 62, "", 24, false, false, false, "gridKey3", "main", true);
        private readonly GuiButton GridMLReset = new(1366, 279, 128, 62, 93, "RESET", 32);
        private readonly GuiTextbox GridMCBox = new(1504, 213, 128, 62, "", 24, false, false, false, "gridKey4", "main", true);
        private readonly GuiButton GridMCReset = new(1504, 279, 128, 62, 94, "RESET", 32);
        private readonly GuiTextbox GridMRBox = new(1642, 213, 128, 62, "", 24, false, false, false, "gridKey5", "main", true);
        private readonly GuiButton GridMRReset = new(1642, 279, 128, 62, 95, "RESET", 32);
        private readonly GuiTextbox GridBLBox = new(1366, 351, 128, 62, "", 24, false, false, false, "gridKey6", "main", true);
        private readonly GuiButton GridBLReset = new(1366, 417, 128, 62, 96, "RESET", 32);
        private readonly GuiTextbox GridBCBox = new(1504, 351, 128, 62, "", 24, false, false, false, "gridKey7", "main", true);
        private readonly GuiButton GridBCReset = new(1504, 417, 128, 62, 97, "RESET", 32);
        private readonly GuiTextbox GridBRBox = new(1642, 351, 128, 62, "", 24, false, false, false, "gridKey8", "main", true);
        private readonly GuiButton GridBRReset = new(1642, 417, 128, 62, 98, "RESET", 32);

        private readonly GuiCheckbox CtrlIndicator = new(64, 828, 64, 64, "", "CTRL Held", 32);
        private readonly GuiCheckbox AltIndicator = new(64, 912, 64, 64, "", "ALT Held", 32);
        private readonly GuiCheckbox ShiftIndicator = new(64, 996, 64, 64, "", "SHIFT Held", 32);

        private readonly GuiLabel StaticKeysLabel = new(480, 150, 960, 40, "", 24);

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
                ">> CTRL + SHIFT + SCROLL to increment by 0.5",
                "",
                "> Scroll through song: SCROLL/LEFT/RIGHT",
                "",
                "> Place stored patterns: 0-9",
                ">> Hold SHIFT to store selected notes as the key's pattern",
                ">> Hold CTRL to clear the key's pattern",
                "",
                "> Select all: CTRL + A",
                "> Save: CTRL + S",
                "> Save as: CTRL + SHIFT + S",
                "> Undo: CTRL + Z",
                "> Redo: CTRL + Y",
                "> Copy: CTRL + C",
                "> Paste: CTRL + V",
                "> Cut: CTRL + X",
                "> Fullscreen: F11",
                "> Delete: DELETE/BACKSPACE",
                "> Play/Pause: SPACE",
                "> Deselect all: ESCAPE"
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
