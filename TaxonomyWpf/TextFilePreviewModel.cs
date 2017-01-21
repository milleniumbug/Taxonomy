using System.IO;
using System.Text;

namespace TaxonomyWpf
{
	class TextFilePreviewModel : FilePreviewModel
	{
		public TextFilePreviewModel(string path) :
			this(path, Encoding.UTF8)
		{
			
		}

		public TextFilePreviewModel(string path, Encoding encoding)
		{
			Text = File.ReadAllText(path, encoding);
		}

		public string Text { get; }
		public override void Dispose()
		{
			
		}
	}
}