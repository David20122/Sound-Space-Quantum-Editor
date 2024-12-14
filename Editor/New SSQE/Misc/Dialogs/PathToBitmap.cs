using Avalonia.Data.Converters;
using System.Globalization;
using Avalonia.Media.Imaging;

namespace New_SSQE.Misc.Dialogs
{
    public class PathToBitmap : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;

            return new Bitmap(path);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
