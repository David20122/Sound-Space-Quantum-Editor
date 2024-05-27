namespace New_SSQE.Font
{
    internal class Translator
    {
        public static string Translate(string text)
        {
            if (Settings.settings["language"] == "english")
                return text;

            return "";
        }
    }
}
