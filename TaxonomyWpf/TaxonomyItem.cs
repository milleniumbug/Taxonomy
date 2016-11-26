using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TaxonomyLib;

namespace TaxonomyWpf
{
	public class TaxonomyItem
	{
		public string Path { get; }

		public string ShortPath
		{
			get
			{
				var splitPath = Path.Split(System.IO.Path.DirectorySeparatorChar);
				return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), splitPath.First(), "...", splitPath.Last());
			}
		}

		public string ShortName { get; private set; }

		public Lazy<TaxonomyLib.Taxonomy> Taxonomy { get; }

		public TaxonomyItem(string path, string shortName)
		{
			Path = path;
			ShortName = shortName;
			Taxonomy = new Lazy<TaxonomyLib.Taxonomy>(() => new TaxonomyLib.Taxonomy(path));
		}
	}

	public class TaxonomyItemToStringConverter : IValueConverter
	{
		public static readonly TaxonomyItemToStringConverter Default = new TaxonomyItemToStringConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var taxonomyItem = (TaxonomyItem) value;
			return $"{taxonomyItem.ShortName} ({taxonomyItem.ShortPath})";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
