using Avalonia.Input.Platform;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia;

namespace New_SSQE
{
    internal class Clipboard
    {
        private static IClipboard clipboard;

        private static void AssignClipboard()
        {
            clipboard = Application.Current.Clipboard;
        }

        public static void SetText(string text)
        {
            AssignClipboard();

            var result = Task.Run(async () =>
            {
                await clipboard.SetTextAsync(text);
            });
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
            AssignClipboard();

            var result = Task.Run(async () =>
            {
                return await clipboard.GetTextAsync();
            });

            return result.Result ?? "";
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
                var note = Note.New(line, culture);

                if (note != null)
                    notes.Add(note);
            }

            return notes;
        }
    }
}
