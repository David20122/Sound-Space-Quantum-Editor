﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
using System.IO;

namespace New_SSQE.GUI
{
    internal class GuiWindowSettings : GuiWindow
    {
        private readonly GuiButton BackButton = new(655, 930, 600, 100, 0, "SAVE AND RETURN", 54, false, false, "square");
        private readonly GuiButton ResetButton = new(700, 865, 500, 50, 1, "RESET TO DEFAULT", 30, false, false, "square");
        private readonly GuiButton OpenDirectoryButton = new(700, 810, 500, 50, 2, "OPEN EDITOR FOLDER", 30, false, false, "square");
        private readonly GuiButton KeybindsButton = new(700, 755, 500, 50, 3, "CHANGE KEYBINDS", 30, false, false, "square");

        private readonly GuiButton Color1Picker = new(210, 160, 200, 50, 4, "PICK COLOR", 30, false, false, "square");
        private readonly GuiLabel Color1Label = new(210, 130, 200, 26, "Color 1:", 30, false, false, "main", false);
        private readonly GuiSquare Color1Square = new(420, 145, 75, 75, Settings.settings["color1"]);

        private readonly GuiButton Color2Picker = new(210, 310, 200, 50, 5, "PICK COLOR", 30, false, false, "square");
        private readonly GuiLabel Color2Label = new(210, 280, 200, 26, "Color 2:", 30, false, false, "main", false);
        private readonly GuiSquare Color2Square = new(420, 295, 75, 75, Settings.settings["color2"]);

        private readonly GuiButton Color3Picker = new(210, 460, 200, 50, 6, "PICK COLOR", 30, false, false, "square");
        private readonly GuiLabel Color3Label = new(210, 430, 200, 26, "Color 3:", 30, false, false, "main", false);
        private readonly GuiSquare Color3Square = new(420, 445, 75, 75, Settings.settings["color3"]);

        private readonly GuiButton Color4Picker = new(210, 610, 200, 50, 7, "PICK COLOR", 30, false, false, "square");
        private readonly GuiLabel Color4Label = new(210, 580, 200, 26, "Color 4:", 30, false, false, "main", false);
        private readonly GuiSquare Color4Square = new(420, 595, 75, 75, Settings.settings["color4"]);

        private readonly GuiButton NoteColorPicker = new(210, 760, 200, 50, 8, "ADD COLOR", 30, false, false, "square");
        private readonly GuiLabel NoteColorLabel = new(210, 730, 200, 26, "Note Colors:", 30, false, false, "main", false);
        private readonly GuiLabel NoteColorInfo = new(215, 815, 195, 26, "LMB: Remove\nRMB: Move left", 30, false, false, "main", false);
        private readonly GuiSquare NoteColorHoverSquare = new(0, 0, 0, 0, Color.FromArgb(255, 0, 127, 255), true);

        private readonly GuiCheckbox WaveformCheckbox = new(950, 380, 55, 55, "waveform", "Enable Waveform", 34);
        private readonly GuiCheckbox ClassicWaveformCheckbox = new(950, 455, 55, 55, "classicWaveform", "Use Classic Waveform", 34);
        private readonly GuiCheckbox AutosaveCheckbox = new(950, 155, 55, 55, "enableAutosave", "Enable Autosave", 34);
        private readonly GuiCheckbox CorrectOnCopyCheckbox = new(1350, 155, 55, 55, "correctOnCopy", "Correct Errors on Copy", 34);
        private readonly GuiCheckbox SkipDownloadCheckbox = new(1350, 230, 55, 55, "skipDownload", "Skip Download from Roblox", 34);
        private readonly GuiCheckbox ReverseScrollCheckbox = new(1350, 305, 55, 55, "reverseScroll", "Reverse Scroll Direction", 34);
        private readonly GuiCheckbox CheckForUpdatesCheckbox = new(1350, 380, 55, 55, "checkUpdates", "Check For Updates", 34);
        private readonly GuiCheckbox UseVSyncCheckbox = new(1350, 455, 55, 55, "useVSync", "Enable VSync", 34);
        private readonly GuiCheckbox LimitPlayerFPSCheckbox = new(1350, 605, 55, 55, "limitPlayerFPS", "Limit Player FPS", 34);
        private readonly GuiCheckbox FullscreenPlayerCheckbox = new(1350, 680, 55, 55, "fullscreenPlayer", "Open Player in Fullscreen", 34);

