using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using System.Windows.Forms;
using SSQE_Player.Properties;
using SSQE_Player.GUI;
using OpenTK.Input;
using System.Drawing;
using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using SSQE_Player.Misc;
using System.Globalization;
using System.IO;

namespace SSQE_Player
{
    class MainWindow : GameWindow
    {
        public static MainWindow Instance;

        public GuiWindowMain CurrentWindow;
        public Camera Camera;

        public MusicPlayer MusicPlayer = new MusicPlayer() { Volume = 0.2f };
        public SoundPlayer SoundPlayer = new SoundPlayer() { Volume = 0.2f };
        public ModelManager ModelManager = new ModelManager();

        public List<Note> Notes = new List<Note>();

        public readonly FontRenderer Font;

        public readonly Vector3 noteSize = new Vector3(0.875f, 0.875f, 0.875f);
        public readonly Vector3 cursorSize = new Vector3(0.275f, 0.275f, 0f);
        public Vector3 cursorPos = new Vector3();

        public float startTime;

        public MainWindow(bool fromStart) : base(1280, 720, new GraphicsMode(32, 8, 0, 8), $"Sound Space Map Player {Application.ProductVersion}")
        {
            VSync = VSyncMode.Off;
            Icon = Resources.icon;
            CursorVisible = false;
            CursorGrabbed = true;
            WindowState = WindowState.Fullscreen;

            Instance = this;

            Font = new FontRenderer("main");

            Settings.Load();

            if (fromStart)
                Settings.settings["currentTime"].Value = 0f;
            startTime = Settings.settings["currentTime"].Value;

            var startIndex = LoadMap(File.ReadAllText("assets/temp/tempmap.txt"));
            RegisterModels();

            Camera = new Camera();
            CurrentWindow = new GuiWindowMain(startIndex);
            CurrentWindow.SetCubeStyle("note");
            CurrentWindow.SetCursorStyle("cursor");

            MusicPlayer.Volume = Settings.settings["masterVolume"].Value;
            SoundPlayer.Volume = Settings.settings["sfxVolume"].Value;
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            var mouse = Mouse.GetState();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Projection);
            
            if (MusicPlayer.IsPlaying)
                Settings.settings["currentTime"].Value = (float)MusicPlayer.CurrentTime.TotalMilliseconds;

            Camera.UpdateCamera(new Point(mouse.X, mouse.Y));
            CurrentWindow?.Render(mouse.X, mouse.Y, (float)e.Time);

            Camera.SetOrtho();

            // accuracy
            GL.Color3(Settings.settings["color1"]);

            var acc = CurrentWindow.CalculateAccuracy();
            var accWidth = Font.GetWidth(acc, 24);
            var accHeight = Font.GetHeight(24);
            Font.Render(acc, ClientSize.Width / 2f - accWidth / 2f, 10, 24);

            // combo
            var combo = CurrentWindow.combo.ToString();
            var comboWidth = Font.GetWidth(combo, 24);
            var comboHeight = Font.GetHeight(24);
            Font.Render(combo, ClientSize.Width / 2f - comboWidth / 2f, 15 + accHeight, 24);

            // misses
            GL.Color3(Settings.settings["color2"]);

            var misses = CurrentWindow.misses.ToString();
            var missesWidth = Font.GetWidth(misses, 20);
            Font.Render(misses, ClientSize.Width / 2f - missesWidth / 2f, 20 + accHeight + comboHeight, 20);

            Font.Render("QUIT: Escape or R", 10, 10, 32);

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(ClientRectangle);

            Camera.CalculateProjection();
            CurrentWindow?.OnResize(ClientRectangle.Size);

            OnRenderFrame(new FrameEventArgs());
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                case Key.F4 when e.Alt:
                    Close();

                    break;

                case Key.F11:
                    WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;

                    break;

                case Key.R:
                    CurrentWindow.Resetting = true;

                    break;

                case Key.Tab:
                    CurrentWindow.Reset();

                    break;
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.R)
                CurrentWindow.Resetting = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MusicPlayer.Dispose();
            ModelManager.Dispose();

            Shader.DestroyAll();
        }

        protected override void OnFocusedChanged(EventArgs e)
        {
            CursorGrabbed = Focused;
            CursorVisible = !Focused;
        }







        private int LoadMap(string data)
        {
            var currentTime = Settings.settings["currentTime"].Value;
            var startIndex = 0;

            var split = data.Split(',');

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            try
            {
                var id = split[0];

                for (int i = 1; i < split.Length; i++)
                {
                    var subsplit = split[i].Split('|');

                    var x = float.Parse(subsplit[0], culture);
                    var y = float.Parse(subsplit[1], culture);
                    var ms = long.Parse(subsplit[2]);

                    if (ms < currentTime)
                        startIndex++;

                    Notes.Add(new Note(x, y, ms));
                }

                var noteColors = Settings.settings["noteColors"];
                var colorCount = noteColors.Count;

                for (int i = 0; i < Notes.Count; i++)
                    Notes[i].Color = noteColors[i % colorCount];

                MusicPlayer.Load($"cached/{id}.asset");
            }
            catch (Exception e)
            {
                CursorVisible = true;
                MessageBox.Show($"Failed to load map\n\n{e.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            return startIndex;
        }

        private void RegisterModels()
        {
            var noteMain = ObjModel.FromFile("assets/models/note.obj");
            var cursorMain = ObjModel.FromFile("assets/models/cursor.obj");

            var quadVerts = new float[]
            {
                0, 0, 0,
                0, 1, 0,
                1, 0, 0,

                1, 1, 0,
                1, 0, 0,
                0, 1, 0,
            };

            ModelManager.RegisterModel("note", noteMain.GetVertexes(), true);
            ModelManager.RegisterModel("cursor", cursorMain.GetVertexes(), true);
            ModelManager.RegisterModel("quad3d", quadVerts);
        }
    }
}
