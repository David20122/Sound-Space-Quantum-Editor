using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using KeyPressEventArgs = OpenTK.KeyPressEventArgs;

namespace Map_Player
{
    class Player : GameWindow
    {
        private List<Note> Notes = new List<Note>();
        private MusicPlayer MusicPlayer;
        private Camera camera;
        public bool Loaded = false;
        public bool Playing = false;
        public bool Paused = false;

        public Player(String file) : base(800, 600, new GraphicsMode(32, 8, 0, 8), "Map Player")
        {
            this.WindowBorder = WindowBorder.Fixed;
            MusicPlayer = new MusicPlayer { Volume = 0.25f };
            if (File.Exists(file))
            {
                this.LoadMap(File.ReadAllText(file));
            }
        }
        private void LoadMap(String data)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0, 0, 0, 1);
            GL.Enable(EnableCap.DepthTest);
            camera = new Camera(this);
            
            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (this.Focused && !Paused)
            {
                this.CursorVisible = false;
            } else
            {
                this.CursorVisible = true;
            }

            GL.LoadIdentity();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            camera.Update();

            // render grid
            GL.Color4(0.8f, 0.8f, 0.8f, 0.4f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-1.5, 0.5, -5);
            GL.Vertex3(1.5, 0.5, -5);
            GL.Vertex3(-0.5, -1.5, -5);
            GL.Vertex3(-0.5, 1.5, -5);
            GL.Vertex3(-1.5, -0.5, -5);
            GL.Vertex3(1.5, -0.5, -5);
            GL.Vertex3(0.5, -1.5, -5);
            GL.Vertex3(0.5, 1.5, -5);
            GL.End();

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 matrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(56), Width / Height, 1, 100);
            GL.LoadMatrix(ref matrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            ref Matrix4 matrix1 = ref camera.matrix;
            GL.LoadMatrix(ref matrix1);

            base.OnResize(e);
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Paused = !Paused;
            }
            base.OnKeyDown(e);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused && !Paused)
            {
                camera.MouseUpdate(e);
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }
            base.OnMouseMove(e);
        }
    }
    class Note
    {
        private readonly float x;
        private readonly float y;
        private readonly int ms;
        public Note(float x, float y, int ms)
        {
            this.x = x;
            this.y = y;
            this.ms = ms;
        }
    }
}
