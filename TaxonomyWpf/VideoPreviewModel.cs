using System;
using System.Windows.Controls;

namespace TaxonomyWpf
{
	class VideoPreviewModel : FilePreviewModel
	{
		private bool soundEnabled = false;
		private Uri source;

		public Uri Source
		{
			get { return source; }
			set
			{
				if(source == value)
					return;
				source = value;
				OnPropertyChanged();
			}
		}

		public bool SoundEnabled
		{
			get { return soundEnabled; }
			set
			{
				if(soundEnabled == value)
					return;
				soundEnabled = value;
				OnPropertyChanged();
			}
		}

		public VideoPreviewModel(string path)
		{
			Source = new Uri(path);
		}

		public override void Dispose()
		{
			
		}
	}
}