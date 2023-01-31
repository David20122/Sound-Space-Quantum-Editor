using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Sound_Space_Editor.Misc;
using System.Linq;

namespace Sound_Space_Editor.GUI
{
	class GuiTrack : Gui
	{
        private float lastPlayedTick;

        public Note lastPlayed;
        public Note hoveringNote;
        public Note draggingNote;

        public TimingPoint hoveringPoint;
        public TimingPoint draggingPoint;

        public bool hovering;

        public RectangleF originRect;

        public PointF dragStartPoint;
        public long dragStartMs;

        public bool draggingTrack;
        public bool rightDraggingTrack;

        private bool replay;

        private float cellSize = 64f;
        private float noteSize => cellSize * 0.65f;
        private float cellGap => (cellSize - noteSize) / 2f;

        public float startPos = 0f;
        public float endPos = 1f;


        public GuiTrack() : base(0, 0, MainWindow.Instance.ClientSize.Width, 0)
        {
            rect.Height = yoffset - 32;
            originRect = new RectangleF(0, 0, rect.Width, yoffset - 32);
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            var editor = MainWindow.Instance;

            var color1 = Settings.settings["color1"];
            var color2 = Settings.settings["color2"];
            var color3 = Settings.settings["color3"];

            GL.Color4(Color.FromArgb(Settings.settings["trackOpacity"], 36, 35, 33));
            GLSpecial.Rect(rect);
            GL.Color3(0.2f, 0.2f, 0.2f);
            GLSpecial.Rect(rect.X, rect.Y + rect.Height, rect.Width, 1);

            var noteStep = editor.NoteStep;

            var currentTime = Settings.settings["currentTime"].Value;
            var totalTime = Settings.settings["currentTime"].Max;
            var sfxOffset = Settings.settings["sfxOffset"];
            var beatDivisor = Settings.settings["beatDivisor"].Value + 1f;

            var posX = currentTime / 1000f * noteStep;
            var maxX = totalTime / 1000f * noteStep;
            var cursorX = rect.Width * Settings.settings["cursorPos"].Value / 100f;
            var endX = cursorX - posX + maxX + 1;

            startPos = Math.Max(0, (-cursorX * 1000f / noteStep + currentTime) / totalTime);
            endPos = Math.Min(1, ((rect.Width - cursorX) * 1000f / noteStep + currentTime) / totalTime);

            if (Settings.settings["waveform"])
            {
                GL.Color3(0.35f, 0.35f, 0.35f);
                GL.PushMatrix();
                GL.BindVertexArray(editor.MusicPlayer.WaveModel.VaoID);
                GL.EnableVertexAttribArray(0);

                var waveX = -posX + cursorX + maxX / 2f;

                GL.Translate(waveX, rect.Height * 0.5, 0);
                GL.Scale(maxX / 100000.0, -rect.Height, 1);
                GL.Translate(-50000, -0.5, 0);
                GL.LineWidth(2f);

                editor.MusicPlayer.WaveModel.Render(PrimitiveType.LineStrip);

                GL.Translate(50000 * maxX, 0.5, 0);
                GL.Scale(1 / maxX * 100000.0, -1.0 / rect.Height, 1);
                GL.Translate(-waveX, -rect.Height * 0.5, 0);

                GL.DisableVertexAttribArray(0);
                GL.BindVertexArray(0);
                GL.PopMatrix();
            }

            GL.LineWidth(1f);

            GL.Color4(color2);
            GLSpecial.Line(cursorX - posX, rect.Y, cursorX - posX, rect.Bottom);

            GL.Color3(1f, 0f, 0f);
            GLSpecial.Line(endX, rect.Y, endX, rect.Bottom);

            hoveringNote = null;
            hoveringPoint = null;
            Note closest = null;

            float? lastRendered = null;
            float? lastTextRendered = null;
            float? lastTickRendered = null;

            var minMs = (posX - cursorX - noteSize) / noteStep * 1000f;
            var maxMs = (rect.Width - cursorX + posX) / noteStep * 1000f;

            var textHeight = TextHeight(16);

            var selecting = editor.rightHeld && rightDraggingTrack;
            var selected = selecting ? new List<Note>() : editor.SelectedNotes;

            var selectHitbox = new RectangleF();

            if (selecting)
            {
                var offsetMs = dragStartMs - currentTime;
                var startX = dragStartPoint.X + offsetMs / 1000f * noteStep;

                var my = MathHelper.Clamp(mousey, 0f, rect.Height);
                var x = Math.Min(mousex, startX);
                var y = Math.Min(my, dragStartPoint.Y);
                var w = Math.Max(mousex, startX) - x;
                var h = Math.Min(rect.Height, Math.Max(my, dragStartPoint.Y) - y);

                selectHitbox = new RectangleF(x, y, w, h);

                GL.Color4(0f, 1f, 0.2f, 0.2f);
                GLSpecial.Rect(selectHitbox);
                GL.Color4(0f, 1f, 0.2f, 1f);
                GLSpecial.Outline(selectHitbox);
            }

            //render notes
            for (int i = 0; i < editor.Notes.Count; i++)
            {
                var note = editor.Notes[i];

                var x = cursorX - posX + note.Ms / 1000f * noteStep;
                var noteRect = new RectangleF(x, cellGap, noteSize, noteSize);

                bool noteSelected = selecting && selectHitbox.IntersectsWith(noteRect);
                if (noteSelected)
                    selected.Add(note);

                if (note.Ms < minMs || note.Ms > maxMs)
                    continue;

                if (note.Ms <= currentTime - sfxOffset)
                    closest = note;

                if (lastRendered != null && x - 1 <= lastRendered)
                    continue;

                noteSelected |= selected.IndexOf(note) >= 0;
                
                var alpha = currentTime - 1 > note.Ms ? 0.35f : 1f;
                var alphaC = alpha * 255f;

                var isHovering = hoveringNote == null && draggingNote == null && noteRect.Contains(mousex, mousey);

                if (isHovering || (noteSelected && draggingNote == null))
                {
                    if (isHovering)
                    {
                        hoveringNote = note;
                        GL.Color3(0f, 1f, 0.25f);
                    }
                    else
                        GL.Color3(0f, 0.5f, 1f);

                    GLSpecial.Outline(x - 4, cellGap - 4, noteSize + 8, noteSize + 8);
                }

                GL.Color4(Color.FromArgb((int)(alpha * 15f), note.Color));
                GLSpecial.Rect(noteRect);
                GL.Color4(Color.FromArgb((int)alphaC, note.Color));
                GLSpecial.Outline(noteRect);

                GL.Color4(Color.FromArgb((int)(alphaC * 0.45f), note.Color));

                if (lastRendered == null || x - 4 > lastRendered)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        var indexX = 2 - j % 3;
                        var indexY = 2 - j / 3;

                        var gridX = x + indexX * 11 + 5.5f;
                        var gridY = cellGap + indexY * 11 + 5.5f;

                        GLSpecial.Outline(gridX, gridY, 9, 9);
                    }

                    {
                        var gridX = x + note.X * 11 + 5.5f;
                        var gridY = cellGap + note.Y * 11 + 5.5f;

                        GL.Color4(Color.FromArgb((int)alphaC, note.Color));
                        GLSpecial.Rect(gridX, gridY, 9, 9);
                    }
                }

