using New_SSQE.Objects.Other;
using System.Text;

namespace New_SSQE.FileParsing.Formats
{
    internal class SSPRE
    {
        private static readonly Dictionary<string, float> speeds = new()
        {
            {"s:---", 1 / 1.35f },
            {"s:--", 1 / 1.25f },
            {"s:-", 1 / 1.15f },
            {"s:=", 1 },
            {"s:+", 1.15f },
            {"s:++", 1.25f },
            {"s:+++", 1.35f },
            {"s:++++", 1.45f }
        };

        public static List<ReplayNode> Parse(string path, out float tempo)
        {
            using FileStream file = new(path, FileMode.Open, FileAccess.Read);
            file.Seek(0, SeekOrigin.Begin);

            using BinaryReader reader = new(file);
            string fileTypeSignature = Encoding.ASCII.GetString(reader.ReadBytes(4));

            List<ReplayNode> nodes = new();
            tempo = 0;

            // making a return from sspm v1 woo
            string GetNextVariableString()
            {
                List<byte> bytes = new();
                byte cur = reader.ReadByte();

                while (cur != 0x0a)
                {
                    bytes.Add(cur);
                    cur = reader.ReadByte();
                }

                return Encoding.UTF8.GetString(bytes.ToArray());
            }

            if (fileTypeSignature != "Ss*R")
                return nodes;

            int sv = reader.ReadInt16(); // is "sv" the replay file version?
            reader.ReadBytes(8); // reserved
            if (sv >= 4)
                GetNextVariableString(); // hwid
            GetNextVariableString(); // song id
            string states = GetNextVariableString(); // song states

            foreach (string state in states.Split(';'))
            {
                if (state.StartsWith("s:c"))
                    tempo = float.Parse(state[3..]);
                else
                    speeds.TryGetValue(state, out tempo);
            }

            // visual settings
            if (sv >= 3)
            {
                reader.ReadSingle(); // approach rate
                reader.ReadSingle(); // spawn distance
                reader.ReadSingle(); // fade length
                reader.ReadSingle(); // parallax
                reader.ReadSingle(); // ui parallax
                reader.ReadSingle(); // so many parallaxes (grid parallax)
                reader.ReadSingle(); // fov
                reader.ReadByte(); // cam unlock
                reader.ReadSingle(); // edge drift
            }

            reader.ReadBytes(1); // more reserved
            reader.ReadInt32(); // end ms
            int count = reader.ReadInt32(); // signal count

            for (int i = 0; i < count; i++)
            {
                byte type = reader.ReadByte();
                if (type == 0x05) // RS_END
                    break;

                int ms;

                switch (type)
                {
                    case 0x00: // RS_CURSOR
                        ms = reader.ReadInt32();
                        float x = 1 - reader.ReadSingle();
                        float y = 1 - reader.ReadSingle();

                        nodes.Add(new(x, y, ms, ReplayType.Cursor));
                        break;
                    // currently unused
                    case 0x01: // RS_HIT
                    case 0x02: // RS_MISS
                    case 0x03: // RS_PAUSE
                    case 0x04: // RS_GIVEUP
                    case 0x06: // RS_START_UNPAUSE
                    case 0x07: // RS_CANCEL_UNPAUSE
                    case 0x08: // RS_FINISH_UNPAUSE
                        if (sv >= 2)
                            reader.ReadInt32(); // ms/note id, always 4 bytes
                        continue;
                    case 0x09 when sv >= 2: // RS_SKIP
                        ms = reader.ReadInt32();

                        nodes.Add(new(0, 0, ms, ReplayType.Skip));
                        break;
                    // this doesnt even have an entry in the source code what is this
                    case 0x0a: // RS_GIVEUP_CANCEL
                        continue;
                }
            }

            // this has no documentation! yippee

            return nodes.OrderBy(n => n.Ms).OrderBy(n => n.Type).ToList();
        }
    }
}
