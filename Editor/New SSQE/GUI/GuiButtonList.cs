using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiButtonList : GuiButton
    {
        private readonly string Setting;

        public GuiButtonList(float x, float y, float w, float h, string setting, int textSize, bool lockSize = false, bool moveWithOffset = false, string font = "main") : base(x, y, w, h, -1, "", textSize, lockSize, moveWithOffset, font)
        {
            Setting = setting;
            Text = Settings.settings[Setting].Current.ToString().ToUpper();
        }

        public GuiButtonList(float x, float y, float w, float h, string setting, int textSize) : this(x, y, w, h, setting, textSize, false, false, "main") { }
        public GuiButtonList(string setting, int textSize) : this(0, 0, 0, 0, setting, textSize, false, false, "main") { }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            var setting = Settings.settings[Setting];
            var possible = setting.Possible;

            var index = Array.IndexOf(possible, setting.Current);
            index = index >= 0 ? index : possible.Length - 1;

            setting.Current = possible[(index + 1) % possible.Length];
            Text = setting.Current.ToString().ToUpper();

            Update();

            base.OnMouseClick(pos, right);
        }
    }
}
