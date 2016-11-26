using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyWpf
{
	class TaxonomyItem
	{
		public string Path { get; }

		public string ShortName { get; private set; }

		public Lazy<TaxonomyLib.Taxonomy> Taxonomy { get; }

		public TaxonomyItem(string path, string shortName)
		{
			
		}
	}
}
