using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Sound_Space_Editor.GUI
{
	class GuiTextbox : Gui
	{
        public string text;
        public int textSize;
        public string font;

        public string setting;
        public bool isKeybind;
        public bool isFloat;

        public int cursorPos;
        public float timer;

        public bool numeric;
        public bool focused;

        public bool Visible = true;
        public bool lockSize;
        public bool moveWithOffset;

        public RectangleF originRect;
        public int originTextSize;

        public GuiTextbox(float posx, float posy, float sizex, float sizey, string Text, int TextSize, bool Numeric, bool LockSize = false, bool MoveWithOffset = false, string Setting = "", string Font = "main", bool IsKeybind = false, bool IsFloat = false) : base(posx, posy, sizex, sizey)
        {
            text = Text;
            textSize = TextSize;
            font = Font;

            setting = Setting;
            isKeybind = IsKeybind;
            isFloat = IsFloat;

            if (isKeybind)
            {
                if (setting.Contains("gridKey"))
                    text = Settings.settings["gridKeys"][int.Parse(setting.Replace("gridKey", ""))].ToString().ToUpper();
                else
                    text = Settings.settings[setting].Key.ToString().ToUpper();
            }
            else if (setting != "")
                text = Settings.settings[setting].ToString();


            numeric = Numeric;

            lockSize = LockSize;
            moveWithOffset = MoveWithOffset;

            originRect = new RectangleF(posx, posy, sizex, sizey);
            originTextSize = textSize;
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (Visible)
            {
                var colored = !(MainWindow.Instance.CurrentWindow is GuiWindowSettings);

                var color2 = colored ? Settings.settings["color2"] : Color.FromArgb(255, 255, 255);

                var x = rect.X + rect.Width / 2f;
                var y = rect.Y + rect.Height / 2f;

                GL.Color3(0.1f, 0.1f, 0.1f);
                GLSpecial.Rect(rect);
                GL.Color3(0.5f, 0.5f, 0.5f);
                GLSpecial.Outline(rect);

                var shiftx = -TextWidth(text, textSize, font) / 2f;

                GL.Color3(color2);
                RenderText(text, x + shiftx, y - TextHeight(textSize, font) / 2f, textSize, font);

                if (focused && (int)timer % 2 == 0)
                {
                    var textBeforeCursor = text.Substring(0, cursorPos);
                    var textBeforeCursorWidth = TextWidth(textBeforeCursor, textSize, font);
                    var height = TextHeight(textSize, font) * 1.4f;

                    var xf = x + shiftx + textBeforeCursorWidth;

                    GL.LineWidth(2f);
                    GLSpecial.Line(xf, y - height / 2f, xf, y + height / 2f);
                }

                timer += frametime * 2f;
            }
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            if (text.Length > 0)
            {
                var textwidth = TextWidth(text, textSize, font);
                var posX = pos.X - rect.X - (rect.Width - textwidth) / 2f;
                var letterwidth = textwidth / text.Length;

                posX = MathHelper.Clamp(posX, 0, textwidth);
                posX = (float)Math.Floor(posX / letterwidth + 0.3);

                cursorPos = (int)posX;
            }

            timer = 0f;
            focused = true;
        }

        private void SetSetting()
        {
            if (setting != "" && (isFloat ? float.TryParse(text, out _) : int.TryParse(text, out _)))
                Settings.settings[setting] = float.Parse(text);
        }

        public override void OnKeyTyped(char key)
        {
            if (isKeybind || !focused)
                return;

            var str = key.ToString();

            if (numeric)
            {
                var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                if (int.TryParse(str, out _) || (str == separator && !text.Contains(str)) || (str == "-" && !text.Contains("-") && cursorPos == 0))
                {
                    text = text.Insert(cursorPos, str);
                    cursorPos++;
                }
            }
            else
            {
                text = text.Insert(cursorPos, str);
                cursorPos++;
            }

            SetSetting();

            timer = 0f;
        }

        public override void OnKeyDown(Key key, bool control)
        {
            if (!focused || key == Key.LControl || key == Key.RControl || key == Key.LAlt || key == Key.RAlt || key == Key.LShift || key == Key.RShift)
                return;

            timer = 0f;

            if (isKeybind)
            {
                if (key == Key.BackSpace)
                    key = Key.Delete;

                if (setting.Contains("gridKey"))
                    Settings.settings["gridKeys"][int.Parse(setting.Replace("gridKey", ""))] = key;
                else
                    Settings.settings[setting] = new Keybind(key, MainWindow.Instance.ctrlHeld, MainWindow.Instance.altHeld, MainWindow.Instance.shiftHeld);

                text = key.ToString().ToUpper();
                cursorPos = text.Length;
                return;
            }

            switch (key)
            {
                case Key.C when control:
                    if (!string.IsNullOrWhiteSpace(text))
                        Clipboard.SetText(text);

                    break;

                case Key.V when control:
                    var clipboard = Clipboard.GetText();

                    if (!string.IsNullOrWhiteSpace(clipboard))
                    {
                        text = text.Insert(cursorPos, clipboard);
                        cursorPos += clipboard.Length;
                    }

                    break;

                case Key.X when control:
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        Clipboard.SetText(text);
                        text = "";
                    }

                    break;

                case Key.Left:
                    if (control)
                        cursorPos = Math.Max(IndexOf(text, " ", cursorPos - 1, false) + 1, 0);
                    else
                        cursorPos--;

                    break;

                case Key.Right:
                    if (control)
                        cursorPos = Math.Max(IndexOf(text, " ", cursorPos + 1, true), 0);
                    else
                        cursorPos++;

                    break;

                case Key.BackSpace:
                    if (control)
                    {
                        int index = Math.Max(IndexOf(text, " ", cursorPos - 1, false), 0);
                        string word = text.Substring(index, Math.Min(cursorPos - index, text.Length - index));

                        text = text.Remove(index, word.Length);
                        cursorPos -= word.Length;
                    }
                    else if (text.Length > 0 && cursorPos > 0)
                    {
                        cursorPos--;
                        text = text.Remove(cursorPos, 1);
                    }

                    break;

                case Key.Delete:
                    if (control)
                    {
                        int index = Math.Max(IndexOf(text, " ", cursorPos + 1, true), 0);
                        string word = text.Substring(MathHelper.Clamp(cursorPos, 0, text.Length - 1), Math.Max(index - cursorPos, 0));

                        text = text.Remove(cursorPos, word.Length);
                    }
                    else if (text.Length > 0 && cursorPos < text.Length)
                        text = text.Remove(Math.Min(cursorPos, text.Length - 1), 1);

                    break;

                case Key.Enter:
                case Key.KeypadEnter:
                    focused = false;

                    break;
            }

            SetSetting();

            cursorPos = MathHelper.Clamp(cursorPos, 0, text.Length);
        }

        private int IndexOf(string set, string match, int stop, bool onceAfter)
        {
            int current = -1;

            void run(bool next)
            {
                if (current + 1 < set.Length)
                {
                    int store = set.IndexOf(match, current + 1);

                    if (next && store < 0)
                        store = text.Length;

                    if (store > current && (store < stop || next || (next && stop + 1 == store)))
                    {
                        current = store;
                        run(onceAfter && (store < stop || stop + 1 == store));
                    }
                }
            }

            run(onceAfter);

            return current;
        }
    }
}