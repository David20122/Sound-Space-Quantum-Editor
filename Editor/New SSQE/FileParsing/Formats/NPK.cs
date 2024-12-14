using New_SSQE.Audio;
using New_SSQE.Maps;
using New_SSQE.Misc.Static;
using New_SSQE.Objects;
using New_SSQE.Preferences;
using System.IO.Compression;
using System.Text.Json;
using Un4seen.Bass;

namespace New_SSQE.FileParsing.Formats
{
    internal class NPK
    {
        public static string Parse(string path)
        {
            string directory = Path.GetDirectoryName(path) ?? "";
            Dictionary<string, JsonElement> result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(path)) ?? new();

            string[] notes = Array.Empty<string>();
            string[] beats = Array.Empty<string>();

            foreach (string key in result.Keys)
            {
                JsonElement value = result[key];

                switch (key)
                {
                    case "songOffset":
                        Settings.songOffset.Value = value.GetDouble().ToString(Program.Culture);
                        break;

                    case "notes":
                        Dictionary<string, JsonElement>[] noteSet = value.Deserialize<Dictionary<string, JsonElement>[]>() ?? Array.Empty<Dictionary<string, JsonElement>>();
                        notes = new string[noteSet.Length];

                        long prevDiffN = 0;
                        long prevMsN = 0;

                        for (int i = 0; i < noteSet.Length; i++)
                        {
                            Dictionary<string, JsonElement> note = noteSet[i];
                            float x = 1 - note["x"].GetSingle();
                            float y = note["y"].GetSingle() + 1;
                            long t = note["t"].GetInt64();

                            long diff = t - prevMsN;
                            long offset = diff - prevDiffN;

                            prevDiffN = diff;
                            prevMsN = t;

                            notes[i] = $"{offset}|{x.ToString(Program.Culture)}|{y.ToString(Program.Culture)}";
                        }

                        break;

                    case "beats":
                        Dictionary<string, JsonElement>[] beatSet = value.Deserialize<Dictionary<string, JsonElement>[]>() ?? Array.Empty<Dictionary<string, JsonElement>>();
                        beats = new string[beatSet.Length];

                        long prevDiffB = 0;
                        long prevMsB = 0;

                        for (int i = 0; i < beatSet.Length; i++)
                        {
                            Dictionary<string, JsonElement> beat = beatSet[i];
                            long t = beat["t"].GetInt64();

                            long diff = t - prevMsB;
                            long offset = diff - prevDiffB;

                            prevDiffB = diff;
                            prevMsB = t;

                            beats[i] = offset.ToString();
                        }

                        break;
                }
            }

            string[] data = new string[13]
            { // beats are id 12 so some padding is required for data alignment
                string.Join(',', notes),
                "", "", "", "", "", "", "", "", "", "", "",
                string.Join(',', beats)
            };

            string id = "";

            string profile = "";
            string icon = "";

            foreach (string file in Directory.GetFiles(directory))
            {
                string filename = Path.GetFileNameWithoutExtension(file);

                switch (Path.GetExtension(file))
                {
                    case ".json" when filename == "metadata":
                        Dictionary<string, JsonElement> metadata = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(file)) ?? new();

                        foreach (string key in metadata.Keys)
                        {
                            try
                            {
                                JsonElement value = metadata[key];

                                switch (key)
                                {
                                    case "songTitle":
                                        Settings.songTitle.Value = value.GetString() ?? "";
                                        break;
                                    case "songArtist":
                                        Settings.songArtist.Value = value.GetString() ?? "";
                                        break;
                                    case "mapCreator":
                                        Settings.mapCreator.Value = value.GetString() ?? "";
                                        break;
                                    case "mapCreatorPersonalLink":
                                        Settings.mapCreatorPersonalLink.Value = value.GetString() ?? "";
                                        break;
                                    case "previewStartTime":
                                        Settings.previewStartTime.Value = ((long)(value.GetDouble() * 1000d)).ToString();
                                        break;
                                    case "previewDuration":
                                        Settings.previewDuration.Value = ((long)(value.GetDouble() * 1000d)).ToString();
                                        break;
                                }
                            }
                            catch { }
                        }

                        break;

                    case ".png" when filename == "profile":
                    case ".jpg" when filename == "profile":
                        profile = file;
                        break;

                    case ".png" when filename != "profile":
                    case ".jpg" when filename != "profile":
                        icon = file;
                        break;

                    case ".mp3":
                    case ".ogg":
                    case ".wav":
                        id = filename;
                        File.Copy(file, $"{Assets.CACHED}\\{filename}.asset", true);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(profile))
            {
                string temp = $"{Assets.CACHED}\\{id}-profile{Path.GetExtension(profile)}";
                File.Copy(profile, temp, true);
                Settings.novaIcon.Value = temp;
            }

