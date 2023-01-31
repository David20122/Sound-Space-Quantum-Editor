using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Json;
using OpenTK.Input;

namespace Sound_Space_Editor
{
	class Settings
	{
        public static Dictionary<string, dynamic> settings = new Dictionary<string, dynamic>
        {
            {"waveform", true },
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

            {"editorBGOpacity", 255 },
            {"gridOpacity", 255 },
            {"trackOpacity", 255 },
            {"autosaveInterval", 5 },
            {"sfxOffset", 0 },
            {"exportOffset", 0 },
            {"bezierDivisor", 4 },
            {"sensitivity", 1 },
            {"parallax", 1 },
            {"approachDistance", 1 },

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

            {"noteColors", new List<Color>() { Color.FromArgb(255, 0, 255), Color.FromArgb(0, 255, 200) } },
            {"mergedColor", Color.FromArgb(0, 0, 0) },

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

            {"selectAll", new Keybind(Key.A, true, false, false) },
            {"save", new Keybind(Key.S, true, false, false) },
            {"saveAs", new Keybind(Key.S, true, false, true) },
            {"undo", new Keybind(Key.Z, true, false, false) },
            {"redo", new Keybind(Key.Y, true, false, false) },
            {"copy", new Keybind(Key.C, true, false, false) },
            {"paste", new Keybind(Key.V, true, false, false) },
            {"cut", new Keybind(Key.X, true, false, false) },
            {"delete", new Keybind(Key.Delete, false, false, false) },
            {"hFlip", new Keybind(Key.H, false, false, true) },
            {"vFlip", new Keybind(Key.V, false, false, true) },
            {"switchClickTool", new Keybind(Key.Tab, false, false, false) },
            {"quantum", new Keybind(Key.Q, true, false, false) },
            {"openTimings", new Keybind(Key.T, true, false, false) },
            {"openBookmarks", new Keybind(Key.B, true, false, false) },
            {"storeNodes", new Keybind(Key.S, false, false, true) },
            {"drawBezier", new Keybind(Key.D, false, false, true) },
            {"anchorNode", new Keybind(Key.A, false, false, true) },
            {"openDirectory", new Keybind(Key.D, true, false, true) },
            {"exportSSPM", new Keybind(Key.E, true, true, false) },

            {"gridKeys", new List<Key> {Key.Q, Key.W, Key.E, Key.A, Key.S, Key.D, Key.Z, Key.X, Key.C } },

            {"patterns", new List<string> {"", "", "", "", "", "", "", "", "", "" } }
        };

        private static List<Key> numpadKeys = new List<Key> { Key.Keypad7, Key.Keypad8, Key.Keypad9, Key.Keypad4, Key.Keypad5, Key.Keypad6, Key.Keypad1, Key.Keypad2, Key.Keypad3 };
        private static List<Key> patternKeys = new List<Key> { Key.Number0, Key.Number1, Key.Number2, Key.Number3, Key.Number4, Key.Number5, Key.Number6, Key.Number7, Key.Number8, Key.Number9 };

        //lets hope shallow works
        public static Dictionary<string, dynamic> settingsCloned = new Dictionary<string, dynamic>(settings);

        public static void Reset()
        {
            var keptSet = new List<string> { "autosavedFile", "autosavedProperties", "lastFile", "defaultPath", "audioPath", "exportPath", "coverPath", "importPath", "patterns" };
            var kept = new Dictionary<string, dynamic>();

            foreach (var item in keptSet)
                kept.Add(item, settings[item]);

            settings = new Dictionary<string, dynamic>(settingsCloned);
            settings["mergedColor"] = MergeColors();

            foreach (var key in kept)
                settings[key.Key] = key.Value;
        }

