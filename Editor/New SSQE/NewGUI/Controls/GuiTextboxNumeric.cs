using New_SSQE.GUI.Input;
using New_SSQE.Preferences;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Globalization;

namespace New_SSQE.NewGUI.Controls
{
    internal class GuiTextboxNumeric : GuiTextbox
    {
        private Setting<float>? setting;
        private bool isFloat;
        private bool isPositive;

        public GuiTextboxNumeric(float x, float y, float w, float h, Setting<float>? setting = null, bool isFloat = false, bool isPositive = false, string text = "0", int textSize = 0, string font = "main", bool centered = true) : base(x, y, w, h, null, text, textSize, font, centered)
        {
            this.setting = setting;
            this.isFloat = isFloat;
            this.isPositive = isPositive;

            if (setting != null)
                SetText(setting.Value.ToString());
        }

        public override void KeyDown(Keys key)
        {
            bool ctrl = MainWindow.Instance.CtrlHeld;
            bool shift = MainWindow.Instance.ShiftHeld;
            
            switch (key)
            {
                case Keys.C when ctrl:
                case Keys.V when ctrl:
                case Keys.X when ctrl:
                case Keys.Left:
                case Keys.Right:
                case Keys.Backspace:
                case Keys.Delete:
                case Keys.Enter:
                case Keys.KeyPadEnter:
                case Keys.Escape:
                    base.KeyDown(key);
                    break;

                default:
                    if (ctrl)
                        break;
                    string str = KeyConverter.GetCharFromInput(key, shift).ToString();
                    string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    
                    if (int.TryParse(str, out _) || (str == sep && !text.Contains(sep)) || (str == "-" && !text.Contains('-') && cursorPos == 0))
                    {
                        SetText(text.Insert(cursorPos, str));
                        cursorPos++;
                    }

                    break;
            }

            cursorPos = MathHelper.Clamp(cursorPos, 0, text.Length);
            Update();

            float numFloat = 0;
            int numInt = 0;

            if (setting != null && (isFloat ? float.TryParse(text, out numFloat) : int.TryParse(text, out numInt)))
            {
                float value = numFloat + numInt;
                if (!isPositive || value > 0)
                    setting.Value = value;
            }
        }
    }
}
