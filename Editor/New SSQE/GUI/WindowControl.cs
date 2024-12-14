using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal abstract class WindowControl : IDisposable
    {
        public VertexArrayHandle VaO;
        public BufferHandle VbO;

        public VertexArrayHandle tVaO;
        public BufferHandle tVbO;

        public TextureHandle tHandle = TextureHandle.Zero;

        public RectangleF Rect;
        public RectangleF OriginRect;

        public int TextSize;
        public int OriginTextSize;
        public string Font;

        public Vector4[] FontVertices;

        public bool Visible = true;
        public bool LockSize = false;
        public bool MoveWithOffset = false;

        public bool Dynamic = false;

        public bool IsDisposed = false;

        public static int TexColorLocation;

        public WindowControl(float x, float y, float w, float h)
        {
            Rect = new(x, y, w, h);
            OriginRect = new(x, y, w, h);
        }

        // Vertex: X, Y, R, G, B, A (float)
        // Text Vertex: X, Y, TX, TY, A (float)
        public void Init()
        {
            Tuple <float[], float[]> vertices = GetVertices();

            
            VaO = GL.GenVertexArray();
            VbO = GL.GenBuffer();
            tVaO = GL.GenVertexArray();
            tVbO = GL.GenBuffer();

            // normal rendering
            GL.BindVertexArray(VaO);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, vertices.Item1, BufferUsageARB.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // texture rendering
            GL.BindVertexArray(tVaO);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, tVbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, vertices.Item2, BufferUsageARB.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 5 * sizeof(float), 4 * sizeof(float));
            GL.EnableVertexAttribArray(2);


            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
            GL.BindVertexArray(VertexArrayHandle.Zero);
        }

        public void Update()
        {
            Tuple<float[], float[]> vertices = GetVertices();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, vertices.Item1, Dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, tVbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, vertices.Item2, Dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
        }

        public virtual void Dispose()
        {
            IsDisposed = true;

            ClearBuffers();

            GL.DeleteVertexArray(VaO);
            GL.DeleteBuffer(VbO);

            GL.DeleteVertexArray(tVaO);
            GL.DeleteBuffer(tVbO);

            if (tHandle.Handle >= 0 && !TextureManager.IsInUse(tHandle))
                GL.DeleteTexture(tHandle);
        }

        public static void OnButtonClicked(int id)
        {
            MainWindow.Instance.CurrentWindow?.OnButtonClicked(id);
        }

        public abstract Tuple<float[], float[]> GetVertices();
        public abstract void Render(float mousex, float mousey, float frametime);
        public abstract void RenderTexture();

        public virtual void OnMouseClick(Point pos, bool right) { }
        public virtual void OnMouseUp(Point pos) { }
        public virtual void OnMouseLeave(Point pos) { }
        public virtual void OnMouseMove(Point pos) { }
        public virtual void OnKeyDown(Keys key, bool control) { }




        // Instancing
        public VertexArrayHandle[] VaOs = Array.Empty<VertexArrayHandle>();
        public BufferHandle[] VbOs = Array.Empty<BufferHandle>();
        public int[] VertexCounts = Array.Empty<int>();

        public virtual void AddToBuffers(float[] vertices, int index)
        {
            VertexArrayHandle vao = GL.GenVertexArray();
            BufferHandle staticVbO = GL.GenBuffer();
            BufferHandle vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, staticVbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

            GL.BindVertexArray(vao);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribDivisor(2, 1);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
            GL.BindVertexArray(VertexArrayHandle.Zero);


            VaOs[index] = vao;
            VbOs[2 * index] = staticVbO;
            VbOs[2 * index + 1] = vbo;
            VertexCounts[index] = vertices.Length / 6;
        }

        public virtual void RegisterData(int index, Vector4[] data, int? count = null)
        {
            if (data.Length > 0)
            {
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbOs[2 * index + 1]);
                GL.BufferData(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.DynamicDraw);

                GL.BindVertexArray(VaOs[index]);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, VertexCounts[index], count ?? data.Length);
            }
        }

        public virtual void ClearBuffers()
        {
            for (int i = 0; i < VbOs.Length; i++)
            {
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbOs[i]);
                GL.BufferData(BufferTargetARB.ArrayBuffer, 0, IntPtr.Zero, BufferUsageARB.StaticDraw);

                GL.DeleteBuffer(VbOs[i]);
            }

            for (int i = 0; i < VaOs.Length; i++)
                GL.DeleteVertexArray(VaOs[i]);
        }

        public virtual void InstanceSetup() { }
        public virtual void GenerateOffsets() { }
    }
}
