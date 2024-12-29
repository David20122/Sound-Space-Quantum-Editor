using SSQE_Player.Types;
using System.Drawing;
using System.Json;
using System.Reflection;

namespace SSQE_Player
{
    internal class Setting<T>
    {
        public T Value;

        public static implicit operator Setting<T>(T value)
        {
            return new() { Value = value };
        }
    }

    internal class Settings
    {
        public static Setting<float> sensitivity = 1f;
        public static Setting<float> parallax = 1f;
        public static Setting<float> approachDistance = 1f;
        public static Setting<float> hitWindow = 55f;
        public static Setting<float> fov = 70f;
        public static Setting<float> noteScale = 1f;
        public static Setting<float> cursorScale = 1f;

        public static Setting<ListSetting> cameraMode = new ListSetting("half lock", "half lock", "full lock", "spin");
        public static Setting<string> hitSound = "hit";

        public static Setting<bool> lockCursor = true;
        public static Setting<bool> fullscreenPlayer = true;
        public static Setting<bool> approachFade = false;
        public static Setting<bool> gridGuides = false;
        public static Setting<bool> useVSync = false;
        public static Setting<bool> limitPlayerFPS = false;
        public static Setting<bool> notePushback = false;

        public static Setting<SliderSetting> currentTime = new SliderSetting(0f, 0f, 0f);
        public static Setting<SliderSetting> tempo = new SliderSetting(0.9f, 1.4f, 0.05f);
        public static Setting<SliderSetting> playerApproachRate = new SliderSetting(11, 29, 1);

        public static Setting<SliderSetting> masterVolume = new SliderSetting(0.05f, 1, 0.01f);
        public static Setting<SliderSetting> sfxVolume = new SliderSetting(0.1f, 1, 0.01f);
        public static Setting<SliderSetting> fpsLimit = new SliderSetting(60f, 305f, 5f);

        public static Setting<Color> color1 = Color.FromArgb(0, 255, 200);
        public static Setting<Color> color2 = Color.FromArgb(255, 0, 255);
        public static Setting<Color> color3 = Color.FromArgb(255, 0, 0);

        public static Setting<List<Color>> noteColors = new List<Color>() { Color.FromArgb(255, 0, 255), Color.FromArgb(0, 255, 200) };
        public static Setting<bool> msaa = true;

        public static void Load()
        {
            try
            {
                JsonObject result = (JsonObject)JsonValue.Parse(File.ReadAllText("settings.txt"));

                foreach (FieldInfo key in typeof(Settings).GetFields())
                {
                    try
                    {
                        dynamic? setting = key.GetValue(null);

                        if (result.TryGetValue(key.Name, out JsonValue value) && setting != null)
                        {
                            if (setting is Setting<Color> color)
                                color.Value = Color.FromArgb(value[0], value[1], value[2]);
                            else if (setting is Setting<SliderSetting> slider)
                                slider.Value = ConvertToSliderSetting(value, slider.Value.Default, slider.Value.Decimals);
                            else if (setting is Setting<ListSetting> list)
                                list.Value.Current = value;
                            else if (key.Name == "noteColors" && setting is Setting<List<Color>> colors)
                            {
                                List<Color> temp = new();

                                foreach (JsonArray c in value)
                                    temp.Add(Color.FromArgb(c[0], c[1], c[2]));

                                colors.Value = temp;
                            }
                            else
                                setting!.Value = value;
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Failed to update setting - {key.Name}");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Failed to load settings");
            }
        }

        private static SliderSetting ConvertToSliderSetting(JsonValue value, float defaultVal, int decimalVal)
        {
            return new(value[0], value[1], value[2]) { Default = defaultVal, Decimals = decimalVal };
        }
    }
}
