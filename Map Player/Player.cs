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
        private bool Loaded = false;
        private bool Playing = false;
        private bool Paused = false;

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
            
            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.LoadIdentity();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Translate(0, 0, 0);

            GL.Begin(PrimitiveType.Quads);
            GL.Vertex3(1, 1, -5);
            GL.Vertex3(-1, 1, -5);
            GL.Vertex3(-1, -1, -5);
            GL.Vertex3(1, -1, -5);
            GL.End();

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 matrix;
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), this.Width / this.Height, 1f, 100f, out matrix); 
            GL.LoadMatrix(ref matrix);
            GL.MatrixMode(MatrixMode.Modelview);

            base.OnResize(e);
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
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
