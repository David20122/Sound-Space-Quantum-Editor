using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SSQE_Player.Models;
using System.Drawing;

namespace SSQE_Player.GUI
{
    internal class GuiWindowMain : GuiWindow
    {
        private readonly GuiLabel AccuracyLabel = new(930, 10, 60, 24, "", 28, true, Settings.color1.Value);
        private readonly GuiLabel ComboLabel = new(930, 35, 60, 24, "", 28, true, Settings.color1.Value);
        private readonly GuiLabel MissesLabel = new(930, 60, 60, 24, "", 28, true, Settings.color2.Value);
        private readonly GuiLabel InfoLabel = new(10, 10, 100, 50, "QUIT: Escape or R\nRESTART: Tab\nPAUSE: Space\n\nOFFSET: Scroll\nUNFOCUS: CTRL+U\nFOCUS: LMB", 28, false, Settings.color2.Value);
        private readonly GuiLabel PausedLabel = new(930, 980, 60, 60, "PAUSED", 72, true, Color.FromArgb(0, 127, 255));
        private readonly GuiLabel HitWindowTempoLabel = new(10, 1050, 60, 40, "", 28, false, Settings.color2.Value);
        private readonly GuiLabel OffsetLabel = new(930, 1040, 60, 40, "", 28, true, Settings.color2.Value);
        private readonly GuiLabel FPSLabel = new(1800, 1050, 60, 40, "", 28, false, Settings.color2.Value);

        private Matrix4 noteScale = Matrix4.CreateScale(1);

        private Model CubeModel;
        private Model CursorModel;

        private static float SpawnZ => Settings.approachDistance.Value * 25f;
        private static float NoteSpeed => (Settings.playerApproachRate.Value.Value + 1f) * 2.5f / MainWindow.Instance.Tempo;
        private readonly float hitWindow; // ms

        private int misses = 0;
        private int hits = 0;
        private int combo = 0;

        private float health = 100;
        private readonly float healthRegen = 100 / 12f;
        private readonly float healthPenalty = 20f;

        private Color healthColor = Color.FromArgb(0, 255, 0);

        public float waitTimer = 2000f;
        private bool started = false;

        public bool Resetting = false;
        private float resetTimer = 0;

        public bool Paused = false;
        public bool Unpausing = false;
        public int Pauses;
        public float PauseTime = float.MinValue;

        private VertexArrayHandle VaO;
        private BufferHandle VbO;
        private int vertexCount;

        private int frames;
        private float time;
        private readonly int startIndex;

        public int Offset;

        private int prevPlayed = -1;
        private int lastHit = -1;

        private bool[] noteSet;

        public GuiWindowMain(int startIndex) : base(0, 0, MainWindow.Instance.Size.X, MainWindow.Instance.Size.Y)
        {
            noteSet = new bool[MainWindow.Instance.Notes.Length + 1];
            for (int i = startIndex; i < noteSet.Length; i++)
                noteSet[i] = true;

            this.startIndex = startIndex;

            Labels =
            [
                AccuracyLabel, ComboLabel, MissesLabel, InfoLabel, PausedLabel, HitWindowTempoLabel, OffsetLabel, FPSLabel
            ];

            hitWindow = (int)Settings.hitWindow.Value;
            waitTimer *= MainWindow.Instance.Tempo;
            float tempo = (float)Math.Round(MainWindow.Instance.Tempo, 4) * 100f;

            if (hitWindow != 55)
                HitWindowTempoLabel.Text = $"HW: {hitWindow}ms";
            if (tempo != 100f)
                HitWindowTempoLabel.Text += (hitWindow != 55 ? " | " : "") + $"Tempo: {tempo:#0.##}%";

            Init();

            OnResize(MainWindow.Instance.Size);
        }

        private void Init()
        {
            Vector3 noteSize = MainWindow.NoteSize;
            CubeModel = MainWindow.Instance.ModelManager.GetModel("note");
            CursorModel = MainWindow.Instance.ModelManager.GetModel("cursor");

            float scale = new float[] { 1f, noteSize.X / CubeModel.Size.X, noteSize.Y / CubeModel.Size.Y, noteSize.Z / CubeModel.Size.Z }.Min();
            noteScale = Matrix4.CreateScale(scale);

            VaO = GL.GenVertexArray();
            VbO = GL.GenBuffer();

            GL.BindVertexArray(VaO);

            float[] vertices = Update();
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
            GL.BindVertexArray(VertexArrayHandle.Zero);
        }

