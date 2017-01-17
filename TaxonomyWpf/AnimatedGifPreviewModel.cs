using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyWpf
{
	class AnimatedGifPreviewModel : FilePreviewModel
	{
		public Uri Source { get; }

		public override void Dispose()
		{
			
		}

		public AnimatedGifPreviewModel(string path)
		{
			Source = new Uri(path);
		}
	}
}
