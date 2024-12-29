using New_SSQE.Preferences;

namespace New_SSQE.NewGUI.Controls
{
    internal class GuiButtonList : GuiButton
    {
        private Setting<ListSetting> setting;
        private string prefix;

        public GuiButtonList(float x, float y, float w, float h, Setting<ListSetting> setting, string prefix = "") : base(x, y, w, h)
        {
            this.setting = setting;
            this.prefix = prefix;

            RightResponsive = true;
        }

        private void UpdateSetting(bool right)
        {
            ListSetting list = setting.Value;
            string[] possible = list.Possible;

            int index = Array.IndexOf(possible, list.Current);
            index = index >= 0 ? index : possible.Length - 1;

            int newIndex = right ? (index - 1 >= 0 ? index - 1 : possible.Length - 1) : (index + 1) % possible.Length;

            list.Current = possible[newIndex];

            SetText(prefix + list.Current.ToString().ToUpper());
            Update();
        }

        public override void MouseClickLeft(float x, float y)
        {
            base.MouseClickLeft(x, y);

            if (Hovering)
                UpdateSetting(true);
        }

        public override void MouseClickRight(float x, float y)
        {
            base.MouseClickRight(x, y);

            if (Hovering)
                UpdateSetting(false);
        }
    }
}
