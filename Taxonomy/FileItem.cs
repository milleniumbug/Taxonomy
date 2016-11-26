using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyWpf
{
	public class FileItem
	{
		public TaxonomyLib.File File { get; }

		public FileItem(TaxonomyLib.File file)
		{
			File = file;
		}
	}
}
