using OpenTK.Graphics.OpenGL;
using New_SSQE.GUI;
using System.ComponentModel;
using System.Drawing;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using SkiaSharp;
using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
using OpenTK.Windowing.Common.Input;
using System.Runtime.InteropServices;
using System.IO.Compression;
using OpenTK.Graphics;
using New_SSQE.Objects;
using Assets = New_SSQE.Misc.Static.Assets;
using New_SSQE.GUI.Font;
using New_SSQE.Audio;
using New_SSQE.FileParsing;
using New_SSQE.GUI.Shaders;
using New_SSQE.Preferences;
using New_SSQE.GUI.Input;
using New_SSQE.Objects.Managers;
using New_SSQE.Misc.Network;
using New_SSQE.Misc.Dialogs;
using New_SSQE.Misc.Static;
using New_SSQE.ExternalUtils;
using New_SSQE.Maps;
using System.Diagnostics;

namespace New_SSQE
{
    /*
     * OpenGL functions used in this program (with their required versions):
     * 
     * 4.3 - glDebugMessageCallback (only used when DebugVersion is true, should be disabled on release)
     * 3.3 - glVertexAttribDivisor
     * 3.1 - glDrawArraysInstanced
     * 3.0 - glBindVertexArray
     * 3.0 - glBindRenderbuffer
     * 3.0 - glBindFramebuffer
     * 3.0 - glBlitFramebuffer
     * 3.0 - glGenRenderbuffers
     * 3.0 - glGenFramebuffers
     * 3.0 - glGenVertexArrays
     * 3.0 - glDeleteVertexArrays
     * 3.0 - glRenderbufferStorageMultisample
     * 3.0 - glFramebufferRenderbuffer
     * 2.0 - glBindBuffer
     * 2.0 - glBindTexture
     * 2.0 - glActiveTexture
     * 2.0 - glGenTextures
     * 2.0 - glGenBuffers
     * 2.0 - glGetUniformLocation
     * 2.0 - glUniform (4f, 3f, 2f, 1i)
     * 2.0 - glUniformMatrix (4f)
     * 2.0 - glDrawArrays
     * 2.0 - glBufferData
     * 2.0 - glDeleteTextures
     * 2.0 - glDeleteBuffers
     * 2.0 - glVertexAttribPointer
     * 2.0 - glUseProgram
     * 2.0 - glTexParameter (i)
     * 2.0 - glTexImage2D
     * 2.0 - glEnableVertexAttribArray
     * 2.0 - glViewport
     * 2.0 - glClear
     * 2.0 - glClearColor
     * 2.0 - glGetString
     * 2.0 - glGetError
     * 2.0 - glGetInteger
     * 2.0 - glEnable
     * 2.0 - glBlendFunc
     * 2.0 - glCreateShader
     * 2.0 - glCreateProgram
     * 2.0 - glShaderSource
     * 2.0 - glLinkProgram
     * 2.0 - glGetShaderInfoLog
     * 2.0 - glDetachShader
     * 2.0 - glDeleteShader
     * 2.0 - glCompileShader
     * 2.0 - glAttachShader
     * 
     * The requested API version should match the version of the highest unconditionally required function in this list.
     * Currently: 3.3 - glVertexAttribDivisor (required for efficient instancing of map objects and text)
     * 
     */

    internal class MainWindow : GameWindow
    {
        public static bool DebugVersion = false;

        public static readonly Vector2i SpriteSize = (4, 4);

        public static string InitialFile = "";

        public static MainWindow Instance;

        public GuiWindow CurrentWindow;

        public readonly Dictionary<Keys, Tuple<int, int>> KeyMapping = new();
        public static bool Focused = true;

        public Point Mouse = new(-1, -1);
        public Vector2 Delta = (0, 0);

        public bool CtrlHeld;
        public bool AltHeld;
        public bool ShiftHeld;
        public bool RightHeld;

        public static Avalonia.Controls.Window DefaultWindow;

        private bool isFullscreen = false;



        private static WindowIcon GetWindowIcon()
        {
            byte[] bytes = File.ReadAllBytes($"{Assets.TEXTURES}\\Icon.ico");
            SKBitmap bmp = SKBitmap.Decode(bytes, new SKImageInfo(256, 256, SKColorType.Rgba8888));
            OpenTK.Windowing.Common.Input.Image image = new(bmp.Width, bmp.Height, bmp.Bytes);

            return new(image);
        }

