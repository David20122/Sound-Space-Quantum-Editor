using System.Drawing;
using New_SSQE.ExternalUtils;
using New_SSQE.Misc.Dialogs;
using New_SSQE.Preferences;
using OpenTK.Mathematics;

namespace New_SSQE.GUI
{
    internal class GuiWindowSettings : GuiWindow
    {
        private readonly GuiButton BackButton = new(655, 930, 600, 100, 0, "SAVE AND RETURN", 54, "square");
        private readonly GuiButton ResetButton = new(700, 865, 500, 50, 1, "RESET TO DEFAULT", 30, "square");
        private readonly GuiButton OpenDirectoryButton = new(700, 810, 500, 50, 2, "OPEN EDITOR FOLDER", 30, "square");
        private readonly GuiButton KeybindsButton = new(700, 755, 500, 50, 3, "CHANGE KEYBINDS", 30, "square");


        private readonly GuiButton Color1Picker = new(180, 80, 200, 50, 4, "PICK COLOR", 30, "square");
        private readonly GuiLabel Color1Label = new(180, 50, 200, 26, "Color 1 (1/X BPM + Primary):", 30, "main", false);
        private readonly GuiSquare Color1Square = new(390, 80, 75, 50, Settings.color1.Value);

        private readonly GuiButton Color2Picker = new(180, 180, 200, 50, 5, "PICK COLOR", 30, "square");
        private readonly GuiLabel Color2Label = new(180, 150, 200, 26, "Color 2 (1/1 BPM + Secondary):", 30, "main", false);
        private readonly GuiSquare Color2Square = new(390, 180, 75, 50, Settings.color2.Value);

        private readonly GuiButton Color3Picker = new(180, 280, 200, 50, 6, "PICK COLOR", 30, "square");
        private readonly GuiLabel Color3Label = new(180, 250, 200, 26, "Color 3 (1/2 BPM + Preview):", 30, "main", false);
        private readonly GuiSquare Color3Square = new(390, 280, 75, 50, Settings.color3.Value);

        private readonly GuiButton Color4Picker = new(180, 380, 200, 50, 7, "PICK COLOR", 30, "square");
        private readonly GuiLabel Color4Label = new(180, 350, 200, 26, "Color 4 (Waveform):", 30, "main", false);
        private readonly GuiSquare Color4Square = new(390, 380, 75, 50, Settings.color4.Value);

        private readonly GuiButton Color5Picker = new(180, 480, 200, 50, 11, "PICK COLOR", 30, "square");
        private readonly GuiLabel Color5Label = new(180, 450, 200, 26, "Color 5 (Modcharts):", 30, "main", false);
        private readonly GuiSquare Color5Square = new(390, 480, 75, 50, Settings.color5.Value);

        private readonly GuiButton NoteColorPicker = new(180, 580, 200, 50, 8, "ADD COLOR", 30, "square");
        private readonly GuiLabel NoteColorLabel = new(180, 550, 200, 26, "Note Colors:", 30, "main", false);
        private readonly GuiLabel NoteColorInfo = new(185, 635, 195, 26, "LMB: Remove\nRMB: Move left", 30, "main", false);
        private readonly GuiSquare NoteColorHoverSquare = new(0, 0, 0, 0, Color.FromArgb(255, 0, 127, 255), true);

        private readonly GuiTextbox EditorBGOpacityTextbox = new(180, 750, 200, 50, 34, Settings.editorBGOpacity, false);
        private readonly GuiLabel EditorBGOpacityLabel = new(180, 720, 200, 26, "Editor BG Opacity:", 30, "main", false);
        private readonly GuiSquare EditorBGOpacitySquare = new(390, 750, 75, 50, Color.FromArgb(255, 255, 255, 255));

        private readonly GuiTextbox GridOpacityTextbox = new(180, 850, 200, 50, 34, Settings.gridOpacity, false);
        private readonly GuiLabel GridOpacityLabel = new(180, 820, 200, 26, "Grid Opacity:", 30, "main", false);
        private readonly GuiSquare GridOpacitySquare = new(390, 850, 75, 50, Color.FromArgb(255, 255, 255, 255));

