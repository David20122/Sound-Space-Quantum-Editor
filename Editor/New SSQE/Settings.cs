using System.Collections.Generic;
using System.Drawing;
using System.Json;
using System;
using System.IO;
using System.Linq;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using New_SSQE.GUI;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace New_SSQE
{
    internal class Settings
    {
        public static Dictionary<string, dynamic> settings = new()
        {
            {"waveform", true },
            {"classicWaveform", false },
            {"enableAutosave", true },
            {"correctOnCopy", true },
            {"gridNumbers", false },
            {"approachSquares", false },
            {"animateBackground", false },
            {"autoplay", true },
            {"numpad", false },
            {"quantumGridLines", true },
            {"quantumGridSnap", true },
            {"metronome", false },
            {"separateClickTools", false },
            {"enableQuantum", false },
            {"autoAdvance", false },
            {"selectTool", false },
            {"curveBezier", true },
            {"gridLetters", true },
            {"exportWarningShown", false },
            {"skipDownload", false },
            {"lockCursor", true },
            {"fromStart", false },
            {"reverseScroll", false },
            {"useVSync", false },
            {"checkUpdates", true },
            {"fullscreenPlayer", true },
            {"approachFade", true },
            {"applyOnPaste", false },

            {"editorBGOpacity", 255 },
            {"gridOpacity", 255 },
            {"trackOpacity", 255 },
            {"autosaveInterval", 5 },
            {"waveformDetail", 5 },
            {"sfxOffset", 0 },
            {"exportOffset", 0 },
            {"bezierDivisor", 4 },
            {"sensitivity", 1 },
            {"parallax", 1 },
            {"approachDistance", 1 },
            {"hitWindow", 55 },

            {"autosavedFile", "" },
            {"autosavedProperties", "" },
            {"lastFile", "" },
            {"clickSound", "click" },
            {"hitSound", "hit" },
            {"defaultPath", "" },
            {"audioPath", "" },
            {"exportPath" , "" },
            {"coverPath", "" },
            {"importPath", "" },
            {"cameraMode", new ListSetting("half lock", "half lock", "full lock", "spin") },

            {"color1", Color.FromArgb(0, 255, 200) },
            {"color2", Color.FromArgb(255, 0, 255) },
            {"color3", Color.FromArgb(255, 0, 100) },
            {"color4", Color.FromArgb(90, 90, 90) },

            {"noteColors", new List<Color>() { Color.FromArgb(255, 0, 255), Color.FromArgb(0, 255, 200) } },

            {"trackHeight", new SliderSetting(16, 32, 1) },
            {"cursorPos", new SliderSetting(40, 100, 1) },
            {"approachRate", new SliderSetting(9, 29, 1) },
            {"playerApproachRate", new SliderSetting(9, 29, 1) },
            {"masterVolume", new SliderSetting(0.05f, 1, 0.01f) },
            {"sfxVolume", new SliderSetting(0.1f, 1, 0.01f) },

            {"currentTime", new SliderSetting(0f, 0f, 0f) },
            {"beatDivisor", new SliderSetting(3f, 31f, 0.5f) },
            {"tempo", new SliderSetting(0.9f, 1.4f, 0.05f) },
            {"quantumSnapping", new SliderSetting(0f, 57f, 1f) },

            {"changelogPosition", new SliderSetting(0f, 0f, 1f) },

            {"selectAll", new Keybind(Keys.A, true, false, false) },
            {"save", new Keybind(Keys.S, true, false, false) },
            {"saveAs", new Keybind(Keys.S, true, false, true) },
            {"undo", new Keybind(Keys.Z, true, false, false) },
            {"redo", new Keybind(Keys.Y, true, false, false) },
            {"copy", new Keybind(Keys.C, true, false, false) },
            {"paste", new Keybind(Keys.V, true, false, false) },
            {"cut", new Keybind(Keys.X, true, false, false) },
            {"delete", new Keybind(Keys.Delete, false, false, false) },
            {"hFlip", new Keybind(Keys.H, false, false, true) },
            {"vFlip", new Keybind(Keys.V, false, false, true) },
            {"switchClickTool", new Keybind(Keys.Tab, false, false, false) },
            {"quantum", new Keybind(Keys.Q, true, false, false) },
            {"openTimings", new Keybind(Keys.T, true, false, false) },
            {"openBookmarks", new Keybind(Keys.B, true, false, false) },
            {"storeNodes", new Keybind(Keys.S, false, false, true) },
            {"drawBezier", new Keybind(Keys.D, false, false, true) },
            {"anchorNode", new Keybind(Keys.A, false, false, true) },
            {"openDirectory", new Keybind(Keys.D, true, false, true) },
            {"exportSSPM", new Keybind(Keys.E, true, true, false) },

            {"gridKeys", new List<Keys> { Keys.Q, Keys.W, Keys.E, Keys.A, Keys.S, Keys.D, Keys.Z, Keys.X, Keys.C } },

            {"patterns", new List<string> {"", "", "", "", "", "", "", "", "", "" } },

            {"mappers", "" },
            {"songName", "" },
            {"difficulty", "N/A" },
            {"useCover", true },
            {"cover", "Default" },

            {"debugMode", false },
        };

        private static readonly List<Keys> numpadKeys = new() { Keys.KeyPad7, Keys.KeyPad8, Keys.KeyPad9, Keys.KeyPad4, Keys.KeyPad5, Keys.KeyPad6, Keys.KeyPad1, Keys.KeyPad2, Keys.KeyPad3 };
        private static readonly List<Keys> patternKeys = new() { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9 };

        private static readonly Dictionary<string, dynamic> settingsCloned = new(settings);

        private static bool isLoaded = false;

        public static void Reset()
        {
            var keptSet = new List<string> { "autosavedFile", "autosavedProperties", "lastFile", "defaultPath", "audioPath", "exportPath", "coverPath", "importPath", "patterns" };
            var kept = new Dictionary<string, dynamic>();

            foreach (var key in keptSet)
                kept.Add(key, settings[key]);

            settings = new(settingsCloned);

            foreach (var key in kept.Keys)
                settings[key] = kept[key];
        }

        public static void RefreshColors()
        {
            GL.UseProgram(Shader.NoteInstancedProgram);
            int location = GL.GetUniformLocation(Shader.NoteInstancedProgram, "NoteColors");
            Vector4[] colors = new Vector4[32];

            for (int i = 0; i < settings["noteColors"].Count; i++)
            {
                var color = settings["noteColors"][i];
                colors[i] = (color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            }

            GL.Uniform4f(location, 32, colors);

            GL.UseProgram(Shader.GridInstancedProgram);
            location = GL.GetUniformLocation(Shader.GridInstancedProgram, "NoteColors");
            GL.Uniform4f(location, 32, colors);

            GL.UseProgram(Shader.InstancedProgram);
            location = GL.GetUniformLocation(Shader.InstancedProgram, "Colors");
            colors = new Vector4[10];

            for (int i = 0; i < 4; i++)
            {
                var color = settings[$"color{i + 1}"];
                colors[i] = (color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            }

            colors[4] = (1f, 1f, 1f, 1f);
            colors[5] = (0f, 1f, 0.25f, 1f);
            colors[6] = (0f, 0.5f, 1f, 1f);
            colors[7] = (0.75f, 0.75f, 0.75f, 1f);
            colors[8] = (1f, 0f, 0f, 1f);
            colors[9] = (0.5f, 0.5f, 0.5f, 1f);

            GL.Uniform4f(location, 10, colors);
        }

        public static void RefreshKeyMapping()
        {
            var keys = settings["numpad"] ? numpadKeys : settings["gridKeys"];
            MainWindow.Instance.KeyMapping.Clear();

            for (int i = 0; i < 9; i++)
                MainWindow.Instance.KeyMapping.Add(keys[i], new Tuple<int, int>(i % 3, i / 3));
        }

        public static void Load()
        {
            try
            {
                Reset();
                JsonObject result = (JsonObject)JsonValue.Parse(File.ReadAllText("settings.txt"));

                foreach (var setting in settingsCloned)
                {
                    try
                    {
                        if (result.TryGetValue(setting.Key, out var value))
                        {
                            if (setting.Value.GetType() == typeof(Color))
                                settings[setting.Key] = Color.FromArgb(value[0], value[1], value[2]);
                            else if (setting.Value.GetType() == typeof(Keybind))
                                settings[setting.Key] = ConvertToKeybind(value);
                            else if (setting.Value.GetType() == typeof(SliderSetting))
                                settings[setting.Key] = ConvertToSliderSetting(value, settings[setting.Key].Default);
                            else if (setting.Value.GetType() == typeof(ListSetting))
                                settings[setting.Key].Current = value;
                            else if (setting.Key == "noteColors")
                            {
                                var colors = new List<Color>();

                                foreach (JsonArray color in value)
                                    colors.Add(Color.FromArgb(color[0], color[1], color[2]));

                                settings[setting.Key] = colors;
                            }
                            else if (setting.Key == "gridKeys")
                            {
                                var keys = new List<Keys>();

                                for (int i = 0; i < 9; i++)
                                    keys.Add(ConvertToKey(value[i]));

                                settings[setting.Key] = keys;
                            }
                            else if (setting.Key == "patterns")
                            {
                                var patterns = new List<string>();

                                for (int i = 0; i < 10; i++)
                                    patterns.Add(value[i]);

                                settings[setting.Key] = patterns;
                            }
                            else if (setting.Key != "mergedColor")
                                settings[setting.Key] = value;
                        }
                    }
                    catch (Exception ex)
                    { ActionLogging.Register($"Failed to update setting - {setting.Key} - {ex.GetType().Name}\n{ex.StackTrace}", "WARN"); }
                }
            }
            catch (Exception ex)
            {
                ActionLogging.Register($"Failed to load settings - {ex.GetType().Name}\n{ex.StackTrace}", "WARN");
                Reset();
            }

            MainWindow.Instance.SetVSync(settings["useVSync"] ? VSyncMode.On : VSyncMode.Off);

            MainWindow.Instance.MusicPlayer.Volume = settings["masterVolume"].Value;
            MainWindow.Instance.SoundPlayer.Volume = settings["sfxVolume"].Value;

            RefreshKeyMapping();
            RefreshColors();

            isLoaded = true;
        }

        public static void Save()
        {
            if (!isLoaded)
                return;

            var finaljson = new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>());

            foreach (var setting in settings)
            {
                if (setting.Value.GetType() == typeof(Color))
                    finaljson.Add(setting.Key, new JsonArray(setting.Value.R, setting.Value.G, setting.Value.B));
                else if (setting.Value.GetType() == typeof(Keybind))
                    finaljson.Add(setting.Key, new JsonArray(setting.Value.Key.ToString(), setting.Value.Ctrl, setting.Value.Shift, setting.Value.Alt));
                else if (setting.Value.GetType() == typeof(SliderSetting))
                    finaljson.Add(setting.Key, new JsonArray(setting.Value.Value, setting.Value.Max, setting.Value.Step));
                else if (setting.Value.GetType() == typeof(ListSetting))
                    finaljson.Add(setting.Key, setting.Value.Current);
                else if (setting.Key == "noteColors")
                {
                    var notecolorsfinal = new JsonArray();

                    foreach (var color in setting.Value)
                        notecolorsfinal.Add(new JsonArray(color.R, color.G, color.B));

                    finaljson.Add(setting.Key, notecolorsfinal);
                }
                else if (setting.Key == "gridKeys")
                {
                    var gridkeysfinal = new JsonArray();

                    for (int i = 0; i < 9; i++)
                        gridkeysfinal.Add(setting.Value[i].ToString());

                    finaljson.Add(setting.Key, gridkeysfinal);
                }
                else if (setting.Key == "patterns")
                {
                    var patternsfinal = new JsonArray();

                    for (int i = 0; i < 10; i++)
                        patternsfinal.Add(setting.Value[i]);

                    finaljson.Add(setting.Key, patternsfinal);
                }
                else if (setting.Key != "mergedColor")
                    finaljson.Add(setting.Key, setting.Value);
            }

            try
            {
                File.WriteAllText("settings.txt", FormatJson(finaljson.ToString()));
            }
            catch { Console.WriteLine("Failed to save settings"); }

            Load();
        }

        private static Keys ConvertToKey(JsonValue value)
        {
            return (Keys)Enum.Parse(typeof(Keys), value, true);
        }

        private static Keybind ConvertToKeybind(JsonValue value)
        {
            var key = ConvertToKey(value[0]);
            // CTRL SHIFT ALT for backwards compatibility
            return new Keybind(key, value[1], value[3], value[2]);
        }

        private static SliderSetting ConvertToSliderSetting(JsonValue value, float defaultVal)
        {
            return new SliderSetting(value[0], value[1], value[2]) { Default = defaultVal };
        }

        public static string CompareKeybind(Keys key, bool ctrl, bool alt, bool shift)
        {
            try
            {
                if (key == Keys.Backspace)
                    key = Keys.Delete;

                foreach (var setting in settings.Keys)
                {
                    var value = settings[setting];

                    if (value.GetType() == typeof(Keybind) && value.Key == key && value.Ctrl == ctrl && value.Alt == alt && value.Shift == shift)
                        return setting;
                }

                var keyCloned = new List<Keys>(MainWindow.Instance.KeyMapping.Keys);

                foreach (var gridKey in keyCloned)
                {
                    var value = MainWindow.Instance.KeyMapping[gridKey];

                    if (gridKey == key)
                        return $"gridKey{value.Item1}|{value.Item2}";
                }

                for (int i = 0; i < patternKeys.Count; i++)
                    if (patternKeys[i] == key)
                        return $"pattern{i}";
            }
            catch { }

            return "";
        }

        public static string FormatJson(string json, string indent = "    ")
        {
            var indentation = 0;
            var quoteCount = 0;
            var escapeCount = 0;

            var result =
                from ch in json ?? string.Empty
                let escaped = (ch == '\\' ? escapeCount++ : escapeCount > 0 ? escapeCount-- : escapeCount) > 0
                let quotes = ch == '"' && !escaped ? quoteCount++ : quoteCount
                let unquoted = quotes % 2 == 0
                let colon = ch == ':' && unquoted ? ": " : null
                let nospace = char.IsWhiteSpace(ch) && unquoted ? string.Empty : null
                let lineBreak = ch == ',' && unquoted ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(indent, indentation)) : null
                let openChar = (ch == '{' || ch == '[') && unquoted ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(indent, ++indentation)) : ch.ToString()
                let closeChar = (ch == '}' || ch == ']') && unquoted ? Environment.NewLine + string.Concat(Enumerable.Repeat(indent, --indentation)) + ch : ch.ToString()
                select colon ?? nospace ?? lineBreak ?? (
                    openChar.Length > 1 ? openChar : closeChar
                );

            return string.Concat(result);
        }
    }
}
