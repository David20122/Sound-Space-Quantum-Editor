using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SSQE_Player.Models;
using SSQE_Player.Types;
using System.Drawing;

namespace SSQE_Player.GUI
{
    internal class GuiWindowMain : GuiWindow
    {
        private readonly GuiLabel AccuracyLabel = new(930, 10, 60, 24, "", 24, true, Settings.settings["color1"]);
        private readonly GuiLabel ComboLabel = new(930, 35, 60, 24, "", 24, true, Settings.settings["color1"]);
        private readonly GuiLabel MissesLabel = new(930, 60, 60, 24, "", 24, true, Settings.settings["color2"]);
        private readonly GuiLabel InfoLabel = new(10, 10, 100, 50, "QUIT: Escape or R\nRESTART: Tab\nPAUSE: Space\nOFFSET: Scroll", 32, false, Settings.settings["color2"]);
        private readonly GuiLabel PausedLabel = new(930, 980, 60, 60, "PAUSED", 64, true, Color.FromArgb(0, 127, 255));
        private readonly GuiLabel HitWindowTempoLabel = new(10, 1050, 60, 40, "", 24, false, Settings.settings["color2"]);
        private readonly GuiLabel OffsetLabel = new(930, 1040, 60, 40, "", 24, true, Settings.settings["color2"]);
        private readonly GuiLabel FPSLabel = new(1800, 1050, 60, 40, "", 24, false, Settings.settings["color2"]);

        private Matrix4 noteScale = Matrix4.CreateScale(1);

        private readonly List<Cube> cubes = new();

        private Model CubeModel;
        private Model CursorModel;

        private float spawnZ => Settings.settings["approachDistance"] * 25f;
        private float NoteSpeed => (Settings.settings["playerApproachRate"].Value + 1f) * 2.5f / MainWindow.Instance.Tempo;
        private readonly float hitWindow; // ms
        private int noteIndex = 0;

        private int misses = 0;
        private int hits = 0;
        private int combo = 0;

        private float health = 100;
        private readonly float healthRegen = 100 / 12f;
        private readonly float healthPenalty = 20f;

        private Color healthColor = Color.FromArgb(0, 255, 0);

        private float waitTimer = 2000f;
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
        private int startIndex;

        public int Offset;

        public GuiWindowMain(int startIndex) : base(0, 0, MainWindow.Instance.Size.X, MainWindow.Instance.Size.Y)
        {
            this.startIndex = startIndex;

            Labels = new List<GuiLabel>
            {
                AccuracyLabel, ComboLabel, MissesLabel, InfoLabel, PausedLabel, HitWindowTempoLabel, OffsetLabel, FPSLabel
            };

            hitWindow = (int)Settings.settings["hitWindow"];
            float tempo = (float)Math.Round(Settings.settings["tempo"].Value + 0.1f, 4) * 100f;

            if (hitWindow != 55)
                HitWindowTempoLabel.Text = $"HW: {hitWindow}ms";
            if (tempo != 100f)
                HitWindowTempoLabel.Text += (hitWindow != 55 ? " | " : "") + $"Tempo: {tempo:#0.##}%";

            noteIndex = startIndex;

            Init();

            OnResize(MainWindow.Instance.Size);
        }

        private void Init()
        {
            var noteSize = MainWindow.NoteSize;
            CubeModel = MainWindow.Instance.ModelManager.GetModel("note");
            CursorModel = MainWindow.Instance.ModelManager.GetModel("cursor");

            var scale = new float[] { 1f, noteSize.X / CubeModel.Size.X, noteSize.Y / CubeModel.Size.Y, noteSize.Z / CubeModel.Size.Z }.Min();
            noteScale = Matrix4.CreateScale(scale);

            VaO = GL.GenVertexArray();
            VbO = GL.GenBuffer();

            GL.BindVertexArray(VaO);

            var vertices = Update();
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

            if (Paused && Unpausing && Settings.settings["currentTime"].Value >= PauseTime)
                Paused = false;

            PausedLabel.Visible = Paused && !Unpausing;

            GL.UseProgram(Shader.Program);
            GL.BindVertexArray(VaO);

            GL.Disable(EnableCap.CullFace);
            var vertices = Update();
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
            GL.Enable(EnableCap.CullFace);

            GL.UseProgram(Shader.ModelProgram);
            RenderCursor();
            AlignNotes(frametime);
            RenderNotes();

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
            
            base.Render(frametime);
        }

        public override void OnResize(Vector2i size)
        {
            Rect = new RectangleF(0, 0, size.X, size.Y);

            base.OnResize(size);
        }