                if (lastTextRendered == null || x - 8 > lastTextRendered)
                {
                    lastTextRendered = x;

                    var numText = $"{i + 1:##,###}";
                    var msText = $"{note.Ms:##,###}";
                    if (msText == "")
                        msText = "0";

                    GL.Color3(color1);
                    RenderText($"Note {numText}", x + 3, rect.Height + 3, 16);
                    GL.Color3(color2);
                    RenderText($"{msText}ms", x + 3, rect.Height + textHeight + 5, 16);

                    GL.Color4(1f, 1f, 1f, alpha);
                    GLSpecial.Line(x + 0.5f, rect.Height + 3, x + 0.5f, rect.Height + 28);
                }

                lastRendered = x;
            }

            if (selecting)
                editor.SelectedNotes = selected;

            //render drag lines
            if (draggingNote != null)
            {
                foreach (var note in editor.SelectedNotes)
                {
                    var x = cursorX - posX + note.DragStartMs / 1000f * noteStep;

                    GL.Color3(0.75f, 0.75f, 0.75f);
                    GLSpecial.Line(x, 0f, x, rect.Height);
                }
            }

            //play hit sound
            if (lastPlayed != closest)
            {
                lastPlayed = closest;

                if (closest != null && editor.MusicPlayer.IsPlaying)
                    editor.SoundPlayer.Play("hit");
            }

            //render points
            var multiplier = beatDivisor % 1 == 0 ? 1f : 1f / (beatDivisor % 1);

