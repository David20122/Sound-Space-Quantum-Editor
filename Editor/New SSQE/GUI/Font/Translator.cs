using New_SSQE.Preferences;

namespace New_SSQE.GUI.Font
{
    internal class Translator
    {
        public static string Translate(string text)
        {
            if (Settings.language.Value == "english")
                return text;

            return "";
        }
    }
}
