using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Collections.Generic;
using Sound_Space_Editor.Misc;
using System.Diagnostics;

namespace Sound_Space_Editor.GUI
{
	class GuiWindowEditor : Gui
	{
        private readonly GuiGrid Grid = new GuiGrid(300, 300);
        private readonly GuiTrack Track = new GuiTrack();
        private readonly GuiButton CopyButton = new GuiButton(0, 0, 301, 42, 0, "COPY MAP DATA", 21, true);
        private readonly GuiButton BackButton = new GuiButton(0, 0, 301, 42, 1, "BACK TO MENU", 21, true);

        private readonly GuiSlider Tempo = new GuiSlider(0, 0, 0, 0, "tempo", false);
        private readonly GuiSlider MasterVolume = new GuiSlider(0, 0, 0, 0, "masterVolume", true);
        private readonly GuiSlider SfxVolume = new GuiSlider(0, 0, 0, 0, "sfxVolume", true);
        private readonly GuiSlider BeatSnapDivisor = new GuiSlider(0, 0, 0, 0, "beatDivisor", false);
        private readonly GuiSlider QuantumSnapDivisor = new GuiSlider(0, 0, 0, 0, "quantumSnapping", false);
        public readonly GuiSliderTimeline Timeline = new GuiSliderTimeline(0, 0, 0, 0, false);
        private readonly GuiButtonPlayPause PlayPause = new GuiButtonPlayPause(0, 0, 0, 0, 2);
        private readonly GuiCheckbox AutoAdvance = new GuiCheckbox(0, 0, 0, 0, "autoAdvance", "Auto-Advance", 25);

        private readonly GuiButton OptionsNav = new GuiButton(10, 60, 400, 50, 3, "OPTIONS >", 25, false, true);
        private readonly GuiCheckbox Autoplay = new GuiCheckbox(10, 130, 40, 40, "autoplay", "Autoplay", 25, false, true);
        private readonly GuiCheckbox ApproachSquares = new GuiCheckbox(10, 180, 40, 40, "approachSquares", "Approach Squares", 25, false, true);
        private readonly GuiCheckbox GridNumbers = new GuiCheckbox(10, 230, 40, 40, "gridNumbers", "Grid Numbers", 25, false, true);
        private readonly GuiCheckbox GridLetters = new GuiCheckbox(10, 280, 40, 40, "gridLetters", "Grid Letters", 25, false, true);
        private readonly GuiCheckbox Quantum = new GuiCheckbox(10, 330, 40, 40, "enableQuantum", "Quantum", 25, false, true);
        private readonly GuiCheckbox Numpad = new GuiCheckbox(10, 380, 40, 40, "numpad", "Use Numpad", 25, false, true);
        private readonly GuiCheckbox QuantumGridLines = new GuiCheckbox(10, 430, 40, 40, "quantumGridLines", "Quantum Grid Lines", 25, false, true);
        private readonly GuiCheckbox QuantumGridSnap = new GuiCheckbox(10, 480, 40, 40, "quantumGridSnap", "Snap to Grid", 25, false, true);
        private readonly GuiCheckbox Metronome = new GuiCheckbox(10, 530, 40, 40, "metronome", "Metronome", 25, false, true);
        private readonly GuiCheckbox SeparateClickTools = new GuiCheckbox(10, 580, 40, 40, "separateClickTools", "Separate Click Tools", 25, false, true);
        private readonly GuiSlider TrackHeight = new GuiSlider(378, 414, 32, 256, "trackHeight", false, false, true);
        private readonly GuiSlider TrackCursorPos = new GuiSlider(10, 656, 400, 32, "cursorPos", false, false, true);
        private readonly GuiSlider ApproachRate = new GuiSlider(378, 124, 32, 256, "approachRate", true, false, true);

        private readonly GuiButton TimingNav = new GuiButton(10, 120, 400, 50, 4, "TIMING >", 25, false, true);
        private readonly GuiTextbox ExportOffset = new GuiTextbox(10, 210, 128, 40, "0", 25, true, false, true, "exportOffset");
        private readonly GuiTextbox SfxOffset = new GuiTextbox(10, 285, 128, 40, "0", 25, true, false, true, "sfxOffset");
        private readonly GuiButton UseCurrentMs = new GuiButton(143, 210, 192, 40, 5, "USE CURRENT MS", 21, false, true);
        private readonly GuiButton OpenTimings = new GuiButton(10, 335, 256, 40, 6, "OPEN BPM SETUP", 21, false, true);
        private readonly GuiButton OpenBookmarks = new GuiButton(10, 385, 256, 40, 7, "EDIT BOOKMARKS", 21, false, true);
        private readonly GuiButton ImportIni = new GuiButton(10, 435, 256, 40, 16, "IMPORT INI", 21, false, true);

