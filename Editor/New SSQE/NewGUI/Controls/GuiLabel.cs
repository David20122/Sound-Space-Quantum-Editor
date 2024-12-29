using New_SSQE.Preferences;
using System.Drawing;

namespace New_SSQE.NewGUI.Controls
{
    internal class GuiLabel : TextControl
    {
        public GuiLabel(float x, float y, float w, float h, Setting<Color>? color = null, string text = "", int textSize = 0, string font = "main", bool centered = true) : base(x, y, w, h, text, textSize, font, centered)
        {
            textColor = color?.Value ?? Color.White;
        }

        public override float[] Draw()
        {
            return [];
        }

        public void SetColor(Color color)
        {
            textColor = color;
            Update();
        }
    }
}
