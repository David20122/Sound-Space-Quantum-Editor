using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Graphics;
using System.Buffers;
using New_SSQE.Objects;
using System;
using New_SSQE.GUI.Font;
using New_SSQE.Audio;
using New_SSQE.GUI.Shaders;
using New_SSQE.Preferences;
using New_SSQE.Objects.Managers;
using System.Linq;
using New_SSQE.Maps;

namespace New_SSQE.GUI
{
    internal class GuiTrack : WindowControl
    {
        public static bool RenderMapObjects = false;
        public static bool VFXObjects = true;

        private float lastPlayedTick;

        public MapObject? LastPlayed;
        public Note? HoveringNote;
        public Note? DraggingNote;
        private List<Note> draggingNotes;

        public TimingPoint? HoveringPoint;
        public TimingPoint? DraggingPoint;

        public MapObject? HoveringVfx;
        public MapObject? DraggingVfx;
        private List<MapObject> draggingVfxs;

        public MapObject? HoveringSpec;
        public MapObject? DraggingSpec;
        private List<MapObject> draggingSpecs;

        public MapObject? HoveringObjDuration;
        public MapObject? DraggingObjDuration;
        
        public bool Hovering;

        public PointF DragStartPoint;
        public long DragStartMs;

        public bool DraggingTrack;
        public bool RightDraggingTrack;

        private bool replay;

        private const float cellSize = 64f;
        private const float noteSize = cellSize * 0.65f;
        private const float cellGap = (cellSize - noteSize) / 2f;
        private float GapF => Rect.Height - noteSize - cellGap;

        public float StartPos = 0f;
        public float EndPos = 1f;

        private Vector4 PosSet = new();

        private TextureHandle spriteSheet;

        private readonly Dictionary<string, int> Indices = new()
        {
            {"rectLength", 0 },
            {"loopLength", 0 },
            {"lineLength", 0 },
        };

        private bool selecting;
        private RectangleF selectHitbox;

        private readonly ArrayPool<Vector4> Pool = ArrayPool<Vector4>.Shared;


        public GuiTrack() : base(0, 0, MainWindow.Instance.ClientSize.X, 0)
        {
            float yoffset = MainWindow.Instance.CurrentWindow?.YOffset ?? 0f;

            Rect.Height = yoffset - 32;
            OriginRect = new RectangleF(0, 0, Rect.Width, yoffset - 32);

            Dynamic = true;

            InstanceSetup();
            Init();
        }

        public override void InstanceSetup()
        {
            ClearBuffers();

            VaOs = new VertexArrayHandle[20];
            VbOs = new BufferHandle[40];
            VertexCounts = new int[20];

            // notes
            RectangleF noteRect = new(0, cellGap, noteSize, noteSize);

            // normal
            List<float> noteVerts = new();
            noteVerts.AddRange(GLU.Rect(noteRect, 1f, 1f, 1f, 1f / 20f));
            noteVerts.AddRange(GLU.OutlineAsTriangles(noteRect, 1, 1f, 1f, 1f, 1f));
            float[] noteLocationVerts = GLU.Rect(0, 0, 9, 9, 1f, 1f, 1f, 1f);
            // text line
            float[] textLineVerts = GLU.Line(0.5f, Rect.Height + 3, 0.5f, Rect.Height + 28, 1, 1f, 1f, 1f, 1f);

            for (int j = 0; j < 9; j++)
            {
                int indexX = 2 - j % 3;
                int indexY = j / 3;

                float gridX = indexX * 12 + 4.5f;
                float gridY = cellGap + indexY * 12 + 4.5f;

                noteVerts.AddRange(GLU.OutlineAsTriangles(gridX, gridY, 9, 9, 1, 1f, 1f, 1f, 1f * 0.45f));
            }

            AddToBuffers(noteVerts.ToArray(), 0);
            AddToBuffers(noteLocationVerts.ToArray(), 1);
            AddToBuffers(textLineVerts, 13);

            // note select box
            float[] noteSelectVerts = GLU.OutlineAsTriangles(-4, cellGap - 4, noteSize + 8, noteSize + 8, 1, 1f, 1f, 1f, 1f);
            AddToBuffers(noteSelectVerts, 4);

            // note hover box
            float[] noteHoverVerts = GLU.OutlineAsTriangles(-4, cellGap - 4, noteSize + 8, noteSize + 8, 1, 1f, 1f, 1f, 1f);
            AddToBuffers(noteHoverVerts, 5);

            // drag line
            float[] dragVerts = GLU.Line(0, 0, 0, Rect.Height, 1, 1f, 1f, 1f, 1f);
            AddToBuffers(dragVerts, 6);

            // start bpm
            float[] startBpmVerts = GLU.Line(0, 0, 0, Rect.Height + 58, 1, 1f, 1f, 1f, 1f);
            AddToBuffers(startBpmVerts, 7);

            // full bpm
            float[] fullBpmVerts = GLU.Line(0, Rect.Height, 0, Rect.Height - GapF, 1, 1f, 1f, 1f, 1f);
            AddToBuffers(fullBpmVerts, 8);

            // half bpm
            float[] halfBpmVerts = GLU.Line(0, Rect.Height - 3 * GapF / 5, 0, Rect.Height, 1, 1f, 1f, 1f, 1f);
            AddToBuffers(halfBpmVerts, 9);

            // sub bpm
            float[] subBpmVerts = GLU.Line(0, Rect.Height - 3 * GapF / 10, 0, Rect.Height, 1, 1f, 1f, 1f, 1f);
            AddToBuffers(subBpmVerts, 10);

            // point hover box
            float[] pointHoverVerts = GLU.OutlineAsTriangles(-4, Rect.Height, 80, 60, 1, 1f, 1f, 1f, 1f);
            AddToBuffers(pointHoverVerts, 11);

            // point select box
            float[] pointSelectVerts = GLU.OutlineAsTriangles(-4, Rect.Height, 80, 60, 1, 1f, 1f, 1f, 1f);
            AddToBuffers(pointSelectVerts, 12);

            // map object outline
            List<float> mapObjectVerts = new();
            mapObjectVerts.AddRange(GLU.CircleOutline(noteSize / 2, noteSize / 2 + cellGap, noteSize / 2, 2, 20, 0, 1f, 1f, 1f, 1f));
            mapObjectVerts.AddRange(GLU.CircleAsTriangles(noteSize / 2, noteSize / 2 + cellGap, noteSize / 2, 20, 0, 1f, 1f, 1f, 1f / 20f));
            AddToBuffers(mapObjectVerts.ToArray(), 14);

            // map object select box
            float[] objSelectVerts = GLU.CircleOutline(noteSize / 2, noteSize / 2 + cellGap, noteSize / 2 + 4, 1, 20, 0, 1f, 1f, 1f, 1f);
            AddToBuffers(objSelectVerts, 2);

            // map object hover box
            float[] objHoverVerts = GLU.CircleOutline(noteSize / 2, noteSize / 2 + cellGap, noteSize / 2 + 4, 1, 20, 0, 1f, 1f, 1f, 1f);
            AddToBuffers(objHoverVerts, 3);

            // map object duration diamond
            float[] objDiamondVerts = GLU.CircleAsTriangles(0, noteSize / 2 + cellGap, noteSize / 4, 4, 0, 1f, 1f, 1f, 1f);
            AddToBuffers(objDiamondVerts, 15);

            // map object duration line
            float[] objDurationVerts = GLU.Line(0, noteSize / 2 + cellGap, 1, noteSize / 2 + cellGap, 4, 1f, 1f, 1f, 1f);
            AddToBuffers(objDurationVerts, 16);

            // map object duration select verts
            float[] objDurationSelectVerts = GLU.CircleOutline(0, noteSize / 2 + cellGap, noteSize / 4 + 4, 1, 4, 0, 1f, 1f, 1f, 1f);
            AddToBuffers(objDurationSelectVerts, 17);

            // map object duration hover verts
            float[] objDurationHoverVerts = GLU.CircleOutline(0, noteSize / 2 + cellGap, noteSize / 4 + 4, 1, 4, 0, 1f, 1f, 1f, 1f);
            AddToBuffers(objDurationHoverVerts, 18);

            // map object icon verts
            float x = noteSize / 8;
            float y = noteSize / 8 + cellGap;
            float w = noteSize * 3 / 4f;
            float h = noteSize * 3 / 4f;

            float[] iconVerts = new float[]
            {
                x, y, 0f, 0f, 0, 0,
                x + w, y, 1f, 0f, 0, 0,
                x, y + h, 0f, 1f, 0, 0,

                x + w, y + h, 1f, 1f, 0, 0,
                x, y + h, 0f, 1f, 0, 0,
                x + w, y, 1f, 0f, 0, 0
            };
            AddToBuffers(iconVerts, 19);

            // set up spritesheet for icons
            GL.ActiveTexture(TextureUnit.Texture2);
            spriteSheet = TextureManager.GetOrRegister("sprites", null, true, TextureUnit.Texture2);

            GL.UseProgram(Shader.IconTexProgram);
            int location = GL.GetUniformLocation(Shader.IconTexProgram, "SpriteSize");
            GL.Uniform2f(location, 1f / MainWindow.SpriteSize.X, 1f / MainWindow.SpriteSize.Y);
            location = GL.GetUniformLocation(Shader.IconTexProgram, "texture0");
            GL.Uniform1i(location, 2);
        }

