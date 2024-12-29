using New_SSQE.Objects;
using TextCopy;

namespace New_SSQE.ExternalUtils
{
    internal class Clipboard
    {
        public static void SetText(string text)
        {
            try
            {
                Task result = Task.Run(async () =>
                {
                    await ClipboardService.SetTextAsync(text);
                });
            }
            catch (AggregateException ex) when (Platform.IsLinux)
            {
                Logging.Register("Failed to set text of clipboard", LogSeverity.WARN, ex);
                MessageBox.Show("Clipboard functions require 'xsel' to be installed and accessible\nhttps://github.com/kfish/xsel", MBoxIcon.Warning, MBoxButtons.OK);
            }
        }

        public static void SetData(List<MapObject> mapObjects)
        {
            /*
            List<string> data = new();

            foreach (MapObject obj in mapObjects)
            {
                if (obj is Note note)
                {
                    double x = Math.Round(note.X, 2);
                    double y = Math.Round(note.Y, 2);

                    data.Add($"{note.X.ToString(Program.Culture)}|{note.Y.ToString(Program.Culture)}|{note.Ms}");
                }
            }

            SetText(string.Join(',', data));
            return;
            */

            string[] objStr = new string[mapObjects.Count + 1];
            objStr[0] = "copy";

            for (int i = 0; i < mapObjects.Count; i++)
                objStr[i + 1] = $"{mapObjects[i].ID}|{mapObjects[i].ToString()}";

            SetText(string.Join(',', objStr));
        }

        public static string GetText()
        {
            try
            {
                Task<string?> result = Task.Run(async () =>
                {
                    return await ClipboardService.GetTextAsync();
                });

                return result.Result ?? "";
            }
            catch (AggregateException ex) when (Platform.IsLinux)
            {
                Logging.Register("Failed to get text of clipboard", LogSeverity.WARN, ex);
                MessageBox.Show("Clipboard functions require 'xsel' to be installed and accessible\nhttps://github.com/kfish/xsel", MBoxIcon.Warning, MBoxButtons.OK);
            }

            return "";
        }

        public static List<MapObject> GetData()
        {
            string text = GetText();
            string[] split = text.Split(',');

            List<MapObject> objects = new();

            if (split.FirstOrDefault() == "copy")
            {
                for (int i = 1; i < split.Length; i++)
                {
                    string item = split[i];
                    MapObject? obj = MOParser.Parse(null, item.Split('|'));

                    if (obj != null)
                        objects.Add(obj);
                }
            }
            else
            {
                for (int i = 0; i < split.Length; i++)
                {
                    string item = split[i];
                    string[] subsplit = item.Split('|');

                    try
                    {
                        float x = float.Parse(subsplit[0], Program.Culture);
                        float y = float.Parse(subsplit[1], Program.Culture);
                        long time = long.Parse(subsplit[2]);

                        objects.Add(new Note(x, y, time));
                    }
                    catch { }
                }
            }

            return objects;
        }
    }
}
