using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics;
using New_SSQE.Objects;
using System.Buffers;
using New_SSQE.GUI.Font;
using New_SSQE.GUI.Shaders;
using New_SSQE.Objects.Managers;
using New_SSQE.Preferences;
using New_SSQE.Misc.Easing;
using New_SSQE.Maps;

namespace New_SSQE.GUI
{
    internal class GuiGrid : WindowControl
    {
        public static bool RenderMapObjects = false;
        public static bool VFXObjects = true;

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

        private VertexArrayHandle altVaO;
        private BufferHandle altVbO;
        private readonly int altVertCount;

        private VertexArrayHandle blurVaO;
        private BufferHandle blurVbO;

        private static FramebufferHandle FBO;
        private static FramebufferHandle MFBO;
        private static RenderbufferHandle RBO;
        private static TextureHandle fboTexture;

        private VertexArrayHandle autoplayVaO;
        private BufferHandle autoplayVbO;

        private readonly ArrayPool<Vector4> Pool = ArrayPool<Vector4>.Shared;

        static GuiGrid()
        {
            RBO = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBO);

            MFBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, MFBO);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, RBO);

            FBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
            fboTexture = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2d, fboTexture);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, fboTexture, 0);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RenderbufferHandle.Zero);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle.Zero);
        }

        public GuiGrid(float w, float h) : base(0, 0, w, h)
        {
            Dynamic = true;

            List<float> altVerts = new();
            altVerts.AddRange(GLU.OutlineAsTriangles(-1.5f, -1.5f, 3f, 3f, 0.02f, 0.5f, 0.5f, 0.5f, 0.5f));
            altVerts.AddRange(GLU.Line(-0.5f, -1.5f, -0.5f, 1.5f, 0.01f, 0.5f, 0.5f, 0.5f, 0.5f));
            altVerts.AddRange(GLU.Line(0.5f, -1.5f, 0.5f, 1.5f, 0.01f, 0.5f, 0.5f, 0.5f, 0.5f));
            altVerts.AddRange(GLU.Line(-1.5f, -0.5f, 1.5f, -0.5f, 0.01f, 0.5f, 0.5f, 0.5f, 0.5f));
            altVerts.AddRange(GLU.Line(-1.5f, 0.5f, 1.5f, 0.5f, 0.01f, 0.5f, 0.5f, 0.5f, 0.5f));

            altVertCount = altVerts.Count;

            altVaO = GL.GenVertexArray();
            altVbO = GL.GenBuffer();

            GL.BindVertexArray(altVaO);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, altVbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, altVerts.ToArray(), BufferUsageARB.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            float[] blurVerts = GLU.TexturedRectNoAlpha(-1f, -1f, 2f, 2f);

            blurVaO = GL.GenVertexArray();
            blurVbO = GL.GenBuffer();

            GL.BindVertexArray(blurVaO);
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, blurVbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, blurVerts, BufferUsageARB.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.UseProgram(Shader.VFXFBOProgram);
            int location = GL.GetUniformLocation(Shader.VFXFBOProgram, "texture0");
            GL.Uniform1i(location, 3);

            autoplayVaO = GL.GenVertexArray();
            autoplayVbO = GL.GenBuffer();

            GL.BindVertexArray(autoplayVaO);
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, autoplayVbO);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            InstanceSetup();
            Init();
        }

        public override void InstanceSetup()
        {
            ClearBuffers();

            VaOs = new VertexArrayHandle[8];
            VbOs = new BufferHandle[16];
            VertexCounts = new int[8];

            float[] noteVerts = GLU.OutlineAsTriangles(0, 0, 75, 75, 2, 1f, 1f, 1f, 1f);
            float[] fillVerts = GLU.Rect(0, 0, 75, 75, 1f, 1f, 1f, 0.15f);
            noteVerts = noteVerts.Concat(fillVerts).ToArray();

            float[] previewFillVerts = GLU.Rect(0, 0, 65, 65, 1f, 1f, 1f, 0.125f);
            float[] previewVerts = GLU.OutlineAsTriangles(0, 0, 65, 65, 2, 1f, 1f, 1f, 0.09375f).Concat(previewFillVerts).ToArray();

            float[] approachVerts = GLU.OutlineAsTriangles(0, 0, 1, 1, 0.0125f, 1f, 1f, 1f, 1f);
            float[] selectVerts = GLU.OutlineAsTriangles(0, 0, 83, 83, 2, 1f, 1f, 1f, 1f);
            float[] hoverVerts = GLU.OutlineAsTriangles(0, 0, 83, 83, 2, 1f, 1f, 1f, 0.25f);

            float[] altNoteVerts = GLU.OutlineAsTriangles(-0.375f, -0.375f, 0.75f, 0.75f, 0.125f, 1f, 1f, 1f, 1f);

            Color c5 = Settings.color5.Value;
            float[] beatVerts = GLU.OutlineAsTriangles(-5, -5, 310, 310, 3f, c5.R / 255f, c5.G / 255f, c5.B / 255f, 1f);
            float[] approachBeatVerts = GLU.OutlineAsTriangles(0, 0, 1, 1, 0.0075f, c5.R / 255f, c5.G / 255f, c5.B / 255f, 1f);

            AddToBuffers(noteVerts, 0);
            AddToBuffers(approachVerts, 1);
            AddToBuffers(selectVerts, 2);
            AddToBuffers(hoverVerts, 3);
            AddToBuffers(previewVerts, 4);

            AddToBuffers(altNoteVerts, 5);
            AddToBuffers(beatVerts, 6);
            AddToBuffers(approachBeatVerts, 7);
        }

        private readonly List<Vector4> previewNoteOffsets = new();
        private readonly List<float> bezierVertices = new();

        private static void RenderColorArray(string[] set, float[] a, int[] x, int[] y, int len, out Vector4[] color, out float[] alpha)
        {
            int pos = 0;

            color = new Vector4[len];
            alpha = new float[len];

            for (int i = 0; i < set.Length; i++)
            {
                FontRenderer.PrintInto(color, pos, x[i], y[i], set[i], 28, "main");

                for (int j = 0; j < set[i].Length; j++)
                    alpha[pos + j] = a[i];
                pos += set[i].Length;
            }
        }

        public override void GenerateOffsets()
        {
            string[] c2Set;
            float[] aSet;
            int[] xSet;
            int[] ySet;

            int c2Len = 0;

            MainWindow editor = MainWindow.Instance;
            Point mouse = editor.Mouse;

            float cellSize = Rect.Width / 3f;
            float noteSize = cellSize * 0.75f;
            float cellGap = (cellSize - noteSize) / 2f;

            float currentTime = Settings.currentTime.Value.Value;
            float approachRate = (Settings.approachRate.Value.Value + 1f) / 10f;
            float maxMs = currentTime + 1000f / approachRate;

            HoveringNote = null;

            if (!RenderMapObjects || !VFXObjects) {
                Note last = startNote;
                Note? next = null;

                bool approachSquares = Settings.approachSquares.Value;
                bool gridNumbers = Settings.gridNumbers.Value;
                int colorCount = Settings.noteColors.Value.Count;

                bool separateClickTools = Settings.separateClickTools.Value;
                bool selectTool = Settings.selectTool.Value;

                (int low, int high) = CurrentMap.Notes.SearchRange(currentTime, maxMs);
                int range = high - low;
                int nc = gridNumbers ? range : 0;

                c2Set = new string[nc];
                aSet = new float[nc];
                xSet = new int[nc];
                ySet = new int[nc];

                if (CurrentMap.Notes.Count > 0)
                {
                    if (CurrentMap.Notes[low].Ms <= currentTime)
                        last = CurrentMap.Notes[low];
                    else if (low > 0)
                        last = CurrentMap.Notes[low - 1];
                    next = CurrentMap.Notes[low];
                }

                Vector4[] approachOffsets = Pool.Rent(range);
                Vector4[] noteOffsets = Pool.Rent(range);
                List<Vector4> selectOffsets = new();
                Vector4 hoverOffset = (-1920, 0, 0, 0);

                for (int i = low; i < high; i++)
                {
                    Note note = CurrentMap.Notes[i];
                    int index = i - low;

                    float x = Rect.X + (2 - note.X) * cellSize + cellGap;
                    float y = Rect.Y + (2 - note.Y) * cellSize + cellGap;

                    float progress = Math.Min(1f, (float)Math.Pow(1 - Math.Min(1f, (note.Ms - currentTime) * approachRate / 750f), 2f));
                    bool hovering = HoveringNote == null && mouse.X > x && mouse.X < x + noteSize && mouse.Y > y && mouse.Y < y + noteSize;

                    int c = i % colorCount;

                    if (approachSquares)
                    {
                        float outlineSize = 4 + noteSize + noteSize * (1 - progress) * 2 + 0.5f;

                        approachOffsets[index] = (x - outlineSize / 2f + noteSize / 2f, y - outlineSize / 2f + noteSize / 2f, 2 * (int)outlineSize + progress, c);
                    }

                    if (note.Selected)
                        selectOffsets.Add((x - 4, y - 4, progress, 6));

                    if (hovering && !RenderMapObjects && (!separateClickTools || selectTool))
                    {
                        HoveringNote = note;
                        hoverOffset = (x - 4, y - 4, 1, 5);
                    }

                    noteOffsets[index] = (x, y, 2f + progress, c);

                    if (gridNumbers)
                    {
                        string numText = $"{i + 1:##,###}";
                        int width = FontRenderer.GetWidth(numText, 28, "main");
                        int height = FontRenderer.GetHeight(28, "main");

                        c2Set[index] = numText;
                        aSet[index] = 1 - progress;
                        xSet[index] = (int)(x + noteSize / 2f - width / 2f);
                        ySet[index] = (int)(y + noteSize / 2f - height / 2f);

                        c2Len += numText.Length;
                    }
                }

                //render fake note
                if (Hovering && !RenderMapObjects && (!separateClickTools || !selectTool) && (HoveringNote == null || separateClickTools))
                    AddPreviewNote(mouse.X, mouse.Y, 9, true);

                GL.UseProgram(Shader.ScalingProgram);
                RegisterData(0, noteOffsets, range);
                if (approachSquares)
                    RegisterData(1, approachOffsets, range);

                Pool.Return(noteOffsets);
                Pool.Return(approachOffsets);

                GL.UseProgram(Shader.TimelineProgram);
                RegisterData(2, selectOffsets.ToArray());
                RegisterData(3, new Vector4[1] { hoverOffset });
                RegisterData(4, previewNoteOffsets.ToArray());

                //render autoplay cursor
                if (Settings.autoplay.Value)
                {
                    next ??= last;

                    long timeDiff = next.Ms - last.Ms;
                    float timePos = currentTime - last.Ms;

                    float progress = timeDiff == 0 ? 1 : (float)timePos / timeDiff;
                    progress = (float)Math.Sin(progress * MathHelper.PiOver2);

                    float width = (float)Math.Sin(progress * MathHelper.Pi) * 8f + 16;

                    float lx = Rect.X + (2 - last.X) * cellSize;
                    float ly = Rect.Y + (2 - last.Y) * cellSize;

                    float nx = Rect.X + (2 - next.X) * cellSize;
                    float ny = Rect.Y + (2 - next.Y) * cellSize;

                    float x = cellSize / 2f + lx + (nx - lx) * progress;
                    float y = cellSize / 2f + ly + (ny - ly) * progress;

                    float cx = x - width / 2f;
                    float cy = y - width / 2f;

                    float[] verts = GLU.Rect(cx, cy, width, width, 1f, 1f, 1f, 0.25f).Concat(GLU.OutlineAsTriangles(cx, cy, width, width, 2, 1f, 1f, 1f, 1f)).ToArray();

                    GL.UseProgram(Shader.Program);
                    GL.BindVertexArray(autoplayVaO);

                    GL.BindBuffer(BufferTargetARB.ArrayBuffer, autoplayVbO);
                    GL.BufferData(BufferTargetARB.ArrayBuffer, verts, BufferUsageARB.DynamicDraw);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, verts.Length / 6);
                }

                if (RenderMapObjects)
                {
                    List<Vector4> beatOffsets = new();
                    List<Vector4> beatApproach = new();

                    for (int i = 0; i < CurrentMap.SpecialObjects.Count; i++)
                    {
                        MapObject obj = CurrentMap.SpecialObjects[i];
                        bool passed = currentTime > obj.Ms + 1;
                        bool visible = !passed && obj.Ms - currentTime <= 1000f / approachRate;

                        if (!visible)
                            continue;

                        switch (obj.ID)
                        {
                            case 12 when obj is Beat beat:
                                float progress = Math.Min(1f, (float)Math.Pow(1 - Math.Min(1f, (beat.Ms - currentTime) * approachRate / 750f), 2f));

                                if (approachSquares)
                                {
                                    float outlineSize = 314 + 310 * (1 - progress) + 0.5f;

                                    beatApproach.Add((Rect.X + 150 - outlineSize / 2f, Rect.Y + 150 - outlineSize / 2f, outlineSize, progress));
                                }

                                beatOffsets.Add((Rect.X, Rect.Y, 1, progress));
                                break;
                        }
                    }

                    GL.UseProgram(Shader.ColoredProgram);
                    RegisterData(6, beatOffsets.ToArray());
                    RegisterData(7, beatApproach.ToArray());
                }
            }
            else
            {
                c2Set = [];
                aSet = [];
                xSet = [];
                ySet = [];

                float brightness = 0.5f;
                float contrast = 0.5f;
                float saturation = 0.5f;
                float blur = 0f;
                float fov = 1f;
                Color tint = Color.White;
                Vector3 pos = (0, 0, 0);
                float rotation = 0;
                float arFactor = 1;
                string text = "";

                MapObject?[] curObjs = new MapObject[10];

                for (int i = 0; i < CurrentMap.VfxObjects.Count; i++)
                {
                    MapObject obj = CurrentMap.VfxObjects[i];
                    float ms = currentTime;

                    if (obj.Ms > currentTime)
                        continue;
                    if (obj.Ms + obj.Duration >= currentTime)
                        curObjs[obj.ID - 2] = obj;

                    if (i + 1 < CurrentMap.VfxObjects.Count)
                    {
                        MapObject next = CurrentMap.VfxObjects[i + 1];

                        if (next.ID == obj.ID)
                            ms = Math.Min(ms, next.Ms);
                    }
                    
                    switch(obj.ID)
                    {
                        case 2 when obj is Brightness temp:
                            brightness = (float)EasingStyles.Process(brightness, temp.Intensity, (ms - temp.Ms) / temp.Duration, temp.Style, temp.Direction);
                            break;

                        case 3 when obj is Contrast temp:
                            contrast = (float)EasingStyles.Process(contrast, temp.Intensity, (ms - temp.Ms) / temp.Duration, temp.Style, temp.Direction);
                            break;

                        case 4 when obj is Saturation temp:
                            saturation = (float)EasingStyles.Process(saturation, temp.Intensity, (ms - temp.Ms) / temp.Duration, temp.Style, temp.Direction);
                            break;

                        case 5 when obj is Blur temp:
                            blur = (float)EasingStyles.Process(blur, temp.Intensity, (ms - temp.Ms) / temp.Duration, temp.Style, temp.Direction);
                            break;

                        case 6 when obj is FOV temp:
                            fov = (float)EasingStyles.Process(fov, temp.Intensity, (ms - temp.Ms) / temp.Duration, temp.Style, temp.Direction);
                            break;

                        case 7 when obj is Tint temp:
                            float tt = (currentTime - temp.Ms) / temp.Duration;

                            double r = EasingStyles.Process(tint.R / 255f, temp.Color.R / 255f, tt, temp.Style, temp.Direction);
                            double g = EasingStyles.Process(tint.G / 255f, temp.Color.G / 255f, tt, temp.Style, temp.Direction);
                            double b = EasingStyles.Process(tint.B / 255f, temp.Color.B / 255f, tt, temp.Style, temp.Direction);

                            tint = Color.FromArgb((int)(r * 255f), (int)(g * 255f), (int)(b * 255f));
                            break;

                        case 8 when obj is Position temp:
                            float tp = (currentTime - temp.Ms) / temp.Duration;

                            double x = EasingStyles.Process(pos.X, temp.Pos.X, tp, temp.Style, temp.Direction);
                            double y = EasingStyles.Process(pos.Y, temp.Pos.Y, tp, temp.Style, temp.Direction);
                            double z = EasingStyles.Process(pos.Z, temp.Pos.Z, tp, temp.Style, temp.Direction);

                            pos = ((float)x, (float)y, (float)z);
                            break;

                        case 9 when obj is Rotation temp:
                            rotation = (float)EasingStyles.Process(rotation, temp.Degrees, (currentTime - temp.Ms) / temp.Duration, temp.Style, temp.Direction);
                            break;

                        case 10 when obj is ARFactor temp:
                            arFactor = temp.Factor;
                            break;

                        case 11 when obj is Text temp:
                            if (currentTime - temp.Ms <= temp.Duration)
                                text = temp.String;
                            break;
                    }
                }

                approachRate *= arFactor;

                int colorCount = Settings.noteColors.Value.Count;

                (int low, int high) = CurrentMap.Notes.SearchRange(currentTime, maxMs);
                int range = high - low;

                Vector4[] noteOffsets = Pool.Rent(range);

                for (int i = low; i < high; i++)
                {
                    Note note = CurrentMap.Notes[i];
                    float progress = (note.Ms - currentTime) * approachRate / 1000f;

                    noteOffsets[range - (i - low) - 1] = (note.X - 1, note.Y - 1, progress * 24, i % colorCount);
                }

                fov = MathHelper.DegreesToRadians(70 * fov);

                if (fov > 0 && fov < Math.PI)
                {
                    Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(fov, editor.Size.X / (float)editor.Size.Y, 0.1f, 1000);
                    Matrix4 rot = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation)) * Matrix4.CreateRotationY(MathHelper.Pi);
                    Vector3 look = (rot * -Vector4.UnitZ).Xyz;
                    Vector3 camPos = -Vector3.UnitZ * 5.5f + look * (1.25f, 1.25f, 0);
                    Matrix4 view = Matrix4.CreateTranslation(-camPos) * rot;
                    Matrix4 trans = Matrix4.CreateTranslation(pos * (-1, 1, 1));

                    Shader.SetPVT(proj, view, trans);
                    Shader.SetBCSBT(brightness, contrast, saturation, blur, (tint.R / 255f, tint.G / 255f, tint.B / 255f));
                    
                    // fbo to allow post-processing (blur)
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, MFBO);
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    GL.Viewport(0, 0, editor.Size.X, editor.Size.Y);

                    // render grid
                    GL.UseProgram(Shader.VFXGridProgram);

                    GL.BindVertexArray(altVaO);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, altVertCount);

                    // render notes
                    GL.UseProgram(Shader.VFXNoteProgram);
                    RegisterData(5, noteOffsets, range);

                    Pool.Return(noteOffsets);

                    // blit multisampled fbo to textured fbo
                    GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, MFBO);
                    GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, FBO);
                    GL.BlitFramebuffer(0, 0, editor.Size.X, editor.Size.Y, 0, 0, editor.Size.X, editor.Size.Y, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle.Zero);

                    // render fbo through post-processor
                    Shader.SetBlur(blur);
                    GL.BindVertexArray(blurVaO);
                    GL.ActiveTexture(TextureUnit.Texture3);
                    GL.BindTexture(TextureTarget.Texture2d, fboTexture);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                }

                // render label
                if (editor.CurrentWindow is GuiWindowEditor gse)
                {
                    gse.SetVFXToast(text);

                    for (int i = 0; i < curObjs.Length; i++)
                    {
                        MapObject? obj = curObjs[i];

                        gse.IconSet[i].Visible = obj != null;
                    }
                }
            }

            RenderColorArray(c2Set, aSet, xSet, ySet, c2Len, out color2Texts, out alphas);
        }

        private int offset;

        public override void Render(float mousex, float mousey, float frametime)
        {
            Update();
            
            if (!RenderMapObjects || !VFXObjects)
            {
                // render background
                GL.UseProgram(Shader.Program);

                GL.BindVertexArray(VaO);
                offset = Indices["rectLength"] + Indices["loopLength"] + Indices["lineLength"];
                GL.DrawArrays(PrimitiveType.Triangles, 0, offset);

                // render keybinds
                GL.UseProgram(FontRenderer.unicode ? Shader.UnicodeProgram : Shader.FontProgram);
                FontRenderer.SetActive("main");

                GL.Uniform4f(TexColorLocation, 0.2f, 0.2f, 0.2f, 1f);
                if (!RenderMapObjects)
                    FontRenderer.RenderData("main", color1Texts.ToArray());

                // render dynamic elements
                GenerateOffsets();
            }
            else // everythings dynamic
                GenerateOffsets();

            // undo program switch
            GL.UseProgram(Shader.Program);
        }

        public override void RenderTexture()
        {
            GL.Uniform4f(TexColorLocation, 1f, 1f, 1f, 1f);
            FontRenderer.RenderData("main", color2Texts.ToArray(), alphas.ToArray());

            if (!RenderMapObjects)
            {
                // layer bezier preview on top
                GL.UseProgram(Shader.Program);

                GL.BindVertexArray(VaO);
                GL.DrawArrays(PrimitiveType.LineStrip, offset, Indices["bezierLength"]);

                GL.UseProgram(FontRenderer.unicode ? Shader.UnicodeProgram : Shader.FontProgram);
            }
        }

        private List<float> rects = new();
        private List<float> loops = new();
        private List<float> lines = new();
        private List<Vector4> color1Texts = new();
        private Vector4[] color2Texts;
        private float[] alphas;

        public override Tuple<float[], float[]> GetVertices()
        {
            MainWindow editor = MainWindow.Instance;

            if (prevRect != Rect)
            {
                prevRect = Rect;

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBO);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, MFBO);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, Math.Min(MainWindow.MaxSamples, 32), InternalFormat.Rgba8, editor.Size.X, editor.Size.Y);

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RenderbufferHandle.Zero);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle.Zero);

                GL.ActiveTexture(TextureUnit.Texture3);
                GL.BindTexture(TextureTarget.Texture2d, fboTexture);
                GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, editor.Size.X, editor.Size.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            }
            
            rects = new();
            loops = new();
            lines = new();
            color1Texts = new();

            float cellSize = Rect.Width / 3f;
            bool quantumLines = Settings.quantumGridLines.Value;

            rects.AddRange(GLU.Rect(Rect, 0.15f, 0.15f, 0.15f, Settings.gridOpacity.Value / 255f));
            loops.AddRange(GLU.OutlineAsTriangles(Rect, 1, 0.2f, 0.2f, 0.2f));

            float[] lineColor = quantumLines ? new float[] { 0.05f, 0.05f, 0.05f } : new float[] { 0.2f, 0.2f, 0.2f };

            for (int i = 1; i < 3; i++)
            {
                float x = Rect.X + Rect.Width / 3f * i;
                float y = Rect.Y + Rect.Height / 3f * i;

                lines.AddRange(GLU.Line(x, Rect.Y, x, Rect.Y + Rect.Height, 1, lineColor));
                lines.AddRange(GLU.Line(Rect.X, y, Rect.X + Rect.Width, y, 1, lineColor));
            }

            //render grid lines
            if (quantumLines)
            {
                float divisor = Settings.quantumSnapping.Value.Value + 3f;
                float offset = Math.Round(divisor) % 2 == 0 ? 0.5f : 1f;

                for (int i = (int)(2 * offset); i <= divisor; i++)
                {
                    float x = Rect.X + Rect.Width / divisor * (i - offset);
                    float y = Rect.Y + Rect.Height / divisor * (i - offset);

                    lines.AddRange(GLU.Line(x, Rect.Y, x, Rect.Y + Rect.Height, 1, 0.2f, 0.2f, 0.2f));
                    lines.AddRange(GLU.Line(Rect.X, y, Rect.X + Rect.Width, y, 1, 0.2f, 0.2f, 0.2f));
                }
            }

            //render grid letters
            if (Settings.gridLetters.Value)
            {
                Dictionary<Keys, Tuple<int, int>> copy = new(MainWindow.Instance.KeyMapping);

                foreach (KeyValuePair<Keys, Tuple<int, int>> key in copy)
                {
                    string letter = key.Key.ToString().Replace("KeyPad", "");
                    Tuple<int, int> tuple = key.Value;

                    float x = Rect.X + tuple.Item1 * cellSize + cellSize / 2f;
                    float y = Rect.Y + tuple.Item2 * cellSize + cellSize / 2f;

                    int width = FontRenderer.GetWidth(letter, 38, "main");
                    int height = FontRenderer.GetHeight(38, "main");

                    color1Texts.AddRange(FontRenderer.Print((int)(x - width / 2f), (int)(y - height / 2f), letter, 38, "main"));
                }
            }

            Indices["rectLength"] = rects.Count / 6;
            Indices["loopLength"] = loops.Count / 6;
            Indices["lineLength"] = lines.Count / 6;
            Indices["bezierLength"] = bezierVertices.Count / 6;

            rects.AddRange(loops);
            rects.AddRange(lines);
            rects.AddRange(bezierVertices);

            return new(rects.ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            MainWindow editor = MainWindow.Instance;

            // enables right click deletion of grid notes, not really sure if i want to keep it yet so its commented out for now
            /*
            if (right && HoveringNote != null)
            {
                OnMouseUp(pos);
                
                NoteManager.Remove("DELETE NOTE", HoveringNote);
                HoveringNote = null;
            }
            */

            if (right)
                return;

            bool separateClickTools = Settings.separateClickTools.Value;
            bool selectTool = Settings.selectTool.Value;

            Dragging = (Hovering || HoveringNote != null) && (HoveringNote != null || !separateClickTools || !selectTool);

            if (Dragging)
            {
                if (HoveringNote == null || (separateClickTools && !selectTool))
                {
                    PointF gridPos = PointToGridSpace(pos.X, pos.Y);
                    long ms = Timing.GetClosestBeat(Settings.currentTime.Value.Value);
                    Note note = new(gridPos.X, gridPos.Y, (long)(ms >= 0 ? ms : Settings.currentTime.Value.Value));
                    NoteManager.Add("ADD NOTE", note);

                    if (Settings.autoAdvance.Value)
                        Timing.Advance();

                    lastPlaced = gridPos;
                }
                else if (HoveringNote != null)
                {
                    DraggingNote = HoveringNote;
                    lastPos = new PointF(HoveringNote.X, HoveringNote.Y);
                    startPos = new Vector2(HoveringNote.X, HoveringNote.Y);

                    List<Note> selected = CurrentMap.Notes.Selected.ToList();

                    if (editor.ShiftHeld && selected.Count > 0)
                    {
                        selected = new List<Note> { selected[0] };

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
                        selected = new() { HoveringNote };

                    CurrentMap.Notes.Selected = new(selected);
                }
            }
        }

        public override void OnMouseMove(Point pos)
        {
            if (Dragging)
            {
                if (DraggingNote == null)
                {
                    PointF gridPos = PointToGridSpace(pos.X, pos.Y);

                    if (gridPos != lastPlaced)
                    {
                        long ms = Timing.GetClosestBeat(Settings.currentTime.Value.Value);
                        Note note = new(gridPos.X, gridPos.Y, (long)(ms >= 0 ? ms : Settings.currentTime.Value.Value));
                        NoteManager.Add("ADD NOTE", note);

                        if (Settings.autoAdvance.Value)
                            Timing.Advance();

                        lastPlaced = gridPos;
                    }
                }
                else
                {
                    PointF newPos = PointToGridSpace(pos.X, pos.Y);

                    if (newPos != lastPos)
                    {
                        Vector2 bounds = Settings.enableQuantum.Value ? (-0.85f, 2.85f) : (0, 2);

                        float xDiff = newPos.X - DraggingNote.X;
                        float yDiff = newPos.Y - DraggingNote.Y;

                        float maxX = DraggingNote.X;
                        float minX = DraggingNote.X;
                        float maxY = DraggingNote.Y;
                        float minY = DraggingNote.Y;

                        foreach (Note note in CurrentMap.Notes.Selected)
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

                        foreach (Note note in CurrentMap.Notes.Selected)
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
                MainWindow editor = MainWindow.Instance;
                List<Note> selected = CurrentMap.Notes.Selected.ToList();
                Vector2 posDiff = (DraggingNote.X, DraggingNote.Y) - startPos;

                foreach (Note note in selected)
                {
                    note.X -= posDiff.X;
                    note.Y -= posDiff.Y;
                }

                NoteManager.Edit("MOVE NOTE[S]", n =>
                {
                    n.X += posDiff.X;
                    n.Y += posDiff.Y;
                });
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

        public void AddPreviewNote(float x, float y, int c, bool mouse = false)
        {
            float cellSize = Rect.Width / 3f;
            float noteSize = cellSize * 0.65f;
            float cellGap = (cellSize - noteSize) / 2f;

            PointF pos = mouse ? PointToGridSpace(x, y) : new PointF(x, y);
            x = Rect.X + (2 - pos.X) * cellSize + cellGap;
            y = Rect.Y + (2 - pos.Y) * cellSize + cellGap;

            previewNoteOffsets.Add((x, y, 1, c));
            bezierVertices.AddRange(new float[6] { x + noteSize / 2f, y + noteSize / 2f, 1f, 1f, 1f, 1f });
        }

        public override void Dispose()
        {
            base.Dispose();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, altVbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, 0, IntPtr.Zero, BufferUsageARB.StaticDraw);

            GL.DeleteBuffer(altVbO);
            GL.DeleteVertexArray(altVaO);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, blurVbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, 0, IntPtr.Zero, BufferUsageARB.StaticDraw);

            GL.DeleteBuffer(blurVbO);
            GL.DeleteVertexArray(blurVaO);
        }

        public PointF PointToGridSpace(float mousex, float mousey)
        {
            bool quantum = Settings.enableQuantum.Value;

            Vector2 bounds = quantum ? (-0.85f, 2.85f) : (0, 2);

            float increment = quantum ? (Settings.quantumSnapping.Value.Value + 3f) / 3f : 1f;
            float x = (mousex - Rect.X - Rect.Width / 2f) / Rect.Width * 3f + 1 / increment;
            float y = (mousey - Rect.Y - Rect.Width / 2f) / Rect.Height * 3f + 1 / increment;

            if (Settings.quantumGridSnap.Value || !quantum)
            {
                x = (float)Math.Floor((x + 1 / increment / 2) * increment) / increment;
                y = (float)Math.Floor((y + 1 / increment / 2) * increment) / increment;
            }

            x = (float)MathHelper.Clamp(x - 1 / increment + 1, bounds.X, bounds.Y);
            y = (float)MathHelper.Clamp(y - 1 / increment + 1, bounds.X, bounds.Y);

            return new(2 - x, 2 - y);
        }
    }
}