        public override void Render(float frametime)
        {
            if (Resetting)
                resetTimer += frametime;
            else
                resetTimer = 0;

            if (resetTimer >= 0.75)
                MainWindow.Instance.Close();

            if (Paused && Unpausing && Settings.currentTime.Value.Value >= PauseTime)
                Paused = false;

            PausedLabel.Visible = Paused && !Unpausing;

            GL.UseProgram(Shader.Program);
            GL.BindVertexArray(VaO);

            GL.Disable(EnableCap.CullFace);
            float[] vertices = Update();
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
            GL.Enable(EnableCap.CullFace);

            GL.UseProgram(Shader.ModelProgram);
            RenderCursor();
            RenderNotes(frametime);

            AccuracyLabel.Text = CalculateAccuracy();
            ComboLabel.Text = combo.ToString();
            MissesLabel.Text = $"{misses} | {Pauses}";
            OffsetLabel.Text = Offset != 0 ? $"Offset: {Offset:#0}ms" : "";

            frames++;
            time += frametime;

            if (time >= 0.25f)
            {
                FPSLabel.Text = CalculateFPS();
                frames = 0;
                time = 0;
            }

            if (lastHit != prevPlayed)
            {
                MainWindow.Instance.SoundPlayer.Play(Settings.hitSound.Value);
                prevPlayed = lastHit;
            }

            base.Render(frametime);
        }

        public override void OnResize(Vector2i size)
        {
            Rect = new(0, 0, size.X, size.Y);

            base.OnResize(size);
        }

        private void RenderNotes(float frametime)
        {
            MainWindow main = MainWindow.Instance;
            float currentTime = Settings.currentTime.Value.Value - waitTimer - Offset;

            waitTimer = Math.Max(0f, waitTimer - frametime * 1000f * main.Tempo);
            if (waitTimer <= 0 && !started)
            {
                started = true;
                main.MusicPlayer.Play();
            }

            float trackLength = SpawnZ / NoteSpeed * 1000f;
            float zHitbox = hitWindow * NoteSpeed / 1000f * MainWindow.Instance.Tempo;

            CubeModel.Bind();
            float minMs = currentTime - 2 * zHitbox / NoteSpeed * 1000f - frametime;
            float maxMs = currentTime + trackLength;

            int low = main.BinarySearchFirst((long)minMs);
            int high = main.BinarySearchLast((long)maxMs);
            if (main.Notes.Length > 0 && main.Notes[high].Ms <= (long)maxMs && main.Notes[low].Ms >= (long)minMs)
                high++;
            int range = high - low;

            bool fade = Settings.approachFade.Value;
            Vector3[] positions = new Vector3[range];
            Vector4[] colors = new Vector4[range];
            Shader.SetTransform(noteScale);

            for (int i = low; i < high; i++)
            {
                int index = i - low;
                Note note = main.Notes[i];

                float x = note.X - 1;
                float y = note.Y - 1;
                float timeDiff = note.Ms - currentTime;
                float prevZ = NoteSpeed * (timeDiff - frametime) / 1000f;
                float z = NoteSpeed * timeDiff / 1000f;

                if (noteSet[i] && z <= 0)
                {
                    bool hovering = !Paused && IsOverNote((x, y, z), zHitbox, prevZ);
                    bool passed = hovering || z < -zHitbox;

                    if (hovering)
                        HitNote(i);
                    else if (passed)
                        MissNote(i);
                }

                positions[index] = (x, y, (noteSet[i] && (Settings.notePushback.Value || z >= 0)) ? z : -10);
                colors[index] = new(note.Color, fade ? Math.Min(1f, (1f - z / SpawnZ) * 2f) : 1f);
            }

            main.ModelManager.Render(positions, colors, range, "note");
            while (!noteSet[MainWindow.AutoIndex])
                MainWindow.AutoIndex++;
        }

