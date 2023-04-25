using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New_SSQE.GUI
{
    internal class GuiWindow
    {
        public RectangleF Rect;

        public List<WindowControl> Controls = new();
        public Dictionary<string, List<WindowControl>> FontSet = new()
        {
            {"main", new List<WindowControl>() },
            {"square", new List<WindowControl>() },
            {"squareo", new List<WindowControl>() },
            {"other", new List<WindowControl>() }
        };

        public GuiTrack? Track;
        public GuiGrid? Grid;
        public GuiSquare? BackgroundSquare;

        public float YOffset = 80;

        private bool buttonClicked = false;

        protected GuiWindow(float posx, float posy, float sizex, float sizey)
        {
            Rect = new RectangleF(posx, posy, sizex, sizey);
        }

        protected void Init()
        {
            foreach (var control in Controls)
            {
                if (control.Font != null && FontSet.ContainsKey(control.Font))
                    FontSet[control.Font].Add(control);
                else
                    FontSet["other"].Add(control);
            }
        }

        public virtual void Render(float mousex, float mousey, float frametime)
        {
            var controlsCopied = Controls.ToList();
            var subcontrolsCopied = new Dictionary<string, List<WindowControl>>(FontSet);

            GL.UseProgram(Shader.TexProgram);

            BackgroundSquare?.RenderTexture();

            foreach (var control in subcontrolsCopied["other"])
                if (control.Visible && !control.IsDisposed)
                    control.RenderTexture();


            GL.UseProgram(Shader.Program);

            BackgroundSquare?.Render(mousex, mousey, frametime);

            foreach (var control in controlsCopied)
                if (control.Visible && !control.IsDisposed)
                    control.Render(mousex, mousey, frametime);

            Grid?.Render(mousex, mousey, frametime);
            Track?.Render(mousex, mousey, frametime);


            GL.UseProgram(Shader.FontTexProgram);

            FontRenderer.SetActive("main");
            foreach (var control in subcontrolsCopied["main"])
                if (control.Visible && !control.IsDisposed)
                    control.RenderTexture();

            Track?.RenderTexture();
            Grid?.RenderTexture();

            FontRenderer.SetActive("square");
            foreach (var control in subcontrolsCopied["square"])
                if (control.Visible && !control.IsDisposed)
                    control.RenderTexture();

            FontRenderer.SetActive("squareo");
            foreach (var control in subcontrolsCopied["squareo"])
                if (control.Visible && !control.IsDisposed)
                    control.RenderTexture();
        }

        public virtual void OnMouseClick(Point pos, bool right)
        {
            var editor = MainWindow.Instance;

            if (Track != null && (Track.Rect.Contains(pos) || Track.HoveringPoint != null || Track.DraggingNote != null || Track.DraggingPoint != null))
                Track.OnMouseClick(pos, right);

            var controlsCopied = Controls.ToList();

            if (!right)
            {
                buttonClicked = false;

                foreach (var control in controlsCopied)
                {
                    var hitbox = control.Rect;

                    if (control is GuiSlider)
                    {
                        var horizontal = control.Rect.Width > control.Rect.Height;
                        var xdiff = horizontal ? 12f : 0f;
                        var ydiff = horizontal ? 0f : 12f;

                        hitbox = new RectangleF(control.Rect.X - xdiff, control.Rect.Y - ydiff, control.Rect.Width + xdiff * 2f, control.Rect.Height + ydiff * 2f);
                    }

                    if (!buttonClicked && control.Visible && hitbox.Contains(pos))
                        control.OnMouseClick(pos, false);
                    else if (control is GuiTextbox box)
                        box.Focused = false;
                }

                var gridRect = Grid == null ? new RectangleF() :
                    (Settings.settings["enableQuantum"] ? new RectangleF(Grid.Rect.X - Grid.Rect.Width / 3f, Grid.Rect.Y - Grid.Rect.Height / 3f, Grid.Rect.Width * 5 / 3f, Grid.Rect.Height * 5 / 3f) : Grid.Rect);

                if (!buttonClicked && Grid != null && gridRect.Contains(pos))
                    Grid.OnMouseClick(pos);
                else if (!buttonClicked && Track != null && !Track.Rect.Contains(pos))
                {
                    editor.SelectedNotes.Clear();
                    editor.UpdateSelection();
                    if (Track.HoveringPoint == null)
                        editor.SelectedPoint = null;
                }
            }
            else
            {
                editor.SelectedNotes.Clear();
                editor.UpdateSelection();
                editor.SelectedPoint = null;

                foreach (var control in controlsCopied)
                {
                    if (control is not GuiSlider || control is GuiSliderTimeline)
                        continue;

                    var horizontal = control.Rect.Width > control.Rect.Height;
                    var xdiff = horizontal ? 12f : 0f;
                    var ydiff = horizontal ? 0f : 12f;

                    var hitbox = new RectangleF(control.Rect.X - xdiff, control.Rect.Y - ydiff, control.Rect.Width + xdiff * 2f, control.Rect.Height + ydiff * 2f);

                    if (control.Visible && hitbox.Contains(pos))
                        control.OnMouseClick(pos, true);
                }
            }
        }

        public virtual void OnMouseUp(Point pos)
        {
            foreach (var control in Controls)
                control.OnMouseUp(pos);

            if (Track != null)
            {
                Track.OnMouseUp(pos);
                Track.DraggingNote = null;
                Track.DraggingPoint = null;
            }

            if (Grid != null)
            {
                Grid.OnMouseUp(pos);
                Grid.Dragging = false;
            }
        }

        public virtual void OnMouseLeave(Point pos)
        {
            foreach (var control in Controls)
                control.OnMouseLeave(pos);
        }

        public virtual void OnMouseMove(Point pos)
        {
            if (Track != null)
            {
                Track.Hovering = Track.Rect.Contains(pos);
                Track.OnMouseMove(pos);
            }

            if (Grid != null)
            {
                var gridrect = Settings.settings["enableQuantum"] ? new RectangleF(Grid.Rect.X - Grid.Rect.Width / 3f, Grid.Rect.Y - Grid.Rect.Height / 3f, Grid.Rect.Width * 5 / 3f, Grid.Rect.Height * 5 / 3f) : Grid.Rect;

                Grid.Hovering = gridrect.Contains(pos);
                Grid.OnMouseMove(pos);
            }
        }

        public virtual void OnResize(Vector2i size)
        {
            var widthdiff = size.X / 1920f;
            var heightdiff = size.Y / 1080f;

            if (BackgroundSquare != null)
            {
                BackgroundSquare.Rect = ResizeRect(BackgroundSquare.OriginRect, widthdiff, heightdiff, false, false);
                BackgroundSquare.Update();
            }

            foreach (var control in Controls)
            {
                control.Rect = ResizeRect(control.OriginRect, widthdiff, heightdiff, control.LockSize, control.MoveWithOffset);
                control.TextSize = (int)(control.OriginTextSize * (control.LockSize ? 1f : heightdiff));

                control.Update();
            }

            if (Track != null)
                Track.Rect = new RectangleF(0, 0, size.X, YOffset);
            if (Grid != null)
                Grid.Rect = new RectangleF(size.X / 2f - Grid.OriginRect.Size.Width / 2f, size.Y / 2f - Grid.OriginRect.Size.Height / 2f, Grid.OriginRect.Size.Width, Grid.OriginRect.Size.Height);
        }

        public virtual void OnButtonClicked(int id)
        {
            buttonClicked = true;
        }

        public virtual void OnKeyDown(Keys key, bool control)
        {
            foreach (var windowControl in Controls)
                windowControl.OnKeyDown(key, control);
        }

        public RectangleF ResizeRect(RectangleF originrect, float width, float height, bool lockSize, bool moveWithOffset)
        {
            var offset = moveWithOffset && MainWindow.Instance.CurrentWindow is GuiWindowEditor ? YOffset : 0;
            var yf = originrect.Y * height + offset;

            var locationWidth = width;

            if (lockSize)
            {
                width = 1f;
                height = 1f;
            }

            return new RectangleF(originrect.X * locationWidth, yf, originrect.Width * width, originrect.Height * height);
        }

        public void Dispose()
        {
            var controlsCopied = Controls.ToList();
            Controls.Clear();

            foreach (var control in controlsCopied)
                control.Dispose();
        }
    }
}
