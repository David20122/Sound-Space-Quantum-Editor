using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Net;
using System.Drawing;
using Sound_Space_Editor.GUI;
using Sound_Space_Editor.Misc;
using System.IO;
using OpenTK.Input;
using KeyPressEventArgs = OpenTK.KeyPressEventArgs;
using System.Json;
using System.Numerics;
using System.Globalization;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using Sound_Space_Editor.Properties;
using Vector2 = System.Numerics.Vector2;
using System.Text;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using Discord;
using MouseButton = OpenTK.Input.MouseButton;

//god

namespace Sound_Space_Editor
{
	class MainWindow : GameWindow
	{
        public static MainWindow Instance;
        public Gui CurrentWindow { get; private set; }

        public Point mouse = new Point(-1, -1);
        public Point lastMouse = new Point(-1, -1);

        public float tempo = 1f;
        public float zoom = 1f;
        public float NoteStep => 500f * zoom;

        public bool ctrlHeld;
        public bool altHeld;
        public bool shiftHeld;
        public bool rightHeld;

        public bool IsFullscreen;
        public Rectangle LastWindowRect;

        public MusicPlayer MusicPlayer = new MusicPlayer() { Volume = 0.2f };
        public SoundPlayer SoundPlayer = new SoundPlayer() { Volume = 0.2f };
        public UndoRedoManager UndoRedoManager = new UndoRedoManager();

        public string fileName;
        public string soundId = "-1";

        public List<Note> Notes = new List<Note>();
        public List<Note> SelectedNotes = new List<Note>();
        public List<Note> BezierNodes = new List<Note>();

        public List<TimingPoint> TimingPoints = new List<TimingPoint>();
        public List<Bookmark> Bookmarks = new List<Bookmark>();

        public TimingPoint SelectedPoint;

        public readonly Dictionary<Key, Tuple<int, int>> KeyMapping = new Dictionary<Key, Tuple<int, int>>();

        public readonly Dictionary<string, FontRenderer> Fonts = new Dictionary<string, FontRenderer>();

        private Discord.Discord discord;
        private ActivityManager activityManager;
        private bool discordEnabled = File.Exists("discord_game_sdk.dll");


        public MainWindow() : base(1280, 720, new GraphicsMode(32, 8, 0, 8), $"Sound Space Quantum Editor {Application.ProductVersion}")
        {
            VSync = VSyncMode.On;
            WindowState = WindowState.Maximized;
            LastWindowRect = new Rectangle(Location, Size);

            TargetUpdateFrequency = 1 / 20.0;

            Icon = Resources.icon;

            CheckForUpdates();

            Instance = this;

            Fonts.Add("main", new FontRenderer("main"));
            Fonts.Add("square", new FontRenderer("square"));
            Fonts.Add("squareo", new FontRenderer("squareo"));

            DiscordInit();
            SetActivity("Sitting in the menu");

            Settings.Load();

            SwitchWindow(new GuiWindowMenu());
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (discordEnabled)
                try { discord.RunCallbacks(); } catch { }
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.PushMatrix();

            if (MusicPlayer.IsPlaying && CurrentWindow is GuiWindowEditor)
                Settings.settings["currentTime"].Value = (float)MusicPlayer.CurrentTime.TotalMilliseconds;

            if (mouse != lastMouse)
            {
                CurrentWindow?.OnMouseMove(mouse);
                lastMouse = mouse;
            }

            CurrentWindow?.Render(mouse.X, mouse.Y, (float)e.Time);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.PopMatrix();
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            ClientSize = new Size(Math.Max(ClientSize.Width, 800), Math.Max(ClientSize.Height, 600));

            GL.Viewport(ClientRectangle);

            GL.MatrixMode(MatrixMode.Projection);
            var m = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, 0, 1);
            GL.LoadMatrix(ref m);

            CurrentWindow?.OnResize(ClientRectangle.Size);

            OnRenderFrame(new FrameEventArgs());
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouse = e.Position;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            CurrentWindow?.OnMouseUp(e.Position);
            if (e.Button == MouseButton.Right)
                rightHeld = false;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            CurrentWindow?.OnMouseClick(e.Position, e.Button == MouseButton.Right);
            rightHeld = rightHeld || e.Button == MouseButton.Right;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            CurrentWindow?.OnMouseLeave(mouse);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            CurrentWindow?.OnKeyTyped(e.KeyChar);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            ctrlHeld = e.Control;
            altHeld = e.Alt;
            shiftHeld = e.Shift;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            ctrlHeld = e.Control;
            altHeld = e.Alt;
            shiftHeld = e.Shift;

