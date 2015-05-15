using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Dae.ToolDev.Framework
{
	/// <summary>
	/// Converts a color to a brush.
	/// </summary>
	public class ColorToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var color = (Color)value;
			var brush = new SolidColorBrush(color);
			return brush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var brush = (SolidColorBrush)value;
			var color = brush.Color;
			return color;
		}
	}
}