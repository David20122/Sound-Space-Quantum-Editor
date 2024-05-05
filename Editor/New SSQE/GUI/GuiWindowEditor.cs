using System.Diagnostics;
using System.Drawing;
using OpenTK.Mathematics;
using New_SSQE.Types;

namespace New_SSQE.GUI
{
    internal class GuiWindowEditor : GuiWindow
    {
        // options nav
        private readonly GuiButton LNavOptions = new(10, 60, 175, 50, 3, "OPTIONS", 31, false, true);
        private readonly GuiCheckbox Numpad = new(10, 120, 30, 30, "numpad", "Use Numpad", 26, false, true);
        private readonly GuiCheckbox SeparateClickTools = new(10, 160, 30, 30, "separateClickTools", "Separate Click Modes", 26, false, true);
        private readonly GuiButton SwapClickMode = new(10, 200, 200, 40, 27, "Swap Click Mode", 26, false, true);
        private readonly GuiCheckbox JumpOnPaste = new(10, 250, 30, 30, "jumpPaste", "Jump on Paste", 26, false, true);
        private readonly GuiCheckbox PauseOnScroll = new(10, 290, 30, 30, "pauseScroll", "Pause on Seek", 26, false, true);

        // timing nav
        private readonly GuiButton LNavTiming = new(195, 60, 175, 50, 4, "TIMING", 31, false, true);
        private readonly GuiTextbox ExportOffset = new(10, 160, 130, 40, 31, "exportOffset", false, false, true);
        private readonly GuiTextbox SfxOffset = new(180, 160, 130, 40, 31, "sfxOffset", false, false, true);
        private readonly GuiButton OpenTimings = new(10, 220, 210, 40, 6, "OPEN BPM SETUP", 27, false, true);
        private readonly GuiButton ImportIni = new(10, 270, 210, 40, 16, "IMPORT INI", 27, false, true);
        private readonly GuiCheckbox Metronome = new(10, 320, 30, 30, "metronome", "Metronome", 26, false, true);
        private readonly GuiButton OpenBookmarks = new(10, 390, 210, 40, 7, "EDIT BOOKMARKS", 27, false, true);
        private readonly GuiButton CopyBookmarks = new(10, 440, 210, 40, 20, "COPY BOOKMARKS", 27, false, true);
        private readonly GuiButton PasteBookmarks = new(10, 490, 210, 40, 21, "PASTE BOOKMARKS", 27, false, true);

