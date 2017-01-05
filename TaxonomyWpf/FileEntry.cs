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
using TaxonomyLib;

namespace TaxonomyWpf
{
	public class FileEntry
	{
		private readonly Lazy<ImageSource> icon;
		public ImageSource Icon => icon.Value;

		public string Name => System.IO.Path.GetFileName(Path);

		private Taxonomy taxonomy;

		private TaxonomyLib.File file;

		public TaxonomyLib.File File => file = MaterializeFile(file);

		private string Path { get; }

		private TaxonomyLib.File MaterializeFile(TaxonomyLib.File file)
		{
			if(file != null)
			{
				file = taxonomy.AddFile(Path);
				taxonomy = null;
			}
			return file;
		}

		public FileEntry(TaxonomyLib.File file, string path, Taxonomy taxonomy)
		{
			this.file = file;
			Path = path;
			icon = new Lazy<ImageSource>(() =>
			{
				using(var extractedIcon = NativeExplorerInterface.GetHIconForFile(Path))
				{
					return Imaging.CreateBitmapSourceFromHIcon(
						extractedIcon.IconHandle,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions());
				}
			});
		}
	}
}
