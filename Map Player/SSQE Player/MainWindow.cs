using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using SSQE_Player.GUI;
using System.Reflection;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
using SSQE_Player.Models;
using System.Drawing;
using SSQE_Player.Types;

namespace SSQE_Player
{
    internal class MainWindow : GameWindow
    {
        public static bool Replay = false;
        public static bool Autoplay = false;
        public static int AutoIndex = 0;

        public static MainWindow Instance;
        public MusicPlayer MusicPlayer = new();
        public SoundPlayer SoundPlayer = new();
        public ModelManager ModelManager = new();

        public Note[] Notes = [];
        public ReplayNode[] CursorNodes = [];
        public ReplayNode[] SkipNodes = [];

        public static readonly Vector3 NoteSize = Vector3.One * 0.875f;
        public static readonly Vector3 CursorSize = new(0.2625f, 0.2625f, 0f);
        public Vector3 CursorPos = new();
        public Vector3 LastCursorPos = new();

        public GuiWindowMain CurrentWindow;
        public Camera Camera = new();

        public float StartTime;
        public float Tempo;

        private bool isFullscreen = false;

        public int ReplayCursorIndex = 0;
        public int ReplaySkipIndex = 0;

        public float ReplayOffset = 0;

        private unsafe void SetInputMode()
        {
            GLFW.SetInputMode(WindowPtr, RawMouseMotionAttribute.RawMouseMotion, true);
        }

