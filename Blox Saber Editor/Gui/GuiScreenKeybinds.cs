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
        private readonly GuiCheckBox SelectAllCTRL = new GuiCheckBox(10, "CTRL", 0, 0, 0, 0, EditorSettings.SelectAll.CTRL);
        private readonly GuiCheckBox SelectAllSHIFT = new GuiCheckBox(11, "SHIFT", 0, 0, 0, 0, EditorSettings.SelectAll.SHIFT);
        private readonly GuiCheckBox SelectAllALT = new GuiCheckBox(12, "ALT", 0, 0, 0, 0, EditorSettings.SelectAll.ALT);
        private readonly GuiButton SelectAllReset = new GuiButton(13, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox SaveBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Save.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiCheckBox SaveCTRL = new GuiCheckBox(20, "CTRL", 0, 0, 0, 0, EditorSettings.Save.CTRL);
        private readonly GuiCheckBox SaveSHIFT = new GuiCheckBox(21, "SHIFT", 0, 0, 0, 0, EditorSettings.Save.SHIFT);
        private readonly GuiCheckBox SaveALT = new GuiCheckBox(22, "ALT", 0, 0, 0, 0, EditorSettings.Save.ALT);
        private readonly GuiButton SaveReset = new GuiButton(23, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox SaveAsBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.SaveAs.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiCheckBox SaveAsCTRL = new GuiCheckBox(30, "CTRL", 0, 0, 0, 0, EditorSettings.SaveAs.CTRL);
        private readonly GuiCheckBox SaveAsSHIFT = new GuiCheckBox(31, "SHIFT", 0, 0, 0, 0, EditorSettings.SaveAs.SHIFT);
        private readonly GuiCheckBox SaveAsALT = new GuiCheckBox(32, "ALT", 0, 0, 0, 0, EditorSettings.SaveAs.ALT);
        private readonly GuiButton SaveAsReset = new GuiButton(33, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox UndoBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Undo.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiCheckBox UndoCTRL = new GuiCheckBox(40, "CTRL", 0, 0, 0, 0, EditorSettings.Undo.CTRL);
        private readonly GuiCheckBox UndoSHIFT = new GuiCheckBox(41, "SHIFT", 0, 0, 0, 0, EditorSettings.Undo.SHIFT);
        private readonly GuiCheckBox UndoALT = new GuiCheckBox(42, "ALT", 0, 0, 0, 0, EditorSettings.Undo.ALT);
        private readonly GuiButton UndoReset = new GuiButton(43, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox RedoBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Redo.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiCheckBox RedoCTRL = new GuiCheckBox(50, "CTRL", 0, 0, 0, 0, EditorSettings.Redo.CTRL);
        private readonly GuiCheckBox RedoSHIFT = new GuiCheckBox(51, "SHIFT", 0, 0, 0, 0, EditorSettings.Redo.SHIFT);
        private readonly GuiCheckBox RedoALT = new GuiCheckBox(52, "ALT", 0, 0, 0, 0, EditorSettings.Redo.ALT);
        private readonly GuiButton RedoReset = new GuiButton(53, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox CopyBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Copy.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiCheckBox CopyCTRL = new GuiCheckBox(60, "CTRL", 0, 0, 0, 0, EditorSettings.Copy.CTRL);
        private readonly GuiCheckBox CopySHIFT = new GuiCheckBox(61, "SHIFT", 0, 0, 0, 0, EditorSettings.Copy.SHIFT);
        private readonly GuiCheckBox CopyALT = new GuiCheckBox(62, "ALT", 0, 0, 0, 0, EditorSettings.Copy.ALT);
        private readonly GuiButton CopyReset = new GuiButton(63, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox PasteBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.Paste.Key.ToString().ToUpper(), Centered = true };
        private readonly GuiCheckBox PasteCTRL = new GuiCheckBox(70, "CTRL", 0, 0, 0, 0, EditorSettings.Paste.CTRL);
        private readonly GuiCheckBox PasteSHIFT = new GuiCheckBox(71, "SHIFT", 0, 0, 0, 0, EditorSettings.Paste.SHIFT);
        private readonly GuiCheckBox PasteALT = new GuiCheckBox(72, "ALT", 0, 0, 0, 0, EditorSettings.Paste.ALT);
        private readonly GuiButton PasteReset = new GuiButton(73, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox TLBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.TL.ToString().ToUpper(), Centered = true };
        private readonly GuiButton TLReset = new GuiButton(80, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox TCBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.TC.ToString().ToUpper(), Centered = true };
        private readonly GuiButton TCReset = new GuiButton(81, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox TRBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.TR.ToString().ToUpper(), Centered = true };
        private readonly GuiButton TRReset = new GuiButton(82, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox MLBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.ML.ToString().ToUpper(), Centered = true };
        private readonly GuiButton MLReset = new GuiButton(83, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox MCBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.MC.ToString().ToUpper(), Centered = true };
        private readonly GuiButton MCReset = new GuiButton(84, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox MRBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.MR.ToString().ToUpper(), Centered = true };
        private readonly GuiButton MRReset = new GuiButton(85, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox BLBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.BL.ToString().ToUpper(), Centered = true };
        private readonly GuiButton BLReset = new GuiButton(86, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox BCBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.BC.ToString().ToUpper(), Centered = true };
        private readonly GuiButton BCReset = new GuiButton(87, 0, 0, 0, 0, "RESET", false);

        private readonly GuiTextBox BRBox = new GuiTextBox(0, 0, 0, 0) { Text = EditorSettings.GridKeys.BR.ToString().ToUpper(), Centered = true };
        private readonly GuiButton BRReset = new GuiButton(88, 0, 0, 0, 0, "RESET", false);


        private readonly GuiButton _backButton = new GuiButton(0, 0, 0, 600, 100, "RETURN TO SETTINGS", "square", 100);

        public GuiScreenKeybinds() : base(0, 0, EditorWindow.Instance.ClientSize.Width, EditorWindow.Instance.ClientSize.Height)
        {
            Buttons.Add(SelectAllCTRL);
            Buttons.Add(SelectAllSHIFT);
            Buttons.Add(SelectAllALT);
            Buttons.Add(SelectAllReset);

            Buttons.Add(SaveCTRL);
            Buttons.Add(SaveSHIFT);
            Buttons.Add(SaveALT);
            Buttons.Add(SaveReset);

            Buttons.Add(SaveAsCTRL);
            Buttons.Add(SaveAsSHIFT);
            Buttons.Add(SaveAsALT);
            Buttons.Add(SaveAsReset);

            Buttons.Add(UndoCTRL);
            Buttons.Add(UndoSHIFT);
            Buttons.Add(UndoALT);
            Buttons.Add(UndoReset);

            Buttons.Add(RedoCTRL);
            Buttons.Add(RedoSHIFT);
            Buttons.Add(RedoALT);
            Buttons.Add(RedoReset);

            Buttons.Add(CopyCTRL);
            Buttons.Add(CopySHIFT);
            Buttons.Add(CopyALT);
            Buttons.Add(CopyReset);

            Buttons.Add(PasteCTRL);
            Buttons.Add(PasteSHIFT);
            Buttons.Add(PasteALT);
            Buttons.Add(PasteReset);

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
            fr.Render("Save", (int)SaveBox.ClientRectangle.X, (int)SaveBox.ClientRectangle.Y - 26, 24);
            fr.Render("Save As", (int)SaveAsBox.ClientRectangle.X, (int)SaveAsBox.ClientRectangle.Y - 26, 24);
            fr.Render("Undo", (int)UndoBox.ClientRectangle.X, (int)UndoBox.ClientRectangle.Y - 26, 24);
            fr.Render("Redo", (int)RedoBox.ClientRectangle.X, (int)RedoBox.ClientRectangle.Y - 26, 24);
            fr.Render("Copy", (int)CopyBox.ClientRectangle.X, (int)CopyBox.ClientRectangle.Y - 26, 24);
            fr.Render("Paste", (int)PasteBox.ClientRectangle.X, (int)PasteBox.ClientRectangle.Y - 26, 24);

            fr.Render("Grid", (int)TLBox.ClientRectangle.X, (int)TLBox.ClientRectangle.Y - 26, 24);

            SelectAllBox.Render(delta, mouseX, mouseY);
            SaveBox.Render(delta, mouseX, mouseY);
            SaveAsBox.Render(delta, mouseX, mouseY);
            UndoBox.Render(delta, mouseX, mouseY);
            RedoBox.Render(delta, mouseX, mouseY);
            CopyBox.Render(delta, mouseX, mouseY);
            PasteBox.Render(delta, mouseX, mouseY);

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

        public override void OnResize(Size size)
        {
            ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);
            var widthdiff = size.Width / 1920f;
            var heightdiff = size.Height / 1080f;

            SelectAllBox.ClientRectangle.Size = new SizeF(128 * widthdiff, 64 * heightdiff);
            SelectAllCTRL.ClientRectangle.Size = new SizeF(64 * widthdiff, 64 * heightdiff);
            SelectAllSHIFT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            SelectAllALT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            SelectAllReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            SaveBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            SaveCTRL.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            SaveSHIFT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            SaveALT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            SaveReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            SaveAsBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            SaveAsCTRL.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            SaveAsSHIFT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            SaveAsALT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            SaveAsReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            UndoBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            UndoCTRL.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            UndoSHIFT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            UndoALT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            UndoReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            RedoBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            RedoCTRL.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            RedoSHIFT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            RedoALT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            RedoReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            CopyBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            CopyCTRL.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            CopySHIFT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            CopyALT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            CopyReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

            PasteBox.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;
            PasteCTRL.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            PasteSHIFT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            PasteALT.ClientRectangle.Size = SelectAllCTRL.ClientRectangle.Size;
            PasteReset.ClientRectangle.Size = SelectAllBox.ClientRectangle.Size;

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
            SelectAllCTRL.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.Right + 10 * widthdiff, SelectAllBox.ClientRectangle.Y);
            SelectAllSHIFT.ClientRectangle.Location = new PointF(SelectAllCTRL.ClientRectangle.Right + 110 * widthdiff, SelectAllBox.ClientRectangle.Y);
            SelectAllALT.ClientRectangle.Location = new PointF(SelectAllSHIFT.ClientRectangle.Right + 110 * widthdiff, SelectAllBox.ClientRectangle.Y);
            SelectAllReset.ClientRectangle.Location = new PointF(SelectAllALT.ClientRectangle.Right + 110 * widthdiff, SelectAllBox.ClientRectangle.Y);

            SaveBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, SelectAllBox.ClientRectangle.Bottom + 40 * heightdiff);
            SaveCTRL.ClientRectangle.Location = new PointF(SelectAllCTRL.ClientRectangle.X, SaveBox.ClientRectangle.Y);
            SaveSHIFT.ClientRectangle.Location = new PointF(SelectAllSHIFT.ClientRectangle.X, SaveBox.ClientRectangle.Y);
            SaveALT.ClientRectangle.Location = new PointF(SelectAllALT.ClientRectangle.X, SaveBox.ClientRectangle.Y);
            SaveReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, SaveBox.ClientRectangle.Y);

            SaveAsBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, SaveBox.ClientRectangle.Bottom + 40 * heightdiff);
            SaveAsCTRL.ClientRectangle.Location = new PointF(SelectAllCTRL.ClientRectangle.X, SaveAsBox.ClientRectangle.Y);
            SaveAsSHIFT.ClientRectangle.Location = new PointF(SelectAllSHIFT.ClientRectangle.X, SaveAsBox.ClientRectangle.Y);
            SaveAsALT.ClientRectangle.Location = new PointF(SelectAllALT.ClientRectangle.X, SaveAsBox.ClientRectangle.Y);
            SaveAsReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, SaveAsBox.ClientRectangle.Y);

            UndoBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, SaveAsBox.ClientRectangle.Bottom + 40 * heightdiff);
            UndoCTRL.ClientRectangle.Location = new PointF(SelectAllCTRL.ClientRectangle.X, UndoBox.ClientRectangle.Y);
            UndoSHIFT.ClientRectangle.Location = new PointF(SelectAllSHIFT.ClientRectangle.X, UndoBox.ClientRectangle.Y);
            UndoALT.ClientRectangle.Location = new PointF(SelectAllALT.ClientRectangle.X, UndoBox.ClientRectangle.Y);
            UndoReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, UndoBox.ClientRectangle.Y);

            RedoBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, UndoBox.ClientRectangle.Bottom + 40 * heightdiff);
            RedoCTRL.ClientRectangle.Location = new PointF(SelectAllCTRL.ClientRectangle.X, RedoBox.ClientRectangle.Y);
            RedoSHIFT.ClientRectangle.Location = new PointF(SelectAllSHIFT.ClientRectangle.X, RedoBox.ClientRectangle.Y);
            RedoALT.ClientRectangle.Location = new PointF(SelectAllALT.ClientRectangle.X, RedoBox.ClientRectangle.Y);
            RedoReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, RedoBox.ClientRectangle.Y);

            CopyBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, RedoBox.ClientRectangle.Bottom + 30 * heightdiff);
            CopyCTRL.ClientRectangle.Location = new PointF(SelectAllCTRL.ClientRectangle.X, CopyBox.ClientRectangle.Y);
            CopySHIFT.ClientRectangle.Location = new PointF(SelectAllSHIFT.ClientRectangle.X, CopyBox.ClientRectangle.Y);
            CopyALT.ClientRectangle.Location = new PointF(SelectAllALT.ClientRectangle.X, CopyBox.ClientRectangle.Y);
            CopyReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, CopyBox.ClientRectangle.Y);

            PasteBox.ClientRectangle.Location = new PointF(SelectAllBox.ClientRectangle.X, CopyBox.ClientRectangle.Bottom + 30 * heightdiff);
            PasteCTRL.ClientRectangle.Location = new PointF(SelectAllCTRL.ClientRectangle.X, PasteBox.ClientRectangle.Y);
            PasteSHIFT.ClientRectangle.Location = new PointF(SelectAllSHIFT.ClientRectangle.X, PasteBox.ClientRectangle.Y);
            PasteALT.ClientRectangle.Location = new PointF(SelectAllALT.ClientRectangle.X, PasteBox.ClientRectangle.Y);
            PasteReset.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.X, PasteBox.ClientRectangle.Y);

            TLBox.ClientRectangle.Location = new PointF(SelectAllReset.ClientRectangle.Right + 110 * widthdiff, SelectAllBox.ClientRectangle.Y);
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

        public override void OnKeyDown(Key key, bool control)
        {
            switch (GetFocused())
            {
                case "SelectAll":
                    SelectAllBox.Text = key.ToString().ToUpper();
                    EditorSettings.SelectAll.Key = key;
                    break;
                case "Save":
                    SaveBox.Text = key.ToString().ToUpper();
                    EditorSettings.Save.Key = key;
                    break;
                case "SaveAs":
                    SaveAsBox.Text = key.ToString().ToUpper();
                    EditorSettings.SaveAs.Key = key;
                    break;
                case "Undo":
                    UndoBox.Text = key.ToString().ToUpper();
                    EditorSettings.Undo.Key = key;
                    break;
                case "Redo":
                    RedoBox.Text = key.ToString().ToUpper();
                    EditorSettings.Redo.Key = key;
                    break;
                case "Copy":
                    CopyBox.Text = key.ToString().ToUpper();
                    EditorSettings.Copy.Key = key;
                    break;
                case "Paste":
                    PasteBox.Text = key.ToString().ToUpper();
                    EditorSettings.Paste.Key = key;
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
                case 10:
                    EditorSettings.SelectAll.CTRL = SelectAllCTRL.Toggle;
                    break;
                case 11:
                    EditorSettings.SelectAll.SHIFT = SelectAllSHIFT.Toggle;
                    break;
                case 12:
                    EditorSettings.SelectAll.ALT = SelectAllALT.Toggle;
                    break;
                case 13:
                    EditorSettings.SelectAll.Key = Key.A;
                    EditorSettings.SelectAll.CTRL = true;
                    EditorSettings.SelectAll.SHIFT = false;
                    EditorSettings.SelectAll.ALT = false;
                    SelectAllBox.Text = "A";
                    SelectAllCTRL.Toggle = true;
                    SelectAllSHIFT.Toggle = false;
                    SelectAllALT.Toggle = false;
                    break;
                case 20:
                    EditorSettings.Save.CTRL = SaveCTRL.Toggle;
                    break;
                case 21:
                    EditorSettings.Save.SHIFT = SaveSHIFT.Toggle;
                    break;
                case 22:
                    EditorSettings.Save.ALT = SaveALT.Toggle;
                    break;
                case 23:
                    EditorSettings.Save.Key = Key.S;
                    EditorSettings.Save.CTRL = true;
                    EditorSettings.Save.SHIFT = false;
                    EditorSettings.Save.ALT = false;
                    SaveBox.Text = "S";
                    SaveCTRL.Toggle = true;
                    SaveSHIFT.Toggle = false;
                    SaveALT.Toggle = false;
                    break;
                case 30:
                    EditorSettings.SaveAs.CTRL = SaveAsCTRL.Toggle;
                    break;
                case 31:
                    EditorSettings.SaveAs.SHIFT = SaveAsSHIFT.Toggle;
                    break;
                case 32:
                    EditorSettings.SaveAs.ALT = SaveAsALT.Toggle;
                    break;
                case 33:
                    EditorSettings.SaveAs.Key = Key.S;
                    EditorSettings.SaveAs.CTRL = true;
                    EditorSettings.SaveAs.SHIFT = true;
                    EditorSettings.SaveAs.ALT = false;
                    SaveAsBox.Text = "S";
                    SaveAsCTRL.Toggle = true;
                    SaveAsSHIFT.Toggle = true;
                    SaveAsALT.Toggle = false;
                    break;
                case 40:
                    EditorSettings.Undo.CTRL = UndoCTRL.Toggle;
                    break;
                case 41:
                    EditorSettings.Undo.SHIFT = UndoSHIFT.Toggle;
                    break;
                case 42:
                    EditorSettings.Undo.ALT = UndoALT.Toggle;
                    break;
                case 43:
                    EditorSettings.Undo.Key = Key.Z;
                    EditorSettings.Undo.CTRL = true;
                    EditorSettings.Undo.SHIFT = false;
                    EditorSettings.Undo.ALT = false;
                    UndoBox.Text = "Z";
                    UndoCTRL.Toggle = true;
                    UndoSHIFT.Toggle = false;
                    UndoALT.Toggle = false;
                    break;
                case 50:
                    EditorSettings.Redo.CTRL = RedoCTRL.Toggle;
                    break;
                case 51:
                    EditorSettings.Redo.SHIFT = RedoSHIFT.Toggle;
                    break;
                case 52:
                    EditorSettings.Redo.ALT = RedoALT.Toggle;
                    break;
                case 53:
                    EditorSettings.Redo.Key = Key.Y;
                    EditorSettings.Redo.CTRL = true;
                    EditorSettings.Redo.SHIFT = false;
                    EditorSettings.Redo.ALT = false;
                    RedoBox.Text = "Y";
                    RedoCTRL.Toggle = true;
                    RedoSHIFT.Toggle = false;
                    RedoALT.Toggle = false;
                    break;
                case 60:
                    EditorSettings.Copy.CTRL = CopyCTRL.Toggle;
                    break;
                case 61:
                    EditorSettings.Copy.SHIFT = CopySHIFT.Toggle;
                    break;
                case 62:
                    EditorSettings.Copy.ALT = CopyALT.Toggle;
                    break;
                case 63:
                    EditorSettings.Copy.Key = Key.C;
                    EditorSettings.Copy.CTRL = true;
                    EditorSettings.Copy.SHIFT = false;
                    EditorSettings.Copy.ALT = false;
                    CopyBox.Text = "C";
                    CopyCTRL.Toggle = true;
                    CopySHIFT.Toggle = false;
                    CopyALT.Toggle = false;
                    break;
                case 70:
                    EditorSettings.Paste.CTRL = PasteCTRL.Toggle;
                    break;
                case 71:
                    EditorSettings.Paste.SHIFT = PasteSHIFT.Toggle;
                    break;
                case 72:
                    EditorSettings.Paste.ALT = PasteALT.Toggle;
                    break;
                case 73:
                    EditorSettings.Paste.Key = Key.V;
                    EditorSettings.Paste.CTRL = true;
                    EditorSettings.Paste.SHIFT = false;
                    EditorSettings.Paste.ALT = false;
                    PasteBox.Text = "V";
                    PasteCTRL.Toggle = true;
                    PasteSHIFT.Toggle = false;
                    PasteALT.Toggle = false;
                    break;
                case 80:
                    EditorSettings.GridKeys.TL = Key.Q;
                    TLBox.Text = "Q";
                    break;
                case 81:
                    EditorSettings.GridKeys.TC = Key.W;
                    TCBox.Text = "W";
                    break;
                case 82:
                    EditorSettings.GridKeys.TR = Key.E;
                    TRBox.Text = "E";
                    break;
                case 83:
                    EditorSettings.GridKeys.ML = Key.A;
                    MLBox.Text = "A";
                    break;
                case 84:
                    EditorSettings.GridKeys.MC = Key.S;
                    MCBox.Text = "S";
                    break;
                case 85:
                    EditorSettings.GridKeys.MR = Key.D;
                    MRBox.Text = "D";
                    break;
                case 86:
                    EditorSettings.GridKeys.BL = Key.Z;
                    BLBox.Text = "Z";
                    break;
                case 87:
                    EditorSettings.GridKeys.BC = Key.X;
                    BCBox.Text = "X";
                    break;
                case 88:
                    EditorSettings.GridKeys.BR = Key.C;
                    BRBox.Text = "C";
                    break;
            }
            base.OnButtonClicked(id);
        }
    }
}
