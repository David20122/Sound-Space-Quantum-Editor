using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using AvaloniaEdit.Utils;

namespace New_SSQE.GUI
{
    internal class GuiGrid : WindowControl
    {
        public Note? HoveringNote;
        public Note? DraggingNote;

        public bool Hovering;
        public bool Dragging;

        private readonly Note startNote = new(1f, 1f, 0);

        private PointF lastPlaced;
        private PointF lastPos;
        private Vector2 startPos;
        private RectangleF prevRect;

        private readonly Dictionary<string, int> Indices = new()
        {
            {"rectLength", 0 },
            {"loopLength", 0 },
            {"lineLength", 0 },
        };

        public GuiGrid(float sizex, float sizey) : base(0f, 0f, sizex, sizey)
        {
            Dynamic = true;
            prevRect = Rect;

            InstanceSetup();
            Init();
        }

        public override void InstanceSetup()
        {
            ClearBuffers();

            VaOs = new int[5];
            VbOs = new int[10];
            VertexCounts = new int[5];

            var noteVerts = GLU.OutlineAsTriangles(0, 0, 75, 75, 2, 1f, 1f, 1f, 1f);
            var fillVerts = GLU.Rect(0, 0, 75, 75, 1f, 1f, 1f, 0.15f);
            noteVerts = noteVerts.Concat(fillVerts).ToArray();

            var previewVerts = GLU.OutlineAsTriangles(0, 0, 65, 65, 2, 1f, 1f, 1f, 0.09375f);
            var previewFillVerts = GLU.Rect(0, 0, 65, 65, 1f, 1f, 1f, 0.125f);
            previewVerts = previewVerts.Concat(previewFillVerts).ToArray();

            var approachVerts = GLU.OutlineAsTriangles(0, 0, 1, 1, 0.0125f, 1f, 1f, 1f, 1f);
            var selectVerts = GLU.OutlineAsTriangles(0, 0, 83, 83, 2, 1f, 1f, 1f, 1f);
            var hoverVerts = GLU.OutlineAsTriangles(0, 0, 83, 83, 2, 1f, 1f, 1f, 0.25f);

            AddToBuffers(noteVerts, 0);
            AddToBuffers(approachVerts, 1);
            AddToBuffers(selectVerts, 2);
            AddToBuffers(hoverVerts, 3);
            AddToBuffers(previewVerts, 4);
        }

        private readonly List<Vector4> previewNoteOffsets = new();
        private readonly List<float> bezierVertices = new();

