using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gu.Wpf.Media;

namespace TaxonomyWpf
{
	/// <summary>
	/// Interaction logic for FilePreview.xaml
	/// </summary>
	public partial class FilePreview : UserControl, INotifyPropertyChanged
	{
		public FilePreview()
		{
			Model = LoadModel(null);
			InitializeComponent();
			Dispatcher.ShutdownStarted += (sender, args) =>
			{
				Model.Dispose();
			};
		}

		public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(
			nameof(FilePath),
			typeof(string),
			typeof(FilePreview),
			new FrameworkPropertyMetadata(null, OnFilePathChanged) { BindsTwoWayByDefault = true });

		private FilePreviewModel LoadModel(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				return new NullPreviewModel();
			switch (System.IO.Path.GetExtension(path).ToLowerInvariant())
			{
				case ".txt":
					return new TextFilePreviewModel(path);
				case ".png":
				case ".jpg":
				case ".jpeg":
					return new ImagePreviewModel(path);
				case ".gif":
					return new AnimatedGifPreviewModel(path);
				case ".mp4":
					return new VideoPreviewModel(path);
				case ".zip":
					return new ArchivePreviewModel(path);
				default:
					return new BinaryFilePreviewModel(path);
			}
		}

		private static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
		{
			var self = (FilePreview)d;
			var newValue = (string)eventArgs.NewValue;
			self.Model?.Dispose();
			self.Model = self.LoadModel(newValue);
		}

		public string FilePath
		{
			get { return (string)GetValue(FilePathProperty); }
			set { SetValue(FilePathProperty, value); }
		}

		private FilePreviewModel model;
		public FilePreviewModel Model
		{
			get { return model; }
			set
			{
				if (model == value)
					return;
				model = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Repeat(object sender, RoutedEventArgs e)
		{
			var mediaElementWrapper = (MediaElementWrapper)sender;
			mediaElementWrapper.Play();
		}
	}
}
