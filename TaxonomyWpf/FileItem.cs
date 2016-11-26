using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TaxonomyWpf
{
	public class FileItem
	{
		private readonly Lazy<ImageSource> icon;
		public ImageSource Icon => icon.Value;

		public string Name => System.IO.Path.GetFileName(Path);

		public TaxonomyLib.File File { get; }

		private string Path { get; }

		public FileItem(TaxonomyLib.File file, string path)
		{
			File = file;
			Path = path;
			icon = new Lazy<ImageSource>(() =>
			{
				var extractedIcon = System.Drawing.Icon.ExtractAssociatedIcon(Path);
				return Imaging.CreateBitmapSourceFromHIcon(
					extractedIcon.Handle,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
			});
		}
	}
}