            if (e.Key == Key.F11)
            {
                ToggleFullscreen();
                return;
            }

            if (e.Key == Key.F4 && altHeld)
                Close();

            if (!FocusingBox())
            {
                if (CurrentWindow is GuiWindowEditor editor)
                {
                    if (e.Key == Key.Space && !editor.Timeline.dragging)
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

                    if (e.Key == Key.Left || e.Key == Key.Right)
                    {
                        if (MusicPlayer.IsPlaying)
                            MusicPlayer.Pause();

                        Advance(e.Key == Key.Left);
                    }

                    if (e.Key == Key.Escape)
                    {
                        SelectedNotes.Clear();
                        SelectedPoint = null;
                    }

                    var keybind = Settings.CompareKeybind(e.Key, ctrlHeld, altHeld, shiftHeld);

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

                        if (shiftHeld)
                            BindPattern(index);
                        else if (ctrlHeld)
                            UnbindPattern(index);
                        else
                            CreatePattern(index);

                        return;
                    }

                    switch (keybind)
                    {
                        case "selectAll":
                            SelectedNotes.Clear();
                            SelectedPoint = null;
                            SelectedNotes = Notes.ToList();

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

                                    Clipboard.SetData("notes", copied);

                                    editor.ShowToast("COPIED NOTES", Settings.settings["color1"]);
                                }
                            }
                            catch { editor.ShowToast("FAILED TO COPY", Settings.settings["color1"]); }

                            break;

                        case "paste":
                            try
                            {
                                if (Clipboard.ContainsData("notes"))
                                {
                                    var copied = ((List<Note>)Clipboard.GetData("notes")).ToList();
                                    var offset = copied.Min(n => n.Ms);

                                    copied.ForEach(n => n.Ms = (long)Settings.settings["currentTime"].Value + n.Ms - offset);

                                    UndoRedoManager.Add("Paste Notes", () =>
                                    {
                                        SelectedNotes.Clear();
                                        SelectedPoint = null;

                                        foreach (var note in copied)
                                            Notes.Remove(note);

                                        SortNotes();
                                    }, () =>
                                    {
                                        SelectedNotes = copied.ToList();
                                        SelectedPoint = null;

                                        Notes.AddRange(copied);

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

                                    Clipboard.SetData("notes", copied);

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
                            Settings.settings["selectTool"] = !Settings.settings["selectTool"];

                            break;

                        case "quantum":
                            Settings.settings["enableQuantum"] = !Settings.settings["enableQuantum"];

                            break;

                        case "openTimings":
                            if (TimingsWindow.Instance != null)
                                TimingsWindow.Instance.Close();
                            if (IsFullscreen)
                                ToggleFullscreen();

                            new TimingsWindow().Show();

                            break;

                        case "openBookmarks":
                            if (BookmarksWindow.Instance != null)
                                BookmarksWindow.Instance.Close();
                            if (IsFullscreen)
                                ToggleFullscreen();

                            new BookmarksWindow().Show();

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
                                    note.Anchored = !note.Anchored;
                            }, () =>
                            {
                                foreach (var note in selectedA)
                                    note.Anchored = !note.Anchored;
                            });

                            break;

                        case "openDirectory":
                            Process.Start(Directory.GetCurrentDirectory());

                            break;

                        case "exportSSPM":
                            new ExportSSPM().Show();

                            break;
                    }
                }
            }

            CurrentWindow?.OnKeyDown(e.Key, e.Control);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            var keyboard = Keyboard.GetState();

            ctrlHeld = keyboard.IsKeyDown(Key.LControl) || keyboard.IsKeyDown(Key.RControl);
            altHeld = keyboard.IsKeyDown(Key.LAlt) || keyboard.IsKeyDown(Key.RAlt);
            shiftHeld = keyboard.IsKeyDown(Key.LShift) || keyboard.IsKeyDown(Key.RShift);