            if (!string.IsNullOrWhiteSpace(icon))
            {
                string temp = $"{Assets.CACHED}\\{id}-icon{Path.GetExtension(icon)}";
                File.Copy(icon, temp, true);
                Settings.novaCover.Value = temp;
                Settings.cover.Value = temp;
                Settings.useCover.Value = true;
            }

            Settings.songName.Value = $"{Settings.songArtist.Value} - {Settings.songTitle.Value}";
            Settings.mappers.Value = Settings.mapCreator.Value;

            return $"ssmapv2`{id.Replace('`', '_')}`0`N/A`{string.Join('/', data)}";
        }

        public static void Save(string path)
        {
            string id = CurrentMap.SoundID;
            Dictionary<string, string> info = Exporting.NovaInfo;

            string temp = $"{Assets.TEMP}\\nova";
            Directory.CreateDirectory(temp);
            foreach (string file in Directory.GetFiles(temp))
                File.Delete(file);

            string extension = MusicPlayer.ctype switch
            {
                BASSChannelType.BASS_CTYPE_STREAM_MP3 => ".mp3",
                BASSChannelType.BASS_CTYPE_STREAM_OGG => ".ogg",
                _ => throw new FormatException($"AUDIO - not MP3/OGG ({MusicPlayer.ctype})"),
            };

            File.Copy(info["coverPath"], $"{temp}\\{id}{Path.GetExtension(info["coverPath"])}", true);
            File.Copy(info["iconPath"], $"{temp}\\profile{Path.GetExtension(info["iconPath"])}", true);
            File.Copy($"{Assets.CACHED}\\{id}.asset", $"{temp}\\{id}{extension}", true);

            Dictionary<string, object>[] notes = new Dictionary<string, object>[CurrentMap.Notes.Count];
            for (int i = 0; i < CurrentMap.Notes.Count; i++)
            {
                Note note = CurrentMap.Notes[i];

                notes[i] = new()
                {
                    {"x", 1 - note.X },
                    {"y", note.Y - 1 },
                    {"t", note.Ms }
                };
            }

            List<Dictionary<string, object>> beats = new();
            foreach (MapObject obj in CurrentMap.SpecialObjects)
            {
                if (obj is Beat beat)
                {
                    beats.Add(new()
                    {
                        {"t", beat.Ms }
                    });
                }
            }

            Dictionary<string, object> chart = new()
            {
                {"songOffset", long.Parse(info["songOffset"]) },
                {"notes", notes },
                {"beats", beats }
            };

            File.WriteAllText($"{temp}\\chart.nch", JsonSerializer.Serialize(chart, Program.JsonOptions));

            Dictionary<string, object> metadata = new()
            {
                {"songTitle", info["songTitle"] },
                {"songArtist", info["songArtist"] },
                {"mapCreator", info["mapCreator"] },
                {"mapCreatorPersonalLink", info["mapCreatorPersonalLink"] },
                {"previewStartTime", long.Parse(info["previewStartTime"]) / 1000f },
                {"previewDuration", long.Parse(info["previewDuration"]) / 1000f }
            };

            File.WriteAllText($"{temp}\\metadata.json", JsonSerializer.Serialize(metadata, Program.JsonOptions));

            if (File.Exists(path))
                File.Delete(path);
            ZipFile.CreateFromDirectory(temp, path);
            foreach (string file in Directory.GetFiles(temp))
                File.Delete(file);
        }
    }
}
