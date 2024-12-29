using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Graphics;
using System.Buffers;
using OpenTK.Mathematics;
using New_SSQE.Objects;
using New_SSQE.GUI.Font;
using New_SSQE.Audio;
using New_SSQE.GUI.Shaders;
using New_SSQE.Objects.Other;
using New_SSQE.Preferences;
using New_SSQE.Maps;

namespace New_SSQE.GUI
{
    internal class GuiSliderTimeline : GuiSlider
    {
        public Bookmark? HoveringBookmark;
        private Bookmark? prevHover;

        private Color textColor;
        private int vertexCount = 0;
        private int offsetCount = 0;

        private RectangleF lineRect;
        private RectangleF prevRect;

        private readonly ArrayPool<Vector4> Pool = ArrayPool<Vector4>.Shared;
        public bool WasPlaying;

        public GuiSliderTimeline() : base(0, 0, 0, 0, Settings.currentTime, false)
        {
            Font = "main";
            Dynamic = true;

            InstanceSetup();
        }

        public override void InstanceSetup()
        {
            ClearBuffers();

            VaOs = new VertexArrayHandle[3];
            VbOs = new BufferHandle[6];
            VertexCounts = new int[3];

            float y = lineRect.Y + lineRect.Height / 2f;

            float[] noteVerts = GLU.Line(0, y + 5f, 0, y + 3f, 1, 1f, 1f, 1f, 1f);
            float[] pointVerts = GLU.Line(0, y - 10f, 0, y - 6f, 2, 1f, 1f, 1f, 1f);
            float[] objVerts = GLU.Line(0, y + 9f, 0, y + 7f, 2, 1f, 1f, 1f, 1f);

            AddToBuffers(noteVerts, 0);
            AddToBuffers(pointVerts, 1);
            AddToBuffers(objVerts, 2);
        }

        private int NoteLen, PointLen, ObjLen;

        // stuff here doesnt need to be updated every frame
        public override void GenerateOffsets()
        {
            SliderSetting setting = Slider.Value;
            int plen = CurrentMap.TimingPoints.Count;
            int mlen = CurrentMap.VfxObjects.Count + CurrentMap.SpecialObjects.Count;

            List<Vector4> noteOffsets = new();
            Vector4[] pointOffsets = Pool.Rent(plen);
            Vector4[] objOffsets = Pool.Rent(mlen);

            int lastRendered = -1;

            // notes
            for (int i = 0; i < CurrentMap.Notes.Count; i++)
            {
                Note note = CurrentMap.Notes[i];

                float progress = note.Ms / setting.Max;
                float x = lineRect.X + progress * lineRect.Width;

                if ((int)x > lastRendered)
                {
                    noteOffsets.Add((x, 0, 1, 0));
                    lastRendered = (int)x;
                }
            }

            // points
            for (int i = 0; i < CurrentMap.TimingPoints.Count; i++)
            {
                TimingPoint point = CurrentMap.TimingPoints[i];

                float progress = point.Ms / setting.Max;
                float x = lineRect.X + progress * lineRect.Width - 1f;

                pointOffsets[i] = (x, 0, 1, 0);
            }

            // vfx objects
            for (int i = 0; i < CurrentMap.VfxObjects.Count; i++)
            {
                MapObject obj = CurrentMap.VfxObjects[i];

                float progress = obj.Ms / setting.Max;
                float x = lineRect.X + progress * lineRect.Width - 1f;

                objOffsets[i] = (x, 0, 1, 0);
            }

            // special objects
            for (int i = 0; i < CurrentMap.SpecialObjects.Count; i++)
            {
                MapObject obj = CurrentMap.SpecialObjects[i];

                float progress = obj.Ms / setting.Max;
                float x = lineRect.X + progress * lineRect.Width - 1f;

                objOffsets[i + CurrentMap.VfxObjects.Count] = (x, 0, 1, 0);
            }

            RegisterData(0, noteOffsets.ToArray());
            RegisterData(1, pointOffsets, plen);
            RegisterData(2, objOffsets, mlen);

            NoteLen = noteOffsets.Count;
            PointLen = plen;
            ObjLen = mlen;

            Pool.Return(pointOffsets);
            Pool.Return(objOffsets);
        }