            if (CurrentWindow is GuiWindowEditor)
            {
                if (shiftHeld)
                {
                    var setting = Settings.settings["beatDivisor"];
                    var step = setting.Step * (ctrlHeld ? 1 : 2) * e.DeltaPrecise;

                    setting.Value = MathHelper.Clamp(setting.Value + step, 0f, setting.Max);
                }
                else if (ctrlHeld)
                {
                    var step = zoom < 0.1f || (zoom == 0.1f && e.DeltaPrecise < 0) ? 0.01f : 0.1f;

                    zoom = (float)Math.Round(zoom + e.DeltaPrecise * step, 2);
                    if (zoom > 0.1f)
                        zoom = (float)Math.Round(zoom * 10) / 10;

                    zoom = MathHelper.Clamp(zoom, 0.01f, 10f);
                }
                else
                {
                    if (MusicPlayer.IsPlaying)
                        MusicPlayer.Pause();

                    var setting = Settings.settings["currentTime"];
                    var currentTime = setting.Value;
                    var totalTime = setting.Max;

                    var closest = GetClosestBeatScroll(currentTime, e.DeltaPrecise < 0);
                    var bpm = GetCurrentBpm(0);

                    currentTime = closest >= 0 || bpm.bpm > 0 ? closest : currentTime + e.DeltaPrecise / 10f * 1000f / zoom * 0.5f;

                    if (GetCurrentBpm(setting.Value).bpm == 0 && GetCurrentBpm(currentTime).bpm != 0)
                        currentTime = GetCurrentBpm(currentTime).Ms;

                    currentTime = MathHelper.Clamp(currentTime, 0f, totalTime);

                    setting.Value = currentTime;
                }
            }
            else if (CurrentWindow is GuiWindowMenu)
            {
                var setting = Settings.settings["changelogPosition"];

                setting.Value = MathHelper.Clamp(setting.Value + setting.Step * e.DeltaPrecise, 0f, setting.Max);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            CurrentWindow?.OnClosing();
            Settings.Save();

            if (TimingsWindow.Instance != null)
                TimingsWindow.Instance.Close();
            if (BookmarksWindow.Instance != null)
                BookmarksWindow.Instance.Close();

            if (CurrentWindow is GuiWindowEditor)
                e.Cancel = !SaveMap(false);

            if (!e.Cancel)
            {
                MusicPlayer.Dispose();
                ModelManager.Cleanup();
            }
        }









        public PointF PointToGridSpace(float mousex, float mousey)
        {
            var pos = new PointF(0, 0);

            if (CurrentWindow is GuiWindowEditor editor)
            {
                var quantum = Settings.settings["enableQuantum"];
                var rect = editor.grid.rect;

                var bounds = quantum ? new Vector2(-0.85f, 2.85f) : new Vector2(0, 2);

                var increment = quantum ? (Settings.settings["quantumSnapping"].Value + 3f) / 3f : 1f;
                var x = (mousex - rect.X - rect.Width / 2f) / rect.Width * 3f + 1 / increment;
                var y = (mousey - rect.Y - rect.Width / 2f) / rect.Height * 3f + 1 / increment;

                if (Settings.settings["quantumGridSnap"])
                {
                    x = (float)Math.Floor((x + 1 / increment / 2) * increment) / increment;
                    y = (float)Math.Floor((y + 1 / increment / 2) * increment) / increment;
                }

                x = MathHelper.Clamp(x - 1 / increment + 1, bounds.X, bounds.Y);
                y = MathHelper.Clamp(y - 1 / increment + 1, bounds.X, bounds.Y);

                pos = new PointF(x, y);
            }

            return pos;
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

            if (point.bpm > 0)
            {
                var interval = 60 / point.bpm * 1000f / (Settings.settings["beatDivisor"].Value + 1f);
                var offset = point.Ms % interval;

                closestMs = (long)Math.Round((long)Math.Round((currentMs - offset) / interval) * interval + offset);
            }

            return closestMs;
        }