        private bool SelectIntersecting(float x)
        {
            return selectHitbox.X < x + noteSize && selectHitbox.Right > x && selectHitbox.Y < cellGap + noteSize && selectHitbox.Bottom > cellGap;
        }

        private bool HoverIntersecting(float mx, float my, float x)
        {
            return mx > x && mx < x + noteSize && my > cellGap && my < cellGap + noteSize;
        }

        private Vector4[] RenderC1Array(string[] set, int len, int[] x, int nCount)
        {
            int i;
            int pos = 0;

            Vector4[] c1 = new Vector4[len];

            for (i = 0; i < nCount; i++)
            {
                FontRenderer.PrintInto(c1, pos, x[i], (int)Rect.Height - 2, set[i], 20, "main");
                pos += set[i].Length;
            }

            for (; i < set.Length; i++)
            {
                FontRenderer.PrintInto(c1, pos, x[i], (int)Rect.Height + 26, set[i], 20, "main");
                pos += set[i].Length;
            }

            return c1;
        }

        private Vector4[] RenderC2Array(string[] set, int len, int[] x, int nCount)
        {
            int i;
            int pos = 0;

            Vector4[] c2 = new Vector4[len];

            for (i = 0; i < nCount; i++)
            {
                FontRenderer.PrintInto(c2, pos, x[i], (int)Rect.Height + 13, set[i], 20, "main");
                pos += set[i].Length;
            }

            for (; i < set.Length; i++)
            {
                FontRenderer.PrintInto(c2, pos, x[i], (int)Rect.Height + 41, set[i], 20, "main");
                pos += set[i].Length;
            }

            return c2;
        }