        private static void OnDebugMessage(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr pMessage, IntPtr pUserParam)
        {
            string message = Marshal.PtrToStringAnsi(pMessage, length);

            if (type == DebugType.DebugTypeError)
                Logging.Register($"[{severity} source={source} type={type} id={id}] {message}", LogSeverity.WARN);
        }
        private static readonly GLDebugProc DebugMessageDelegate = OnDebugMessage;
        
        public static int MaxSamples = 0;

        public static bool MSAA;
        public static string FileToLoad = "";

        public MainWindow(int samples) : base(GameWindowSettings.Default, new()
        {
            Size = (1280, 720),
            Title = $"Sound Space Quantum Editor {Program.Version}",
            WindowState = WindowState.Maximized,
            NumberOfSamples = samples,
            Icon = GetWindowIcon(),
            Flags = DebugVersion ? ContextFlags.Debug : 0,

            APIVersion = new(3, 3)
        })
        {
            MSAA = samples > 0;

            // requires higher OpenGL version
            if (DebugVersion)
            {
                GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
                GL.Enable(EnableCap.DebugOutput);
            }

            Logging.Register($"Required OpenGL version: {APIVersion}");
            Logging.Register("Current OpenGL version: " + (GL.GetString(StringName.Version) ?? "N/A"));
            
            string version = GL.GetString(StringName.Version) ?? "";
            int major = 0, minor = 0;
            GL.GetInteger(GetPName.MajorVersion, ref major);
            GL.GetInteger(GetPName.MinorVersion, ref minor);
            GL.GetInteger(GetPName.MaxFramebufferSamples, ref MaxSamples);

            if (string.IsNullOrWhiteSpace(version) || new Version(major, minor) < APIVersion)
                throw new Exception($"Unsupported OpenGL version (Minimum: {APIVersion})");

            Shader.Init();
            
            Instance = this;
            DefaultWindow = new BackgroundWindow();

            DiscordManager.Init();

            Settings.Init();

            CheckForUpdates();
            MessageBox.Instance?.Close();

            MapManager.LoadCache();

            OnMouseWheel(new MouseWheelEventArgs());
            SwitchWindow(new GuiWindowMenu());

            if (!string.IsNullOrWhiteSpace(InitialFile))
                MapManager.Load(InitialFile, true);
        }

        public void UpdateFPS(VSyncMode mode, float fps)
        {
            if (Context.IsCurrent)
                VSync = mode;

            float max = Settings.fpsLimit.Value.Max;

            RenderFrequency = Math.Round(fps) == Math.Round(max) ? 0f : fps + 60f;
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0f, 0f, 0f, 1f);
            
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        private double frameTime;
        private const double updateFrequency = 1 / 10.0;

        private double gcTime;
        private bool gcEnabled = true;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (attemptClose)
                RunClose();
            if (closed)
                return;

            if (Platform.IsLinux) // because all the other events for this are a key behind on linux (???)
            {
                KeyboardState keyboard = KeyboardState;

                CtrlHeld = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
                AltHeld = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
                ShiftHeld = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            }

