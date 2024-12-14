using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Globalization;

namespace New_SSQE.GUI.Input
{
    // opentk is a stupid
    internal class KeyConverter
    {
        private static readonly Dictionary<char, char> KeyLookup = new()
        {
            {'1', '!' },
            {'2', '@' },
            {'3', '#' },
            {'4', '$' },
            {'5', '%' },
            {'6', '^' },
            {'7', '&' },
            {'8', '*' },
            {'9', '(' },
            {'0', ')' },
            {'-', '_' },
            {'=', '+' },
            {'`', '~' },
            {'[', '{' },
            {']', '}' },
            {'\\', '|' },
            {';', ':' },
            {'\'', '"' },
            {',', '<' },
            {'.', '>' },
            {'/', '?' },
        };

        private static readonly Dictionary<string, char> NumpadLookup = new()
        {
            {"0", '0' },
            {"1", '1' },
            {"2", '2' },
            {"3", '3' },
            {"4", '4' },
            {"5", '5' },
            {"6", '6' },
            {"7", '7' },
            {"8", '8' },
            {"9", '9' },
            {"Multiply", '*' },
            {"Divide", '/' },
            {"Add", '+' },
            {"Subtract", '-' },
            {"Equals", '=' },
            {"Decimal", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0] },
        };

        public static char GetCharFromInput(Keys key, bool shift)
        {
            char c = (char)key;

            if (!shift)
            {
                if (key.ToString().Contains("KeyPad"))
                    return NumpadLookup[key.ToString().Replace("KeyPad", "")];
                else
                    return c.ToString().ToLower()[0];
            }
            else if (KeyLookup.TryGetValue(c, out char value))
                return value;
            else
                return c;
        }
    }
}
