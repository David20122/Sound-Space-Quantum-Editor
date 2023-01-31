using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SSQE_Player.Misc;

namespace SSQE_Player.GUI
{
    class GuiWindowMain : Gui
    {
        public Matrix4 noteScale = Matrix4.CreateScale(1);

        private readonly List<Cube> cubes = new List<Cube>();

        private static Model CubeModel;
        private static Shader CubeShader;

        private static Model CursorModel;
        private static Shader CursorShader;

        private float spawnZ => 37f * Settings.settings["approachDistance"];

        private int noteIndex = 0;
        private float noteSpeed => (Settings.settings["playerApproachRate"].Value + 1f) * 2.5f;
        private const float hitWindow = 55f; // ms

        public int misses = 0;
        private int hits = 0;
        public int combo = 0;

        private float health = 100;
        private float healthRegen = 100 / 12f;
        private float healthPenalty = 20f;
        private Color healthColor = Color.FromArgb(0, 255, 0);
        private Color healthColorR = Color.FromArgb(255, 0, 0);

        private float waitTimer = 2000f;
        private bool started = false;

        public bool Resetting = false;
        private float resetTimer = 0;

        public GuiWindowMain(int startIndex) : base(0, 0, MainWindow.Instance.ClientSize.Width, MainWindow.Instance.ClientSize.Height)
        {
            noteIndex = startIndex;

            CubeShader = new Shader("note", "colorIn");
            CursorShader = new Shader("cursor", "colorIn");

            OnResize(MainWindow.Instance.ClientSize);
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (Resetting)
                resetTimer += frametime;
            else
                resetTimer = 0;

            if (resetTimer >= 0.75)
                MainWindow.Instance.Close();



            var color1 = Settings.settings["color1"];

            // grid

            GL.LineWidth(2f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Color3(color1);
            GL.Vertex3(-1.5, -1.5, 0);
            GL.Vertex3(-1.5, 1.5, 0);
            GL.Vertex3(1.5, 1.5, 0);
            GL.Vertex3(1.5, -1.5, 0);
            GL.End();

            // lines

            GL.LineWidth(6f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-2.5, -1.5, -10);
            GL.Color4(0f, 0f, 0f, 0f);
            GL.Vertex3(-2.5, -1.5, 75);
            GL.Vertex3(2.5, -1.5, 75);
            GL.Color3(color1);
            GL.Vertex3(2.5, -1.5, -10);
            GL.End();

            // health

            var ratio = health / 100f * 2.5f;

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(healthColor);
            GL.Vertex3(1.25, -1.7, 0);
            GL.Vertex3(1.25 - ratio, -1.7, 0);
            GL.Color3(healthColorR);
            GL.Vertex3(1.25 - ratio, -1.7, 0);
            GL.Vertex3(-1.25, -1.7, 0);
            GL.End();

            // progress

            var setting = Settings.settings["currentTime"];
            var progress = setting.Value / setting.Max;
            var final = progress * 2.8;
            var first = MainWindow.Instance.startTime / setting.Max;
            var firstFinal = first * 2.8;

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(1f, 1f, 1f);
            GL.Vertex3(1.4, 1.75, -0.1);
            GL.Vertex3(1.4 - final, 1.75, -0.1);
            GL.Vertex3(1.4 - final, 1.8, -0.101);
            GL.Vertex3(1.4 - final, 1.7, -0.101);
            GL.Color3(1f, 0.4f, 0f);
            GL.Vertex3(1.4 - firstFinal, 1.8, -0.099);
            GL.Vertex3(1.4 - firstFinal, 1.7, -0.099);
            GL.Color3(0.5f, 0.5f, 0.5f);
            GL.Vertex3(1.4 - final, 1.75, -0.1);
            GL.Vertex3(-1.4, 1.75, -0.1);
            GL.End();
            
            // quit

            if (Resetting)
            {
                GL.LineWidth(12f);
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(1f, 0f, 0f);
                GL.Vertex3(2, -2, 0);
                GL.Vertex3(2 - resetTimer / 0.75 * 4, -2, 0);
                GL.End();
            }



            RenderCursor();

            AlignNotes(frametime);
            RenderNotes();

            base.Render(mousex, mousey, frametime);
        }

        public override void OnResize(Size size)
        {
            rect = new RectangleF(0, 0, size.Width, size.Height);

            base.OnResize(size);
        }

        private void RenderCursor()
        {
            var pos = MainWindow.Instance.cursorPos - Vector3.UnitZ * 0.01f;
            var color2 = Settings.settings["color2"];
            var c = new Vector4(color2.R, color2.G, color2.B, 1);

            var cursorSize = MainWindow.Instance.cursorSize;

            var scale = new float[] { 1f, cursorSize.X / CursorModel.Size.X, cursorSize.Y / CursorModel.Size.Y, (cursorSize.Z + 0.05f) / CursorModel.Size.Z }.Min();
            var s = Matrix4.CreateScale(scale);

            CursorShader.Bind();
            CursorModel.Bind();

            CursorShader.SetMatrix4("transformationMatrix", s * Matrix4.CreateTranslation(pos));
            CursorShader.SetVector4("colorIn", c);

            CursorModel.Render();

            if (!Settings.settings["lockCursor"] && Settings.settings["cameraMode"].Current != "spin")
            {
                var actualPos = MainWindow.Instance.Camera.lockedPos;
                pos = new Vector3(actualPos.X, actualPos.Y, 0);
                s = Matrix4.CreateScale(scale * 0.95f);
                c = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);

                CursorShader.SetMatrix4("transformationMatrix", s * Matrix4.CreateTranslation(pos));
                CursorShader.SetVector4("colorIn", c);

                CursorModel.Render();
            }

            CursorModel.Unbind();
            CursorShader.Unbind();
        }