        private readonly GuiButton PatternsNav = new GuiButton(10, 180, 400, 50, 8, "PATTERNS >", 25, false, true);
        private readonly GuiButton HFlip = new GuiButton(10, 250, 256, 40, 9, "HORIZONTAL FLIP", 21, false, true);
        private readonly GuiButton VFlip = new GuiButton(10, 300, 256, 40, 10, "VERTICAL FLIP", 21, false, true);
        private readonly GuiButton StoreNodes = new GuiButton(10, 360, 256, 40, 11, "STORE NODES", 21, false, true);
        private readonly GuiButton ClearNodes = new GuiButton(10, 410, 256, 40, 12, "CLEAR NODES", 21, false, true);
        private readonly GuiCheckbox CurveBezier = new GuiCheckbox(10, 460, 40, 40, "curveBezier", "Curve Bezier", 25, false, true);
        private readonly GuiTextbox BezierBox = new GuiTextbox(10, 532, 128, 40, "4", 25, true, false, true, "bezierDivisor");
        private readonly GuiButton BezierButton = new GuiButton(143, 532, 128, 40, 13, "DRAW", 21, false, true);
        private readonly GuiTextbox RotateBox = new GuiTextbox(10, 607, 128, 40, "90", 25, true, false, true);
        private readonly GuiButton RotateButton = new GuiButton(143, 607, 128, 40, 14, "ROTATE", 21, false, true);
        private readonly GuiTextbox ScaleBox = new GuiTextbox(10, 682, 128, 40, "150", 25, true, false, true);
        private readonly GuiButton ScaleButton = new GuiButton(143, 682, 128, 40, 15, "SCALE", 21, false, true);

        private readonly GuiButton PlayerNav = new GuiButton(10, 240, 400, 50, 17, "PLAYER >", 25, false, true);
        private readonly GuiButtonList CameraMode = new GuiButtonList(10, 335, 256, 40, "cameraMode", 21, false, true);
        private readonly GuiCheckbox LockCursor = new GuiCheckbox(10, 385, 40, 40, "lockCursor", "Lock Cursor Within Grid", 25, false, true);
        private readonly GuiTextbox Sensitivity = new GuiTextbox(10, 460, 128, 40, "1", 25, true, false, true, "sensitivity", "main", false, true);
        private readonly GuiTextbox Parallax = new GuiTextbox(10, 535, 128, 40, "1", 25, true, false, true, "parallax", "main", false, true);
        private readonly GuiTextbox ApproachDistance = new GuiTextbox(10, 610, 128, 40, "1", 25, true, false, true, "approachDistance", "main", false, true);
        private readonly GuiSlider PlayerApproachRate = new GuiSlider(10, 685, 400, 32, "playerApproachRate", false, false, true);
        private readonly GuiCheckbox FromStart = new GuiCheckbox(10, 725, 40, 40, "fromStart", "Play From Start", 25, false, true);
        private readonly GuiButton PlayMap = new GuiButton(10, 775, 256, 40, 18, "PLAY MAP", 21, false, true);

        private readonly GuiLabel ToastLabel = new GuiLabel(0, 0, 0, 0, "", 36);

        private float toastTime = 0f;
        private string navEnabled = "";

        private int txid;
        private bool bgImg;

        private bool started = false;

