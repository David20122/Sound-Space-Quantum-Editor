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
        private int VBO;
        private int VAO;
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
            
            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex2(1.0, 1.0);
            GL.Vertex2(1.0, 0);
            GL.Vertex2(0, 0);
            GL.End();

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

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
