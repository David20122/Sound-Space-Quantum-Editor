using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Linq;
using Sound_Space_Editor.Misc;

namespace Sound_Space_Editor.GUI
{
	class GuiGrid : Gui
	{
        public Note hoveringNote;
        public Note draggingNote;

        public bool hovering;
        public bool dragging;

        public RectangleF originRect;

        private readonly Note startNote = new Note(1f, 1f, 0);

        private PointF lastPlaced;
        private PointF lastPos;
        private Vector2 startPos;

        public GuiGrid(float sizex, float sizey) : base(0f, 0f, sizex, sizey)
        {
            originRect = new RectangleF(0, 0, sizex, sizey);
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            var approachRate = (Settings.settings["approachRate"].Value + 1f) / 10f;
            var gridDim = Settings.settings["gridOpacity"];

            var approachSquares = Settings.settings["approachSquares"];
            var gridNumbers = Settings.settings["gridNumbers"];
            var separateClickTools = Settings.settings["separateClickTools"];
            var selectTool = Settings.settings["selectTool"];

            GL.Color4(Color.FromArgb(gridDim, 36, 35, 33));
            GLSpecial.Rect(rect);

            var cellSize = rect.Width / 3f;
            var noteSize = cellSize * 0.75f;
            var cellGap = (cellSize - noteSize) / 2f;

            var currentTime = Settings.settings["currentTime"].Value;
            var quantumLines = Settings.settings["quantumGridLines"];

            GL.Color3(0.2f, 0.2f, 0.2f);
            GLSpecial.Outline(rect);

            if (quantumLines)
                GL.Color3(0.05f, 0.05f, 0.05f);

            for (int i = 1; i < 3; i++)
            {
                var x = rect.X + rect.Width / 3f * i;
                var y = rect.Y + rect.Height / 3f * i;

                GLSpecial.Line(x, rect.Y, x, rect.Y + rect.Height);
                GLSpecial.Line(rect.X, y, rect.X + rect.Width, y);
            }

            //render grid lines
            if (quantumLines)
            {
                GL.Color3(0.2f, 0.2f, 0.2f);

                var divisor = Settings.settings["quantumSnapping"].Value + 3f;
                var offset = Math.Round(divisor) % 2 == 0 ? 0.5f : 1f;

                for (int i = (int)(2 * offset); i <= divisor; i++)
                {
                    var x = rect.X + rect.Width / divisor * (i - offset);
                    var y = rect.Y + rect.Height / divisor * (i - offset);

                    GLSpecial.Line(x, rect.Y, x, rect.Y + rect.Height);
                    GLSpecial.Line(rect.X, y, rect.X + rect.Width, y);
                }
            }

            //render grid letters
            if (Settings.settings["gridLetters"])
            {
                // kept breaking for some reason
                try
                {
                    var copy = new Dictionary<Key, Tuple<int, int>>(MainWindow.Instance.KeyMapping);

                    foreach (var key in copy)
                    {
                        var letter = key.Key.ToString().Replace("Keypad", "");
                        var tuple = key.Value;

                        var x = rect.X + tuple.Item1 * cellSize + cellSize / 2f;
                        var y = rect.Y + tuple.Item2 * cellSize + cellSize / 2f;

                        var width = TextWidth(letter, 38);
                        var height = TextHeight(38);

                        RenderText(letter, x - width / 2f, y - height / 2f, 38);
                    }
                }
                catch { }
            }

            //render notes
            GL.LineWidth(2f);

            Note last = null;
            Note next = null;

            bool isHoveringNote = false;

            for (var i = 0; i < MainWindow.Instance.Notes.Count; i++)
            {
                var note = MainWindow.Instance.Notes[i];
                var passed = currentTime > note.Ms + 1;
                var visible = !passed && note.Ms - currentTime <= 1000f / approachRate;

                if (passed)
                    last = note;
                else if (next == null)
                    next = note;

                if (!visible)
                {
                    if (passed && next != null)
                        break;
                    continue;
                }

                var x = rect.X + note.X * cellSize + cellGap;
                var y = rect.Y + note.Y * cellSize + cellGap;

                var progress = Math.Min(1f, (float)Math.Pow(1 - Math.Min(1f, (note.Ms - currentTime) * approachRate / 750f), 2f));
                var noteRect = new RectangleF(x, y, noteSize, noteSize);

                GL.Color4(Color.FromArgb((int)(progress * 0.15f * 255f), note.GridColor));
                GLSpecial.Rect(noteRect);
                GL.Color4(Color.FromArgb((int)(progress * 255f), note.GridColor));
                GLSpecial.Outline(noteRect);

                if (approachSquares)
                {
                    var outlineSize = 4 + noteSize + noteSize * (1 - progress) * 2;

                    GLSpecial.Outline(x - outlineSize / 2f + noteSize / 2f, y - outlineSize / 2f + noteSize / 2f, outlineSize, outlineSize);
                }

                if (gridNumbers)
                {
                    GL.Color4(1f, 1f, 1f, progress);

                    var numText = $"{i + 1:##,###}";
                    var width = TextWidth(numText, 24);
                    var height = TextHeight(24);

                    RenderText(numText, noteRect.X + noteRect.Width / 2f - width / 2f, noteRect.Y + noteRect.Height / 2f - height / 2f, 24);
                }

                if (MainWindow.Instance.SelectedNotes.Contains(note))
                {
                    var outlineSize = noteSize + 8f;

                    GL.Color4(0f, 0.5f, 1f, progress);
                    GLSpecial.Outline(x - outlineSize / 2f + noteSize / 2f, y - outlineSize / 2f + noteSize / 2f, outlineSize, outlineSize);
                }

                if (!isHoveringNote && noteRect.Contains(mousex, mousey) && (!separateClickTools || selectTool))
                {
                    hoveringNote = note;
                    isHoveringNote = true;

                    GL.Color3(0f, 1f, 0.25f);
                    GLSpecial.Outline(x - 4, y - 4, noteSize + 8, noteSize + 8);
                }
            }

            if (!isHoveringNote)
                hoveringNote = null;

            //render autoplay cursor
            if (Settings.settings["autoplay"])
            {
                if (last == null)
                    last = startNote;
                if (next == null)
                    next = last;

                var timeDiff = next.Ms - last.Ms;
                var timePos = currentTime - last.Ms;

                var progress = timeDiff == 0 ? 1 : (float)timePos / timeDiff;
                progress = (float)Math.Sin(progress * MathHelper.PiOver2);

                var width = (float)Math.Sin(progress * MathHelper.Pi) * 8f + 16;

                var lx = rect.X + last.X * cellSize;
                var ly = rect.Y + last.Y * cellSize;

                var nx = rect.X + next.X * cellSize;
                var ny = rect.Y + next.Y * cellSize;

                var x = cellSize / 2f + lx + (nx - lx) * progress;
                var y = cellSize / 2f + ly + (ny - ly) * progress;

                var cx = x - width / 2f;
                var cy = y - width / 2f;

                GL.Color4(1f, 1f, 1f, 0.25f);
                GLSpecial.Rect(cx, cy, width, width);

                GL.Color4(1f, 1f, 1f, 1f);
                GLSpecial.Outline(cx, cy, width, width);
            }

            //render fake note
            if (hovering && (!separateClickTools || !selectTool) && (!isHoveringNote || separateClickTools))
                RenderPreviewNote(mousex, mousey, null, true);
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            var separateClickTools = Settings.settings["separateClickTools"];
            var selectTool = Settings.settings["selectTool"];

            dragging = hovering && (hoveringNote != null || (!separateClickTools || !selectTool));

            if (dragging)
            {
                var editor = MainWindow.Instance;

                if (hoveringNote == null || (separateClickTools && !selectTool))
                {
                    var gridPos = editor.PointToGridSpace(pos.X, pos.Y);
                    var ms = editor.GetClosestBeat(Settings.settings["currentTime"].Value);
                    var note = new Note(gridPos.X, gridPos.Y, (long)(ms >= 0 ? ms : Settings.settings["currentTime"].Value));

                    editor.UndoRedoManager.Add("ADD NOTE", () =>
                    {
                        editor.Notes.Remove(note);
                        editor.SortNotes();
                    }, () =>
                    {
                        editor.Notes.Add(note);
                        editor.SortNotes();
                    });

                    if (Settings.settings["autoAdvance"])
                        editor.Advance();

                    lastPlaced = gridPos;
                }
                else if (hoveringNote != null)
                {
                    draggingNote = hoveringNote;
                    lastPos = new PointF(hoveringNote.X, hoveringNote.Y);
                    startPos = new Vector2(hoveringNote.X, hoveringNote.Y);

                    var selected = editor.SelectedNotes.ToList();

                    if (editor.shiftHeld)
                    {
                        selected = new List<Note> { selected[0] };

                        var first = selected[0];
                        var last = hoveringNote;
                        var min = Math.Min(first.Ms, last.Ms);
                        var max = Math.Max(first.Ms, last.Ms);

                        foreach (var note in editor.Notes)
                            if (note.Ms >= min && note.Ms <= max && !selected.Contains(note))
                                selected.Add(note);
                    }
                    else if (editor.ctrlHeld)
                    {
                        if (selected.Contains(hoveringNote))
                            selected.Remove(hoveringNote);
                        else
                            selected.Add(hoveringNote);
                    }
                    else if (!selected.Contains(hoveringNote))
                        selected = new List<Note>() { hoveringNote };

                    editor.SelectedNotes = selected.ToList();
                }
            }
        }