            frameTime += args.Time;
            if (frameTime > updateFrequency)
            {
                ExportSSPM.UpdateID();
                frameTime = 0;

                if (!string.IsNullOrWhiteSpace(FileToLoad))
                {
                    MapManager.Load(FileToLoad, true);
                    FileToLoad = "";
                }
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (MusicPlayer.IsPlaying && CurrentWindow is GuiWindowEditor)
                Settings.currentTime.Value.Value = (float)MusicPlayer.CurrentTime.TotalMilliseconds;

            Delta = MouseState.Position - (Mouse.X, Mouse.Y);

            if (Delta.X != 0 || Delta.Y != 0)
            {
                Mouse.X = (int)MouseState.X;
                Mouse.Y = (int)MouseState.Y;
                CurrentWindow?.OnMouseMove(Mouse);
            }

            try
            {
                CurrentWindow?.Render(Mouse.X, Mouse.Y, (float)args.Time);

                if (RecordingMouse)
                {
                    float cur = Settings.currentTime.Value.Value;
                    long ms = Timing.GetClosestBeat(cur);

                    if (ms > 0 && ms <= cur && CurrentMap.Notes.FirstOrDefault(n => Math.Abs(n.Ms - ms) < 2) == null)
                    {
                        PointF point = CurrentWindow?.Grid?.PointToGridSpace(Mouse.X, Mouse.Y) ?? new();
                        NoteManager.Add("ADD NOTE", new Note(point.X, point.Y, ms));
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Register($"Failed to render frame", LogSeverity.ERROR, ex);
            }

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
            GL.BindVertexArray(VertexArrayHandle.Zero);

            OpenTK.Graphics.OpenGL.ErrorCode err = GL.GetError();
            while (err != OpenTK.Graphics.OpenGL.ErrorCode.NoError)
            {
                Logging.Register($"OpenGL Error: '{err}'", LogSeverity.WARN);
                err = GL.GetError();
            }

            SwapBuffers();

            // apparently the garbage collector doesnt want to deal with everything on its own .-.
            gcTime += args.Time;

            if (gcTime >= 2 && gcEnabled)
            {
                Stopwatch sw = Stopwatch.StartNew();

                GC.Collect();
                gcTime = 0;

                double duration = sw.Elapsed.TotalMilliseconds;

                // something is causing the gc to have a noticeable lag spike when it runs, so maybe now its doing more harm than good
                if (duration > 50)
                {
                    Logging.Register($"GC took {duration}ms to process! Disabling forced garbage collection to improve performance");
                    gcEnabled = false;
                }

                sw.Stop();
            }

            DiscordManager.Process(args.Time);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            if (e.Width > 0 && e.Height > 0)
            {
                int w = Math.Max(e.Width, 800);
                int h = Math.Max(e.Height, 600);
                Size = new Vector2i(w, h);

                base.OnResize(new ResizeEventArgs(w, h));
                GL.Viewport(0, 0, w, h);
                Shader.UploadOrtho(w, h);

                CurrentWindow?.OnResize(Size);

                OnRenderFrame(new FrameEventArgs());
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            CurrentWindow?.OnMouseUp(Mouse);
            if (e.Button == MouseButton.Right)
                RightHeld = false;
        }

        private bool ClickLocked = false;

        public void LockClick() => ClickLocked = true;
        public void UnlockClick() => ClickLocked = false;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!ClickLocked)
                CurrentWindow?.OnMouseClick(Mouse, e.Button == MouseButton.Right);
            RightHeld = RightHeld || e.Button == MouseButton.Right;
        }

        protected override void OnMouseLeave()
        {
            CurrentWindow?.OnMouseLeave(Mouse);

            Mouse = new Point(-1, -1);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            CtrlHeld = e.Control;
            AltHeld = e.Alt;
            ShiftHeld = e.Shift;
        }

        private void SwitchFullscreen()
        {
            isFullscreen ^= true;
            WindowState = isFullscreen ? WindowState.Fullscreen : WindowState.Maximized;
        }

        private bool RecordingMouse = false;

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            CtrlHeld = e.Control;
            AltHeld = e.Alt;
            ShiftHeld = e.Shift;

            if (e.Key == Keys.F11)
            {
                SwitchFullscreen();
                return;
            }

            if (e.Key == Keys.F4 && AltHeld)
                Close();

            if (!FocusingBox())
            {
                if (CurrentWindow is GuiWindowEditor editor)
                {
                    if (e.Key == Keys.Space && !editor.Timeline.Dragging)
                    {
                        if (MusicPlayer.IsPlaying)
                        {
                            RecordingMouse = false;
                            MusicPlayer.Pause();
                        }
                        else
                        {
                            if (ShiftHeld && CtrlHeld)
                                RecordingMouse = true;
                            SliderSetting currentTime = Settings.currentTime.Value;
                            
                            if (currentTime.Value >= currentTime.Max - 1)
                                currentTime.Value = 0;

                            MusicPlayer.Play();
                        }
                    }

                    if (e.Key == Keys.Left || e.Key == Keys.Right)
                        Timing.Scroll(e.Key == Keys.Left);

                    if (e.Key == Keys.Escape)
                        CurrentMap.ClearSelection();

                    KeybindManager.ParseKeybind(e.Key, CtrlHeld, AltHeld, ShiftHeld);
                }
            }

            CurrentWindow?.OnKeyDown(e.Key, e.Control);
        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            for (int i = 0; i < e.FileNames.Length; i++)
            {
                string file = e.FileNames[i];

                if (File.Exists(file))
                {
                    bool loaded = true;
                    Map? prev = CurrentMap.LoadedMap;

                    if (Path.GetExtension(file) == ".ini" && CurrentWindow is GuiWindowEditor)
                        MapManager.ImportProperties(file);
                    else if (MusicPlayer.SupportedExtensions.Contains(Path.GetExtension(file)))
                    {
                        string id = Exporting.FixID(Path.GetFileNameWithoutExtension(file));
                        if (file != $"{Assets.CACHED}\\{id}.asset")
                            File.Copy(file, $"{Assets.CACHED}\\{id}.asset", true);

                        loaded = MapManager.Load(id);
                    }
                    else
                        loaded = MapManager.Load(file, true);

                    if (!loaded && prev != null)
                    {
                        prev.Load();

                        if (CurrentWindow is GuiWindowEditor editor)
                            editor.Timeline.GenerateOffsets();
                    }
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            KeyboardState keyboard = KeyboardState;

            CtrlHeld = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            AltHeld = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
            ShiftHeld = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            if (CurrentWindow is GuiWindowEditor)
            {
                if (ShiftHeld)
                {
                    SliderSetting setting = Settings.beatDivisor.Value;
                    float step = setting.Step * (CtrlHeld ? 1 : 2) * e.OffsetY;

                    setting.Value = MathHelper.Clamp(setting.Value + step, 0f, setting.Max);
                }
                else if (CtrlHeld)
                    CurrentMap.IncrementZoom(e.OffsetY);
                else
                    Timing.Scroll(e.OffsetY < 0 ^ Settings.reverseScroll.Value, Math.Abs(e.OffsetY));
            }
            else if (CurrentWindow is GuiWindowMenu menu)
            {
                if (menu.MapSelectBackdrop.Rect.Contains(Mouse))
                    menu.ScrollMaps(e.OffsetY > 0);
                else
                {
                    SliderSetting setting = Settings.changelogPosition.Value;

                    setting.Value = MathHelper.Clamp(setting.Value + setting.Step * e.OffsetY, 0f, setting.Max);
                }
            }
        }

        private bool forceClose = false;
        private bool attemptClose = false;
        private bool closed = false;

        private void RunClose()
        {
            attemptClose = false;

            bool cancel = false;

            List<Map> tempSave = new();
            List<Map> tempKeep = new();

            foreach (Map map in MapManager.Cache.ToList())
            {
                if (map.IsSaved)
                    tempKeep.Add(map);
                else
                    tempSave.Add(map);
            }

            foreach (Map map in tempSave)
            {
                MapManager.Load(map, false);
                OnRenderFrame(new FrameEventArgs());

                cancel |= !map.Close(false);
                if (cancel)
                    break;
            }

            if (!cancel)
            {
                foreach (Map map in tempKeep)
                    map.Close(false, false, false);
            }
            else
                SwitchWindow(new GuiWindowMenu());

            forceClose = !cancel;

            if (!cancel)
                Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!forceClose)
            {
                attemptClose = true;
                e.Cancel = true;
            }
            else
            {
                closed = true;

                if (CurrentWindow is GuiWindowMenu menu)
                    menu.AssembleMapList();

                Settings.Save();

                TimingsWindow.Instance?.Close();
                BookmarksWindow.Instance?.Close();

                MusicPlayer.Dispose();
                CurrentWindow?.Dispose();
                DiscordManager.Dispose();
            }
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            Focused = e.IsFocused;

            if (e.IsFocused)
                UpdateFPS(Settings.useVSync.Value ? VSyncMode.On : VSyncMode.Off, Settings.fpsLimit.Value.Value);
            else
                UpdateFPS(VSyncMode.Off, -45f);
        }



        private bool FocusingBox()
        {
            if (CurrentWindow == null)
                return false;

            foreach (WindowControl control in CurrentWindow.Controls)
                if (control is GuiTextbox box && box.Focused)
                    return true;

            return false;
        }



        public void SwitchWindow(GuiWindow window)
        {
            if (CurrentWindow is GuiWindowEditor)
                CurrentMap.LoadedMap?.Save();

            if (window is GuiWindowEditor)
            {
                DiscordManager.SetActivity(DiscordStatus.Editor);
                MapManager.BeginAutosaveLoop(DateTime.Now.Ticks);
            }
            else if (window is GuiWindowMenu)
            {
                DiscordManager.SetActivity(DiscordStatus.Menu);
                Waveform.Dispose();
            }

            ExportSSPM.Instance?.Close();
            BPMTapper.Instance?.Close();
            TimingsWindow.Instance?.Close();
            BookmarksWindow.Instance?.Close();

            FontRenderer.unicode = Settings.language.Value != "english" || window is GuiWindowLanguage;
            foreach (WindowControl control in window.Controls)
                control.Update();

            CurrentWindow?.Dispose();
            CurrentWindow = window;

            Settings.Save();
        }



        public static void CheckForUpdates()
        {
            if (!Settings.checkUpdates.Value)
                return;

            static void ExtractFile(string path)
            {
                using (ZipArchive archive = ZipFile.OpenRead(path))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        try
                        {
                            entry.ExtractToFile(entry.FullName, true);
                        }
                        catch (Exception ex) { Logging.Register($"Failed to extract file: {entry.FullName}", LogSeverity.WARN, ex); }
                    }
                }

                File.Delete(path);
            }

