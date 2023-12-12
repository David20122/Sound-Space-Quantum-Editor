using OpenTK.Graphics.OpenGL;
using New_SSQE.GUI;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Json;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using SkiaSharp;
using Discord;
using Activity = Discord.Activity;
using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
using OpenTK.Windowing.Common.Input;
using System.Runtime.InteropServices;
using BigInteger = System.Numerics.BigInteger;
using System.IO.Compression;
using OpenTK.Graphics;
using New_SSQE.Types;

namespace New_SSQE
{
    internal class MainWindow : GameWindow
    {
        public static MainWindow Instance;
        public MusicPlayer MusicPlayer = new();
        public SoundPlayer SoundPlayer = new();
        public UndoRedoManager UndoRedoManager = new();

        public GuiWindow CurrentWindow;

        public List<Map> Maps = new();
        public Map CurrentMap;
        private Map prevMap;

        public List<Note> Notes = new();
        public List<Note> SelectedNotes = new();
        public List<Note> BezierNodes = new();

        public List<TimingPoint> TimingPoints = new();
        public TimingPoint? SelectedPoint;

        public List<Bookmark> Bookmarks = new();

        public readonly Dictionary<Keys, Tuple<int, int>> KeyMapping = new();

        public Point Mouse = new(-1, -1);

        public float Tempo = 1f;
        public float Zoom = 1f;
        public float NoteStep => 500f * Zoom;

        public bool CtrlHeld;
        public bool AltHeld;
        public bool ShiftHeld;
        public bool RightHeld;

        public string? FileName;
        public string SoundID = "-1";

        public static Avalonia.Controls.Window DefaultWindow = new BackgroundWindow();

        private Discord.Discord discord;
        private ActivityManager activityManager;
        private bool discordEnabled = File.Exists("discord_game_sdk.dll");



        private bool closing = false;

        // hacky workaround for fullscreen being awful
        private bool isFullscreen = false;
        private Vector2i startSize = new(1920, 1080);

        private void SwitchFullscreen()
        {
            isFullscreen ^= true;

            WindowState = isFullscreen ? WindowState.Normal : WindowState.Maximized;
            WindowBorder = isFullscreen ? WindowBorder.Hidden : WindowBorder.Resizable;

            if (isFullscreen)
            {
                Size = startSize;
                Location = (0, 0);
            }
        }



        private static void OnDebugMessage(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr pMessage, IntPtr pUserParam)
        {
            string message = Marshal.PtrToStringAnsi(pMessage, length);

            if (type == DebugType.DebugTypeError)
                ActionLogging.Register($"[OpenGL Error] (source={source} type={type} id={id}) {message}", "WARN");
        }

        private static GLDebugProc DebugMessageDelegate = OnDebugMessage;



        private static WindowIcon GetWindowIcon()
        {
            var bytes = File.ReadAllBytes("assets/textures/Icon.ico");
            var bmp = SKBitmap.Decode(bytes, new SKImageInfo(256, 256, SKColorType.Rgba8888));
            var image = new OpenTK.Windowing.Common.Input.Image(bmp.Width, bmp.Height, bmp.Bytes);

            return new WindowIcon(image);
        }

        private const string cacheFile = "assets/temp/cache.txt";