        private readonly GuiCheckbox UseRhythia = new(1350, 800, 55, 55, "useRhythia", "Use Rhythia as Player", 34);
        private readonly GuiLabel RhythiaPathLabel = new(1350, 876, 200, 26, "", 30, false, false, "main", false);
        private readonly GuiButton RhythiaPath = new(1350, 910, 200, 50, 9, "CHANGE PATH", 30);

        private readonly GuiTextbox EditorBGOpacityTextbox = new(560, 160, 200, 50, "", 34, true, false, false, "editorBGOpacity");
        private readonly GuiLabel EditorBGOpacityLabel = new(560, 130, 200, 26, "Editor BG Opacity:", 30, false, false, "main", false);
        private readonly GuiSquare EditorBGOpacitySquare = new(770, 145, 75, 75, Color.FromArgb(255, 255, 255, 255));
        
        private readonly GuiTextbox GridOpacityTextbox = new(560, 310, 200, 50, "", 34, true, false, false, "gridOpacity");
        private readonly GuiLabel GridOpacityLabel = new(560, 280, 200, 26, "Grid Opacity:", 30, false, false, "main", false);
        private readonly GuiSquare GridOpacitySquare = new(770, 295, 75, 75, Color.FromArgb(255, 255, 255, 255));
        
        private readonly GuiTextbox TrackOpacityTextbox = new(560, 460, 200, 50, "", 34, true, false, false, "trackOpacity");
        private readonly GuiLabel TrackOpacityLabel = new(560, 430, 200, 26, "Track Opacity:", 30, false, false, "main", false);
        private readonly GuiSquare TrackOpacitySquare = new(770, 445, 75, 75, Color.FromArgb(255, 255, 255, 255));

        private readonly GuiTextbox AutosaveIntervalTextbox = new(950, 256, 200, 50, "", 34, true, false, false, "autosaveInterval", "main", false, true, true);
        private readonly GuiLabel AutosaveIntervalLabel = new(950, 226, 200, 26, "Autosave Interval (min):", 30, false, false, "main", false);

        private readonly GuiTextbox WaveformDetailTextbox = new(950, 556, 200, 50, "", 34, true, false, false, "waveformDetail", "main", false, true, true);
        private readonly GuiLabel WaveformDetailLabel = new(950, 526, 200, 26, "Waveform Level of Detail:", 30, false, false, "main", false);

        private readonly GuiSlider FPSLimitSlider = new(1350, 500, 400, 55, "fpsLimit", false);
        private readonly GuiLabel FPSLimitLabel = new(1350, 551, 400, 55, "FPS Limit: ", 34, false, false, "main", false);

        private readonly List<GuiSquare> ColorPickerSquares = new();
        private readonly List<GuiSquare> OpacitySquares = new();
        private readonly List<GuiTextbox> Opacities = new();
        private readonly List<GuiSquare> NoteColorSquares = new();

        private int hoveringColor;

        private int resetQueryTime;
        private bool resetQuery;

