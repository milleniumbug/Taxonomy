using System.IO;
using System.Text;

namespace TaxonomyWpf
{
	class TextFilePreviewModel : FilePreviewModel
	{
		public TextFilePreviewModel(string path)
		{
			Text = File.ReadAllText(path, Encoding.UTF8);
		}

		public string Text { get; }
		public override void Dispose()
		{
			
		}
	}
}