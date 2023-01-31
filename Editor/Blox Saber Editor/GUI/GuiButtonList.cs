using System.Drawing;

using System;

namespace Sound_Space_Editor.GUI
{
    class GuiButtonList : GuiButton
    {
        private string setting;

        public GuiButtonList(float posx, float posy, float sizex, float sizey, string Setting, int TextSize, bool LockSize = false, bool MoveWithOffset = false, string Font = "main") : base(posx, posy, sizex, sizey, -1, "", TextSize, LockSize, MoveWithOffset, Font)
        {
            setting = Setting;
            text = Settings.settings[setting].Current.ToString().ToUpper();
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            var Setting = Settings.settings[setting];
            var possible = Setting.Possible;

            var index = possible.IndexOf(Setting.Current);
            index = index >= 0 ? index : possible.Count - 1;
            
            Setting.Current = possible[(index + 1) % possible.Count];
            text = Setting.Current.ToString().ToUpper();

            base.OnMouseClick(pos, right);
        }
    }
}