        private readonly GuiTextbox TrackOpacityTextbox = new(180, 950, 200, 50, 34, Settings.trackOpacity, false);
        private readonly GuiLabel TrackOpacityLabel = new(180, 920, 200, 26, "Track Opacity:", 30, "main", false);
        private readonly GuiSquare TrackOpacitySquare = new(390, 950, 75, 50, Color.FromArgb(255, 255, 255, 255));


        private readonly GuiCheckbox UseVSyncCheckbox = new(630, 380, 45, 45, Settings.useVSync, "Enable VSync", 34);
        private readonly GuiCheckbox LimitPlayerFPSCheckbox = new(630, 440, 45, 45, Settings.limitPlayerFPS, "Limit Player FPS", 34);
        private readonly GuiSlider FPSLimitSlider = new(630, 490, 400, 55, Settings.fpsLimit, false);
        private readonly GuiLabel FPSLimitLabel = new(630, 540, 400, 55, "FPS Limit: ", 34, "main", false);

        private readonly GuiCheckbox WaveformCheckbox = new(630, 80, 45, 45, Settings.waveform, "Enable Waveform", 34);
        private readonly GuiCheckbox ClassicWaveformCheckbox = new(630, 140, 45, 45, Settings.classicWaveform, "Use Classic Waveform", 34);
        private readonly GuiTextbox WaveformDetailTextbox = new(630, 230, 200, 50, 34, Settings.waveformDetail, true, true);
        private readonly GuiLabel WaveformDetailLabel = new(630, 200, 200, 26, "Waveform Level of Detail:", 30, "main", false);


        private readonly GuiCheckbox AutosaveCheckbox = new(1070, 140, 45, 45, Settings.enableAutosave, "Enable Autosave", 34);
        private readonly GuiTextbox AutosaveIntervalTextbox = new(1070, 230, 200, 50, 34, Settings.autosaveInterval, true, true);
        private readonly GuiLabel AutosaveIntervalLabel = new(1070, 200, 200, 26, "Autosave Interval (min):", 30, "main", false);

        private readonly GuiCheckbox FullscreenPlayerCheckbox = new(1070, 380, 45, 45, Settings.fullscreenPlayer, "Open Player in Fullscreen", 34);
        private readonly GuiCheckbox UseRhythia = new(1070, 440, 45, 45, Settings.useRhythia, "Use Rhythia as Player", 34);
        private readonly GuiLabel RhythiaPathLabel = new(1070, 500, 200, 26, "", 30, "main", false);
        private readonly GuiButton RhythiaPath = new(1070, 530, 200, 50, 9, "CHANGE PATH", 30);


        private readonly GuiCheckbox CheckForUpdatesCheckbox = new(1420, 80, 45, 45, Settings.checkUpdates, "Check For Updates", 34);
        private readonly GuiCheckbox SkipDownloadCheckbox = new(1420, 140, 45, 45, Settings.skipDownload, "Skip Download from Roblox", 34);
        private readonly GuiCheckbox CorrectOnCopyCheckbox = new(1420, 200, 45, 45, Settings.correctOnCopy, "Correct Errors on Copy", 34);
        private readonly GuiCheckbox ReverseScrollCheckbox = new(1420, 260, 45, 45, Settings.reverseScroll, "Reverse Scroll Direction", 34);


        private readonly GuiCheckbox MSAACheckbox = new(1420, 650, 45, 45, Settings.msaa, "Use Anti-Aliasing", 34);
        private readonly GuiLabel RestartLabel = new(1420, 700, 200, 26, "Requires restart!", 30, "main", false);


