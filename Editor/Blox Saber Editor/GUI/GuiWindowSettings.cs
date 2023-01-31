using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;
using System;
using System.Collections.Generic;
using OpenTK;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sound_Space_Editor.GUI
{
	class GuiWindowSettings : Gui
	{
        private readonly GuiButton BackButton = new GuiButton(655, 930, 600, 100, 0, "SAVE AND RETURN", 48, false, false, "square");
        private readonly GuiButton ResetButton = new GuiButton(700, 865, 500, 50, 1, "RESET TO DEFAULT", 24, false, false, "square");
        private readonly GuiButton OpenDirectoryButton = new GuiButton(700, 810, 500, 50, 2, "OPEN EDITOR FOLDER", 24, false, false, "square");
        private readonly GuiButton KeybindsButton = new GuiButton(700, 755, 500, 50, 3, "CHANGE KEYBINDS", 24, false, false, "square");

        private readonly GuiButton Color1Picker = new GuiButton(160, 210, 200, 50, 4, "PICK COLOR", 24, false, false, "square");
        private readonly GuiButton Color2Picker = new GuiButton(160, 360, 200, 50, 5, "PICK COLOR", 24, false, false, "square");
        private readonly GuiButton Color3Picker = new GuiButton(160, 510, 200, 50, 6, "PICK COLOR", 24, false, false, "square");
        private readonly GuiButton NoteColorPicker = new GuiButton(160, 660, 200, 50, 7, "ADD COLOR", 24, false, false, "square");

        private readonly GuiCheckbox WaveformCheckbox = new GuiCheckbox(1435, 195, 72, 72, "waveform", "Waveform", 32);
        private readonly GuiCheckbox AutosaveCheckbox = new GuiCheckbox(800, 195, 72, 72, "enableAutosave", "Enable Autosave", 32);
        private readonly GuiCheckbox CorrectOnCopyCheckbox = new GuiCheckbox(800, 460, 72, 72, "correctOnCopy", "Correct Errors on Copy", 32);
        private readonly GuiCheckbox SkipDownloadCheckbox = new GuiCheckbox(800, 580, 72, 72, "skipDownload", "Skip Attempt to Download from Roblox", 32);

        private readonly GuiTextbox EditorBGOpacityTextbox = new GuiTextbox(1435, 360, 200, 50, "", 28, true, false, false, "editorBGOpacity");
        private readonly GuiTextbox GridOpacityTextbox = new GuiTextbox(1435, 510, 200, 50, "", 28, true, false, false, "gridOpacity");
        private readonly GuiTextbox TrackOpacityTextbox = new GuiTextbox(1435, 660, 200, 50, "", 28, true, false, false, "trackOpacity");
        private readonly GuiTextbox AutosaveIntervalTextbox = new GuiTextbox(800, 360, 200, 50, "", 28, true, false, false, "autosaveInterval", "main", false, true);

        private readonly List<GuiButton> ColorPickers = new List<GuiButton>();
        private readonly List<GuiTextbox> Opacities = new List<GuiTextbox>();
        private readonly List<string> OpacityLabels = new List<string> { "Editor BG Opacity:", "Grid Opacity:", "Track Opacity:" };

        private int hoveringColor;

        private int txid;
        private bool bgImg;

        private int resetQueryTime;
        private bool resetQuery;

        public GuiWindowSettings() : base(0, 0, MainWindow.Instance.ClientSize.Width, MainWindow.Instance.ClientSize.Height)
        {
            buttons = new List<GuiButton> { BackButton, ResetButton, OpenDirectoryButton, KeybindsButton, Color1Picker, Color2Picker, Color3Picker, NoteColorPicker };
            checkboxes = new List<GuiCheckbox> { WaveformCheckbox, AutosaveCheckbox, CorrectOnCopyCheckbox, SkipDownloadCheckbox };
            boxes = new List<GuiTextbox> { EditorBGOpacityTextbox, GridOpacityTextbox, TrackOpacityTextbox, AutosaveIntervalTextbox };

            ColorPickers.Add(Color1Picker);
            ColorPickers.Add(Color2Picker);
            ColorPickers.Add(Color3Picker);

            Opacities.Add(EditorBGOpacityTextbox);
            Opacities.Add(GridOpacityTextbox);
            Opacities.Add(TrackOpacityTextbox);

            EditorBGOpacityTextbox.text = Settings.settings["editorBGOpacity"].ToString();
            GridOpacityTextbox.text = Settings.settings["gridOpacity"].ToString();
            TrackOpacityTextbox.text = Settings.settings["trackOpacity"].ToString();

            OnResize(MainWindow.Instance.ClientSize);

            if (File.Exists("background_menu.png"))
            {
                bgImg = true;

                using (Bitmap img = new Bitmap("background_menu.png"))
                    txid = TextureManager.GetOrRegister("menubg", img, true);
            }
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            var noteColors = Settings.settings["noteColors"];

            var widthdiff = rect.Width / 1920f;
            var heightdiff = rect.Height / 1080f;
            var colorWidth = 75f * widthdiff / noteColors.Count;

            GL.Color4(bgImg ? Color.FromArgb(255, 255, 255, 255) : Color.FromArgb(255, 30, 30, 30));

            if (bgImg)
                GLSpecial.TexturedQuad(rect, 0, 0, 1, 1, txid);
            else
                GLSpecial.Rect(rect);

            //colors 1-3
            for (int i = 0; i < 3; i++)
            {
                var picker = ColorPickers[i];

                GL.Color3(1f, 1f, 1f);
                RenderText($"Color {i + 1}:", picker.rect.X, picker.rect.Y - 26f, 24);

                GL.Color3(Settings.settings[$"color{i + 1}"]);
                GLSpecial.Rect(picker.rect.X + 210 * widthdiff, picker.rect.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
            }

            //note colors
            for (int i = 0; i < noteColors.Count; i++)
            {
                var colorRect = new RectangleF(NoteColorPicker.rect.X + 210 * widthdiff + colorWidth * i, NoteColorPicker.rect.Y - 15 * heightdiff, colorWidth * widthdiff, 75 * heightdiff);

                GL.Color3(noteColors[i]);
                GLSpecial.Rect(colorRect);

                if (hoveringColor == i)
                {
                    GL.LineWidth(2f);
                    GL.Color3(0f, 0.5f, 1f);
                    GLSpecial.Outline(colorRect);
                }
            }

            GL.Color3(1f, 1f, 1f);
            RenderText("LMB: Remove\nRMB: Move left", NoteColorPicker.rect.X + 5f * widthdiff, NoteColorPicker.rect.Bottom + 10f * heightdiff, 24);

            //opacities
            for (int i = 0; i < 3; i++)
            {
                var opacityBox = Opacities[i];

                int.TryParse(opacityBox.text, out int opacity);
                opacity = MathHelper.Clamp(opacity, 0, 255);

                opacityBox.text = opacity.ToString();

                GL.Color3(1f, 1f, 1f);
                RenderText(OpacityLabels[i], opacityBox.rect.X, opacityBox.rect.Y - 26f, 24);

                GL.Color4(Color.FromArgb(opacity, 255, 255, 255));
                GLSpecial.Rect(opacityBox.rect.X + 210 * widthdiff, opacityBox.rect.Y - 15 * heightdiff, 75 * widthdiff, 75 * heightdiff);
            }

            //labels
            GL.Color3(1f, 1f, 1f);
            RenderText("Note Colors:", NoteColorPicker.rect.X, NoteColorPicker.rect.Y - 26f, 24);
            RenderText("Autosave Interval (min):", AutosaveIntervalTextbox.rect.X, AutosaveIntervalTextbox.rect.Y - 26f, 24);

            base.Render(mousex, mousey, frametime);
        }

        public override void OnResize(Size size)
        {
            rect = new RectangleF(0, 0, size.Width, size.Height);

            base.OnResize(size);
        }

        public override void OnMouseClick(Point pos, bool right = false)
        {
            var setting = Settings.settings["noteColors"];

            if (hoveringColor >= 0)
            {
                if (!right)
                    setting.RemoveAt(hoveringColor);
                else if (hoveringColor > 0)
                {
                    var old = setting[hoveringColor - 1];

                    setting[hoveringColor - 1] = setting[hoveringColor];
                    setting[hoveringColor] = old;
                }

            }

            base.OnMouseClick(pos, right);
        }

        public override void OnMouseMove(Point pos)
        {
            var widthdiff = rect.Width / 1920f;
            var heightdiff = rect.Height / 1080f;

            var setting = Settings.settings["noteColors"];

            var x = pos.X - (NoteColorPicker.rect.X + 210 * widthdiff);
            var y = pos.Y - (NoteColorPicker.rect.Y - 15 * widthdiff);
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
                    ResetButton.text = "RESET TO DEFAULT";
                }
            });
        }

        protected override void OnButtonClicked(int id)
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
                        ResetButton.text = "RESET TO DEFAULT";
                    }
                    else
                    {
                        resetQuery = true;
                        RunQueryReset(DateTime.Now.Millisecond);

                        ResetButton.text = "ARE YOU SURE?";
                    }

                    break;

                case 2:
                    Process.Start(Environment.CurrentDirectory);

                    break;

                case 3:
                    MainWindow.Instance.SwitchWindow(new GuiWindowKeybinds());

                    break;

                case 4:
                    using (var dialog = new ColorDialog() { Color = Settings.settings["color1"] })
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                            Settings.settings["color1"] = dialog.Color;
                    }

                    break;

                case 5:
                    using (var dialog = new ColorDialog() { Color = Settings.settings["color2"] })
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                            Settings.settings["color2"] = dialog.Color;
                    }

                    break;

                case 6:
                    using (var dialog = new ColorDialog() { Color = Settings.settings["color3"] })
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                            Settings.settings["color3"] = dialog.Color;
                    }

                    break;

                case 7:
                    using (var dialog = new ColorDialog() { Color = Color.White })
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                            Settings.settings["noteColors"].Add(dialog.Color);
                    }

                    break;
            }
        }
    }
}