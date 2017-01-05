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

		private Lazy<ICollection<Tag>> tags = new Lazy<ICollection<Tag>>(() =>
		{
			return new ObservableCollection<Tag>();
		});

		public ICollection<Tag> Tags => tags.Value;

		public NamespaceItem(Namespace ns)
		{
			Namespace = ns;
		}
	}
}
