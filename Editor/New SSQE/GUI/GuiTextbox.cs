using System;
using System.Globalization;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace New_SSQE.GUI
{
    internal class GuiTextbox : WindowControl
    {
        public string Text;
        private string prevText;

        public string Setting;
        public bool IsKeybind;
        public bool IsFloat;
        public bool IsPositive;

        public bool Numeric;
        public bool Focused;

        private int cursorPos;
        private float timer;
        private Color textColor;
        private Color prevColor = Color.White;

        public GuiTextbox(float posx, float posy, float sizex, float sizey, string text, int textSize, bool numeric, bool lockSize = false, bool moveWithOffset = false, string setting = "", string font = "main", bool isKeybind = false, bool isFloat = false, bool isPositive = false) : base(posx, posy, sizex, sizey)
        {
            Text = text;
            prevText = Text;
            Font = font;

            TextSize = textSize;
            OriginTextSize = textSize;

            Setting = setting;
            IsKeybind = isKeybind;
            IsFloat = isFloat;
            IsPositive = isPositive;

            if (isKeybind)
            {
                if (setting.Contains("gridKey"))
                    Text = Settings.settings["gridKeys"][int.Parse(setting.Replace("gridKey", ""))].ToString().ToUpper();
                else
                    Text = Settings.settings[setting].Key.ToString().ToUpper();
            }
            else if (setting != "")
                Text = Settings.settings[setting].ToString();

            Numeric = numeric;

            LockSize = lockSize;
            MoveWithOffset = moveWithOffset;

            Init();
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (prevText != Text || prevColor != textColor)
            {
                Update();

                prevText = Text;
                prevColor = textColor;

                cursorPos = MathHelper.Clamp(cursorPos, 0, Text.Length);
            }

            GL.BindVertexArray(VaO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 6, 10);

            if (Focused && (int)timer % 2 == 0)
                GL.DrawArrays(PrimitiveType.Triangles, 16, 6);

            timer += frametime * 2f;
        }

        public override void RenderTexture()
        {
            GL.Uniform4f(TexColorLocation, textColor.R / 255f, textColor.G / 255f, textColor.B / 255f, textColor.A / 255f);
            FontRenderer.RenderData(Font, FontVertices);
        }

        public override Tuple<float[], float[]> GetVertices()
        {
            var colored = MainWindow.Instance.CurrentWindow is not GuiWindowSettings;

            var color2 = colored ? Settings.settings["color2"] : Color.FromArgb(255, 255, 255);

            string textBeforeCursor = "";
            if (cursorPos >= 0)
            {
                if (cursorPos >= Text.Length)
                    textBeforeCursor = Text;
                else
                    textBeforeCursor = Text[..cursorPos];
            }

            cursorPos = Math.Min(cursorPos, Text.Length);

            float textBeforeCursorWidth = FontRenderer.GetWidth(textBeforeCursor, TextSize, Font);

            float txW = FontRenderer.GetWidth(Text, TextSize, Font);
            float txH = FontRenderer.GetHeight(TextSize, Font);
            float txX = Rect.X + Rect.Width / 2f - txW / 2f;
            float txY = Rect.Y + Rect.Height / 2f - txH / 2f;

            float xf = txX + textBeforeCursorWidth;

            float[] fill = GLU.Rect(Rect, 0.1f, 0.1f, 0.1f);
            float[] outline = GLU.Outline(Rect, 2, 0.5f, 0.5f, 0.5f);
            float[] cursor = GLU.Line(xf, txY, xf, txY + txH, 2, color2.R / 255f, color2.G / 255f, color2.B / 255f);

            List<float> vertices = new(fill);
            vertices.AddRange(outline);
            vertices.AddRange(cursor);

            FontVertices = FontRenderer.Print(txX, txY, Text, TextSize, Font);
            textColor = color2;

            return new Tuple<float[], float[]>(vertices.ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right)
        {
            if (Text.Length > 0)
            {
                var textWidth = FontRenderer.GetWidth(Text, TextSize, Font);
                var posX = pos.X - Rect.X - (Rect.Width - textWidth) / 2f;
                var letterWidth = textWidth / Text.Length;

                posX = MathHelper.Clamp(posX, 0, textWidth);
                posX = (float)Math.Floor(posX / letterWidth + 0.3);

                cursorPos = (int)posX;
            }

            timer = 0f;
            Focused = true;

            OnButtonClicked(-1);
            Update();
        }

        public override void OnKeyDown(Keys key, bool control)
        {
            if (!Focused || key == Keys.LeftControl || key == Keys.RightControl || key == Keys.LeftAlt || key == Keys.RightAlt || key == Keys.LeftShift || key == Keys.RightShift)
                return;

            timer = 0f;

            if (IsKeybind)
            {
                if (key == Keys.Backspace)
                    key = Keys.Delete;

                if (Setting.Contains("gridKey"))
                    Settings.settings["gridKeys"][int.Parse(Setting.Replace("gridKey", ""))] = key;
                else
                    Settings.settings[Setting] = new Keybind(key, MainWindow.Instance.CtrlHeld, MainWindow.Instance.AltHeld, MainWindow.Instance.ShiftHeld);

                Text = key.ToString().ToUpper();
                cursorPos = Text.Length;

                return;
            }

            switch (key)
            {
                case Keys.C when control:
                    if (!string.IsNullOrWhiteSpace(Text))
                        Clipboard.SetText(Text);

                    break;

                case Keys.V when control:
                    var clipboard = Clipboard.GetText();

                    if (!string.IsNullOrWhiteSpace(clipboard))
                    {
                        Text = Text.Insert(cursorPos, clipboard);
                        cursorPos += clipboard.Length;
                    }

                    break;

                case Keys.X when control:
                    if (!string.IsNullOrWhiteSpace(Text))
                    {
                        Clipboard.SetText(Text);
                        Text = "";
                    }

                    break;

                case Keys.Left:
                    if (control)
                        cursorPos = Math.Max(IndexOf(Text, " ", cursorPos - 1, false) + 1, 0);
                    else
                        cursorPos--;

                    break;

                case Keys.Right:
                    if (control)
                        cursorPos = Math.Max(IndexOf(Text, " ", cursorPos + 1, true), 0);
                    else
                        cursorPos++;

                    break;

                case Keys.Backspace:
                    if (control)
                    {
                        int index = Math.Max(IndexOf(Text, " ", cursorPos - 1, false), 0);
                        string word = Text.Substring(index, Math.Min(cursorPos - index, Text.Length - index));

                        Text = Text.Remove(index, word.Length);
                        cursorPos -= word.Length;
                    }
                    else if (Text.Length > 0 && cursorPos > 0)
                    {
                        cursorPos--;
                        Text = Text.Remove(cursorPos, 1);
                    }

                    break;

                case Keys.Delete:
                    if (control)
                    {
                        int index = Math.Max(IndexOf(Text, " ", cursorPos + 1, true), 0);
                        string word = Text.Substring(MathHelper.Clamp(cursorPos, 0, Text.Length - 1), Math.Max(index - cursorPos, 0));

                        Text = Text.Remove(cursorPos, word.Length);
                    }
                    else if (Text.Length > 0 && cursorPos < Text.Length)
                        Text = Text.Remove(Math.Min(cursorPos, Text.Length - 1), 1);

                    break;

                case Keys.Enter:
                case Keys.KeyPadEnter:
                    // maybe?
                    /*if (MainWindow.Instance.ShiftHeld)
                    {
                        Text += "\n";
                        cursorPos++;
                    }
                    else*/
                        Focused = false;

                    break;

                case Keys.Escape:
                    Focused = false;

                    break;

                default:
                    var str = KeyConverter.GetCharFromInput(key, MainWindow.Instance.ShiftHeld).ToString();

                    if (Numeric)
                    {
                        var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                        if (int.TryParse(str, out _) || (str == separator && !Text.Contains(str)) || (str == "-" && !Text.Contains('=') && cursorPos == 0))
                        {
                            Text = Text.Insert(cursorPos, str);
                            cursorPos++;
                        }
                    }
                    else
                    {
                        Text = Text.Insert(cursorPos, str);
                        cursorPos++;
                    }

                    SetSetting();

                    break;
            }

            cursorPos = MathHelper.Clamp(cursorPos, 0, Text.Length);
            SetSetting();

            Update();
        }

        private void SetSetting()
        {
            if (Setting != "" && (IsFloat ? float.TryParse(Text, out _) : int.TryParse(Text, out _)))
            {
                var num = IsFloat ? float.Parse(Text) : int.Parse(Text);
                if (!IsPositive || (IsPositive && num > 0))
                    Settings.settings[Setting] = num;
            }
        }

        // i dont remember how this works but it does so woo
        // indexing for ctrl backspace/delete
        private int IndexOf(string set, string match, int stop, bool onceAfter)
        {
            int current = -1;

            void Run(bool next)
            {
                if (current + 1 < set.Length)
                {
                    int store = set.IndexOf(match, current + 1);

                    if (next && store < 0)
                        store = Text.Length;

                    if (store > current && (store < stop || next || (next && stop + 1 == store)))
                    {
                        current = store;
                        Run(onceAfter && (store < stop || stop + 1 == store));
                    }
                }
            }

            Run(onceAfter);

            return current;
        }
    }
}