        private void AlignNotes(float frametime)
        {
            var main = MainWindow.Instance;
            var currentTime = Settings.settings["currentTime"].Value - waitTimer - Offset;

            waitTimer = Math.Max(0f, waitTimer - frametime * 1000f);
            if (waitTimer <= 0 && !started)
            {
                started = true;
                main.MusicPlayer.Play();
            }

            var trackLength = spawnZ / NoteSpeed * 1000f;

            for (int i = noteIndex; i < main.Notes.Count; i++)
            {
                var note = main.Notes[i];

                if (note.Ms >= currentTime + trackLength)
                    break;

                var timeDiff = note.Ms - currentTime;
                var x = NoteSpeed * timeDiff / 1000f;

                cubes.Add(new Cube(x, note.X, note.Y, note.Ms, note.Color));
                noteIndex++;
            }

            var zHitbox = hitWindow * NoteSpeed / 1000f * MainWindow.Instance.Tempo;

            for (int i = cubes.Count - 1; i >= 0; i--)
            {
                var note = cubes[i];
                var timeDiff = note.Ms - currentTime;
                note.Z = NoteSpeed * timeDiff / 1000f;

                if (note.Z <= 0)
                {
                    Vector3 pos = (note.X - 1, note.Y - 1, note.Z);
                    var hovering = !Paused && IsOverNote(pos, zHitbox);
                    var passed = hovering || note.Z < -zHitbox;

                    if (hovering)
                        HitNote();
                    if (passed)
                    {
                        if (!hovering)
                            MissNote();
                        cubes.RemoveAt(i);
                    }
                }
            }
        }

        private void RenderNotes()
        {
            CubeModel.Bind();

            var sizeZ = (new Vector4(CubeModel.Size) * noteScale).Z;
            var fade = Settings.settings["approachFade"];

            for (int i = 0; i < cubes.Count; i++)
            {
                var note = cubes[i];

                var x = note.X - 1;
                var y = note.Y - 1;
                var z = note.Z - sizeZ / 2f;

                Vector4 color = (note.Color.R / 255f, note.Color.G / 255f, note.Color.B / 255f, 1);
                if (fade)
                    color.W = Math.Min(1, (spawnZ - z) / 10);

                Shader.SetTransform(noteScale * Matrix4.CreateTranslation(x, y, z));
                Shader.SetColor(color);
                CubeModel.Render();
            }
        }

        private void RenderCursor()
        {
            var pos = MainWindow.Instance.CursorPos - Vector3.UnitZ * 0.01f;
            var color2 = Settings.settings["color2"];
            Vector4 c = (color2.R / 255f, color2.G / 255f, color2.B / 255f, 1);

            var cursorSize = MainWindow.CursorSize;

            var scale = new float[] { 1f, cursorSize.X / CursorModel.Size.X, cursorSize.Y / CursorModel.Size.Y, (cursorSize.Z + 0.05f) / CursorModel.Size.Z }.Min();
            var s = Matrix4.CreateScale(scale);

            CursorModel.Bind();
            Shader.SetTransform(s * Matrix4.CreateTranslation(pos));
            Shader.SetColor(c);
            CursorModel.Render();

            if (!Settings.settings["lockCursor"] && Settings.settings["cameraMode"].Current != "spin")
            {
                var actualPos = MainWindow.Instance.Camera.LockedPos;
                pos = (actualPos.X, actualPos.Y, 0);
                s = Matrix4.CreateScale(scale * 0.95f);
                c = (0.5f, 0.5f, 0.5f, 0.5f);

                Shader.SetTransform(s * Matrix4.CreateTranslation(pos));
                Shader.SetColor(c);
                CursorModel.Render();
            }
        }

        private float[] Update()
        {
            List<float> vertices = new();

            var color1 = Settings.settings["color1"];
            var c1 = new float[] { color1.R / 255f, color1.G / 255f, color1.B / 255f, 1f };
            var ch = new float[] { healthColor.R / 255f, healthColor.G / 255f, healthColor.B / 255f, 1f };

            var ratio = health / 100f * 2.5f;

            var setting = Settings.settings["currentTime"];
            var progress = setting.Value / setting.Max;
            var final = progress * 2.8f;
            var first = MainWindow.Instance.StartTime / setting.Max;
            var firstFinal = first * 2.8f;

            // grid
            vertices.AddRange(GLU.Outline(-1.5f, -1.5f, 0, 3, 3, 0.02f, c1));
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

            return vertices.ToArray();
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

        private void HitNote()
        {
            health = MathHelper.Clamp(health + healthRegen, 0, 100);

            MainWindow.Instance.SoundPlayer.Play(Settings.settings["hitSound"]);

            hits++;
            combo++;
        }

        private void MissNote()
        {
            health = MathHelper.Clamp(health - healthPenalty, 0, 100);

            if (health <= 0)
                healthColor = Color.FromArgb(255, 100, 0);

            misses++;
            combo = 0;
        }

        private bool IsOverNote(Vector3 notePos, float zHitbox)
        {
            var cursorSize = MainWindow.CursorSize;
            var noteSize = MainWindow.NoteSize;

            var halfMax = (noteSize + cursorSize) / 2f;
            var halfMin = halfMax * new Vector3(1, 1, 0);

            var max = notePos + halfMax;
            var min = notePos - halfMin;

            var vec = MainWindow.Instance.CursorPos;

            return vec.X >= min.X && vec.Y >= min.Y && vec.Z >= min.Z &&
                   vec.X <= max.X && vec.Y <= max.Y && vec.Z <= notePos.Z + zHitbox;
        }

        public void Reset()
        {
            cubes.Clear();
            noteIndex = startIndex;

            hits = 0;
            misses = 0;
            combo = 0;

            health = 100;
            healthColor = Color.FromArgb(0, 255, 0);

            waitTimer = 2000f;
            started = false;

            Paused = false;
            Unpausing = false;
            PauseTime = float.MinValue;
            Pauses = 0;

            resetTimer = 0;

            MainWindow.Instance.MusicPlayer.Reset();
            Settings.settings["currentTime"].Value = MainWindow.Instance.StartTime;
        }
    }
}
