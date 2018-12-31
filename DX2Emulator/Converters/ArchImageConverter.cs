using DX2Data;
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
    public class ArchImageConverter : IValueConverter
    {
        public static ImageSource clear;
        public static ImageSource red;
        public static ImageSource yellow;
        public static ImageSource purple;
        public static ImageSource teal;

        static ArchImageConverter()
        {
            clear = new BitmapImage(new Uri("pack://application:,,,/Img/clear.png"));
            red = new BitmapImage(new Uri("pack://application:,,,/Img/red.png"));
            yellow = new BitmapImage(new Uri("pack://application:,,,/Img/yellow.png"));
            purple = new BitmapImage(new Uri("pack://application:,,,/Img/purple.png"));
            teal = new BitmapImage(new Uri("pack://application:,,,/Img/teal.png"));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object val = value;
            if (value.GetType() == typeof(string))
                val = Enum.Parse(typeof(Archetype), value.ToString());

            switch ((Archetype)val)
            {
                case Archetype.Clear:
                    return clear;
                case Archetype.Red:
                    return red;
                case Archetype.Yellow:
                    return yellow;
                case Archetype.Purple:
                    return purple;
                case Archetype.Teal:
                    return teal;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
