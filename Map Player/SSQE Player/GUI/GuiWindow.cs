using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;

namespace SSQE_Player.GUI
{
    internal class GuiWindow
    {
        public RectangleF Rect;

        public List<GuiLabel> Labels;

        protected GuiWindow(float posx, float posy, float sizex, float sizey)
        {
            Rect = new(posx, posy, sizex, sizey);
        }

        public virtual void Render(float frametime)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);

            GL.UseProgram(Shader.FontTexProgram);
            FontRenderer.SetActive();

            foreach (GuiLabel label in Labels)
                label.Render();

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        public virtual void OnResize(Vector2i size)
        {
            float widthdiff = size.X / 1920f;
            float heightdiff = size.Y / 1080f;

            foreach (GuiLabel label in Labels)
            {
                label.Rect = ResizeRect(label.OriginRect, widthdiff, heightdiff);
                label.TextSize = (int)(label.OriginTextSize * heightdiff);

                label.Update();
            }
        }

        private static RectangleF ResizeRect(RectangleF originrect, float width, float height)
        {
            return new(originrect.X * width, originrect.Y * height, originrect.Width * width, originrect.Height * height);
        }
    }
}