        public override void GenerateOffsets()
        {
            var editor = MainWindow.Instance;
            var mouse = editor.Mouse;

            var cellSize = Rect.Width / 3f;
            var noteSize = cellSize * 0.75f;
            var cellGap = (cellSize - noteSize) / 2f;

            var currentTime = Settings.settings["currentTime"].Value;
            var approachRate = (Settings.settings["approachRate"].Value + 1f) / 10f;

            var approachSquares = Settings.settings["approachSquares"];
            var gridNumbers = Settings.settings["gridNumbers"];
            var noteColors = Settings.settings["noteColors"];

            var separateClickTools = Settings.settings["separateClickTools"];
            var selectTool = Settings.settings["selectTool"];

            var approachOffsets = new List<Vector4>();
            var noteOffsets = new List<Vector4>();
            var selectOffsets = new List<Vector4>();
            var hoverOffset = new Vector4(-1920, 0, 0, 0);

            var isHoveringNote = false;
            Note? nextNote = null;

            for (int i = 0; i < editor.Notes.Count; i++)
            {
                var note = editor.Notes[i];
                var passed = currentTime > note.Ms + 1;
                var visible = !passed && note.Ms - currentTime <= 1000f / approachRate;

                if (!passed)
                    nextNote ??= note;

                if (!visible)
                {
                    if (passed && nextNote != null)
                        break;
                    continue;
                }

                var x = Rect.X + note.X * cellSize + cellGap;
                var y = Rect.Y + note.Y * cellSize + cellGap;

                var progress = Math.Min(1f, (float)Math.Pow(1 - Math.Min(1f, (note.Ms - currentTime) * approachRate / 750f), 2f));
                var noteRect = new RectangleF(x, y, noteSize, noteSize);

                var color = noteColors[i % noteColors.Count];
                var c = new float[] { color.R / 255f, color.G / 255f, color.B / 255f };
                Vector4 vec = (2 * (int)x + c[0], 2 * (int)y + c[1], 2f + c[2], 2f + progress);

                if (approachSquares)
                {
                    var outlineSize = 4 + noteSize + noteSize * (1 - progress) * 2 + 0.5f;

                    approachOffsets.Add((2 * (int)(x - outlineSize / 2f + noteSize / 2f + 0.5f) + c[0], 2 * (int)(y - outlineSize / 2f + noteSize / 2f + 0.5f)
                                        + c[1], 2 * (int)outlineSize + c[2], 2 * (int)outlineSize + progress));
                }

                if (note.Selected)
                    selectOffsets.Add((2 * (int)(x - 4) + 0f, 2 * (int)(y - 4) + 0.5f, 2 + 1f, 2 + progress));

                if (!isHoveringNote && noteRect.Contains(mouse) && (!separateClickTools || selectTool))
                {
                    HoveringNote = note;
                    hoverOffset = (2 * (int)(x - 4) + 0f, 2 * (int)(y - 4) + 1f, 2 + 0.25f, 2 + 1f);
                    isHoveringNote = true;
                }

                noteOffsets.Add(vec);

                if (gridNumbers)
                {
                    var numText = $"{i + 1:##,###}";
                    var width = FontRenderer.GetWidth(numText, 24, "main");
                    var height = FontRenderer.GetHeight(24, "main");

                    color2Texts.AddRange(FontRenderer.Print((int)(noteRect.X + noteRect.Width / 2f - width / 2f), (int)(noteRect.Y + noteRect.Height / 2f - height / 2f + 3f),
                        numText, 24, "main"));
                    for (int j = 0; j < numText.Length; j++)
                        alphas.Add(1 - progress);
                }
            }

            if (!isHoveringNote)
                HoveringNote = null;
            
            //render fake note
            if (Hovering && (!separateClickTools || !selectTool) && (HoveringNote == null || separateClickTools))
                AddPreviewNote(mouse.X, mouse.Y, Color.FromArgb(127, 127, 127), true);

            RegisterData(0, noteOffsets.ToArray());
            RegisterData(1, approachOffsets.ToArray());
            RegisterData(2, selectOffsets.ToArray());
            RegisterData(3, new Vector4[1] { hoverOffset });
            RegisterData(4, previewNoteOffsets.ToArray());
        }

        private int offset;

        public override void Render(float mousex, float mousey, float frametime)
        {
            Update();

            // render background
            GL.UseProgram(Shader.Program);

            GL.BindVertexArray(VaO);
            offset = Indices["rectLength"] + Indices["loopLength"] + Indices["lineLength"];
            GL.DrawArrays(PrimitiveType.Triangles, 0, offset);

            // render keybinds
            GL.UseProgram(Shader.FontTexProgram);
            FontRenderer.SetActive("main");

            GL.Uniform4(TexColorLocation, 0.2f, 0.2f, 0.2f, 1f);
            FontRenderer.RenderData("main", color1Texts.ToArray());

            // render dynamic elements
            GL.UseProgram(Shader.GridInstancedProgram);

            GenerateOffsets();

            // undo program switch
            GL.UseProgram(Shader.Program);
        }

        public override void RenderTexture()
        {
            GL.Uniform4(TexColorLocation, 1f, 1f, 1f, 1f);
            FontRenderer.RenderData("main", color2Texts.ToArray(), alphas.ToArray());

            // layer bezier preview and autoplay cursor on top
            GL.UseProgram(Shader.Program);

            GL.BindVertexArray(VaO);
            GL.DrawArrays(PrimitiveType.LineStrip, offset, Indices["bezierLength"]);
            offset += Indices["bezierLength"];
            GL.DrawArrays(PrimitiveType.Triangles, offset, 6);
            GL.DrawArrays(PrimitiveType.TriangleStrip, offset + 6, 24);

            GL.UseProgram(Shader.FontTexProgram);
        }

        private List<float> rects = new();
        private List<float> loops = new();
        private List<float> lines = new();
        private List<Vector4> color1Texts = new();
        private List<Vector4> color2Texts = new();
        private List<float> alphas = new();