        //private readonly GuiButton LanguageButton = new(10);
        //private readonly GuiSquare LanguageIcon = new(Color.FromArgb(255, 0, 0, 0), false, $"{Assets.Textures}\\Translate.png", "translate");


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
                Color1Square, Color2Square, Color3Square, Color4Square, NoteColorHoverSquare, EditorBGOpacitySquare, GridOpacitySquare, TrackOpacitySquare, Color5Square,
                // Buttons
                BackButton, ResetButton, OpenDirectoryButton, KeybindsButton, Color1Picker, Color2Picker, Color3Picker, Color4Picker, NoteColorPicker, RhythiaPath, Color5Picker,
                // Checkboxes
                WaveformCheckbox, ClassicWaveformCheckbox, AutosaveCheckbox, CorrectOnCopyCheckbox, SkipDownloadCheckbox, ReverseScrollCheckbox, UseVSyncCheckbox,
                CheckForUpdatesCheckbox, FullscreenPlayerCheckbox, LimitPlayerFPSCheckbox, UseRhythia, MSAACheckbox,
                // Sliders
                FPSLimitSlider,
                // Boxes
                EditorBGOpacityTextbox, GridOpacityTextbox, TrackOpacityTextbox, AutosaveIntervalTextbox, WaveformDetailTextbox,
                // Labels
                Color1Label, Color2Label, Color3Label, Color4Label, NoteColorLabel, NoteColorInfo, EditorBGOpacityLabel, GridOpacityLabel, TrackOpacityLabel, AutosaveIntervalLabel,
                WaveformDetailLabel, FPSLimitLabel, RhythiaPathLabel, RestartLabel, Color5Label
            };

            BackgroundSquare = new(Color.FromArgb(255, 30, 30, 30), "background_menu.png", "menubg");
            Init();

            ColorPickerSquares.Add(Color1Square);
            ColorPickerSquares.Add(Color2Square);
            ColorPickerSquares.Add(Color3Square);
            ColorPickerSquares.Add(Color4Square);
            ColorPickerSquares.Add(Color5Square);

            OpacitySquares.Add(EditorBGOpacitySquare);
            OpacitySquares.Add(GridOpacitySquare);
            OpacitySquares.Add(TrackOpacitySquare);

            Opacities.Add(EditorBGOpacityTextbox);
            Opacities.Add(GridOpacityTextbox);
            Opacities.Add(TrackOpacityTextbox);

            EditorBGOpacityTextbox.Text = Settings.editorBGOpacity.Value.ToString();
            GridOpacityTextbox.Text = Settings.gridOpacity.Value.ToString();
            TrackOpacityTextbox.Text = Settings.trackOpacity.Value.ToString();

            RefreshNoteColors();
            RefreshRhythiaPath();
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            List<Color> noteColors = Settings.noteColors.Value;

            float widthdiff = Rect.Width / 1920f;
            float heightdiff = Rect.Height / 1080f;
            float colorWidth = 75f * widthdiff / noteColors.Count;

            //colors 1-5
            ColorPickerSquares[0].Color = Settings.color1.Value;
            ColorPickerSquares[1].Color = Settings.color2.Value;
            ColorPickerSquares[2].Color = Settings.color3.Value;
            ColorPickerSquares[3].Color = Settings.color4.Value;
            ColorPickerSquares[4].Color = Settings.color5.Value;

            //note color hover box
            NoteColorHoverSquare.Visible = hoveringColor >= 0;