        public override void GenerateOffsets()
        {
            string[] c1Set;
            string[] c2Set;
            int[] xSet;

            int c1Len = 0;
            int c2Len = 0;
            int nCount = 0;

            MainWindow editor = MainWindow.Instance;
            float currentTime = Settings.currentTime.Value.Value;

            Point mouse = editor.Mouse;
            float noteStep = CurrentMap.NoteStep;

            float totalTime = Settings.currentTime.Value.Max;
            float sfxOffset = Settings.sfxOffset.Value;
            float beatDivisor = Settings.beatDivisor.Value.Value + 1f;

            float posX = currentTime / 1000f * noteStep;
            float cursorX = Rect.Width * Settings.cursorPos.Value.Value / 100f;

            int colorCount = Settings.noteColors.Value.Count;

            selecting = editor.RightHeld && RightDraggingTrack;

            float minMs = (-cursorX - noteSize) * 1000f / noteStep + currentTime;
            float maxMs = (Rect.Width - cursorX) * 1000f / noteStep + currentTime;

            float? lastRendered = null;

            HoveringNote = null;
            HoveringPoint = null;
            HoveringVfx = null;
            HoveringSpec = null;
            HoveringObjDuration = null;

            if (!RenderMapObjects)
            {
                List<Note> selected = selecting ? new() : CurrentMap.Notes.Selected;
                MapObject? closest = null;

                (int low, int high) = CurrentMap.Notes.SearchRange(minMs, maxMs);
                int range = high - low;

                c1Set = new string[range + CurrentMap.TimingPoints.Count];
                c2Set = new string[c1Set.Length];
                xSet = new int[c1Set.Length];
                nCount = range;

                Vector4[] noteOffsets = Pool.Rent(range);
                Vector4[] textLineOffsets = Pool.Rent(range);
                Vector4[] noteLocationOffsets = Pool.Rent(range);

                Vector4 noteHoverOffset = (-1920, 0, 0, 0);
                List<Vector4> noteSelectOffsets = new();
                List<Vector4> noteDragOffsets = new();

                // notes
                for (int i = 0; i < CurrentMap.Notes.Count; i++)
                {
                    Note note = CurrentMap.Notes[i];
                    float x = cursorX - posX + note.Ms / 1000f * noteStep;

                    bool noteSelected = selecting && SelectIntersecting(x);
                    if (noteSelected)
                        selected.Add(note);

                    if (i >= low && i < high)
                    {
                        float gridX = x + (2 - note.X) * 12 + 4.5f;
                        float gridY = cellGap + (2 - note.Y) * 12 + 4.5f;

                        int c = i % colorCount;
                        float a = note.Ms < currentTime - 1 ? 0.35f : 1f;
                        int index = i - low;

                        bool hovering = HoveringNote == null && DraggingNote == null && HoverIntersecting(mouse.X, mouse.Y, x);

                        noteOffsets[index] = (x, 0, a, c);
                        noteLocationOffsets[index] = (gridX, gridY, a, c);
                        textLineOffsets[index] = (x, 0, a, 4);

                        if (note.Ms <= currentTime - sfxOffset)
                            closest = note;

                        if (selecting)
                            note.Selected = noteSelected;
                        noteSelected |= note.Selected;

                        if (hovering)
                        {
                            noteHoverOffset = (x, 0, 1, 5);
                            HoveringNote = note;
                        }
                        else if (noteSelected)
                        {
                            if (DraggingNote == null)
                                noteSelectOffsets.Add((x, 0, 1, 6));
                            else
                            {
                                float dragX = cursorX - posX + note.DragStartMs / 1000f * noteStep;
                                noteDragOffsets.Add((dragX, 0, 1, 7));
                            }
                        }

                        if (lastRendered == null || x - 8 > lastRendered)
                        {
                            c1Set[index] = $"Note {i + 1:##,###}";
                            c2Set[index] = $"{note.Ms:##,##0}ms";
                            xSet[index] = (int)x + 3;

                            c1Len += c1Set[index].Length;
                            c2Len += c2Set[index].Length;

                            lastRendered = x;
                        }
                        else
                        {
                            c1Set[index] = "";
                            c2Set[index] = "";
                        }
                    }
                }

                GL.UseProgram(Shader.TrackProgram);
                RegisterData(1, noteLocationOffsets, range);
                RegisterData(0, noteOffsets, range);

                Pool.Return(noteOffsets);
                Pool.Return(noteLocationOffsets);

                GL.UseProgram(Shader.TimelineProgram);
                RegisterData(13, textLineOffsets, range);

                Pool.Return(textLineOffsets);

                RegisterData(4, noteSelectOffsets.ToArray());
                RegisterData(5, new Vector4[1] { noteHoverOffset });
                RegisterData(6, noteDragOffsets.ToArray());

                if (selecting)
                    CurrentMap.Notes.Selected = new(selected);

                //play hit sound
                if (LastPlayed != closest)
                {
                    LastPlayed = closest;

                    if (closest != null && MusicPlayer.IsPlaying && MainWindow.Focused)
                        SoundPlayer.Play("hit");
                }
            }
            else if (VFXObjects)
            {
                List<MapObject> selected = selecting ? new() : CurrentMap.VfxObjects.Selected;
                int count = CurrentMap.VfxObjects.Count;

                c1Set = new string[count + CurrentMap.TimingPoints.Count];
                c2Set = new string[c1Set.Length];
                xSet = new int[c1Set.Length];
                nCount = count;

                Vector4[] iconOffsets = Pool.Rent(count);
                Vector4[] objOffsets = Pool.Rent(count);
                Vector4[] objDurations = Pool.Rent(count);
                Vector4[] objDiamonds = Pool.Rent(count);
                Vector4[] textLineOffsets = Pool.Rent(count);

                Vector4 objHoverOffset = (-1920, 0, 0, 0);
                List<Vector4> objSelectOffsets = new();
                List<Vector4> objDragOffsets = new();

                Vector4 objDurationHoverOffset = (-1920, 0, 0, 0);
                Vector4 objDurationSelectOffset = (-1920, 0, 0, 0);

                List<int> indices = new() { 0, 0 };

                // map objects
                for (int i = 0; i < count; i++)
                {
                    MapObject obj = CurrentMap.VfxObjects[i];
                    float a = obj.Ms < currentTime - 1 ? 0.35f : 1f;

                    float x = cursorX - posX + obj.Ms / 1000f * noteStep;

                    int c = i % colorCount;

                    iconOffsets[i] = (x, c, obj.ID, a);
                    objOffsets[i] = (x, 0, a, c);
                    textLineOffsets[i] = (x, 0, a, 4);

                    if (indices.Count <= obj.ID)
                    {
                        for (int j = indices.Count - 1; j < obj.ID; j++)
                            indices.Add(0);
                    }

                    indices[obj.ID]++;

                    if (obj.ID != 10)
                    {
                        float w = obj.Duration / 1000f * noteStep;

                        objDurations[i] = (x + noteSize, 0, a + Math.Max(2 * (int)(w - noteSize), 0), c);
                        objDiamonds[i] = (x + w, 0, a, c);

                        bool diamondHovering = mouse.X > x + w - noteSize / 4 && mouse.X < x + w + noteSize / 4 && mouse.Y > cellGap + noteSize / 4 && mouse.Y < cellGap + 3 * noteSize / 4;

                        if (diamondHovering)
                        {
                            objDurationHoverOffset = (x + w, 0, 1, 5);
                            HoveringObjDuration = obj;
                        }
                        else if (DraggingObjDuration == obj)
                            objDurationSelectOffset = (x + w, 0, 1, 6);
                    }

                    bool hovering = HoveringObjDuration == null && DraggingObjDuration == null && HoveringVfx == null && DraggingVfx == null && HoverIntersecting(mouse.X, mouse.Y, x);

                    bool objSelected = selecting && SelectIntersecting(x);
                    if (objSelected)
                        selected.Add(obj);

                    if (selecting)
                        obj.Selected = objSelected;
                    objSelected |= obj.Selected;

                    if (hovering)
                    {
                        objHoverOffset = (x, 0, 1, 5);
                        HoveringVfx = obj;
                    }
                    else if (objSelected)
                    {
                        if (DraggingVfx == null)
                            objSelectOffsets.Add((x, 0, 1, 6));
                        else
                        {
                            float dragX = cursorX - posX + obj.DragStartMs / 1000f * noteStep;
                            objDragOffsets.Add((dragX, 0, 1, 7));
                        }
                    }

                    c1Set[i] = $"{obj.Name ?? "null"} {indices[obj.ID]:##,###}";
                    c2Set[i] = $"{obj.Ms:##,##0}ms";
                    xSet[i] = (int)x + 3;

                    c1Len += c1Set[i].Length;
                    c2Len += c2Set[i].Length;
                }

                GL.UseProgram(Shader.TrackProgram);
                RegisterData(15, objDiamonds, count);

                Pool.Return(objDiamonds);

                GL.UseProgram(Shader.XScalingProgram);
                RegisterData(16, objDurations, count);

                Pool.Return(objDurations);

                GL.UseProgram(Shader.TrackProgram);
                RegisterData(14, objOffsets, count);

                Pool.Return(objOffsets);

                GL.ActiveTexture(TextureUnit.Texture3);
                GL.BindTexture(TextureTarget.Texture2d, spriteSheet);
                GL.UseProgram(Shader.IconTexProgram);
                RegisterData(19, iconOffsets, count);

                Pool.Return(iconOffsets);

                GL.UseProgram(Shader.TimelineProgram);
                RegisterData(13, textLineOffsets, count);

                Pool.Return(textLineOffsets);

                RegisterData(2, objSelectOffsets.ToArray());
                RegisterData(3, new Vector4[1] { objHoverOffset });
                RegisterData(6, objDragOffsets.ToArray());

                RegisterData(17, new Vector4[1] { objDurationSelectOffset });
                RegisterData(18, new Vector4[1] { objDurationHoverOffset });

                if (selecting)
                    CurrentMap.VfxObjects.Selected = new(selected);
            }
            else
            {
                List<MapObject> selected = selecting ? new() : CurrentMap.SpecialObjects.Selected;
                int count = CurrentMap.SpecialObjects.Count;

                c1Set = new string[count + CurrentMap.TimingPoints.Count];
                c2Set = new string[c1Set.Length];
                xSet = new int[c1Set.Length];
                nCount = count;

                Vector4[] iconOffsets = Pool.Rent(count);
                Vector4[] objOffsets = Pool.Rent(count);
                Vector4[] textLineOffsets = Pool.Rent(count);

                Vector4 objHoverOffset = (-1920, 0, 0, 0);
                List<Vector4> objSelectOffsets = new();
                List<Vector4> objDragOffsets = new();

                List<float?> lastRenderedSet = new() { 0, 0 };
                List<int> indices = new() { 0, 0 };

                MapObject? closest = null;

                // map objects
                for (int i = 0; i < count; i++)
                {
                    MapObject obj = CurrentMap.SpecialObjects[i];
                    float a = obj.Ms < currentTime - 1 ? 0.35f : 1f;

                    float x = cursorX - posX + obj.Ms / 1000f * noteStep;

                    int c = i % colorCount;

                    iconOffsets[i] = (x, c, obj.ID, a);
                    objOffsets[i] = (x, 0, a, c);
                    textLineOffsets[i] = (x, 0, a, 4);

                    if (indices.Count <= obj.ID)
                    {
                        for (int j = indices.Count - 1; j < obj.ID; j++)
                        {
                            lastRenderedSet.Add(null);
                            indices.Add(0);
                        }
                    }

                    indices[obj.ID]++;

                    bool hovering = HoveringObjDuration == null && DraggingObjDuration == null && HoveringSpec == null && DraggingSpec == null && HoverIntersecting(mouse.X, mouse.Y, x);

                    bool objSelected = selecting && SelectIntersecting(x);
                    if (objSelected)
                        selected.Add(obj);

                    if (obj.Ms <= currentTime - sfxOffset)
                        closest = obj;

                    if (selecting)
                        obj.Selected = objSelected;
                    objSelected |= obj.Selected;

                    if (hovering)
                    {
                        objHoverOffset = (x, 0, 1, 5);
                        HoveringSpec = obj;
                    }
                    else if (objSelected)
                    {
                        if (DraggingSpec == null)
                            objSelectOffsets.Add((x, 0, 1, 6));
                        else
                        {
                            float dragX = cursorX - posX + obj.DragStartMs / 1000f * noteStep;
                            objDragOffsets.Add((dragX, 0, 1, 7));
                        }
                    }

                    c1Set[i] = $"{obj.Name ?? "null"} {indices[obj.ID]:##,###}";
                    c2Set[i] = $"{obj.Ms:##,##0}ms";
                    xSet[i] = (int)x + 3;

                    c1Len += c1Set[i].Length;
                    c2Len += c2Set[i].Length;
                }

                GL.UseProgram(Shader.TrackProgram);
                RegisterData(14, objOffsets, count);

                Pool.Return(objOffsets);

                GL.ActiveTexture(TextureUnit.Texture3);
                GL.BindTexture(TextureTarget.Texture2d, spriteSheet);
                GL.UseProgram(Shader.IconTexProgram);
                RegisterData(19, iconOffsets, count);

                Pool.Return(iconOffsets);

                GL.UseProgram(Shader.TimelineProgram);
                RegisterData(13, textLineOffsets, count);

                Pool.Return(textLineOffsets);

                RegisterData(2, objSelectOffsets.ToArray());
                RegisterData(3, new Vector4[1] { objHoverOffset });
                RegisterData(6, objDragOffsets.ToArray());

                if (selecting)
                    CurrentMap.SpecialObjects.Selected = new(selected);

                //play hit sound
                if (LastPlayed != closest)
                {
                    LastPlayed = closest;

                    if (closest != null && MusicPlayer.IsPlaying && MainWindow.Focused)
                        SoundPlayer.Play("hit");
                }
            }

            // bpm lines
            double multiplier = beatDivisor % 1 == 0 ? 1f : 1f / (beatDivisor % 1);
            int divisor = (int)Math.Round(beatDivisor * multiplier);
            bool doubleDiv = divisor % 2 == 0;
            int divOff = divisor - 1 - (doubleDiv ? 1 : 0);

            int numPoints = CurrentMap.TimingPoints.Count;
            int numFull = 0, numHalf = 0, numSub = 0;

            Vector3i[] metrics = new Vector3i[numPoints];
            Vector3i current = (0, 0, 0);

            for (int i = 0; i < numPoints; i++)
            {
                TimingPoint point = CurrentMap.TimingPoints[i];
                if (point.BPM <= 0 || point.Ms > totalTime)
                    continue;

                double nextMs = i + 1 < numPoints ? Math.Min(CurrentMap.TimingPoints[i + 1].Ms, totalTime) : totalTime;
                double totalMs = nextMs - point.Ms;

                double stepMs = 60000 / point.BPM * multiplier;

                int full = (int)(totalMs / stepMs);
                int half = doubleDiv ? (int)(totalMs / (stepMs / 2) - full) : 0;
                int sub = (int)(totalMs / (stepMs / divisor) - full - half);

                metrics[i] = (full, half, sub);

                numFull += full;
                numHalf += half;
                numSub += sub;
            }

            Vector4[] startBpmOffsets = Pool.Rent(numPoints);
            Vector4[] fullBpmOffsets = Pool.Rent(numFull);
            Vector4[] halfBpmOffsets = Pool.Rent(numHalf);
            Vector4[] subBpmOffsets = Pool.Rent(numSub);

            Vector4 pointHoverOffset = (-1920, 0, 0, 0);
            Vector4 pointSelectOffset = (-1920, 0, 0, 0);

            for (int i = 0; i < numPoints; i++)
            {
                TimingPoint point = CurrentMap.TimingPoints[i];
                int index = c1Set.Length - numPoints + i;

                if (point.BPM == 0)
                {
                    c1Set[index] = "";
                    c2Set[index] = "";
                    continue;
                }

                Vector3i pointMetrics = metrics[i];

                double stepMs = 60000 / point.BPM * multiplier;
                double halfStep = stepMs / 2;
                double stepSmall = stepMs / divisor;
                double curStep = stepSmall;
                double lineXD = cursorX - posX + point.Ms / 1000f * noteStep;
                float lineX = (float)lineXD;
                startBpmOffsets[i] = (lineX, 0, 1, 8);
                double x;

                bool hovering = !RenderMapObjects && HoveringPoint == null && DraggingPoint == null && mouse.X > lineX && mouse.X < lineX + 72 && mouse.Y > Rect.Height && mouse.Y < Rect.Height + 52;
                bool pointSelected = CurrentMap.SelectedPoint == point;

                c1Set[index] = $"{point.BPM:##,###.###} BPM";
                c2Set[index] = $"{point.Ms:##,##0}ms";
                xSet[index] = (int)lineX + 3;

                c1Len += c1Set[index].Length;
                c2Len += c2Set[index].Length;

                if (hovering)
                {
                    pointHoverOffset = (lineX, 0, 1, 5);
                    HoveringPoint = point;
                }
                else if (pointSelected)
                    pointSelectOffset = (lineX, 0, 1, 6);

                for (int j = 0; j < pointMetrics.X; j++)
                {
                    x = lineXD + stepMs * (j + 1) / 1000f * noteStep;
                    fullBpmOffsets[current.X + j] = ((float)x, 0, 1, 1);
                }

                for (int j = 0; j < pointMetrics.Y; j++)
                {
                    x = lineXD + (stepMs * j + halfStep) / 1000f * noteStep;
                    halfBpmOffsets[current.Y + j] = ((float)x, 0, 1, 2);
                }

                for (int j = 0; j < pointMetrics.Z; j++)
                {
                    int cur = j % divOff;

                    if (cur == 0 && j > 0)
                        curStep += stepSmall;
                    else if (cur + 1 == divisor / 2 && doubleDiv)
                        curStep += stepSmall;

                    x = lineXD + (stepSmall * j + curStep) / 1000f * noteStep;
                    subBpmOffsets[current.Z + j] = ((float)x, 0, 1, 0);
                }

                current += pointMetrics;
            }

            RegisterData(7, startBpmOffsets, numPoints);
            RegisterData(8, fullBpmOffsets, numFull);
            RegisterData(9, halfBpmOffsets, numHalf);
            RegisterData(10, subBpmOffsets, numSub);
            RegisterData(11, new Vector4[1] { pointHoverOffset });
            RegisterData(12, new Vector4[1] { pointSelectOffset });

            Pool.Return(startBpmOffsets);
            Pool.Return(fullBpmOffsets);
            Pool.Return(halfBpmOffsets);
            Pool.Return(subBpmOffsets);

            color1Texts = RenderC1Array(c1Set, c1Len, xSet, nCount);
            color2Texts = RenderC2Array(c2Set, c2Len, xSet, nCount);
        }

