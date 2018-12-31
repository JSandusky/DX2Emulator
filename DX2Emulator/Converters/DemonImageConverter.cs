using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DX2Emulator.Converters
{
    public class DemonImageConverter : IValueConverter
    {
        static Dictionary<string, ImageSource> cache_ = new Dictionary<string, ImageSource>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string fileName = value.ToString();
            fileName = AppUtil.GetImageFilePath(fileName);
            if (cache_.ContainsKey(fileName))
                return cache_[fileName];

            try
            {
                BitmapImage img = new BitmapImage(new Uri(fileName, UriKind.Absolute));
                cache_[fileName] = img;
                return img;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