        public GuiWindowSettings() : base(0, 0, MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y)
        {
            Controls = new List<WindowControl>
            {
                // Squares
                Color1Square, Color2Square, Color3Square, Color4Square, NoteColorHoverSquare, EditorBGOpacitySquare, GridOpacitySquare, TrackOpacitySquare,
                // Buttons
                BackButton, ResetButton, OpenDirectoryButton, KeybindsButton, Color1Picker, Color2Picker, Color3Picker, Color4Picker, NoteColorPicker, RhythiaPath,
                // Checkboxes
                WaveformCheckbox, ClassicWaveformCheckbox, AutosaveCheckbox, CorrectOnCopyCheckbox, SkipDownloadCheckbox, ReverseScrollCheckbox, UseVSyncCheckbox,
                CheckForUpdatesCheckbox, FullscreenPlayerCheckbox, LimitPlayerFPSCheckbox, UseRhythia,
                // Sliders
                FPSLimitSlider,
                // Boxes
                EditorBGOpacityTextbox, GridOpacityTextbox, TrackOpacityTextbox, AutosaveIntervalTextbox, WaveformDetailTextbox,
                // Labels
                Color1Label, Color2Label, Color3Label, Color4Label, NoteColorLabel, NoteColorInfo, EditorBGOpacityLabel, GridOpacityLabel, TrackOpacityLabel, AutosaveIntervalLabel,
                WaveformDetailLabel, FPSLimitLabel, RhythiaPathLabel,
            };

            BackgroundSquare = new(0, 0, 1920, 1080, Color.FromArgb(255, 30, 30, 30), false, "background_menu.png", "menubg");
            Init();

            ColorPickerSquares.Add(Color1Square);
            ColorPickerSquares.Add(Color2Square);
            ColorPickerSquares.Add(Color3Square);
            ColorPickerSquares.Add(Color4Square);

            OpacitySquares.Add(EditorBGOpacitySquare);
            OpacitySquares.Add(GridOpacitySquare);
            OpacitySquares.Add(TrackOpacitySquare);

            Opacities.Add(EditorBGOpacityTextbox);
            Opacities.Add(GridOpacityTextbox);
            Opacities.Add(TrackOpacityTextbox);

            EditorBGOpacityTextbox.Text = Settings.settings["editorBGOpacity"].ToString();
            GridOpacityTextbox.Text = Settings.settings["gridOpacity"].ToString();
            TrackOpacityTextbox.Text = Settings.settings["trackOpacity"].ToString();

            RefreshNoteColors();
            RefreshRhythiaPath();
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            var noteColors = Settings.settings["noteColors"];

            var widthdiff = Rect.Width / 1920f;
            var heightdiff = Rect.Height / 1080f;
            var colorWidth = 75f * widthdiff / noteColors.Count;

            //colors 1-4
            for (int i = 0; i < 4; i++)
                ColorPickerSquares[i].Color = Settings.settings[$"color{i + 1}"];

            //note color hover box
            NoteColorHoverSquare.Visible = hoveringColor >= 0;

            if (NoteColorHoverSquare.Visible)
            {
                var colorRect = new RectangleF(NoteColorPicker.Rect.X + 210 * widthdiff + colorWidth * hoveringColor, NoteColorPicker.Rect.Y - 15 * heightdiff, colorWidth, 75 * heightdiff);

                NoteColorHoverSquare.Rect = colorRect;
                NoteColorHoverSquare.Update();
            }

            //opacities
            for (int i = 0; i < 3; i++)
            {
                if (int.TryParse(Opacities[i].Text, out int opacity))
                {
                    opacity = MathHelper.Clamp(opacity, 0, 255);
                    OpacitySquares[i].Color = Color.FromArgb(opacity, 255, 255, 255);

                    Opacities[i].Text = opacity.ToString();
                    Opacities[i].Update();
                }
            }

            var fps = Settings.settings["fpsLimit"].Value;
            var max = Settings.settings["fpsLimit"].Max;

            FPSLimitLabel.Text = $"FPS Limit: {(Math.Round(fps) == Math.Round(max) ? "Unlimited" : Math.Round(fps + 60f))}";

            base.Render(mousex, mousey, frametime);
        }

        public override void OnResize(Vector2i size)
        {
            Rect = new RectangleF(0, 0, size.X, size.Y);

            base.OnResize(size);
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            var setting = Settings.settings["noteColors"];

            if (hoveringColor >= 0 && setting.Count > 1)
            {
                if (!right)
                    setting.RemoveAt(hoveringColor);
                else if (hoveringColor > 0)
                    (setting[hoveringColor], setting[hoveringColor - 1]) = (setting[hoveringColor - 1], setting[hoveringColor]);

                RefreshNoteColors();
            }

            base.OnMouseClick(pos, right);
        }

        public override void OnMouseMove(Point pos)
        {
            var widthdiff = Rect.Width / 1920f;
            var heightdiff = Rect.Height / 1080f;

            var setting = Settings.settings["noteColors"];

            var x = pos.X - (NoteColorPicker.Rect.X + 210 * widthdiff);
            var y = pos.Y - (NoteColorPicker.Rect.Y - 15 * widthdiff);
            var xint = x / (75f * widthdiff / setting.Count);

            if (setting.Count > 1 && xint >= 0 && xint < setting.Count && y >= 0 && y < 75 * heightdiff)
                hoveringColor = (int)xint;
            else
                hoveringColor = -1;

            base.OnMouseMove(pos);
        }

        private void RunQueryReset(int time)
        {
            resetQueryTime = time;

            var delay = Task.Delay(5000).ContinueWith(_ =>
            {
                if (resetQueryTime == time)
                {
                    resetQuery = false;
                    ResetButton.Text = "RESET TO DEFAULT";
                }
            });
        }

        private void RefreshNoteColors()
        {
            var setting = Settings.settings["noteColors"];

            foreach (var control in NoteColorSquares)
            {
                Controls.Remove(control);
                control.Dispose();
            }

            NoteColorSquares.Clear();
            Controls.Remove(NoteColorHoverSquare);

            for (int i = 0; i < setting.Count; i++)
            {
                var colorWidth = 75f / setting.Count;
                var square = new GuiSquare(420 + i * colorWidth, 745, colorWidth, 75, setting[i]);

                Controls.Add(square);
                NoteColorSquares.Add(square);
            }

            Controls.Add(NoteColorHoverSquare);
            

            OnResize(MainWindow.Instance.ClientSize);
            OnMouseMove(MainWindow.Instance.Mouse);
        }

