using System.Collections.Generic;
using System.Drawing;
using OpenTK.Input;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.GUI
{
    class GuiWindowKeybinds : Gui
    {
        private readonly GuiButton BackButton = new GuiButton(655, 930, 600, 100, 0, "RETURN TO SETTINGS", 48, false, false, "square");

        private readonly GuiTextbox HFlipBox = new GuiTextbox(150, 75, 128, 40, "", 24, false, false, false, "hFlip", "main", true);
        private readonly GuiButton HFlipReset = new GuiButton(288, 75, 128, 40, 1, "RESET", 24);

        private readonly GuiTextbox VFlipBox = new GuiTextbox(150, 145, 128, 40, "", 24, false, false, false, "vFlip", "main", true);
        private readonly GuiButton VFlipReset = new GuiButton(288, 145, 128, 40, 2, "RESET", 24);

        private readonly GuiTextbox SwitchClickBox = new GuiTextbox(150, 215, 128, 40, "", 24, false, false, false, "switchClickTool", "main", true);
        private readonly GuiButton SwitchClickReset = new GuiButton(288, 215, 128, 40, 3, "RESET", 24);

        private readonly GuiTextbox ToggleQuantumBox = new GuiTextbox(150, 285, 128, 40, "", 24, false, false, false, "quantum", "main", true);
        private readonly GuiButton ToggleQuantumReset = new GuiButton(288, 285, 128, 40, 4, "RESET", 24);

        private readonly GuiTextbox OpenTimingsBox = new GuiTextbox(150, 355, 128, 40, "", 24, false, false, false, "openTimings", "main", true);
        private readonly GuiButton OpenTimingsReset = new GuiButton(288, 355, 128, 40, 5, "RESET", 24);

        private readonly GuiTextbox OpenBookmarksBox = new GuiTextbox(150, 425, 128, 40, "", 24, false, false, false, "openBookmarks", "main", true);
        private readonly GuiButton OpenBookmarksReset = new GuiButton(288, 425, 128, 40, 6, "RESET", 24);

        private readonly GuiTextbox StoreNodesBox = new GuiTextbox(150, 495, 128, 40, "", 24, false, false, false, "storeNodes", "main", true);
        private readonly GuiButton StoreNodesReset = new GuiButton(288, 495, 128, 40, 7, "RESET", 24);

        private readonly GuiTextbox DrawBezierBox = new GuiTextbox(150, 565, 128, 40, "", 24, false, false, false, "drawBezier", "main", true);
        private readonly GuiButton DrawBezierReset = new GuiButton(288, 565, 128, 40, 8, "RESET", 24);

        private readonly GuiTextbox AnchorNodeBox = new GuiTextbox(150, 635, 128, 40, "", 24, false, false, false, "anchorNode", "main", true);
        private readonly GuiButton AnchorNodeReset = new GuiButton(288, 635, 128, 40, 9, "RESET", 24);

        private readonly GuiTextbox OpenDirectoryBox = new GuiTextbox(150, 705, 128, 40, "", 24, false, false, false, "openDirectory", "main", true);
        private readonly GuiButton OpenDirectoryReset = new GuiButton(288, 705, 128, 40, 10, "RESET", 24);

        private readonly GuiTextbox GridTLBox = new GuiTextbox(1366, 75, 128, 62, "", 24, false, false, false, "gridKey0", "main", true);
        private readonly GuiButton GridTLReset = new GuiButton(1366, 141, 128, 62, 90, "RESET", 32);
        private readonly GuiTextbox GridTCBox = new GuiTextbox(1504, 75, 128, 62, "", 24, false, false, false, "gridKey1", "main", true);
        private readonly GuiButton GridTCReset = new GuiButton(1504, 141, 128, 62, 91, "RESET", 32);
        private readonly GuiTextbox GridTRBox = new GuiTextbox(1642, 75, 128, 62, "", 24, false, false, false, "gridKey2", "main", true);
        private readonly GuiButton GridTRReset = new GuiButton(1642, 141, 128, 62, 92, "RESET", 32);
        private readonly GuiTextbox GridMLBox = new GuiTextbox(1366, 213, 128, 62, "", 24, false, false, false, "gridKey3", "main", true);
        private readonly GuiButton GridMLReset = new GuiButton(1366, 279, 128, 62, 93, "RESET", 32);
        private readonly GuiTextbox GridMCBox = new GuiTextbox(1504, 213, 128, 62, "", 24, false, false, false, "gridKey4", "main", true);
        private readonly GuiButton GridMCReset = new GuiButton(1504, 279, 128, 62, 94, "RESET", 32);
        private readonly GuiTextbox GridMRBox = new GuiTextbox(1642, 213, 128, 62, "", 24, false, false, false, "gridKey5", "main", true);
        private readonly GuiButton GridMRReset = new GuiButton(1642, 279, 128, 62, 95, "RESET", 32);
        private readonly GuiTextbox GridBLBox = new GuiTextbox(1366, 351, 128, 62, "", 24, false, false, false, "gridKey6", "main", true);
        private readonly GuiButton GridBLReset = new GuiButton(1366, 417, 128, 62, 96, "RESET", 32);
        private readonly GuiTextbox GridBCBox = new GuiTextbox(1504, 351, 128, 62, "", 24, false, false, false, "gridKey7", "main", true);
        private readonly GuiButton GridBCReset = new GuiButton(1504, 417, 128, 62, 97, "RESET", 32);
        private readonly GuiTextbox GridBRBox = new GuiTextbox(1642, 351, 128, 62, "", 24, false, false, false, "gridKey8", "main", true);
        private readonly GuiButton GridBRReset = new GuiButton(1642, 417, 128, 62, 98, "RESET", 32);

        private readonly GuiCheckbox CtrlIndicator = new GuiCheckbox(64, 828, 64, 64, "", "CTRL Held", 32);
        private readonly GuiCheckbox AltIndicator = new GuiCheckbox(64, 912, 64, 64, "", "ALT Held", 32);
        private readonly GuiCheckbox ShiftIndicator = new GuiCheckbox(64, 996, 64, 64, "", "SHIFT Held", 32);

        private int txid;
        private bool bgImg;

        private string staticKeys;

        public GuiWindowKeybinds() : base(0, 0, MainWindow.Instance.ClientSize.Width, MainWindow.Instance.ClientSize.Height)
        {
            buttons = new List<GuiButton> { BackButton, HFlipReset, VFlipReset, SwitchClickReset, ToggleQuantumReset, OpenTimingsReset, OpenBookmarksReset, StoreNodesReset, DrawBezierReset, AnchorNodeReset,
            OpenDirectoryReset, GridTLReset, GridTCReset, GridTRReset, GridMLReset, GridMCReset, GridMRReset, GridBLReset, GridBCReset, GridBRReset };
            checkboxes = new List<GuiCheckbox> { CtrlIndicator, AltIndicator, ShiftIndicator };
            boxes = new List<GuiTextbox> { HFlipBox, VFlipBox, SwitchClickBox, ToggleQuantumBox, OpenTimingsBox, OpenBookmarksBox, StoreNodesBox, DrawBezierBox, AnchorNodeBox, OpenDirectoryBox,
            GridTLBox, GridTCBox, GridTRBox, GridMLBox, GridMCBox, GridMRBox, GridBLBox, GridBCBox, GridBRBox };

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
            staticKeys = string.Join("\n", staticList);

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
            var editor = MainWindow.Instance;

            CtrlIndicator.Toggle = editor.ctrlHeld;
            AltIndicator.Toggle = editor.altHeld;
            ShiftIndicator.Toggle = editor.shiftHeld;

            var widthdiff = rect.Width / 1920f;
            var heightdiff = rect.Height / 1080f;

            GL.Color4(bgImg ? Color.FromArgb(255, 255, 255, 255) : Color.FromArgb(255, 30, 30, 30));

            if (bgImg)
                GLSpecial.TexturedQuad(rect, 0, 0, 1, 1, txid);
            else
                GLSpecial.Rect(rect);

            GL.Color3(1f, 1f, 1f);

            var labelSize = (int)(24f * heightdiff);
            var labelOffset = 26 * heightdiff;
            var casOffset = 12 * heightdiff;

            RenderText("Horizontal Flip", HFlipBox.rect.X, HFlipBox.rect.Y - labelOffset, labelSize);
            RenderText(CAS(Settings.settings["hFlip"]), HFlipReset.rect.Right + 10, HFlipReset.rect.Y + HFlipReset.rect.Height / 2f - casOffset, labelSize);

            RenderText("Vertical Flip", VFlipBox.rect.X, VFlipBox.rect.Y - labelOffset, labelSize);
            RenderText(CAS(Settings.settings["vFlip"]), VFlipReset.rect.Right + 10, VFlipReset.rect.Y + VFlipReset.rect.Height / 2f - casOffset, labelSize);

            RenderText("Switch Click Function", SwitchClickBox.rect.X, SwitchClickBox.rect.Y - labelOffset, labelSize);
            RenderText(CAS(Settings.settings["switchClickTool"]), SwitchClickReset.rect.Right + 10, SwitchClickReset.rect.Y + SwitchClickReset.rect.Height / 2f - casOffset, labelSize);

            RenderText("Toggle Quantum", ToggleQuantumBox.rect.X, ToggleQuantumBox.rect.Y - labelOffset, labelSize);
            RenderText(CAS(Settings.settings["quantum"]), ToggleQuantumReset.rect.Right + 10, ToggleQuantumReset.rect.Y + ToggleQuantumReset.rect.Height / 2f - casOffset, labelSize);

            RenderText("Open Timings", OpenTimingsBox.rect.X, OpenTimingsBox.rect.Y - labelOffset, labelSize);
            RenderText(CAS(Settings.settings["openTimings"]), OpenTimingsReset.rect.Right + 10, OpenTimingsReset.rect.Y + OpenTimingsReset.rect.Height / 2f - casOffset, labelSize);

            RenderText("Open Bookmarks", OpenBookmarksBox.rect.X, OpenBookmarksBox.rect.Y - labelOffset, labelSize);
            RenderText(CAS(Settings.settings["openBookmarks"]), OpenBookmarksReset.rect.Right + 10, OpenBookmarksReset.rect.Y + OpenBookmarksReset.rect.Height / 2f - casOffset, labelSize);

            RenderText("Store Bezier Nodes", StoreNodesBox.rect.X, StoreNodesBox.rect.Y - labelOffset, labelSize);
            RenderText(CAS(Settings.settings["storeNodes"]), StoreNodesReset.rect.Right + 10, StoreNodesReset.rect.Y + StoreNodesReset.rect.Height / 2f - casOffset, labelSize);

            RenderText("Draw Bezier Curve", DrawBezierBox.rect.X, DrawBezierBox.rect.Y - labelOffset, labelSize);
            RenderText(CAS(Settings.settings["drawBezier"]), DrawBezierReset.rect.Right + 10, DrawBezierReset.rect.Y + DrawBezierReset.rect.Height / 2f - casOffset, labelSize);

            RenderText("Anchor Bezier Node", AnchorNodeBox.rect.X, AnchorNodeBox.rect.Y - labelOffset, labelSize);
            RenderText(CAS(Settings.settings["anchorNode"]), AnchorNodeReset.rect.Right + 10, AnchorNodeReset.rect.Y + AnchorNodeReset.rect.Height / 2f - casOffset, labelSize);

            RenderText("Open Directory", OpenDirectoryBox.rect.X, OpenDirectoryBox.rect.Y - labelOffset, labelSize);
            RenderText(CAS(Settings.settings["openDirectory"]), OpenDirectoryReset.rect.Right + 10, OpenDirectoryReset.rect.Y + OpenDirectoryReset.rect.Height / 2f - casOffset, labelSize);

            RenderText("Grid", GridTLBox.rect.X, GridTLBox.rect.Y - 26, 24);

            var staticWidth = TextWidth(staticKeys, labelSize);
            RenderText(staticKeys, 960 * widthdiff - staticWidth / 2f, 150 * heightdiff, labelSize);

            base.Render(mousex, mousey, frametime);
        }

        public override void OnResize(Size size)
        {
            rect = new RectangleF(0, 0, size.Width, size.Height);

            base.OnResize(size);
        }

        protected override void OnButtonClicked(int id)
        {
            switch (id)
            {
                case 0:
                    MainWindow.Instance.SwitchWindow(new GuiWindowSettings());

                    break;

                case 1:
                    Settings.settings["hFlip"] = new Keybind(Key.H, false, false, true);
                    HFlipBox.text = "H";
                    break;
                case 2:
                    Settings.settings["vFlip"] = new Keybind(Key.V, false, false, true);
                    VFlipBox.text = "V";
                    break;
                case 3:
                    Settings.settings["switchClickTool"] = new Keybind(Key.Tab, false, false, false);
                    SwitchClickBox.text = "TAB";
                    break;
                case 4:
                    Settings.settings["quantum"] = new Keybind(Key.Q, true, false, false);
                    ToggleQuantumBox.text = "Q";
                    break;
                case 5:
                    Settings.settings["openTimings"] = new Keybind(Key.T, true, false, false);
                    OpenTimingsBox.text = "T";
                    break;
                case 6:
                    Settings.settings["openBookmarks"] = new Keybind(Key.B, true, false, false);
                    OpenBookmarksBox.text = "B";
                    break;
                case 7:
                    Settings.settings["storeNodes"] = new Keybind(Key.S, false, false, true);
                    StoreNodesBox.text = "S";
                    break;
                case 8:
                    Settings.settings["drawBezier"] = new Keybind(Key.D, false, false, true);
                    DrawBezierBox.text = "D";
                    break;
                case 9:
                    Settings.settings["anchorNode"] = new Keybind(Key.A, false, false, true);
                    AnchorNodeBox.text = "A";
                    break;
                case 10:
                    Settings.settings["openDirectory"] = new Keybind(Key.D, true, false, true);
                    OpenDirectoryBox.text = "D";
                    break;

                case 90:
                    Settings.settings["gridKeys"][0] = Key.Q;
                    GridTLBox.text = "Q";
                    break;
                case 91:
                    Settings.settings["gridKeys"][1] = Key.W;
                    GridTCBox.text = "W";
                    break;
                case 92:
                    Settings.settings["gridKeys"][2] = Key.E;
                    GridTRBox.text = "E";
                    break;
                case 93:
                    Settings.settings["gridKeys"][3] = Key.A;
                    GridMLBox.text = "A";
                    break;
                case 94:
                    Settings.settings["gridKeys"][4] = Key.S;
                    GridMCBox.text = "S";
                    break;
                case 95:
                    Settings.settings["gridKeys"][5] = Key.D;
                    GridMRBox.text = "D";
                    break;
                case 96:
                    Settings.settings["gridKeys"][6] = Key.Z;
                    GridBLBox.text = "Z";
                    break;
                case 97:
                    Settings.settings["gridKeys"][7] = Key.X;
                    GridBCBox.text = "X";
                    break;
                case 98:
                    Settings.settings["gridKeys"][8] = Key.C;
                    GridBRBox.text = "C";
                    break;
            }
        }

        private string CAS(Keybind key)
        {
            var cas = new List<string>();

            if (key.CTRL)
                cas.Add("CTRL");
            if (key.ALT)
                cas.Add("ALT");
            if (key.SHIFT)
                cas.Add("SHIFT");

            return string.Join(" + ", cas);
        }
    }
}
