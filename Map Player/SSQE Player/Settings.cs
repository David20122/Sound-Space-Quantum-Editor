using SSQE_Player.Types;
using System.Drawing;
using System.Json;

namespace SSQE_Player
{
    internal class Settings
    {
        public static Dictionary<string, dynamic> settings = new()
        {
            {"sensitivity", 1f },
            {"parallax", 1f },
            {"approachDistance", 1f },
            {"hitWindow", 55f },

            {"cameraMode", new ListSetting("half lock", "half lock", "full lock", "spin") },
            {"hitSound", "hit" },

            {"lockCursor", true },
            {"fullscreenPlayer", true },
            {"approachFade", false },

            {"currentTime", new SliderSetting(0f, 0f, 0f) },
            {"tempo", new SliderSetting(0.9f, 1.4f, 0.05f) },
            {"playerApproachRate", new SliderSetting(9, 29, 1) },

            {"masterVolume", new SliderSetting(0.05f, 1, 0.01f) },
            {"sfxVolume", new SliderSetting(0.1f, 1, 0.01f) },

            {"color1", Color.FromArgb(0, 255, 200) },
            {"color2", Color.FromArgb(255, 0, 255) },
            {"color3", Color.FromArgb(255, 0, 0) },

            {"noteColors", new List<Color>() { Color.FromArgb(255, 0, 255), Color.FromArgb(0, 255, 200) } },
        };

        private static readonly Dictionary<string, dynamic> settingsCloned = new(settings);

        public static void Load()
        {
            try
            {
                JsonObject result = (JsonObject)JsonValue.Parse(File.ReadAllText("settings.txt"));

                foreach (var setting in settingsCloned)
                {
                    try
                    {
                        if (result.TryGetValue(setting.Key, out var value))
                        {
                            if (setting.Value.GetType() == typeof(Color))
                                settings[setting.Key] = Color.FromArgb(value[0], value[1], value[2]);
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
                            else if (setting.Key != "mergedColor")
                                settings[setting.Key] = value;
                        }
                    }
                    catch
                    { Console.WriteLine($"Failed to update setting - {setting.Key}"); }
                }
            }
            catch
            {
                Console.WriteLine("Failed to load settings");
            }
        }

        private static SliderSetting ConvertToSliderSetting(JsonValue value)
        {
            return new SliderSetting(value[0], value[1], value[2]);
        }
    }
}
