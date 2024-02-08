using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Drawing;
using OpenTK.Mathematics;
using New_SSQE.Types;

namespace New_SSQE.GUI
{
    internal class GuiWindowEditor : GuiWindow
    {
        private readonly GuiButton CopyButton = new(0, 0, 301, 42, 0, "COPY MAP DATA", 21, true);
        private readonly GuiButton BackButton = new(0, 0, 235, 42, 1, "BACK TO MENU", 21, true);
        private readonly GuiButton SaveButton = new(0, 0, 61, 42, 24, "SAVE", 21, true);

        private readonly GuiSlider Tempo = new(0, 0, 0, 0, "tempo", false);
        private readonly GuiSlider MasterVolume = new(0, 0, 0, 0, "masterVolume", true);
        private readonly GuiSlider SfxVolume = new(0, 0, 0, 0, "sfxVolume", true);
        private readonly GuiSlider BeatSnapDivisor = new(0, 0, 0, 0, "beatDivisor", false);
        private readonly GuiSlider QuantumSnapDivisor = new(0, 0, 0, 0, "quantumSnapping", false);
        public readonly GuiSliderTimeline Timeline = new(0, 0, 0, 0, false);
        private readonly GuiButtonPlayPause PlayPause = new(0, 0, 0, 0, 2);
        private readonly GuiCheckbox AutoAdvance = new(0, 0, 0, 0, "autoAdvance", "Auto-Advance", 25);

        private readonly GuiButton OptionsNav = new(10, 60, 400, 50, 3, "OPTIONS >", 25, false, true);
        private readonly GuiCheckbox Autoplay = new(10, 130, 30, 30, "autoplay", "Autoplay", 20, false, true);
        private readonly GuiCheckbox ApproachSquares = new(10, 170, 30, 30, "approachSquares", "Approach Squares", 20, false, true);
        private readonly GuiCheckbox GridNumbers = new(10, 210, 30, 30, "gridNumbers", "Grid Numbers", 20, false, true);
        private readonly GuiCheckbox GridLetters = new(10, 250, 30, 30, "gridLetters", "Grid Letters", 20, false, true);
        private readonly GuiCheckbox Quantum = new(10, 290, 30, 30, "enableQuantum", "Quantum", 20, false, true);
        private readonly GuiCheckbox Numpad = new(10, 330, 30, 30, "numpad", "Use Numpad", 20, false, true);
        private readonly GuiCheckbox QuantumGridLines = new(10, 370, 30, 30, "quantumGridLines", "Quantum Grid Lines", 20, false, true);
        private readonly GuiCheckbox QuantumGridSnap = new(10, 410, 30, 30, "quantumGridSnap", "Snap to Grid", 20, false, true);
        private readonly GuiCheckbox Metronome = new(10, 450, 30, 30, "metronome", "Metronome", 20, false, true);
        private readonly GuiCheckbox SeparateClickTools = new(10, 490, 30, 30, "separateClickTools", "Separate Click Tools", 20, false, true);
        private readonly GuiCheckbox JumpOnPaste = new(10, 530, 30, 30, "jumpPaste", "Jump on Paste", 20, false, true);
        private readonly GuiSlider TrackHeight = new(378, 384, 32, 224, "trackHeight", false, false, true);
        private readonly GuiSlider TrackCursorPos = new(10, 596, 400, 32, "cursorPos", false, false, true);
        private readonly GuiSlider ApproachRate = new(378, 124, 32, 224, "approachRate", true, false, true);

        private readonly GuiButton TimingNav = new(10, 120, 400, 50, 4, "TIMING >", 25, false, true);
        private readonly GuiTextbox ExportOffset = new(10, 210, 128, 40, "0", 25, true, false, true, "exportOffset");
        private readonly GuiTextbox SfxOffset = new(10, 285, 128, 40, "0", 25, true, false, true, "sfxOffset");
        private readonly GuiButton UseCurrentMs = new(143, 210, 192, 40, 5, "USE CURRENT MS", 21, false, true);
        private readonly GuiButton OpenTimings = new(10, 335, 256, 40, 6, "OPEN BPM SETUP", 21, false, true);
        private readonly GuiButton ImportIni = new(10, 385, 256, 40, 16, "IMPORT INI", 21, false, true);