        public MainWindow() : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (1280, 720),
            Title = $"Sound Space Quantum Editor {Assembly.GetExecutingAssembly().GetName().Version}",
            NumberOfSamples = 32,
            WindowState = WindowState.Maximized,
            Icon = GetWindowIcon(),
            Flags = ContextFlags.Debug
        })
        {
            GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);

            Shader.Init();

            Instance = this;

            UpdateFrequency = 1 / 20.0;

            DiscordInit();
            SetActivity("Sitting in the menu");

            Settings.Load();

            CheckForUpdates();

            if (File.Exists(cacheFile) && !string.IsNullOrWhiteSpace(File.ReadAllText(cacheFile)))
                LoadCache();

            OnMouseWheel(new MouseWheelEventArgs());
            SwitchWindow(new GuiWindowMenu());
        }

        public void SetVSync(VSyncMode mode)
        {
            VSync = mode;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (discordEnabled)
                try { discord.RunCallbacks(); } catch { }

            ExportSSPM.UpdateID();
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0f, 0f, 0f, 1f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (closing)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            if (MusicPlayer.IsPlaying && CurrentWindow is GuiWindowEditor)
                Settings.settings["currentTime"].Value = (float)MusicPlayer.CurrentTime.TotalMilliseconds;

            var mouse = MouseState;
            if (mouse.Delta.Length != 0)
                CurrentWindow?.OnMouseMove(Mouse);
            Mouse.X = (int)mouse.X;
            Mouse.Y = (int)mouse.Y;

            try
            {
                CurrentWindow?.Render(Mouse.X, Mouse.Y, (float)args.Time);
            }
            catch (Exception ex)
            {
                ActionLogging.Register($"Failed to render frame - {ex.GetType().Name}\n{ex.StackTrace}", "ERROR");
            }

            if (CurrentMap != prevMap && CurrentWindow is GuiWindowEditor editor)
            {
                editor.Timeline.GenerateOffsets();
                prevMap = CurrentMap;
            }

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
            GL.BindVertexArray(VertexArrayHandle.Zero);
            
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            var w = Math.Max(e.Width, 800);
            var h = Math.Max(e.Height, 600);
            Size = new Vector2i(w, h);

            base.OnResize(new ResizeEventArgs(w, h));
            GL.Viewport(0, 0, w, h);

            Shader.UploadOrtho(Shader.Program, w, h);
            Shader.UploadOrtho(Shader.TexProgram, w, h);
            Shader.UploadOrtho(Shader.FontTexProgram, w, h);
            Shader.UploadOrtho(Shader.NoteInstancedProgram, w, h);
            Shader.UploadOrtho(Shader.InstancedProgram, w, h);
            Shader.UploadOrtho(Shader.GridInstancedProgram, w, h);
            Shader.UploadOrtho(Shader.WaveformProgram, w, h);

            CurrentWindow?.OnResize(Size);

            OnRenderFrame(new FrameEventArgs());
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
                            MusicPlayer.Pause();
                        else
                        {
                            var currentTime = Settings.settings["currentTime"];

                            if (currentTime.Value >= currentTime.Max - 1)
                                currentTime.Value = 0;

                            MusicPlayer.Play();
                        }
                    }

                    if (e.Key == Keys.Left || e.Key == Keys.Right)
                    {
                        if (MusicPlayer.IsPlaying)
                            MusicPlayer.Pause();

                        Advance(e.Key == Keys.Left);
                    }

                    if (e.Key == Keys.Escape)
                    {
                        SelectedNotes.Clear();
                        UpdateSelection();
                        SelectedPoint = null;
                    }

                    var keybind = Settings.CompareKeybind(e.Key, CtrlHeld, AltHeld, ShiftHeld);

                    if (keybind.Contains("gridKey"))
                    {
                        var rep = keybind.Replace("gridKey", "");
                        string[] xy = rep.Split('|');

                        var x = int.Parse(xy[0]);
                        var y = int.Parse(xy[1]);
                        var ms = GetClosestBeat(Settings.settings["currentTime"].Value);

                        var note = new Note(x, y, (long)(ms >= 0 ? ms : Settings.settings["currentTime"].Value));

                        UndoRedoManager.Add("ADD NOTE", () =>
                        {
                            Notes.Remove(note);
                            SortNotes();
                        }, () =>
                        {
                            Notes.Add(note);
                            SortNotes();
                        });

                        if (Settings.settings["autoAdvance"])
                            Advance();

                        return;
                    }

                    if (keybind.Contains("pattern"))
                    {
                        var index = int.Parse(keybind.Replace("pattern", ""));

                        if (ShiftHeld)
                            BindPattern(index);
                        else if (CtrlHeld)
                            UnbindPattern(index);
                        else
                            CreatePattern(index);

                        return;
                    }

                    switch (keybind)
                    {
                        case "selectAll":
                            SelectedPoint = null;
                            SelectedNotes = Notes.ToList();
                            UpdateSelection();

                            break;

                        case "save":
                            if (SaveMap(true))
                                editor.ShowToast("SAVED", Settings.settings["color1"]);

                            break;

                        case "saveAs":
                            if (SaveMap(true, true))
                                editor.ShowToast("SAVED", Settings.settings["color1"]);

                            break;

                        case "undo":
                            UndoRedoManager.Undo();

                            break;

                        case "redo":
                            UndoRedoManager.Redo();

                            break;

                        case "copy":
                            try
                            {
                                if (SelectedNotes.Count > 0)
                                {
                                    var copied = SelectedNotes.ToList();

                                    Clipboard.SetData(copied);

                                    editor.ShowToast("COPIED NOTES", Settings.settings["color1"]);
                                }
                            }
                            catch { editor.ShowToast("FAILED TO COPY", Settings.settings["color1"]); }

                            break;

                        case "paste":
                            try
                            {
                                var copied = Clipboard.GetData().ToList();

                                if (copied.Count > 0)
                                {
                                    var offset = copied.Min(n => n.Ms);

                                    copied.ForEach(n => n.Ms = (long)Settings.settings["currentTime"].Value + n.Ms - offset);

                                    if (Settings.settings["applyOnPaste"])
                                    {
                                        if (float.TryParse(editor.RotateBox.Text, out var deg) && float.TryParse(editor.ScaleBox.Text, out var scale))
                                        {
                                            foreach (var note in copied.ToList())
                                            {
                                                var angle = MathHelper.RadiansToDegrees(Math.Atan2(note.Y - 1, note.X - 1));
                                                var distance = Math.Sqrt(Math.Pow(note.X - 1, 2) + Math.Pow(note.Y - 1, 2));
                                                var anglef = MathHelper.DegreesToRadians(angle + deg);

                                                note.X = (float)(Math.Cos(anglef) * distance + 1);
                                                note.Y = (float)(Math.Sin(anglef) * distance + 1);

                                                var scalef = scale / 100f;

                                                note.X = (note.X - 1) * scalef + 1;
                                                note.Y = (note.Y - 1) * scalef + 1;
                                            }
                                        }
                                    }

                                    UndoRedoManager.Add("Paste Notes", () =>
                                    {
                                        SelectedNotes.Clear();
                                        SelectedPoint = null;

                                        for (int i = 0; i < copied.Count; i++)
                                            Notes.Remove(copied[i]);
                                        UpdateSelection();

                                        SortNotes();
                                    }, () =>
                                    {
                                        SelectedNotes = copied.ToList();
                                        SelectedPoint = null;

                                        Notes.AddRange(copied);
                                        UpdateSelection();

                                        SortNotes();
                                    });
                                }
                            }
                            catch { editor.ShowToast("FAILED TO PASTE", Settings.settings["color1"]); }

                            break;

                        case "cut":
                            try
                            {
                                if (SelectedNotes.Count > 0)
                                {
                                    var copied = SelectedNotes.ToList();

                                    Clipboard.SetData(copied);

                                    UndoRedoManager.Add($"CUT NOTE{(copied.Count > 1 ? "S" : "")}", () =>
                                    {
                                        Notes.AddRange(copied);

                                        SortNotes();
                                    }, () =>
                                    {
                                        foreach (var note in copied)
                                            Notes.Remove(note);

                                        SortNotes();
                                    });

                                    SelectedNotes.Clear();
                                    UpdateSelection();
                                    SelectedPoint = null;

                                    editor.ShowToast("CUT NOTES", Settings.settings["color1"]);
                                }
                            }
                            catch { editor.ShowToast("FAILED TO CUT", Settings.settings["color1"]); }

                            break;

                        case "delete":
                            if (SelectedNotes.Count > 0)
                            {
                                var toRemove = SelectedNotes.ToList();

                                UndoRedoManager.Add($"DELETE NOTE{(toRemove.Count > 1 ? "S" : "")}", () =>
                                {
                                    Notes.AddRange(toRemove);

                                    SortNotes();
                                }, () =>
                                {
                                    foreach (var note in toRemove)
                                        Notes.Remove(note);

                                    SortNotes();
                                });

                                SelectedNotes.Clear();
                                UpdateSelection();
                                SelectedPoint = null;
                            }
                            else if (SelectedPoint != null)
                            {
                                var clone = SelectedPoint;

                                UndoRedoManager.Add("DELETE POINT", () =>
                                {
                                    TimingPoints.Add(clone);

                                    SortTimings();
                                }, () =>
                                {
                                    TimingPoints.Remove(clone);

                                    SortTimings();
                                });
                            }

                            break;

                        case "hFlip":
                            var selectedH = SelectedNotes.ToList();

                            editor.ShowToast("HORIZONTAL FLIP", Settings.settings["color1"]);

                            UndoRedoManager.Add("HORIZONTAL FLIP", () =>
                            {
                                foreach (var note in selectedH)
                                    note.X = 2 - note.X;
                            }, () =>
                            {
                                foreach (var note in selectedH)
                                    note.X = 2 - note.X;
                            });

                            break;

                        case "vFlip":
                            var selectedV = SelectedNotes.ToList();

                            editor.ShowToast("VERTICAL FLIP", Settings.settings["color1"]);

                            UndoRedoManager.Add("VERTICAL FLIP", () =>
                            {
                                foreach (var note in selectedV)
                                    note.Y = 2 - note.Y;
                            }, () =>
                            {
                                foreach (var note in selectedV)
                                    note.Y = 2 - note.Y;
                            });

                            break;

                        case "switchClickTool":
                            Settings.settings["selectTool"] ^= true;

                            break;

                        case "quantum":
                            Settings.settings["enableQuantum"] ^= true;

                            break;

                        case "openTimings":
                            TimingsWindow.ShowWindow();

                            break;

                        case "openBookmarks":
                            BookmarksWindow.ShowWindow();

                            break;

                        case "storeNodes":
                            if (SelectedNotes.Count > 1)
                                BezierNodes = SelectedNotes.ToList();

                            break;

                        case "drawBezier":
                            RunBezier();

                            break;

                        case "anchorNode":
                            var selectedA = SelectedNotes.ToList();

                            UndoRedoManager.Add($"ANCHOR NODE{(selectedA.Count > 1 ? "S" : "")}", () =>
                            {
                                foreach (var note in selectedA)
                                    note.Anchored ^= true;
                            }, () =>
                            {
                                foreach (var note in selectedA)
                                    note.Anchored ^= true;
                            });

                            break;

                        case "openDirectory":
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                                // if mac
                                Process.Start("open", $"\"{Environment.CurrentDirectory}\"");
                            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                // if windows
                                Process.Start("explorer.exe", Environment.CurrentDirectory);
                            else // linux probably
                                ActionLogging.Register($"Open dir not implemented on platform {RuntimeInformation.OSDescription}", "WARN");

                            break;

                        case "exportSSPM":
                            ExportSSPM.ShowWindow();
                            
                            break;
                    }
                }
            }

            CurrentWindow?.OnKeyDown(e.Key, e.Control);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            var keyboard = KeyboardState;

            CtrlHeld = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            AltHeld = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
            ShiftHeld = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            if (CurrentWindow is GuiWindowEditor)
            {
                if (ShiftHeld)
                {
                    var setting = Settings.settings["beatDivisor"];
                    var step = setting.Step * (CtrlHeld ? 1 : 2) * e.OffsetY;

                    setting.Value = MathHelper.Clamp(setting.Value + step, 0f, setting.Max);
                }
                else if (CtrlHeld)
                {
                    var step = Zoom < 0.1f || (Zoom == 0.1f && e.OffsetY < 0) ? 0.01f : 0.1f;

                    Zoom = (float)Math.Round(Zoom + e.OffsetY * step, 2);
                    if (Zoom > 0.1f)
                        Zoom = (float)Math.Round(Zoom * 10) / 10;

                    Zoom = MathHelper.Clamp(Zoom, 0.01f, 10f);
                }
                else
                {
                    if (MusicPlayer.IsPlaying)
                        MusicPlayer.Pause();

                    float delta = e.OffsetY * (Settings.settings["reverseScroll"] ? -1 : 1);

                    var setting = Settings.settings["currentTime"];
                    var currentTime = setting.Value;
                    var totalTime = setting.Max;

                    var closest = GetClosestBeatScroll(currentTime, delta < 0);
                    var bpm = GetCurrentBpm(0);

                    currentTime = closest >= 0 || bpm.BPM > 0 ? closest : currentTime + delta / 10f * 1000f / Zoom * 0.5f;

                    if (GetCurrentBpm(setting.Value).BPM == 0 && GetCurrentBpm(currentTime).BPM != 0)
                        currentTime = GetCurrentBpm(currentTime).Ms;

                    currentTime = MathHelper.Clamp(currentTime, 0f, totalTime);

                    setting.Value = currentTime;
                }
            }
            else if (CurrentWindow is GuiWindowMenu)
            {
                var setting = Settings.settings["changelogPosition"];

                setting.Value = MathHelper.Clamp(setting.Value + setting.Step * e.OffsetY, 0f, setting.Max);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;

            bool cancel = false;

            Map temp = CurrentMap;

            List<Map> tempSave = new();
            List<Map> tempKeep = new();

            foreach (Map map in Maps.ToList())
            {
                if (map.IsSaved())
                    tempKeep.Add(map);
                else
                    tempSave.Add(map);
            }

            foreach (Map map in tempSave)
                cancel |= !map.Close(false);

            if (!cancel)
            {
                foreach (Map map in tempKeep)
                    map.Close(false, false, false);
            }
            else
                temp?.MakeCurrent();

            e.Cancel = cancel;

            if (CurrentWindow is GuiWindowMenu menu)
                menu.AssembleMapList();

            Settings.Save();

            TimingsWindow.Instance?.Close();
            BookmarksWindow.Instance?.Close();

            if (!e.Cancel)
            {
                MusicPlayer.Dispose();
                CurrentWindow?.Dispose();
            }

            closing = !e.Cancel;
        }








        public PointF PointToGridSpace(float mousex, float mousey)
        {
            var pos = new PointF(0, 0);

            if (CurrentWindow is GuiWindowEditor editor)
            {
                var quantum = Settings.settings["enableQuantum"];
                var rect = editor.Grid.Rect;

                var bounds = quantum ? new Vector2d(-0.85f, 2.85f) : new Vector2d(0, 2);

                var increment = quantum ? (Settings.settings["quantumSnapping"].Value + 3f) / 3f : 1f;
                var x = (mousex - rect.X - rect.Width / 2f) / rect.Width * 3f + 1 / increment;
                var y = (mousey - rect.Y - rect.Width / 2f) / rect.Height * 3f + 1 / increment;

                if (Settings.settings["quantumGridSnap"])
                {
                    x = (float)Math.Floor((x + 1 / increment / 2) * increment) / increment;
                    y = (float)Math.Floor((y + 1 / increment / 2) * increment) / increment;
                }

                x = (float)MathHelper.Clamp(x - 1 / increment + 1, bounds.X, bounds.Y);
                y = (float)MathHelper.Clamp(y - 1 / increment + 1, bounds.X, bounds.Y);

                pos = new PointF(x, y);
            }

            return pos;
        }

        public void UpdateSelection()
        {
            for (int i = 0; i < Notes.Count; i++)
                Notes[i].Selected = false;
            for (int i = 0; i < SelectedNotes.Count; i++)
                SelectedNotes[i].Selected = true;
        }

        public long GetClosestNote(float currentMs)
        {
            long closestMs = -1;

            for (int i = 0; i < Notes.Count; i++)
            {
                var note = Notes[i];

                if (Math.Abs(note.Ms - currentMs) < Math.Abs(closestMs - currentMs))
                    closestMs = note.Ms;
            }

            return closestMs;
        }

        public long GetClosestBeat(float currentMs, bool draggingPoint = false)
        {
            long closestMs = -1;
            var point = GetCurrentBpm(currentMs, draggingPoint);

            if (point.BPM > 0)
            {
                var interval = 60 / point.BPM * 1000f / (Settings.settings["beatDivisor"].Value + 1f);
                var offset = point.Ms % interval;

                closestMs = (long)Math.Round((long)Math.Round((currentMs - offset) / interval) * interval + offset);
            }

            return closestMs;
        }

        public long GetClosestBeatScroll(float currentMs, bool negative = false, int iterations = 1)
        {
            var closestMs = GetClosestBeat(currentMs);

            if (GetCurrentBpm(closestMs).BPM == 0)
                return -1;

            for (int i = 0; i < iterations; i++)
            {
                var currentPoint = GetCurrentBpm(currentMs, negative);
                var interval = 60000 / currentPoint.BPM / (Settings.settings["beatDivisor"].Value + 1f);

                if (negative)
                {
                    closestMs = GetClosestBeat(currentMs, true);

                    if (closestMs >= currentMs)
                        closestMs = GetClosestBeat(closestMs - (long)interval);
                }
                else
                {
                    if (closestMs <= currentMs)
                        closestMs = GetClosestBeat(closestMs + (long)interval);

                    if (GetCurrentBpm(currentMs).Ms != GetCurrentBpm(closestMs).Ms)
                        closestMs = GetCurrentBpm(closestMs, false).Ms;
                }

                currentMs = closestMs;
            }

            if (closestMs < 0)
                return -1;

            return (long)MathHelper.Clamp(closestMs, 0, Settings.settings["currentTime"].Max);
        }

        public TimingPoint GetCurrentBpm(float currentMs, bool draggingPoint = false)
        {
            var currentPoint = new TimingPoint(0, 0);

            for (int i = 0; i < TimingPoints.Count; i++)
            {
                var point = TimingPoints[i];

                if (point.Ms < currentMs || (!draggingPoint && point.Ms == currentMs))
                    currentPoint = point;
            }

            return currentPoint;
        }

        public void Advance(bool reverse = false)
        {
            var currentMs = Settings.settings["currentTime"];
            var bpm = GetCurrentBpm(currentMs.Value).BPM;

            if (bpm > 0)
                currentMs.Value = GetClosestBeatScroll(currentMs.Value, reverse);
        }




        public void BindPattern(int index)
        {
            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            string pattern = "";
            long minDist = 0;

            for (int i = 0; i + 1 < SelectedNotes.Count; i++)
            {
                var dist = Math.Abs(SelectedNotes[i].Ms - SelectedNotes[i + 1].Ms);

                if (dist > 0)
                    minDist = minDist > 0 ? Math.Min(minDist, dist) : dist;
            }

            foreach (var note in SelectedNotes)
            {
                var offset = SelectedNotes[0].Ms;

                var x = note.X.ToString(culture);
                var y = note.Y.ToString(culture);
                var time = (minDist > 0 ? Math.Round((double)(note.Ms - offset) / minDist) : 0).ToString(culture);

                pattern += $",{x}|{y}|{time}";
            }

            if (pattern.Length > 0)
                pattern = pattern[1..];

            if (CurrentWindow is GuiWindowEditor editor)
                editor.ShowToast($"BOUND PATTERN TO KEY {index}", Settings.settings["color1"]);

            Settings.settings["patterns"][index] = pattern;
        }

        public void UnbindPattern(int index)
        {
            if (CurrentWindow is GuiWindowEditor editor)
            {
                Settings.settings["patterns"][index] = "";
                editor.ShowToast($"UNBOUND PATTERN {index}", Settings.settings["color1"]);
            }
        }

        public void CreatePattern(int index)
        {
            var pattern = Settings.settings["patterns"][index];

            if (pattern == "")
                return;

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            string[] patternSplit = pattern.split(',');
            var toAdd = new List<Note>();

            foreach (var note in patternSplit)
            {
                string[] noteSplit = note.Split('|');
                var x = float.Parse(noteSplit[0], culture);
                var y = float.Parse(noteSplit[1], culture);
                var time = int.Parse(noteSplit[2], culture);
                var ms = GetClosestBeatScroll((long)Settings.settings["currentTime"].Value, false, time);

                toAdd.Add(new Note(x, y, ms));
            }

            UndoRedoManager.Add("ADD PATTERN", () =>
            {
                foreach (var note in toAdd)
                    Notes.Remove(note);

                SortNotes();
            }, () =>
            {
                Notes.AddRange(toAdd);

                SortNotes();
            });
        }




        public bool PromptImport(string id, bool create = false)
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Select Audio File",
                Filter = "Audio Files (*.mp3;*.ogg;*.wav;*.flac;*.egg;*.m4a;*.asset)|*.mp3;*.ogg;*.wav;*.flac;*.egg;*.m4a;*.asset"
            };

            if (Settings.settings["audioPath"] != "")
                dialog.InitialDirectory = Settings.settings["audioPath"];

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.settings["audioPath"] = Path.GetDirectoryName(dialog.FileName) ?? "";
                if (string.IsNullOrWhiteSpace(id))
                    id = Path.GetFileNameWithoutExtension(dialog.FileName);

                if (dialog.FileName != $"{Directory.GetCurrentDirectory()}\\cached\\{id}.asset")
                    File.Copy(dialog.FileName, $"cached/{id}.asset", true);

                MusicPlayer.Load($"cached/{id}.asset");
                if (create)
                    SoundID = id;

                return true;
            }

            return false;
        }

        public bool LoadAudio(string id)
        {
            try
            {
                if (!Directory.Exists("cached/"))
                    Directory.CreateDirectory("cached/");

                if (!File.Exists($"cached/{id}.asset"))
                {
                    if (Settings.settings["skipDownload"])
                    {
                        var message = MessageBox.Show($"No asset with id '{id}' is present in cache.\n\nWould you like to import a file with this id?", "Warning", "OK", "Cancel");

                        return message == DialogResult.OK && PromptImport(id);
                    }
                    else
                        WebClient.DownloadFile($"https://assetdelivery.roblox.com/v1/asset/?id={id}", $"cached/{id}.asset", true);
                }

                MusicPlayer.Load($"cached/{id}.asset");

                return true;
            }
            catch (Exception e)
            {
                var message = MessageBox.Show($"Failed to download asset with id '{id}':\n\n{e.Message}\n\nWould you like to import a file with this id instead?", "Error", "OK", "Cancel");

                if (message == DialogResult.OK)
                    return PromptImport(id);
            }

            return false;
        }

        public string ParseData(bool copy = false, bool applyOffset = true)
        {
            var offset = (long)Settings.settings["exportOffset"];

            var final = new string[Notes.Count + 1];
            final[0] = SoundID.ToString();

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            for (int i = 0; i < Notes.Count; i++)
            {
                var note = Notes[i];
                var clone = copy ? new Note(MathHelper.Clamp(note.X, -0.85f, 2.85f), MathHelper.Clamp(note.Y, -0.85f, 2.85f), (long)MathHelper.Clamp(note.Ms, 0, Settings.settings["currentTime"].Max)) : note;
                if (applyOffset)
                    clone.Ms += offset;

                final[i + 1] = clone.ToString(culture);
            }

            return string.Join("", final);
        }

        public string ParseProperties()
        {
            var timingfinal = new JsonArray();

            foreach (var point in TimingPoints)
                timingfinal.Add(new JsonArray() { point.BPM, point.Ms });

            var bookmarkfinal = new JsonArray();

            foreach (var bookmark in Bookmarks)
                bookmarkfinal.Add(new JsonArray() { bookmark.Text, bookmark.Ms, bookmark.EndMs });

            var json = new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>())
            {
                {"timings", timingfinal },
                {"bookmarks", bookmarkfinal },
                {"currentTime", Settings.settings["currentTime"].Value },
                {"beatDivisor", Settings.settings["beatDivisor"].Value },
                {"exportOffset", Settings.settings["exportOffset"] },

                {"mappers", Settings.settings["mappers"] },
                {"songName", Settings.settings["songName"] },
                {"difficulty", Settings.settings["difficulty"] },
                {"useCover", Settings.settings["useCover"] },
                {"cover", Settings.settings["cover"] },
                {"customDifficulty", Settings.settings["customDifficulty"] }
            };

            return json.ToString();
        }

        public bool IsSaved()
        {
            return FileName != null && File.ReadAllText(FileName) == ParseData();
        }

        public bool SaveMap(bool forced, bool fileForced = false)
        {
            if (FileName != null && !File.Exists(FileName))
                FileName = null;

            if (FileName != null)
                Settings.settings["lastFile"] = FileName;
            Settings.Save();

            var data = ParseData();

            if (forced || (FileName == null && (Notes.Count > 0 || TimingPoints.Count > 0)) || (FileName != null && data != File.ReadAllText(FileName)))
            {
                var result = DialogResult.No;

                if (!forced)
                    result = MessageBox.Show($"{Path.GetFileNameWithoutExtension(FileName) ?? "Untitled Song"} ({SoundID})\n\nWould you like to save before closing?", "Warning", "Yes", "No", "Cancel");

                if (forced || result == DialogResult.Yes)
                {
                    if (FileName == null || fileForced)
                    {
                        var dialog = new SaveFileDialog()
                        {
                            Title = "Save Map As",
                            Filter = "Text Documents(*.txt)|*.txt"
                        };

                        if (Settings.settings["defaultPath"] != "")
                            dialog.InitialDirectory = Settings.settings["defaultPath"];

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            Settings.settings["defaultPath"] = Path.GetDirectoryName(dialog.FileName) ?? "";

                            File.WriteAllText(dialog.FileName, data);
                            SaveProperties(dialog.FileName);
                            FileName = dialog.FileName;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        File.WriteAllText(FileName, data);
                        SaveProperties(FileName);
                    }
                }
                else if (result == DialogResult.Cancel)
                    return false;
            }

            return true;
        }

        public void SaveProperties(string filePath)
        {
            var file = Path.ChangeExtension(filePath, ".ini");

            File.WriteAllText(file, ParseProperties());
            Settings.settings["lastFile"] = filePath;
        }

        public bool LoadMap(string pathOrData, bool file = false, bool autosave = false)
        {
            CurrentMap?.Save();
            
            foreach (Map map in Maps)
            {
                if (file && pathOrData == map.RawFileName && map.IsSaved())
                {
                    map.MakeCurrent();
                    SwitchWindow(new GuiWindowEditor());

                    return true;
                }
            }
            
            CurrentMap = new Map();

            SoundID = "-1";
            FileName = file ? pathOrData : null;

            Settings.settings["mappers"] = "";
            Settings.settings["songName"] = Path.GetFileNameWithoutExtension(FileName) ?? "Untitled Song";
            Settings.settings["difficulty"] = "N/A";
            Settings.settings["useCover"] = true;
            Settings.settings["cover"] = "Default";
            Settings.settings["customDifficulty"] = "";

            Notes.Clear();
            SelectedNotes.Clear();
            UpdateSelection();
            SelectedPoint = null;

            TimingPoints.Clear();
            Bookmarks.Clear();

            UndoRedoManager.Clear();
            MusicPlayer.Reset();

            if (file && Path.GetExtension(pathOrData) == ".sspm")
            {
                pathOrData = RunSSPMImport(pathOrData);
                file = false;
                FileName = null;
            }
            if (pathOrData == "")
                return false;

            var data = file ? File.ReadAllText(pathOrData) : pathOrData;

            try
            {
                while (true)
                    data = WebClient.DownloadString(data);
            }
            catch { }

            var split = data.Split(',');

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            try
            {
                var id = split[0];

                for (int i = 1; i < split.Length; i++)
                    if (!string.IsNullOrWhiteSpace(split[i]))
                        Notes.Add(new(split[i], culture));

                SortNotes();

                if (LoadAudio(id))
                {
                    SoundID = id;

                    Settings.settings["currentTime"] = new SliderSetting(0f, (float)MusicPlayer.TotalTime.TotalMilliseconds, (float)MusicPlayer.TotalTime.TotalMilliseconds / 2000f);
                    Settings.settings["beatDivisor"].Value = 3f;
                    Settings.settings["tempo"].Value = 0.9f;
                    Settings.settings["exportOffset"] = 0;

                    Tempo = 1f;
                    Zoom = 1f;

                    if (file)
                    {
                        var propertyFile = Path.ChangeExtension(FileName, ".ini");

                        if (File.Exists(propertyFile))
                            LoadProperties(File.ReadAllText(propertyFile));
                    }
                    else if (autosave)
                        LoadProperties(Settings.settings["autosavedProperties"]);

                    SortTimings();
                    SortBookmarks();

                    CurrentMap.Save();
                    CurrentMap.MakeCurrent();

                    Maps.Add(CurrentMap);
                    CacheMaps();

                    SwitchWindow(new GuiWindowEditor());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load map data", "Warning", "OK");
                ActionLogging.Register($"Failed to load map data - {ex.Message}\n\n{ex.StackTrace}\n\n", "WARN");
                Console.WriteLine(ex);

                return false;
            }

            return SoundID != "-1";
        }

        public void CacheMaps()
        {
            string[] data = new string[Maps.Count];

            for (int i = 0; i < Maps.Count; i++)
                data[i] = Maps[i].ToString();

            string text = string.Join("\r\0", data);

            if (!Directory.Exists("assets/temp"))
                Directory.CreateDirectory("assets/temp");
            File.WriteAllText(cacheFile, text);
        }

        public void LoadCache()
        {
            Maps.Clear();
            string[] data = File.ReadAllText(cacheFile).Split("\r\0");

            for (int i = 0; i < data.Length; i++)
            {
                Map map = new();

                if (map.FromString(data[i]))
                    Maps.Add(map);
            }
        }

        public void LoadLegacyProperties(string text)
        {
            var lines = text.Split('\n');
            var oldVer = false; // pre 1.7

            foreach (var line in lines)
            {
                var split = line.Split('=');

                switch (split[0])
                {
                    case "BPM":
                        var points = split[1].Split(',');

                        foreach (var point in points)
                        {
                            var pointsplit = point.Split('|');
                            if (pointsplit.Length == 1)
                            {
                                pointsplit = new string[] { pointsplit[0], "0" };
                                oldVer = true;
                            }

                            if (pointsplit.Length == 2 && float.TryParse(pointsplit[0], out var bpm) && long.TryParse(pointsplit[1], out var ms))
                                TimingPoints.Add(new TimingPoint(bpm, ms));
                        }

                        SortTimings();

                        break;

                    case "Bookmarks":
                        var bookmarks = split[1].Split(',');

                        foreach (var bookmark in bookmarks)
                        {
                            var bookmarksplit = bookmark.Split('|');

                            if (bookmarksplit.Length == 2 && long.TryParse(bookmarksplit[1], out var ms))
                                Bookmarks.Add(new Bookmark(bookmarksplit[0], ms, ms));
                            else if (bookmarksplit.Length == 3 && long.TryParse(bookmarksplit[1], out var startMs) && long.TryParse(bookmarksplit[2], out var endMs))
                                Bookmarks.Add(new Bookmark(bookmarksplit[0], startMs, endMs));
                        }

                        SortBookmarks();

                        break;

                    case "Offset":
                        if (oldVer) // back when timing points didnt exist and the offset meant bpm/note offset
                        {
                            if (TimingPoints.Count > 0 && long.TryParse(split[1], out var bpmOffset))
                                TimingPoints[0].Ms = bpmOffset;
                        }
                        else
                        {
                            foreach (var note in Notes)
                                note.Ms += (long)Settings.settings["exportOffset"];

                            if (long.TryParse(split[1], out var offset))
                                Settings.settings["exportOffset"] = offset;

                            foreach (var note in Notes)
                                note.Ms -= (long)Settings.settings["exportOffset"];
                        }

                        break;

                    case "Time":
                        if (long.TryParse(split[1], out var time))
                            Settings.settings["currentTime"].Value = time;

                        break;

                    case "Divisor":
                        if (float.TryParse(split[1], out var divisor))
                            Settings.settings["beatDivisor"].Value = divisor - 1f;

                        break;
                }
            }
        }

        public void LoadProperties(string text)
        {
            try
            {
                var result = (JsonObject)JsonValue.Parse(text);

                foreach (var key in result)
                {
                    switch (key.Key)
                    {
                        case "timings":
                            foreach (JsonArray timing in key.Value)
                                TimingPoints.Add(new TimingPoint(timing[0], timing[1]));

                            break;

                        case "bookmarks":
                            foreach (JsonArray bookmark in key.Value)
                                Bookmarks.Add(new Bookmark(bookmark[0], bookmark[1], bookmark[^1]));

                            break;

                        case "currentTime":
                            Settings.settings["currentTime"].Value = key.Value;

                            break;

                        case "beatDivisor":
                            Settings.settings["beatDivisor"].Value = key.Value;

                            break;

                        case "exportOffset":
                            foreach (var note in Notes)
                                note.Ms += (long)Settings.settings["exportOffset"];

                            Settings.settings["exportOffset"] = key.Value;

                            foreach (var note in Notes)
                                note.Ms -= (long)Settings.settings["exportOffset"];

                            break;

                        case "mappers":
                            Settings.settings["mappers"] = key.Value;

                            break;

                        case "songName":
                            Settings.settings["songName"] = key.Value;

                            break;

                        case "difficulty":
                            Settings.settings["difficulty"] = key.Value;

                            break;

                        case "useCover":
                            Settings.settings["useCover"] = key.Value;

                            break;

                        case "cover":
                            Settings.settings["cover"] = key.Value;

                            break;

                        case "customDifficulty":
                            Settings.settings["customDifficulty"] = key.Value;

                            break;
                    }
                }
            }
            catch
            {
                try
                {
                    LoadLegacyProperties(text);
                }
                catch { }
            }
        }

        public void ImportProperties()
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Select .ini File",
                Filter = "Map Property Files (*.ini)|*.ini"
            };

            if (Settings.settings["defaultPath"] != "")
                dialog.InitialDirectory = Settings.settings["defaultPath"];

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TimingPoints.Clear();
                Bookmarks.Clear();

                Settings.settings["defaultPath"] = Path.GetDirectoryName(dialog.FileName) ?? "";

                LoadProperties(File.ReadAllText(dialog.FileName));

                if (CurrentWindow is GuiWindowEditor editor)
                    editor.Timeline.GenerateOffsets();
            }
        }

        public void CopyBookmarks()
        {
            string[] data = new string[Bookmarks.Count];

            for (int i = 0; i < Bookmarks.Count; i++)
            {
                var bookmark = Bookmarks[i];

                if (bookmark.Ms != bookmark.EndMs)
                    data[i] = $"{bookmark.Ms}-{bookmark.EndMs} ~ {bookmark.Text}";
                else
                    data[i] = $"{bookmark.Ms} ~ {bookmark.Text}";
            }

            if (data.Length == 0)
                return;

            Clipboard.SetText(string.Join("\n", data));

            if (CurrentWindow is GuiWindowEditor editor)
                editor.ShowToast("COPIED TO CLIPBOARD", Color.FromArgb(0, 255, 200));
        }

        public void PasteBookmarks()
        {
            var data = Clipboard.GetText();
            string[] bookmarks = data.Split('\n');

            List<Bookmark> tempBookmarks = new();

            for (int i = 0; i < bookmarks.Length; i++)
            {
                var split = bookmarks[i].Split(" ~ ");
                if (split.Length != 2)
                    continue;

                var subsplit = split[0].Split("-");

                if (subsplit.Length == 1 && long.TryParse(subsplit[0], out var ms))
                    tempBookmarks.Add(new Bookmark(split[1], ms, ms));
                else if (subsplit.Length == 2 && long.TryParse(subsplit[0], out var startMs) && long.TryParse(subsplit[1], out var endMs))
                    tempBookmarks.Add(new Bookmark(split[1], startMs, endMs));
            }

            if (tempBookmarks.Count > 0)
                Bookmarks = tempBookmarks.ToList();
        }

        private int currentAutosave;

        private void RunAutosave(int time)
        {
            currentAutosave = time;

            var delay = Task.Delay((int)(Settings.settings["autosaveInterval"] * 60000f)).ContinueWith(_ =>
            {
                if (currentAutosave == time)
                {
                    RunAutosave(time);
                    AttemptAutosave();
                }
            });
        }

        private void AttemptAutosave()
        {
            if (CurrentWindow is GuiWindowEditor editor && Notes.Count > 0)
            {
                if (FileName == null)
                {
                    Settings.settings["autosavedFile"] = ParseData();
                    Settings.settings["autosavedProperties"] = ParseProperties();
                    Settings.Save();

                    editor.ShowToast("AUTOSAVED", Settings.settings["color1"]);
                }
                else if (SaveMap(true))
                    editor.ShowToast("AUTOSAVED", Settings.settings["color1"]);
            }
        }




        public void RunBezier()
        {
            if (CurrentWindow is GuiWindowEditor editor)
            {
                var divisor = (int)((float)Settings.settings["bezierDivisor"] + 0.5f);

                if (divisor > 0 && ((BezierNodes != null && BezierNodes.Count > 1) || SelectedNotes.Count > 1))
                {
                    var success = true;
                    var finalNodes = BezierNodes != null && BezierNodes.Count > 1 ? BezierNodes.ToList() : SelectedNotes.ToList();
                    var finalNotes = new List<Note>();

                    var anchored = new List<int>() { 0 };

                    for (int i = 0; i < finalNodes.Count; i++)
                        if (finalNodes[i].Anchored && !anchored.Contains(i))
                            anchored.Add(i);

                    if (!anchored.Contains(finalNodes.Count - 1))
                        anchored.Add(finalNodes.Count - 1);

                    for (int i = 1; i < anchored.Count; i++)
                    {
                        var newNodes = new List<Note>();

                        for (int j = anchored[i - 1]; j <= anchored[i]; j++)
                            newNodes.Add(finalNodes[j]);

                        var finalbez = Bezier(newNodes, divisor);
                        success = finalbez.Count > 0;

                        if (success)
                            foreach (var note in finalbez)
                                finalNotes.Add(note);
                    }

                    SelectedNotes.Clear();
                    UpdateSelection();
                    SelectedPoint = null;

                    finalNotes.Add(finalNodes[0]);

                    if (success)
                    {
                        UndoRedoManager.Add("DRAW BEZIER", () =>
                        {
                            foreach (var note in finalNotes)
                                Notes.Remove(note);
                            Notes.AddRange(finalNodes);

                            SortNotes();
                        }, () =>
                        {
                            foreach (var note in finalNodes)
                                Notes.Remove(note);
                            Notes.AddRange(finalNotes);

                            SortNotes();
                        });
                    }
                }

                foreach (var note in Notes)
                    note.Anchored = false;

                BezierNodes?.Clear();
            }
        }

        public static BigInteger FactorialApprox(int k)
        {
            var result = new BigInteger(1);

            if (k < 10)
                for (int i = 1; i <= k; i++)
                    result *= i;
            else
                result = (BigInteger)(Math.Sqrt(2 * Math.PI * k) * Math.Pow(k / Math.E, k));

            return result;
        }

        public static BigInteger BinomialCoefficient(int k, int v)
        {
            return FactorialApprox(k) / (FactorialApprox(v) * FactorialApprox(k - v));
        }

        public List<Note> Bezier(List<Note> finalNodes, int divisor)
        {
            var finalNotes = new List<Note>();

            if (CurrentWindow is GuiWindowEditor editor)
            {
                try
                {
                    var k = finalNodes.Count - 1;
                    decimal tdiff = finalNodes[k].Ms - finalNodes[0].Ms;
                    decimal d = 1m / (divisor * k);

                    if (Settings.settings["curveBezier"])
                    {
                        for (decimal t = d; t <= 1 + d / 2m; t += d)
                        {
                            float xf = 0;
                            float yf = 0;
                            decimal tf = finalNodes[0].Ms + tdiff * t;

                            for (int v = 0; v <= k; v++)
                            {
                                var note = finalNodes[v];
                                var bez = (double)BinomialCoefficient(k, v) * (Math.Pow(1 - (double)t, k - v) * Math.Pow((double)t, v));

                                xf += (float)(bez * note.X);
                                yf += (float)(bez * note.Y);
                            }

                            finalNotes.Add(new Note(xf, yf, (long)tf));
                        }
                    }
                    else
                    {
                        d = 1m / divisor;

                        for (int v = 0; v < k; v++)
                        {
                            var note = finalNodes[v];
                            var nextnote = finalNodes[v + 1];
                            decimal xdist = (decimal)(nextnote.X - note.X);
                            decimal ydist = (decimal)(nextnote.Y - note.Y);
                            decimal tdist = nextnote.Ms - note.Ms;

                            for (decimal t = 0; t < 1 + d / 2m; t += d)
                            {
                                if (t > 0)
                                {
                                    var xf = (decimal)note.X + xdist * t;
                                    var yf = (decimal)note.Y + ydist * t;
                                    var tf = note.Ms + tdist * t;

                                    finalNotes.Add(new Note((float)xf, (float)yf, (long)tf));
                                }
                            }
                        }
                    }
                }
                catch (OverflowException)
                {
                    editor.ShowToast("TOO MANY NODES", Color.FromArgb(255, 255, 200, 0));
                    return new List<Note>();
                }
                catch
                {
                    editor.ShowToast("FAILED TO DRAW CURVE", Color.FromArgb(255, 255, 200, 0));
                    return new List<Note>();
                }
            }

            return finalNotes;
        }




        private bool FocusingBox()
        {
            foreach (var control in CurrentWindow.Controls)
                if (control is GuiTextbox box && box.Focused)
                    return true;

            return false;
        }

        public void SetTempo(float newTempo)
        {
            var tempoA = Math.Min(newTempo, 0.9f);
            var tempoB = (newTempo - tempoA) * 2f;

            Tempo = tempoA + tempoB + 0.1f;
            MusicPlayer.Tempo = Tempo;
        }




        public void SortNotes()
        {
            Notes = new List<Note>(Notes.OrderBy(n => n.Ms));

            if (CurrentWindow is GuiWindowEditor editor)
                editor.Timeline.GenerateOffsets();
        }

        public void SortTimings(bool updateList = true)
        {
            TimingPoints = new List<TimingPoint>(TimingPoints.OrderBy(n => n.Ms));

            if (updateList)
                TimingsWindow.Instance?.ResetList();

            if (CurrentWindow is GuiWindowEditor editor)
                editor.Timeline.GenerateOffsets();
        }

        public void SortBookmarks(bool updateList = true)
        {
            Bookmarks = new List<Bookmark>(Bookmarks.OrderBy(n => n.Ms));

            if (updateList)
                BookmarksWindow.Instance?.ResetList();

            if (CurrentWindow is GuiWindowEditor editor)
                editor.Timeline.GenerateOffsets();
        }




        public void SwitchWindow(GuiWindow window)
        {
            if (CurrentWindow is GuiWindowEditor)
                CurrentMap.Save();

            if (window is GuiWindowEditor)
            {
                SetActivity("Editing a map");
                RunAutosave(DateTime.Now.Millisecond);
            }
            else if (window is GuiWindowMenu)
                SetActivity("Sitting in the menu");

            ExportSSPM.Instance?.Close();
            BPMTapper.Instance?.Close();
            TimingsWindow.Instance?.Close();
            BookmarksWindow.Instance?.Close();

            CurrentWindow?.Dispose();
            CurrentWindow = window;

            Settings.Save();
        }




        public static void CheckForUpdates()
        {
            if (!Settings.settings["checkUpdates"])
                return;

            var versionInfo = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule?.FileName ?? "");
            var currentVersion = versionInfo.FileVersion;

            try
            {
                var playerVersion = WebClient.DownloadString("https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/player_version").Replace("\n", "");

                // player exists check
                ActionLogging.Register("Searching for file 'SSQE Player'");
                if (!File.Exists("SSQE Player.exe"))
                {
                    var diag = MessageBox.Show("Map player is not present in this directory. Would you like to download it?", "Warning", "Yes", "No");

                    if (diag == DialogResult.Yes)
                    {
                        ActionLogging.Register("Attempting to download file 'SSQE Player.exe'");
                        WebClient.DownloadFile("https://github.com/David20122/Sound-Space-Quantum-Editor/raw/2.0%2B_rewrite/SSQE%20Player.zip", "SSQE Player.zip");
                        ExtractFile("SSQE Player.zip");
                    }
                }

                // player version check
                ActionLogging.Register("Checking version of file 'SSQE Player'");
                if (File.Exists("SSQE Player.exe"))
                {
                    var currentPlayerVersion = FileVersionInfo.GetVersionInfo("SSQE Player.exe").FileVersion;

                    if (currentPlayerVersion != playerVersion)
                    {
                        var diag = MessageBox.Show($"New Player version is available ({playerVersion}). Would you like to download the new version?", "Warning", "Yes", "No");

                        if (diag == DialogResult.Yes)
                        {
                            ActionLogging.Register("Attempting to download file 'SSQE Player'");
                            WebClient.DownloadFile("https://github.com/David20122/Sound-Space-Quantum-Editor/raw/2.0%2B_rewrite/SSQE%20Player.zip", "SSQE Player.zip");
                            ExtractFile("SSQE Player.zip");
                        }
                    }
                }

                var redirect = WebClient.GetRedirect("https://github.com/David20122/Sound-Space-Quantum-Editor/releases/latest");

                if (redirect != "")
                {
                    var rep = redirect.LastIndexOf("/") + 1;
                    var version = redirect[rep..];

                    var updaterVersion = WebClient.DownloadString("https://raw.githubusercontent.com/David20122/Sound-Space-Quantum-Editor/2.0%2B_rewrite/updater_version").Replace("\n", "");

                    // updater exists check
                    ActionLogging.Register("Searching for file 'SSQE Updater'");
                    if (!File.Exists("SSQE Updater.exe"))
                    {
                        var diag = MessageBox.Show("Auto updater is not present in this directory. Would you like to download it?", "Warning", "Yes", "No");

                        if (diag == DialogResult.Yes)
                        {
                            ActionLogging.Register("Attempting to download file 'SSQE Updater'");
                            WebClient.DownloadFile("https://github.com/David20122/Sound-Space-Quantum-Editor/raw/2.0%2B_rewrite/SSQE%20Updater.zip", "SSQE Updater.zip");
                            ExtractFile("SSQE Updater.zip");
                        }
                    }

                    // run updater
                    ActionLogging.Register("Checking version of file 'SSQE Updater'");
                    if (File.Exists("SSQE Updater.exe"))
                    {
                        var currentUpdaterVersion = FileVersionInfo.GetVersionInfo("SSQE Updater.exe").FileVersion;

                        // updater version check
                        if (currentUpdaterVersion != updaterVersion)
                        {
                            var diag = MessageBox.Show($"New Updater version is available ({updaterVersion}). Would you like to download the new version?", "Warning", "Yes", "No");

                            if (diag == DialogResult.Yes)
                            {
                                ActionLogging.Register("Attempting to download file 'SSQE Updater'");
                                WebClient.DownloadFile("https://github.com/David20122/Sound-Space-Quantum-Editor/raw/2.0%2B_rewrite/SSQE%20Updater.zip", "SSQE Updater.zip");
                                ExtractFile("SSQE Updater.zip");
                            }
                        }

                        // editor version check
                        ActionLogging.Register("Checking version of editor");
                        if (version != currentVersion)
                        {
                            var diag = MessageBox.Show($"New Editor version is available ({version}). Would you like to download the new version?", "Warning", "Yes", "No");

                            if (diag == DialogResult.Yes)
                            {
                                ActionLogging.Register("Attempting to run updater");
                                Process.Start("SSQE Updater.exe");
                            }
                        }
                    }
                }

                static void ExtractFile(string path)
                {
                    using (ZipArchive archive = ZipFile.OpenRead(path))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            try
                            {
                                entry.ExtractToFile(entry.FullName, true);
                            }
                            catch { }
                        }
                    }

                    File.Delete(path);
                }
            }
            catch
            {
                MessageBox.Show("Failed to check for updates", "Warning", "OK");
            }
        }

        private void DiscordInit()
        {
            if (!discordEnabled)
                return;

            try
            {
                discord = new Discord.Discord(1067849747710345346, (ulong)CreateFlags.NoRequireDiscord);
                activityManager = discord.GetActivityManager();
            }
            catch { discordEnabled = false; }
        }

        public void SetActivity(string status)
        {
            if (!discordEnabled)
                return;

            var activity = new Activity
            {
                State = status,
                Details = $"Version {Assembly.GetExecutingAssembly().GetName().Version}",
                Timestamps = { Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                Assets = { LargeImage = "logo" },
                Instance = true,
            };

            activityManager.UpdateActivity(activity, (result) =>
            {
                Console.WriteLine($"{(result == Result.Ok ? "Activity success" : "Activity failed")}");

                if (result != Result.Ok)
                    ActionLogging.Register($"Failed to update Discord Rich Presence activity - {status}", "WARN");
            });
        }




        private static Vector2 VecFromNote(Note note)
        {
            var x = Math.Round(note.X, 2);
            var y = Math.Round(note.Y, 2);

            return ((float)x, (float)y);
        }

        public float CheckClones(int threshold)
        {
            List<Vector2> locations = new();
            int score = 0;

            for (int i = 0; i < Notes.Count; i++)
            {
                var note = Notes[i];
                if (!locations.Contains(VecFromNote(note)))
                    locations.Add(VecFromNote(note));
            }

            for (int i = 0; i < locations.Count; i++)
            {
                Vector2 currentLocation = locations[i];
                List<int> indexes = new();

                for (int j = 0; j < Notes.Count; j++)
                {
                    var note = Notes[j];
                    Vector2 location = VecFromNote(note);

                    if (location == currentLocation)
                        indexes.Add(j);
                }

                for (int j = 0; j < indexes.Count; j++)
                {
                    List<int> tempIndexes = new(indexes);
                    tempIndexes.RemoveAt(j);

                    int index = indexes[j];
                    int offset = 0;

                    while (tempIndexes.Count > 0)
                    {
                        offset++;

                        if (index + offset >= Notes.Count)
                            break;

                        var compareNote = Notes[index + offset];
                        currentLocation = VecFromNote(compareNote);

                        for (int k = 0; k < tempIndexes.Count; k++)
                        {
                            int checkIndex = tempIndexes[k];

                            if (checkIndex + offset >= Notes.Count || index >= checkIndex)
                            {
                                tempIndexes.RemoveAt(k);
                                k--;
                                continue;
                            }

                            if (k + 1 < tempIndexes.Count && checkIndex + offset >= tempIndexes[k + 1])
                            {
                                tempIndexes.RemoveAt(k + 1);
                                k--;
                                continue;
                            }

                            var checkNote = Notes[checkIndex + offset];
                            Vector2 checkLocation = VecFromNote(checkNote);

                            if (currentLocation != checkLocation)
                            {
                                tempIndexes.RemoveAt(k);
                                k--;
                                continue;
                            }

                            if (offset >= threshold)
                                score++;
                        }
                    }
                }
            }

            return (float)score / Notes.Count;
        }




        public Dictionary<string, string> info = new()
        {
            {"songId", "" },
            {"mapName", "" },
            {"mappers", "" },
            {"coverPath", "" },
            {"difficulty", "" },
            {"customDifficulty", "" },
        };

        public static Dictionary<string, byte> difficulties = new()
        {
            {"N/A", 0x00 },
            {"Easy", 0x01 },
            {"Medium", 0x02 },
            {"Hard", 0x03 },
            {"Logic", 0x04 },
            {"Tasukete", 0x05 },
        };
        public void RunSSPMExport()
        {
            if (CurrentWindow is GuiWindowEditor editor)
            {
                if (!Settings.settings["exportWarningShown"])
                {
                    MessageBox.Show("It's recommended to put this file directly into your SS+ maps folder or have it open so it's easier to import or replace later!", "Warning", "OK");
                    Settings.settings["exportWarningShown"] = true;
                }

                var dialog = new SaveFileDialog()
                {
                    Title = "Export SSPM",
                    Filter = "Sound Space Plus Maps (*.sspm)|*.sspm"
                };

                if (Settings.settings["exportPath"] != "")
                    dialog.InitialDirectory = Settings.settings["exportPath"];

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Settings.settings["exportPath"] = Path.GetDirectoryName(dialog.FileName) ?? "";

                    try
                    {
                        var data = ParseSSPM();
                        File.WriteAllBytes(dialog.FileName, data);

                        editor.ShowToast("SUCCESSFULLY EXPORTED", Settings.settings["color1"]);
                    }
                    catch
                    {
                        editor.ShowToast("FAILED TO EXPORT", Settings.settings["color1"]);
                    }
                }
            }
        }

        private byte[] ParseSSPM()
        {
            var header = new byte[]
            {
                0x53, 0x53, 0x2b, 0x6d, // file type signature
                0x02, 0x00, // SSPM format version - 2
                0x00, 0x00, 0x00, 0x00, // reserved space
            };

            var hasCover = info["coverPath"] != ""; // whether cover should be present
            var lastMs = BitConverter.GetBytes((uint)Notes.Last().Ms); // last note ms - 4 bytes

            var noteCount = BitConverter.GetBytes((uint)Notes.Count); // note count - 4 bytes
            var markerCount = (byte[])noteCount.Clone(); // marker count, repeated from last since no other markers

            var metadata = new byte[]
            {
                lastMs[0], lastMs[1], lastMs[2], lastMs[3],
                noteCount[0], noteCount[1], noteCount[2], noteCount[3],
                markerCount[0], markerCount[1], markerCount[2], markerCount[3],

                difficulties[info["difficulty"]],
                0x00, 0x00, // rating? whatever that means
                0x01, // contains audio
                (byte)(hasCover ? 0x01 : 0x00), // may contain cover
                0x00, // does not require a mod
            };

            var songID = Encoding.ASCII.GetBytes(info["songId"]); // song ID from form
            var songIDf = BitConverter.GetBytes((ushort)songID.Length).Concat(songID).ToArray(); // song ID array with length
            var mapName = Encoding.ASCII.GetBytes(info["mapName"]); // map name from form
            var mapNamef = BitConverter.GetBytes((ushort)mapName.Length).Concat(mapName).ToArray(); // map name array with length
            var songNamef = mapNamef.ToArray(); // song name copied from map name

            var mappers = info["mappers"].Split('\n'); // list of provided valid mappers or "None" if empty
            var mapperCount = BitConverter.GetBytes((ushort)mappers.Length); // number of provided valid mappers
            var mappersf = new List<byte>(); // final list of mappers as bytes

            foreach (var mapper in mappers)
            {
                var mapperf = Encoding.ASCII.GetBytes(mapper);
                var mapperFinal = BitConverter.GetBytes((ushort)mapperf.Length).Concat(mapperf).ToArray(); // mapper array with length

                mappersf.AddRange(mapperFinal);
            }

            var strings = songIDf.Concat(mapNamef).Concat(songNamef).Concat(mapperCount).Concat(mappersf).ToArray(); // merged string data

            var offset = header.Length + 20 + metadata.Length + 80 + strings.Length; // for pointers

            var customData = new byte[] { 0x00, 0x00 }; // default custom data - none

            if (!string.IsNullOrWhiteSpace(info["customDifficulty"]))
            {
                var field = Encoding.ASCII.GetBytes("difficulty_name");
                var fieldf = BitConverter.GetBytes((ushort)field.Length).Concat(field).ToArray();

                var customDifficulty = Encoding.ASCII.GetBytes(info["customDifficulty"]);
                var customDifficultyf = BitConverter.GetBytes((ushort)customDifficulty.Length).Concat(customDifficulty).ToArray();

                customData = (new byte[] { 0x01, 0x00 }).Concat(fieldf).Concat(new byte[] { 0x09 }).Concat(customDifficultyf).ToArray();
            }

            var customDataOffset = BitConverter.GetBytes((ulong)offset);
            var customDataLength = BitConverter.GetBytes((ulong)customData.Length);
            offset += customData.Length;

            var audio = File.ReadAllBytes($"cached/{SoundID}.asset"); // audio in bytes
            var audioOffset = BitConverter.GetBytes((ulong)offset);
            var audioLength = BitConverter.GetBytes((ulong)audio.Length);
            offset += audio.Length;

            var path = info["coverPath"];
            var cover = hasCover ? (path == "Default" || !File.Exists(path) ? File.ReadAllBytes("assets/textures/Cover.png") : File.ReadAllBytes(path)) : Array.Empty<byte>(); // cover in bytes
            var coverOffset = BitConverter.GetBytes((ulong)(hasCover ? offset : 0));
            var coverLength = BitConverter.GetBytes((ulong)cover.Length);
            offset += cover.Length;

            var noteDefinition = Encoding.ASCII.GetBytes("ssp_note");
            var noteDefinitionf = BitConverter.GetBytes((ushort)noteDefinition.Length).Concat(noteDefinition).ToArray();
            var markerDefStart = new byte[] { 0x01 /* one definition */ };
            var markerDefEnd = new byte[] { 0x01, /* one value */ 0x07, /* data type 07 - note */ 0x00 /* end of definition */ };

            var markerDefinitions = markerDefStart.Concat(noteDefinitionf).Concat(markerDefEnd).ToArray(); // defines how to process notes
            var markerDefinitionsOffset = BitConverter.GetBytes((ulong)offset);
            var markerDefinitionsLength = BitConverter.GetBytes((ulong)markerDefinitions.Length);
            offset += markerDefinitions.Length;

            var markers = new List<byte>(); // note data
            var exportOffset = (long)Settings.settings["exportOffset"];
            foreach (var note in Notes)
            {
                var ms = BitConverter.GetBytes((uint)(note.Ms + exportOffset));
                var markerType = new byte[1];

                var xyInt = Math.Round(note.X) == Math.Round(note.X, 2) && Math.Round(note.Y) == Math.Round(note.Y, 2);
                var identifier = new byte[] { (byte)(xyInt ? 0x00 : 0x01) };

                var x = xyInt ? new byte[] { BitConverter.GetBytes((ushort)Math.Round(note.X))[0] } : BitConverter.GetBytes(note.X);
                var y = xyInt ? new byte[] { BitConverter.GetBytes((ushort)Math.Round(note.Y))[0] } : BitConverter.GetBytes(note.Y);

                var final = ms.Concat(markerType).Concat(identifier).Concat(x).Concat(y).ToArray();

                markers.AddRange(final);
            }
            var markerOffset = BitConverter.GetBytes((ulong)offset);
            var markerLength = BitConverter.GetBytes((ulong)markers.Count);

            var pointers = customDataOffset.Concat(customDataLength).Concat(audioOffset).Concat(audioLength).Concat(coverOffset).Concat(coverLength).ToArray(); // pointers
            pointers = pointers.Concat(markerDefinitionsOffset).Concat(markerDefinitionsLength).Concat(markerOffset).Concat(markerLength).ToArray();

            var markerSet = markerDefinitions.Concat(markers).ToArray();
            using SHA1 sHash = SHA1.Create();
            var hash = sHash.ComputeHash(markerSet);

            var data = new List<byte>(); // final converted data
            data.AddRange(header);
            data.AddRange(hash);
            data.AddRange(metadata);
            data.AddRange(pointers);
            data.AddRange(strings);
            data.AddRange(customData);
            data.AddRange(audio);
            data.AddRange(cover);
            data.AddRange(markerDefinitions);
            data.AddRange(markers);

            // man the documentation for this stuff really isnt great ngl, some examples would be nice
            // https://github.com/basils-garden/types/blob/main/sspm/v2.md

            return data.ToArray();
        }

        public static string RunSSPMImport(string path)
        {
            using (FileStream file = new(path, FileMode.Open, FileAccess.Read))
            {
                MemoryStream data = new();
                file.CopyTo(data);

                string? output = ProcessSSPM(data);
                data.Dispose();

                return output ?? "";
            }
        }

        private static string? ProcessSSPM(MemoryStream data)
        {
            data.Seek(0, SeekOrigin.Begin);
            string? mapData = null;

            byte[] fileTypeSignature = new byte[4];
            data.Read(fileTypeSignature, 0, 4);

            if (!(fileTypeSignature[0] == 0x53 &&
                fileTypeSignature[1] == 0x53 &&
                fileTypeSignature[2] == 0x2b &&
                fileTypeSignature[3] == 0x6d))
            {
                MessageBox.Show("File type not recognized or supported\nCurrently supported: SSPM v1/v2", "Warning", "OK");
                return null;
            }

            byte[] formatVersion = new byte[2];
            data.Read(formatVersion, 0, 2);

            if (formatVersion[0] == 0x01 && formatVersion[1] == 0x00)
            {
                string GetNextVariableString()
                {
                    List<byte> bytes = new();

                    byte[] currentByte = new byte[1];
                    data.Read(currentByte, 0, 1);

                    while (currentByte[0] != 0x0a)
                    {
                        bytes.Add(currentByte[0]);
                        data.Read(currentByte, 0, 1);
                    }

                    return Encoding.ASCII.GetString(bytes.ToArray());
                }


                // v1
                byte[] reservedSpace = new byte[2];
                data.Read(reservedSpace, 0, 2);

                // metadata
                string mapID = GetNextVariableString();
                string mapName = GetNextVariableString();
                string mappers = GetNextVariableString();

                Settings.settings["songName"] = mapName;
                Settings.settings["mappers"] = mappers;

                byte[] lastMs = new byte[4];
                byte[] noteCount = new byte[4];
                byte[] difficulty = new byte[1];

                // read metadata
                data.Read(lastMs, 0, 4);
                data.Read(noteCount, 0, 4);
                data.Read(difficulty, 0, 1);

                foreach (var key in difficulties)
                {
                    if (key.Value == difficulty[0])
                        Settings.settings["difficulty"] = key.Key;
                }

                // read cover
                byte[] containsCover = new byte[1];
                data.Read(containsCover, 0, 1);

                Settings.settings["useCover"] = containsCover[0] == 0x02;

                if (containsCover[0] == 0x02)
                {
                    byte[] coverLength = new byte[8];
                    data.Read(coverLength, 0, 8);

                    int coverLengthF = BitConverter.ToInt32(coverLength);
                    byte[] cover = new byte[coverLengthF];
                    data.Read(cover, 0, coverLengthF);

                    File.WriteAllBytes($"cached/{mapID}.png", cover);
                    Settings.settings["cover"] = $"cached/{mapID}.png";
                }

                // read audio
                byte[] containsAudio = new byte[1];
                data.Read(containsAudio, 0, 1);

                if (containsAudio[0] == 0x01)
                {
                    byte[] audioLength = new byte[8];
                    data.Read(audioLength, 0, 8);

                    int audioLengthF = BitConverter.ToInt32(audioLength);
                    byte[] audio = new byte[audioLengthF];
                    data.Read(audio, 0, audioLengthF);

                    File.WriteAllBytes($"cached/{mapID}.asset", audio);
                }

                mapData = mapID;

                // read notes
                uint noteCountF = BitConverter.ToUInt32(noteCount);
                for (int i = 0; i < noteCountF; i++)
                {
                    byte[] ms = new byte[4];
                    data.Read(ms, 0, 4);

                    byte[] isQuantum = new byte[1];
                    data.Read(isQuantum, 0, 1);

                    float xF;
                    float yF;

                    if (isQuantum[0] == 0x00)
                    {
                        byte[] x = new byte[1];
                        byte[] y = new byte[1];

                        data.Read(x, 0, 1);
                        data.Read(y, 0, 1);

                        xF = x[0];
                        yF = y[0];
                    }
                    else
                    {
                        byte[] x = new byte[4];
                        byte[] y = new byte[4];

                        data.Read(x, 0, 4);
                        data.Read(y, 0, 4);

                        xF = BitConverter.ToSingle(x);
                        yF = BitConverter.ToSingle(y);
                    }

                    uint msF = BitConverter.ToUInt32(ms);

                    mapData += $",{2 - xF}|{2 - yF}|{msF}";
                }
            }
            else if (formatVersion[0] == 0x02 && formatVersion[1] == 0x00)
            {
                string GetNextVariableString(bool fourBytes = false)
                {
                    byte[] length = new byte[2];
                    data.Read(length, 0, 2);
                    int lengthF = (int)(fourBytes ? BitConverter.ToUInt32(length) : BitConverter.ToUInt16(length));

                    byte[] str = new byte[lengthF];
                    data.Read(str, 0, lengthF);

                    return Encoding.ASCII.GetString(str);
                }


                // v2
                byte[] reservedSpace = new byte[4];
                data.Read(reservedSpace, 0, 4);

                // metadata
                byte[] hash = new byte[20];
                byte[] lastMs = new byte[4];
                byte[] noteCount = new byte[4];
                byte[] markerCount = new byte[4];

                byte[] difficulty = new byte[1];
                byte[] mapRating = new byte[2];
                byte[] containsAudio = new byte[1];
                byte[] containsCover = new byte[1];
                byte[] requiresMod = new byte[1];

                // pointers
                byte[] customDataOffset = new byte[8];
                byte[] customDataLength = new byte[8];
                byte[] audioOffset = new byte[8];
                byte[] audioLength = new byte[8];
                byte[] coverOffset = new byte[8];
                byte[] coverLength = new byte[8];
                byte[] markerDefinitionsOffset = new byte[8];
                byte[] markerDefinitionsLength = new byte[8];
                byte[] markerOffset = new byte[8];
                byte[] markerLength = new byte[8];

                // read metadata
                data.Read(hash, 0, 20);
                data.Read(lastMs, 0, 4);
                data.Read(noteCount, 0, 4);
                data.Read(markerCount, 0, 4);

                data.Read(difficulty, 0, 1);
                data.Read(mapRating, 0, 2);
                data.Read(containsAudio, 0, 1);
                data.Read(containsCover, 0, 1);
                data.Read(requiresMod, 0, 1);

                foreach (var key in difficulties)
                {
                    if (key.Value == difficulty[0])
                        Settings.settings["difficulty"] = key.Key;
                }

                // read pointers
                data.Read(customDataOffset, 0, 8);
                data.Read(customDataLength, 0, 8);
                data.Read(audioOffset, 0, 8);
                data.Read(audioLength, 0, 8);
                data.Read(coverOffset, 0, 8);
                data.Read(coverLength, 0, 8);
                data.Read(markerDefinitionsOffset, 0, 8);
                data.Read(markerDefinitionsLength, 0, 8);
                data.Read(markerOffset, 0, 8);
                data.Read(markerLength, 0, 8);

                // get song name stuff and mappers
                string mapID = GetNextVariableString();
                string mapName = GetNextVariableString();
                string songName = GetNextVariableString();

                Settings.settings["songName"] = mapName;

                byte[] mapperCount = new byte[2];
                data.Read(mapperCount, 0, 2);
                uint mapperCountF = BitConverter.ToUInt16(mapperCount);

                string[] mappers = new string[mapperCountF];

                for (int i = 0; i < mapperCountF; i++)
                    mappers[i] = GetNextVariableString();

                Settings.settings["mappers"] = string.Join("\n", mappers);

                // read custom data block
                // may implement more fields in the future, but right now only 'difficulty_name' is used
                try
                {
                    void SetField(string field, dynamic value)
                    {
                        switch (field)
                        {
                            case "difficulty_name":
                                Settings.settings["customDifficulty"] = value;
                                break;
                        }
                    }

                    byte[] customCount = new byte[2];
                    data.Read(customCount, 0, 2);
                    uint customCountF = BitConverter.ToUInt16(customCount);

                    for (int i = 0; i < customCountF; i++)
                    {
                        string field = GetNextVariableString();
                        byte[] id = new byte[1];
                        data.Read(id, 0, 1);

                        // discard all but 0x08 and 0x0a
                        switch (id[0])
                        {
                            case 0x00:
                                continue;
                            case 0x01:
                                data.Read(new byte[1], 0, 1);
                                break;
                            case 0x02:
                                data.Read(new byte[2], 0, 2);
                                break;
                            case 0x03:
                            case 0x05:
                                data.Read(new byte[4], 0, 4);
                                break;
                            case 0x04:
                            case 0x06:
                                data.Read(new byte[8], 0, 8);
                                break;
                            case 0x07:
                                byte[] type = new byte[1];
                                data.Read(type, 0, 1);

                                if (type[0] == 0x00)
                                    data.Read(new byte[2], 0, 2);
                                else if (type[0] == 0x01)
                                    data.Read(new byte[16], 0, 2);
                                break;
                            case 0x08:
                                GetNextVariableString();
                                break;
                            case 0x09:
                                SetField(field, GetNextVariableString());
                                break;
                            case 0x0a:
                                GetNextVariableString(true);
                                break;
                            case 0x0b:
                                SetField(field, GetNextVariableString(true));
                                break;
                            case 0x0c:
                                data.Read(new byte[1], 0, 1);

                                byte[] valueLength = new byte[4];
                                data.Read(valueLength, 0, 4);
                                int valueLengthF = (int)BitConverter.ToUInt32(valueLength);

                                data.Read(new byte[valueLengthF], 0, valueLengthF);
                                break;
                        }
                    }
                }
                catch { }

                // jump to beginning of audio block in case custom data reading was unsuccessful
                long audioOffsetF = BitConverter.ToInt64(audioOffset);
                data.Seek(audioOffsetF, SeekOrigin.Begin);

                // read and cache audio
                if (containsAudio[0] == 0x01)
                {
                    int audioLengthF = (int)BitConverter.ToInt64(audioLength);
                    byte[] audio = new byte[audioLengthF];
                    data.Read(audio, 0, audioLengthF);

                    File.WriteAllBytes($"cached/{mapID}.asset", audio);
                }

                Settings.settings["useCover"] = containsCover[0] == 0x01;

                // read cover
                if (containsCover[0] == 0x01)
                {
                    int coverLengthF = (int)BitConverter.ToInt64(coverLength);
                    byte[] cover = new byte[coverLengthF];
                    data.Read(cover, 0, coverLengthF);

                    File.WriteAllBytes($"cached/{mapID}.png", cover);
                    Settings.settings["cover"] = $"cached/{mapID}.png";
                }

                mapData = mapID;

                // read marker definitions
                bool hasNotes = false;

                byte[] numDefinitions = new byte[1];
                data.Read(numDefinitions, 0, 1);

                for (int i = 0; i < numDefinitions[0]; i++)
                {
                    string definition = GetNextVariableString();
                    hasNotes |= definition == "ssp_note" && i == 0;

                    byte[] numValues = new byte[1];
                    data.Read(numValues, 0, 1);

                    byte[] definitionData = new byte[1] { 0x01 };
                    while (definitionData[0] != 0x00)
                        data.Read(definitionData, 0, 1);
                }

                if (!hasNotes)
                    return mapData;

                // process notes
                uint noteCountF = BitConverter.ToUInt32(noteCount);
                for (int i = 0; i < noteCountF; i++)
                {
                    byte[] ms = new byte[4];
                    data.Read(ms, 0, 4);
                    byte[] markerType = new byte[1];
                    data.Read(markerType, 0, 1);

                    byte[] isQuantum = new byte[1];
                    data.Read(isQuantum, 0, 1);

                    float xF;
                    float yF;

                    if (isQuantum[0] == 0x00)
                    {
                        byte[] x = new byte[1];
                        byte[] y = new byte[1];

                        data.Read(x, 0, 1);
                        data.Read(y, 0, 1);

                        xF = x[0];
                        yF = y[0];
                    }
                    else
                    {
                        byte[] x = new byte[4];
                        byte[] y = new byte[4];

                        data.Read(x, 0, 4);
                        data.Read(y, 0, 4);

                        xF = BitConverter.ToSingle(x);
                        yF = BitConverter.ToSingle(y);
                    }

                    uint msF = BitConverter.ToUInt32(ms);

                    mapData += $",{2 - xF}|{2 - yF}|{msF}";
                }
            }
            else
                MessageBox.Show("File version not recognized or supported\nCurrently supported: SSPM v1/v2", "Warning", "OK");

            return mapData;
        }
    }
}
