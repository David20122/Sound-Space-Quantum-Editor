using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.ComponentModel;
using OpenTK;
using Sound_Space_Editor.Properties;
using System.Globalization;

namespace Sound_Space_Editor.Gui
{
    public class GuiScreenTimings : GuiScreen
    {
        public GuiScreen GuiScreen { get; private set; }

        public GuiSlider ScrollBar;
        public GuiTextBox BPMBox;
        public GuiTextBox OffsetBox;
        public GuiButton AddPoint;
        public GuiButton RemovePoint;
        public GuiButton UpdatePoint;
        public GuiButton CurrentMs;
        public GuiLabel BPMLabel = new GuiLabel(0, 0, "BPM:", true);
        public GuiLabel OffsetLabel = new GuiLabel(0, 0, "Offset[ms]:", true);
        public int HoverIndex = -1;
        public int ScrollIndex = 0;
        public int SelectedIndex = -1;

        public GuiScreenTimings() : base(0, 0, 0, 0)
        {
            BPMBox = new GuiTextBox(0, 0, 256, 48) { Text = GuiTrack.Bpm.ToString(), Centered = true, Numeric = true, CanBeNegative = false, Timings = true };
            OffsetBox = new GuiTextBox(0, 0, 256, 48) { Text = GuiTrack.BpmOffset.ToString(), Centered = true, Numeric = true, CanBeNegative = false, Timings = true };

            AddPoint = new GuiButton(0, 0, 0, 256, 48, "ADD POINT", true);
            RemovePoint = new GuiButton(1, 0, 0, 256, 48, "REMOVE POINT", true);
            UpdatePoint = new GuiButton(2, 0, 0, 256, 48, "UPDATE POINT", true);
            CurrentMs = new GuiButton(3, 0, 0, 256, 48, "USE CURRENT MS", true);

            BPMLabel.Color = Color.FromArgb(255, 255, 255);
            OffsetLabel.Color = Color.FromArgb(255, 255, 255);

            BPMBox.Focused = true;
            OffsetBox.Focused = true;
            BPMBox.OnKeyDown(Key.Right, false);
            OffsetBox.OnKeyDown(Key.Right, false);
            BPMBox.Focused = false;
            OffsetBox.Focused = false;

            ScrollBar = new GuiSlider(0, 0, 20, 100)
            {
                MaxValue = GuiTrack.BPMs.Count,
                Value = GuiTrack.BPMs.Count,
            };

            OnResize(TimingPoints.Instance.ClientSize);

            Buttons.Add(AddPoint);
            Buttons.Add(RemovePoint);
            Buttons.Add(UpdatePoint);
            Buttons.Add(CurrentMs);
            Buttons.Add(ScrollBar);
        }

        public override void Render(float delta, float mouseX, float mouseY)
        {
            BPMBox.Render(delta, mouseX, mouseY);
            OffsetBox.Render(delta, mouseX, mouseY);

            foreach (var button in Buttons)
            {
                button.Render(delta, mouseX, mouseY);
            }

            BPMLabel.Render(delta, mouseX, mouseY);
            OffsetLabel.Render(delta, mouseX, mouseY);

            var fr = TimingPoints.Instance.FontRenderer;

            var width = OffsetBox.ClientRectangle.Right - BPMBox.ClientRectangle.Left;
            var height = ClientRectangle.Height / 2;
            var x = BPMBox.ClientRectangle.X;
            var y = BPMLabel.ClientRectangle.Y - height - 10;

            GL.LineWidth(5);
            GL.Color3(1f, 1f, 1f);
            Glu.RenderOutline(x, y, width, height);

            for (var i = 0; i < GuiTrack.BPMs.Count; i++)
            {
                if (i >= ScrollIndex && i < ScrollIndex + 8)
                {
                    var bpm = GuiTrack.BPMs[i];
                    var by = y + height / 8 * (i - ScrollIndex);
                    var bpmRect = new RectangleF(x, by, width, height / 8);

                    if (bpmRect.Contains(mouseX, mouseY))
                        GL.Color3(0, 0.5f, 1f);
                    if (i == SelectedIndex)
                        GL.Color3(0, 1f, 0);

                    Glu.RenderOutline(x, by, width, height / 8);
                    GL.Color3(1f, 1f, 1f);
                    fr.Render($"BPM: {bpm.bpm}", (int)x + 5, (int)by + 10, (int)(height / 8) - 10);
                    fr.Render($"Offset: {bpm.Ms - GuiTrack.NoteOffset}", (int)(x + 5 + width / 2), (int)by + 10, (int)(height / 8) - 10);
                }

            }

            GL.LineWidth(1);
        }

        public override bool AllowInput()
        {
            return !BPMBox.Focused && !OffsetBox.Focused;
        }