        public GuiWindowEditor() : base(0, 0, MainWindow.Instance.ClientSize.Width, MainWindow.Instance.ClientSize.Height)
        {
            buttons = new List<GuiButton> { CopyButton, BackButton, PlayPause, OptionsNav, TimingNav, UseCurrentMs, OpenTimings, OpenBookmarks, ImportIni, PatternsNav, HFlip, VFlip,
                StoreNodes, ClearNodes, BezierButton, RotateButton, ScaleButton, PlayerNav, CameraMode, PlayMap };
            checkboxes = new List<GuiCheckbox> { AutoAdvance, Autoplay, ApproachSquares, GridNumbers, GridLetters, Quantum, Numpad, QuantumGridLines, QuantumGridSnap, Metronome, SeparateClickTools,
            CurveBezier, LockCursor, FromStart };
            sliders = new List<GuiSlider> { Tempo, MasterVolume, SfxVolume, BeatSnapDivisor, QuantumSnapDivisor, Timeline, TrackHeight, TrackCursorPos, ApproachRate, PlayerApproachRate };
            boxes = new List<GuiTextbox> { ExportOffset, SfxOffset, BezierBox, RotateBox, ScaleBox, Sensitivity, Parallax, ApproachDistance };
            labels = new List<GuiLabel> { ToastLabel };
            track = Track;
            grid = Grid;

            yoffset = Settings.settings["trackHeight"].Value + 64;

            if (File.Exists("background_editor.png"))
            {
                bgImg = true;

                using (Bitmap img = new Bitmap("background_editor.png"))
                    txid = TextureManager.GetOrRegister("editorbg", img, true);
            }

            UpdateNav();
            started = false;
        }