        private readonly GuiButton PatternsNav = new(10, 180, 400, 50, 8, "PATTERNS >", 25, false, true);
        private readonly GuiButton HFlip = new(10, 250, 256, 40, 9, "HORIZONTAL FLIP", 21, false, true);
        private readonly GuiButton VFlip = new(10, 300, 256, 40, 10, "VERTICAL FLIP", 21, false, true);
        private readonly GuiButton StoreNodes = new(10, 360, 256, 40, 11, "STORE NODES", 21, false, true);
        private readonly GuiButton ClearNodes = new(10, 410, 256, 40, 12, "CLEAR NODES", 21, false, true);
        private readonly GuiCheckbox CurveBezier = new(10, 460, 40, 40, "curveBezier", "Curve Bezier", 25, false, true);
        private readonly GuiTextbox BezierBox = new(10, 532, 128, 40, "4", 25, true, false, true, "bezierDivisor");
        private readonly GuiButton BezierButton = new(143, 532, 128, 40, 13, "DRAW", 21, false, true);
        public readonly GuiTextbox RotateBox = new(10, 607, 128, 40, "90", 25, true, false, true);
        private readonly GuiButton RotateButton = new(143, 607, 128, 40, 14, "ROTATE", 21, false, true);
        public readonly GuiTextbox ScaleBox = new(10, 682, 128, 40, "150", 25, true, false, true);
        private readonly GuiButton ScaleButton = new(143, 682, 128, 40, 15, "SCALE", 21, false, true);
        private readonly GuiCheckbox ApplyOnPaste = new(10, 732, 40, 40, "applyOnPaste", "Apply Rotate/Scale On Paste", 25, false, true);

        private readonly GuiButton ReviewNav = new(10, 240, 400, 50, 19, "REVIEW >", 25, false, true);
        private readonly GuiButton OpenBookmarks = new(10, 310, 256, 40, 7, "EDIT BOOKMARKS", 21, false, true);
        private readonly GuiButton CopyBookmarks = new(10, 360, 256, 40, 20, "COPY BOOKMARKS", 21, false, true);
        private readonly GuiButton PasteBookmarks = new(10, 410, 256, 40, 21, "PASTE BOOKMARKS", 21, false, true);
        private readonly GuiButton ExportSSPMButton = new(10, 460, 256, 40, 23, "EXPORT SSPM", 21, false, true);

        private readonly GuiButton PlayerNav = new(10, 300, 400, 50, 17, "PLAYER >", 25, false, true);
        private readonly GuiButtonList CameraMode = new(10, 395, 148, 40, "cameraMode", 21, false, true);
        private readonly GuiTextbox NoteScale = new(168, 395, 108, 40, "1", 25, true, false, true, "noteScale", "main", false, true);
        private readonly GuiTextbox CursorScale = new(285, 395, 108, 40, "1", 25, true, false, true, "cursorScale", "main", false, true);
        private readonly GuiCheckbox LockCursor = new(10, 445, 40, 40, "lockCursor", "Lock Cursor Within Grid", 25, false, true);
        private readonly GuiTextbox Sensitivity = new(10, 520, 108, 40, "1", 25, true, false, true, "sensitivity", "main", false, true);
        private readonly GuiTextbox Parallax = new(128, 520, 108, 40, "1", 25, true, false, true, "parallax", "main", false, true);
        private readonly GuiTextbox FieldOfView = new(245, 520, 108, 40, "70", 25, true, false, true, "fov", "main", false, true);
        private readonly GuiTextbox ApproachDistance = new(10, 595, 128, 40, "1", 25, true, false, true, "approachDistance", "main", false, true);
        private readonly GuiTextbox HitWindow = new(225, 595, 128, 40, "55", 25, true, false, true, "hitWindow", "main", false, true);
        private readonly GuiSlider PlayerApproachRate = new(10, 670, 400, 32, "playerApproachRate", false, false, true);
        private readonly GuiCheckbox ApproachFade = new(10, 710, 40, 40, "approachFade", "Enable Approach Fade", 25, false, true);
        private readonly GuiCheckbox GridGuides = new(10, 760, 40, 40, "gridGuides", "Show Grid Guides", 25, false, true);
        private readonly GuiCheckbox FromStart = new(10, 810, 40, 40, "fromStart", "Play From Start", 25, false, true);
        private readonly GuiButton PlayMap = new(10, 860, 256, 40, 18, "PLAY MAP", 21, false, true);

