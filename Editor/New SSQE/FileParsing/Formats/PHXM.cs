using New_SSQE.Misc.Static;
using New_SSQE.Preferences;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace New_SSQE.FileParsing.Formats
{
    internal class PHXM
    {
        public static string Parse(string path)
        {
            string temp = $"{Assets.TEMP}\\phxm";
            Directory.CreateDirectory(temp);
            foreach (string file in Directory.GetFiles(temp))
                File.Delete(file);

            bool hasAudio = false;
            string audioExt = "";
            string audioId = "";

            ZipFile.ExtractToDirectory(path, temp, true);
            Dictionary<string, JsonElement> metadata = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText($"{temp}\\metadata.json")) ?? new();

            foreach (string key in metadata.Keys)
            {
                JsonElement value = metadata[key];

                switch (key)
                {
                    case "Artist":
                        Settings.songArtist.Value = value.GetString() ?? "";
                        break;
                    case "AudioExt":
                        audioExt = value.GetString() ?? "";
                        break;
                    case "Difficulty":
                        Settings.difficulty.Value = Exporting.Difficulties.Keys.ToArray()[value.GetInt32()];
                        break;
                    case "DifficultyName":
                        Settings.customDifficulty.Value = value.GetString() ?? "";
                        break;
                    case "HasAudio":
                        hasAudio = value.GetBoolean();
                        break;
                    case "HasCover":
                        Settings.useCover.Value = value.GetBoolean();
                        break;
                    case "HasVideo":
                        Settings.useVideo.Value = value.GetBoolean();
                        break;
                    case "ID":
                        audioId = value.GetString() ?? "";
                        break;
                    case "Mappers":
                        string[] mappers = value.Deserialize<string[]>() ?? [];
                        Settings.mappers.Value = string.Join('\n', mappers);
                        break;
                    case "Rating":
                        Settings.rating.Value = value.GetSingle();
                        break;
                    case "Title":
                        Settings.songTitle.Value = value.GetString() ?? "";
                        break;
                }

                Settings.songName.Value = $"{Settings.songArtist.Value} - {Settings.songTitle.Value}";
            }

            if (hasAudio)
                File.Copy($"{temp}\\audio.{audioExt}", $"{Assets.CACHED}\\{audioId}.asset", true);

            if (Settings.useCover.Value)
            {
                string cover = $"{Assets.CACHED}\\{audioId}-cover.png";
                File.Copy($"{temp}\\cover.png", cover, true);

                Settings.cover.Value = cover;
                Settings.novaCover.Value = cover;
            }

            if (Settings.useVideo.Value)
            {
                string video = $"{Assets.CACHED}\\{audioId}-video.mp4";
                File.Copy($"{temp}\\video.mp4", video, true);

                Settings.video.Value = video;
            }

            using FileStream data = new($"{temp}\\objects.phxmo", FileMode.Open, FileAccess.Read);
            data.Seek(0, SeekOrigin.Begin);
            using BinaryReader reader = new(data);

            int types = reader.ReadInt32();
            string[] mapData = new string[types + 1];

            int noteCount = reader.ReadInt32();
            string[] notes = new string[noteCount];

            Parser.ResetEncode();

            for (int i = 0; i < noteCount; i++)
            {
                uint ms = reader.ReadUInt32();
                bool quantum = reader.ReadBoolean();

                float x;
                float y;

                if (quantum)
                {
                    x = 1 - reader.ReadSingle();
                    y = reader.ReadSingle() + 1;
                }
                else
                {
                    x = 2 - reader.ReadByte();
                    y = reader.ReadByte();
                }

                notes[i] = $"{Parser.EncodeTimestamp(ms)}|{x.ToString(Program.Culture)}|{y.ToString(Program.Culture)}";
            }

            mapData[0] = string.Join(',', notes);

            string ReadString()
            {
                ushort length = reader.ReadUInt16();
                byte[] str = reader.ReadBytes(length);

                return Encoding.UTF8.GetString(str);
            }

            object[] ParseObject(int id)
            {
                return id switch
                {
                    2 => new object[] // brightness
                    {
                        reader.ReadUInt32(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadSingle().ToString(Program.Culture)
                    },
                    3 => new object[] // contrast
                    {
                        reader.ReadUInt32(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadSingle().ToString(Program.Culture)
                    },
                    4 => new object[] // saturation
                    {
                        reader.ReadUInt32(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadSingle().ToString(Program.Culture)
                    },
                    5 => new object[] // blur
                    {
                        reader.ReadUInt32(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadSingle().ToString(Program.Culture)
                    },
                    6 => new object[] // fov
                    {
                        reader.ReadUInt32(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadSingle().ToString(Program.Culture)
                    },
                    7 => new object[] // tint
                    {
                        reader.ReadUInt32(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte()
                    },
                    8 => new object[] // position
                    {
                        reader.ReadUInt32(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadSingle().ToString(Program.Culture),
                        reader.ReadSingle().ToString(Program.Culture),
                        reader.ReadSingle().ToString(Program.Culture)
                    },
                    9 => new object[] // rotation
                    {
                        reader.ReadUInt32(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadSingle().ToString(Program.Culture)
                    },
                    10 => new object[] // ar factor
                    {
                        reader.ReadSingle().ToString(Program.Culture)
                    },
                    11 => new object[] // text
                    {
                        reader.ReadUInt32(),
                        ReadString(),
                        reader.ReadByte()
                    },
                    _ => Array.Empty<string>(),
                };
            }

            for (int i = 2; i < types + 1; i++)
            {
                Parser.ResetEncode();

                int count = reader.ReadInt32();
                string[] set = new string[count];

                for (int j = 0; j < count; j++)
                    set[j] = string.Join('|', Parser.EncodeTimestamp(reader.ReadUInt32()), ParseObject(i));

                mapData[i] = string.Join(',', set);
            }

            return string.Join('`', "ssmapv2", audioId.Replace('`', '_'), Exporting.Difficulties[Settings.difficulty.Value].ToString(), Settings.customDifficulty.Value, string.Join('`', mapData));
        }
    }
}