        private void ShowBezier(List<Note> finalnodes, int divisor)
        {
            try
            {
                var xprev = (finalnodes[0].X + 0.5f) * grid.rect.Width / 3f + grid.rect.X;
                var yprev = (finalnodes[0].Y + 0.5f) * grid.rect.Width / 3f + grid.rect.Y;
                var color3 = Settings.settings["color3"];

                var k = finalnodes.Count - 1;
                decimal tdiff = finalnodes[k].Ms - finalnodes[0].Ms;
                decimal d = 1m / (divisor * k);

                if (!Settings.settings["curveBezier"])
                    d = 1m / divisor;

                if (Settings.settings["curveBezier"])
                {
                    for (decimal t = d; t <= 1; t += d)
                    {
                        float xf = 0;
                        float yf = 0;
                        decimal tf = finalnodes[0].Ms + tdiff * t;

                        for (int v = 0; v <= k; v++)
                        {
                            var note = finalnodes[v];
                            var bez = (double)MainWindow.Instance.BinomialCoefficient(k, v) * (Math.Pow(1 - (double)t, k - v) * Math.Pow((double)t, v));

                            xf += (float)(bez * note.X);
                            yf += (float)(bez * note.Y);
                        }

                        var xg = (xf + 0.5f) * grid.rect.Width / 3f + grid.rect.X;
                        var yg = (yf + 0.5f) * grid.rect.Width / 3f + grid.rect.Y;

                        GL.Color3(1f, 1f, 1f);
                        GLSpecial.Line(xprev, yprev, xg, yg);

                        grid.RenderPreviewNote(xf, yf, color3);

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

                            var xg = (xf + 0.5f) * grid.rect.Width / 3f + grid.rect.X;
                            var yg = (yf + 0.5f) * grid.rect.Width / 3f + grid.rect.Y;

                            GL.Color3(1f, 1f, 1f);
                            GLSpecial.Line(xprev, yprev, xg, yg);

                            grid.RenderPreviewNote(xf, yf, color3);

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
            var widthdiff = rect.Width / 1920f;
            var heightdiff = rect.Height / 1080f;
            var textScale = (int)Math.Min(24 * widthdiff, 24 * heightdiff);

            if (bgImg)
            {
                GL.Color4(Color.FromArgb((int)Settings.settings["editorBGOpacity"], 255, 255, 255));
                GLSpecial.TexturedQuad(rect, 0, 0, 1, 1, txid);
            }

            base.Render(mousex, mousey, frametime);

            toastTime = Math.Min(2, toastTime + frametime);

            var toastOffset = 1f;
            var toastHeight = TextHeight(ToastLabel.textSize);

            if (toastTime <= 0.5f)
                toastOffset = (float)Math.Sin(Math.Min(0.5f, toastTime) / 0.5f * MathHelper.PiOver2);
            if (toastTime >= 1.75f)
                toastOffset = (float)Math.Cos(Math.Min(0.25, toastTime - 1.75f) / 0.25f * MathHelper.PiOver2);

            ToastLabel.rect.Location = new PointF(960 * widthdiff, rect.Height - toastOffset * toastHeight * 3.25f + toastHeight / 2f);
            ToastLabel.color = Color.FromArgb((int)(Math.Pow(toastOffset, 3) * 255), ToastLabel.color);

            var editor = MainWindow.Instance;
            var color1 = Settings.settings["color1"];
            var color2 = Settings.settings["color2"];

            var zoomWidth = TextWidth("Zoom: ", 24);

            GL.Color3(color1);
            RenderText("Zoom: ", OptionsNav.rect.Right + 10f, OptionsNav.rect.Y, 24);
            GL.Color3(color2);
            RenderText($"{Math.Round(editor.zoom, 2) * 100}%", OptionsNav.rect.Right + zoomWidth + 10f, OptionsNav.rect.Y, 24);

            GL.Color3(color1);

            var trackHeight = 64f + Settings.settings["trackHeight"].Value;
            var approachRate = Settings.settings["approachRate"].Value + 1f;
            var playerApproachRate = Settings.settings["playerApproachRate"].Value + 1f;

            if (Settings.settings["separateClickTools"])
                RenderText($"Click Mode: {(Settings.settings["selectTool"] ? "Select" : "Place")}", Grid.rect.X, BackButton.rect.Bottom + 10f, 24);

            switch  (navEnabled)
            {
                case "Options":
                    var trackHeightWidth = TextWidth("Track Height: 00", textScale);
                    var approachRateWidth = TextWidth("Approach Rate: 00", textScale);

                    RenderText($"Track Height: {trackHeight}", TrackHeight.rect.X - trackHeightWidth, SeparateClickTools.rect.Bottom + 10 * heightdiff, textScale);
                    RenderText($"Cursor Pos: {Math.Round(Settings.settings["cursorPos"].Value)}%", TrackCursorPos.rect.X, SeparateClickTools.rect.Bottom + 10 * heightdiff, textScale);
                    RenderText($"Approach Rate: {approachRate}", ApproachRate.rect.X - approachRateWidth, Quantum.rect.Y + 10 * heightdiff, textScale);
                    break;

                case "Timing":
                    RenderText("Export Offset[ms]:", ExportOffset.rect.X, ExportOffset.rect.Y - 24f, 24);
                    RenderText("SFX Offset[ms]:", SfxOffset.rect.X, SfxOffset.rect.Y - 24f, 24);
                    break;

                case "Patterns":
                    RenderText("Draw Bezier with Divisor:", BezierBox.rect.X, BezierBox.rect.Y - 24f, 24);
                    RenderText("Rotate by Degrees:", RotateBox.rect.X, RotateBox.rect.Y - 24f, 24);
                    RenderText("Scale by Percent:", ScaleBox.rect.X, ScaleBox.rect.Y - 24f, 24);
                    break;

                case "Player":
                    RenderText("Camera Mode:", CameraMode.rect.X, CameraMode.rect.Y - 24f, 24);
                    RenderText("Sensitivity:", Sensitivity.rect.X, Sensitivity.rect.Y - 24f, 24);
                    RenderText("Parallax:", Parallax.rect.X, Parallax.rect.Y - 24f, 24);
                    RenderText("Approach Distance:", ApproachDistance.rect.X, ApproachDistance.rect.Y - 24f, 24);
                    RenderText($"Player Approach Rate: {playerApproachRate}", PlayerApproachRate.rect.X, PlayerApproachRate.rect.Y - 24f, 24);
                    break;
            }

            var divisor = $"Beat Divisor: {Math.Round(Settings.settings["beatDivisor"].Value * 10) / 10 + 1f}";
            var divisorWidth = TextWidth(divisor, 24);
            var snapping = $"Snapping: 3/{Settings.settings["quantumSnapping"].Value + 3f}";
            var snappingWidth = TextWidth(snapping, 24);

            RenderText(divisor, BeatSnapDivisor.rect.X + BeatSnapDivisor.rect.Width / 2f - divisorWidth / 2f, BeatSnapDivisor.rect.Y - 20f, 24);
            RenderText(snapping, QuantumSnapDivisor.rect.X + QuantumSnapDivisor.rect.Width / 2f - snappingWidth / 2f, QuantumSnapDivisor.rect.Y - 20f, 24);

            var tempo = $"PLAYBACK SPEED - {Math.Round(editor.tempo * 100f)}%";
            var tempoWidth = TextWidth(tempo, 24);

            RenderText(tempo, Tempo.rect.X + Tempo.rect.Width / 2f - tempoWidth / 2f, Tempo.rect.Bottom - 24f, 24);

            var musicWidth = TextWidth("Music", 18);
            var sfxWidth = TextWidth("SFX", 18);
            var musicVal = (Math.Round(Settings.settings["masterVolume"].Value * 100f)).ToString();
            var sfxVal = (Math.Round(Settings.settings["sfxVolume"].Value * 100f)).ToString();
            var musicValWidth = TextWidth(musicVal, 18);
            var sfxValWidth = TextWidth(sfxVal, 18);

            RenderText("Music", MasterVolume.rect.X + MasterVolume.rect.Width / 2f - musicWidth / 2f, MasterVolume.rect.Y - 2f, 18);
            RenderText(musicVal, MasterVolume.rect.X + MasterVolume.rect.Width / 2f - musicValWidth / 2f, MasterVolume.rect.Bottom - 16f, 18);
            RenderText("SFX", SfxVolume.rect.X + SfxVolume.rect.Width / 2f - sfxWidth / 2f, SfxVolume.rect.Y - 2f, 18);
            RenderText(sfxVal, SfxVolume.rect.X + SfxVolume.rect.Width / 2f - sfxValWidth / 2f, SfxVolume.rect.Bottom - 16f, 18);

            var currentTime = Settings.settings["currentTime"];
            var progress = currentTime.Value / currentTime.Max;

            var current = $"{(int)(currentTime.Value / 60000f)}:{(int)(currentTime.Value % 60000 / 1000f):0#}";
            var currentWidth = TextWidth(current, 20);
            var currentMs = (long)currentTime.Value == 0 ? "0ms" : $"{(long)currentTime.Value:##,###}ms";
            var currentMsWidth = TextWidth(currentMs, 20);

            var total = $"{(int)(currentTime.Max / 60000f)}:{(int)(currentTime.Max % 60000 / 1000f):0#}";
            var totalWidth = TextWidth(total, 20);

            var notes = $"{editor.Notes.Count} Notes";
            var notesWidth = TextWidth(notes, 24);

            RenderText(current, Timeline.rect.X + Timeline.rect.Height / 2f - currentWidth / 2f, Timeline.rect.Bottom - 24f, 20);
            RenderText(currentMs, Timeline.rect.X + Timeline.rect.Height / 2f + (Timeline.rect.Width - Timeline.rect.Height) * progress - currentMsWidth / 2f, Timeline.rect.Y, 20);
            RenderText(total, Timeline.rect.X - Timeline.rect.Height / 2f - totalWidth / 2f + Timeline.rect.Width, Timeline.rect.Bottom - 24f, 20);
            RenderText(notes, Timeline.rect.X + Timeline.rect.Width / 2f - notesWidth / 2f, Timeline.rect.Bottom - 24f, 24);

            //bezier preview
            GL.LineWidth(2f);

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
                OnResize(new Size((int)rect.Width, (int)rect.Height));
                started = true;
            }
        }

        protected override void OnButtonClicked(int id)
        {
            var editor = MainWindow.Instance;
            var currentTime = Settings.settings["currentTime"];

            switch (id)
            {
                case 0:
                    try
                    {
                        Clipboard.SetText(editor.ParseData(Settings.settings["correctOnCopy"]));
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
                    ExportOffset.text = ((long)currentTime.Value).ToString();
                    Settings.settings["exportOffset"] = currentTime.Value;

                    break;

                case 6:
                    if (TimingsWindow.Instance != null)
                        TimingsWindow.Instance.Close();
                    if (editor.IsFullscreen)
                        editor.ToggleFullscreen();

                    new TimingsWindow().Show();

                    break;

                case 7:
                    if (BookmarksWindow.Instance != null)
                        BookmarksWindow.Instance.Close();
                    if (editor.IsFullscreen)
                        editor.ToggleFullscreen();

                    new BookmarksWindow().Show();

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

                    if (float.TryParse(RotateBox.text, out var deg) && selectedR.Count > 0)
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

                    if (float.TryParse(ScaleBox.text, out var scale) && selectedS.Count > 0)
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

                    if (File.Exists("SSQE Player.exe"))
                    {
                        if (!Directory.Exists("assets/temp"))
                            Directory.CreateDirectory("assets/temp");

                        Settings.Save();

                        File.WriteAllText($"assets/temp/tempmap.txt", editor.ParseData());
                        Process.Start("SSQE Player.exe", Settings.settings["fromStart"].ToString());
                    }

                    break;
            }
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            if (Timeline.hoveringBookmark != null && !right)
                Settings.settings["currentTime"].Value = Timeline.hoveringBookmark.Ms;

            base.OnMouseClick(pos, right);
        }

        private void UpdateNav()
        {
            var optionsNav = navEnabled == "Options";
            var timingNav = navEnabled == "Timing";
            var patternsNav = navEnabled == "Patterns";
            var playerNav = navEnabled == "Player";

            OptionsNav.text = $"OPTIONS {(optionsNav ? "<" : ">")}";
            TimingNav.text = $"TIMING {(timingNav ? "<" : ">")}";
            PatternsNav.text = $"PATTERNS {(patternsNav ? "<" : ">")}";
            PlayerNav.text = $"PLAYER {(playerNav ? "<" : ">")}";

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
            TrackHeight.Visible = optionsNav;
            TrackCursorPos.Visible = optionsNav;
            ApproachRate.Visible = optionsNav;

            ExportOffset.Visible = timingNav;
            SfxOffset.Visible = timingNav;
            UseCurrentMs.Visible = timingNav;
            OpenTimings.Visible = timingNav;
            OpenBookmarks.Visible = timingNav;
            ImportIni.Visible = timingNav;

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

            CameraMode.Visible = playerNav;
            LockCursor.Visible = playerNav;
            Sensitivity.Visible = playerNav;
            Parallax.Visible = playerNav;
            ApproachDistance.Visible = playerNav;
            PlayerApproachRate.Visible = playerNav;
            FromStart.Visible = playerNav;
            PlayMap.Visible = playerNav;

            OnResize(new Size((int)rect.Width, (int)rect.Height));
        }

        public override void OnResize(Size size)
        {
            rect = new RectangleF(0, 0, size.Width, size.Height);

            base.OnResize(size);

            PlayerNav.Visible = File.Exists("SSQE Player.exe");

            var heightdiff = size.Height / 1080f;

            switch (navEnabled)
            {
                case "Options":
                    TimingNav.rect.Y = TrackCursorPos.rect.Bottom + 20 * heightdiff;
                    PatternsNav.rect.Y = TimingNav.rect.Bottom + 10 * heightdiff;
                    PlayerNav.rect.Y = PatternsNav.rect.Bottom + 10 * heightdiff;
                    break;

                case "Timing":
                    PatternsNav.rect.Y = ImportIni.rect.Bottom + 20 * heightdiff;
                    PlayerNav.rect.Y = PatternsNav.rect.Bottom + 10 * heightdiff;
                    break;

                case "Patterns":
                    PlayerNav.rect.Y = ScaleButton.rect.Bottom + 20 * heightdiff;
                    break;
            }


            CopyButton.rect.Location = new PointF(Grid.rect.X, Grid.rect.Y - 42 - 75 * heightdiff);
            BackButton.rect.Location = new PointF(Grid.rect.X, Grid.rect.Bottom + 84 * heightdiff);

            Timeline.rect = new RectangleF(0, rect.Height - 64f, rect.Width - 576f, 64f);
            PlayPause.rect = new RectangleF(rect.Width - 576f, rect.Height - 64f, 64f, 64f);
            Tempo.rect = new RectangleF(rect.Width - 512f, rect.Height - 64f, 512f, 64f);

            AutoAdvance.textSize = AutoAdvance.originTextSize;
            AutoAdvance.rect = new RectangleF(rect.Width - 236f, Grid.rect.Y - 70f, 40f, 40f);
            BeatSnapDivisor.rect = new RectangleF(rect.Width - 256f, Grid.rect.Y + 28f, 256f, 40f);
            QuantumSnapDivisor.rect = new RectangleF(rect.Width - 256f, Grid.rect.Y + 100f, 256f, 40f);
            MasterVolume.rect = new RectangleF(rect.Width - 64f, rect.Height - 320f, 40f, 256f);
            SfxVolume.rect = new RectangleF(rect.Width - 128f, rect.Height - 320f, 40f, 256f);
        }

        public void ShowToast(string text, Color color)
        {
            toastTime = 0f;

            ToastLabel.text = text;
            ToastLabel.color = color;
        }
    }
}