        public override void OnMouseMove(Point pos)
        {
            if (dragging)
            {
                var editor = MainWindow.Instance;

                if (draggingNote == null)
                {
                    var gridPos = editor.PointToGridSpace(pos.X, pos.Y);

                    if (gridPos != lastPlaced)
                    {
                        var ms = editor.GetClosestBeat(Settings.settings["currentTime"].Value);
                        var note = new Note(gridPos.X, gridPos.Y, (long)(ms >= 0 ? ms : Settings.settings["currentTime"].Value));

                        editor.UndoRedoManager.Add("ADD NOTE", () =>
                        {
                            editor.Notes.Remove(note);
                            editor.SortNotes();
                        }, () =>
                        {
                            editor.Notes.Add(note);
                            editor.SortNotes();
                        });

                        if (Settings.settings["autoAdvance"])
                            editor.Advance();

                        lastPlaced = gridPos;
                    }
                }
                else
                {
                    var newPos = editor.PointToGridSpace(pos.X, pos.Y);

                    if (newPos != lastPos)
                    {
                        var bounds = Settings.settings["enableQuantum"] ? new Vector2(-0.85f, 2.85f) : new Vector2(0, 2);

                        var xDiff = newPos.X - draggingNote.X;
                        var yDiff = newPos.Y - draggingNote.Y;

                        var maxX = draggingNote.X;
                        var minX = draggingNote.X;
                        var maxY = draggingNote.Y;
                        var minY = draggingNote.Y;

                        foreach (var note in editor.SelectedNotes)
                        {
                            maxX = Math.Max(note.X, maxX);
                            minX = Math.Min(note.X, minX);
                            maxY = Math.Max(note.Y, maxY);
                            minY = Math.Min(note.Y, minY);
                        }

                        xDiff = Math.Max(bounds.X, minX + xDiff) - minX;
                        xDiff = Math.Min(bounds.Y, maxX + xDiff) - maxX;
                        yDiff = Math.Max(bounds.X, minY + yDiff) - minY;
                        yDiff = Math.Min(bounds.Y, maxY + yDiff) - maxY;

                        foreach (var note in editor.SelectedNotes)
                        {
                            note.X += xDiff;
                            note.Y += yDiff;
                        }

                        lastPos = newPos;
                    }
                }
            }
        }

