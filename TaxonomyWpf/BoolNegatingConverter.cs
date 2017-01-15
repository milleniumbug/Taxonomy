using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TaxonomyWpf
{
	public class BoolNegatingConverter : IValueConverter
	{
		public static readonly BoolNegatingConverter Default = new BoolNegatingConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool) value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Convert(value, targetType, parameter, culture);
		}
	}
}
