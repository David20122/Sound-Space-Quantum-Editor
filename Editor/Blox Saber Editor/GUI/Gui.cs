using System.Drawing;
using System.Collections.Generic;
using OpenTK.Input;

namespace Sound_Space_Editor.GUI
{
	class Gui
	{
        public RectangleF rect;

        public List<GuiButton> buttons = new List<GuiButton>();
        public List<GuiCheckbox> checkboxes = new List<GuiCheckbox>();
        public List<GuiSlider> sliders = new List<GuiSlider>();
        public List<GuiTextbox> boxes = new List<GuiTextbox>();
        public List<GuiLabel> labels = new List<GuiLabel>();

        public GuiTrack track;
        public GuiGrid grid;

        public float yoffset = 80;

        protected Gui(float posx, float posy, float sizex, float sizey)
        {
            rect = new RectangleF(posx, posy, sizex, sizey);
        }

        public virtual void Render(float mousex, float mousey, float frametime)
        {
            track?.Render(mousex, mousey, frametime);

            foreach (var button in buttons)
                button.Render(mousex, mousey, frametime);
            foreach (var checkbox in checkboxes)
                checkbox.Render(mousex, mousey, frametime);
            foreach (var slider in sliders)
                slider.Render(mousex, mousey, frametime);
            foreach (var box in boxes)
                box.Render(mousex, mousey, frametime);
            foreach (var label in labels)
                label.Render(mousex, mousey, frametime);

            grid?.Render(mousex, mousey, frametime);
        }

        public virtual void OnMouseClick(Point pos, bool right = false)
        {
            var editor = MainWindow.Instance;

            if (track != null && (track.rect.Contains(pos) || track.hoveringPoint != null || track.draggingNote != null || track.draggingPoint != null))
                track.OnMouseClick(pos, right);

            if (!right)
            {
                var buttonClicked = false;

                foreach (var button in buttons)
                {
                    if (button.Visible && button.rect.Contains(pos))
                    {
                        button.OnMouseClick(pos);
                        OnButtonClicked(button.ID);
                        buttonClicked = true;
                    }
                }

                foreach (var checkbox in checkboxes)
                    if (checkbox.Visible && checkbox.rect.Contains(pos))
                        checkbox.OnMouseClick(pos);

                if (!buttonClicked)
                {
                    foreach (var slider in sliders)
                    {
                        var horizontal = slider.rect.Width > slider.rect.Height;
                        var xdiff = horizontal ? 12f : 0f;
                        var ydiff = horizontal ? 0f : 12f;

                        var hitbox = new RectangleF(slider.rect.X - xdiff, slider.rect.Y - ydiff, slider.rect.Width + xdiff * 2f, slider.rect.Height + ydiff * 2f);

                        if (slider.Visible && hitbox.Contains(pos))
                        {
                            slider.OnMouseClick(pos);
                            slider.dragging = !(slider is GuiSliderTimeline s) || s.hoveringBookmark == null;
                        }
                    }
                }

                foreach (var box in boxes)
                {
                    if (box.Visible && box.rect.Contains(pos))
                    {
                        box.OnMouseClick(pos);
                        buttonClicked = true;
                    }
                    else
                        box.focused = false;
                }

                var gridrect = grid == null ? new RectangleF() :
                    (Settings.settings["enableQuantum"] ? new RectangleF(grid.rect.X - grid.rect.Width / 3f, grid.rect.Y - grid.rect.Height / 3f, grid.rect.Width * 5 / 3f, grid.rect.Height * 5 / 3f) : grid.rect);

                if (!buttonClicked && grid != null && gridrect.Contains(pos))
                    grid.OnMouseClick(pos);
                else if (!buttonClicked && track != null && !track.rect.Contains(pos))
                {
                    editor.SelectedNotes.Clear();
                    if (track.hoveringPoint == null)
                        editor.SelectedPoint = null;
                }

            }
            else
            {
                editor.SelectedNotes.Clear();
                editor.SelectedPoint = null;
            }
        }

        public virtual void OnMouseUp(Point pos, bool right = false)
        {
            foreach (var slider in sliders)
                slider.dragging = false;

            if (track != null)
            {
                track.OnMouseUp(pos);
                track.draggingNote = null;
                track.draggingPoint = null;
            }

            if (grid != null)
            {
                grid?.OnMouseUp(pos);
                grid.dragging = false;
            }
        }