            for (int i = 0; i < editor.TimingPoints.Count; i++)
            {
                var point = editor.TimingPoints[i];

                if (point.bpm > 0)
                {
                    var nextMs = i + 1 < editor.TimingPoints.Count ? editor.TimingPoints[i + 1].Ms : totalTime * 2;
                    var nextX = i + 1 < editor.TimingPoints.Count ? cursorX - posX + nextMs / 1000f * noteStep : endX;

                    var stepX = 60 / point.bpm * noteStep;
                    var stepXSmall = stepX / beatDivisor;

                    var lineX = cursorX - posX + point.Ms / 1000f * noteStep;

                    if (stepX > 0 && lineX < rect.Width && lineX > 0)
                    {
                        GL.Color4(Color.FromArgb(255, 0, 0));
                        GLSpecial.Line(lineX, 0, lineX, rect.Height + 56);
                    }

                    var numText = $"{point.bpm:##,###.###} BPM";
                    var msText = $"{point.Ms:##,###}ms";
                    if (msText == "ms")
                        msText = "0ms";

                    GL.Color3(color1);
                    RenderText(numText, lineX + 3, rect.Height + 31, 16);
                    GL.Color3(color2);
                    RenderText(msText, lineX + 3, rect.Height + 33 + textHeight, 16);

                    var divisor = multiplier * beatDivisor;
                    stepX *= multiplier;

                    var width = Math.Max(TextWidth(numText, 16), TextWidth(msText, 16));
                    var hitbox = new RectangleF(lineX, rect.Height, width + 3, 56);

                    var hovering = hoveringNote == null && hitbox.Contains(mousex, mousey);

                    if (hovering || editor.SelectedPoint == point)
                    {
                        if (hovering)
                        {
                            hoveringPoint = point;
                            GL.Color3(0f, 1f, 0.25f);
                        }
                        else
                            GL.Color3(0f, 0.5f, 1f);

                        GLSpecial.Outline(lineX - 4, rect.Height, width + 11, 60);
                    }

                    if (lineX < 0)
                        lineX %= stepX;

                    var gapf = rect.Height - noteSize - cellGap;

                    //render bpm lines
                    while (stepX > 0 && lineX < rect.Width && lineX < nextX && lineX < endX)
                    {
                        GL.Color3(color2);
                        GLSpecial.Line(lineX, rect.Height, lineX, rect.Height - gapf);

                        for (int j = 1; j < divisor; j++)
                        {
                            var x = lineX + j * stepXSmall;

                            if (x < nextX && x < endX && (lastTickRendered == null || x - 1 > lastTickRendered))
                            {
                                var half = j == divisor / 2 && divisor % 2 == 0;

                                GL.Color3(half ? color3 : color1);
                                GLSpecial.Line(x, rect.Height - 3 * gapf / (half ? 5 : 10), x, rect.Height);

                                lastTickRendered = x;
                            }
                        }

                        lineX += stepX;
                    }
                }
            }

            //play metronome
            if (Settings.settings["metronome"])
            {
                var ms = currentTime - sfxOffset;
                var bpm = editor.GetCurrentBpm(currentTime);
                var interval = 60000f / bpm.bpm / beatDivisor;
                var remainder = (ms - bpm.Ms) % interval;
                var closestMs = ms - remainder;

                if (lastPlayedTick != closestMs && remainder >= 0 && editor.MusicPlayer.IsPlaying)
                {
                    lastPlayedTick = closestMs;

                    editor.SoundPlayer.Play("metronome");
                }
            }

            GL.Color4(1f, 1f, 1f, 0.75f);
            GLSpecial.Line(cursorX, 4, cursorX, rect.Height - 4);
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            if (right)
                OnMouseUp(pos);

            var startMs = (long)Settings.settings["currentTime"].Value;
            var editor = MainWindow.Instance;

            var replayf = editor.MusicPlayer.IsPlaying && !right;
            replay = false;

            if (replayf)
                editor.MusicPlayer.Pause();

            if (hoveringNote != null && !right)
            {
                draggingNote = hoveringNote;

                dragStartPoint = pos;
                dragStartMs = startMs;

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

                foreach (var note in editor.SelectedNotes)
                    note.DragStartMs = note.Ms;
            }
            else if (hoveringPoint != null && !right)
            {
                draggingPoint = hoveringPoint;

                dragStartPoint = pos;
                dragStartMs = startMs;

                editor.SelectedPoint = hoveringPoint;

                draggingPoint.DragStartMs = draggingPoint.Ms;
            }
            else
            {
                replay = replayf;

                dragStartPoint = pos;
                dragStartMs = startMs;
            }

