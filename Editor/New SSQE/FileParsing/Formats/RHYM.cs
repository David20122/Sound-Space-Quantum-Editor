using New_SSQE.Maps;
using New_SSQE.Objects;
using System.Text.Json;

namespace New_SSQE.FileParsing.Formats
{
    internal class RHYM
    {
        public static string Parse(string path)
        {
            using FileStream file = new(path, FileMode.Open, FileAccess.Read);
            List<float[]> objects = JsonSerializer.Deserialize<List<float[]>>(file) ?? new();

            int numFields = 3;
            float[] data = [];
            long curMs = 0;

            float[] objFields = [0, 0, 0];

            int iter = Math.Min(numFields, objFields.Length);

            for (int i = 0; i < data.Length; i += numFields)
            {
                for (int j = 0; j < iter; j++)
                    objFields[j] = data[i + j];

                long time = (long)objFields[0];
                float x = objFields[1];
                float y = objFields[2];
                curMs += time;

                Note note = new(x, y, curMs);
            }

            return "id_rhym";
        }

        public static void Save(string path)
        {
            using FileStream file = new(path, FileMode.Create, FileAccess.Write);

            List<Note> notes = CurrentMap.Notes;
            float[] data = new float[notes.Count * 3];

            List<float[]> objects = new();
            long curMs = 0;

            for (int i = 0; i < notes.Count; i++)
            {
                Note note = notes[i];
                data[i * 3 + 0] = note.Ms - curMs;
                data[i * 3 + 1] = (float)Math.Round(note.X, 2);
                data[i * 3 + 2] = (float)Math.Round(note.Y, 2);
                curMs = note.Ms;
            }

            objects.Add(data);
            JsonSerializer.Serialize(file, objects);
        }
    }
}
