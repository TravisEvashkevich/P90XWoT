using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Dae.ToolDev.Framework
{
    public class StringToBoolConverter:IValueConverter
    {
        /// <summary>
        /// Converts a reference to true (when non-null), or false (when null).
        /// </summary>

        /// <summary>
        /// Inverts the output.
        /// </summary>
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null) ^ IsInverted;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

