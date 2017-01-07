using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TaxonomyLib;
using System.Runtime.CompilerServices;

namespace TaxonomyWpf
{
	public class TaxonomyItem : IEquatable<TaxonomyItem>, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public bool Equals(TaxonomyItem other)
		{
			if(ReferenceEquals(null, other)) return false;
			if(ReferenceEquals(this, other)) return true;
			return string.Equals(Path, other.Path);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != this.GetType()) return false;
			return Equals((TaxonomyItem) obj);
		}

		public override int GetHashCode()
		{
			return (Path != null ? Path.GetHashCode() : 0);
		}

		public static bool operator ==(TaxonomyItem left, TaxonomyItem right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TaxonomyItem left, TaxonomyItem right)
		{
			return !Equals(left, right);
		}

		public string Path { get; }

		public string ShortPath
		{
			get
			{
				var splitPath = Path.Split(System.IO.Path.DirectorySeparatorChar);
				return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), splitPath.First(), "...", splitPath.Last());
			}
		}

		public string ShortName => Taxonomy.IsValueCreated ? Taxonomy.Value.ShortName : "...";

		public Lazy<TaxonomyLib.Taxonomy> Taxonomy { get; }

		public TaxonomyItem(string path, Func<string, Taxonomy> taxonomyFactory)
		{
			Path = path;
			Taxonomy = new Lazy<TaxonomyLib.Taxonomy>(() => taxonomyFactory(path));
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
