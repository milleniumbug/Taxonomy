using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyLib
{
	public class Tag
	{
		internal long Id { get; set; }
		public Namespace Namespace { get; }
		public TagName Name { get; }

		internal Tag(long id, Namespace ns, TagName name)
		{
			Id = id;
			Namespace = ns;
			Name = name;
		}
	}
}
