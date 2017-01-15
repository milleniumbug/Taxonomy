using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TaxonomyWpf
{
	class ImagePreviewModel : FilePreviewModel
	{
		private ImageSource source;

		public ImageSource Source
		{
			get { return source; }
			set
			{
				if(ReferenceEquals(source, value))
					return;
				source = value;
				OnPropertyChanged();
			}
		}

		public ImagePreviewModel(string path)
		{
			var buffer = File.ReadAllBytes(path);
			var memoryStream = new MemoryStream(buffer);

			var image = new BitmapImage();
			image.BeginInit();
			image.StreamSource = memoryStream;
			image.EndInit();
			image.Freeze();
			Source = image;
		}

		public override void Dispose()
		{
			
		}
	}
}