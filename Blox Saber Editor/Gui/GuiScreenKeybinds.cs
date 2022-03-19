﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK.Input;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.Gui
{
    class GuiScreenKeybinds : GuiScreen
    {
        private readonly int _textureId;
        private bool bgImg = false;

        private readonly GuiTextBox SelectAllBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.SelectAll.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton SelectAllReset = new GuiButton(1, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox SaveBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Save.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton SaveReset = new GuiButton(2, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox SaveAsBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.SaveAs.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton SaveAsReset = new GuiButton(3, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox UndoBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Undo.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton UndoReset = new GuiButton(4, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox RedoBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Redo.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton RedoReset = new GuiButton(5, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox CopyBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Copy.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton CopyReset = new GuiButton(6, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox PasteBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Paste.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton PasteReset = new GuiButton(7, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox DeleteBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Delete.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton DeleteReset = new GuiButton(8, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox HFlipBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.HFlip.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton HFlipReset = new GuiButton(9, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox VFlipBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.VFlip.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton VFlipReset = new GuiButton(10, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox SwitchClickToolBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.SwitchClickTool.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton SwitchClickToolReset = new GuiButton(11, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox QuantumBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Quantum.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton QuantumReset = new GuiButton(12, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox OpenTimingsBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.OpenTimings.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton OpenTimingsReset = new GuiButton(13, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox OpenBookmarksBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.OpenBookmarks.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton OpenBookmarksReset = new GuiButton(14, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox StoreNodesBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.StoreNodes.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton StoreNodesReset = new GuiButton(15, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox DrawBezierBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.DrawBezier.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiButton DrawBezierReset = new GuiButton(16, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox TLBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.TL.ToString().ToUpper(), Centered = true };
        private readonly GuiButton TLReset = new GuiButton(90, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox TCBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.TC.ToString().ToUpper(), Centered = true };
        private readonly GuiButton TCReset = new GuiButton(91, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox TRBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.TR.ToString().ToUpper(), Centered = true };
        private readonly GuiButton TRReset = new GuiButton(92, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox MLBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.ML.ToString().ToUpper(), Centered = true };
        private readonly GuiButton MLReset = new GuiButton(93, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox MCBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.MC.ToString().ToUpper(), Centered = true };
        private readonly GuiButton MCReset = new GuiButton(94, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox MRBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.MR.ToString().ToUpper(), Centered = true };
        private readonly GuiButton MRReset = new GuiButton(95, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox BLBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.BL.ToString().ToUpper(), Centered = true };
        private readonly GuiButton BLReset = new GuiButton(96, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox BCBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.BC.ToString().ToUpper(), Centered = true };
        private readonly GuiButton BCReset = new GuiButton(97, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox BRBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.BR.ToString().ToUpper(), Centered = true };
        private readonly GuiButton BRReset = new GuiButton(98, 0, 0, 0, 0, "RESET", false);


        private readonly GuiCheckBox CTRLIndicator = new GuiCheckBox(-1, "CTRL Held", 0, 0, 0, 0, EditorWindow.Instance._controlDown);
        private readonly GuiCheckBox SHIFTIndicator = new GuiCheckBox(-1, "SHIFT Held", 0, 0, 0, 0, EditorWindow.Instance._shiftDown);
        private readonly GuiCheckBox ALTIndicator = new GuiCheckBox(-1, "ALT Held", 0, 0, 0, 0, EditorWindow.Instance._altDown);

        private readonly GuiButton _backButton = new GuiButton(0, 0, 0, 600, 100, "RETURN TO SETTINGS", "square", 100);

        public GuiScreenKeybinds() : base(0, 0, EditorWindow.Instance.ClientSize.Width, EditorWindow.Instance.ClientSize.Height)
        {
            Buttons.Add(SelectAllReset);
            Buttons.Add(SaveReset);
            Buttons.Add(SaveAsReset);
            Buttons.Add(UndoReset);
            Buttons.Add(RedoReset);
            Buttons.Add(CopyReset);
            Buttons.Add(PasteReset);
            Buttons.Add(DeleteReset);
            Buttons.Add(HFlipReset);
            Buttons.Add(VFlipReset);
            Buttons.Add(SwitchClickToolReset);
            Buttons.Add(QuantumReset);
            Buttons.Add(OpenTimingsReset);
            Buttons.Add(OpenBookmarksReset);
            Buttons.Add(StoreNodesReset);
            Buttons.Add(DrawBezierReset);

            Buttons.Add(TLReset);
            Buttons.Add(TCReset);
            Buttons.Add(TRReset);
            Buttons.Add(MLReset);
            Buttons.Add(MCReset);
            Buttons.Add(MRReset);
            Buttons.Add(BLReset);
            Buttons.Add(BCReset);
            Buttons.Add(BRReset);

            Buttons.Add(CTRLIndicator);
            Buttons.Add(SHIFTIndicator);
            Buttons.Add(ALTIndicator);

            Buttons.Add(_backButton);

            if (File.Exists(Path.Combine(EditorWindow.Instance.LauncherDir, "background_menu.png")))
            {
                bgImg = true;
                using (Bitmap img = new Bitmap(Path.Combine(EditorWindow.Instance.LauncherDir, "background_menu.png")))
                {
                    _textureId = TextureManager.GetOrRegister("settingsbg", img, true);
                }
            }

            OnResize(EditorWindow.Instance.ClientSize);
        }

        public override void Render(float delta, float mouseX, float mouseY)
        {
            var size = EditorWindow.Instance.ClientSize;
            var scale = Math.Min(size.Width / 1920f, size.Height / 1080f);
            var labelsize = (int)(24 * scale);
            var labeloffset = (int)(26 * scale);

            if (bgImg)
            {
                GL.Color4(Color.FromArgb(255, 255, 255, 255));
                Glu.RenderTexturedQuad(0, 0, size.Width, size.Height, 0, 0, 1, 1, _textureId);
            }
            else
            {
                GL.Color4(Color.FromArgb(255, 30, 30, 30));
                Glu.RenderQuad(0, 0, size.Width, size.Height);
            }

            if (CTRLIndicator.Toggle != EditorWindow.Instance._controlDown)
                CTRLIndicator.Toggle = EditorWindow.Instance._controlDown;
            if (SHIFTIndicator.Toggle != EditorWindow.Instance._shiftDown)
                SHIFTIndicator.Toggle = EditorWindow.Instance._shiftDown;
            if (ALTIndicator.Toggle != EditorWindow.Instance._altDown)
                ALTIndicator.Toggle = EditorWindow.Instance._altDown;

            var fr = EditorWindow.Instance.FontRenderer;

            GL.Color4(Color.FromArgb(255, 255, 255, 255));

            fr.Render("Select All", (int)SelectAllBox.ClientRectangle.X, (int)SelectAllBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.SelectAll), (int)SelectAllReset.ClientRectangle.Right + 10, (int)SelectAllReset.ClientRectangle.Y + (int)(SelectAllReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Save", (int)SaveBox.ClientRectangle.X, (int)SaveBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.Save), (int)SaveReset.ClientRectangle.Right + 10, (int)SaveReset.ClientRectangle.Y + (int)(SaveReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Save As", (int)SaveAsBox.ClientRectangle.X, (int)SaveAsBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.SaveAs), (int)SaveAsReset.ClientRectangle.Right + 10, (int)SaveAsReset.ClientRectangle.Y + (int)(SaveAsReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Undo", (int)UndoBox.ClientRectangle.X, (int)UndoBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.Undo), (int)UndoReset.ClientRectangle.Right + 10, (int)UndoReset.ClientRectangle.Y + (int)(UndoReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Redo", (int)RedoBox.ClientRectangle.X, (int)RedoBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.Redo), (int)RedoReset.ClientRectangle.Right + 10, (int)RedoReset.ClientRectangle.Y + (int)(RedoReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Copy", (int)CopyBox.ClientRectangle.X, (int)CopyBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.Copy), (int)CopyReset.ClientRectangle.Right + 10, (int)CopyReset.ClientRectangle.Y + (int)(CopyReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Paste", (int)PasteBox.ClientRectangle.X, (int)PasteBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.Paste), (int)PasteReset.ClientRectangle.Right + 10, (int)PasteReset.ClientRectangle.Y + (int)(PasteReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Delete Note(s)", (int)DeleteBox.ClientRectangle.X, (int)DeleteBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.Delete), (int)DeleteReset.ClientRectangle.Right + 10, (int)DeleteReset.ClientRectangle.Y + (int)(DeleteReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Horizontal Flip", (int)HFlipBox.ClientRectangle.X, (int)HFlipBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.HFlip), (int)HFlipReset.ClientRectangle.Right + 10, (int)HFlipReset.ClientRectangle.Y + (int)(HFlipReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Vertical Flip", (int)VFlipBox.ClientRectangle.X, (int)VFlipBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.VFlip), (int)VFlipReset.ClientRectangle.Right + 10, (int)VFlipReset.ClientRectangle.Y + (int)(VFlipReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Switch Click Function", (int)SwitchClickToolBox.ClientRectangle.X, (int)SwitchClickToolBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.SwitchClickTool), (int)SwitchClickToolReset.ClientRectangle.Right + 10, (int)SwitchClickToolReset.ClientRectangle.Y + (int)(SwitchClickToolReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Toggle Quantum", (int)QuantumBox.ClientRectangle.X, (int)QuantumBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.Quantum), (int)QuantumReset.ClientRectangle.Right + 10, (int)QuantumReset.ClientRectangle.Y + (int)(QuantumReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Open Timings", (int)OpenTimingsBox.ClientRectangle.X, (int)OpenTimingsBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.OpenTimings), (int)OpenTimingsReset.ClientRectangle.Right + 10, (int)OpenTimingsReset.ClientRectangle.Y + (int)(OpenTimingsReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Open Bookmarks", (int)OpenBookmarksBox.ClientRectangle.X, (int)OpenBookmarksBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.OpenBookmarks), (int)OpenBookmarksReset.ClientRectangle.Right + 10, (int)OpenBookmarksReset.ClientRectangle.Y + (int)(OpenBookmarksReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Store Bezier Nodes", (int)StoreNodesBox.ClientRectangle.X, (int)StoreNodesBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.StoreNodes), (int)StoreNodesReset.ClientRectangle.Right + 10, (int)StoreNodesReset.ClientRectangle.Y + (int)(StoreNodesReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Draw Bezier Curve", (int)DrawBezierBox.ClientRectangle.X, (int)DrawBezierBox.ClientRectangle.Y - labeloffset, labelsize);
            fr.Render(CSAString(EditorSettings.DrawBezier), (int)DrawBezierReset.ClientRectangle.Right + 10, (int)DrawBezierReset.ClientRectangle.Y + (int)(DrawBezierReset.ClientRectangle.Height / 2) - (int)(12 * scale), labelsize);

            fr.Render("Grid", (int)TLBox.ClientRectangle.X, (int)TLBox.ClientRectangle.Y - 26, 24);

            string[] lockedlist =
            {
                "Other [LOCKED]",
                "Zoom: CTRL + SCROLL",
                "Beat Divisor: SHIFT + SCROLL",
                "Travel through timeline: SCROLL/LEFT/RIGHT",
                "Play/Pause: SPACE",
                "Fullscreen: F11",
                "Stored Patterns: 0-9",
                ">Hold SHIFT to bind a pattern to the key",
                ">Hold CTRL to clear a pattern from the key",
            };

            var lockedstring = string.Join("\n>", lockedlist);
            var lockedwidth = fr.GetWidth(lockedstring, labelsize);
            var lockedoffset = (_backButton.ClientRectangle.Width - lockedwidth) / 2;

            fr.Render(lockedstring, (int)(_backButton.ClientRectangle.X + lockedoffset), (int)_backButton.ClientRectangle.Top - (labelsize + 2) * lockedlist.Count(), labelsize);

            SelectAllBox.Render(delta, mouseX, mouseY);
            SaveBox.Render(delta, mouseX, mouseY);
            SaveAsBox.Render(delta, mouseX, mouseY);
            UndoBox.Render(delta, mouseX, mouseY);
            RedoBox.Render(delta, mouseX, mouseY);
            CopyBox.Render(delta, mouseX, mouseY);
            PasteBox.Render(delta, mouseX, mouseY);
            DeleteBox.Render(delta, mouseX, mouseY);
            HFlipBox.Render(delta, mouseX, mouseY);
            VFlipBox.Render(delta, mouseX, mouseY);
            SwitchClickToolBox.Render(delta, mouseX, mouseY);
            QuantumBox.Render(delta, mouseX, mouseY);
            OpenTimingsBox.Render(delta, mouseX, mouseY);
            OpenBookmarksBox.Render(delta, mouseX, mouseY);
            StoreNodesBox.Render(delta, mouseX, mouseY);
            DrawBezierBox.Render(delta, mouseX, mouseY);

            TLBox.Render(delta, mouseX, mouseY);
            TCBox.Render(delta, mouseX, mouseY);
            TRBox.Render(delta, mouseX, mouseY);
            MLBox.Render(delta, mouseX, mouseY);
            MCBox.Render(delta, mouseX, mouseY);
            MRBox.Render(delta, mouseX, mouseY);
            BLBox.Render(delta, mouseX, mouseY);
            BCBox.Render(delta, mouseX, mouseY);
            BRBox.Render(delta, mouseX, mouseY);

            base.Render(delta, mouseX, mouseY);
        }

        private string CSAString(EditorSettings.KeyType key)
        {
            List<string> csa = new List<string>();
            if (key.CTRL)
                csa.Add("CTRL");
            if (key.SHIFT)
                csa.Add("SHIFT");
            if (key.ALT)
                csa.Add("ALT");
            return string.Join(" + ", csa);
        }

        public override void OnResize(Size size)
        {
            ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);
            var widthdiff = size.Width / 1920f;
            var heightdiff = size.Height / 1080f;

            EditorWindow.Instance.FontRenderer.Render("", 0, 0, (int)(24 * Math.Min(widthdiff, heightdiff)));

            var csawidth = EditorWindow.Instance.FontRenderer.GetWidth("CTRL + SHIFT + ALT", (int)(24 * Math.Min(widthdiff, heightdiff)));

            SelectAllBox.ClientRectangle.Size = new SizeF(128 * widthdiff, 40 * heightdiff);
            SelectAllReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            SaveBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            SaveReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            SaveAsBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            SaveAsReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            UndoBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            UndoReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            RedoBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            RedoReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            CopyBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            CopyReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            PasteBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            PasteReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            DeleteBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            DeleteReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            HFlipBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            HFlipReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            VFlipBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            VFlipReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            SwitchClickToolBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            SwitchClickToolReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            QuantumBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            QuantumReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            OpenTimingsBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            OpenTimingsReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            OpenBookmarksBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            OpenBookmarksReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            StoreNodesBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            StoreNodesReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            DrawBezierBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            DrawBezierReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            TLBox.ClientRectangle.Size = new SizeF(128 * widthdiff, 62 * heightdiff);
            TLReset.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            TCBox.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            TCReset.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            TRBox.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            TRReset.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            MLBox.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            MLReset.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            MCBox.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            MCReset.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            MRBox.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            MRReset.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            BLBox.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            BLReset.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            BCBox.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            BCReset.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            BRBox.ClientRectangle.Size = TLBox.ClientRectangle.Size;
            BRReset.ClientRectangle.Size = TLBox.ClientRectangle.Size;

            CTRLIndicator.ClientRectangle.Size = new SizeF(64 * widthdiff, 64 * heightdiff);
            SHIFTIndicator.ClientRectangle.Size = CTRLIndicator.ClientRectangle.Size;
            ALTIndicator.ClientRectangle.Size = CTRLIndicator.ClientRectangle.Size;

            _backButton.ClientRectangle.Size = new SizeF(600 * widthdiff, 100 * heightdiff);


            var widthspacelarge = csawidth + 80 * widthdiff;
            var widthspacesmall = 10 * widthdiff;
            var heightspace = 30 * heightdiff;
            SelectAllBox.ClientRectangle.Location = new PointF(150 * widthdiff, 75 * heightdiff);
            SelectAllReset.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.Right + widthspacesmall, SelectAllBox.ClientRectangle.Y);

            SaveBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, SelectAllBox.ClientRectangle.Bottom + heightspace);
            SaveReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, SaveBox.ClientRectangle.Y);

            SaveAsBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, SaveBox.ClientRectangle.Bottom + heightspace);
            SaveAsReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, SaveAsBox.ClientRectangle.Y);

            UndoBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, SaveAsBox.ClientRectangle.Bottom + heightspace);
            UndoReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, UndoBox.ClientRectangle.Y);

            RedoBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, UndoBox.ClientRectangle.Bottom + heightspace);
            RedoReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, RedoBox.ClientRectangle.Y);

            CopyBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, RedoBox.ClientRectangle.Bottom + heightspace);
            CopyReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, CopyBox.ClientRectangle.Y);

            PasteBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, CopyBox.ClientRectangle.Bottom + heightspace);
            PasteReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, PasteBox.ClientRectangle.Y);

            DeleteBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, PasteBox.ClientRectangle.Bottom + heightspace);
            DeleteReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, DeleteBox.ClientRectangle.Y);

            HFlipBox.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.Right + widthspacelarge, SelectAllBox.ClientRectangle.Y);
            HFlipReset.ClientRectangle.Location = new PointF(HFlipBox.ClientRectangle.Right + widthspacesmall, HFlipBox.ClientRectangle.Y);

            VFlipBox.ClientRectangle.Location = new PointF(HFlipBox.ClientRectangle.X, HFlipBox.ClientRectangle.Bottom + heightspace);
            VFlipReset.ClientRectangle.Location = new PointF(HFlipReset.ClientRectangle.X, VFlipBox.ClientRectangle.Y);

            SwitchClickToolBox.ClientRectangle.Location = new PointF(HFlipBox.ClientRectangle.X, VFlipBox.ClientRectangle.Bottom + heightspace);
            SwitchClickToolReset.ClientRectangle.Location = new PointF(HFlipReset.ClientRectangle.X, SwitchClickToolBox.ClientRectangle.Y);

            QuantumBox.ClientRectangle.Location = new PointF(HFlipBox.ClientRectangle.X, SwitchClickToolBox.ClientRectangle.Bottom + heightspace);
            QuantumReset.ClientRectangle.Location = new PointF(HFlipReset.ClientRectangle.X, QuantumBox.ClientRectangle.Y);

            OpenTimingsBox.ClientRectangle.Location = new PointF(HFlipBox.ClientRectangle.X, QuantumBox.ClientRectangle.Bottom + heightspace);
            OpenTimingsReset.ClientRectangle.Location = new PointF(HFlipReset.ClientRectangle.X, OpenTimingsBox.ClientRectangle.Y);

            OpenBookmarksBox.ClientRectangle.Location = new PointF(HFlipBox.ClientRectangle.X, OpenTimingsBox.ClientRectangle.Bottom + heightspace);
            OpenBookmarksReset.ClientRectangle.Location = new PointF(HFlipReset.ClientRectangle.X, OpenBookmarksBox.ClientRectangle.Y);

            StoreNodesBox.ClientRectangle.Location = new PointF(HFlipBox.ClientRectangle.X, OpenBookmarksBox.ClientRectangle.Bottom + heightspace);
            StoreNodesReset.ClientRectangle.Location = new PointF(HFlipReset.ClientRectangle.X, StoreNodesBox.ClientRectangle.Y);

            DrawBezierBox.ClientRectangle.Location = new PointF(HFlipBox.ClientRectangle.X, StoreNodesBox.ClientRectangle.Bottom + heightspace);
            DrawBezierReset.ClientRectangle.Location = new PointF(HFlipReset.ClientRectangle.X, DrawBezierBox.ClientRectangle.Y);


            TLBox.ClientRectangle.Location = new PointF(size.Width - TLBox.ClientRectangle.Width * 3 - 10 * 2 * widthdiff - 150 * widthdiff, HFlipBox.ClientRectangle.Y);
            TLReset.ClientRectangle.Location = new PointF(TLBox.ClientRectangle.X, TLBox.ClientRectangle.Bottom + 4 * heightdiff);
            TCBox.ClientRectangle.Location = new PointF(TLBox.ClientRectangle.Right + 10 * widthdiff, TLBox.ClientRectangle.Y);
            TCReset.ClientRectangle.Location = new PointF(TCBox.ClientRectangle.X, TLReset.ClientRectangle.Y);
            TRBox.ClientRectangle.Location = new PointF(TCBox.ClientRectangle.Right + 10 * widthdiff, TLBox.ClientRectangle.Y);
            TRReset.ClientRectangle.Location = new PointF(TRBox.ClientRectangle.X, TLReset.ClientRectangle.Y);
            MLBox.ClientRectangle.Location = new PointF(TLBox.ClientRectangle.X, TLReset.ClientRectangle.Bottom + 10 * heightdiff);
            MLReset.ClientRectangle.Location = new PointF(TLBox.ClientRectangle.X, MLBox.ClientRectangle.Bottom + 4 * heightdiff);
            MCBox.ClientRectangle.Location = new PointF(TCBox.ClientRectangle.X, MLBox.ClientRectangle.Y);
            MCReset.ClientRectangle.Location = new PointF(TCBox.ClientRectangle.X, MLReset.ClientRectangle.Y);
            MRBox.ClientRectangle.Location = new PointF(TRBox.ClientRectangle.X, MLBox.ClientRectangle.Y);
            MRReset.ClientRectangle.Location = new PointF(TRBox.ClientRectangle.X, MLReset.ClientRectangle.Y);
            BLBox.ClientRectangle.Location = new PointF(TLBox.ClientRectangle.X, MLReset.ClientRectangle.Bottom + 10 * heightdiff);
            BLReset.ClientRectangle.Location = new PointF(TLBox.ClientRectangle.X, BLBox.ClientRectangle.Bottom + 4 * heightdiff);
            BCBox.ClientRectangle.Location = new PointF(TCBox.ClientRectangle.X, BLBox.ClientRectangle.Y);
            BCReset.ClientRectangle.Location = new PointF(TCBox.ClientRectangle.X, BLReset.ClientRectangle.Y);
            BRBox.ClientRectangle.Location = new PointF(TRBox.ClientRectangle.X, BLBox.ClientRectangle.Y);
            BRReset.ClientRectangle.Location = new PointF(TRBox.ClientRectangle.X, BLReset.ClientRectangle.Y);

            CTRLIndicator.ClientRectangle.Location = new PointF(64 * widthdiff, size.Height - 3 * CTRLIndicator.ClientRectangle.Height - 3 * 20 * heightdiff);
            SHIFTIndicator.ClientRectangle.Location = new PointF(CTRLIndicator.ClientRectangle.X, CTRLIndicator.ClientRectangle.Bottom + 20 * heightdiff);
            ALTIndicator.ClientRectangle.Location = new PointF(CTRLIndicator.ClientRectangle.X, SHIFTIndicator.ClientRectangle.Bottom + 20 * heightdiff);

            _backButton.ClientRectangle.Location = new PointF(655 * widthdiff, 930 * heightdiff);


            base.OnResize(size);
        }

        private string GetFocused()
        {
            if (SelectAllBox.Focused)
                return "SelectAll";
            if (SaveBox.Focused)
                return "Save";
            if (SaveAsBox.Focused)
                return "SaveAs";
            if (UndoBox.Focused)
                return "Undo";
            if (RedoBox.Focused)
                return "Redo";
            if (CopyBox.Focused)
                return "Copy";
            if (PasteBox.Focused)
                return "Paste";
            if (DeleteBox.Focused)
                return "Delete";
            if (HFlipBox.Focused)
                return "HFlip";
            if (VFlipBox.Focused)
                return "VFlip";
            if (SwitchClickToolBox.Focused)
                return "SwitchClickTool";
            if (QuantumBox.Focused)
                return "Quantum";
            if (OpenTimingsBox.Focused)
                return "OpenTimings";
            if (OpenBookmarksBox.Focused)
                return "OpenBookmarks";
            if (StoreNodesBox.Focused)
                return "StoreNodes";
            if (DrawBezierBox.Focused)
                return "DrawBezier";

            if (TLBox.Focused)
                return "TL";
            if (TCBox.Focused)
                return "TC";
            if (TRBox.Focused)
                return "TR";
            if (MLBox.Focused)
                return "ML";
            if (MCBox.Focused)
                return "MC";
            if (MRBox.Focused)
                return "MR";
            if (BLBox.Focused)
                return "BL";
            if (BCBox.Focused)
                return "BC";
            if (BRBox.Focused)
                return "BR";

            return "";
        }

        private EditorSettings.KeyType AssignKey(Key key)
        {
            var keyf = new EditorSettings.KeyType();
            keyf.Key = key;
            keyf.CTRL = EditorWindow.Instance._controlDown;
            keyf.SHIFT = EditorWindow.Instance._shiftDown;
            keyf.ALT = EditorWindow.Instance._altDown;
            return keyf;
        }

        private EditorSettings.KeyType ResetKey(Key key, bool ctrl, bool shift, bool alt)
        {
            var keyf = new EditorSettings.KeyType();
            keyf.Key = key;
            keyf.CTRL = ctrl;
            keyf.SHIFT = shift;
            keyf.ALT = alt;
            return keyf;
        }

        public override void OnKeyDown(Key keyf, bool control)
        {
            var key = keyf;

            if (keyf == Key.BackSpace)
                key = Key.Delete;

            if (keyf == Key.LControl || keyf == Key.RControl || keyf == Key.LShift || keyf == Key.RShift || keyf == Key.LAlt || keyf == Key.RAlt)
                return;

            switch (GetFocused())
            {
                case "SelectAll":
                    SelectAllBox.Text = key.ToString().ToUpper();
                    EditorSettings.SelectAll = AssignKey(key);
                    break;
                case "Save":
                    SaveBox.Text = key.ToString().ToUpper();
                    EditorSettings.Save = AssignKey(key);
                    break;
                case "SaveAs":
                    SaveAsBox.Text = key.ToString().ToUpper();
                    EditorSettings.SaveAs = AssignKey(key);
                    break;
                case "Undo":
                    UndoBox.Text = key.ToString().ToUpper();
                    EditorSettings.Undo = AssignKey(key);
                    break;
                case "Redo":
                    RedoBox.Text = key.ToString().ToUpper();
                    EditorSettings.Redo = AssignKey(key);
                    break;
                case "Copy":
                    CopyBox.Text = key.ToString().ToUpper();
                    EditorSettings.Copy = AssignKey(key);
                    break;
                case "Paste":
                    PasteBox.Text = key.ToString().ToUpper();
                    EditorSettings.Paste = AssignKey(key);
                    break;
                case "Delete":
                    DeleteBox.Text = key.ToString().ToUpper();
                    EditorSettings.Delete = AssignKey(key);
                    break;
                case "HFlip":
                    HFlipBox.Text = key.ToString().ToUpper();
                    EditorSettings.HFlip = AssignKey(key);
                    break;
                case "VFlip":
                    VFlipBox.Text = key.ToString().ToUpper();
                    EditorSettings.VFlip = AssignKey(key);
                    break;
                case "SwitchClickTool":
                    SwitchClickToolBox.Text = key.ToString().ToUpper();
                    EditorSettings.SwitchClickTool = AssignKey(key);
                    break;
                case "Quantum":
                    QuantumBox.Text = key.ToString().ToUpper();
                    EditorSettings.Quantum = AssignKey(key);
                    break;
                case "OpenTimings":
                    OpenTimingsBox.Text = key.ToString().ToUpper();
                    EditorSettings.OpenTimings = AssignKey(key);
                    break;
                case "OpenBookmarks":
                    OpenBookmarksBox.Text = key.ToString().ToUpper();
                    EditorSettings.OpenBookmarks = AssignKey(key);
                    break;
                case "StoreNodes":
                    StoreNodesBox.Text = key.ToString().ToUpper();
                    EditorSettings.StoreNodes = AssignKey(key);
                    break;
                case "DrawBezier":
                    DrawBezierBox.Text = key.ToString().ToUpper();
                    EditorSettings.DrawBezier = AssignKey(key);
                    break;
                case "TL":
                    TLBox.Text = key.ToString().ToUpper();
                    EditorSettings.GridKeys.TL = key;
                    break;
                case "TC":
                    TCBox.Text = key.ToString().ToUpper();
                    EditorSettings.GridKeys.TC = key;
                    break;
                case "TR":
                    TRBox.Text = key.ToString().ToUpper();
                    EditorSettings.GridKeys.TR = key;
                    break;
                case "ML":
                    MLBox.Text = key.ToString().ToUpper();
                    EditorSettings.GridKeys.ML = key;
                    break;
                case "MC":
                    MCBox.Text = key.ToString().ToUpper();
                    EditorSettings.GridKeys.MC = key;
                    break;
                case "MR":
                    MRBox.Text = key.ToString().ToUpper();
                    EditorSettings.GridKeys.MR = key;
                    break;
                case "BL":
                    BLBox.Text = key.ToString().ToUpper();
                    EditorSettings.GridKeys.BL = key;
                    break;
                case "BC":
                    BCBox.Text = key.ToString().ToUpper();
                    EditorSettings.GridKeys.BC = key;
                    break;
                case "BR":
                    BRBox.Text = key.ToString().ToUpper();
                    EditorSettings.GridKeys.BR = key;
                    break;
            }
        }

        public override void OnMouseClick(float x, float y)
        {
            SelectAllBox.OnMouseClick(x, y);
            SaveBox.OnMouseClick(x, y);
            SaveAsBox.OnMouseClick(x, y);
            UndoBox.OnMouseClick(x, y);
            RedoBox.OnMouseClick(x, y);
            CopyBox.OnMouseClick(x, y);
            PasteBox.OnMouseClick(x, y);
            DeleteBox.OnMouseClick(x, y);
            HFlipBox.OnMouseClick(x, y);
            VFlipBox.OnMouseClick(x, y);
            SwitchClickToolBox.OnMouseClick(x, y);
            QuantumBox.OnMouseClick(x, y);
            OpenTimingsBox.OnMouseClick(x, y);
            OpenBookmarksBox.OnMouseClick(x, y);
            StoreNodesBox.OnMouseClick(x, y);
            DrawBezierBox.OnMouseClick(x, y);

            TLBox.OnMouseClick(x, y);
            TCBox.OnMouseClick(x, y);
            TRBox.OnMouseClick(x, y);
            MLBox.OnMouseClick(x, y);
            MCBox.OnMouseClick(x, y);
            MRBox.OnMouseClick(x, y);
            BLBox.OnMouseClick(x, y);
            BCBox.OnMouseClick(x, y);
            BRBox.OnMouseClick(x, y);

            base.OnMouseClick(x, y);
        }

        protected override void OnButtonClicked(int id)
        {
            switch (id)
            {
                case 0:
                    EditorWindow.Instance.OpenGuiScreen(new GuiScreenSettings());
                    break;
                case 1:
                    EditorSettings.SelectAll = ResetKey(Key.A, true, false, false);
                    SelectAllBox.Text = "A";
                    break;
                case 2:
                    EditorSettings.Save = ResetKey(Key.S, true, false, false);
                    SaveBox.Text = "S";
                    break;
                case 3:
                    EditorSettings.SaveAs = ResetKey(Key.S, true, true, false);
                    SaveAsBox.Text = "S";
                    break;
                case 4:
                    EditorSettings.Undo = ResetKey(Key.Z, true, false, false);
                    UndoBox.Text = "Z";
                    break;
                case 5:
                    EditorSettings.Redo = ResetKey(Key.Y, true, false, false);
                    RedoBox.Text = "Y";
                    break;
                case 6:
                    EditorSettings.Copy = ResetKey(Key.C, true, false, false);
                    CopyBox.Text = "C";
                    break;
                case 7:
                    EditorSettings.Paste = ResetKey(Key.V, true, false, false);
                    PasteBox.Text = "V";
                    break;
                case 8:
                    EditorSettings.Delete = ResetKey(Key.Delete, false, false, false);
                    DeleteBox.Text = "DELETE";
                    break;
                case 9:
                    EditorSettings.HFlip = ResetKey(Key.H, false, true, false);
                    HFlipBox.Text = "H";
                    break;
                case 10:
                    EditorSettings.VFlip = ResetKey(Key.V, false, true, false);
                    VFlipBox.Text = "V";
                    break;
                case 11:
                    EditorSettings.SwitchClickTool = ResetKey(Key.Tab, false, false, false);
                    SwitchClickToolBox.Text = "TAB";
                    break;
                case 12:
                    EditorSettings.Quantum = ResetKey(Key.Q, true, false, false);
                    QuantumBox.Text = "Q";
                    break;
                case 13:
                    EditorSettings.OpenTimings = ResetKey(Key.T, true, false, false);
                    OpenTimingsBox.Text = "T";
                    break;
                case 14:
                    EditorSettings.OpenBookmarks = ResetKey(Key.B, true, false, false);
                    OpenBookmarksBox.Text = "B";
                    break;
                case 15:
                    EditorSettings.StoreNodes = ResetKey(Key.S, false, true, false);
                    StoreNodesBox.Text = "S";
                    break;
                case 16:
                    EditorSettings.DrawBezier = ResetKey(Key.D, false, true, false);
                    DrawBezierBox.Text = "D";
                    break;
                case 90:
                    EditorSettings.GridKeys.TL = Key.Q;
                    TLBox.Text = "Q";
                    break;
                case 91:
                    EditorSettings.GridKeys.TC = Key.W;
                    TCBox.Text = "W";
                    break;
                case 92:
                    EditorSettings.GridKeys.TR = Key.E;
                    TRBox.Text = "E";
                    break;
                case 93:
                    EditorSettings.GridKeys.ML = Key.A;
                    MLBox.Text = "A";
                    break;
                case 94:
                    EditorSettings.GridKeys.MC = Key.S;
                    MCBox.Text = "S";
                    break;
                case 95:
                    EditorSettings.GridKeys.MR = Key.D;
                    MRBox.Text = "D";
                    break;
                case 96:
                    EditorSettings.GridKeys.BL = Key.Z;
                    BLBox.Text = "Z";
                    break;
                case 97:
                    EditorSettings.GridKeys.BC = Key.X;
                    BCBox.Text = "X";
                    break;
                case 98:
                    EditorSettings.GridKeys.BR = Key.C;
                    BRBox.Text = "C";
                    break;
            }
            base.OnButtonClicked(id);
        }
    }
}
