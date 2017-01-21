using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace TaxonomyWpf
{
	public class EncodingToNameConverter : IValueConverter
	{
		public static readonly EncodingToNameConverter Default = new EncodingToNameConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var encoding = value as Encoding;
			return encoding?.WebName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}