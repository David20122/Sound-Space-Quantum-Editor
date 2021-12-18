using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sound_Space_Editor.Gui;
using Sound_Space_Editor.Properties;

namespace Sound_Space_Editor
{
    class TimingPoints : GameWindow
    {
        public GuiScreen GuiScreen { get; private set; }
        private Point _clickedMouse;
        private Point _lastMouse;
        public static TimingPoints Instance;
        public FontRenderer FontRenderer;
        public bool _rightDown;


        public TimingPoints() : base(800, 600, new OpenTK.Graphics.GraphicsMode(32, 8, 0, 8), "Timing Setup Panel")
        {
            Instance = this;
            OpenGuiScreen(new GuiScreenTimings());
            Icon = Resources.icon;
            FontRenderer = new FontRenderer("main");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.PushMatrix();

            GuiScreen?.Render((float)e.Time, _lastMouse.X, _lastMouse.Y);

            GL.PopMatrix();
            SwapBuffers();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            _lastMouse = e.Position;
            GuiScreen?.OnMouseMove(e.X, e.Y);

            if (GuiScreen is GuiScreenTimings gst)
            {
                if (gst.ScrollBar.Dragging)
                {
                    var rect = gst.ScrollBar.ClientRectangle;
                    var lineSize = rect.Height - rect.Width;
                    var step = lineSize / gst.ScrollBar.MaxValue;

                    var tick = (int)MathHelper.Clamp(Math.Round((lineSize - (e.Y - rect.Y - rect.Width / 2)) / step), 0, gst.ScrollBar.MaxValue);

                    gst.ScrollBar.Value = tick;

                    gst.ScrollIndex = GuiTrack.BPMs.Count - tick;
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _clickedMouse = e.Position;

            if (e.Button == MouseButton.Left)
                GuiScreen?.OnMouseClick(e.X, e.Y);
            if (e.Button == MouseButton.Right)
                _rightDown = true;

            if (GuiScreen is GuiScreenTimings gst)
            {
                if (!_rightDown)
                {
                    if (gst.ScrollBar.ClientRectangle.Contains(e.Position))
                    {
                        gst.ScrollBar.Dragging = true;
                        OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
                    }
                    var width = gst.OffsetBox.ClientRectangle.Right - gst.BPMBox.ClientRectangle.Left;
                    var height = gst.ClientRectangle.Height / 2;
                    var x = gst.BPMBox.ClientRectangle.X;
                    var y = gst.BPMLabel.ClientRectangle.Y - height - 10;
                    var rect = new RectangleF(x, y, width, height);
                    if (rect.Contains(e.Position))
                    {
                        var index = Math.Ceiling((e.Y - y) / height * 8 - 1);
                        if (index + gst.ScrollIndex < GuiTrack.BPMs.Count && index >= 0)
                        {
                            gst.SelectedIndex = (int)index + gst.ScrollIndex;
                            gst.BPMBox.Text = GuiTrack.BPMs[gst.SelectedIndex].bpm.ToString();
                            gst.OffsetBox.Text = (GuiTrack.BPMs[gst.SelectedIndex].Ms - GuiTrack.NoteOffset).ToString();
                        }
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
                _rightDown = false;

            if (GuiScreen is GuiScreenTimings gst)
            {
                gst.ScrollBar.Dragging = false;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (GuiScreen is GuiScreenTimings gst)
            {
                gst.ScrollBar.Value += (int)e.DeltaPrecise;
                gst.ScrollBar.Value = MathHelper.Clamp(gst.ScrollBar.Value, 0, gst.ScrollBar.MaxValue);
                gst.ScrollIndex -= (int)e.DeltaPrecise;
                gst.ScrollIndex = MathHelper.Clamp(gst.ScrollIndex, 0, GuiTrack.BPMs.Count);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            GuiScreen.OnKeyTyped(e.KeyChar);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            GuiScreen.OnKeyDown(e.Key, e.Control);
        }

        protected override void OnClosing(CancelEventArgs e)
        {

        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (GuiScreen is GuiScreenTimings gst)
                gst.OnMouseLeave();
        }

        public void OpenGuiScreen(GuiScreen s)
        {
            GuiScreen?.OnClosing();
            GuiScreen = s;
        }

        protected override void OnResize(EventArgs e)
        {
            if (ClientSize.Width < 800)
                ClientSize = new Size(800, ClientSize.Height);
            if (ClientSize.Height < 600)
                ClientSize = new Size(ClientSize.Width, 600);

            GL.Viewport(ClientRectangle);

            GL.MatrixMode(MatrixMode.Projection);
            var m = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, 0, 1);
            GL.LoadMatrix(ref m);

            GuiScreen?.OnResize(ClientSize);

            OnRenderFrame(new FrameEventArgs());
        }
    }
}
