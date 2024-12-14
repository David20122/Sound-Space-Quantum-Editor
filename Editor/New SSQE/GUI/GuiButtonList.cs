using System.Drawing;
using New_SSQE.Preferences;

namespace New_SSQE.GUI
{
    internal class GuiButtonList : GuiButton
    {
        private readonly Setting<ListSetting> Setting;
        private readonly string Prefix;

        public GuiButtonList(float x, float y, float w, float h, Setting<ListSetting> setting, int textSize, bool lockSize = false, bool moveWithOffset = false, string font = "main", int id = -1, string prefix = "") : base(x, y, w, h, id, "", textSize, lockSize, moveWithOffset, font)
        {
            Setting = setting;
            Text = prefix + Setting.Value.Current.ToString().ToUpper();

            Prefix = prefix;
        }

        public GuiButtonList(float x, float y, float w, float h, Setting<ListSetting> setting, int textSize, int id = -1, string prefix = "") : this(x, y, w, h, setting, textSize, false, false, "main", id, prefix) { }
        public GuiButtonList(Setting<ListSetting> setting, int textSize, int id = -1, string prefix = "") : this(0, 0, 0, 0, setting, textSize, false, false, "main", id, prefix) { }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            ListSetting list = Setting.Value;
            string[] possible = list.Possible;

            int index = Array.IndexOf(possible, list.Current);
            index = index >= 0 ? index : possible.Length - 1;

            int newIndex = right ? (index - 1 >= 0 ? index - 1 : possible.Length - 1) : (index + 1) % possible.Length;

            list.Current = possible[newIndex];
            Text = Prefix + list.Current.ToString().ToUpper();

            Update();

            base.OnMouseClick(pos, right);
        }
    }
}
