using System;
using System.Globalization;
using Xamarin.Forms;

namespace Samples.Converters
{
	public class NotValueConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			return object.Equals (value, false);
		}

		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			return object.Equals (value, false);
		}
	}
}
