﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TaxonomyLib;

namespace TaxonomyWpf
{
	public class FileEntry : INotifyPropertyChanged
	{
		private ImageSource icon = null;

		public ImageSource Icon
		{
			get
			{
				if(icon == null)
					return icon = LoadIcon(Path);
				return icon;
			}
		}

		public string Name => System.IO.Path.GetFileName(Path);

		private readonly WeakReference<Taxonomy> taxonomy;

		private TaxonomyLib.File file;

		public bool IsDirectory { get; }

		public TaxonomyLib.File File
		{
			get
			{
				if(IsDirectory)
					return null;
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

		private static ImageSource LoadIcon(string path)
		{
			return LoadIcon(path, p => NativeExplorerInterface.GetHIconForFile(path));
		}

		private static ImageSource LoadIcon(string path, Func<string, NativeExplorerInterface.HIcon> load)
		{
			using (var extractedIcon = load(path))
			{
				var icon = Imaging.CreateBitmapSourceFromHIcon(
					extractedIcon.IconHandle,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
				icon.Freeze();
				return icon;
			}
		}

		public string Path { get; }

		public FileEntry(TaxonomyLib.File file, string path, Taxonomy taxonomy)
		{
			this.file = file;
			Path = path;
			IsDirectory = Directory.Exists(path);
			this.taxonomy = new WeakReference<Taxonomy>(taxonomy);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