        public long GetClosestBeatScroll(float currentMs, bool negative = false, int iterations = 1)
        {
            var closestMs = GetClosestBeat(currentMs);

            if (GetCurrentBpm(closestMs).bpm == 0)
                return -1;

            for (int i = 0; i < iterations; i++)
            {
                var currentPoint = GetCurrentBpm(currentMs, negative);
                var interval = 60000 / currentPoint.bpm / (Settings.settings["beatDivisor"].Value + 1f);

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
            var bpm = GetCurrentBpm(currentMs.Value).bpm;

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
                pattern = pattern.Substring(1, pattern.Length - 1);

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

            string[] patternsplit = pattern.split(',');
            var toAdd = new List<Note>();

            foreach (var note in patternsplit)
            {
                string[] notesplit = note.Split('|');
                var x = float.Parse(notesplit[0], culture);
                var y = float.Parse(notesplit[1], culture);
                var time = int.Parse(notesplit[2], culture);
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

        public bool PromptImport(string ID, bool create = false)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Select Audio File",
                Filter = "Audio Files (*.mp3;*.ogg;*.wav;*.flac;*.egg;*.asset)|*.mp3;*.ogg;*.wav;*.flac;*.egg;*.asset"
            })
            {
                if (Settings.settings["audioPath"] != "")
                    dialog.InitialDirectory = Settings.settings["audioPath"];

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Settings.settings["audioPath"] = Path.GetDirectoryName(dialog.FileName);
                    if (string.IsNullOrWhiteSpace(ID))
                        ID = Path.GetFileNameWithoutExtension(dialog.FileName);

                    if (dialog.FileName != $"{Path.GetDirectoryName(Application.ExecutablePath)}\\cached\\{ID}.asset")
                        File.Copy(dialog.FileName, $"cached/{ID}.asset", true);

                    MusicPlayer.Load($"cached/{ID}.asset");
                    if (create)
                        soundId = ID;

                    return true;
                }
            }

