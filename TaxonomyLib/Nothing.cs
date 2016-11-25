using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyLib
{
	public sealed class Nothing
	{
		private Nothing()
		{

		}

		public static Nothing AtAll { get { return null; } }
	}
}
