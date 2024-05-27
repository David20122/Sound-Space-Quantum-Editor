using System.Globalization;
using TextCopy;

namespace New_SSQE
{
    internal class Clipboard
    {
        public static void SetText(string text)
        {
            try
            {
                var result = Task.Run(async () =>
                {
                    await ClipboardService.SetTextAsync(text);
                });
            }
            catch (AggregateException ex) when (MainWindow.IsLinux)
            {
                ActionLogging.Register("Failed to set text of clipboard", "WARN", ex);
                MessageBox.Show("Clipboard functions require 'xsel' to be installed and accessible\nhttps://github.com/kfish/xsel", "Warning", "OK");
            }
        }

        public static void SetData(List<Note> notes)
        {
            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            var data = "";

            foreach (Note note in notes)
                data += note.ToString(culture);

            data = data[1..];

            SetText(data);
        }

        public static string GetText()
        {
            try
            {
                var result = Task.Run(async () =>
                {
                    return await ClipboardService.GetTextAsync();
                });

                return result.Result ?? "";
            }
            catch (AggregateException ex) when (MainWindow.IsLinux)
            {
                ActionLogging.Register("Failed to get text of clipboard", "WARN", ex);
                MessageBox.Show("Clipboard functions require 'xsel' to be installed and accessible\nhttps://github.com/kfish/xsel", "Warning", "OK");
            }

            return "";
        }

        public static List<Note> GetData()
        {
            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            var text = GetText();
            var split = text.Split(',');

            var notes = new List<Note>();

            foreach (var line in split)
            {
                var note = new Note(line, culture);

                if (note != null)
                    notes.Add(note);
            }

            return notes;
        }
    }
}