        private readonly GuiLabel ExportOffsetLabel = new(10, 130, 100, 30, "Export Offset:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel SfxOffsetLabel = new(180, 130, 100, 30, "SFX Offset:", 30, false, true, "main", false, "color1");

        // patterns nav
        private readonly GuiButton LNavPatterns = new(380, 60, 175, 50, 8, "PATTERNS", 31, false, true);
        private readonly GuiButton HFlip = new(10, 130, 175, 40, 9, "HORIZONTAL FLIP", 27, false, true);
        private readonly GuiButton VFlip = new(195, 130, 175, 40, 10, "VERTICAL FLIP", 27, false, true);
        public readonly GuiTextbox RotateBox = new(10, 220, 100, 40, "90", 31, true);
        private readonly GuiButton RotateButton = new(120, 220, 100, 40, 14, "ROTATE", 27, false, true);
        public readonly GuiTextbox ScaleBox = new(10, 300, 100, 40, "150", 31, true);
        private readonly GuiButton ScaleButton = new(120, 300, 100, 40, 15, "SCALE", 27, false, true);
        private readonly GuiCheckbox ApplyOnPaste = new(10, 360, 30, 30, "applyOnPaste", "Apply Rotate/Scale On Paste", 27, false, true);
        private readonly GuiCheckbox ClampSR = new(10, 400, 30, 30, "clampSR", "Clamp Rotate/Scale In Bounds", 27, false, true);
        private readonly GuiButton StoreNodes = new(10, 460, 175, 40, 11, "STORE NODES", 27, false, true);
        private readonly GuiButton ClearNodes = new(195, 460, 175, 40, 12, "CLEAR NODES", 27, false, true);
        private readonly GuiCheckbox CurveBezier = new(10, 520, 30, 30, "curveBezier", "Curve Bezier", 27, false, true);
        private readonly GuiTextbox BezierBox = new(10, 590, 100, 40, 31, "bezierDivisor", false, true, true);
        private readonly GuiButton BezierButton = new(120, 590, 100, 40, 13, "DRAW", 27, false, true);

        private readonly GuiLabel RotateLabel = new(10, 190, 175, 30, "Rotate by Degrees:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel ScaleLabel = new(10, 270, 100, 30, "Scale by Percent:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel DrawBezierLabel = new(10, 560, 100, 30, "Draw Bezier with Divisor:", 30, false, true, "main", false, "color1");

        // player nav
        private readonly GuiButton LNavPlayer = new(10, 0, 400, 50, 17, "PLAYTEST", 31, false, true);
        private readonly GuiButtonList CameraMode = new(10, 160, 150, 40, "cameraMode", 27, false, true);
        private readonly GuiTextbox NoteScale = new(185, 160, 100, 40, 31, "noteScale", true, false, true);
        private readonly GuiTextbox CursorScale = new(310, 160, 100, 40, 31, "cursorScale", true, false, true);
        private readonly GuiCheckbox LockCursor = new(10, 220, 30, 30, "lockCursor", "Lock Cursor Within Grid", 27, false, true);
        private readonly GuiCheckbox GridGuides = new(10, 270, 30, 30, "gridGuides", "Grid Guides", 27, false, true);
        private readonly GuiTextbox Sensitivity = new(10, 350, 115, 40, 31, "sensitivity", true, false, true);
        private readonly GuiTextbox Parallax = new(145, 350, 115, 40, 31, "parallax", true, false, true);
        private readonly GuiTextbox FieldOfView = new(280, 350, 115, 40, 31, "fov", true, false, true);
        private readonly GuiTextbox ApproachDistance = new(10, 435, 150, 40, 31, "approachDistance", true, false, true);
        private readonly GuiTextbox HitWindow = new(245, 435, 150, 40, 31, "hitWindow", true, false, true);
        private readonly GuiSlider PlayerApproachRate = new(10, 520, 400, 32, "playerApproachRate", false, false, true);
        private readonly GuiCheckbox ApproachFade = new(10, 570, 30, 30, "approachFade", "Approach Fade", 27, false, true);
        private readonly GuiButton FromStart = new(10, 630, 200, 40, 18, "PLAY FROM START", 27, false, true);
        private readonly GuiButton PlayMap = new(220, 630, 200, 40, 22, "PLAY HERE", 27, false, true);

        private readonly GuiLabel CameraModeLabel = new(10, 130, 100, 30, "Camera Mode:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel NoteScaleLabel = new(185, 130, 100, 30, "Note Size:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel CursorScaleLabel = new(310, 130, 100, 30, "Cursor Size:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel SensitivityLabel = new(10, 320, 100, 30, "Sensitivity:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel ParallaxLabel = new(145, 320, 100, 30, "Parallax:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel FieldOfViewLabel = new(280, 320, 100, 30, "FOV:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel ApproachDistanceLabel = new(10, 405, 100, 30, "Approach Distance:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel HitWindowLabel = new(245, 405, 100, 30, "Hit Window:", 30, false, true, "main", false, "color1");
        private readonly GuiLabel PlayerApproachRateLabel = new(10, 490, 400, 32, "", 30, false, true, "main", true, "color1");
        
        // snapping nav
        private readonly GuiButton RNavSnapping = new(1365, 60, 175, 50, 26, "SNAPPING", 31, false, true);
        private readonly GuiCheckbox Quantum = new(1610, 120, 30, 30, "enableQuantum", "Quantum", 27, false, true);
        private readonly GuiCheckbox QuantumGridSnap = new(1610, 160, 30, 30, "quantumGridSnap", "Snap to Grid", 27, false, true);
        private readonly GuiCheckbox AutoAdvance = new(1610, 200, 30, 30, "autoAdvance", "Auto-Advance", 27, false, true);
        private readonly GuiSlider BeatSnapDivisor = new(1610, 280, 250, 32, "beatDivisor", false, false, true);
        private readonly GuiSlider QuantumSnapDivisor = new(1610, 360, 250, 32, "quantumSnapping", false, false, true);

        private readonly GuiLabel BeatDivisorLabel = new(1610, 250, 250, 32, "", 30, false, true, "main", true, "color1");
        private readonly GuiLabel SnappingLabel = new(1610, 330, 250, 32, "", 30, false, true, "main", true, "color1");

        // graphics nav
        private readonly GuiButton RNavGraphics = new(1550, 60, 175, 50, 28, "GRAPHICS", 31, false, true);
        private readonly GuiCheckbox Autoplay = new(1610, 120, 30, 30, "autoplay", "Autoplay", 26, false, true);
        private readonly GuiCheckbox ApproachSquares = new(1610, 160, 30, 30, "approachSquares", "Approach Squares", 26, false, true);
        private readonly GuiCheckbox GridNumbers = new(1610, 200, 30, 30, "gridNumbers", "Grid Numbers", 26, false, true);
        private readonly GuiCheckbox GridLetters = new(1610, 240, 30, 30, "gridLetters", "Grid Letters", 26, false, true);
        private readonly GuiCheckbox QuantumGridLines = new(1610, 280, 30, 30, "quantumGridLines", "Quantum Grid Lines", 26, false, true);
        private readonly GuiSlider ApproachRate = new(1610, 360, 250, 32, "approachRate", false, false, true);
        private readonly GuiSlider TrackHeight = new(1610, 440, 250, 32, "trackHeight", false, false, true);
        private readonly GuiSlider TrackCursorPos = new(1610, 520, 250, 32, "cursorPos", false, false, true);

        private readonly GuiLabel ApproachRateLabel = new(1610, 330, 250, 32, "", 28, false, true, "main", true, "color1");
        private readonly GuiLabel TrackHeightLabel = new(1610, 410, 250, 32, "", 28, false, true, "main", true, "color1");
        private readonly GuiLabel CursorPosLabel = new(1610, 490, 250, 32, "", 28, false, true, "main", true, "color1");

        // export nav
        private readonly GuiButton RNavExport = new(1735, 60, 175, 50, 5, "EXPORT", 31, false, true);
        private readonly GuiButton SaveButton = new(1610, 130, 100, 40, 24, "SAVE", 27, false, true);
        private readonly GuiButton SaveAsButton = new(1720, 130, 100, 40, 25, "SAVE AS", 27, false, true);
        private readonly GuiButton ExportSSPMButton = new(1610, 180, 210, 40, 23, "EXPORT SSPM", 27, false, true);
        private readonly GuiTextbox ReplaceIDBox = new(1610, 320, 210, 40, 27, true);
        private readonly GuiButton ReplaceID = new(1610, 370, 210, 40, 29, "REPLACE", 27, false, true);

        private readonly GuiLabel ReplaceIDLabel = new(1610, 290, 100, 30, "Replace Audio ID", 30, false, true, "main", false, "color1");



        private readonly GuiButton CopyButton = new(0, 0, 301, 42, 0, "COPY MAP DATA", 27, true);
        private readonly GuiButton BackButton = new(0, 0, 301, 42, 1, "BACK TO MENU", 27, true);

        private readonly GuiSlider Tempo = new("tempo", false);
        private readonly GuiSlider MasterVolume = new("masterVolume", true);
        private readonly GuiSlider SfxVolume = new("sfxVolume", true);
        public readonly GuiSliderTimeline Timeline = new();
        private readonly GuiButtonPlayPause PlayPause = new(2);



        private readonly GuiLabel ToastLabel = new(42);

        private readonly GuiLabel ZoomLabel = new(565, 60, 80, 30, "Zoom: ", 32, false, true, "main", false, "color1");
        private readonly GuiLabel ZoomValueLabel = new(640, 60, 80, 30, "", 32, false, true, "main", false, "color2");
        private readonly GuiLabel ClickModeLabel = new(0, 0, 301, 42, "", 30, true, false, "main", false, "color1");

        private readonly GuiLabel TempoLabel = new(0, 0, 0, 30, "", 30, true, false, "main", true, "color1");
        private readonly GuiLabel MusicLabel = new(0, 0, 0, 30, "Music", 24, true, false, "main", true, "color1");
        private readonly GuiLabel MusicValueLabel = new(0, 0, 0, 30, "", 24, true, false, "main", true, "color1");
        private readonly GuiLabel SfxLabel = new(0, 0, 0, 30, "SFX", 24, true, false, "main", true, "color1");
        private readonly GuiLabel SfxValueLabel = new(0, 0, 0, 30, "", 24, true, false, "main", true, "color1");

        private readonly GuiLabel CurrentTimeLabel = new(0, 0, 0, 30, "", 26, true, false, "main", true, "color1");
        private readonly GuiLabel CurrentMsLabel = new(0, 0, 0, 30, "", 26, true, false, "main", true, "color1");
        private readonly GuiLabel TotalTimeLabel = new(0, 0, 0, 30, "", 26, true, false, "main", true, "color1");
        private readonly GuiLabel NotesLabel = new(0, 0, 0, 30, "", 30, true, false, "main", true, "color1");

        private float toastTime = 0f;
        private static string leftNav = "Timing";
        private static string rightNav = "Snapping";
        private bool started = false;

        private readonly bool rhythia = false;

        public GuiWindowEditor() : base(0, 0, MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y)
        {
            rhythia = Settings.settings["useRhythia"];
            if (rhythia && leftNav == "Player")
                leftNav = "Timing";

            Controls = new List<WindowControl>
            {
                // Buttons
                CopyButton, BackButton, SaveButton, PlayPause, LNavOptions, LNavTiming, OpenTimings, ImportIni, LNavPatterns, HFlip, VFlip, StoreNodes, ClearNodes,
                BezierButton, RotateButton, ScaleButton, RNavExport, OpenBookmarks, CopyBookmarks, PasteBookmarks, LNavPlayer, CameraMode, PlayMap, ExportSSPMButton,
                FromStart, RNavGraphics, RNavSnapping, SwapClickMode, SaveAsButton, ReplaceID,
                // Checkboxes
                AutoAdvance, Autoplay, ApproachSquares, GridNumbers, GridLetters, Quantum, Numpad, QuantumGridLines, QuantumGridSnap, Metronome, SeparateClickTools, JumpOnPaste,
                CurveBezier, ApplyOnPaste, LockCursor, ApproachFade, GridGuides, PauseOnScroll, ClampSR,
                // Sliders
                Tempo, MasterVolume, SfxVolume, BeatSnapDivisor, QuantumSnapDivisor, Timeline, TrackHeight, TrackCursorPos, ApproachRate, PlayerApproachRate,
                // Boxes
                ExportOffset, SfxOffset, BezierBox, RotateBox, ScaleBox, NoteScale, CursorScale, Sensitivity, Parallax, FieldOfView, ApproachDistance, HitWindow,
                ReplaceIDBox,
                // Labels
                ZoomLabel, ZoomValueLabel, ClickModeLabel, BeatDivisorLabel, SnappingLabel, TempoLabel, MusicLabel, MusicValueLabel, SfxLabel, SfxValueLabel, CurrentTimeLabel,
                CurrentMsLabel, TotalTimeLabel, NotesLabel, TrackHeightLabel, CursorPosLabel, ApproachRateLabel, ExportOffsetLabel, SfxOffsetLabel, DrawBezierLabel, RotateLabel,
                ScaleLabel, CameraModeLabel, NoteScaleLabel, CursorScaleLabel, SensitivityLabel, ParallaxLabel, FieldOfViewLabel, ReplaceIDLabel,
                ApproachDistanceLabel, PlayerApproachRateLabel, HitWindowLabel, ToastLabel
            };

            BackgroundSquare = new(Color.FromArgb(Settings.settings["editorBGOpacity"], 30, 30, 30), "background_editor.png", "editorbg");
            Track = new();
            Grid = new(300, 300);

            YOffset = Settings.settings["trackHeight"].Value + 64;
            Init();

            UpdateNav();
            started = false;
        }

        private void ShowBezier(List<Note> finalnodes, int divisor)
        {
            try
            {
                var xprev = (finalnodes[0].X + 0.5f) * Grid.Rect.Width / 3f + Grid.Rect.X;
                var yprev = (finalnodes[0].Y + 0.5f) * Grid.Rect.Width / 3f + Grid.Rect.Y;
                var color3 = Settings.settings["color3"];

                var k = finalnodes.Count - 1;
                decimal tdiff = finalnodes[k].Ms - finalnodes[0].Ms;
                decimal d = 1m / (divisor * k);

                if (!Settings.settings["curveBezier"])
                    d = 1m / divisor;

                if (Settings.settings["curveBezier"])
                {
                    for (decimal t = 0; t <= 1; t += d)
                    {
                        float xf = 0;
                        float yf = 0;
                        decimal tf = finalnodes[0].Ms + tdiff * t;

                        for (int v = 0; v <= k; v++)
                        {
                            var note = finalnodes[v];
                            var bez = (double)MainWindow.BinomialCoefficient(k, v) * (Math.Pow(1 - (double)t, k - v) * Math.Pow((double)t, v));

                            xf += (float)(bez * note.X);
                            yf += (float)(bez * note.Y);
                        }

                        var xg = (xf + 0.5f) * Grid.Rect.Width / 3f + Grid.Rect.X;
                        var yg = (yf + 0.5f) * Grid.Rect.Width / 3f + Grid.Rect.Y;

                        Grid.AddPreviewNote(xf, yf, 2);

                        xprev = xg;
                        yprev = yg;
                    }
                }
                else
                {
                    for (int v = 0; v < k; v++)
                    {
                        var note = finalnodes[v];
                        var nextNote = finalnodes[v + 1];

                        var xDist = nextNote.X - note.X;
                        var yDist = nextNote.Y - note.Y;

                        for (decimal t = d; t <= 1; t += d)
                        {
                            var xf = note.X + xDist * (float)t;
                            var yf = note.Y + yDist * (float)t;

                            var xg = (xf + 0.5f) * Grid.Rect.Width / 3f + Grid.Rect.X;
                            var yg = (yf + 0.5f) * Grid.Rect.Width / 3f + Grid.Rect.Y;

                            Grid.AddPreviewNote(xf, yf, 2);

                            xprev = xg;
                            yprev = yg;
                        }
                    }
                }
            }
            catch { MainWindow.Instance.BezierNodes.Clear(); }
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            Grid?.ClearPreviewNotes();

            if (frametime < 2)
                toastTime = Math.Min(2, toastTime + frametime);

            var toastOffset = 1f;

            if (toastTime <= 0.5f)
                toastOffset = (float)Math.Sin(Math.Min(0.5f, toastTime) / 0.5f * MathHelper.PiOver2);
            if (toastTime >= 1.75f)
                toastOffset = (float)Math.Cos(Math.Min(0.25f, toastTime - 1.75f) / 0.25f * MathHelper.PiOver2);

            var toastHeight = FontRenderer.GetHeight(ToastLabel.TextSize, ToastLabel.Font);
            ToastLabel.Rect.Location = new PointF(Rect.X + Rect.Width / 2f, Rect.Height - toastOffset * toastHeight * 2.25f + toastHeight / 2f);
            ToastLabel.Color = Color.FromArgb((int)(Math.Pow(toastOffset, 3) * 255), ToastLabel.Color);
            
            ToastLabel.Update();

            var editor = MainWindow.Instance;
            var currentTime = Settings.settings["currentTime"];

            ZoomValueLabel.Text = $"{Math.Round(editor.Zoom * 100)}%";
            ClickModeLabel.Text = $"Click Mode: {(Settings.settings["selectTool"] ? "Select" : "Place")}";
            ClickModeLabel.Visible = Settings.settings["separateClickTools"];

            TrackHeightLabel.Text = $"Track Height: {Math.Round(64f + Settings.settings["trackHeight"].Value)}";
            CursorPosLabel.Text = $"Cursor Pos: {Math.Round(Settings.settings["cursorPos"].Value)}%";
            ApproachRateLabel.Text = $"Approach Rate: {(int)(Settings.settings["approachRate"].Value + 1.5f)}";
            PlayerApproachRateLabel.Text = $"Player Approach Rate: {(int)Math.Round(Settings.settings["playerApproachRate"].Value) + 1}";

            BeatDivisorLabel.Text = $"Beat Divisor: {Math.Round(Settings.settings["beatDivisor"].Value * 10) / 10 + 1f}";
            SnappingLabel.Text = $"Snapping: 3/{Math.Round(Settings.settings["quantumSnapping"].Value) + 3}";
            TempoLabel.Text = $"PLAYBACK SPEED - {Math.Round(editor.Tempo * 100f)}%";
            MusicValueLabel.Text = Math.Round(Settings.settings["masterVolume"].Value * 100f).ToString();
            SfxValueLabel.Text = Math.Round(Settings.settings["sfxVolume"].Value * 100f).ToString();

            CurrentTimeLabel.Text = $"{(int)(currentTime.Value / 60000f)}:{(int)(currentTime.Value % 60000 / 1000f):0#}";
            TotalTimeLabel.Text = $"{(int)(currentTime.Max / 60000f)}:{(int)(currentTime.Max % 60000 / 1000f):0#}";
            NotesLabel.Text = $"{editor.Notes.Count} Notes";

            var currentMs = $"{(long)currentTime.Value:##,##0}ms";
            var progress = currentTime.Value / currentTime.Max;
            CurrentMsLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Height / 2f + (Timeline.Rect.Width - Timeline.Rect.Height) * progress, Timeline.Rect.Y - 4f);
            CurrentMsLabel.Text = currentMs;

            //bezier preview
            var bezierDivisor = (float)Settings.settings["bezierDivisor"];

            if (bezierDivisor > 0 && editor.BezierNodes.Count > 1)
            {
                var anchored = new List<int>() { 0 };

                for (int i = 0; i < editor.BezierNodes.Count; i++)
                    if (editor.BezierNodes[i].Anchored && !anchored.Contains(i))
                        anchored.Add(i);

                if (!anchored.Contains(editor.BezierNodes.Count - 1))
                    anchored.Add(editor.BezierNodes.Count - 1);

                for (int i = 1; i < anchored.Count; i++)
                {
                    var newnodes = new List<Note>();

                    for (int j = anchored[i - 1]; j <= anchored[i]; j++)
                        newnodes.Add(editor.BezierNodes[j]);
                    ShowBezier(newnodes, (int)(bezierDivisor + 0.5f));
                }
            }

            if (!started)
            {
                OnResize(new Vector2i((int)Rect.Width, (int)Rect.Height));
                started = true;
            }

            base.Render(mousex, mousey, frametime);
        }

        private bool playerRunning = false;

        public override void OnButtonClicked(int id)
        {
            var editor = MainWindow.Instance;
            var currentTime = Settings.settings["currentTime"];

            switch (id)
            {
                case 0:
                    try
                    {
                        Clipboard.SetText(Map.Save(editor.SoundID, editor.Notes, Settings.settings["correctOnCopy"]));
                        ShowToast("COPIED TO CLIPBOARD", Color.FromArgb(0, 255, 200));
                    }
                    catch { ShowToast("FAILED TO COPY", Color.FromArgb(255, 200, 0)); }

                    break;

                case 1:
                    editor.SwitchWindow(new GuiWindowMenu());

                    break;

                case 2:
                    if (editor.MusicPlayer.IsPlaying)
                        editor.MusicPlayer.Pause();
                    else
                    {
                        if (currentTime.Value >= currentTime.Max - 1)
                            currentTime.Value = 0;
                        editor.MusicPlayer.Play();
                    }

                    break;

                case 3:
                    leftNav = leftNav == "Options" ? "" : "Options";
                    UpdateNav();

                    break;

                case 4:
                    leftNav = leftNav == "Timing" ? "" : "Timing";
                    UpdateNav();

                    break;

                case 5:
                    rightNav = rightNav == "Export" ? "" : "Export";
                    UpdateNav();

                    break;

                case 6:
                    TimingsWindow.ShowWindow();

                    break;

                case 7:
                    BookmarksWindow.ShowWindow();

                    break;

                case 8:
                    leftNav = leftNav == "Patterns" ? "" : "Patterns";
                    UpdateNav();

                    break;

                case 9:
                    var selectedH = editor.SelectedNotes.ToList();

                    if (selectedH.Count > 0)
                    {
                        editor.UndoRedoManager.Add("HORIZONTAL FLIP", () =>
                        {
                            foreach (var note in selectedH)
                                note.X = 2 - note.X;
                        }, () =>
                        {
                            foreach (var note in selectedH)
                                note.X = 2 - note.X;
                        });
                    }

                    break;

                case 10:
                    var selectedV = editor.SelectedNotes.ToList();

                    if (selectedV.Count > 0)
                    {
                        editor.UndoRedoManager.Add("VERTICAL FLIP", () =>
                        {
                            foreach (var note in selectedV)
                                note.Y = 2 - note.Y;
                        }, () =>
                        {
                            foreach (var note in selectedV)
                                note.Y = 2 - note.Y;
                        });
                    }

                    break;

                case 11:
                    if (editor.SelectedNotes.Count > 1)
                        editor.BezierNodes = editor.SelectedNotes.ToList();

                    break;

                case 12:
                    editor.BezierNodes.Clear();

                    break;

                case 13:
                    editor.RunBezier();

                    break;

                case 14:
                    var selectedR = editor.SelectedNotes.ToList();

                    if (float.TryParse(RotateBox.Text, out var deg) && selectedR.Count > 0)
                    {
                        Dictionary<Note, (float, float)> oldSet = new();
                        Dictionary<Note, (float, float)> newSet = new();

                        for (int i = 0; i < selectedR.Count; i++)
                        {
                            var note = selectedR[i];
                            var angle = MathHelper.RadiansToDegrees(Math.Atan2(note.Y - 1, note.X - 1));
                            var distance = Math.Sqrt(Math.Pow(note.X - 1, 2) + Math.Pow(note.Y - 1, 2));
                            var anglef = MathHelper.DegreesToRadians(angle + deg);

                            var x = (float)(Math.Cos(anglef) * distance + 1);
                            var y = (float)(Math.Sin(anglef) * distance + 1);

                            if (Settings.settings["clampSR"])
                            {
                                x = Math.Clamp(x, -0.85f, 2.85f);
                                y = Math.Clamp(y, -0.85f, 2.85f);
                            }

                            oldSet[note] = new(note.X, note.Y);
                            newSet[note] = new(x, y);
                        }

                        editor.UndoRedoManager.Add($"ROTATE {deg}", () =>
                        {
                            foreach (var note in selectedR)
                            {
                                note.X = oldSet[note].Item1;
                                note.Y = oldSet[note].Item2;
                            }
                        }, () =>
                        {
                            foreach (var note in selectedR)
                            {
                                note.X = newSet[note].Item1;
                                note.Y = newSet[note].Item2;
                            }
                        });
                    }

                    break;

                case 15:
                    var selectedS = editor.SelectedNotes.ToList();

                    if (float.TryParse(ScaleBox.Text, out var scale) && selectedS.Count > 0)
                    {
                        var scalef = scale / 100f;

                        Dictionary<Note, (float, float)> oldSet = new();
                        Dictionary<Note, (float, float)> newSet = new();

                        for (int i = 0; i < selectedS.Count; i++)
                        {
                            var note = selectedS[i];

                            var x = (note.X - 1) * scalef + 1;
                            var y = (note.Y - 1) * scalef + 1;

                            if (Settings.settings["clampSR"])
                            {
                                x = Math.Clamp(x, -0.85f, 2.85f);
                                y = Math.Clamp(y, -0.85f, 2.85f);
                            }

                            oldSet[note] = new(note.X, note.Y);
                            newSet[note] = new(x, y);
                        }

                        editor.UndoRedoManager.Add($"SCALE {scale}%", () =>
                        {
                            foreach (var note in selectedS)
                            {
                                note.X = oldSet[note].Item1;
                                note.Y = oldSet[note].Item2;
                            }
                        }, () =>
                        {
                            foreach (var note in selectedS)
                            {
                                note.X = newSet[note].Item1;
                                note.Y = newSet[note].Item2;
                            }
                        });
                    }

                    break;

                case 16:
                    editor.ImportProperties();

                    break;

                case 17:
                    if (rhythia)
                    {
                        if (!File.Exists(Settings.settings["rhythiaPath"]))
                            ShowToast("INVALID RHYTHIA PATH - CHECK SETTINGS", Settings.settings["color1"]);
                        else
                        {
                            try
                            {
                                if (editor.MusicPlayer.IsPlaying)
                                    editor.MusicPlayer.Pause();

                                if (!Directory.Exists("assets/temp"))
                                    Directory.CreateDirectory("assets/temp");

                                Settings.Save();

                                File.WriteAllText($"assets/temp/tempmap.txt", Map.Save(editor.SoundID, editor.Notes, false, false));

                                string[] args =
                                {
                                    $"--t=\"{Path.GetFullPath("assets/temp/tempmap.txt").Replace("\\", "/")}\"",
                                    $"--a=\"{Path.GetFullPath($"cached/{editor.SoundID}.asset").Replace("\\", "/")}\""
                                };

                                Process.Start(Settings.settings["rhythiaPath"], string.Join(" ", args));
                            }
                            catch (Exception ex)
                            {
                                ActionLogging.Register($"Failed to start Rhythia", "WARN", ex);
                                ShowToast("FAILED TO START RHYTHIA", Settings.settings["color1"]);
                            }
                        }
                    }
                    else
                    {
                        leftNav = leftNav == "Player" ? "" : "Player";
                        UpdateNav();
                    }

                    break;

                case 18:
                    if (editor.MusicPlayer.IsPlaying)
                        editor.MusicPlayer.Pause();

                    string fileT = MainWindow.IsLinux ? "SSQE Player" : "SSQE Player.exe";

                    if (!playerRunning && File.Exists(fileT))
                    {
                        if (!Directory.Exists("assets/temp"))
                            Directory.CreateDirectory("assets/temp");

                        Settings.Save();

                        File.WriteAllText($"assets/temp/tempmap.txt", Map.Save(editor.SoundID, editor.Notes, false, false));

                        Process process = Process.Start(fileT, "true");
                        playerRunning = process != null;
                        
                        if (process != null)
                        {
                            process.EnableRaisingEvents = true;
                            process.Exited += delegate { playerRunning = false; };
                        }
                    }

                    break;

                case 19:
                    leftNav = leftNav == "Review" ? "" : "Review";
                    UpdateNav();

                    break;

                case 20:
                    editor.CopyBookmarks();

                    break;

                case 21:
                    editor.PasteBookmarks();

                    break;

                case 22:
                    if (editor.MusicPlayer.IsPlaying)
                        editor.MusicPlayer.Pause();

                    string fileF = MainWindow.IsLinux ? "SSQE Player" : "SSQE Player.exe";

                    if (!playerRunning && File.Exists(fileF))
                    {
                        if (!Directory.Exists("assets/temp"))
                            Directory.CreateDirectory("assets/temp");

                        Settings.Save();

                        File.WriteAllText($"assets/temp/tempmap.txt", Map.Save(editor.SoundID, editor.Notes, false, false));

                        Process process = Process.Start(fileF, "false");
                        playerRunning = process != null;

                        if (process != null)
                        {
                            process.EnableRaisingEvents = true;
                            process.Exited += delegate { playerRunning = false; };
                        }
                    }

                    break;

                case 23:
                    ExportSSPM.ShowWindow();

                    break;

                case 24:
                    if (editor.SaveMap(true))
                        ShowToast("SAVED", Settings.settings["color1"]);

                    break;

                case 25:
                    if (editor.SaveMap(true, true))
                        ShowToast("SAVED", Settings.settings["color1"]);

                    break;

                case 26:
                    rightNav = rightNav == "Snapping" ? "" : "Snapping";
                    UpdateNav();

                    break;

                case 27:
                    Settings.settings["selectTool"] ^= true;

                    break;

                case 28:
                    rightNav = rightNav == "Graphics" ? "" : "Graphics";
                    UpdateNav();

                    break;

                case 29:
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(ReplaceIDBox.Text))
                        {
                            var result = MessageBox.Show("Are you sure you want to replace this ID?\n\nAny existing asset with this ID will be overwritten and the current map will be saved.", "Warning", "Yes", "No");
                            if (result == DialogResult.No)
                                return;

                            string newID = ReplaceIDBox.Text;
                            editor.MusicPlayer.Reset();

                            File.Move($"cached/{editor.SoundID}.asset", $"cached/{newID}.asset", true);
                            editor.SoundID = newID;

                            editor.LoadAudio(newID);
                            editor.MusicPlayer.Volume = Settings.settings["masterVolume"].Value;

                            if (editor.FileName != null)
                                editor.SaveMap(true);
                            else
                                editor.AttemptAutosave(true);
                            ShowToast($"REPLACED AUDIO ID WITH: {newID}", Settings.settings["color1"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowToast("FAILED TO REPLACE ID", Settings.settings["color1"]);
                        ActionLogging.Register("Failed to replace audio ID", "WARN", ex);
                    }
                    
                    break;
            }

            base.OnButtonClicked(id);
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            if (Timeline.HoveringBookmark != null && !right)
            {
                MainWindow.Instance.MusicPlayer.Pause();
                Settings.settings["currentTime"].Value = Timeline.HoveringBookmark.Ms;
            }

            base.OnMouseClick(pos, right);
        }

        public void Update()
        {
            Timeline.Update();
            Track?.Update();
        }

        public void UpdateNav()
        {
            var timingNav = leftNav == "Timing";
            var patternsNav = leftNav == "Patterns";
            var optionsNav = leftNav == "Options";
            var playerNav = leftNav == "Player";

            var snappingNav = rightNav == "Snapping";
            var exportNav = rightNav == "Export";
            var graphicsNav = rightNav == "Graphics";

            LNavTiming.Text = timingNav ? "[TIMING]" : "TIMING";
            LNavOptions.Text = optionsNav ? "[OPTIONS]" : "OPTIONS";
            LNavPatterns.Text = patternsNav ? "[PATTERNS]" : "PATTERNS";
            if (!rhythia)
                LNavPlayer.Text = playerNav ? "[PLAYTEST]" : "PLAYTEST";

            RNavSnapping.Text = snappingNav ? "[SNAPPING]" : "SNAPPING";
            RNavExport.Text = exportNav ? "[EXPORT]" : "EXPORT";
            RNavGraphics.Text = graphicsNav ? "[GRAPHICS]" : "GRAPHICS";


            ExportOffset.Visible = timingNav;
            SfxOffset.Visible = timingNav;
            OpenTimings.Visible = timingNav;
            ImportIni.Visible = timingNav;
            Metronome.Visible = timingNav;
            OpenBookmarks.Visible = timingNav;
            CopyBookmarks.Visible = timingNav;
            PasteBookmarks.Visible = timingNav;

            ExportOffsetLabel.Visible = timingNav;
            SfxOffsetLabel.Visible = timingNav;


            HFlip.Visible = patternsNav;
            VFlip.Visible = patternsNav;
            RotateBox.Visible = patternsNav;
            RotateButton.Visible = patternsNav;
            ScaleBox.Visible = patternsNav;
            ScaleButton.Visible = patternsNav;
            ApplyOnPaste.Visible = patternsNav;
            ClampSR.Visible = patternsNav;
            StoreNodes.Visible = patternsNav;
            ClearNodes.Visible = patternsNav;
            CurveBezier.Visible = patternsNav;
            BezierBox.Visible = patternsNav;
            BezierButton.Visible = patternsNav;

            RotateLabel.Visible = patternsNav;
            ScaleLabel.Visible = patternsNav;
            DrawBezierLabel.Visible = patternsNav;


            SaveButton.Visible = exportNav;
            SaveAsButton.Visible = exportNav;
            ExportSSPMButton.Visible = exportNav;
            ReplaceIDBox.Visible = exportNav;
            ReplaceID.Visible = exportNav;

            ReplaceIDLabel.Visible = exportNav;


            CameraMode.Visible = playerNav;
            NoteScale.Visible = playerNav;
            CursorScale.Visible = playerNav;
            LockCursor.Visible = playerNav;
            Sensitivity.Visible = playerNav;
            Parallax.Visible = playerNav;
            FieldOfView.Visible = playerNav;
            ApproachDistance.Visible = playerNav;
            HitWindow.Visible = playerNav;
            PlayerApproachRate.Visible = playerNav;
            ApproachFade.Visible = playerNav;
            GridGuides.Visible = playerNav;
            FromStart.Visible = playerNav;
            PlayMap.Visible = playerNav;

            CameraModeLabel.Visible = playerNav;
            NoteScaleLabel.Visible = playerNav;
            CursorScaleLabel.Visible = playerNav;
            SensitivityLabel.Visible = playerNav;
            ParallaxLabel.Visible = playerNav;
            FieldOfViewLabel.Visible = playerNav;
            ApproachDistanceLabel.Visible = playerNav;
            HitWindowLabel.Visible = playerNav;
            PlayerApproachRateLabel.Visible = playerNav;


            Quantum.Visible = snappingNav;
            QuantumGridSnap.Visible = snappingNav;
            AutoAdvance.Visible = snappingNav;
            BeatSnapDivisor.Visible = snappingNav;
            QuantumSnapDivisor.Visible = snappingNav;

            BeatDivisorLabel.Visible = snappingNav;
            SnappingLabel.Visible = snappingNav;


            Numpad.Visible = optionsNav;
            SeparateClickTools.Visible = optionsNav;
            SwapClickMode.Visible = optionsNav;
            JumpOnPaste.Visible = optionsNav;
            PauseOnScroll.Visible = optionsNav;


            Autoplay.Visible = graphicsNav;
            ApproachSquares.Visible = graphicsNav;
            GridNumbers.Visible = graphicsNav;
            GridLetters.Visible = graphicsNav;
            QuantumGridLines.Visible = graphicsNav;
            ApproachRate.Visible = graphicsNav;
            TrackHeight.Visible = graphicsNav;
            TrackCursorPos.Visible = graphicsNav;

            ApproachRateLabel.Visible = graphicsNav;
            TrackHeightLabel.Visible = graphicsNav;
            CursorPosLabel.Visible = graphicsNav;


            OnResize(new Vector2i((int)Rect.Width, (int)Rect.Height));
        }

        public override void OnResize(Vector2i size)
        {
            base.OnResize(size);
            string file = MainWindow.IsLinux ? "SSQE Player" : "SSQE Player.exe";

            LNavPlayer.Visible = File.Exists(file) || Settings.settings["useRhythia"];

            var heightdiff = size.Y / 1080f;

            LNavTiming.Update();
            LNavPatterns.Update();
            LNavOptions.Update();
            LNavPlayer.Update();

            RNavSnapping.Update();
            RNavExport.Update();
            RNavGraphics.Update();

            CopyButton.Rect.Location = new PointF(Grid.Rect.X, Grid.Rect.Y - 42 - 75 * heightdiff);
            BackButton.Rect.Location = new PointF(Grid.Rect.X, Grid.Rect.Bottom + 84 * heightdiff);
            ClickModeLabel.Rect.Location = new PointF(Grid.Rect.X, BackButton.Rect.Bottom + 10 * heightdiff);

            Timeline.Rect = new RectangleF(0, Rect.Height - 64f, Rect.Width - 576f, 64f);
            PlayPause.Rect = new RectangleF(Rect.Width - 576f, Rect.Height - 64f, 64f, 64f);
            Tempo.Rect = new RectangleF(Rect.Width - 512f, Rect.Height - 64f, 512f, 64f);

            MasterVolume.Rect = new RectangleF(Rect.Width - 64f, Rect.Height - 320f * heightdiff, 40f, 256f * heightdiff);
            SfxVolume.Rect = new RectangleF(Rect.Width - 128f, Rect.Height - 320f * heightdiff, 40f, 256f * heightdiff);

            TempoLabel.Rect.Location = new PointF(Tempo.Rect.X + Tempo.Rect.Width / 2f, Tempo.Rect.Bottom - 32f);
            MusicLabel.Rect.Location = new PointF(MasterVolume.Rect.X + MasterVolume.Rect.Width / 2f, MasterVolume.Rect.Y - 10f);
            SfxLabel.Rect.Location = new PointF(SfxVolume.Rect.X + SfxVolume.Rect.Width / 2f, SfxVolume.Rect.Y - 10f);
            MusicValueLabel.Rect.Location = new PointF(MasterVolume.Rect.X + MasterVolume.Rect.Width / 2f, MasterVolume.Rect.Bottom - 20f);
            SfxValueLabel.Rect.Location = new PointF(SfxVolume.Rect.X + SfxVolume.Rect.Width / 2f, SfxVolume.Rect.Bottom - 20f);

            var currentTime = Settings.settings["currentTime"];
            var progress = currentTime.Value / currentTime.Max;

            CurrentTimeLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Height / 2f, Timeline.Rect.Bottom - 32f);
            CurrentMsLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Height / 2f + (Timeline.Rect.Width - Timeline.Rect.Height) * progress, Timeline.Rect.Y - 4f);
            TotalTimeLabel.Rect.Location = new PointF(Timeline.Rect.X - Timeline.Rect.Height / 2f + Timeline.Rect.Width, Timeline.Rect.Bottom - 32f);
            NotesLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Width / 2f, Timeline.Rect.Bottom - 32f);

            LNavPlayer.Rect.Location = new PointF(LNavPlayer.Rect.X, Timeline.Rect.Y - LNavPlayer.Rect.Height - 20f);

            CopyButton.Update();
            BackButton.Update();
            ClickModeLabel.Update();

            Timeline.Update();
            PlayPause.Update();
            Tempo.Update();

            MasterVolume.Update();
            SfxVolume.Update();

            TempoLabel.Update();
            MusicLabel.Update();
            SfxLabel.Update();
            MusicValueLabel.Update();
            SfxValueLabel.Update();

            CurrentTimeLabel.Update();
            CurrentMsLabel.Update();
            TotalTimeLabel.Update();
            NotesLabel.Update();

            LNavPlayer.Update();
        }

        public void ShowToast(string text, Color color)
        {
            toastTime = 0f;

            ToastLabel.Text = text;
            ToastLabel.Color = color;
        }
    }
}
