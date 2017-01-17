using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;

namespace TaxonomyWpf
{
	class ArchivePreviewModel : FilePreviewModel
	{
		public ArchivePreviewModel(string path)
		{
			using(var archive = ZipFile.OpenRead(path))
			{
				Entries = archive.Entries.Select(entry => entry.FullName).ToList();
			}
		}

		public IReadOnlyCollection<string> Entries { get; }

		public override void Dispose()
		{
			
		}
	}
}