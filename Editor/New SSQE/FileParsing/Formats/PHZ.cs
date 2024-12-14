using New_SSQE.Misc.Network;
using New_SSQE.Misc.Static;
using System.Text.Json;

namespace New_SSQE.FileParsing.Formats
{
    internal class PHZ
    {
        public static bool IsValid(string path)
        {
            Dictionary<string, JsonElement> result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(path)) ?? new();
            bool beat = result.TryGetValue("beat", out JsonElement beats);
            bool song = result.TryGetValue("song", out _);

            return beat && song && JsonSerializer.Deserialize<JsonElement[]>(beats) != null;
        }

        public static string Parse(string path)
        {
            if (!File.Exists(path))
                return "";

            List<string> data = [];
            string id = "processing";
            float bpm = 120;

            Dictionary<string, JsonElement> result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(path)) ?? new();
            JsonElement[] beats = [];
            double offset = 0;

            foreach (string key in result.Keys)
            {
                JsonElement value = result[key];

                switch (key)
                {
                    case "bpm":
                        bpm = value.GetSingle();
                        break;
                    case "song":
                        id = $"{value.GetInt32()}";
                        break;
                    case "beat":
                        beats = JsonSerializer.Deserialize<JsonElement[]>(value) ?? beats;
                        break;
                    case "songOffset":
                        offset = value.GetDouble() / 1000;
                        break;
                }
            }

            foreach (JsonElement beat in beats)
            {
                JsonElement[] values = JsonSerializer.Deserialize<JsonElement[]>(beat) ?? [];

                if (values.Length >= 2)
                {
                    int tile = values[0].GetInt32();
                    double time = values[1].GetDouble() / (bpm / 60) + offset;

                    data.Add($"{2 - tile % 3}|{2 - tile / 3}|{(long)(time * 1000)}");
                }
            }

            if (!File.Exists($"{Assets.CACHED}\\{id}.asset"))
                WebClient.DownloadFile($"https://pulsus.cc/play/client/s/{id}.mp3", $"{Assets.CACHED}\\{id}.asset", FileSource.Pulsus);

            return $"{id},{string.Join(',', data)}";
        }

        public static void Save(string path)
        {

        }
    }
}
