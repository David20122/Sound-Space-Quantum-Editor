using New_SSQE.Preferences;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New_SSQE.NewGUI.Controls
{
    internal class GuiTextboxGridKeybind : GuiTextbox
    {
        private int gridKey;

        public GuiTextboxGridKeybind(float x, float y, float w, float h, int gridKey, string text = "", int textSize = 0, string font = "main", bool centered = true) : base(x, y, w, h, null, text, textSize, font, centered)
        {
            this.gridKey = gridKey;
        }

        public override void KeyDown(Keys key)
        {
            if (key == Keys.LeftControl || key == Keys.RightControl)
                return;
            if (key == Keys.LeftAlt || key == Keys.RightAlt)
                return;
            if (key == Keys.LeftShift || key == Keys.RightShift)
                return;

            if (key == Keys.Backspace)
                key = Keys.Delete;

            Settings.gridKeys.Value[gridKey] = key;

            SetText(key.ToString().ToUpper());
            cursorPos = text.Length;
        }
    }
}
