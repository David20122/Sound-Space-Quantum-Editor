using New_SSQE.Objects.Other;

namespace New_SSQE.FileParsing.Formats
{
    internal class QER
    {
        public static List<ReplayNode> Parse(string path, out float tempo)
        {
            using FileStream file = new(path, FileMode.Open, FileAccess.Read);
            file.Seek(0, SeekOrigin.Begin);

            using BinaryReader reader = new(file);

            List<ReplayNode> nodes = new();

            tempo = reader.ReadSingle();
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                nodes.Add(new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadInt32(), ReplayType.Cursor));

            count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                nodes.Add(new(0, 0, reader.ReadInt32(), ReplayType.Skip));

            return nodes;
        }

        public static void Save(string path, float tempo, List<ReplayNode> nodes)
        {
            using FileStream file = new(path, FileMode.Create, FileAccess.Write);
            using BinaryWriter writer = new(file);

            int cursorCount = nodes.Count(n => n.Type == ReplayType.Cursor);
            int skipCount = nodes.Count(n => n.Type == ReplayType.Skip);

            writer.Write(tempo);
            writer.Write(cursorCount);

            for (int i = 0; i < cursorCount; i++)
            {
                ReplayNode node = nodes[i];

                writer.Write(node.X);
                writer.Write(node.Y);
                writer.Write(node.Ms);
            }

            writer.Write(skipCount);

            for (int i = cursorCount; i < nodes.Count; i++)
            {
                ReplayNode node = nodes[i];

                writer.Write(node.Ms);
            }
        }
    }
}
