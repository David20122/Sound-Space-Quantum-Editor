using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal abstract class WindowControl : IDisposable
    {
        public int VaO;
        public int VbO;

        public int tVaO;
        public int tVbO;

        public int tHandle = -1;

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

        public readonly int TexColorLocation;

        public WindowControl(float posx, float posy, float sizex, float sizey)
        {
            Rect = new(posx, posy, sizex, sizey);
            OriginRect = new(posx, posy, sizex, sizey);

            TexColorLocation = GL.GetUniformLocation(Shader.FontTexProgram, "TexColor");
        }

        // Vertex: X, Y, R, G, B, A (float)
        // Text Vertex: X, Y, TX, TY, A (float)
        public void Init()
        {
            var vertices = GetVertices();

            
            VaO = GL.GenVertexArray();
            VbO = GL.GenBuffer();
            tVaO = GL.GenVertexArray();
            tVbO = GL.GenBuffer();

            // normal rendering
            GL.BindVertexArray(VaO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VbO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Item1.Length, vertices.Item1, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // texture rendering
            GL.BindVertexArray(tVaO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, tVbO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Item2.Length, vertices.Item2, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 5 * sizeof(float), 4 * sizeof(float));
            GL.EnableVertexAttribArray(2);


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void Update()
        {
            var vertices = GetVertices();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VbO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Item1.Length, vertices.Item1, Dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, tVbO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Item2.Length, vertices.Item2, Dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public virtual void Dispose()
        {
            IsDisposed = true;

            ClearBuffers();

            GL.DeleteVertexArray(VaO);
            GL.DeleteBuffer(VbO);

            GL.DeleteVertexArray(tVaO);
            GL.DeleteBuffer(tVbO);

            if (tHandle >= 0)
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
        public int[] VaOs = Array.Empty<int>();
        public int[] VbOs = Array.Empty<int>();
        public int[] VertexCounts = Array.Empty<int>();

        public virtual void AddToBuffers(float[] vertices, int index)
        {
            var vao = GL.GenVertexArray();
            var staticVbO = GL.GenBuffer();
            var vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, staticVbO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(vao);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribFormat(0, 2, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vao, 0, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribFormat(1, 4, VertexAttribType.Float, false, 2 * sizeof(float));
            GL.VertexArrayAttribBinding(vao, 1, 0);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribFormat(2, 4, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(vao, 2, 1);
            GL.VertexBindingDivisor(1, 1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);


            VaOs[index] = vao;
            VbOs[2 * index] = staticVbO;
            VbOs[2 * index + 1] = vbo;
            VertexCounts[index] = vertices.Length / 6;
        }

        public virtual void RegisterData(int index, Vector4[] data)
        {
            if (data.Length > 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, VbOs[2 * index + 1]);
                GL.BufferData(BufferTarget.ArrayBuffer, 4 * sizeof(float) * data.Length, data, BufferUsageHint.DynamicDraw);

                GL.BindVertexArray(VaOs[index]);
                GL.BindVertexBuffer(0, VbOs[2 * index], IntPtr.Zero, 6 * sizeof(float));
                GL.BindVertexBuffer(1, VbOs[2 * index + 1], IntPtr.Zero, 4 * sizeof(float));

                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, VertexCounts[index], data.Length);
            }
        }

        public virtual void ClearBuffers()
        {
            for (int i = 0; i < VaOs.Length; i++)
                GL.DeleteVertexArray(VaOs[i]);
            for (int i = 0; i < VbOs.Length; i++)
                GL.DeleteBuffer(VbOs[i]);
        }

        public virtual void InstanceSetup() { }
        public virtual void GenerateOffsets() { }
    }
}