        private void RenderOffsets()
        {
            GL.BindVertexArray(VaOs[0]);
            if (NoteLen > 0)
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, VertexCounts[0], NoteLen);
            GL.BindVertexArray(VaOs[1]);
            if (PointLen > 0)
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, VertexCounts[1], PointLen);
            GL.BindVertexArray(VaOs[2]);
            if (ObjLen > 0)
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, VertexCounts[2], ObjLen);
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            lineRect = new RectangleF(Rect.X + Rect.Height / 2f, Rect.Y + Rect.Height / 2f - 1.5f, Rect.Width - Rect.Height, 3f);

            GL.UseProgram(Shader.TimelineProgram);

            if (prevRect != lineRect || prevHover != HoveringBookmark)
            {
                InstanceSetup();
                GenerateOffsets();

                prevRect = lineRect;
                prevHover = HoveringBookmark;
            }
            else
                RenderOffsets();

            GL.UseProgram(Shader.Program);
            base.Render(mousex, mousey, frametime);

            Update();
            GL.DrawArrays(PrimitiveType.Triangles, offsetCount, vertexCount);
        }

        public override void RenderTexture()
        {
            if (HoveringBookmark != null)
            {
                GL.Uniform4f(TexColorLocation, textColor.R / 255f, textColor.G / 255f, textColor.B / 255f, textColor.A / 255f);
                FontRenderer.RenderData("main", FontVertices);
            }
        }

        public override Tuple<float[], float[]> GetVertices()
        {
            Tuple<float[], float[]> baseVerts = base.GetVertices();
            MainWindow editor = MainWindow.Instance;
            Point mouse = editor.Mouse;

            SliderSetting setting = Slider.Value;

            Color color2 = Settings.color2.Value;
            float[] c2 = new float[] { color2.R / 255f, color2.G / 255f, color2.B / 255f };
            Color color3 = Settings.color3.Value;
            float[] c3 = new float[] { color3.R / 255f, color3.G / 255f, color3.B / 255f };

            List<float> bookmarkVerts = new();

            // bookmarks
            bool isHovering = false;
            int hoveringIndex = 0;

            for (int i = 0; i < CurrentMap.Bookmarks.Count; i++)
            {
                Bookmark bookmark = CurrentMap.Bookmarks[i];

                float progress = bookmark.Ms / setting.Max;
                float endProgress = bookmark.EndMs / setting.Max;
                float x = lineRect.X + progress * lineRect.Width;
                float endX = lineRect.X + endProgress * lineRect.Width;
                float y = lineRect.Y + lineRect.Height;

                RectangleF bRect = new(x - 4f, y - 40f, 8f + (endX - x), 8f);
                bool hovering = bRect.Contains(mouse.X, mouse.Y);

                bookmarkVerts.AddRange(GLU.Rect(bRect, c3[0], c3[1], c3[2], 0.75f));

                isHovering |= hovering;
                if (hovering)
                {
                    hoveringIndex = i;
                    HoveringBookmark = bookmark;
                }
            }

            if (!isHovering)
                HoveringBookmark = null;

            if (HoveringBookmark != null)
            {
                float progress = HoveringBookmark.Ms / setting.Max;
                float x = lineRect.X + progress * lineRect.Width;
                float y = lineRect.Y + lineRect.Height;

                float height = FontRenderer.GetHeight(16, "main");

                FontVertices = FontRenderer.Print(x - 4f, y - 40f - height, HoveringBookmark.Text, 16, "main");
                textColor = Settings.color2.Value;

                int index = hoveringIndex * 6 * 6 + 2;

                for (int i = 0; i < 6; i++)
                {
                    bookmarkVerts[index++] = c2[0];
                    bookmarkVerts[index++] = c2[1];
                    bookmarkVerts[index++] = c2[2];

                    index += 3;
                }
            }

            offsetCount = baseVerts.Item1.Length / 6;
            vertexCount = bookmarkVerts.Count / 6;

            return new(baseVerts.Item1.Concat(bookmarkVerts).ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right)
        {
            WasPlaying = MusicPlayer.IsPlaying && !Settings.pauseScroll.Value;
            if (MusicPlayer.IsPlaying)
                MusicPlayer.Pause();

            if (HoveringBookmark == null)
                base.OnMouseClick(pos, right);
        }
    }
}
