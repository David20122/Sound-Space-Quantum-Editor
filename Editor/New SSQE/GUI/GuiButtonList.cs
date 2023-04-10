using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiButtonList : GuiButton
    {
        private readonly string Setting;

        public GuiButtonList(float posx, float posy, float sizex, float sizey, string setting, int textSize, bool lockSize = false, bool moveWithOffset = false, string font = "main") : base(posx, posy, sizex, sizey, -1, "", textSize, lockSize, moveWithOffset, font)
        {
            Setting = setting;
            Text = Settings.settings[Setting].Current.ToString().ToUpper();
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            var setting = Settings.settings[Setting];
            var possible = setting.Possible;

            var index = possible.IndexOf(setting.Current);
            index = index >= 0 ? index : possible.Count - 1;

            setting.Current = possible[(index + 1) % possible.Count];
            Text = setting.Current.ToString().ToUpper();

            Update();

            base.OnMouseClick(pos, right);
        }
    }
}