        private void RefreshRhythiaPath()
        {
            string path = Settings.settings["rhythiaPath"];
            int length = 15;

            int startLength = Math.Min(path.Length, length);
            int endLength = Math.Min(length, Math.Max(path.Length - length, 0));

            string start = path[..startLength];
            string end = path[(path.Length - endLength)..];
            string final = start;

            if (!string.IsNullOrWhiteSpace(end))
                final += $"{(path.Length > length * 2 ? "..." : "")}{end}";

            if (string.IsNullOrWhiteSpace(path))
                RhythiaPathLabel.Text = "Rhythia Path: NONE";
            else if (!File.Exists(path))
                RhythiaPathLabel.Text = "Rhythia Path: INVALID";
            else
                RhythiaPathLabel.Text = $"Rhythia Path: {final}";
        }

        public override void OnButtonClicked(int id)
        {
            switch (id)
            {
                case 0:
                    Settings.Save();
                    MainWindow.Instance.SwitchWindow(new GuiWindowMenu());

                    break;

                case 1:
                    if (resetQuery)
                    {
                        Settings.Reset();

                        resetQuery = false;
                        ResetButton.Text = "RESET TO DEFAULT";
                    }
                    else
                    {
                        resetQuery = true;
                        RunQueryReset(DateTime.Now.Millisecond);

                        ResetButton.Text = "ARE YOU SURE?";
                    }

                    break;

                case 2:
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        // if mac
                        Process.Start("open", $"\"{Environment.CurrentDirectory}\"");
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        // if windows
                        Process.Start("explorer.exe", Environment.CurrentDirectory);
                    else // linux probably
                        ActionLogging.Register($"Open dir not implemented on platform {RuntimeInformation.OSDescription}", "WARN");

                    break;

                case 3:
                    MainWindow.Instance.SwitchWindow(new GuiWindowKeybinds());

                    break;

                case 4:
                    var dialog1 = new ColorDialog()
                    {
                        Color = Settings.settings["color1"]
                    };

                    if (dialog1.ShowDialog() == DialogResult.OK)
                    {
                        Settings.settings["color1"] = dialog1.Color;
                        Settings.RefreshColors();
                    }

                    break;

                case 5:
                    var dialog2 = new ColorDialog()
                    {
                        Color = Settings.settings["color2"]
                    };

                    if (dialog2.ShowDialog() == DialogResult.OK)
                    {
                        Settings.settings["color2"] = dialog2.Color;
                        Settings.RefreshColors();
                    }

                    break;

                case 6:
                    var dialog3 = new ColorDialog()
                    {
                        Color = Settings.settings["color3"]
                    };

                    if (dialog3.ShowDialog() == DialogResult.OK)
                    {
                        Settings.settings["color3"] = dialog3.Color;
                        Settings.RefreshColors();
                    }

                    break;

                case 7:
                    var dialog4 = new ColorDialog()
                    {
                        Color = Settings.settings["color4"]
                    };

                    if (dialog4.ShowDialog() == DialogResult.OK)
                    {
                        Settings.settings["color4"] = dialog4.Color;
                        Settings.RefreshColors();
                    }

                    break;

                case 8:
                    if (Settings.settings["noteColors"].Count < 32)
                    {
                        var dialogN = new ColorDialog()
                        {
                            Color = Color.White
                        };

                        if (dialogN.ShowDialog() == DialogResult.OK)
                        {
                            Settings.settings["noteColors"].Add(dialogN.Color);
                            RefreshNoteColors();
                            Settings.RefreshColors();
                        }
                    }

                    break;

                case 9:
                    var dialog = new OpenFileDialog()
                    {
                        Title = "Select Rhythia Executable",
                        Filter = "Executable Files (*.exe)|*.exe",
                    };

                    if (Settings.settings["rhythiaFolderPath"] != "")
                        dialog.InitialDirectory = Settings.settings["rhythiaFolderPath"];

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        Settings.settings["rhythiaFolderPath"] = Path.GetDirectoryName(dialog.FileName) ?? "";
                        Settings.settings["rhythiaPath"] = dialog.FileName;

                        RefreshRhythiaPath();
                    }

                    break;
            }

            base.OnButtonClicked(id);
        }
    }
}