        public override void OnMouseUp(Point pos, bool right = false)
        {
            if (draggingNote != null && new Vector2(draggingNote.X, draggingNote.Y) != startPos)
            {
                var editor = MainWindow.Instance;
                var selected = editor.SelectedNotes.ToList();
                var oldPos = new List<Vector2>();
                var newPos = new List<Vector2>();

                var posDiff = new Vector2(draggingNote.X, draggingNote.Y) - startPos;

                for (int i = 0; i < selected.Count; i++)
                {
                    var xy = new Vector2(selected[i].X, selected[i].Y);

                    oldPos.Add(xy - posDiff);
                    newPos.Add(xy);
                }

                editor.UndoRedoManager.Add($"MOVE NOTE{(selected.Count > 1 ? "S" : "")}", () =>
                {
                    for (int i = 0; i < selected.Count; i++)
                    {
                        selected[i].X = oldPos[i].X;
                        selected[i].Y = oldPos[i].Y;
                    }
                }, () =>
                {
                    for (int i = 0; i < selected.Count; i++)
                    {
                        selected[i].X = newPos[i].X;
                        selected[i].Y = newPos[i].Y;
                    }
                }, false);
            }

            dragging = false;
            draggingNote = null;
            lastPlaced = new PointF();
        }

        public void RenderPreviewNote(float x, float y, Color? color, bool Mouse = false)
        {
            color = color ?? Color.FromArgb(128, 128, 128);
            var cellSize = rect.Width / 3f;
            var noteSize = cellSize * 0.65f;
            var cellGap = (cellSize - noteSize) / 2f;

            var pos = Mouse ? MainWindow.Instance.PointToGridSpace(x, y) : new PointF(x, y);
            x = rect.X + pos.X * cellSize + cellGap;
            y = rect.Y + pos.Y * cellSize + cellGap;

            var noteRect = new RectangleF(x, y, noteSize, noteSize);

            GL.Color4(Color.FromArgb(24, (Color)color));
            GLSpecial.Rect(noteRect);
            GL.Color4(Color.FromArgb(32, (Color)color));
            GLSpecial.Outline(noteRect);
        }
    }
}