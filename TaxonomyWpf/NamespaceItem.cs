using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxonomyLib;

namespace TaxonomyWpf
{
	public class NamespaceItem
	{
		public Namespace Namespace { get; }

		private Lazy<ICollection<TagName>> tags = new Lazy<ICollection<TagName>>(() =>
		{
			return new ObservableCollection<TagName>();
		});

		public ICollection<TagName> Tags => tags.Value;

		public NamespaceItem(Namespace ns)
		{
			Namespace = ns;
		}
	}
}
