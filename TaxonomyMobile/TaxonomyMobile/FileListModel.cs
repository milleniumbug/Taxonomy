using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Common;
using TaxonomyLib;

namespace TaxonomyMobile
{
	class FileListModel : INotifyPropertyChanged
	{
		private readonly ObservableBatchCollection<FileItem> files = new ObservableBatchCollection<FileItem>();
		public ICollection<FileItem> Files => files;

		private string path;
		private Taxonomy managedTaxonomy;

		public void ChangeDirectory(string path)
		{
			var dir = new DirectoryInfo(path);
			IEnumerable<FileSystemInfo> directories = dir.EnumerateDirectories();
			IEnumerable<FileSystemInfo> files = dir.EnumerateFiles();
			var entries = directories
				.Concat(files)
				.Select(fileInfo => new FileItem(fileInfo.FullName));
			this.files.Clear();
			this.files.AddRange(entries);
		}

		public string TaxonomyShortName => ManagedTaxonomy?.ShortName ?? "Files";

		public Taxonomy ManagedTaxonomy
		{
			get { return managedTaxonomy; }
			set
			{
				if(managedTaxonomy == value)
					return;
				managedTaxonomy = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public FileListModel()
		{
			//ChangeDirectory(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
			ChangeDirectory(AppCompat.GetExternalSdCardLocation());
		}
	} 

	internal class FileItem
	{
		public FileItem(string path)
		{
			Path = path;
		}

		public bool IsDirectory => Directory.Exists(Path);

		public string Path { get; }
	}
}
