using System.Drawing;

namespace SSQE_Player.GUI
{
    class Gui
    {
        public RectangleF rect;

        protected Gui(float posx, float posy, float sizex, float sizey)
        {
            rect = new RectangleF(posx, posy, sizex, sizey);
        }

        public virtual void Render(float mousex, float mousey, float frametime)
        {

        }

        public virtual void OnResize(Size size)
        {

        }
    }
}
