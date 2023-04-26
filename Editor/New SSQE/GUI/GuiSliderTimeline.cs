using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System;
using OpenTK.Mathematics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

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

        public GuiSliderTimeline(float posx, float posy, float sizex, float sizey, bool reverse, bool lockSize = false) : base(posx, posy, sizex, sizey, "currentTime", reverse, lockSize)
        {
            Font = "main";
            Dynamic = true;

            InstanceSetup();
        }

        public override void InstanceSetup()
        {
            ClearBuffers();

            VaOs = new int[2];
            VbOs = new int[4];
            VertexCounts = new int[2];

            var y = lineRect.Y + lineRect.Height / 2f;
            var by = lineRect.Y + lineRect.Height;

            var noteVerts = GLU.Line(0, y + 5f, 0, y + lineRect.Height, 1, 1f, 1f, 1f, 1f);
            var pointVerts = GLU.Line(0, y - 10f, 0, y - lineRect.Height * 2f, 2, 1f, 1f, 1f, 1f);

            AddToBuffers(noteVerts, 0);
            AddToBuffers(pointVerts, 1);
        }

        private Vector4[] noteOffsets = Array.Empty<Vector4>();
        private Vector4[] pointOffsets = Array.Empty<Vector4>();

        // stuff here doesnt need to be updated every frame
        public override void GenerateOffsets()
        {
            var editor = MainWindow.Instance;

            var setting = Settings.settings[Setting];

            noteOffsets = new Vector4[editor.Notes.Count];
            pointOffsets = new Vector4[editor.TimingPoints.Count];

            var color1 = Settings.settings["color1"];
            var c1 = new float[] { color1.R / 255f, color1.G / 255f, color1.B / 255f };

            // notes
            for (int i = 0; i < editor.Notes.Count; i++)
            {
                var note = editor.Notes[i];

                var progress = note.Ms / setting.Max;
                var x = lineRect.X + progress * lineRect.Width;

                noteOffsets[i] = (x, c1[0], c1[1], c1[2]);
            }

            // points
            for (int i = 0; i < editor.TimingPoints.Count; i++)
            {
                var point = editor.TimingPoints[i];

                var progress = point.Ms / setting.Max;
                var x = lineRect.X + progress * lineRect.Width - 1f;

                pointOffsets[i] = (x, c1[0], c1[1], c1[2]);
            }

            RegisterData(0, noteOffsets);
            RegisterData(1, pointOffsets);
        }

        private void RenderOffsets()
        {
            GL.BindVertexArray(VaOs[0]);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, VertexCounts[0], noteOffsets.Length);
            GL.BindVertexArray(VaOs[1]);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, VertexCounts[1], pointOffsets.Length);
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            lineRect = new RectangleF(Rect.X + Rect.Height / 2f, Rect.Y + Rect.Height / 2f - 1.5f, Rect.Width - Rect.Height, 3f);

            GL.UseProgram(Shader.InstancedProgram);

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
                GL.Uniform4(TexColorLocation, textColor.R / 255f, textColor.G / 255f, textColor.B / 255f, textColor.A / 255f);
                FontRenderer.RenderData("main", FontVertices);
            }
        }

        public override Tuple<float[], float[]> GetVertices()
        {
            var baseVerts = base.GetVertices();
            var editor = MainWindow.Instance;
            var mouse = editor.Mouse;

            var setting = Settings.settings[Setting];

            var color2 = Settings.settings["color2"];
            var c2 = new float[] { color2.R / 255f, color2.G / 255f, color2.B / 255f };
            var color3 = Settings.settings["color3"];
            var c3 = new float[] { color3.R / 255f, color3.G / 255f, color3.B / 255f };

            List<float> bookmarkVerts = new();

            // bookmarks
            var isHovering = false;
            int hoveringIndex = 0;

            for (int i = 0; i < editor.Bookmarks.Count; i++)
            {
                var bookmark = editor.Bookmarks[i];

                var progress = bookmark.Ms / setting.Max;
                var endProgress = bookmark.EndMs / setting.Max;
                var x = lineRect.X + progress * lineRect.Width;
                var endX = lineRect.X + endProgress * lineRect.Width;
                var y = lineRect.Y + lineRect.Height;

                var bRect = new RectangleF(x - 4f, y - 40f, 8f + (endX - x), 8f);
                var hovering = bRect.Contains(mouse.X, mouse.Y);

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
                var progress = HoveringBookmark.Ms / setting.Max;
                var x = lineRect.X + progress * lineRect.Width;
                var y = lineRect.Y + lineRect.Height;

                float height = FontRenderer.GetHeight(16, "main");

                FontVertices = FontRenderer.Print(x - 4f, y - 40f - height, HoveringBookmark.Text, 16, "main");
                textColor = Settings.settings["color2"];

                var index = hoveringIndex * 6 * 6 + 2;

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

            return new Tuple<float[], float[]>(baseVerts.Item1.Concat(bookmarkVerts).ToArray(), Array.Empty<float>());
        }

        public override void OnMouseClick(Point pos, bool right)
        {
            if (MainWindow.Instance.MusicPlayer.IsPlaying)
                MainWindow.Instance.MusicPlayer.Pause();

            if (HoveringBookmark == null)
                base.OnMouseClick(pos, right);
        }
    }
}