        public override void OnKeyTyped(char key)
        {
            BPMBox.OnKeyTyped(key);
            OffsetBox.OnKeyTyped(key);

            if (BPMBox.Focused)
            {
                var text = BPMBox.Text;
                var decimalPont = false;

                if (text.Length > 0 && text[text.Length - 1].ToString() ==
                    CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                {
                    text = text + 0;

                    decimalPont = true;
                }

                if (text.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                {
                    decimalPont = true;
                }

                decimal.TryParse(text, out var bpm);

                if (bpm < 0)
                    bpm = 0;
                else if (bpm > 5000)
                    bpm = 5000;
                if (!decimalPont && bpm > 0)
                    BPMBox.Text = bpm.ToString();
            }
            if (OffsetBox.Focused)
            {
                long.TryParse(OffsetBox.Text, out var offset);

                offset = (long)MathHelper.Clamp(offset, 0, EditorWindow.Instance.MusicPlayer.TotalTime.TotalMilliseconds);

                if (offset > 0)
                    OffsetBox.Text = offset.ToString();
            }

            if (Settings.Default.LegacyBPM)
            {
                if (float.TryParse(BPMBox.Text, out float bpm))
                    GuiTrack.Bpm = bpm;
                if (long.TryParse(OffsetBox.Text, out long offset))
                    GuiTrack.BpmOffset = offset;
            }
        }

        public override void OnKeyDown(Key key, bool control)
        {
            BPMBox.OnKeyDown(key, control);
            OffsetBox.OnKeyDown(key, control);
        }

        public override void OnMouseClick(float x, float y)
        {
            BPMBox.OnMouseClick(x, y);
            OffsetBox.OnMouseClick(x, y);

            base.OnMouseClick(x, y);
        }

        protected override void OnButtonClicked(int id)
        {
            switch (id)
            {
                case 0:
                    if (float.TryParse(BPMBox.Text, out float bpm) && bpm > 33 && long.TryParse(OffsetBox.Text, out long offset) && offset >= 0)
                    {
                        var exists = false;
                        foreach (var point in GuiTrack.BPMs)
                        {
                            if (point.Ms == offset)
                                exists = true;
                        }
                        if (!exists)
                        {
                            GuiTrack.BPMs.Add(new BPM(bpm, offset + GuiTrack.NoteOffset));
                            GuiTrack.BPMs = GuiTrack.BPMs.OrderBy(o => o.Ms).ToList();
                            ScrollBar.MaxValue = GuiTrack.BPMs.Count;
                            ScrollBar.Value = MathHelper.Clamp(ScrollBar.Value + 1, 0, ScrollBar.MaxValue);
                        }
                    }
                    break;
                case 1:
                    if (SelectedIndex >= 0)
                    {
                        GuiTrack.BPMs.RemoveAt(SelectedIndex);
                        GuiTrack.BPMs = GuiTrack.BPMs.OrderBy(o => o.Ms).ToList();
                        if (GuiTrack.BPMs.Count == 0)
                            SelectedIndex = -1;
                        ScrollBar.MaxValue = GuiTrack.BPMs.Count;
                        ScrollBar.Value = MathHelper.Clamp(ScrollBar.Value - 1, 0, ScrollBar.MaxValue);
                    }
                    break;
                case 2:
                    if (SelectedIndex >= 0)
                    {
                        if (float.TryParse(BPMBox.Text, out float bPM) && bPM > 33 && long.TryParse(OffsetBox.Text, out long OFfset) && OFfset >= 0)
                        {
                            var selectedpoint = GuiTrack.BPMs[SelectedIndex];
                            var exists = false;
                            foreach (var point in GuiTrack.BPMs)
                            {
                                if (point.Ms == OFfset && point != selectedpoint)
                                    exists = true;
                            }
                            if (!exists)
                            {
                                selectedpoint.bpm = bPM;
                                selectedpoint.Ms = OFfset + GuiTrack.NoteOffset;
                                GuiTrack.BPMs = GuiTrack.BPMs.OrderBy(o => o.Ms).ToList();
                            }
                        }
                    }
                    break;
                case 3:
                    OffsetBox.Text = EditorWindow.Instance.MusicPlayer.CurrentTime.TotalMilliseconds.ToString();
                    break;
            }
        }

        public void OnMouseLeave()
        {
            ScrollBar.Dragging = false;
        }

        public override void OnResize(Size size)
        {
            ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);
            var middle = new PointF(ClientRectangle.Width / 2f, ClientRectangle.Height / 2f);

            BPMBox.ClientRectangle.Location = new PointF(middle.X - BPMBox.ClientRectangle.Width - 5, middle.Y + ClientRectangle.Height / 6);
            OffsetBox.ClientRectangle.Location = new PointF(middle.X + 5, BPMBox.ClientRectangle.Y);
            UpdatePoint.ClientRectangle.Location = new PointF(BPMBox.ClientRectangle.X, BPMBox.ClientRectangle.Bottom + 10);
            RemovePoint.ClientRectangle.Location = new PointF(BPMBox.ClientRectangle.X, UpdatePoint.ClientRectangle.Bottom + 10);
            CurrentMs.ClientRectangle.Location = new PointF(OffsetBox.ClientRectangle.X, UpdatePoint.ClientRectangle.Y);
            AddPoint.ClientRectangle.Location = new PointF(OffsetBox.ClientRectangle.X, RemovePoint.ClientRectangle.Y);
            BPMLabel.ClientRectangle.Location = new PointF(BPMBox.ClientRectangle.X + 5, BPMBox.ClientRectangle.Top - 24);
            OffsetLabel.ClientRectangle.Location = new PointF(OffsetBox.ClientRectangle.X + 5, OffsetBox.ClientRectangle.Top - 24);

            ScrollBar.ClientRectangle.Location = new PointF(OffsetBox.ClientRectangle.Right, BPMLabel.ClientRectangle.Y - ClientRectangle.Height / 2f - 10);
            ScrollBar.ClientRectangle.Size = new SizeF(25, ClientRectangle.Height / 2f);
            ScrollBar.MaxValue = GuiTrack.BPMs.Count;
            base.OnResize(size);
        }
    }
}