        private void RenderNotes()
        {
            CubeShader.Bind();
            CubeModel.Bind();

            var sizeZ = (new Vector4(CubeModel.Size) * noteScale).Z;

            for (var i = 0; i < cubes.Count; i++)
            {
                var note = cubes[i];

                var x = note.XIndex - 1;
                var y = note.YIndex - 1;
                var z = note.Z - sizeZ / 2f;

                var color = new Vector4(note.Color.R, note.Color.G, note.Color.B, Math.Min(1, (spawnZ - z) / 10));

                CubeShader.SetMatrix4("transformationMatrix", noteScale * Matrix4.CreateTranslation(x, y, z));
                CubeShader.SetVector4("colorIn", color);

                CubeModel.Render();
            }

            CubeModel.Unbind();
            CubeShader.Unbind();
        }

        private void AlignNotes(float delta)
        {
            var main = MainWindow.Instance;
            var currentTime = Settings.settings["currentTime"].Value - waitTimer;

            waitTimer = Math.Max(0f, waitTimer - delta * 1000f);
            if (waitTimer <= 0 && !started)
            {
                started = true;
                main.MusicPlayer.Play();
            }

            var speed = noteSpeed;
            var trackLength = spawnZ / speed * 1000f;

            for (var i = noteIndex; i < main.Notes.Count; i++)
            {
                var note = main.Notes[i];
                
                if (note.Ms >= currentTime + trackLength)
                    break;

                var timeDiff = note.Ms - currentTime;
                var x = speed * timeDiff / 1000f;

                cubes.Add(new Cube(x, note.X, note.Y, note.Ms, note.Color));
                noteIndex++;
            }
            
            var zHitbox = hitWindow * speed / 1000f;

            for (var i = cubes.Count - 1; i >= 0; i--)
            {
                var note = cubes[i];
                var timeDiff = note.Ms - currentTime;
                note.Z = speed * timeDiff / 1000f;

                if (note.Z <= 0)
                {
                    var pos = new Vector3(note.XIndex - 1, note.YIndex - 1, note.Z);
                    var hovering = IsOverNote(pos, zHitbox);
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

        public string CalculateAccuracy()
        {
            if (hits + misses == 0)
                return "--.--%";
            else
                return $"{(float)hits / (hits + misses) * 100:##0.00}%";
        }

        private void HitNote()
        {
            health += healthRegen;
            health = MathHelper.Clamp(health, 0, 100);

            MainWindow.Instance.SoundPlayer.Play(Settings.settings["hitSound"]);

            hits++;
            combo++;
        }

        private void MissNote()
        {
            health -= healthPenalty;
            health = MathHelper.Clamp(health, 0, 100);

            if (health <= 0)
                healthColor = Color.FromArgb(255, 100, 0);

            misses++;
            combo = 0;
        }

        private bool IsOverNote(Vector3 notePos, float zHitbox)
        {
            var cursorSize = MainWindow.Instance.cursorSize;
            var noteSize = MainWindow.Instance.noteSize;

            var halfMax = (noteSize + cursorSize) / 2f;
            var halfMin = halfMax * new Vector3(1, 1, 0);

            var max = notePos + halfMax;
            var min = notePos - halfMin;

            var vec = MainWindow.Instance.cursorPos;

            return vec.X >= min.X && vec.Y >= min.Y && vec.Z >= min.Z &&
                   vec.X <= max.X && vec.Y <= max.Y && vec.Z <= notePos.Z + zHitbox;
        }

        public void SetCubeStyle(string name)
        {
            var noteSize = MainWindow.Instance.noteSize;

            CubeModel = MainWindow.Instance.ModelManager.GetModel(name);

            var scale = new float[] { 1f, noteSize.X / CubeModel.Size.X, noteSize.Y / CubeModel.Size.Y, noteSize.Z / CubeModel.Size.Z }.Min();

            noteScale = Matrix4.CreateScale(scale);
        }

        public void SetCursorStyle(string name)
        {
            CursorModel = MainWindow.Instance.ModelManager.GetModel(name);
        }

        public void Reset()
        {
            cubes.Clear();
            noteIndex = 0;

            hits = 0;
            misses = 0;
            combo = 0;

            health = 100;
            healthColor = Color.FromArgb(0, 255, 0);

            waitTimer = 2000f;
            started = false;

            MainWindow.Instance.MusicPlayer.Reset();
            Settings.settings["currentTime"].Value = MainWindow.Instance.startTime;
        }
    }
}
