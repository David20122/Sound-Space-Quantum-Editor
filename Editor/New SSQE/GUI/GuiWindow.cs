using System.Drawing;
using New_SSQE.GUI.Font;
using New_SSQE.GUI.Shaders;
using New_SSQE.Maps;
using New_SSQE.Preferences;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New_SSQE.GUI
{
    internal class GuiWindow
    {
        public RectangleF Rect;

        public List<WindowControl> Controls = new();
        private readonly Dictionary<string, List<WindowControl>> FontSet = new()
        {
            {"main", new List<WindowControl>() },
            {"square", new List<WindowControl>() },
            {"squareo", new List<WindowControl>() },
            {"other", new List<WindowControl>() }
        };

        public GuiTrack? Track;
        public GuiGrid? Grid;
        public GuiSquareBackground? BackgroundSquare;

        public float YOffset = 80;

        private bool buttonClicked = false;

        protected GuiWindow(float x, float y, float w, float h)
        {
            Rect = new RectangleF(x, y, w, h);
        }

        protected void Init()
        {
            foreach (WindowControl control in Controls)
            {
                if (control.Font != null && FontSet.TryGetValue(control.Font, out List<WindowControl>? value))
                    value.Add(control);
                else
                    FontSet["other"].Add(control);
            }
        }

        public virtual void Render(float mousex, float mousey, float frametime)
        {
            List<WindowControl> controlsCopied = Controls.ToList();
            Dictionary<string, List<WindowControl>> subcontrolsCopied = new(FontSet);

            GL.UseProgram(Shader.Program);
            BackgroundSquare?.Render(mousex, mousey, frametime);
            GL.UseProgram(Shader.TextureProgram);
            BackgroundSquare?.RenderTexture();
            GL.UseProgram(Shader.Program);

            if (GuiGrid.RenderMapObjects)
                Grid?.Render(mousex, mousey, frametime);

            foreach (WindowControl control in controlsCopied)
                if (control.Visible && !control.IsDisposed)
                    control.Render(mousex, mousey, frametime);

            if (!GuiGrid.RenderMapObjects)
                Grid?.Render(mousex, mousey, frametime);
            Track?.Render(mousex, mousey, frametime);

            ProgramHandle prog = FontRenderer.unicode ? Shader.UnicodeProgram : Shader.FontProgram;
            GL.UseProgram(prog);
            WindowControl.TexColorLocation = GL.GetUniformLocation(prog, "TexColor");

            FontRenderer.SetActive("main");
            foreach (WindowControl control in subcontrolsCopied["main"])
                if (control.Visible && !control.IsDisposed)
                    control.RenderTexture();

            Track?.RenderTexture();
            Grid?.RenderTexture();

            FontRenderer.SetActive("square");
            foreach (WindowControl control in subcontrolsCopied["square"])
                if (control.Visible && !control.IsDisposed)
                    control.RenderTexture();

            FontRenderer.SetActive("squareo");
            foreach (WindowControl control in subcontrolsCopied["squareo"])
                if (control.Visible && !control.IsDisposed)
                    control.RenderTexture();

            GL.UseProgram(Shader.TextureProgram);

            foreach (WindowControl control in subcontrolsCopied["other"])
                if (control.Visible && !control.IsDisposed)
                    control.RenderTexture();
        }

        public virtual void OnMouseClick(Point pos, bool right)
        {
            MainWindow editor = MainWindow.Instance;

            if (Track != null && (Track.Rect.Contains(pos) || Track.HoveringPoint != null || Track.DraggingNote != null || Track.DraggingPoint != null || Track.DraggingVfx != null || Track.DraggingSpec != null))
                Track.OnMouseClick(pos, right);

            List<WindowControl> controlsCopied = Controls.ToList();

            if (!right)
            {
                buttonClicked = false;

                for (int i = controlsCopied.Count; i > 0; i--)
                {
                    WindowControl control = controlsCopied[i - 1];
                    
                    if (!buttonClicked && control.Visible && control.Rect.Contains(pos))
                        control.OnMouseClick(pos, false);
                    else if (control is GuiTextbox box)
                        box.Focused = false;
                }

                RectangleF gridRect = Grid == null ? new() :
                    (Settings.enableQuantum.Value ? new(Grid.Rect.X - Grid.Rect.Width / 3f, Grid.Rect.Y - Grid.Rect.Height / 3f, Grid.Rect.Width * 5 / 3f, Grid.Rect.Height * 5 / 3f) : Grid.Rect);

                if (!buttonClicked && Grid != null && (gridRect.Contains(pos) || Grid.HoveringNote != null) && Track?.Rect.Contains(pos) != true)
                    Grid.OnMouseClick(pos);
                else if (!buttonClicked && Track != null && !Track.Rect.Contains(pos))
                {
                    CurrentMap.Notes.Selected = new();
                    if (Track.HoveringPoint == null)
                        CurrentMap.SelectedPoint = null;
                }
            }
            else
            {
                buttonClicked = false;

                for (int i = controlsCopied.Count; i > 0; i--)
                {
                    WindowControl control = controlsCopied[i - 1];

                    if (!buttonClicked && control.Visible && control.Rect.Contains(pos) && control is GuiButtonList)
                        control.OnMouseClick(pos, true);
                }

                if (!buttonClicked && Grid != null && Grid.HoveringNote != null)
                {
                    Grid.OnMouseClick(pos, right);
                    buttonClicked = true;
                }
                
                if (!buttonClicked)
                {
                    CurrentMap.ClearSelection();

                    foreach (WindowControl control in controlsCopied)
                    {
                        if (control is not GuiSlider || control is GuiSliderTimeline)
                            continue;

                        bool horizontal = control.Rect.Width > control.Rect.Height;
                        float xdiff = horizontal ? 12f : 0f;
                        float ydiff = horizontal ? 0f : 12f;

                        RectangleF hitbox = new(control.Rect.X - xdiff, control.Rect.Y - ydiff, control.Rect.Width + xdiff * 2f, control.Rect.Height + ydiff * 2f);

                        if (control.Visible && hitbox.Contains(pos))
                            control.OnMouseClick(pos, true);
                    }
                }
            }
        }

        public virtual void OnMouseUp(Point pos)
        {
            foreach (WindowControl control in Controls)
                control.OnMouseUp(pos);

            if (Track != null)
            {
                Track.OnMouseUp(pos);
                Track.DraggingNote = null;
                Track.DraggingPoint = null;
                Track.DraggingVfx = null;
                Track.DraggingSpec = null;
            }

            if (Grid != null)
            {
                Grid.OnMouseUp(pos);
                Grid.Dragging = false;
            }
        }

        public virtual void OnMouseLeave(Point pos)
        {
            foreach (WindowControl control in Controls)
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
                RectangleF gridrect = Settings.enableQuantum.Value ? new(Grid.Rect.X - Grid.Rect.Width / 3f, Grid.Rect.Y - Grid.Rect.Height / 3f, Grid.Rect.Width * 5 / 3f, Grid.Rect.Height * 5 / 3f) : Grid.Rect;

                Grid.Hovering = gridrect.Contains(pos);
                Grid.OnMouseMove(pos);
            }
        }

        public virtual void OnResize(Vector2i size)
        {
            Rect = new(0, 0, size.X, size.Y);

            float widthdiff = size.X / 1920f;
            float heightdiff = size.Y / 1080f;
            float textMult = Math.Min(widthdiff, heightdiff);

            if (BackgroundSquare != null)
            {
                BackgroundSquare.Rect = ResizeRect(BackgroundSquare.OriginRect, widthdiff, heightdiff, false, false);
                BackgroundSquare.Update();
            }

            foreach (WindowControl control in Controls)
            {
                control.Rect = ResizeRect(control.OriginRect, widthdiff, heightdiff, control.LockSize, control.MoveWithOffset);
                control.TextSize = (int)(control.OriginTextSize * (control.LockSize ? 1f : textMult));

                control.Update();
            }

            if (Track != null)
                Track.Rect = new(0, 0, size.X, YOffset);
            if (Grid != null)
                Grid.Rect = new(size.X / 2f - Grid.OriginRect.Size.Width / 2f, size.Y / 2f - Grid.OriginRect.Size.Height / 2f, Grid.OriginRect.Size.Width, Grid.OriginRect.Size.Height);
        }

        public virtual void OnButtonClicked(int id)
        {
            buttonClicked = true;
        }

        public virtual void OnKeyDown(Keys key, bool control)
        {
            foreach (WindowControl windowControl in Controls)
                windowControl.OnKeyDown(key, control);
        }

        public RectangleF ResizeRect(RectangleF originrect, float width, float height, bool lockSize, bool moveWithOffset)
        {
            float offset = moveWithOffset && MainWindow.Instance.CurrentWindow is GuiWindowEditor ? YOffset : 0;
            float yf = originrect.Y * height + offset;

            float locationWidth = width;

            if (lockSize)
            {
                width = 1f;
                height = 1f;
            }

            return new(originrect.X * locationWidth, yf, originrect.Width * width, originrect.Height * height);
        }

        public void Dispose()
        {
            List<WindowControl> controlsCopied = Controls.ToList();
            Controls.Clear();

            foreach (WindowControl control in controlsCopied)
                control.Dispose();
        }
    }
}