            void Download(string file)
            {
                Logging.Register($"Attempting to download file '{file}'");
                WebClient.DownloadFile(Links.ALL[$"{file} Zip"], $"{Assets.THIS}\\{file}.zip");
                ExtractFile($"{Assets.THIS}\\{file}.zip");
            }

            void Run(string file, string tag, Setting<string> setting)
            {
                Logging.Register($"Searching for file '{file}'");

                if (Platform.ExecutableExists(file))
                {
                    string current = Platform.IsLinux ? setting.Value : Platform.GetExecutableVersionInfo(file).FileVersion ?? "";
                    string version = WebClient.DownloadString(Links.ALL[$"{file} Version"]).Trim();

                    if (Version.Parse(current) < Version.Parse(version))
                    {
                        Logging.Register($"Current and latest versions differ! Current: {current} | Latest: {version}");

                        DialogResult diag = MessageBox.Show($"New {tag} version is available ({version}). Would you like to download the new version?", MBoxIcon.Info, MBoxButtons.Yes_No);

                        if (diag == DialogResult.Yes)
                        {
                            Download(file);
                            setting.Value = version;
                        }
                    }
                }
                else
                {
                    DialogResult diag = MessageBox.Show($"{tag} is not present in this directory. Would you like to download it?", MBoxIcon.Info, MBoxButtons.Yes_No);

                    if (diag == DialogResult.Yes)
                    {
                        string version = WebClient.DownloadString(Links.ALL[$"{file} Version"]).Trim();
                        
                        Download(file);
                        setting.Value = version;
                    }
                }
            }
            
            try
            {
                Run("SSQE Player", "Map Player", Settings.SSQE_Player_Version);
                Run("SSQE Updater", "Auto Updater", Settings.SSQE_Updater_Version);

                string redirect = WebClient.GetRedirect(Links.EDITOR_REDIRECT);

                if (Platform.ExecutableExists("SSQE Updater") && redirect != "")
                {
                    string version = redirect[(redirect.LastIndexOf('/') + 1)..];

                    Logging.Register("Checking version of editor");
                    if (Version.Parse(version) > Version.Parse(Program.Version))
                    {
                        DialogResult diag = MessageBox.Show($"New Editor version is available ({version}). Would you like to download the new version?", MBoxIcon.Info, MBoxButtons.Yes_No);

                        if (diag == DialogResult.Yes)
                        {
                            Logging.Register("Attempting to run updater");
                            Platform.RunExecutable("SSQE Updater", "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Register("Failed to check for updates", LogSeverity.WARN, ex);
                MessageBox.Show("Failed to check for updates", MBoxIcon.Warning, MBoxButtons.OK);
            }
        }
    }
}
