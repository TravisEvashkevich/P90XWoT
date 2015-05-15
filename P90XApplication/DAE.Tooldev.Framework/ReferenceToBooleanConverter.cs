using System;
using System.Globalization;
using System.Windows.Data;

namespace Dae.ToolDev.Framework
{
	/// <summary>
	/// Converts a reference to true (when non-null), or false (when null).
	/// </summary>
	public class ReferenceToBooleanConverter : IValueConverter
	{
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