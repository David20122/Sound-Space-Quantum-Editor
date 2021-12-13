using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
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


        public TimingPoints() : base(800, 600, new OpenTK.Graphics.GraphicsMode(32, 8, 0, 8), "Timing Setup Panel")
        {
            Instance = this;
            OpenGuiScreen(new GuiScreenTimings());
            Icon = Resources.icon;
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
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _clickedMouse = e.Position;

            if (e.Button == MouseButton.Left)
                GuiScreen?.OnMouseClick(e.X, e.Y);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            GuiScreen.OnKeyTyped(e.KeyChar);
        }

        protected override void OnClosing(CancelEventArgs e)
        {

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