        private readonly GuiLabel ToastLabel = new(0, 0, 0, 0, "", 36);

        private readonly GuiLabel ZoomLabel = new(420, 60, 75, 30, "Zoom: ", 24, true, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel ZoomValueLabel = new(495, 60, 75, 30, "", 24, true, true, "main", false, Settings.settings["color2"]);
        private readonly GuiLabel ClickModeLabel = new(0, 0, 301, 42, "", 24, true, false, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel BeatDivisorLabel = new(0, 0, 0, 30, "", 24, true, true, "main", true, Settings.settings["color1"]);
        private readonly GuiLabel SnappingLabel = new(0, 0, 0, 30, "", 24, true, true, "main", true, Settings.settings["color1"]);

        private readonly GuiLabel TempoLabel = new(0, 0, 0, 30, "", 24, true, false, "main", true, Settings.settings["color1"]);
        private readonly GuiLabel MusicLabel = new(0, 0, 0, 30, "Music", 18, true, false, "main", true, Settings.settings["color1"]);
        private readonly GuiLabel MusicValueLabel = new(0, 0, 0, 30, "", 18, true, false, "main", true, Settings.settings["color1"]);
        private readonly GuiLabel SfxLabel = new(0, 0, 0, 30, "SFX", 18, true, false, "main", true, Settings.settings["color1"]);
        private readonly GuiLabel SfxValueLabel = new(0, 0, 0, 30, "", 18, true, false, "main", true, Settings.settings["color1"]);

        private readonly GuiLabel CurrentTimeLabel = new(0, 0, 0, 30, "", 20, true, false, "main", true, Settings.settings["color1"]);
        private readonly GuiLabel CurrentMsLabel = new(0, 0, 0, 30, "", 20, true, false, "main", true, Settings.settings["color1"]);
        private readonly GuiLabel TotalTimeLabel = new(0, 0, 0, 30, "", 20, true, false, "main", true, Settings.settings["color1"]);
        private readonly GuiLabel NotesLabel = new(0, 0, 0, 30, "", 24, true, false, "main", true, Settings.settings["color1"]);

        private readonly GuiLabel TrackHeightLabel = new(220, 576, 158, 30, "", 22, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel CursorPosLabel = new(10, 576, 158, 30, "", 22, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel ApproachRateLabel = new(200, 308, 158, 30, "", 22, false, true, "main", false, Settings.settings["color1"]);

        private readonly GuiLabel ExportOffsetLabel = new(10, 183, 158, 30, "Export Offset[ms]:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel SfxOffsetLabel = new(10, 258, 158, 30, "SFX Offset[ms]:", 24, false, true, "main", false, Settings.settings["color1"]);

        private readonly GuiLabel DrawBezierLabel = new(10, 505, 158, 30, "Draw Bezier with Divisor:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel RotateLabel = new(10, 580, 158, 30, "Rotate by Degrees:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel ScaleLabel = new(10, 655, 158, 30, "Scale by Percent:", 24, false, true, "main", false, Settings.settings["color1"]);

        private readonly GuiLabel CameraModeLabel = new(10, 368, 128, 30, "Camera Mode:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel NoteScaleLabel = new(168, 368, 128, 30, "Note Size:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel CursorScaleLabel = new(285, 368, 128, 30, "Cursor Size:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel SensitivityLabel = new(10, 493, 128, 30, "Sensitivity:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel ParallaxLabel = new(128, 493, 128, 30, "Parallax:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel FieldOfViewLabel = new(245, 493, 128, 30, "FOV:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel ApproachDistanceLabel = new(10, 568, 158, 30, "Approach Distance:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel HitWindowLabel = new(225, 568, 158, 30, "Hit Window[ms]:", 24, false, true, "main", false, Settings.settings["color1"]);
        private readonly GuiLabel PlayerApproachRateLabel = new(10, 643, 158, 30, "", 24, false, true, "main", false, Settings.settings["color1"]);

        private float toastTime = 0f;
        private string navEnabled = "";
        private bool started = false;

        public GuiWindowEditor() : base(0, 0, MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y)
        {
            Controls = new List<WindowControl>
            {
                // Buttons
                CopyButton, BackButton, SaveButton, PlayPause, OptionsNav, TimingNav, UseCurrentMs, OpenTimings, ImportIni, PatternsNav, HFlip, VFlip, StoreNodes, ClearNodes,
                BezierButton, RotateButton, ScaleButton, ReviewNav, OpenBookmarks, CopyBookmarks, PasteBookmarks, PlayerNav, CameraMode, PlayMap, ExportSSPMButton,
                // Checkboxes
                AutoAdvance, Autoplay, ApproachSquares, GridNumbers, GridLetters, Quantum, Numpad, QuantumGridLines, QuantumGridSnap, Metronome, SeparateClickTools, JumpOnPaste,
                CurveBezier, ApplyOnPaste, LockCursor, ApproachFade, FromStart, GridGuides,
                // Sliders
                Tempo, MasterVolume, SfxVolume, BeatSnapDivisor, QuantumSnapDivisor, Timeline, TrackHeight, TrackCursorPos, ApproachRate, PlayerApproachRate,
                // Boxes
                ExportOffset, SfxOffset, BezierBox, RotateBox, ScaleBox, NoteScale, CursorScale, Sensitivity, Parallax, FieldOfView, ApproachDistance, HitWindow,
                // Labels
                ZoomLabel, ZoomValueLabel, ClickModeLabel, BeatDivisorLabel, SnappingLabel, TempoLabel, MusicLabel, MusicValueLabel, SfxLabel, SfxValueLabel, CurrentTimeLabel,
                CurrentMsLabel, TotalTimeLabel, NotesLabel, TrackHeightLabel, CursorPosLabel, ApproachRateLabel, ExportOffsetLabel, SfxOffsetLabel, DrawBezierLabel, RotateLabel,
                ScaleLabel, CameraModeLabel, NoteScaleLabel, CursorScaleLabel, SensitivityLabel, ParallaxLabel, FieldOfViewLabel,
                ApproachDistanceLabel, PlayerApproachRateLabel, HitWindowLabel, ToastLabel
            };

            BackgroundSquare = new(0, 0, 1920, 1080, Color.FromArgb(Settings.settings["editorBGOpacity"], 30, 30, 30), false, "background_editor.png", "editorbg");
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
                    navEnabled = navEnabled == "Options" ? "" : "Options";
                    UpdateNav();

                    break;

                case 4:
                    navEnabled = navEnabled == "Timing" ? "" : "Timing";
                    UpdateNav();

                    break;

                case 5:
                    ExportOffset.Text = ((long)currentTime.Value).ToString();
                    Settings.settings["exportOffset"] = currentTime.Value;

                    break;

                case 6:
                    TimingsWindow.ShowWindow();

                    break;

                case 7:
                    BookmarksWindow.ShowWindow();

                    break;

                case 8:
                    navEnabled = navEnabled == "Patterns" ? "" : "Patterns";
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
                        var undodeg = 360 - deg;

                        editor.UndoRedoManager.Add($"ROTATE {deg}", () =>
                        {
                            foreach (var note in selectedR)
                            {
                                var angle = MathHelper.RadiansToDegrees(Math.Atan2(note.Y - 1, note.X - 1));
                                var distance = Math.Sqrt(Math.Pow(note.X - 1, 2) + Math.Pow(note.Y - 1, 2));
                                var anglef = MathHelper.DegreesToRadians(angle + undodeg);

                                note.X = (float)(Math.Cos(anglef) * distance + 1);
                                note.Y = (float)(Math.Sin(anglef) * distance + 1);
                            }
                        }, () =>
                        {
                            foreach (var note in selectedR)
                            {
                                var angle = MathHelper.RadiansToDegrees(Math.Atan2(note.Y - 1, note.X - 1));
                                var distance = Math.Sqrt(Math.Pow(note.X - 1, 2) + Math.Pow(note.Y - 1, 2));
                                var anglef = MathHelper.DegreesToRadians(angle + deg);

                                note.X = (float)(Math.Cos(anglef) * distance + 1);
                                note.Y = (float)(Math.Sin(anglef) * distance + 1);
                            }
                        });
                    }

                    break;

                case 15:
                    var selectedS = editor.SelectedNotes.ToList();

                    if (float.TryParse(ScaleBox.Text, out var scale) && selectedS.Count > 0)
                    {
                        var scalef = scale / 100f;

                        editor.UndoRedoManager.Add($"SCALE {scale}%", () =>
                        {
                            foreach (var note in selectedS)
                            {
                                note.X = (note.X - 1) / scalef + 1;
                                note.Y = (note.Y - 1) / scalef + 1;
                            }
                        }, () =>
                        {
                            foreach (var note in selectedS)
                            {
                                note.X = (note.X - 1) * scalef + 1;
                                note.Y = (note.Y - 1) * scalef + 1;
                            }
                        });
                    }

                    break;

                case 16:
                    editor.ImportProperties();

                    break;

                case 17:
                    navEnabled = navEnabled == "Player" ? "" : "Player";
                    UpdateNav();

                    break;

                case 18:
                    if (editor.MusicPlayer.IsPlaying)
                        editor.MusicPlayer.Pause();

                    if (!playerRunning && File.Exists("SSQE Player.exe"))
                    {
                        if (!Directory.Exists("assets/temp"))
                            Directory.CreateDirectory("assets/temp");

                        Settings.Save();

                        File.WriteAllText($"assets/temp/tempmap.txt", Map.Save(editor.SoundID, editor.Notes, false, false));

                        Process process = Process.Start("SSQE Player.exe", Settings.settings["fromStart"].ToString());
                        playerRunning = process != null;
                        
                        if (process != null)
                        {
                            process.EnableRaisingEvents = true;
                            process.Exited += delegate { playerRunning = false; };
                        }
                    }

                    break;

                case 19:
                    navEnabled = navEnabled == "Review" ? "" : "Review";
                    UpdateNav();

                    break;

                case 20:
                    editor.CopyBookmarks();

                    break;

                case 21:
                    editor.PasteBookmarks();

                    break;

                case 23:
                    ExportSSPM.ShowWindow();

                    break;

                case 24:
                    if (editor.SaveMap(true))
                        ShowToast("SAVED", Settings.settings["color1"]);

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

        private void UpdateNav()
        {
            var optionsNav = navEnabled == "Options";
            var timingNav = navEnabled == "Timing";
            var patternsNav = navEnabled == "Patterns";
            var reviewNav = navEnabled == "Review";
            var playerNav = navEnabled == "Player";

            OptionsNav.Text = $"OPTIONS {(optionsNav ? "<" : ">")}";
            TimingNav.Text = $"TIMING {(timingNav ? "<" : ">")}";
            PatternsNav.Text = $"PATTERNS {(patternsNav ? "<" : ">")}";
            ReviewNav.Text = $"REVIEW {(reviewNav ? "<" : ">")}";
            PlayerNav.Text = $"PLAYER {(playerNav ? "<" : ">")}";

            Autoplay.Visible = optionsNav;
            ApproachSquares.Visible = optionsNav;
            GridNumbers.Visible = optionsNav;
            GridLetters.Visible = optionsNav;
            Quantum.Visible = optionsNav;
            Numpad.Visible = optionsNav;
            QuantumGridLines.Visible = optionsNav;
            QuantumGridSnap.Visible = optionsNav;
            Metronome.Visible = optionsNav;
            SeparateClickTools.Visible = optionsNav;
            JumpOnPaste.Visible = optionsNav;
            TrackHeight.Visible = optionsNav;
            TrackCursorPos.Visible = optionsNav;
            ApproachRate.Visible = optionsNav;
            TrackHeightLabel.Visible = optionsNav;
            CursorPosLabel.Visible = optionsNav;
            ApproachRateLabel.Visible = optionsNav;

            ExportOffset.Visible = timingNav;
            SfxOffset.Visible = timingNav;
            UseCurrentMs.Visible = timingNav;
            OpenTimings.Visible = timingNav;
            ImportIni.Visible = timingNav;
            ExportOffsetLabel.Visible = timingNav;
            SfxOffsetLabel.Visible = timingNav;

            HFlip.Visible = patternsNav;
            VFlip.Visible = patternsNav;
            StoreNodes.Visible = patternsNav;
            ClearNodes.Visible = patternsNav;
            CurveBezier.Visible = patternsNav;
            BezierBox.Visible = patternsNav;
            BezierButton.Visible = patternsNav;
            RotateBox.Visible = patternsNav;
            RotateButton.Visible = patternsNav;
            ScaleBox.Visible = patternsNav;
            ScaleButton.Visible = patternsNav;
            DrawBezierLabel.Visible = patternsNav;
            RotateLabel.Visible = patternsNav;
            ScaleLabel.Visible = patternsNav;
            ApplyOnPaste.Visible = patternsNav;

            OpenBookmarks.Visible = reviewNav;
            CopyBookmarks.Visible = reviewNav;
            PasteBookmarks.Visible = reviewNav;
            ExportSSPMButton.Visible = reviewNav;

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

            OnResize(new Vector2i((int)Rect.Width, (int)Rect.Height));
        }

        public override void OnResize(Vector2i size)
        {
            Rect = new RectangleF(0, 0, size.X, size.Y);

            base.OnResize(size);

            PlayerNav.Visible = File.Exists("SSQE Player.exe");

            var heightdiff = size.Y / 1080f;

            switch (navEnabled)
            {
                case "Options":
                    TimingNav.Rect.Y = TrackCursorPos.Rect.Bottom + 20 * heightdiff;
                    PatternsNav.Rect.Y = TimingNav.Rect.Bottom + 10 * heightdiff;
                    ReviewNav.Rect.Y = PatternsNav.Rect.Bottom + 10 * heightdiff;
                    PlayerNav.Rect.Y = ReviewNav.Rect.Bottom + 10 * heightdiff;
                    break;

                case "Timing":
                    PatternsNav.Rect.Y = ImportIni.Rect.Bottom + 20 * heightdiff;
                    ReviewNav.Rect.Y = PatternsNav.Rect.Bottom + 10 * heightdiff;
                    PlayerNav.Rect.Y = ReviewNav.Rect.Bottom + 10 * heightdiff;
                    break;

                case "Patterns":
                    ReviewNav.Rect.Y = ApplyOnPaste.Rect.Bottom + 20 * heightdiff;
                    PlayerNav.Rect.Y = ReviewNav.Rect.Bottom + 10 * heightdiff;
                    break;

                case "Review":
                    PlayerNav.Rect.Y = ExportSSPMButton.Rect.Bottom + 20 * heightdiff;
                    break;
            }


            CopyButton.Rect.Location = new PointF(Grid.Rect.X, Grid.Rect.Y - 42 - 75 * heightdiff);
            BackButton.Rect.Location = new PointF(Grid.Rect.X, Grid.Rect.Bottom + 84 * heightdiff);
            SaveButton.Rect.Location = new PointF(BackButton.Rect.Right + 5, BackButton.Rect.Y);
            ClickModeLabel.Rect.Location = new PointF(Grid.Rect.X, BackButton.Rect.Bottom + 10 * heightdiff);

            Timeline.Rect = new RectangleF(0, Rect.Height - 64f, Rect.Width - 576f, 64f);
            PlayPause.Rect = new RectangleF(Rect.Width - 576f, Rect.Height - 64f, 64f, 64f);
            Tempo.Rect = new RectangleF(Rect.Width - 512f, Rect.Height - 64f, 512f, 64f);

            AutoAdvance.TextSize = AutoAdvance.OriginTextSize;
            AutoAdvance.Rect = new RectangleF(Rect.Width - 236f, Grid.Rect.Y - 70f, 40f, 40f);
            BeatSnapDivisor.Rect = new RectangleF(Rect.Width - 256f, Grid.Rect.Y + 28f, 256f, 40f);
            QuantumSnapDivisor.Rect = new RectangleF(Rect.Width - 256f, Grid.Rect.Y + 100f, 256f, 40f);
            MasterVolume.Rect = new RectangleF(Rect.Width - 64f, Rect.Height - 320f, 40f, 256f);
            SfxVolume.Rect = new RectangleF(Rect.Width - 128f, Rect.Height - 320f, 40f, 256f);

            BeatDivisorLabel.Rect.Location = new PointF(BeatSnapDivisor.Rect.X + BeatSnapDivisor.Rect.Width / 2f, BeatSnapDivisor.Rect.Y - 20f);
            SnappingLabel.Rect.Location = new PointF(QuantumSnapDivisor.Rect.X + QuantumSnapDivisor.Rect.Width / 2f, QuantumSnapDivisor.Rect.Y - 20f);
            TempoLabel.Rect.Location = new PointF(Tempo.Rect.X + Tempo.Rect.Width / 2f, Tempo.Rect.Bottom - 28f);
            MusicLabel.Rect.Location = new PointF(MasterVolume.Rect.X + MasterVolume.Rect.Width / 2f, MasterVolume.Rect.Y - 6f);
            SfxLabel.Rect.Location = new PointF(SfxVolume.Rect.X + SfxVolume.Rect.Width / 2f, SfxVolume.Rect.Y - 6f);
            MusicValueLabel.Rect.Location = new PointF(MasterVolume.Rect.X + MasterVolume.Rect.Width / 2f, MasterVolume.Rect.Bottom - 20f);
            SfxValueLabel.Rect.Location = new PointF(SfxVolume.Rect.X + SfxVolume.Rect.Width / 2f, SfxVolume.Rect.Bottom - 20f);

            var currentTime = Settings.settings["currentTime"];
            var progress = currentTime.Value / currentTime.Max;

            CurrentTimeLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Height / 2f, Timeline.Rect.Bottom - 28f);
            CurrentMsLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Height / 2f + (Timeline.Rect.Width - Timeline.Rect.Height) * progress, Timeline.Rect.Y - 4f);
            TotalTimeLabel.Rect.Location = new PointF(Timeline.Rect.X - Timeline.Rect.Height / 2f + Timeline.Rect.Width, Timeline.Rect.Bottom - 28f);
            NotesLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Width / 2f, Timeline.Rect.Bottom - 28f);

            TimingNav.Update();
            PatternsNav.Update();
            ReviewNav.Update();
            PlayerNav.Update();

            CopyButton.Update();
            BackButton.Update();
            SaveButton.Update();
            ClickModeLabel.Update();

            Timeline.Update();
            PlayPause.Update();
            Tempo.Update();

            AutoAdvance.Update();
            BeatSnapDivisor.Update();
            QuantumSnapDivisor.Update();
            MasterVolume.Update();
            SfxVolume.Update();

            BeatDivisorLabel.Update();
            SnappingLabel.Update();
            TempoLabel.Update();
            MusicLabel.Update();
            SfxLabel.Update();
            MusicValueLabel.Update();
            SfxValueLabel.Update();

            CurrentTimeLabel.Update();
            CurrentMsLabel.Update();
            TotalTimeLabel.Update();
            NotesLabel.Update();
        }

        public void ShowToast(string text, Color color)
        {
            toastTime = 0f;

            ToastLabel.Text = text;
            ToastLabel.Color = color;
        }
    }
}
