using System.Diagnostics;
using System.Drawing;
using OpenTK.Mathematics;
using New_SSQE.Objects;
using SkiaSharp;
using New_SSQE.GUI.Font;
using New_SSQE.Audio;
using New_SSQE.FileParsing;
using New_SSQE.Objects.Other;
using New_SSQE.Preferences;
using New_SSQE.Objects.Managers;
using New_SSQE.Misc.Easing;
using New_SSQE.Misc.Dialogs;
using New_SSQE.Misc.Static;
using New_SSQE.ExternalUtils;
using New_SSQE.Maps;

namespace New_SSQE.GUI
{
    internal class GuiWindowEditor : GuiWindow
    {
        // options nav
        private readonly GuiButton LNavOptions = new(10, 60, 175, 50, 3, "OPTIONS", 31, false, true);
        private readonly GuiCheckbox Numpad = new(10, 120, 30, 30, Settings.numpad, "Use Numpad", 26, false, true);
        private readonly GuiCheckbox SeparateClickTools = new(10, 160, 30, 30, Settings.separateClickTools, "Separate Click Modes", 26, false, true);
        private readonly GuiButton SwapClickMode = new(10, 200, 200, 40, 27, "Swap Click Mode", 26, false, true);
        private readonly GuiCheckbox JumpOnPaste = new(10, 250, 30, 30, Settings.jumpPaste, "Jump on Paste", 26, false, true);
        private readonly GuiCheckbox PauseOnScroll = new(10, 290, 30, 30, Settings.pauseScroll, "Pause on Seek", 26, false, true);
        private readonly GuiButton EditMapVFX = new(10, 350, 200, 40, 30, "Edit Map VFX", 26, false, true);
        private readonly GuiButton EditSpecial = new(10, 400, 200, 40, 45, "Edit Extra Objects", 26, false, true);

        // timing nav
        private readonly GuiButton LNavTiming = new(195, 60, 175, 50, 4, "TIMING", 31, false, true);
        private readonly GuiTextbox ExportOffset = new(10, 160, 130, 40, 31, Settings.exportOffset, false, false, true);
        private readonly GuiTextbox SfxOffset = new(180, 160, 130, 40, 31, Settings.sfxOffset, false, false, true);
        private readonly GuiButton OpenTimings = new(10, 220, 210, 40, 6, "OPEN BPM SETUP", 27, false, true);
        private readonly GuiButton ImportIni = new(10, 270, 210, 40, 16, "IMPORT INI", 27, false, true);
        private readonly GuiCheckbox Metronome = new(10, 320, 30, 30, Settings.metronome, "Metronome", 26, false, true);
        private readonly GuiButton OpenBookmarks = new(10, 390, 210, 40, 7, "EDIT BOOKMARKS", 27, false, true);
        private readonly GuiButton CopyBookmarks = new(10, 440, 210, 40, 20, "COPY BOOKMARKS", 27, false, true);
        private readonly GuiButton PasteBookmarks = new(10, 490, 210, 40, 21, "PASTE BOOKMARKS", 27, false, true);