        private void RenderCursor()
        {
            Vector3 pos = MainWindow.Instance.CursorPos - Vector3.UnitZ * 0.01f;
            Color color2 = Settings.color2.Value;

            Vector3 cursorSize = MainWindow.CursorSize;

            float scale = new float[] { 1f, cursorSize.X / CursorModel.Size.X, cursorSize.Y / CursorModel.Size.Y, (cursorSize.Z + 0.05f) / CursorModel.Size.Z }.Min();
            Matrix4 s = Matrix4.CreateScale(scale);

            CursorModel.Bind();
            Shader.SetTransform(s);
            MainWindow.Instance.ModelManager.Render([(pos.X, pos.Y, pos.Z)], [(color2.R / 255f, color2.G / 255f, color2.B / 255f, 1f)], 1, "cursor");
            
            if (!Settings.lockCursor.Value && Settings.cameraMode.Value.Current != "spin")
            {
                Vector2 actualPos = MainWindow.Instance.Camera.LockedPos;
                pos = (actualPos.X, actualPos.Y, 0);
                s = Matrix4.CreateScale(scale * 0.95f);

                Shader.SetTransform(s);
                MainWindow.Instance.ModelManager.Render([(pos.X, pos.Y, pos.Z)], [(0.5f, 0.5f, 0.5f, 0.5f)], 1, "cursor");
            }
        }

        private float[] Update()
        {
            List<float> vertices = [];

            Color color1 = Settings.color1.Value;
            float[] c1 = [color1.R / 255f, color1.G / 255f, color1.B / 255f, 1f];
            float[] ch = [healthColor.R / 255f, healthColor.G / 255f, healthColor.B / 255f, 1f];

            float ratio = health / 100f * 2.5f;

            SliderSetting setting = Settings.currentTime.Value;
            float progress = setting.Value / setting.Max;
            float final = progress * 2.8f;
            float first = MainWindow.Instance.StartTime / setting.Max;
            float firstFinal = first * 2.8f;

            // grid
            vertices.AddRange(GLU.Outline(-1.5f, -1.5f, 0, 3, 3, 0.02f, c1));

            // grid guides
            if (Settings.gridGuides.Value)
            {
                vertices.AddRange(GLU.Line(-0.5f, -1.5f, 0, -0.5f, 1.5f, 0, 0.01f, 0.5f, 0.5f, 0.5f, 0.5f));
                vertices.AddRange(GLU.Line(0.5f, -1.5f, 0, 0.5f, 1.5f, 0, 0.01f, 0.5f, 0.5f, 0.5f, 0.5f));
                vertices.AddRange(GLU.Line(-1.5f, -0.5f, 0, 1.5f, -0.5f, 0, 0.01f, 0.5f, 0.5f, 0.5f, 0.5f));
                vertices.AddRange(GLU.Line(-1.5f, 0.5f, 0, 1.5f, 0.5f, 0, 0.01f, 0.5f, 0.5f, 0.5f, 0.5f));
            }

            // lines
            vertices.AddRange(GLU.FadingLine(-2.5f, -1.5f, -10, -2.5f, -1.5f, 75, 0.06f, c1));
            vertices.AddRange(GLU.FadingLine(2.5f, -1.5f, -10, 2.5f, -1.5f, 75, 0.06f, c1));
            // health
            vertices.AddRange(GLU.Line(1.25f, -1.7f, 0, 1.25f - ratio, -1.7f, 0, 0.03f, ch));
            vertices.AddRange(GLU.Line(1.25f - ratio, -1.7f, 0, -1.25f, -1.7f, 0, 0.03f, 1f, 0f, 0f, 1f));
            // progress
            vertices.AddRange(GLU.Line(1.4f, 1.75f, -0.1f, 1.4f - final, 1.75f, -0.1f, 0.03f, 1f, 1f, 1f, 1f));
            vertices.AddRange(GLU.Line(1.4f - final, 1.8f, -0.101f, 1.4f - final, 1.7f, -0.101f, 0.03f, 1f, 1f, 1f, 1f));
            vertices.AddRange(GLU.Line(1.4f - firstFinal, 1.8f, -0.099f, 1.4f - firstFinal, 1.7f, -0.099f, 0.03f, 1f, 0.4f, 0f, 1f));
            vertices.AddRange(GLU.Line(1.4f - final, 1.75f, -0.1f, -1.4f, 1.75f, -0.1f, 0.03f, 0.5f, 0.5f, 0.5f, 1f));
            // quit
            vertices.AddRange(GLU.Line(2, -2, 0, 2 - resetTimer / 0.75f * 4, -2, 0, 0.06f, 1f, 0f, 0f, 1f));
            // unpause
            if (Paused && Unpausing)
                vertices.AddRange(GLU.Line(2, -2, 0.001f, 2 - (750f - PauseTime + setting.Value) / 750f * 4, -2, 0.001f, 0.06f, 0f, 0.5f, 1f, 1f));

            vertexCount = vertices.Count / 7;

            return [.. vertices];
        }

