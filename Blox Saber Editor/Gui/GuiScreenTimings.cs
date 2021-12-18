using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sound_Space_Editor.Gui
{
    class GuiScreenTimings : GuiScreen
    {
        public GuiScreen GuiScreen { get; private set; }

        public GuiTextBox BPMBox;
        public GuiTextBox OffsetBox;
        public GuiButton AddPoint;
        public GuiButton RemovePoint;
        public GuiButton UpdatePoint;
        public GuiLabel BPMLabel = new GuiLabel(0, 0, "BPM:", "main");
        public GuiLabel OffsetLabel = new GuiLabel(0, 0, "Offset[ms]:", "main");

        public GuiScreenTimings() : base(0, 0, 0, 0)
        {
            BPMBox = new GuiTextBox(0, 0, 256, 48) { Text = "0", Centered = true, Numeric = true, CanBeNegative = false };
            OffsetBox = new GuiTextBox(0, 0, 256, 48) { Text = "0", Centered = true, Numeric = true, CanBeNegative = false };

            AddPoint = new GuiButton(0, 0, 0, 200, 32, "ADD POINT");
            RemovePoint = new GuiButton(1, 0, 0, 200, 32, "REMOVE POINT");
            UpdatePoint = new GuiButton(2, 0, 0, 200, 32, "UPDATE POINT");

            BPMLabel.Color = Color.FromArgb(255, 255, 255);
            OffsetLabel.Color = Color.FromArgb(255, 255, 255);

            BPMBox.Focused = true;
            OffsetBox.Focused = true;
            BPMBox.OnKeyDown(Key.Right, false);
            OffsetBox.OnKeyDown(Key.Right, false);
            BPMBox.Focused = false;
            OffsetBox.Focused = false;

            OnResize(TimingPoints.Instance.ClientSize);

            Buttons.Add(AddPoint);
            Buttons.Add(RemovePoint);
            Buttons.Add(UpdatePoint);
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


        }

        public override bool AllowInput()
        {
            return !BPMBox.Focused && !OffsetBox.Focused;
        }

        public override void OnKeyTyped(char key)
        {
            BPMBox.OnKeyTyped(key);
            OffsetBox.OnKeyTyped(key);
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

                    break;
                case 1:

                    break;
                case 2:

                    break;
            }
        }

        public override void OnResize(Size size)
        {
            ClientRectangle = new RectangleF(0, 0, size.Width, size.Height);
            var middle = new PointF(ClientRectangle.Width / 2f, ClientRectangle.Height / 2f);

            BPMBox.ClientRectangle.Location = new PointF(middle.X - BPMBox.ClientRectangle.Width - 5, middle.Y);
            OffsetBox.ClientRectangle.Location = new PointF(middle.X + 5, BPMBox.ClientRectangle.Y);
            RemovePoint.ClientRectangle.Location = new PointF(BPMBox.ClientRectangle.X, BPMBox.ClientRectangle.Bottom + 10);
            UpdatePoint.ClientRectangle.Location = new PointF(OffsetBox.ClientRectangle.X, RemovePoint.ClientRectangle.Y);
            AddPoint.ClientRectangle.Location = new PointF(middle.X - AddPoint.ClientRectangle.Width / 2, RemovePoint.ClientRectangle.Bottom + 10);
            BPMLabel.ClientRectangle.Location = new PointF(BPMBox.ClientRectangle.X + 5, BPMBox.ClientRectangle.Top - 20);
            OffsetLabel.ClientRectangle.Location = new PointF(OffsetBox.ClientRectangle.X + 5, OffsetBox.ClientRectangle.Top - 20);

            base.OnResize(size);
        }
    }
}