        private readonly GuiLabel ExportOffsetLabel = new(10, 130, 100, 30, "Export Offset:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel SfxOffsetLabel = new(180, 130, 100, 30, "SFX Offset:", 30, false, true, "main", false, Settings.color1);

        // patterns nav
        private readonly GuiButton LNavPatterns = new(380, 60, 175, 50, 8, "PATTERNS", 31, false, true);
        private readonly GuiButton HFlip = new(10, 130, 175, 40, 9, "HORIZONTAL FLIP", 27, false, true);
        private readonly GuiButton VFlip = new(195, 130, 175, 40, 10, "VERTICAL FLIP", 27, false, true);
        public readonly GuiTextbox RotateBox = new(10, 220, 100, 40, "90", 31, true);
        private readonly GuiButton RotateButton = new(120, 220, 100, 40, 14, "ROTATE", 27, false, true);
        public readonly GuiTextbox ScaleBox = new(10, 300, 100, 40, "150", 31, true);
        private readonly GuiButton ScaleButton = new(120, 300, 100, 40, 15, "SCALE", 27, false, true);
        private readonly GuiCheckbox ApplyOnPaste = new(10, 360, 30, 30, Settings.applyOnPaste, "Apply Rotate/Scale On Paste", 27, false, true);
        private readonly GuiCheckbox ClampSR = new(10, 400, 30, 30, Settings.clampSR, "Clamp Rotate/Scale In Bounds", 27, false, true);
        private readonly GuiCheckbox PasteReversed = new(10, 440, 30, 30, Settings.pasteReversed, "Paste Reversed", 27, false, true);
        private readonly GuiButton StoreNodes = new(10, 500, 175, 40, 11, "STORE NODES", 27, false, true);
        private readonly GuiButton ClearNodes = new(195, 500, 175, 40, 12, "CLEAR NODES", 27, false, true);
        private readonly GuiCheckbox CurveBezier = new(10, 560, 30, 30, Settings.curveBezier, "Curve Bezier", 27, false, true);
        private readonly GuiTextbox BezierBox = new(10, 630, 100, 40, 31, Settings.bezierDivisor, false, true, true);
        private readonly GuiButton BezierButton = new(120, 630, 100, 40, 13, "DRAW", 27, false, true);

        private readonly GuiLabel RotateLabel = new(10, 190, 175, 30, "Rotate by Degrees:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel ScaleLabel = new(10, 270, 100, 30, "Scale by Percent:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel DrawBezierLabel = new(10, 600, 100, 30, "Draw Bezier with Divisor:", 30, false, true, "main", false, Settings.color1);

        // player nav
        private readonly GuiButton LNavPlayer = new(10, 0, 545, 50, 17, "PLAYTEST", 31);
        private readonly GuiButtonList CameraMode = new(10, 160, 150, 40, Settings.cameraMode, 27, false, true);
        private readonly GuiTextbox NoteScale = new(185, 160, 100, 40, 31, Settings.noteScale, true, false, true);
        private readonly GuiTextbox CursorScale = new(310, 160, 100, 40, 31, Settings.cursorScale, true, false, true);
        private readonly GuiCheckbox LockCursor = new(10, 220, 30, 30, Settings.lockCursor, "Lock Cursor Within Grid", 27, false, true);
        private readonly GuiCheckbox GridGuides = new(10, 270, 30, 30, Settings.gridGuides, "Grid Guides", 27, false, true);
        private readonly GuiTextbox Sensitivity = new(10, 350, 115, 40, 31, Settings.sensitivity, true, false, true);
        private readonly GuiTextbox Parallax = new(145, 350, 115, 40, 31, Settings.parallax, true, false, true);
        private readonly GuiTextbox FieldOfView = new(280, 350, 115, 40, 31, Settings.fov, true, false, true);
        private readonly GuiTextbox ApproachDistance = new(10, 435, 150, 40, 31, Settings.approachDistance, true, false, true);
        private readonly GuiTextbox HitWindow = new(245, 435, 150, 40, 31, Settings.hitWindow, true, false, true);
        private readonly GuiSlider PlayerApproachRate = new(10, 520, 400, 32, Settings.playerApproachRate, false, false, true);
        private readonly GuiCheckbox ApproachFade = new(10, 570, 30, 30, Settings.approachFade, "Approach Fade", 27, false, true);
        private readonly GuiButton FromStart = new(10, 630, 200, 40, 18, "PLAY FROM START", 27, false, true);
        private readonly GuiButton PlayMap = new(220, 630, 200, 40, 22, "PLAY HERE", 27, false, true);

        private readonly GuiLabel CameraModeLabel = new(10, 130, 100, 30, "Camera Mode:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel NoteScaleLabel = new(185, 130, 100, 30, "Note Size:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel CursorScaleLabel = new(310, 130, 100, 30, "Cursor Size:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel SensitivityLabel = new(10, 320, 100, 30, "Sensitivity:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel ParallaxLabel = new(145, 320, 100, 30, "Parallax:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel FieldOfViewLabel = new(280, 320, 100, 30, "FOV:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel ApproachDistanceLabel = new(10, 405, 100, 30, "Approach Distance:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel HitWindowLabel = new(245, 405, 100, 30, "Hit Window:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiLabel PlayerApproachRateLabel = new(10, 490, 400, 32, "", 30, false, true, "main", true, Settings.color1);
        
        // snapping nav
        private readonly GuiButton RNavSnapping = new(1365, 60, 175, 50, 26, "SNAPPING", 31, false, true);
        private readonly GuiCheckbox Quantum = new(1610, 120, 30, 30, Settings.enableQuantum, "Quantum", 27, false, true);
        private readonly GuiCheckbox QuantumGridSnap = new(1610, 160, 30, 30, Settings.quantumGridSnap, "Snap to Grid", 27, false, true);
        private readonly GuiCheckbox AutoAdvance = new(1610, 200, 30, 30, Settings.autoAdvance, "Auto-Advance", 27, false, true);
        private readonly GuiSlider BeatSnapDivisor = new(1610, 280, 250, 32, Settings.beatDivisor, false, false, true);
        private readonly GuiSlider QuantumSnapDivisor = new(1610, 360, 250, 32, Settings.quantumSnapping, false, false, true);

        private readonly GuiLabel BeatDivisorLabel = new(1610, 250, 250, 32, "", 30, false, true, "main", true, Settings.color1);
        private readonly GuiLabel SnappingLabel = new(1610, 330, 250, 32, "", 30, false, true, "main", true, Settings.color1);

        // graphics nav
        private readonly GuiButton RNavGraphics = new(1550, 60, 175, 50, 28, "GRAPHICS", 31, false, true);
        private readonly GuiCheckbox Autoplay = new(1610, 120, 30, 30, Settings.autoplay, "Autoplay", 26, false, true);
        private readonly GuiCheckbox ApproachSquares = new(1610, 160, 30, 30, Settings.approachSquares, "Approach Squares", 26, false, true);
        private readonly GuiCheckbox GridNumbers = new(1610, 200, 30, 30, Settings.gridNumbers, "Grid Numbers", 26, false, true);
        private readonly GuiCheckbox GridLetters = new(1610, 240, 30, 30, Settings.gridLetters, "Grid Letters", 26, false, true);
        private readonly GuiCheckbox QuantumGridLines = new(1610, 280, 30, 30, Settings.quantumGridLines, "Quantum Grid Lines", 26, false, true);
        private readonly GuiSlider ApproachRate = new(1610, 360, 250, 32, Settings.approachRate, false, false, true);
        private readonly GuiSlider TrackHeight = new(1610, 440, 250, 32, Settings.trackHeight, false, false, true);
        private readonly GuiSlider TrackCursorPos = new(1610, 520, 250, 32, Settings.cursorPos, false, false, true);

        private readonly GuiLabel ApproachRateLabel = new(1610, 330, 250, 32, "", 28, false, true, "main", true, Settings.color1);
        private readonly GuiLabel TrackHeightLabel = new(1610, 410, 250, 32, "", 28, false, true, "main", true, Settings.color1);
        private readonly GuiLabel CursorPosLabel = new(1610, 490, 250, 32, "", 28, false, true, "main", true, Settings.color1);

        // export nav
        private readonly GuiButton RNavExport = new(1735, 60, 175, 50, 5, "EXPORT", 31, false, true);
        private readonly GuiButton SaveButton = new(1610, 130, 100, 40, 24, "SAVE", 27, false, true);
        private readonly GuiButton SaveAsButton = new(1720, 130, 100, 40, 25, "SAVE AS", 27, false, true);
        private readonly GuiButtonList ExportSwitch = new(1590, 200, 250, 40, Settings.exportType, 27, false, true);
        private readonly GuiButton ExportButton = new(1610, 250, 210, 40, 44, "EXPORT MAP", 27, false, true);
        private readonly GuiTextbox ReplaceIDBox = new(1610, 340, 210, 40, 27, true);
        private readonly GuiButton ReplaceID = new(1610, 390, 210, 40, 29, "REPLACE", 27, false, true);
        private readonly GuiButton ConvertAudio = new(1610, 460, 210, 40, 49, "CONVERT TO MP3", 27, false, true);

        private readonly GuiLabel ReplaceIDLabel = new(1610, 300, 210, 40, "Replace Audio ID", 30, false, true, "main", true, Settings.color1);

        // vfx
        private readonly GuiButton ExitMapVFX = new(10, 60, 545, 50, 31, "CLOSE MAP VFX", 31, false, true) { Visible = false };
        private readonly GuiButton NavBrightness = new(10, 120, 150, 40, 32, "Brightness", 27, false, true) { Visible = false };
        private readonly GuiButton NavContrast = new(10, 170, 150, 40, 33, "Contrast", 27, false, true) { Visible = false };
        private readonly GuiButton NavSaturation = new(10, 220, 150, 40, 34, "Saturation", 27, false, true) { Visible = false };
        private readonly GuiButton NavBlur = new(10, 270, 150, 40, 35, "Blur", 27, false, true) { Visible = false };
        private readonly GuiButton NavFOV = new(10, 320, 150, 40, 36, "FOV", 27, false, true) { Visible = false };
        private readonly GuiButton NavTint = new(10, 370, 150, 40, 37, "Tint", 27, false, true) { Visible = false };
        private readonly GuiButton NavPosition = new(10, 420, 150, 40, 38, "Position", 27, false, true) { Visible = false };
        private readonly GuiButton NavRotation = new(10, 470, 150, 40, 39, "Rotation", 27, false, true) { Visible = false };
        private readonly GuiButton NavARFactor = new(10, 520, 150, 40, 40, "AR Factor", 27, false, true) { Visible = false };
        private readonly GuiButton NavText = new(10, 570, 150, 40, 41, "Text", 27, false, true) { Visible = false };

        private readonly GuiLabel VFXDurationLabel = new(185, 125, 150, 30, "Duration:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiTextbox VFXDuration = new(355, 120, 150, 40, 31, Settings.vfxDuration, false, true, true);
        private readonly GuiLabel VFXStyleLabel = new(185, 175, 150, 30, "Easing Style:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiButtonList VFXStyle = new(185, 210, 150, 40, Settings.vfxStyle, 27, false, true);
        private readonly GuiLabel VFXDirectionLabel = new(355, 175, 150, 30, "Easing Direction:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiButtonList VFXDirection = new(355, 210, 150, 40, Settings.vfxDirection, 27, false, true);
        private readonly GuiLabel VFXIntensityLabel = new(185, 260, 320, 32, "", 30, false, true, "main", true, Settings.color1);
        private readonly GuiSlider VFXIntensity = new(185, 290, 320, 32, Settings.vfxIntensity, false, false, true);
        private readonly GuiLabel VFXFOVLabel = new(185, 265, 150, 30, "FOV:", 30, false, true, "main", true, Settings.color1);
        private readonly GuiTextbox VFXFOV = new(355, 260, 150, 40, 31, Settings.vfxFOV, true, true, true);
        private readonly GuiButton VFXColor = new(185, 260, 150, 40, 43, "Color", 27, false, true);
        private readonly GuiSquare VFXColorSquare = new(355, 260, 150, 40, Color.Black, false, "", "", true);
        private readonly GuiLabel VFXVectorLabel = new(185, 260, 150, 30, "Vector:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiTextbox VFXVectorX = new(185, 295, 100, 40, 31, Settings.vfxVectorX, true, false, true);
        private readonly GuiTextbox VFXVectorY = new(295, 295, 100, 40, 31, Settings.vfxVectorY, true, false, true);
        private readonly GuiTextbox VFXVectorZ = new(405, 295, 100, 40, 31, Settings.vfxVectorZ, true, false, true);
        private readonly GuiLabel VFXDegreesLabel = new(185, 265, 150, 30, "Degrees:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiTextbox VFXDegrees = new(355, 260, 150, 40, 31, Settings.vfxDegrees, false, false, true);
        private readonly GuiLabel VFXFactorLabel = new(185, 125, 150, 30, "Factor:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiTextbox VFXFactor = new(355, 120, 150, 40, 31, Settings.vfxFactor, true, true, true);
        private readonly GuiLabel VFXStringLabel = new(185, 175, 150, 30, "String:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiTextbox VFXString = new(185, 210, 320, 40, "", 31, false, true, "main", null, null, Settings.vfxString);
        private readonly GuiLabel VFXStrengthLabel = new(185, 260, 150, 30, "Visibility:", 30, false, true, "main", false, Settings.color1);
        private readonly GuiButtonList VFXStrength = new(185, 295, 100, 40, Settings.vfxStrength, 31, false, true);
        private readonly GuiLabel VFXToast = new(0, 0, 301, 42, "", 40, true) { Color = Settings.color1.Value };

        private readonly GuiButton VFXApply = new(185, 370, 320, 40, 42, "", 31, false, true);

        // special objects
        private readonly GuiButton ExitSpecial = new(10, 60, 545, 50, 46, "CLOSE EXTRA OBJECTS", 31, false, true) { Visible = false };
        private readonly GuiButton NavBeat = new(10, 120, 150, 40, 47, $"[{Settings.extrasBeat.Value.Key}] Beat", 27, false, true) { Visible = false };


        private readonly GuiButtonList GameSwitch = new(10, 0, 545, 50, Settings.modchartGame, 31, 48) { Visible = false };

        private readonly GuiButton CopyButton = new(0, 0, 301, 42, 0, "COPY MAP DATA", 27, true);
        private readonly GuiButton BackButton = new(0, 0, 301, 42, 1, "BACK TO MENU", 27, true);

        private readonly GuiSlider Tempo = new(Settings.tempo, false);
        private readonly GuiSlider MasterVolume = new(Settings.masterVolume, true);
        private readonly GuiSlider SfxVolume = new(Settings.sfxVolume, true);
        public readonly GuiSliderTimeline Timeline = new();
        private readonly GuiButtonPlayPause PlayPause = new(2);



        private readonly GuiLabel ToastLabel = new(42);

        private readonly GuiLabel ZoomLabel = new(565, 60, 80, 30, "Zoom: ", 32, false, true, "main", false, Settings.color1);
        private readonly GuiLabel ZoomValueLabel = new(640, 60, 80, 30, "", 32, false, true, "main", false, Settings.color2);
        private readonly GuiLabel ClickModeLabel = new(0, 0, 301, 42, "", 30, true, false, "main", false, Settings.color1);

        private readonly GuiLabel TempoLabel = new(0, 0, 0, 30, "", 30, true, false, "main", true, Settings.color1);
        private readonly GuiLabel MusicLabel = new(0, 0, 0, 30, "Music", 24, true, false, "main", true, Settings.color1);
        private readonly GuiLabel MusicValueLabel = new(0, 0, 0, 30, "", 24, true, false, "main", true, Settings.color1);
        private readonly GuiLabel SfxLabel = new(0, 0, 0, 30, "SFX", 24, true, false, "main", true, Settings.color1);
        private readonly GuiLabel SfxValueLabel = new(0, 0, 0, 30, "", 24, true, false, "main", true, Settings.color1);

        private readonly GuiLabel CurrentTimeLabel = new(0, 0, 0, 30, "", 26, true, false, "main", true, Settings.color1);
        private readonly GuiLabel CurrentMsLabel = new(0, 0, 0, 30, "", 26, true, false, "main", true, Settings.color1);
        private readonly GuiLabel TotalTimeLabel = new(0, 0, 0, 30, "", 26, true, false, "main", true, Settings.color1);
        private readonly GuiLabel NotesLabel = new(0, 0, 0, 30, "", 30, true, false, "main", true, Settings.color1);

        private float toastTime = 0f;
        private static string leftNav = "Timing";
        private static string rightNav = "Snapping";
        private bool started = false;

        private static string vfxNav = "";
        private static string specNav = "";

        private static string storedLeft = "";

        private readonly bool rhythia = false;
        private readonly bool playerWasVisible = false;

        public readonly GuiSquare[] IconSet = new GuiSquare[10];

        public GuiWindowEditor() : base(0, 0, MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y)
        {
            GuiTrack.RenderMapObjects = false;
            GuiGrid.RenderMapObjects = false;

            rhythia = Settings.useRhythia.Value;
            if (rhythia && leftNav == "Player")
                leftNav = "Timing";

            vfxNav = "";

            Controls = new List<WindowControl>
            {
                // Squares
                VFXColorSquare,
                // Buttons
                CopyButton, BackButton, SaveButton, PlayPause, LNavOptions, LNavTiming, OpenTimings, ImportIni, LNavPatterns, HFlip, VFlip, StoreNodes, ClearNodes,
                BezierButton, RotateButton, ScaleButton, RNavExport, OpenBookmarks, CopyBookmarks, PasteBookmarks, LNavPlayer, CameraMode, PlayMap, ExportButton,
                FromStart, RNavGraphics, RNavSnapping, SwapClickMode, SaveAsButton, ReplaceID, /*EditMapVFX,*/ ExitMapVFX, ConvertAudio,
                NavBrightness, NavContrast, NavSaturation, NavBlur, NavFOV, NavTint, NavPosition, NavRotation, NavARFactor, NavText,
                VFXStyle, VFXDirection, VFXColor, VFXApply, ExportSwitch, EditSpecial, ExitSpecial, NavBeat, GameSwitch, VFXStrength,
                // Checkboxes
                AutoAdvance, Autoplay, ApproachSquares, GridNumbers, GridLetters, Quantum, Numpad, QuantumGridLines, QuantumGridSnap, Metronome, SeparateClickTools, JumpOnPaste,
                CurveBezier, ApplyOnPaste, LockCursor, ApproachFade, GridGuides, PauseOnScroll, ClampSR, PasteReversed,
                // Sliders
                Tempo, MasterVolume, SfxVolume, BeatSnapDivisor, QuantumSnapDivisor, Timeline, TrackHeight, TrackCursorPos, ApproachRate, PlayerApproachRate,
                VFXIntensity,
                // Boxes
                ExportOffset, SfxOffset, BezierBox, RotateBox, ScaleBox, NoteScale, CursorScale, Sensitivity, Parallax, FieldOfView, ApproachDistance, HitWindow, ReplaceIDBox,
                VFXDuration, VFXVectorX, VFXVectorY, VFXVectorZ, VFXDegrees, VFXFactor, VFXString, VFXFOV,
                // Labels
                ZoomLabel, ZoomValueLabel, ClickModeLabel, BeatDivisorLabel, SnappingLabel, TempoLabel, MusicLabel, MusicValueLabel, SfxLabel, SfxValueLabel, CurrentTimeLabel,
                CurrentMsLabel, TotalTimeLabel, NotesLabel, TrackHeightLabel, CursorPosLabel, ApproachRateLabel, ExportOffsetLabel, SfxOffsetLabel, DrawBezierLabel, RotateLabel,
                ScaleLabel, CameraModeLabel, NoteScaleLabel, CursorScaleLabel, SensitivityLabel, ParallaxLabel, FieldOfViewLabel, ReplaceIDLabel,
                ApproachDistanceLabel, PlayerApproachRateLabel, HitWindowLabel, ToastLabel,
                VFXDurationLabel, VFXStyleLabel, VFXDirectionLabel, VFXIntensityLabel, VFXFOVLabel, VFXVectorLabel,
                VFXDegreesLabel, VFXFactorLabel, VFXStringLabel, VFXToast, VFXStrengthLabel
            };

            using FileStream fs = File.OpenRead($"{Assets.TEXTURES}\\Sprites.png");
            SKBitmap img = SKBitmap.Decode(fs);

            Vector2 size = (img.Width / MainWindow.SpriteSize.X, img.Height / MainWindow.SpriteSize.Y);
            SKImageInfo info = new((int)size.X, (int)size.Y);
            SKSurface surface = SKSurface.Create(info);
            SKCanvas canvas = surface.Canvas;

            for (int i = 0; i < IconSet.Length; i++)
            {
                int index = i + 2;

                int x = -(int)(index % MainWindow.SpriteSize.X * size.X);
                int y = -(int)(index / MainWindow.SpriteSize.X * size.Y);

                canvas.Clear();
                canvas.DrawBitmap(img, x, y);
                SKBitmap cur = SKBitmap.FromImage(surface.Snapshot());

                TextureManager.GetOrRegister($"icon{index}", cur);
                GuiSquare square = new(10 + 20 * i, 620, 20, 20, Color.White, false, "", $"icon{index}", true);

                IconSet[i] = square;
                Controls.Add(square);
            }

            LNavPlayer.Visible = Platform.ExecutableExists("SSQE Player") || Settings.useRhythia.Value;
            playerWasVisible = LNavPlayer.Visible;

            BackgroundSquare = new(Color.FromArgb((int)Settings.editorBGOpacity.Value, 30, 30, 30), "background_editor.png", "editorbg");
            Track = new();
            Grid = new(300, 300);

            YOffset = Settings.trackHeight.Value.Value + 64;
            Init();

            UpdateNav();
            UpdateVFX();
            started = false;
        }

        private void ShowBezier(List<Note> nodes, int divisor)
        {
            try
            {
                List<Note> final = Patterns.DrawBezier(nodes, divisor);

                foreach (Note note in final)
                    Grid?.AddPreviewNote(note.X, note.Y, 2);
            }
            catch { CurrentMap.BezierNodes.Clear(); }
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            for (int i = 0; i < IconSet.Length; i++)
                IconSet[i].Visible = false;

            Grid?.ClearPreviewNotes();

            if (frametime < 2)
                toastTime = Math.Min(2, toastTime + frametime);

            float toastOffset = 1f;

            if (toastTime <= 0.5f)
                toastOffset = (float)Math.Sin(Math.Min(0.5f, toastTime) / 0.5f * MathHelper.PiOver2);
            if (toastTime >= 1.75f)
                toastOffset = (float)Math.Cos(Math.Min(0.25f, toastTime - 1.75f) / 0.25f * MathHelper.PiOver2);

            int toastHeight = FontRenderer.GetHeight(ToastLabel.TextSize, ToastLabel.Font);
            ToastLabel.Rect.Location = new PointF(Rect.X + Rect.Width / 2f, Rect.Height - toastOffset * toastHeight * 2.25f + toastHeight / 2f);
            ToastLabel.Color = Color.FromArgb((int)(Math.Pow(toastOffset, 3) * 255), ToastLabel.Color);

            ToastLabel.Update();

            SliderSetting currentTime = Settings.currentTime.Value;

            ZoomValueLabel.Text = $"{Math.Round(CurrentMap.Zoom * 100)}%";
            ClickModeLabel.Text = $"Click: {(Settings.selectTool.Value ? "Select" : "Place")}";
            ClickModeLabel.Visible = Settings.separateClickTools.Value && !GuiGrid.RenderMapObjects;

            TrackHeightLabel.Text = $"Track Height: {Math.Round(64f + Settings.trackHeight.Value.Value)}";
            CursorPosLabel.Text = $"Cursor Pos: {Math.Round(Settings.cursorPos.Value.Value)}%";
            ApproachRateLabel.Text = $"Approach Rate: {(int)(Settings.approachRate.Value.Value + 1.5f)}";
            PlayerApproachRateLabel.Text = $"Player Approach Rate: {(int)Math.Round(Settings.playerApproachRate.Value.Value) + 1}";

            BeatDivisorLabel.Text = $"Beat Divisor: {Math.Round(Settings.beatDivisor.Value.Value * 10) / 10 + 1f}";
            SnappingLabel.Text = $"Snapping: 3/{Math.Round(Settings.quantumSnapping.Value.Value) + 3}";
            TempoLabel.Text = $"PLAYBACK SPEED - {Math.Round(CurrentMap.Tempo * 100f)}%";
            MusicValueLabel.Text = Math.Round(Settings.masterVolume.Value.Value * 100f).ToString();
            SfxValueLabel.Text = Math.Round(Settings.sfxVolume.Value.Value * 100f).ToString();

            CurrentTimeLabel.Text = $"{(int)(currentTime.Value / 60000f)}:{(int)(currentTime.Value % 60000 / 1000f):0#}";
            TotalTimeLabel.Text = $"{(int)(currentTime.Max / 60000f)}:{(int)(currentTime.Max % 60000 / 1000f):0#}";

            string currentMs = $"{(long)currentTime.Value:##,##0}ms";
            float progress = currentTime.Value / currentTime.Max;
            CurrentMsLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Height / 2f + (Timeline.Rect.Width - Timeline.Rect.Height) * progress, Timeline.Rect.Y - 4f);
            CurrentMsLabel.Text = currentMs;

            VFXIntensityLabel.Text = $"Intensity: {Math.Round(Settings.vfxIntensity.Value.Value, 2)}";
            VFXColorSquare.Color = Settings.vfxColor.Value;
            VFXApply.Text = CurrentMap.VfxObjects.Selected.Count == 1 ? "MODIFY" : "CREATE";

            bool mapObj = GuiGrid.RenderMapObjects;
            bool vfx = GuiGrid.VFXObjects;

            if (BackgroundSquare != null)
                BackgroundSquare.Visible = !mapObj || !vfx;
            int objNum = mapObj ? (vfx ? CurrentMap.VfxObjects.Count : CurrentMap.SpecialObjects.Count) : CurrentMap.Notes.Count;
            NotesLabel.Text = $"{objNum} {(mapObj ? "Objects" : "Notes")}";

            //bezier preview
            float bezierDivisor = Settings.bezierDivisor.Value;

            if (bezierDivisor > 0 && CurrentMap.BezierNodes.Count > 1)
            {
                List<int> anchored = new() { 0 };

                for (int i = 0; i < CurrentMap.BezierNodes.Count; i++)
                    if (CurrentMap.BezierNodes[i].Anchored && !anchored.Contains(i))
                        anchored.Add(i);

                if (!anchored.Contains(CurrentMap.BezierNodes.Count - 1))
                    anchored.Add(CurrentMap.BezierNodes.Count - 1);

                for (int i = 1; i < anchored.Count; i++)
                {
                    List<Note> newnodes = new();

                    for (int j = anchored[i - 1]; j <= anchored[i]; j++)
                        newnodes.Add(CurrentMap.BezierNodes[j]);
                    ShowBezier(newnodes, (int)(bezierDivisor + 0.5f));
                }
            }

            if (!started)
            {
                OnResize(new((int)Rect.Width, (int)Rect.Height));
                started = true;
            }

            base.Render(mousex, mousey, frametime);
        }

        private bool playerRunning = false;

        public override void OnButtonClicked(int id)
        {
            MainWindow editor = MainWindow.Instance;
            SliderSetting currentTime = Settings.currentTime.Value;

            switch (id)
            {
                case 0:
                    try
                    {
                        if (editor.AltHeld)
                            Clipboard.SetText(Parser.SaveLegacy(CurrentMap.SoundID, Settings.correctOnCopy.Value));
                        else
                            Clipboard.SetText(Parser.Save(CurrentMap.SoundID, Settings.correctOnCopy.Value));
                        ShowToast("COPIED TO CLIPBOARD", Color.FromArgb(0, 255, 200));
                    }
                    catch (Exception ex) {
                        Logging.Register("Failed to copy", LogSeverity.WARN, ex);
                        ShowToast("FAILED TO COPY", Color.FromArgb(255, 200, 0));
                    }

                    break;

                case 1:
                    if (!string.IsNullOrWhiteSpace(storedLeft))
                        leftNav = storedLeft;
                    editor.SwitchWindow(new GuiWindowMenu());

                    break;

                case 2:
                    if (MusicPlayer.IsPlaying)
                        MusicPlayer.Pause();
                    else
                    {
                        if (currentTime.Value >= currentTime.Max - 1)
                            currentTime.Value = 0;
                        MusicPlayer.Play();
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
                    NoteManager.Edit("HORIZONTAL FLIP", Patterns.HorizontalFlip);

                    break;

                case 10:
                    NoteManager.Edit("VERTICAL FLIP", Patterns.VerticalFlip);

                    break;

                case 11:
                    if (CurrentMap.Notes.Selected.Count > 1)
                        CurrentMap.BezierNodes = CurrentMap.Notes.Selected.ToList();

                    break;

                case 12:
                    CurrentMap.BezierNodes.Clear();

                    break;

                case 13:
                    Patterns.RunBezier();

                    break;

                case 14:
                    if (float.TryParse(RotateBox.Text, out float deg))
                        NoteManager.Edit($"ROTATE {deg}", n => Patterns.Rotate(n, deg));

                    break;

                case 15:
                    if (float.TryParse(ScaleBox.Text, out float scale))
                        NoteManager.Edit($"SCALE {scale}%", n => Patterns.Scale(n, scale));

                    break;

                case 16:
                    MapManager.ImportProperties();

                    break;

                case 17:
                    if (rhythia)
                    {
                        if (!File.Exists(Settings.rhythiaPath.Value))
                        {
                            Logging.Register($"Invalid rhythia path - {Settings.rhythiaPath.Value} | {File.Exists(Settings.rhythiaPath.Value)}");
                            ShowToast("INVALID RHYTHIA PATH - CHECK SETTINGS", Settings.color1.Value);
                        }
                        else
                        {
                            try
                            {
                                if (MusicPlayer.IsPlaying)
                                    MusicPlayer.Pause();

                                Settings.Save();

                                File.WriteAllText($"{Assets.TEMP}\\tempmap.txt", Parser.SaveLegacy(CurrentMap.SoundID, false, false));

                                string[] args =
                                {
                                    $"--a=\"{Path.GetFullPath($"{Assets.CACHED}\\{CurrentMap.SoundID}.asset").Replace("\\", "/")}\"",
                                    $"--t=\"{Path.GetFullPath($"{Assets.TEMP}\\tempmap.txt").Replace("\\", "/")}\""
                                };

                                Process.Start(Settings.rhythiaPath.Value, string.Join(" ", args));
                            }
                            catch (Exception ex)
                            {
                                Logging.Register($"Failed to start Rhythia", LogSeverity.WARN, ex);
                                ShowToast("FAILED TO START RHYTHIA", Settings.color1.Value);
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
                    if (MusicPlayer.IsPlaying)
                        MusicPlayer.Pause();

                    if (!playerRunning && Platform.ExecutableExists("SSQE Player"))
                    {
                        Settings.Save();
                        bool replay = editor.CtrlHeld && editor.AltHeld && GetReplayFile();

                        File.WriteAllText($"{Assets.TEMP}\\tempmap.txt", Parser.SaveLegacy(CurrentMap.SoundID, false, false));

                        Process process = Platform.RunExecutable("SSQE Player", $"true {replay} {editor.AltHeld}");
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
                    CurrentMap.CopyBookmarks();

                    break;

                case 21:
                    CurrentMap.PasteBookmarks();

                    break;

                case 22:
                    if (MusicPlayer.IsPlaying)
                        MusicPlayer.Pause();

                    if (!playerRunning && Platform.ExecutableExists("SSQE Player"))
                    {
                        Settings.Save();
                        bool replay = editor.CtrlHeld && editor.AltHeld && GetReplayFile();

                        File.WriteAllText($"{Assets.TEMP}\\tempmap.txt", Parser.SaveLegacy(CurrentMap.SoundID, false, false));

                        Process process = Platform.RunExecutable("SSQE Player", $"false {replay} {editor.AltHeld}");
                        playerRunning = process != null;

                        if (process != null)
                        {
                            process.EnableRaisingEvents = true;
                            process.Exited += delegate { playerRunning = false; };
                        }
                    }

                    break;

                case 24:
                    if (MapManager.Save(true))
                        ShowToast("SAVED", Settings.color1.Value);

                    break;

                case 25:
                    if (MapManager.Save(true, true))
                        ShowToast("SAVED", Settings.color1.Value);

                    break;

                case 26:
                    rightNav = rightNav == "Snapping" ? "" : "Snapping";
                    UpdateNav();

                    break;

                case 27:
                    Settings.selectTool.Value ^= true;

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
                            DialogResult result = MessageBox.Show("Are you sure you want to replace this ID?\n\nAny existing asset with this ID will be overwritten and the current map will be saved.", MBoxIcon.Warning, MBoxButtons.Yes_No);
                            if (result == DialogResult.No)
                                return;

                            string newID = ReplaceIDBox.Text;
                            MusicPlayer.Reset();

                            File.Move($"{Assets.CACHED}\\{CurrentMap.SoundID}.asset", $"{Assets.CACHED}\\{newID}.asset", true);
                            CurrentMap.SoundID = newID;

                            MapManager.LoadAudio(newID);
                            MusicPlayer.Volume = Settings.masterVolume.Value.Value;

                            if (CurrentMap.FileName != null)
                                MapManager.Save(true);
                            else
                                MapManager.Autosave(true);
                            ShowToast($"REPLACED AUDIO ID WITH: {newID}", Settings.color1.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Register("Failed to replace audio ID", LogSeverity.WARN, ex);
                        ShowToast("FAILED TO REPLACE ID", Settings.color1.Value);
                    }
                    
                    break;

                case 30: // open modchart
                    CurrentMap.SelectedPoint = null;
                    CurrentMap.Notes.ClearSelection();

                    storedLeft = leftNav;
                    leftNav = "";

                    UpdateNav();

                    vfxNav = "";
                    UpdateVFX();

                    LNavOptions.Visible = false;
                    LNavTiming.Visible = false;
                    LNavPatterns.Visible = false;
                    LNavPlayer.Visible = false;

                    ExitMapVFX.Visible = true;

                    GuiTrack.RenderMapObjects = true;
                    GuiTrack.VFXObjects = true;
                    GuiGrid.RenderMapObjects = true;
                    GuiGrid.VFXObjects = true;

                    NavBrightness.Visible = true;
                    NavContrast.Visible = true;
                    NavSaturation.Visible = true;
                    NavBlur.Visible = true;
                    NavFOV.Visible = true;
                    NavTint.Visible = true;
                    NavPosition.Visible = true;
                    NavRotation.Visible = true;
                    NavARFactor.Visible = true;
                    NavText.Visible = true;

                    GameSwitch.Visible = true;

                    UpdateGame();
                    OnResize(((int)Rect.Width, (int)Rect.Height));

                    break;

                case 31: // close modchart
                    CurrentMap.SelectedObjDuration = null;
                    CurrentMap.VfxObjects.ClearSelection();

                    leftNav = storedLeft;
                    storedLeft = "";

                    UpdateNav();

                    vfxNav = "";
                    UpdateVFX();

                    LNavOptions.Visible = true;
                    LNavTiming.Visible = true;
                    LNavPatterns.Visible = true;
                    LNavPlayer.Visible = playerWasVisible;

                    ExitMapVFX.Visible = false;

                    GuiTrack.RenderMapObjects = false;
                    GuiGrid.RenderMapObjects = false;

                    NavBrightness.Visible = false;
                    NavContrast.Visible = false;
                    NavSaturation.Visible = false;
                    NavBlur.Visible = false;
                    NavFOV.Visible = false;
                    NavTint.Visible = false;
                    NavPosition.Visible = false;
                    NavRotation.Visible = false;
                    NavARFactor.Visible = false;
                    NavText.Visible = false;

                    VFXToast.Visible = false;
                    GameSwitch.Visible = false;

                    OnResize(((int)Rect.Width, (int)Rect.Height));

                    break;

                case 32:
                    vfxNav = vfxNav == "Brightness" ? "" : "Brightness";
                    UpdateVFX();

                    break;

                case 33:
                    vfxNav = vfxNav == "Contrast" ? "" : "Contrast";
                    UpdateVFX();

                    break;

                case 34:
                    vfxNav = vfxNav == "Saturation" ? "" : "Saturation";
                    UpdateVFX();

                    break;

                case 35:
                    vfxNav = vfxNav == "Blur" ? "" : "Blur";
                    UpdateVFX();

                    break;

                case 36:
                    vfxNav = vfxNav == "FOV" ? "" : "FOV";
                    UpdateVFX();

                    break;

                case 37:
                    vfxNav = vfxNav == "Tint" ? "" : "Tint";
                    UpdateVFX();

                    break;

                case 38:
                    vfxNav = vfxNav == "Position" ? "" : "Position";
                    UpdateVFX();

                    break;

                case 39:
                    vfxNav = vfxNav == "Rotation" ? "" : "Rotation";
                    UpdateVFX();

                    break;

                case 40:
                    vfxNav = vfxNav == "AR Factor" ? "" : "AR Factor";
                    UpdateVFX();

                    break;

                case 41:
                    vfxNav = vfxNav == "Text" ? "" : "Text";
                    UpdateVFX();

                    break;

                case 42:
                    long vfxMs = (long)Settings.currentTime.Value.Value;
                    long vfxDuration = (long)Settings.vfxDuration.Value;
                    EasingStyle vfxStyle = (EasingStyle)Array.IndexOf(Settings.vfxStyle.Value.Possible, Settings.vfxStyle.Value.Current);
                    EasingDirection vfxDirection = (EasingDirection)Array.IndexOf(Settings.vfxDirection.Value.Possible, Settings.vfxDirection.Value.Current);
                    float vfxIntensity = Settings.vfxIntensity.Value.Value;
                    float vfxFOV = Settings.vfxFOV.Value;
                    Color vfxColor = Settings.vfxColor.Value;
                    Vector3 vfxVector = (Settings.vfxVectorX.Value, Settings.vfxVectorY.Value, Settings.vfxVectorZ.Value);
                    int vfxDegrees = (int)Settings.vfxDegrees.Value;
                    float vfxFactor = Settings.vfxFactor.Value;
                    string vfxString = Settings.vfxString.Value;
                    ListSetting strength = Settings.vfxStrength.Value;
                    int vfxStrength = Array.IndexOf(strength.Possible, strength.Current);

                    MapObject? mapObj = null;

                    switch (vfxNav)
                    {
                        case "Brightness":
                            mapObj = new Brightness(vfxMs, vfxDuration, vfxStyle, vfxDirection, vfxIntensity);
                            break;

                        case "Contrast":
                            mapObj = new Contrast(vfxMs, vfxDuration, vfxStyle, vfxDirection, vfxIntensity);
                            break;

                        case "Saturation":
                            mapObj = new Saturation(vfxMs, vfxDuration, vfxStyle, vfxDirection, vfxIntensity);
                            break;

                        case "Blur":
                            mapObj = new Blur(vfxMs, vfxDuration, vfxStyle, vfxDirection, vfxIntensity);
                            break;

                        case "FOV":
                            mapObj = new FOV(vfxMs, vfxDuration, vfxStyle, vfxDirection, vfxFOV);
                            break;

                        case "Tint":
                            mapObj = new Tint(vfxMs, vfxDuration, vfxStyle, vfxDirection, vfxColor);
                            break;

                        case "Position":
                            mapObj = new Position(vfxMs, vfxDuration, vfxStyle, vfxDirection, vfxVector);
                            break;

                        case "Rotation":
                            mapObj = new Rotation(vfxMs, vfxDuration, vfxStyle, vfxDirection, vfxDegrees);
                            break;

                        case "AR Factor":
                            mapObj = new ARFactor(vfxMs, vfxFactor);
                            break;

                        case "Text":
                            mapObj = new Text(vfxMs, vfxDuration, vfxString, vfxStrength);
                            break;
                    }

                    if (mapObj != null)
                    {
                        if (CurrentMap.VfxObjects.Selected.Count == 1)
                        {
                            MapObject oldObj = CurrentMap.VfxObjects.Selected[0];
                            mapObj.Ms = oldObj.Ms;

                            VfxObjectManager.Replace("MODIFY OBJECT", [oldObj], [mapObj]);
                        }
                        else
                            VfxObjectManager.Add("ADD OBJECT", mapObj);
                    }

                    break;
                    
                case 43:
                    ColorDialog dialog2 = new()
                    {
                        Color = Settings.vfxColor.Value
                    };

                    if (dialog2.ShowDialog() == DialogResult.OK)
                    {
                        Settings.vfxColor.Value = dialog2.Color;
                        Settings.RefreshColors();
                    }

                    break;

                case 44:
                    switch (Settings.exportType.Value.Current)
                    {
                        case "Rhythia (SSPM)":
                            ExportSSPM.ShowWindow();
                            break;
                        case "Nova (NPK)":
                            ExportNOVA.ShowWindow();
                            break;
                    }

                    break;

                case 45: // open special
                    CurrentMap.SelectedPoint = null;
                    CurrentMap.Notes.ClearSelection();

                    storedLeft = leftNav;
                    leftNav = "";

                    UpdateNav();

                    specNav = "";
                    UpdateSpecial();

                    LNavOptions.Visible = false;
                    LNavTiming.Visible = false;
                    LNavPatterns.Visible = false;
                    LNavPlayer.Visible = false;

                    ExitSpecial.Visible = true;

                    GuiTrack.RenderMapObjects = true;
                    GuiTrack.VFXObjects = false;
                    GuiGrid.RenderMapObjects = true;
                    GuiGrid.VFXObjects = false;

                    NavBeat.Visible = true;
                    GameSwitch.Visible = true;

                    UpdateGame();
                    OnResize(((int)Rect.Width, (int)Rect.Height));

                    break;

                case 46: // close special
                    CurrentMap.SpecialObjects.ClearSelection();

                    leftNav = storedLeft;
                    storedLeft = "";

                    UpdateNav();

                    specNav = "";
                    UpdateSpecial();

                    LNavOptions.Visible = true;
                    LNavTiming.Visible = true;
                    LNavPatterns.Visible = true;
                    LNavPlayer.Visible = playerWasVisible;

                    ExitSpecial.Visible = false;

                    GuiTrack.RenderMapObjects = false;
                    GuiGrid.RenderMapObjects = false;

                    NavBeat.Visible = false;
                    GameSwitch.Visible = false;

                    OnResize(((int)Rect.Width, (int)Rect.Height));

                    break;

                case 47:
                    long ms = Timing.GetClosestBeat(Settings.currentTime.Value.Value);
                    Beat beat = new(ms > 0 ? ms : (long)Settings.currentTime.Value.Value);

                    SpecialObjectManager.Add("ADD BEAT", beat);

                    break;

                case 48:
                    vfxNav = "";
                    specNav = "";

                    UpdateVFX();
                    UpdateSpecial();
                    UpdateGame();
                    break;

                case 49:
                    MusicPlayer.ConvertToMP3();
                    break;
            }

            base.OnButtonClicked(id);
        }

        private static bool GetReplayFile()
        {
            DialogResult result = new OpenFileDialog()
            {
                Title = "Select Replay File",
                Filter = "Replay Files (*.sspre;*.phxre;*.qer)|*.sspre;*.phxre;*.qer"
            }.RunWithSetting(Settings.replayPath, out string fileName);

            if (result == DialogResult.OK)
            {
                List<ReplayNode> nodes;
                float tempo;

                try
                {
                    switch (Path.GetExtension(fileName))
                    {
                        case ".qer":
                            nodes = Parser.ParseQER(fileName, out tempo);
                            break;
                        case ".sspre":
                            nodes = Parser.ParseSSPRE(fileName, out tempo);
                            break;
                        case ".phxre":
                            nodes = Parser.ParsePHXR(fileName, out tempo);
                            break;
                        default:
                            return false;
                    }
                }
                catch { return false; }

                Parser.SaveQER($"{Assets.TEMP}\\tempreplay.qer", tempo, nodes);
                return nodes.Count > 0;
            }

            return false;
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            if (Timeline.HoveringBookmark != null && !right)
            {
                MusicPlayer.Pause();
                Settings.currentTime.Value.Value = MainWindow.Instance.ShiftHeld ? Timeline.HoveringBookmark.EndMs : Timeline.HoveringBookmark.Ms;
            }

            base.OnMouseClick(pos, right);
        }

        private void UpdateVFX()
        {
            bool brightnessNav = vfxNav == "Brightness";
            bool contrastNav = vfxNav == "Contrast";
            bool saturationNav = vfxNav == "Saturation";
            bool blurNav = vfxNav == "Blur";
            bool fovNav = vfxNav == "FOV";
            bool tintNav = vfxNav == "Tint";
            bool positionNav = vfxNav == "Position";
            bool rotationNav = vfxNav == "Rotation";
            bool arFactorNav = vfxNav == "AR Factor";
            bool textNav = vfxNav == "Text";

            NavBrightness.Text = brightnessNav ? "[Brightness]" : "Brightness";
            NavContrast.Text = contrastNav ? "[Contrast]" : "Contrast";
            NavSaturation.Text = saturationNav ? "[Saturation]" : "Saturation";
            NavBlur.Text = blurNav ? "[Blur]" : "Blur";
            NavFOV.Text = fovNav ? "[FOV]" : "FOV";
            NavTint.Text = tintNav ? "[Tint]" : "Tint";
            NavPosition.Text = positionNav ? "[Position]" : "Position";
            NavRotation.Text = rotationNav ? "[Rotation]" : "Rotation";
            NavARFactor.Text = arFactorNav ? "[AR Factor]" : "AR Factor";
            NavText.Text = textNav ? "[Text]" : "Text";


            VFXDurationLabel.Visible = brightnessNav || contrastNav || saturationNav || blurNav || fovNav || tintNav || positionNav || rotationNav || textNav;
            VFXDuration.Visible = brightnessNav || contrastNav || saturationNav || blurNav || fovNav || tintNav || positionNav || rotationNav || textNav;
            VFXStyleLabel.Visible = brightnessNav || contrastNav || saturationNav || blurNav || fovNav || tintNav || positionNav || rotationNav;
            VFXStyle.Visible = brightnessNav || contrastNav || saturationNav || blurNav || fovNav || tintNav || positionNav || rotationNav;
            VFXDirectionLabel.Visible = brightnessNav || contrastNav || saturationNav || blurNav || fovNav || tintNav || positionNav || rotationNav;
            VFXDirection.Visible = brightnessNav || contrastNav || saturationNav || blurNav || fovNav || tintNav || positionNav || rotationNav;
            VFXIntensityLabel.Visible = brightnessNav || contrastNav || saturationNav || blurNav;
            VFXIntensity.Visible = brightnessNav || contrastNav || saturationNav || blurNav;
            VFXFOVLabel.Visible = fovNav;
            VFXFOV.Visible = fovNav;
            VFXColor.Visible = tintNav;
            VFXColorSquare.Visible = tintNav;
            VFXVectorLabel.Visible = positionNav;
            VFXVectorX.Visible = positionNav;
            VFXVectorY.Visible = positionNav;
            VFXVectorZ.Visible = positionNav;
            VFXDegreesLabel.Visible = rotationNav;
            VFXDegrees.Visible = rotationNav;
            VFXFactorLabel.Visible = arFactorNav;
            VFXFactor.Visible = arFactorNav;
            VFXStringLabel.Visible = textNav;
            VFXString.Visible = textNav;
            VFXStrengthLabel.Visible = textNav;
            VFXStrength.Visible = textNav;

            VFXApply.Visible = vfxNav != "";
        }

        private void UpdateSpecial()
        {

        }

        private void UpdateGame()
        {
            string game = Settings.modchartGame.Value.Current;
            bool phoenyx = game == "Phoenyx";
            bool nova = game == "Nova";
            bool vfx = GuiGrid.VFXObjects;
            bool spec = !vfx;

            NavBrightness.Visible = phoenyx && vfx;
            NavContrast.Visible = phoenyx && vfx;
            NavSaturation.Visible = phoenyx && vfx;
            NavBlur.Visible = phoenyx && vfx;
            NavFOV.Visible = phoenyx && vfx;
            NavTint.Visible = phoenyx && vfx;
            NavPosition.Visible = phoenyx && vfx;
            NavRotation.Visible = phoenyx && vfx;
            NavARFactor.Visible = phoenyx && vfx;
            NavText.Visible = phoenyx && vfx;

            NavBeat.Visible = nova && spec;
        }

        public void ShowVFXSettings(MapObject obj)
        {
            switch (obj.Name)
            {
                case "Brightness" when obj is Brightness temp:
                    Settings.vfxDuration.Value = temp.Duration;
                    Settings.vfxStyle.Value.Current = temp.Style.ToString();
                    Settings.vfxDirection.Value.Current = temp.Direction.ToString();
                    Settings.vfxIntensity.Value.Value = temp.Intensity;

                    VFXDuration.Text = temp.Duration.ToString();
                    VFXStyle.Text = temp.Style.ToString().ToUpper();
                    VFXDirection.Text = temp.Direction.ToString().ToUpper();

                    VFXIntensity.Update();
                    break;

                case "Contrast" when obj is Contrast temp:
                    Settings.vfxDuration.Value = temp.Duration;
                    Settings.vfxStyle.Value.Current = temp.Style.ToString();
                    Settings.vfxDirection.Value.Current = temp.Direction.ToString();
                    Settings.vfxIntensity.Value.Value = temp.Intensity;

                    VFXDuration.Text = temp.Duration.ToString();
                    VFXStyle.Text = temp.Style.ToString().ToUpper();
                    VFXDirection.Text = temp.Direction.ToString().ToUpper();

                    VFXIntensity.Update();
                    break;

                case "Saturation" when obj is Saturation temp:
                    Settings.vfxDuration.Value = temp.Duration;
                    Settings.vfxStyle.Value.Current = temp.Style.ToString();
                    Settings.vfxDirection.Value.Current = temp.Direction.ToString();
                    Settings.vfxIntensity.Value.Value = temp.Intensity;

                    VFXDuration.Text = temp.Duration.ToString();
                    VFXStyle.Text = temp.Style.ToString().ToUpper();
                    VFXDirection.Text = temp.Direction.ToString().ToUpper();

                    VFXIntensity.Update();
                    break;

                case "Blur" when obj is Blur temp:
                    Settings.vfxDuration.Value = temp.Duration;
                    Settings.vfxStyle.Value.Current = temp.Style.ToString();
                    Settings.vfxDirection.Value.Current = temp.Direction.ToString();
                    Settings.vfxIntensity.Value.Value = temp.Intensity;

                    VFXDuration.Text = temp.Duration.ToString();
                    VFXStyle.Text = temp.Style.ToString().ToUpper();
                    VFXDirection.Text = temp.Direction.ToString().ToUpper();

                    VFXIntensity.Update();
                    break;

                case "FOV" when obj is FOV temp:
                    Settings.vfxDuration.Value = temp.Duration;
                    Settings.vfxStyle.Value.Current = temp.Style.ToString();
                    Settings.vfxDirection.Value.Current = temp.Direction.ToString();
                    Settings.vfxFOV.Value = temp.Intensity;

                    VFXDuration.Text = temp.Duration.ToString();
                    VFXStyle.Text = temp.Style.ToString().ToUpper();
                    VFXDirection.Text = temp.Direction.ToString().ToUpper();

                    VFXFOV.Update();
                    break;

                case "Tint" when obj is Tint temp:
                    Settings.vfxDuration.Value = temp.Duration;
                    Settings.vfxStyle.Value.Current = temp.Style.ToString();
                    Settings.vfxDirection.Value.Current = temp.Direction.ToString();
                    Settings.vfxColor.Value = temp.Color;

                    VFXDuration.Text = temp.Duration.ToString();
                    VFXStyle.Text = temp.Style.ToString().ToUpper();
                    VFXDirection.Text = temp.Direction.ToString().ToUpper();
                    break;

                case "Position" when obj is Position temp:
                    Settings.vfxDuration.Value = temp.Duration;
                    Settings.vfxStyle.Value.Current = temp.Style.ToString();
                    Settings.vfxDirection.Value.Current = temp.Direction.ToString();
                    Settings.vfxVectorX.Value = temp.Pos.X;
                    Settings.vfxVectorY.Value = temp.Pos.Y;
                    Settings.vfxVectorZ.Value = temp.Pos.Z;

                    VFXDuration.Text = temp.Duration.ToString();
                    VFXStyle.Text = temp.Style.ToString().ToUpper();
                    VFXDirection.Text = temp.Direction.ToString().ToUpper();
                    VFXVectorX.Text = temp.Pos.X.ToString();
                    VFXVectorY.Text = temp.Pos.Y.ToString();
                    VFXVectorZ.Text = temp.Pos.Z.ToString();
                    break;

                case "Rotation" when obj is Rotation temp:
                    Settings.vfxDuration.Value = temp.Duration;
                    Settings.vfxStyle.Value.Current = temp.Style.ToString();
                    Settings.vfxDirection.Value.Current = temp.Direction.ToString();
                    Settings.vfxDegrees.Value = temp.Degrees;

                    VFXDuration.Text = temp.Duration.ToString();
                    VFXStyle.Text = temp.Style.ToString().ToUpper();
                    VFXDirection.Text = temp.Direction.ToString().ToUpper();
                    VFXDegrees.Text = temp.Degrees.ToString();
                    break;

                case "ARFactor" when obj is ARFactor temp:
                    Settings.vfxFactor.Value = temp.Factor;

                    VFXFactor.Text = temp.Factor.ToString();
                    break;

                case "Text" when obj is Text temp:
                    Settings.vfxDuration.Value = temp.Duration;
                    Settings.vfxString.Value = temp.String;
                    Settings.vfxStrength.Value.Current = Settings.vfxStrength.Value.Possible[temp.Visibility];

                    VFXDuration.Text = temp.Duration.ToString();
                    VFXString.Text = temp.String;
                    break;
            }

            vfxNav = obj.Name ?? "";
            UpdateVFX();
        }

        public void ShowSpecialSettings(MapObject obj)
        {
            switch (obj.Name)
            {

            }

            specNav = obj.Name ?? "";
            UpdateSpecial();
        }

        private void UpdateNav()
        {
            bool optionsNav = leftNav == "Options";
            bool timingNav = leftNav == "Timing";
            bool patternsNav = leftNav == "Patterns";
            bool playerNav = leftNav == "Player";

            bool snappingNav = rightNav == "Snapping";
            bool graphicsNav = rightNav == "Graphics";
            bool exportNav = rightNav == "Export";

            LNavOptions.Text = optionsNav ? "[OPTIONS]" : "OPTIONS";
            LNavTiming.Text = timingNav ? "[TIMING]" : "TIMING";
            LNavPatterns.Text = patternsNav ? "[PATTERNS]" : "PATTERNS";
            if (!rhythia)
                LNavPlayer.Text = playerNav ? "[PLAYTEST]" : "PLAYTEST";

            RNavSnapping.Text = snappingNav ? "[SNAPPING]" : "SNAPPING";
            RNavGraphics.Text = graphicsNav ? "[GRAPHICS]" : "GRAPHICS";
            RNavExport.Text = exportNav ? "[EXPORT]" : "EXPORT";


            Numpad.Visible = optionsNav;
            SeparateClickTools.Visible = optionsNav;
            SwapClickMode.Visible = optionsNav;
            JumpOnPaste.Visible = optionsNav;
            PauseOnScroll.Visible = optionsNav;
            EditMapVFX.Visible = optionsNav;
            EditSpecial.Visible = optionsNav;


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
            PasteReversed.Visible = patternsNav;
            StoreNodes.Visible = patternsNav;
            ClearNodes.Visible = patternsNav;
            CurveBezier.Visible = patternsNav;
            BezierBox.Visible = patternsNav;
            BezierButton.Visible = patternsNav;

            RotateLabel.Visible = patternsNav;
            ScaleLabel.Visible = patternsNav;
            DrawBezierLabel.Visible = patternsNav;


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


            SaveButton.Visible = exportNav;
            SaveAsButton.Visible = exportNav;
            ExportSwitch.Visible = exportNav;
            ExportButton.Visible = exportNav;
            ReplaceIDBox.Visible = exportNav;
            ReplaceID.Visible = exportNav;
            ConvertAudio.Visible = exportNav && !Platform.IsLinux;

            ReplaceIDLabel.Visible = exportNav;
        }

        public override void OnResize(Vector2i size)
        {
            base.OnResize(size);

            float heightdiff = size.Y / 1080f;

            CopyButton.Rect.Location = new PointF(Grid.Rect.X, Grid.Rect.Y - 42 - 75 * heightdiff);
            BackButton.Rect.Location = new PointF(Grid.Rect.X, Grid.Rect.Bottom + 84 * heightdiff);
            ClickModeLabel.Rect.Location = new PointF(Grid.Rect.X, BackButton.Rect.Bottom + 10 * heightdiff);
            VFXToast.Rect.Location = ClickModeLabel.Rect.Location;

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

            SliderSetting currentTime = Settings.currentTime.Value;
            float progress = currentTime.Value / currentTime.Max;

            CurrentTimeLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Height / 2f, Timeline.Rect.Bottom - 32f);
            CurrentMsLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Height / 2f + (Timeline.Rect.Width - Timeline.Rect.Height) * progress, Timeline.Rect.Y - 4f);
            TotalTimeLabel.Rect.Location = new PointF(Timeline.Rect.X - Timeline.Rect.Height / 2f + Timeline.Rect.Width, Timeline.Rect.Bottom - 32f);
            NotesLabel.Rect.Location = new PointF(Timeline.Rect.X + Timeline.Rect.Width / 2f, Timeline.Rect.Bottom - 32f);

            LNavPlayer.Rect.Location = new PointF(LNavPlayer.Rect.X, Timeline.Rect.Y - LNavPlayer.Rect.Height - 20f);
            GameSwitch.Rect = LNavPlayer.Rect;

            CopyButton.Update();
            BackButton.Update();
            ClickModeLabel.Update();
            VFXToast.Update();

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
            GameSwitch.Update();
        }

        public void ShowToast(string text, Color color)
        {
            toastTime = 0f;

            ToastLabel.Text = text;
            ToastLabel.Color = color;
        }

        public void SetVFXToast(string text)
        {
            VFXToast.Text = text;
            VFXToast.Visible = !string.IsNullOrWhiteSpace(text) && GuiGrid.RenderMapObjects;
        }
    }
}
