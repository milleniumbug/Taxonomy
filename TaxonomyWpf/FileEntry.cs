using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

		private readonly WeakReference<Taxonomy> taxonomy;

		private TaxonomyLib.File file;

		public TaxonomyLib.File File
		{
			get
			{
				if(file == null)
				{
					Taxonomy t;
					if(taxonomy.TryGetTarget(out t))
					{
						file = t.GetFile(Path);
						taxonomy.SetTarget(null);
					}
					else
					{
						Debug.Assert(false);
					}
				}
				return file;
			}
		}

		private string Path { get; }

		public FileEntry(TaxonomyLib.File file, string path, Taxonomy taxonomy)
		{
			this.file = file;
			Path = path;
			this.taxonomy = new WeakReference<Taxonomy>(taxonomy);
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
