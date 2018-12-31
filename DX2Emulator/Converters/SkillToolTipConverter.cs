using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DX2Emulator.Converters
{
    public class SkillToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            string v = value.ToString();
            if (v == "--- none ---")
                return "";
            string r = "";
            var found = AppData.GetInst().Skills.FirstOrDefault(s => s.Name == v);
            if (found != null)
                r = found.DisplayString;
            return r;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