        public virtual void OnMouseLeave(Point pos)
        {
            foreach (var slider in sliders)
                slider.dragging = false;
        }

        public virtual void OnMouseMove(Point pos)
        {
            foreach (var button in buttons)
                button.hovering = button.rect.Contains(pos);
            foreach (var slider in sliders)
                slider.hovering = slider.rect.Contains(pos);

            if (track != null)
            {
                track.hovering = track.rect.Contains(pos);
                track.OnMouseMove(pos);
            }

            if (grid != null)
            {
                var gridrect = Settings.settings["enableQuantum"] ? new RectangleF(grid.rect.X - grid.rect.Width / 3f, grid.rect.Y - grid.rect.Height / 3f, grid.rect.Width * 5 / 3f, grid.rect.Height * 5 / 3f) : grid.rect;

                grid.hovering = gridrect.Contains(pos);
                grid.OnMouseMove(pos);
            }
        }

        public virtual void OnResize(Size size)
        {
            var widthdiff = size.Width / 1920f;
            var heightdiff = size.Height / 1080f;

            foreach (var button in buttons)
            {
                button.rect = ResizeRect(button.originRect, widthdiff, heightdiff, button.lockSize, button.moveWithOffset);
                button.textSize = (int)(button.originTextSize * (button.lockSize ? 1f : heightdiff));
            }
            foreach (var checkbox in checkboxes)
            {
                checkbox.rect = ResizeRect(checkbox.originRect, widthdiff, heightdiff, checkbox.lockSize, checkbox.moveWithOffset);
                checkbox.textSize = (int)(checkbox.originTextSize * (checkbox.lockSize ? 1f : heightdiff));
            }
            foreach (var slider in sliders)
                slider.rect = ResizeRect(slider.originRect, widthdiff, heightdiff, slider.lockSize, slider.moveWithOffset);
            foreach (var box in boxes)
            {
                box.rect = ResizeRect(box.originRect, widthdiff, heightdiff, box.lockSize, box.moveWithOffset);
                box.textSize = (int)(box.originTextSize * (box.lockSize ? 1f : heightdiff));
            }
            foreach (var label in labels)
            {
                label.rect = ResizeRect(label.originRect, widthdiff, heightdiff, label.lockSize, label.moveWithOffset);
                label.textSize = (int)(label.originTextSize * (label.lockSize ? 1f : heightdiff));
            }

            if (track != null)
                track.rect = new RectangleF(0, 0, size.Width, yoffset);
            if (grid != null)
                grid.rect = new RectangleF(size.Width / 2f - grid.originRect.Size.Width / 2f, size.Height / 2f - grid.originRect.Size.Height / 2f, grid.originRect.Size.Width, grid.originRect.Size.Height);
        }

        protected virtual void OnButtonClicked(int id)
        {

        }

        public virtual void OnKeyDown(Key key, bool control)
        {
            foreach (var box in boxes)
                box.OnKeyDown(key, control);
        }

        public virtual void OnKeyTyped(char key)
        {
            foreach (var box in boxes)
                box.OnKeyTyped(key);
        }

        public virtual void OnClosing()
        {

        }

        public void RenderText(string text, float posx, float posy, int size, string font = "main")
        {
            var renderer = MainWindow.Instance.Fonts[font];

            renderer.Render(text, posx, posy, size);
        }

        public int TextWidth(string text, int size, string font = "main")
        {
            var renderer = MainWindow.Instance.Fonts[font];

            return renderer.GetWidth(text, size);
        }

        public int TextHeight(int size, string font = "main")
        {
            var renderer = MainWindow.Instance.Fonts[font];

            return renderer.GetHeight(size);
        }

        public RectangleF ResizeRect(RectangleF originrect, float width, float height, bool lockSize = false, bool moveWithOffset = false)
        {
            var offset = moveWithOffset && MainWindow.Instance.CurrentWindow is GuiWindowEditor ? yoffset : 0;
            var yf = originrect.Y * height + offset;

            var locationWidth = width;

            if (lockSize)
            {
                width = 1f;
                height = 1f;
            }

            return new RectangleF(originrect.X * locationWidth, yf, originrect.Width * width, originrect.Height * height);
        }
    }
}