        public static void RefreshKeymapping()
        {
            var keys = settings["numpad"] ? numpadKeys : settings["gridKeys"];
            MainWindow.Instance.KeyMapping.Clear();

            for (int i = 0; i < 9; i++)
                MainWindow.Instance.KeyMapping.Add(keys[i], new Tuple<int, int>(i % 3, (int)(i / 3f)));

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
                                settings[setting.Key] = ConvertToSliderSetting(value);
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
                                var keys = new List<Key>();

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
                    catch
                    { Console.WriteLine($"Failed to update setting - {setting.Key}"); }
                }

                settings["mergedColor"] = MergeColors();
            }
            catch
            {
                Console.WriteLine("Failed to load settings");
                Reset();
            }

            MainWindow.Instance.MusicPlayer.Volume = settings["masterVolume"].Value;
            MainWindow.Instance.SoundPlayer.Volume = settings["sfxVolume"].Value;

            RefreshKeymapping();
        }

        public static void Save()
        {
            var finaljson = new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>());

            foreach (var setting in settings)
            {
                if (setting.Value.GetType() == typeof(Color))
                    finaljson.Add(setting.Key, new JsonArray(setting.Value.R, setting.Value.G, setting.Value.B));
                else if (setting.Value.GetType() == typeof(Keybind))
                    finaljson.Add(setting.Key, new JsonArray(setting.Value.Key.ToString(), setting.Value.CTRL, setting.Value.SHIFT, setting.Value.ALT));
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

        private static Color MergeColors()
        {
            float r = 0, g = 0, b = 0;
            var colors = settings["noteColors"];

            foreach (var color in colors)
            {
                r += color.R;
                g += color.G;
                b += color.B;
            }

            r /= colors.Count;
            g /= colors.Count;
            b /= colors.Count;

            return Color.FromArgb(1, (int)r, (int)g, (int)b);
        }

        private static Key ConvertToKey(JsonValue value)
        {
            return (Key)Enum.Parse(typeof(Key), value, true);
        }

        private static Keybind ConvertToKeybind(JsonValue value)
        {
            var key = ConvertToKey(value[0]);
            //reversed shift and alt in the original settings from what i prefer... whoops
            return new Keybind(key, value[1], value[3], value[2]);
        }

        private static SliderSetting ConvertToSliderSetting(JsonValue value)
        {
            return new SliderSetting(value[0], value[1], value[2]);
        }

        public static string CompareKeybind(Key key, bool ctrl, bool alt, bool shift)
        {
            try
            {
                if (key == Key.BackSpace)
                    key = Key.Delete;

                var keycloned = new List<Key>(MainWindow.Instance.KeyMapping.Keys);

                foreach (var setting in settings.Keys)
                {
                    var value = settings[setting];

                    if (value.GetType() == typeof(Keybind) && value.Key == key && value.CTRL == ctrl && value.ALT == alt && value.SHIFT == shift)
                        return setting;
                }

                foreach (var gridkey in keycloned)
                {
                    var value = MainWindow.Instance.KeyMapping[gridkey];

                    if (gridkey == key)
                        return $"gridKey{value.Item1}|{value.Item2}";
                }

                for (int i = 0; i < patternKeys.Count; i++)
                    if (patternKeys[i] == key)
                        return $"pattern{i}";
            }
            catch { }

            return "";
        }

        public static string FormatJson(string json, string indent = "     ")
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

    [Serializable]
    class SliderSetting
    {
        public float Value;
        public float Max;
        public float Step;

        public SliderSetting(float value, float max, float step)
        {
            Value = value;
            Max = max;
            Step = step;
        }
    }

    [Serializable]
    class Keybind
    {
        public Key Key;
        public bool CTRL;
        public bool ALT;
        public bool SHIFT;

        public Keybind(Key key, bool ctrl, bool alt, bool shift)
        {
            Key = key;
            CTRL = ctrl;
            ALT = alt;
            SHIFT = shift;
        }
    }

    [Serializable]
    class ListSetting
    {
        public string Current;
        public List<string> Possible;

        public ListSetting(string current, params string[] possible)
        {
            Current = current;
            Possible = possible.ToList();
        }
    }
}