            return false;
        }

        public bool LoadAudio(string ID)
        {
            try
            {
                if (!Directory.Exists("cached/"))
                    Directory.CreateDirectory("cached/");

                if (!File.Exists($"cached/{ID}.asset"))
                {
                    if (Settings.settings["skipDownload"])
                    {
                        var message = ShowMessageBox($"No asset with id '{ID}' is present in cache.\n\nWould you like to import a file with this id?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                        return message == DialogResult.OK && PromptImport(ID);
                    }
                    else
                        using (var wc = new SecureWebClient())
                            wc.DownloadFile($"https://assetdelivery.roblox.com/v1/asset/?id={ID}", $"cached/{ID}.asset");
                }

                MusicPlayer.Load($"cached/{ID}.asset");

                return true;
            }
            catch (Exception e)
            {
                var message = ShowMessageBox($"Failed to download asset with id '{ID}':\n\n{e.Message}\n\nWould you like to import a file with this id instead?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                if (message == DialogResult.OK)
                    return PromptImport(ID);
            }

            return false;
        }

        public void RunBezier()
        {
            if (CurrentWindow is GuiWindowEditor editor)
            {
                var divisor = (int)((float)Settings.settings["bezierDivisor"] + 0.5f);

                if (divisor > 0 && ((BezierNodes != null && BezierNodes.Count > 1) || SelectedNotes.Count > 1))
                {
                    var success = true;
                    var finalnodes = BezierNodes != null && BezierNodes.Count > 1 ? BezierNodes.ToList() : SelectedNotes.ToList();
                    var finalnotes = new List<Note>();

                    var anchored = new List<int>() { 0 };

                    for (int i = 0; i < finalnodes.Count; i++)
                        if (finalnodes[i].Anchored && !anchored.Contains(i))
                            anchored.Add(i);

                    if (!anchored.Contains(finalnodes.Count - 1))
                        anchored.Add(finalnodes.Count - 1);

                    for (int i = 1; i < anchored.Count; i++)
                    {
                        var newnodes = new List<Note>();

                        for (int j = anchored[i - 1]; j <= anchored[i]; j++)
                            newnodes.Add(finalnodes[j]);

                        var finalbez = Bezier(newnodes, divisor);
                        success = finalbez != null;

                        if (success)
                            foreach (var note in finalbez)
                                finalnotes.Add(note);
                    }

                    SelectedNotes.Clear();
                    SelectedPoint = null;

                    finalnotes.Add(finalnodes[0]);

                    if (success)
                    {
                        UndoRedoManager.Add("DRAW BEZIER", () =>
                        {
                            foreach (var note in finalnotes)
                                Notes.Remove(note);
                            Notes.AddRange(finalnodes);

                            SortNotes();
                        }, () =>
                        {
                            foreach (var note in finalnodes)
                                Notes.Remove(note);
                            Notes.AddRange(finalnotes);

                            SortNotes();
                        });
                    }
                }

                foreach (var note in Notes)
                    note.Anchored = false;

                BezierNodes.Clear();
            }
        }

        public BigInteger FactorialApprox(int k)
        {
            var result = new BigInteger(1);

            if (k < 10)
                for (int i = 1; i <= k; i++)
                    result *= i;
            else
                result = (BigInteger)(Math.Sqrt(2 * Math.PI * k) * Math.Pow(k / Math.E, k));

            return result;
        }

        public BigInteger BinomialCoefficient(int k, int v)
        {
            return FactorialApprox(k) / (FactorialApprox(v) * FactorialApprox(k - v));
        }

        public List<Note> Bezier(List<Note> finalnodes, int divisor)
        {
            var finalnotes = new List<Note>();

            if (CurrentWindow is GuiWindowEditor editor)
            {
                try
                {
                    var k = finalnodes.Count - 1;
                    decimal tdiff = finalnodes[k].Ms - finalnodes[0].Ms;
                    decimal d = 1m / (divisor * k);

                    if (Settings.settings["curveBezier"])
                    {
                        for (decimal t = d; t <= 1 + d / 2m; t += d)
                        {
                            float xf = 0;
                            float yf = 0;
                            decimal tf = finalnodes[0].Ms + tdiff * t;

                            for (int v = 0; v <= k; v++)
                            {
                                var note = finalnodes[v];
                                var bez = (double)BinomialCoefficient(k, v) * (Math.Pow(1 - (double)t, k - v) * Math.Pow((double)t, v));

                                xf += (float)(bez * note.X);
                                yf += (float)(bez * note.Y);
                            }

                            finalnotes.Add(new Note(xf, yf, (long)tf));
                        }
                    }
                    else
                    {
                        d = 1m / divisor;

                        for (int v = 0; v < k; v++)
                        {
                            var note = finalnodes[v];
                            var nextnote = finalnodes[v + 1];
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

                                    finalnotes.Add(new Note((float)xf, (float)yf, (long)tf));
                                }
                            }
                        }
                    }
                }
                catch (OverflowException)
                {
                    editor.ShowToast("TOO MANY NODES", Color.FromArgb(255, 200, 0));
                    return null;
                }
                catch
                {
                    editor.ShowToast("FAILED TO DRAW CURVE", Color.FromArgb(255, 200, 0));
                    return null;
                }
            }

            return finalnotes;
        }

        private bool FocusingBox()
        {
            foreach (var box in CurrentWindow.boxes)
                if (box.focused)
                    return true;

            return false;
        }

        public bool IsMegaNote(int i)
        {
            var ms = Notes[i].Ms;

            return (i - 1 >= 0 && Notes[i - 1].Ms == ms) || (i + 1 < Notes.Count && Notes[i + 1].Ms == ms);
        }

        public void SortNotes()
        {
            Notes = new List<Note>(Notes.OrderBy(n => n.Ms));

            var noteColors = Settings.settings["noteColors"];
            var colorCount = noteColors.Count;
            var mergedColor = Settings.settings["mergedColor"];

            for (int i = 0; i < Notes.Count; i++)
            {
                var color = noteColors[i % colorCount];

                Notes[i].Color = IsMegaNote(i) ? mergedColor : color;
                Notes[i].GridColor = color;
            }
        }

        public void SortTimings(bool updateList = true)
        {
            TimingPoints = new List<TimingPoint>(TimingPoints.OrderBy(n => n.Ms));

            if (updateList && TimingsWindow.Instance != null)
                TimingsWindow.Instance.ResetList();
        }

        public void SortBookmarks(bool updateList = true)
        {
            Bookmarks = new List<Bookmark>(Bookmarks.OrderBy(n => n.Ms));

            if (updateList && BookmarksWindow.Instance != null)
                BookmarksWindow.Instance.ResetList();
        }

        public void SwitchWindow(Gui window)
        {
            if (!(CurrentWindow is GuiWindowEditor) || SaveMap(false))
            {
                if (CurrentWindow is GuiWindowEditor)
                {
                    soundId = "-1";
                    fileName = null;

                    Notes.Clear();
                    UndoRedoManager.Clear();
                    SelectedNotes.Clear();
                    SelectedPoint = null;

                    MusicPlayer.Reset();
                }

                if (window is GuiWindowEditor)
                {
                    SetActivity("Editing a map");
                    RunAutosave(DateTime.Now.Millisecond);
                }
                else if (window is GuiWindowMenu)
                    SetActivity("Sitting in the menu");

                CurrentWindow = window;

                Settings.Save();
            }
        }

        public string ParseData(bool copy = false)
        {
            var offset = (long)Settings.settings["exportOffset"];

            var final = soundId.ToString();
            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            foreach (var note in Notes)
            {
                var clone = copy ? new Note(MathHelper.Clamp(note.X, -0.85f, 2.85f), MathHelper.Clamp(note.Y, -0.85f, 2.85f), (long)MathHelper.Clamp(note.Ms, 0, Settings.settings["currentTime"].Max)) : note;

                var x = Math.Round(2 - clone.X, 2).ToString(culture);
                var y = Math.Round(2 - clone.Y, 2).ToString(culture);
                var ms = (clone.Ms + offset).ToString(culture);

                final += $",{x}|{y}|{ms}";
            }

            return final;
        }

        public string ParseProperties()
        {
            var timingfinal = new JsonArray();

            foreach (var point in TimingPoints)
                timingfinal.Add(new JsonArray() { point.bpm, point.Ms });

            var bookmarkfinal = new JsonArray();

            foreach (var bookmark in Bookmarks)
                bookmarkfinal.Add(new JsonArray() { bookmark.Text, bookmark.Ms });

            var json = new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>())
            {
                {"timings", timingfinal },
                {"bookmarks", bookmarkfinal },
                {"currentTime", Settings.settings["currentTime"].Value },
                {"beatDivisor", Settings.settings["beatDivisor"].Value },
                {"exportOffset", Settings.settings["exportOffset"] },
            };

            return json.ToString();
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
                if (fileName == null)
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

        public bool SaveMap(bool forced, bool fileforced = false)
        {
            if (fileName != null)
                Settings.settings["lastFile"] = fileName;
            Settings.Save();

            var data = ParseData();

            if (forced || (fileName == null && (Notes.Count > 0 || TimingPoints.Count > 0)) || (fileName != null && data != File.ReadAllText(fileName)))
            {
                var result = DialogResult.No;

                if (!forced)
                    result = ShowMessageBox("Would you like to save before closing?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (forced || result == DialogResult.Yes)
                {
                    if (fileName == null || fileforced)
                    {
                        using (var dialog = new SaveFileDialog
                        {
                            Title = "Save Map As",
                            Filter = "Text Documents (*.txt)|*.txt"
                        })
                        {
                            if (Settings.settings["defaultPath"] != "")
                                dialog.InitialDirectory = Settings.settings["defaultPath"];

                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                Settings.settings["defaultPath"] = Path.GetDirectoryName(dialog.FileName);

                                File.WriteAllText(dialog.FileName, data);
                                SaveProperties(dialog.FileName);
                                fileName = dialog.FileName;
                            }
                            else
                                return false;
                        }
                    }
                    else
                    {
                        File.WriteAllText(fileName, data);
                        SaveProperties(fileName);
                    }
                }
                else if (result == DialogResult.Cancel)
                    return false;
            }

            return true;
        }

        public void SaveProperties(string filepath)
        {
            var file = Path.ChangeExtension(filepath, ".ini");

            File.WriteAllText(file, ParseProperties());
            Settings.settings["lastFile"] = filepath;
        }

        public bool LoadMap(string pathordata, bool file = false, bool autosave = false)
        {
            fileName = file ? pathordata : null;

            Notes.Clear();
            SelectedNotes.Clear();
            SelectedPoint = null;

            TimingPoints.Clear();
            Bookmarks.Clear();

            UndoRedoManager.Clear();
            MusicPlayer.Reset();

            var data = file ? File.ReadAllText(pathordata) : pathordata;
            var split = data.Split(',');

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            try
            {
                var id = split[0];

                for (int i = 1; i < split.Length; i++)
                {
                    var subsplit = split[i].Split('|');

                    var x = 2 - float.Parse(subsplit[0], culture);
                    var y = 2 - float.Parse(subsplit[1], culture);
                    var ms = long.Parse(subsplit[2]);

                    Notes.Add(new Note(x, y, ms));
                }

                SortNotes();

                if (LoadAudio(id))
                {
                    soundId = id;

                    Settings.settings["currentTime"] = new SliderSetting(0f, (float)MusicPlayer.TotalTime.TotalMilliseconds, (float)MusicPlayer.TotalTime.TotalMilliseconds / 2000f);
                    Settings.settings["beatDivisor"].Value = 3f;
                    Settings.settings["tempo"].Value = 0.9f;
                    Settings.settings["exportOffset"] = 0;

                    tempo = 1f;
                    zoom = 1f;

                    if (file)
                    {
                        var propertyFile = Path.ChangeExtension(fileName, ".ini");

                        if (File.Exists(propertyFile))
                            LoadProperties(File.ReadAllText(propertyFile));
                    }
                    else if (autosave)
                        LoadProperties(Settings.settings["autosavedProperties"]);

                    SortTimings();
                    SortBookmarks();

                    SwitchWindow(new GuiWindowEditor());
                }
            }
            catch
            {
                ShowMessageBox("Failed to load map data");

                return false;
            }

            return soundId != "-1";
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
                                Bookmarks.Add(new Bookmark(bookmarksplit[0], ms));
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
                                Bookmarks.Add(new Bookmark(bookmark[0], bookmark[1]));

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
            using (var dialog = new OpenFileDialog
            {
                Title = "Select .ini File",
                Filter = "Map Property Files (*.ini)|*.ini"
            })
            {
                if (Settings.settings["defaultPath"] != "")
                    dialog.InitialDirectory = Settings.settings["defaultPath"];

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var newfile = Path.ChangeExtension(fileName, ".ini");

                    if (dialog.FileName != newfile)
                    {
                        TimingPoints.Clear();
                        Bookmarks.Clear();

                        Settings.settings["defaultPath"] = Path.GetDirectoryName(dialog.FileName);

                        File.Copy(dialog.FileName, newfile, true);

                        LoadProperties(File.ReadAllText(newfile));
                    }
                }
            }
        }

        public DialogResult ShowMessageBox(string text, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            var playing = CurrentWindow is GuiWindowEditor && MusicPlayer.IsPlaying;

            if (IsFullscreen)
                ToggleFullscreen();
            if (playing)
                MusicPlayer.Pause();

            var result = MessageBox.Show(text, caption, buttons, icon);

            if (IsFullscreen)
                ToggleFullscreen();
            if (playing)
                MusicPlayer.Play();

            return result;
        }

        public void ToggleFullscreen()
        {
            if (IsFullscreen)
            {
                WindowBorder = WindowBorder.Resizable;
                Location = LastWindowRect.Location;
                Size = LastWindowRect.Size;
            }
            else
            {
                LastWindowRect = new Rectangle(Location, Size);

                WindowBorder = WindowBorder.Hidden;
                Location = Point.Empty;
                Size = Screen.GetBounds(Location).Size;
            }

            IsFullscreen = !IsFullscreen;
        }

        public void CheckForUpdates()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
            var currentVersion = versionInfo.FileVersion;

            try
            {
                var wc = new WebClient();
                var playerVersion = wc.DownloadString("https://raw.githubusercontent.com/Avibah/Sound-Space-Quantum-Editor/map_player/PlayerVersion").Replace("\n", "");

                // player exists check
                if (!File.Exists("SSQE Player.exe"))
                {
                    var diag = ShowMessageBox("Map player is not present in this directory. Would you like to download it?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (diag == DialogResult.Yes)
                        wc.DownloadFile("https://github.com/Avibah/Sound-Space-Quantum-Editor/raw/map_player/SSQE%20Player.exe", "SSQE Player.exe");
                }

                // player version check
                if (File.Exists("SSQE Player.exe"))
                {
                    var currentPlayerVersion = FileVersionInfo.GetVersionInfo("SSQE Player.exe").FileVersion;

                    if (currentPlayerVersion != playerVersion)
                    {
                        var diag = ShowMessageBox($"New Player version is available ({playerVersion}). Would you like to download the new version?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (diag == DialogResult.Yes)
                            wc.DownloadFile("https://github.com/Avibah/Sound-Space-Quantum-Editor/raw/map_player/SSQE%20Player.exe", "SSQE Player.exe");
                    }
                }

                var request = (HttpWebRequest)WebRequest.Create("https://github.com/Avibah/Sound-Space-Quantum-Editor/releases/latest");
                request.AllowAutoRedirect = false;

                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.Redirect)
                {
                    var location = response.Headers["Location"];
                    var rep = location.LastIndexOf("/") + 1;
                    var version = location.Substring(rep, location.Length - rep);

                    var updaterVersion = wc.DownloadString("https://raw.githubusercontent.com/Avibah/Sound-Space-Quantum-Editor/updater/UpdaterVersion").Replace("\n", "");

                    // updater exists check
                    if (!File.Exists("SSQE Updater.exe"))
                    {
                        var diag = ShowMessageBox("Auto updater is not present in this directory. Would you like to download it?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (diag == DialogResult.Yes)
                            wc.DownloadFile("https://github.com/Avibah/Sound-Space-Quantum-Editor/raw/updater/SSQE%20Updater.exe", "SSQE Updater.exe");
                    }

                    // run updater
                    if (File.Exists("SSQE Updater.exe"))
                    {
                        var currentUpdaterVersion = FileVersionInfo.GetVersionInfo("SSQE Updater.exe").FileVersion;

                        // updater version check
                        if (currentUpdaterVersion != updaterVersion)
                        {
                            var diag = ShowMessageBox($"New Updater version is available ({updaterVersion}). Would you like to download the new version?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                            if (diag == DialogResult.Yes)
                                wc.DownloadFile("https://github.com/Avibah/Sound-Space-Quantum-Editor/raw/updater/SSQE%20Updater.exe", "SSQE Updater.exe");
                        }

                        // editor version check
                        if (version != currentVersion)
                        {
                            var diag = ShowMessageBox($"New Editor version is available ({version}). Would you like to download the new version?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                            if (diag == DialogResult.Yes)
                                Process.Start("SSQE Updater.exe");
                        }
                    }
                }
            }
            catch
            {
                ShowMessageBox("Failed to check for updates", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void SetTempo(float newTempo)
        {
            var tempoA = Math.Min(newTempo, 0.9f);
            var tempoB = (newTempo - tempoA) * 2f;

            tempo = tempoA + tempoB + 0.1f;
            MusicPlayer.Tempo = tempo;
        }

        public Dictionary<string, string> info = new Dictionary<string, string>
        {
            {"songId", "" },
            {"mapName", "" },
            {"mappers", "" },
            {"coverPath", "" },
            {"difficulty", "" },
        };

        public Dictionary<string, byte> difficulties = new Dictionary<string, byte>
        {
            {"N/A", 0x00 },
            {"Easy", 0x01 },
            {"Medium", 0x02 },
            {"Hard", 0x03 },
            {"Logic", 0x04 },
            {"Tasukete", 0x05 },
        };

        public void ExportSSPM()
        {
            if (CurrentWindow is GuiWindowEditor editor)
            {
                if (!Settings.settings["exportWarningShown"])
                {
                    ShowMessageBox("It's recommended to put this file directly into your SS+ maps folder or have it open so it's easier to import or replace later!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Settings.settings["exportWarningShown"] = true;
                }

                using (var dialog = new SaveFileDialog
                {
                    Title = "Export SSPM",
                    Filter = "Sound Space Plus Maps (*.sspm)|*.sspm"
                })
                {
                    if (Settings.settings["exportPath"] != "")
                        dialog.InitialDirectory = Settings.settings["exportPath"];

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        Settings.settings["exportPath"] = Path.GetDirectoryName(dialog.FileName);

                        try
                        {
                            var data = ParseSSPM();

                            using (FileStream fs = File.Create(dialog.FileName))
                                fs.Write(data, 0, data.Length);

                            editor.ShowToast("SUCCESSFULLY EXPORTED", Settings.settings["color1"]);
                        }
                        catch
                        {
                            editor.ShowToast("FAILED TO EXPORT", Settings.settings["color1"]);
                        }
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

            var customData = new byte[] { 0x00, 0x00 }; // no custom data
            var customDataOffset = BitConverter.GetBytes((ulong)offset);
            var customDataLength = BitConverter.GetBytes((ulong)customData.Length);
            offset += customData.Length;

            var audio = File.ReadAllBytes($"cached/{soundId}.asset"); // audio in bytes
            var audioOffset = BitConverter.GetBytes((ulong)offset);
            var audioLength = BitConverter.GetBytes((ulong)audio.Length);
            offset += audio.Length;

            var path = info["coverPath"];
            var cover = hasCover ? (path == "Default" || !File.Exists(path) ? BitmapToByteArray(Resources.logo) : File.ReadAllBytes(path)) : new byte[0]; // cover in bytes
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
            var hash = GetHash(markerSet);

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

        private byte[] GetHash(byte[] input)
        {
            using (SHA1 hash = SHA1.Create())
                return hash.ComputeHash(input);
        }

        private byte[] BitmapToByteArray(Bitmap input)
        {
            input.Save("ssqe_export_temp.png", ImageFormat.Png);

            var final = File.ReadAllBytes("ssqe_export_temp.png");
            File.Delete("ssqe_export_temp.png");

            return final;
        }



        public void DiscordInit()
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
                Details = $"Version {Application.ProductVersion}",
                Timestamps = { Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                Assets = { LargeImage = "logo" },
                Instance = true,
            };

            activityManager.UpdateActivity(activity, (result) =>
            {
                Console.WriteLine($"{(result == Result.Ok ? "Activity success" : "Activity failed")}");
            });
        }
    }

    class SecureWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            if (request != null)
            {
                request.UserAgent = "RobloxProxy";
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }
            return request;
        }
    }
}