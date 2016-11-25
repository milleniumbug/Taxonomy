using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyLib
{
	public class Tag
	{
		public Namespace Namespace { get; }
		public TagName Name { get; }

		internal Tag(Namespace ns, TagName name)
		{
			Namespace = ns;
			Name = name;
		}
	}
}