        private string CalculateAccuracy()
        {
            if (hits + misses == 0)
                return "--.--%";
            else
                return $"{(float)hits / (hits + misses) * 100:##0.00}%";
        }

        private string CalculateFPS()
        {
            return $"FPS: {frames / time:##0}";
        }

        private void HitNote(int index)
        {
            health = MathHelper.Clamp(health + healthRegen, 0, 100);

            lastHit = index;

            hits++;
            combo++;

            noteSet[index] = false;
        }

        private void MissNote(int index)
        {
            health = MathHelper.Clamp(health - healthPenalty, 0, 100);

            if (health <= 0)
                healthColor = Color.FromArgb(255, 100, 0);

            misses++;
            combo = 0;

            noteSet[index] = false;
        }

        private static bool IsOverNote(Vector3 notePos, float zHitbox, float prevZ)
        {
            if (prevZ + zHitbox < 0 || notePos.Z > 0)
                return false;

            Vector3 cursorSize = MainWindow.CursorSize;
            Vector3 noteSize = MainWindow.NoteSize;

            Vector3 halfMax = (noteSize + cursorSize) / 2f;
            Vector3 halfMin = halfMax * (1, 1, 0);

            Vector3 max = notePos + halfMax;
            Vector3 min = notePos - halfMin;

            Vector3 prevVec = MainWindow.Instance.LastCursorPos;
            Vector3 vec = MainWindow.Instance.CursorPos;

            float start = prevZ / (notePos.Z - prevZ);
            float end = zHitbox / (notePos.Z - prevZ);

            start = Math.Clamp(start, 0, 1);
            end = Math.Clamp(end, 0, 1);

            Vector3 v1 = prevVec + (vec - prevVec) * start;
            Vector3 v2 = prevVec + (vec - prevVec) * end;

            Vector2 m1 = (Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y));
            Vector2 m2 = (Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y));

            if (m2.X < min.X || max.X < m1.X)
                return false;
            if (m2.Y < min.Y || max.Y < m1.Y)
                return false;

            if ((v2 - v1).Length <= 0)
                return true;

            Vector2 s = (-(v2.Y - v1.Y), v2.X - v1.X);

            Vector2 proj1 = Vector2.Dot((v1.X, v1.Y), s) * s;
            float dist1 = proj1.Length * (Vector2.Dot(proj1, s) < 0 ? -1 : 1);
            Vector2 proj2 = Vector2.Dot((v2.X, v2.Y), s) * s;
            float dist2 = proj2.Length * (Vector2.Dot(proj2, s) < 0 ? -1 : 1);

            Vector2[] projBox =
            [
                Vector2.Dot((min.X, min.Y), s) * s,
                Vector2.Dot((min.X, max.Y), s) * s,
                Vector2.Dot((max.X, min.Y), s) * s,
                Vector2.Dot((max.X, max.Y), s) * s,
            ];

            float[] distBox =
            [
                projBox[0].Length * (Vector2.Dot(projBox[0], s) < 0 ? -1 : 1),
                projBox[1].Length * (Vector2.Dot(projBox[1], s) < 0 ? -1 : 1),
                projBox[2].Length * (Vector2.Dot(projBox[2], s) < 0 ? -1 : 1),
                projBox[3].Length * (Vector2.Dot(projBox[3], s) < 0 ? -1 : 1),
            ];

            return Math.Max(dist1, dist2) >= distBox.Min() && distBox.Max() >= Math.Min(dist1, dist2);
        }

        public void Reset()
        {
            MainWindow main = MainWindow.Instance;

            noteSet = new bool[main.Notes.Length + 1];
            for (int i = startIndex; i < noteSet.Length; i++)
                noteSet[i] = true;

            hits = 0;
            misses = 0;
            combo = 0;
            MainWindow.AutoIndex = 0;

            health = 100;
            healthColor = Color.FromArgb(0, 255, 0);

            waitTimer = 2000f * main.Tempo;
            started = false;

            Paused = false;
            Unpausing = false;
            PauseTime = float.MinValue;
            Pauses = 0;

            resetTimer = 0;
            main.ReplayCursorIndex = 0;
            main.ReplaySkipIndex = 0;

            main.MusicPlayer.Reset();
            Settings.currentTime.Value.Value = main.StartTime;
        }
    }
}
