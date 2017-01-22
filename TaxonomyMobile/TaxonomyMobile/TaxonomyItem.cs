using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TaxonomyLib;
using Xamarin.Forms;

namespace TaxonomyMobile
{
    public class TaxonomyItem
    {
	    private readonly Lazy<Taxonomy> taxonomy;
	    public Taxonomy Taxonomy => taxonomy.Value;

	    public string Path { get; }

	    public TaxonomyItem(string path)
	    {
		    Path = path;
			taxonomy = new Lazy<Taxonomy>(() => new Taxonomy(Path));
	    }
    }

	public class TaxonomyItemToStringConverter : IValueConverter
	{
		public static readonly TaxonomyItemToStringConverter Default = new TaxonomyItemToStringConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var taxonomyItem = (TaxonomyItem)value;
			return taxonomyItem.Path;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
