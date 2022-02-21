using System;
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

            Buttons.Add(TLReset);
            Buttons.Add(TCReset);
            Buttons.Add(TRReset);
            Buttons.Add(MLReset);
            Buttons.Add(MCReset);
            Buttons.Add(MRReset);
            Buttons.Add(BLReset);
            Buttons.Add(BCReset);
            Buttons.Add(BRReset);

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

            var fr = EditorWindow.Instance.FontRenderer;

            GL.Color4(Color.FromArgb(255, 255, 255, 255));

            fr.Render("Select All", (int)SelectAllBox.ClientRectangle.X, (int)SelectAllBox.ClientRectangle.Y - 26, 24);
            fr.Render(CSAString(EditorSettings.SelectAll), (int)SelectAllReset.ClientRectangle.Right + 10, (int)SelectAllReset.ClientRectangle.Y + (int)(SelectAllReset.ClientRectangle.Height / 2) - 12, 24);

            fr.Render("Save", (int)SaveBox.ClientRectangle.X, (int)SaveBox.ClientRectangle.Y - 26, 24);
            fr.Render(CSAString(EditorSettings.Save), (int)SaveReset.ClientRectangle.Right + 10, (int)SaveReset.ClientRectangle.Y + (int)(SaveReset.ClientRectangle.Height / 2) - 12, 24);

            fr.Render("Save As", (int)SaveAsBox.ClientRectangle.X, (int)SaveAsBox.ClientRectangle.Y - 26, 24);
            fr.Render(CSAString(EditorSettings.SaveAs), (int)SaveAsReset.ClientRectangle.Right + 10, (int)SaveAsReset.ClientRectangle.Y + (int)(SaveAsReset.ClientRectangle.Height / 2) - 12, 24);

            fr.Render("Undo", (int)UndoBox.ClientRectangle.X, (int)UndoBox.ClientRectangle.Y - 26, 24);
            fr.Render(CSAString(EditorSettings.Undo), (int)UndoReset.ClientRectangle.Right + 10, (int)UndoReset.ClientRectangle.Y + (int)(UndoReset.ClientRectangle.Height / 2) - 12, 24);

            fr.Render("Redo", (int)RedoBox.ClientRectangle.X, (int)RedoBox.ClientRectangle.Y - 26, 24);
            fr.Render(CSAString(EditorSettings.Redo), (int)RedoReset.ClientRectangle.Right + 10, (int)RedoReset.ClientRectangle.Y + (int)(RedoReset.ClientRectangle.Height / 2) - 12, 24);

            fr.Render("Copy", (int)CopyBox.ClientRectangle.X, (int)CopyBox.ClientRectangle.Y - 26, 24);
            fr.Render(CSAString(EditorSettings.Copy), (int)CopyReset.ClientRectangle.Right + 10, (int)CopyReset.ClientRectangle.Y + (int)(CopyReset.ClientRectangle.Height / 2) - 12, 24);

            fr.Render("Paste", (int)PasteBox.ClientRectangle.X, (int)PasteBox.ClientRectangle.Y - 26, 24);
            fr.Render(CSAString(EditorSettings.Paste), (int)PasteReset.ClientRectangle.Right + 10, (int)PasteReset.ClientRectangle.Y + (int)(PasteReset.ClientRectangle.Height / 2) - 12, 24);

            fr.Render("Delete Note(s)", (int)DeleteBox.ClientRectangle.X, (int)DeleteBox.ClientRectangle.Y - 26, 24);
            fr.Render(CSAString(EditorSettings.Delete), (int)DeleteReset.ClientRectangle.Right + 10, (int)DeleteReset.ClientRectangle.Y + (int)(DeleteReset.ClientRectangle.Height / 2) - 12, 24);

            fr.Render("Grid", (int)TLBox.ClientRectangle.X, (int)TLBox.ClientRectangle.Y - 26, 24);

            string[] lockedlist =
            {
                "Other [LOCKED]",
                "Zoom: CTRL + SCROLL",
                "Beat Divisor: SHIFT + SCROLL",
                "Travel through timeline: SCROLL/LEFT ARROW/RIGHT ARROW",
                "Play/Pause: SPACE",
                "Fullscreen: F11",
                "Stored Patterns: 0-9",
                ">Hold SHIFT to bind a pattern to the key",
                ">Hold CTRL to clear a pattern from the key",
            };

            var lockedstring = string.Join("\n>", lockedlist);

            fr.Render(lockedstring, (int)BLReset.ClientRectangle.X, (int)BLReset.ClientRectangle.Bottom + 10, 24);

            SelectAllBox.Render(delta, mouseX, mouseY);
            SaveBox.Render(delta, mouseX, mouseY);
            SaveAsBox.Render(delta, mouseX, mouseY);
            UndoBox.Render(delta, mouseX, mouseY);
            RedoBox.Render(delta, mouseX, mouseY);
            CopyBox.Render(delta, mouseX, mouseY);
            PasteBox.Render(delta, mouseX, mouseY);
            DeleteBox.Render(delta, mouseX, mouseY);

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

            var csawidth = EditorWindow.Instance.FontRenderer.GetWidth("CTRL + SHIFT + ALT", 24);

            SelectAllBox.ClientRectangle.Size = new SizeF(128 * widthdiff, 64 * heightdiff);
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

            _backButton.ClientRectangle.Size = new SizeF(600 * widthdiff, 100 * heightdiff);


            SelectAllBox.ClientRectangle.Location = new PointF(200 * widthdiff, 100 * heightdiff);
            SelectAllReset.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.Right + 10 * widthdiff, SelectAllBox.ClientRectangle.Y);

            SaveBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, SelectAllBox.ClientRectangle.Bottom + 40 * heightdiff);
            SaveReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, SaveBox.ClientRectangle.Y);

            SaveAsBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, SaveBox.ClientRectangle.Bottom + 40 * heightdiff);
            SaveAsReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, SaveAsBox.ClientRectangle.Y);

            UndoBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, SaveAsBox.ClientRectangle.Bottom + 40 * heightdiff);
            UndoReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, UndoBox.ClientRectangle.Y);

            RedoBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, UndoBox.ClientRectangle.Bottom + 40 * heightdiff);
            RedoReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, RedoBox.ClientRectangle.Y);

            CopyBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, RedoBox.ClientRectangle.Bottom + 30 * heightdiff);
            CopyReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, CopyBox.ClientRectangle.Y);

            PasteBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, CopyBox.ClientRectangle.Bottom + 30 * heightdiff);
            PasteReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, PasteBox.ClientRectangle.Y);

            DeleteBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, PasteBox.ClientRectangle.Bottom + 30 * heightdiff);
            DeleteReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, DeleteBox.ClientRectangle.Y);

            TLBox.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.Right + csawidth + 100 * widthdiff, SelectAllBox.ClientRectangle.Y);
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
                    EditorSettings.SelectAll.Key = key;
                    EditorSettings.SelectAll.CTRL = EditorWindow.Instance._controlDown;
                    EditorSettings.SelectAll.SHIFT = EditorWindow.Instance._shiftDown;
                    EditorSettings.SelectAll.ALT = EditorWindow.Instance._altDown;
                    break;
                case "Save":
                    SaveBox.Text = key.ToString().ToUpper();
                    EditorSettings.Save.Key = key;
                    EditorSettings.Save.CTRL = EditorWindow.Instance._controlDown;
                    EditorSettings.Save.SHIFT = EditorWindow.Instance._shiftDown;
                    EditorSettings.Save.ALT = EditorWindow.Instance._altDown;
                    break;
                case "SaveAs":
                    SaveAsBox.Text = key.ToString().ToUpper();
                    EditorSettings.SaveAs.Key = key;
                    EditorSettings.SaveAs.CTRL = EditorWindow.Instance._controlDown;
                    EditorSettings.SaveAs.SHIFT = EditorWindow.Instance._shiftDown;
                    EditorSettings.SaveAs.ALT = EditorWindow.Instance._altDown;
                    break;
                case "Undo":
                    UndoBox.Text = key.ToString().ToUpper();
                    EditorSettings.Undo.Key = key;
                    EditorSettings.Undo.CTRL = EditorWindow.Instance._controlDown;
                    EditorSettings.Undo.SHIFT = EditorWindow.Instance._shiftDown;
                    EditorSettings.Undo.ALT = EditorWindow.Instance._altDown;
                    break;
                case "Redo":
                    RedoBox.Text = key.ToString().ToUpper();
                    EditorSettings.Redo.Key = key;
                    EditorSettings.Redo.CTRL = EditorWindow.Instance._controlDown;
                    EditorSettings.Redo.SHIFT = EditorWindow.Instance._shiftDown;
                    EditorSettings.Redo.ALT = EditorWindow.Instance._altDown;
                    break;
                case "Copy":
                    CopyBox.Text = key.ToString().ToUpper();
                    EditorSettings.Copy.Key = key;
                    EditorSettings.Copy.CTRL = EditorWindow.Instance._controlDown;
                    EditorSettings.Copy.SHIFT = EditorWindow.Instance._shiftDown;
                    EditorSettings.Copy.ALT = EditorWindow.Instance._altDown;
                    break;
                case "Paste":
                    PasteBox.Text = key.ToString().ToUpper();
                    EditorSettings.Paste.Key = key;
                    EditorSettings.Paste.CTRL = EditorWindow.Instance._controlDown;
                    EditorSettings.Paste.SHIFT = EditorWindow.Instance._shiftDown;
                    EditorSettings.Paste.ALT = EditorWindow.Instance._altDown;
                    break;
                case "Delete":
                    DeleteBox.Text = key.ToString().ToUpper();
                    EditorSettings.Delete.Key = key;
                    EditorSettings.Delete.CTRL = EditorWindow.Instance._controlDown;
                    EditorSettings.Delete.SHIFT = EditorWindow.Instance._shiftDown;
                    EditorSettings.Delete.ALT = EditorWindow.Instance._altDown;
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
                    EditorSettings.SelectAll.Key = Key.A;
                    EditorSettings.SelectAll.CTRL = true;
                    EditorSettings.SelectAll.SHIFT = false;
                    EditorSettings.SelectAll.ALT = false;
                    SelectAllBox.Text = "A";
                    break;
                case 2:
                    EditorSettings.Save.Key = Key.S;
                    EditorSettings.Save.CTRL = true;
                    EditorSettings.Save.SHIFT = false;
                    EditorSettings.Save.ALT = false;
                    SaveBox.Text = "S";
                    break;
                case 3:
                    EditorSettings.SaveAs.Key = Key.S;
                    EditorSettings.SaveAs.CTRL = true;
                    EditorSettings.SaveAs.SHIFT = true;
                    EditorSettings.SaveAs.ALT = false;
                    SaveAsBox.Text = "S";
                    break;
                case 4:
                    EditorSettings.Undo.Key = Key.Z;
                    EditorSettings.Undo.CTRL = true;
                    EditorSettings.Undo.SHIFT = false;
                    EditorSettings.Undo.ALT = false;
                    UndoBox.Text = "Z";
                    break;
                case 5:
                    EditorSettings.Redo.Key = Key.Y;
                    EditorSettings.Redo.CTRL = true;
                    EditorSettings.Redo.SHIFT = false;
                    EditorSettings.Redo.ALT = false;
                    RedoBox.Text = "Y";
                    break;
                case 6:
                    EditorSettings.Copy.Key = Key.C;
                    EditorSettings.Copy.CTRL = true;
                    EditorSettings.Copy.SHIFT = false;
                    EditorSettings.Copy.ALT = false;
                    CopyBox.Text = "C";
                    break;
                case 7:
                    EditorSettings.Paste.Key = Key.V;
                    EditorSettings.Paste.CTRL = true;
                    EditorSettings.Paste.SHIFT = false;
                    EditorSettings.Paste.ALT = false;
                    PasteBox.Text = "V";
                    break;
                case 8:
                    EditorSettings.Delete.Key = Key.Delete;
                    EditorSettings.Delete.CTRL = false;
                    EditorSettings.Delete.SHIFT = false;
                    EditorSettings.Delete.ALT = false;
                    DeleteBox.Text = "DELETE";
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