        private float prevHeight = 0;

        public override void Render(float mousex, float mousey, float frametime)
        {
            //Console.WriteLine(frametime * 1000);
            if (prevHeight != Rect.Height)
            {
                InstanceSetup();

                prevHeight = Rect.Height;
            }

            Update();

            // render background
            GL.UseProgram(Shader.Program);

            GL.BindVertexArray(VaO);
            int offset = 30;
            GL.DrawArrays(PrimitiveType.Triangles, 0, offset);

            // render waveform
            if (Settings.waveform.Value)
                Waveform.Render(PosSet, Rect.Height);

            // render dynamic elements
            GenerateOffsets();

            // render static elements
            GL.UseProgram(Shader.Program);

            GL.BindVertexArray(VaO);
            int length = Indices["rectLength"] + Indices["loopLength"] + Indices["lineLength"] - 30;
            GL.DrawArrays(PrimitiveType.Triangles, offset, length);

            float currentTime = Settings.currentTime.Value.Value;
            float sfxOffset = Settings.sfxOffset.Value;

            //play metronome
            if (Settings.metronome.Value && MainWindow.Focused)
            {
                float ms = currentTime - sfxOffset;
                float beat = Timing.GetClosestBeat(ms);

                if (lastPlayedTick != beat && beat <= ms && beat > 0 && MusicPlayer.IsPlaying)
                {
                    lastPlayedTick = beat;

                    SoundPlayer.Play("metronome");
                }
            }
        }