            if (NoteColorHoverSquare.Visible)
            {
                RectangleF colorRect = new(NoteColorPicker.Rect.X + 210 * widthdiff + colorWidth * hoveringColor, NoteColorPicker.Rect.Y, colorWidth, 50 * heightdiff);

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
                    Opacities[i].SetSetting();
                    Opacities[i].Update();
                }
            }

            float fps = Settings.fpsLimit.Value.Value;
            float max = Settings.fpsLimit.Value.Max;

            FPSLimitLabel.Text = $"FPS Limit: {(Math.Round(fps) == Math.Round(max) ? "Unlimited" : Math.Round(fps + 60f))}";
            RestartLabel.Visible = MainWindow.MSAA != Settings.msaa.Value;

            base.Render(mousex, mousey, frametime);
        }

        public override void OnResize(Vector2i size)
        {
            base.OnResize(size);

            //float widthdiff = size.X / 1920f;

            //LanguageButton.Rect = new(BackButton.Rect.Right + 10f * widthdiff, BackButton.Rect.Y, BackButton.Rect.Height, BackButton.Rect.Height);
            //LanguageIcon.Rect = LanguageButton.Rect;

            //LanguageButton.Update();
            //LanguageIcon.Update();
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            List<Color> setting = Settings.noteColors.Value;

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
            float widthdiff = Rect.Width / 1920f;
            float heightdiff = Rect.Height / 1080f;

            List<Color> setting = Settings.noteColors.Value;

            float x = pos.X - (NoteColorPicker.Rect.X + 210 * widthdiff);
            float y = pos.Y - NoteColorPicker.Rect.Y;
            float xint = x / (75f * widthdiff / setting.Count);

            if (setting.Count > 1 && xint >= 0 && xint < setting.Count && y >= 0 && y < 50 * heightdiff)
                hoveringColor = (int)xint;
            else
                hoveringColor = -1;

            base.OnMouseMove(pos);
        }

        private void RunQueryReset(int time)
        {
            resetQueryTime = time;

            Task delay = Task.Delay(5000).ContinueWith(_ =>
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
            List<Color> setting = Settings.noteColors.Value;

            foreach (GuiSquare control in NoteColorSquares)
            {
                Controls.Remove(control);
                control.Dispose();
            }

            NoteColorSquares.Clear();
            Controls.Remove(NoteColorHoverSquare);

            for (int i = 0; i < setting.Count; i++)
            {
                float colorWidth = 75f / setting.Count;
                GuiSquare square = new(NoteColorPicker.OriginRect.X + 210 + i * colorWidth, NoteColorPicker.OriginRect.Y, colorWidth, 50, setting[i]);

                Controls.Add(square);
                NoteColorSquares.Add(square);
            }

            Controls.Add(NoteColorHoverSquare);
            

            OnResize(MainWindow.Instance.ClientSize);
            OnMouseMove(MainWindow.Instance.Mouse);
        }

        private void RefreshRhythiaPath()
        {
            string path = Settings.rhythiaPath.Value;
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
                    Platform.OpenDirectory();

                    break;

                case 3:
                    MainWindow.Instance.SwitchWindow(new GuiWindowKeybinds());

                    break;

                case 4:
                    ColorDialog dialog1 = new()
                    {
                        Color = Settings.color1.Value
                    };

                    if (dialog1.ShowDialog() == DialogResult.OK)
                    {
                        Settings.color1.Value = dialog1.Color;
                        Settings.RefreshColors();
                    }

                    break;

                case 5:
                    ColorDialog dialog2 = new()
                    {
                        Color = Settings.color2.Value
                    };

                    if (dialog2.ShowDialog() == DialogResult.OK)
                    {
                        Settings.color2.Value = dialog2.Color;
                        Settings.RefreshColors();
                    }

                    break;

                case 6:
                    ColorDialog dialog3 = new()
                    {
                        Color = Settings.color3.Value
                    };

                    if (dialog3.ShowDialog() == DialogResult.OK)
                    {
                        Settings.color3.Value = dialog3.Color;
                        Settings.RefreshColors();
                    }

                    break;

                case 7:
                    ColorDialog dialog4 = new()
                    {
                        Color = Settings.color4.Value
                    };

                    if (dialog4.ShowDialog() == DialogResult.OK)
                    {
                        Settings.color4.Value = dialog4.Color;
                        Settings.RefreshColors();
                    }

                    break;

                case 8:
                    if (Settings.noteColors.Value.Count < 32)
                    {
                        ColorDialog dialogN = new()
                        {
                            Color = Color.White
                        };

                        if (dialogN.ShowDialog() == DialogResult.OK)
                        {
                            Settings.noteColors.Value.Add(dialogN.Color);
                            RefreshNoteColors();
                            Settings.RefreshColors();
                        }
                    }

                    break;

                case 9:
                    DialogResult result = new OpenFileDialog()
                    {
                        Title = "Select Rhythia Executable",
                        Filter = Platform.ExecutableFilter
                    }.RunWithSetting(Settings.rhythiaFolderPath, out string fileName);

                    if (result == DialogResult.OK)
                    {
                        Settings.rhythiaPath.Value = fileName;
                        RefreshRhythiaPath();
                    }

                    break;

                case 10:
                    MainWindow.Instance.SwitchWindow(new GuiWindowLanguage());

                    break;

                case 11:
                    ColorDialog dialog5 = new()
                    {
                        Color = Settings.color5.Value
                    };

                    if (dialog5.ShowDialog() == DialogResult.OK)
                    {
                        Settings.color5.Value = dialog5.Color;
                        Settings.RefreshColors();
                    }

                    break;
            }

            base.OnButtonClicked(id);
        }
    }
}