        public MainWindow(bool fromStart, int samples) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (1920, 1080),
            Title = $"Sound Space Map Player {Assembly.GetExecutingAssembly().GetName().Version}",
            NumberOfSamples = samples,
            WindowState = WindowState.Fullscreen
        }) 
        {
            SwitchFullscreen();

            VSync = VSyncMode.Off;

            CursorState = (!Replay && !Autoplay) ? CursorState.Grabbed : CursorState.Normal;
            SetInputMode();

            Shader.Init();
            FontRenderer.Init();

            Instance = this;

            if (Settings.limitPlayerFPS.Value)
            {
                if (Settings.useVSync.Value)
                    VSync = VSyncMode.On;
                else
                {
                    float fps = Settings.fpsLimit.Value.Value;
                    float max = Settings.fpsLimit.Value.Max;

                    RenderFrequency = Math.Round(fps) == Math.Round(max) ? 0f : fps + 60f;
                    UpdateFrequency = RenderFrequency;
                }
            }

            if (!Settings.fullscreenPlayer.Value)
                SwitchFullscreen();

            if (fromStart)
                Settings.currentTime.Value.Value = 0f;
            StartTime = Settings.currentTime.Value.Value;

            int startIndex = LoadMap(File.ReadAllText("assets/temp/tempmap.txt"));
            RegisterModels();

            MusicPlayer.Volume = Settings.masterVolume.Value.Value;
            SoundPlayer.Volume = Settings.sfxVolume.Value.Value;

            SetTempo(Settings.tempo.Value.Value);
            if (Replay)
                LoadReplay();

            Camera = new();
            CurrentWindow = new(startIndex);
        }

        private double time = -65d;
        private Vector2 mousePos;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Vector2 delta = MousePosition - mousePos;
            mousePos = MousePosition;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            time += args.Time * 1000;

            if (MusicPlayer.IsPlaying)
                Settings.currentTime.Value.Value = (float)MusicPlayer.CurrentTime.TotalMilliseconds;
            float currentTime = Settings.currentTime.Value.Value;

            if (Replay)
            {
                if (ReplayCursorIndex < CursorNodes.Length)
                {
                    ReplayNode a = CursorNodes[Math.Max(ReplayCursorIndex - 1, 0)];
                    ReplayNode b = CursorNodes[ReplayCursorIndex];

                    while (b.Ms <= time && ++ReplayCursorIndex < CursorNodes.Length)
                    {
                        a = b;
                        b = CursorNodes[ReplayCursorIndex];
                    }

                    float t = ((float)time - a.Ms) / (b.Ms - a.Ms);
                    Vector2 result = Lerp((a.X, a.Y), (b.X, b.Y), t);

                    Camera.SetReplay(result);
                    LastCursorPos = (result.X, result.Y, 0);
                }
                else
                    Close();

                if (ReplaySkipIndex < SkipNodes.Length)
                {
                    ReplayNode skip = SkipNodes[ReplaySkipIndex];

                    while (skip.Ms <= time && ReplaySkipIndex < SkipNodes.Length)
                    {
                        Skip();
                        skip = SkipNodes[ReplaySkipIndex++];
                    }
                }
            }
            else if (Autoplay)
            {
                if (AutoIndex < Notes.Length)
                {
                    Note last = AutoIndex > 0 ? Notes[AutoIndex - 1] : new(1, 1, 0, Vector3.Zero);
                    Note next = Notes[AutoIndex];

                    long diff = next.Ms - last.Ms;
                    float t = diff == 0 ? 1 : (currentTime - last.Ms - CurrentWindow.Offset) / diff;
                    Vector2 result = Lerp((last.X - 1, last.Y - 1), (next.X - 1, next.Y - 1), (float)Math.Sin(Math.Clamp(t, 0, 1) * MathHelper.PiOver2));

                    Camera.SetReplay(result);
                    LastCursorPos = (result.X, result.Y, 0);
                }
            }
            else
            {
                LastCursorPos = CursorPos;
                Camera.SetMouse(delta);
            }

            CurrentWindow?.Render((float)args.Time);

            SwapBuffers();
        }

        private static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return (b - a) * t + a;
        }

        private void Skip()
        {
            if (AutoIndex < Notes.Length)
            {
                MusicPlayer.Pause();
                Settings.currentTime.Value.Value = Notes[AutoIndex].Ms - 1000 * (0.875f + Tempo);
                MusicPlayer.Play();
            }
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0f, 0f, 0f, 1f);

            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(TriangleFace.Back);

            mousePos = MousePosition;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            if (e.Width > 0 && e.Height > 0)
            {
                int w = Math.Max(e.Width, 1280);
                int h = Math.Max(e.Height, 720);
                Size = (w, h);

                GL.Viewport(0, 0, w, h);
                base.OnResize(new(w, h));

                Shader.UploadOrtho(Shader.FontTexProgram, w, h);

                CurrentWindow?.OnResize(Size);

                if (Instance == null)
                    return;

                Camera.CalculateProjection();
            }
        }

        private void SwitchFullscreen()
        {
            isFullscreen ^= true;
            WindowState = isFullscreen ? WindowState.Fullscreen : WindowState.Maximized;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Escape:
                case Keys.F4 when e.Alt:
                    Close();
                    break;

                case Keys.F11:
                    SwitchFullscreen();
                    break;

                case Keys.R:
                    CurrentWindow.Resetting = true;
                    break;

                case Keys.Tab:
                    CurrentWindow.Reset();
                    break;

                case Keys.Space:
                    if (CurrentWindow.Paused && !CurrentWindow.Unpausing)
                    {
                        Settings.currentTime.Value.Value = CurrentWindow.PauseTime - 750f;
                        MusicPlayer.Play();
                        CurrentWindow.Unpausing = true;
                    }
                    else if (!CurrentWindow.Unpausing && MusicPlayer.IsPlaying && CurrentWindow.PauseTime + 1500f <= Settings.currentTime.Value.Value)
                    {
                        MusicPlayer.Pause();

                        CurrentWindow.PauseTime = Settings.currentTime.Value.Value;
                        CurrentWindow.Pauses++;
                        CurrentWindow.Paused = true;
                    }
                    break;

                case Keys.U when e.Control:
                    CursorState = CursorState.Normal;
                    break;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left && IsFocused)
                CursorState = CursorState.Grabbed;
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.R:
                    CurrentWindow.Resetting = false;
                    break;

                case Keys.Space:
                    if (CurrentWindow.Paused)
                    {
                        MusicPlayer.Pause();
                        Settings.currentTime.Value.Value = CurrentWindow.PauseTime;
                    }

                    CurrentWindow.Unpausing = false;
                    break;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            CurrentWindow.Offset += (int)e.OffsetY;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MusicPlayer.Dispose();
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            CursorState = (IsFocused && !Replay && !Autoplay) ? CursorState.Grabbed : CursorState.Normal;
        }




        public void LoadReplay()
        {
            using FileStream file = new("assets/temp/tempreplay.qer", FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(file);

            ConfirmTempo(reader.ReadSingle());

            int cursorCount = reader.ReadInt32();
            CursorNodes = new ReplayNode[cursorCount];

            for (int i = 0; i < cursorCount; i++)
                CursorNodes[i] = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadInt32(), ReplayType.Cursor);

            int skipCount = reader.ReadInt32();
            SkipNodes = new ReplayNode[skipCount];

            for (int i = 0; i < skipCount; i++)
                SkipNodes[i] = new(0, 0, reader.ReadInt32(), ReplayType.Skip);
        }

        private int LoadMap(string data)
        {
            float currentTime = Settings.currentTime.Value.Value;
            List<Color> noteColors = Settings.noteColors.Value;
            int colorCount = noteColors.Count;

            int startIndex = 0;

            string[] split = data.Split(',');

            try
            {
                string id = split[0];
                Notes = new Note[split.Length - 1];

                for (int i = 1; i < split.Length; i++)
                {
                    string[] subsplit = split[i].Split('|');

                    float x = float.Parse(subsplit[0]);
                    float y = float.Parse(subsplit[1]);
                    long ms = long.Parse(subsplit[2]);

                    if (ms < currentTime)
                        startIndex++;

                    Color color = noteColors[(i - 1) % colorCount];
                    Notes[i - 1] = new(x, y, ms, (color.R / 255f, color.G / 255f, color.B / 255f));
                }

                MusicPlayer.Load($"cached/{id}.asset");
            }
            catch
            {
                CursorState = CursorState.Normal;
                Close();
            }

            return startIndex;
        }

        private void RegisterModels()
        {
            ObjModel noteModel = ObjModel.FromFile("assets/models/note.obj");
            ObjModel cursorModel = ObjModel.FromFile("assets/models/cursor.obj");

            ModelManager.RegisterModel("note", noteModel.GetVertices(), Settings.noteScale.Value);
            ModelManager.RegisterModel("cursor", cursorModel.GetVertices(), Settings.cursorScale.Value);
        }

        private void SetTempo(float newTempo)
        {
            float tempoA = Math.Min(newTempo, 0.9f);
            float tempoB = (newTempo - tempoA) * 2f;

            ConfirmTempo(tempoA + tempoB + 0.1f);
        }

        private void ConfirmTempo(float newTempo)
        {
            Tempo = newTempo;
            MusicPlayer.Tempo = Tempo;
        }

        public int BinarySearchFirst(long key)
        {
            int first = 0;
            int count = Notes.Length - 1;

            int iter, step;

            while (count > 0)
            {
                step = count / 2;
                iter = first + step;

                if (Notes[iter].Ms < key)
                {
                    first = iter + 1;
                    count -= step + 1;
                }
                else
                    count = step;
            }

            return first;
        }

        public int BinarySearchLast(long key)
        {
            int first = 0;
            int count = Notes.Length - 1;

            int iter, step;

            while (count > 0)
            {
                step = count / 2;
                iter = first + step;

                if (Notes[iter].Ms <= key)
                {
                    first = iter + 1;
                    count -= step + 1;
                }
                else
                    count = step;
            }

            return first;
        }
    }
}
