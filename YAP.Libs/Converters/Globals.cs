using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAP.Libs.Converters
{
    public class SVGToPNGConverter : IValueConverter
    {
        public static string ConvertSVGToPNG(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                if (val.ToLower().Contains(".svg"))
                {
                    return val.Replace(".svg", ".png");
                }
            }
            return val;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }

            if (value is string)
            {
                var val = (string)value;
                return ConvertSVGToPNG(val);

            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