            rightDraggingTrack = rightDraggingTrack || right;
            draggingTrack = draggingTrack || !right;
        }

        public override void OnMouseMove(Point pos)
        {
            if (draggingTrack)
            {
                var editor = MainWindow.Instance;
                var currentTime = Settings.settings["currentTime"];
                var divisor = Settings.settings["beatDivisor"].Value;
                var cursorPos = Settings.settings["cursorPos"].Value;

                var cellStep = editor.NoteStep;

                var x = Math.Abs(pos.X - dragStartPoint.X) >= 5 ? pos.X : dragStartPoint.X;
                var offset = (x - dragStartPoint.X) / cellStep * 1000f;
                var cursorms = (x - rect.Width * cursorPos / 100f - noteSize / 2f) / cellStep * 1000f + currentTime.Value;

                if (draggingNote != null)
                {
                    offset = draggingNote.DragStartMs - cursorms;
                    var currentBpm = editor.GetCurrentBpm(cursorms).bpm;

                    if (currentBpm > 0)
                    {
                        var stepX = 60f / currentBpm * cellStep;
                        var stepXSmall = stepX / divisor;

                        var threshold = MathHelper.Clamp(stepXSmall / 1.75f, 1f, 12f);
                        var snappedMs = editor.GetClosestBeat(draggingNote.Ms);

                        if (Math.Abs(snappedMs - cursorms) / 1000f * cellStep <= threshold)
                            offset = draggingNote.DragStartMs - snappedMs;
                    }

                    foreach (var note in editor.SelectedNotes)
                        note.Ms = (long)MathHelper.Clamp(note.DragStartMs - offset, 0f, currentTime.Max);

                    editor.SortNotes();
                }
                else if (draggingPoint != null)
                {
                    offset = draggingPoint.DragStartMs - cursorms;
                    var currentBpm = editor.GetCurrentBpm(cursorms).bpm;

                    var stepX = 60f / currentBpm * cellStep;
                    var stepXSmall = stepX / divisor;

                    var threshold = MathHelper.Clamp(stepXSmall / 1.75f, 1f, 12f);
                    var snappedMs = editor.GetClosestBeat(draggingPoint.Ms, true);
                    var snappedNote = editor.GetClosestNote(draggingPoint.Ms);

                    if (Math.Abs(snappedNote - cursorms) < Math.Abs(snappedMs - cursorms))
                        snappedMs = snappedNote;
                    if (Math.Abs(snappedMs - cursorms) / 1000f * cellStep <= threshold)
                        offset = draggingPoint.DragStartMs - snappedMs;
                    if (Math.Abs(cursorms) / 1000f * cellStep <= threshold)
                        offset = draggingPoint.DragStartMs;

                    draggingPoint.Ms = (long)Math.Min(draggingPoint.DragStartMs - offset, currentTime.Max);

                    editor.SortTimings(false);
                }
                else
                {
                    var finalTime = dragStartMs - offset;

                    if (editor.GetCurrentBpm(finalTime).bpm > 0)
                        finalTime = editor.GetClosestBeat(finalTime);

                    finalTime = MathHelper.Clamp(finalTime, 0f, currentTime.Max);

                    currentTime.Value = finalTime;
                }
            }
        }

        public override void OnMouseUp(Point pos, bool right = false)
        {
            if (draggingTrack)
            {
                var editor = MainWindow.Instance;

                if (draggingNote != null && draggingNote.DragStartMs != draggingNote.Ms)
                {
                    var selected = editor.SelectedNotes.ToList();
                    var startList = new List<long>();
                    var msList = new List<long>();

                    for (int i = 0; i < selected.Count; i++)
                    {
                        startList.Add(selected[i].DragStartMs);
                        msList.Add(selected[i].Ms);
                    }

                    editor.UndoRedoManager.Add($"MOVE NOTE{(selected.Count > 1 ? "S" : "")}", () =>
                    {
                        for (int i = 0; i < selected.Count; i++)
                            selected[i].Ms = startList[i];

                        editor.SortNotes();
                    }, () =>
                    {
                        for (int i = 0; i < selected.Count; i++)
                            selected[i].Ms = msList[i];

                        editor.SortNotes();
                    }, false);
                }
                else if (draggingPoint != null && draggingPoint.DragStartMs != draggingPoint.Ms)
                {
                    var point = draggingPoint;
                    var ms = point.Ms;
                    var msStart = point.DragStartMs;

                    editor.UndoRedoManager.Add("MOVE POINT", () =>
                    {
                        point.Ms = msStart;

                        editor.SortTimings();
                    }, () =>
                    {
                        point.Ms = ms;

                        editor.SortTimings();
                    }, false);
                }

                draggingTrack = false;
                draggingNote = null;
                draggingPoint = null;

                if (replay)
                    editor.MusicPlayer.Play();
            }

            if (rightDraggingTrack)
                rightDraggingTrack = false;
        }
    }
}