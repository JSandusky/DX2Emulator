using DX2Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DX2Emulator.Converters
{
    public class ResistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Resistance)value)
            {
                case Resistance.None:
                    return "-";
                case Resistance.Weak:
                    return "Wk";
                case Resistance.Drain:
                    return "Dr";
                case Resistance.Repel:
                    return "Rp";
                case Resistance.Resist:
                    return "Rs";
            }
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