        public override Tuple<float[], float[]> GetVertices()
        {
            rects = new();
            loops = new();
            lines = new();
            color1Texts = new();
            color2Texts = new();
            alphas = new();

            var editor = MainWindow.Instance;

            var cellSize = Rect.Width / 3f;

            var currentTime = Settings.settings["currentTime"].Value;
            var approachRate = (Settings.settings["approachRate"].Value + 1f) / 10f;
            var quantumLines = Settings.settings["quantumGridLines"];

            rects.AddRange(GLU.Rect(Rect, 0.15f, 0.15f, 0.15f, Settings.settings["gridOpacity"] / 255f));
            loops.AddRange(GLU.OutlineAsTriangles(Rect, 1, 0.2f, 0.2f, 0.2f));

            var lineColor = quantumLines ? new float[] { 0.05f, 0.05f, 0.05f } : new float[] { 0.2f, 0.2f, 0.2f };

            for (int i = 1; i < 3; i++)
            {
                var x = Rect.X + Rect.Width / 3f * i;
                var y = Rect.Y + Rect.Height / 3f * i;

                lines.AddRange(GLU.Line(x, Rect.Y, x, Rect.Y + Rect.Height, 1, lineColor));
                lines.AddRange(GLU.Line(Rect.X, y, Rect.X + Rect.Width, y, 1, lineColor));
            }

            //render grid lines
            if (quantumLines)
            {
                var divisor = Settings.settings["quantumSnapping"].Value + 3f;
                var offset = Math.Round(divisor) % 2 == 0 ? 0.5f : 1f;

                for (int i = (int)(2 * offset); i <= divisor; i++)
                {
                    var x = Rect.X + Rect.Width / divisor * (i - offset);
                    var y = Rect.Y + Rect.Height / divisor * (i - offset);

                    lines.AddRange(GLU.Line(x, Rect.Y, x, Rect.Y + Rect.Height, 1, 0.2f, 0.2f, 0.2f));
                    lines.AddRange(GLU.Line(Rect.X, y, Rect.X + Rect.Width, y, 1, 0.2f, 0.2f, 0.2f));
                }
            }

            //render grid letters
            if (Settings.settings["gridLetters"])
            {
                // kept breaking for some reason
                try
                {
                    var copy = new Dictionary<Keys, Tuple<int, int>>(MainWindow.Instance.KeyMapping);

                    foreach (var key in copy)
                    {
                        var letter = key.Key.ToString().Replace("KeyPad", "");
                        var tuple = key.Value;

                        var x = Rect.X + tuple.Item1 * cellSize + cellSize / 2f;
                        var y = Rect.Y + tuple.Item2 * cellSize + cellSize / 2f;

                        var width = FontRenderer.GetWidth(letter, 38, "main");
                        var height = FontRenderer.GetHeight(38, "main");

                        color1Texts.AddRange(FontRenderer.Print((int)(x - width / 2f), (int)(y - height / 2f + 5f), letter, 38, "main"));
                    }
                }
                catch { }
            }

            Indices["rectLength"] = rects.Count / 6;
            Indices["loopLength"] = loops.Count / 6;
            Indices["lineLength"] = lines.Count / 6;
            Indices["bezierLength"] = bezierVertices.Count / 6;

            rects.AddRange(loops);
            rects.AddRange(lines);
            rects.AddRange(bezierVertices);

            //process notes
            Note? last = null;
            Note? next = null;

            for (int i = 0; i < editor.Notes.Count; i++)
            {
                var note = editor.Notes[i];
                var passed = currentTime > note.Ms + 1;
                var visible = !passed && note.Ms - currentTime <= 1000f / approachRate;

                if (passed)
                    last = note;
                else
                    next ??= note;

                if (!visible && passed && next != null)
                    break;
            }

            //render autoplay cursor
            if (Settings.settings["autoplay"])
            {
                last ??= startNote;
                next ??= last;

                var timeDiff = next.Ms - last.Ms;
                var timePos = currentTime - last.Ms;

                var progress = timeDiff == 0 ? 1 : (float)timePos / timeDiff;
                progress = (float)Math.Sin(progress * MathHelper.PiOver2);

                var width = (float)Math.Sin(progress * MathHelper.Pi) * 8f + 16;

                var lx = Rect.X + last.X * cellSize;
                var ly = Rect.Y + last.Y * cellSize;

                var nx = Rect.X + next.X * cellSize;
                var ny = Rect.Y + next.Y * cellSize;

                var x = cellSize / 2f + lx + (nx - lx) * progress;
                var y = cellSize / 2f + ly + (ny - ly) * progress;

                var cx = x - width / 2f;
                var cy = y - width / 2f;

                rects.AddRange(GLU.Rect(cx, cy, width, width, 1f, 1f, 1f, 0.25f));
                rects.AddRange(GLU.OutlineAsTriangles(cx, cy, width, width, 2, 1f, 1f, 1f, 1f));
            }

            return new Tuple<float[], float[]>(rects.ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            var separateClickTools = Settings.settings["separateClickTools"];
            var selectTool = Settings.settings["selectTool"];

            Dragging = Hovering && (HoveringNote != null || (!separateClickTools || !selectTool));

            if (Dragging)
            {
                var editor = MainWindow.Instance;

                if (HoveringNote == null || (separateClickTools && !selectTool))
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
                else if (HoveringNote != null)
                {
                    DraggingNote = HoveringNote;
                    lastPos = new PointF(HoveringNote.X, HoveringNote.Y);
                    startPos = new Vector2(HoveringNote.X, HoveringNote.Y);

                    var selected = editor.SelectedNotes.ToList();

                    if (editor.ShiftHeld)
                    {
                        selected = new List<Note> { selected[0] };

                        var first = selected[0];
                        var last = HoveringNote;
                        var min = Math.Min(first.Ms, last.Ms);
                        var max = Math.Max(first.Ms, last.Ms);

                        foreach (var note in editor.Notes)
                            if (note.Ms >= min && note.Ms <= max && !selected.Contains(note))
                                selected.Add(note);
                    }
                    else if (editor.CtrlHeld)
                    {
                        if (selected.Contains(HoveringNote))
                            selected.Remove(HoveringNote);
                        else
                            selected.Add(HoveringNote);
                    }
                    else if (!selected.Contains(HoveringNote))
                        selected = new List<Note>() { HoveringNote };

                    editor.SelectedNotes = selected.ToList();
                    editor.UpdateSelection();
                }
            }
        }

        public override void OnMouseMove(Point pos)
        {
            if (Dragging)
            {
                var editor = MainWindow.Instance;

                if (DraggingNote == null)
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

                        var xDiff = newPos.X - DraggingNote.X;
                        var yDiff = newPos.Y - DraggingNote.Y;

                        var maxX = DraggingNote.X;
                        var minX = DraggingNote.X;
                        var maxY = DraggingNote.Y;
                        var minY = DraggingNote.Y;

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

        public override void OnMouseUp(Point pos)
        {
            if (DraggingNote != null && new Vector2(DraggingNote.X, DraggingNote.Y) != startPos)
            {
                var editor = MainWindow.Instance;
                var selected = editor.SelectedNotes.ToList();
                var oldPos = new List<Vector2>();
                var newPos = new List<Vector2>();

                var posDiff = new Vector2(DraggingNote.X, DraggingNote.Y) - startPos;

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

            Dragging = false;
            DraggingNote = null;
            lastPlaced = new PointF();
        }

        public void ClearPreviewNotes()
        {
            previewNoteOffsets.Clear();
            bezierVertices.Clear();
        }

        public void AddPreviewNote(float x, float y, Color color, bool mouse = false)
        {
            var c = new float[] { color.R / 255f, color.G / 255f, color.B / 255f };

            var cellSize = Rect.Width / 3f;
            var noteSize = cellSize * 0.65f;
            var cellGap = (cellSize - noteSize) / 2f;

            var pos = mouse ? MainWindow.Instance.PointToGridSpace(x, y) : new PointF(x, y);
            x = Rect.X + pos.X * cellSize + cellGap;
            y = Rect.Y + pos.Y * cellSize + cellGap;

            previewNoteOffsets.Add((2 * (int)x + c[0], 2 * (int)y + c[1], 2f + c[2], 3f));
            bezierVertices.AddRange(new float[6] { x + noteSize / 2f, y + noteSize / 2f, 1f, 1f, 1f, 1f });
        }
    }
}