        public override void RenderTexture()
        {
            Color color1 = Settings.color1.Value;
            Color color2 = Settings.color2.Value;

            GL.Uniform4f(TexColorLocation, color1.R / 255f, color1.G / 255f, color1.B / 255f, color1.A / 255f);
            FontRenderer.RenderData("main", color1Texts);
            GL.Uniform4f(TexColorLocation, color2.R / 255f, color2.G / 255f, color2.B / 255f, color2.A / 255f);
            FontRenderer.RenderData("main", color2Texts);
        }

        private List<float> vertices = new();

        private List<float> loops = new();
        private List<float> lines = new();

        private Vector4[] color1Texts;
        private Vector4[] color2Texts;

        public override Tuple<float[], float[]> GetVertices()
        {
            loops = new();
            lines = new();

            vertices = new(GLU.Rect(Rect, 0.15f, 0.15f, 0.15f, Settings.trackOpacity.Value / 255f));
            loops.AddRange(GLU.OutlineAsTriangles(Rect, 1, 0.2f, 0.2f, 0.2f));

            MainWindow editor = MainWindow.Instance;
            Point mouse = editor.Mouse;
            float noteStep = CurrentMap.NoteStep;

            Color sc2 = Settings.color2.Value;
            float[] color2 = new float[] { sc2.R / 255f, sc2.G / 255f, sc2.B / 255f };

            float currentTime = Settings.currentTime.Value.Value;
            float totalTime = Settings.currentTime.Value.Max;

            float posX = currentTime / 1000f * noteStep;
            float maxX = totalTime / 1000f * noteStep;
            float cursorX = Rect.Width * Settings.cursorPos.Value.Value / 100f;
            float endX = cursorX - posX + maxX + 1;

            StartPos = Math.Max(0, (-cursorX * 1000f / noteStep + currentTime) / totalTime);
            EndPos = Math.Min(1, ((Rect.Width - cursorX) * 1000f / noteStep + currentTime) / totalTime);

            PosSet = (-posX + cursorX, endX, StartPos, EndPos);

            HoveringNote = null;
            HoveringPoint = null;
            HoveringVfx = null;
            HoveringSpec = null;
            HoveringObjDuration = null;

            selecting = editor.RightHeld && RightDraggingTrack;
            selectHitbox = new RectangleF();

            if (selecting)
            {
                float offsetMs = DragStartMs - currentTime;
                float startX = DragStartPoint.X + offsetMs / 1000f * noteStep;

                float my = MathHelper.Clamp(mouse.Y, 0f, Rect.Height);
                float x = Math.Min(mouse.X, startX);
                float y = Math.Min(my, DragStartPoint.Y);
                float w = Math.Max(mouse.X, startX) - x;
                float h = Math.Min(Rect.Height, Math.Max(my, DragStartPoint.Y) - y);

                selectHitbox = new RectangleF(x, y, w, h);

                vertices.AddRange(GLU.Rect(selectHitbox, 0f, 1f, 0.2f, 0.2f));
                loops.AddRange(GLU.OutlineAsTriangles(selectHitbox, 1, 0f, 1f, 0.2f, 1f));
            }

            //render static lines
            lines.AddRange(GLU.Line(cursorX - posX, 0, cursorX - posX, Rect.Height, 1, color2));
            lines.AddRange(GLU.Line(endX, 0, endX, Rect.Height, 1, 1f, 0f, 0f));
            lines.AddRange(GLU.Line(cursorX, 4, cursorX, Rect.Height - 4, 1, 1f, 1f, 1f, 0.75f));

            Indices["rectLength"] = vertices.Count / 6;
            Indices["loopLength"] = loops.Count / 6;
            Indices["lineLength"] = lines.Count / 6;

            vertices.AddRange(loops);
            vertices.AddRange(lines);

            return new(vertices.ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            if (right)
                OnMouseUp(pos);
            else
                RightDraggingTrack = false;

            long startMs = (long)Settings.currentTime.Value.Value;
            MainWindow editor = MainWindow.Instance;

            bool replayf = MusicPlayer.IsPlaying && !right;
            replay = false;

            if (replayf)
                MusicPlayer.Pause();

            if (HoveringNote != null && !right)
            {
                DraggingNote = HoveringNote;

                DragStartPoint = pos;
                DragStartMs = startMs;

                List<Note> selected = CurrentMap.Notes.Selected;

                if (editor.ShiftHeld && selected.Count > 0)
                {
                    selected = new() { selected[0] };

                    Note first = selected[0];
                    Note last = HoveringNote;
                    long min = Math.Min(first.Ms, last.Ms);
                    long max = Math.Max(first.Ms, last.Ms);

                    foreach (Note note in CurrentMap.Notes)
                        if (note.Ms >= min && note.Ms <= max && !selected.Contains(note))
                            selected.Add(note);
                }
                else if (editor.CtrlHeld)
                {
                    if (!selected.Remove(HoveringNote))
                        selected.Add(HoveringNote);
                }
                else if (!selected.Contains(HoveringNote))
                    selected = new List<Note>() { HoveringNote };

                CurrentMap.Notes.Selected = new(selected);
                draggingNotes = selected;

                foreach (Note note in draggingNotes)
                    note.DragStartMs = note.Ms;
            }
            else if (HoveringPoint != null && !right)
            {
                DraggingPoint = HoveringPoint;

                DragStartPoint = pos;
                DragStartMs = startMs;

                CurrentMap.SelectedPoint = HoveringPoint;

                DraggingPoint.DragStartMs = DraggingPoint.Ms;
            }
            else if (HoveringObjDuration != null && !right)
            {
                DraggingObjDuration = HoveringObjDuration;

                DragStartPoint = pos;
                DragStartMs = startMs;

                CurrentMap.SelectedObjDuration = HoveringObjDuration;

                DraggingObjDuration.DragStartMs = DraggingObjDuration.Duration;
            }
            else if (HoveringVfx != null && !right)
            {
                DraggingVfx = HoveringVfx;

                DragStartPoint = pos;
                DragStartMs = startMs;

                List<MapObject> selected = CurrentMap.VfxObjects.Selected;

                if (editor.ShiftHeld && selected.Count > 0)
                {
                    selected = new() { selected[0] };

                    MapObject first = selected[0];
                    MapObject last = HoveringVfx;
                    long min = Math.Min(first.Ms, last.Ms);
                    long max = Math.Max(first.Ms, last.Ms);

                    foreach (MapObject obj in CurrentMap.VfxObjects)
                        if (obj.Ms >= min && obj.Ms <= max && !selected.Contains(obj))
                            selected.Add(obj);
                }
                else if (editor.CtrlHeld)
                {
                    if (!selected.Remove(HoveringVfx))
                    {
                        if (selected.Count == 0 && editor.CurrentWindow is GuiWindowEditor gse)
                            gse.ShowVFXSettings(HoveringVfx);
                        selected.Add(HoveringVfx);
                    }
                }
                else if (!selected.Contains(HoveringVfx))
                {
                    if (editor.CurrentWindow is GuiWindowEditor gse)
                        gse.ShowVFXSettings(HoveringVfx);
                    selected = new List<MapObject>() { HoveringVfx };
                }

                CurrentMap.VfxObjects.Selected = new(selected);
                draggingVfxs = selected;

                foreach (MapObject obj in draggingVfxs)
                    obj.DragStartMs = obj.Ms;
            }
            else if (HoveringSpec != null && !right)
            {
                DraggingSpec = HoveringSpec;

                DragStartPoint = pos;
                DragStartMs = startMs;

                List<MapObject> selected = CurrentMap.SpecialObjects.Selected;

                if (editor.ShiftHeld && selected.Count > 0)
                {
                    selected = new() { selected[0] };

                    MapObject first = selected[0];
                    MapObject last = HoveringSpec;
                    long min = Math.Min(first.Ms, last.Ms);
                    long max = Math.Max(first.Ms, last.Ms);

                    foreach (MapObject obj in CurrentMap.SpecialObjects)
                        if (obj.Ms >= min && obj.Ms <= max && !selected.Contains(obj))
                            selected.Add(obj);
                }
                else if (editor.CtrlHeld)
                {
                    if (!selected.Remove(HoveringSpec))
                    {
                        if (selected.Count == 0 && editor.CurrentWindow is GuiWindowEditor gse)
                            gse.ShowSpecialSettings(HoveringSpec);
                        selected.Add(HoveringSpec);
                    }
                }
                else if (!selected.Contains(HoveringSpec))
                {
                    if (editor.CurrentWindow is GuiWindowEditor gse)
                        gse.ShowSpecialSettings(HoveringSpec);
                    selected = new() { HoveringSpec };
                }

                CurrentMap.SpecialObjects.Selected = new(selected);
                draggingSpecs = selected;

                foreach (MapObject obj in draggingSpecs)
                    obj.DragStartMs = obj.Ms;
            }
            else
            {
                replay = replayf;

                DragStartPoint = pos;
                DragStartMs = startMs;
            }

            RightDraggingTrack |= right && Rect.Contains(pos);
            DraggingTrack |= !right;
        }

        public override void OnMouseMove(Point pos)
        {
            if (DraggingTrack)
            {
                SliderSetting currentTime = Settings.currentTime.Value;
                float divisor = Settings.beatDivisor.Value.Value;
                float cursorPos = Settings.cursorPos.Value.Value;

                float cellStep = CurrentMap.NoteStep;

                float offset = (pos.X - DragStartPoint.X) / cellStep * 1000f;
                float cursorms = (pos.X - Rect.Width * cursorPos / 100f - noteSize / 2f) / cellStep * 1000f + currentTime.Value;

                if (DraggingNote != null)
                {
                    offset = DraggingNote.DragStartMs - cursorms;
                    offset = Math.Abs(offset) / 1000f * cellStep <= 5f ? 0 : offset;
                    float currentBpm = Timing.GetCurrentBpm(cursorms).BPM;

                    if (currentBpm > 0)
                    {
                        float stepX = 60f / currentBpm * cellStep;
                        float stepXSmall = stepX / divisor;

                        float threshold = MathHelper.Clamp(stepXSmall / 1.75f, 1f, 12f);
                        float snappedMs = Timing.GetClosestBeat(DraggingNote.Ms);

                        if (Math.Abs(snappedMs - cursorms) / 1000f * cellStep <= threshold)
                            offset = DraggingNote.DragStartMs - snappedMs;
                    }

                    foreach (Note note in draggingNotes)
                        note.Ms = (long)MathHelper.Clamp(note.DragStartMs - offset, 0f, currentTime.Max);

                    CurrentMap.Notes.Sort();
                }
                else if (DraggingPoint != null)
                {
                    offset = DraggingPoint.DragStartMs - cursorms;
                    float currentBpm = Timing.GetCurrentBpm(cursorms).BPM;

                    float stepX = 60f / currentBpm * cellStep;
                    float stepXSmall = stepX / divisor;

                    float threshold = MathHelper.Clamp(stepXSmall / 1.75f, 1f, 12f);
                    float snappedMs = Timing.GetClosestBeat(DraggingPoint.Ms, true);
                    float snappedNote = CurrentMap.Notes.GetClosest(DraggingPoint.Ms);

                    if (Math.Abs(snappedNote - cursorms) < Math.Abs(snappedMs - cursorms))
                        snappedMs = snappedNote;
                    if (Math.Abs(snappedMs - cursorms) / 1000f * cellStep <= threshold)
                        offset = DraggingPoint.DragStartMs - snappedMs;
                    if (Math.Abs(cursorms) / 1000f * cellStep <= threshold)
                        offset = DraggingPoint.DragStartMs;

                    DraggingPoint.Ms = (long)Math.Min(DraggingPoint.DragStartMs - offset, currentTime.Max);

                    CurrentMap.SortTimings(false);
                }
                else if (DraggingObjDuration != null)
                {
                    MapObject obj = DraggingObjDuration;
                    cursorms += noteSize / 2 / cellStep * 1000f;

                    offset = obj.DragStartMs - cursorms + obj.Ms;
                    offset = Math.Abs(offset) / 1000f * cellStep <= 5f ? 0 : offset;
                    float currentBpm = Timing.GetCurrentBpm(cursorms).BPM;

                    if (currentBpm > 0)
                    {
                        float stepX = 60f / currentBpm * cellStep;
                        float stepXSmall = stepX / divisor;

                        float threshold = MathHelper.Clamp(stepXSmall / 1.75f, 1f, 12f);
                        float snappedMs = Timing.GetClosestBeat(obj.Ms + obj.Duration);

                        if (Math.Abs(snappedMs - cursorms) / 1000f * cellStep <= threshold)
                            offset = obj.DragStartMs - snappedMs + obj.Ms;
                    }

                    obj.Duration = (long)Math.Max(obj.DragStartMs - offset, 0);
                }
                else if (DraggingVfx != null)
                {
                    offset = DraggingVfx.DragStartMs - cursorms;
                    offset = Math.Abs(offset) / 1000f * cellStep <= 5f ? 0 : offset;
                    float currentBpm = Timing.GetCurrentBpm(cursorms).BPM;

                    if (currentBpm > 0)
                    {
                        float stepX = 60f / currentBpm * cellStep;
                        float stepXSmall = stepX / divisor;

                        float threshold = MathHelper.Clamp(stepXSmall / 1.75f, 1f, 12f);
                        float snappedMs = Timing.GetClosestBeat(DraggingVfx.Ms);

                        if (Math.Abs(snappedMs - cursorms) / 1000f * cellStep <= threshold)
                            offset = DraggingVfx.DragStartMs - snappedMs;
                    }

                    foreach (MapObject obj in draggingVfxs)
                        obj.Ms = (long)MathHelper.Clamp(obj.DragStartMs - offset, 0f, currentTime.Max);

                    CurrentMap.VfxObjects.Sort();
                }
                else if (DraggingSpec != null)
                {
                    offset = DraggingSpec.DragStartMs - cursorms;
                    offset = Math.Abs(offset) / 1000f * cellStep <= 5f ? 0 : offset;
                    float currentBpm = Timing.GetCurrentBpm(cursorms).BPM;

                    if (currentBpm > 0)
                    {
                        float stepX = 60f / currentBpm * cellStep;
                        float stepXSmall = stepX / divisor;

                        float threshold = MathHelper.Clamp(stepXSmall / 1.75f, 1f, 12f);
                        float snappedMs = Timing.GetClosestBeat(DraggingSpec.Ms);

                        if (Math.Abs(snappedMs - cursorms) / 1000f * cellStep <= threshold)
                            offset = DraggingSpec.DragStartMs - snappedMs;
                    }

                    foreach (MapObject obj in draggingSpecs)
                        obj.Ms = (long)MathHelper.Clamp(obj.DragStartMs - offset, 0f, currentTime.Max);

                    CurrentMap.SpecialObjects.Sort();
                }
                else
                {
                    float finalTime = DragStartMs - offset;

                    if (Timing.GetCurrentBpm(finalTime).BPM > 0)
                        finalTime = Timing.GetClosestBeat(finalTime);

                    finalTime = MathHelper.Clamp(finalTime, 0f, currentTime.Max);

                    currentTime.Value = finalTime;
                }
            }
        }

        public override void OnMouseUp(Point pos)
        {
            if (DraggingTrack)
            {
                MainWindow editor = MainWindow.Instance;

                if (DraggingNote != null && DraggingNote.DragStartMs != DraggingNote.Ms)
                {
                    long msDiff = DraggingNote.Ms - DraggingNote.DragStartMs;

                    foreach (Note note in draggingNotes)
                        note.Ms -= msDiff;

                    NoteManager.Edit("MOVE NOTE[S]", n => n.Ms += msDiff);
                }
                else if (DraggingPoint != null && DraggingPoint.DragStartMs != DraggingPoint.Ms)
                {
                    TimingPoint point = DraggingPoint;
                    long ms = point.Ms;
                    point.Ms = point.DragStartMs;

                    PointManager.Edit("MOVE POINT", n => n.Ms = ms);
                }
                else if (DraggingObjDuration != null && DraggingObjDuration.DragStartMs != DraggingObjDuration.Duration)
                {
                    MapObject obj = DraggingObjDuration;
                    long ms = obj.Duration;
                    obj.Duration = obj.DragStartMs;

                    VfxObjectManager.Edit("MOVE OBJ DURATION", [obj], n => n.Duration = ms);
                }
                else if (DraggingVfx != null && DraggingVfx.DragStartMs != DraggingVfx.Ms)
                {
                    long msDiff = DraggingVfx.Ms - DraggingVfx.DragStartMs;

                    foreach (MapObject obj in draggingVfxs)
                        obj.Ms -= msDiff;

                    VfxObjectManager.Edit("MOVE OBJECT[S]", n => n.Ms += msDiff);
                }
                else if (DraggingSpec != null && DraggingSpec.DragStartMs != DraggingSpec.Ms)
                {
                    long msDiff = DraggingSpec.Ms - DraggingSpec.DragStartMs;

                    foreach (MapObject obj in draggingSpecs)
                        obj.Ms -= msDiff;

                    SpecialObjectManager.Edit("MOVE OBJECT[S]", n => n.Ms += msDiff);
                }

                DraggingTrack = false;
                DraggingNote = null;
                DraggingPoint = null;
                DraggingVfx = null;
                DraggingSpec = null;
                DraggingObjDuration = null;

                if (replay)
                    MusicPlayer.Play();
            }

            if (RightDraggingTrack)
                RightDraggingTrack = false;
        }
    }
}
