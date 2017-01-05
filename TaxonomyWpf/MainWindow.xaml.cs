using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using Gu.Reactive;
using Microsoft.Win32;
using TaxonomyLib;
using File = TaxonomyLib.File;
using System.Runtime.CompilerServices;

namespace TaxonomyWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private TaxonomyItem currentTaxonomy;
		private string searchQuery;
		private FileEntry currentFile;

		public event PropertyChangedEventHandler PropertyChanged;

		public File File { get; }

		public string SearchQuery
		{
			get
			{
				return this.searchQuery;
			}

			set
			{
				if (value == this.searchQuery)
				{
					return;
				}

				this.searchQuery = value;
				this.OnPropertyChanged();
			}
		}

		public TaxonomyItem CurrentTaxonomy
		{
			get { return currentTaxonomy; }
			set
			{
				currentTaxonomy = value;
				var taxonomy = currentTaxonomy.Taxonomy.Value;
				Namespaces.Clear();
				foreach(var ns in taxonomy.AllNamespaces())
				{
					var namespaceItem = new NamespaceItem(ns);
					foreach(var tag in taxonomy.TagsInNamespace(ns))
						namespaceItem.Tags.Add(tag);
					Namespaces.Add(namespaceItem);
				}
				OnPropertyChanged();
			}
		}

		public FileEntry CurrentFile
		{
			get
			{
				return this.currentFile;
			}

			set
			{
				if (ReferenceEquals(value, this.currentFile))
				{
					return;
				}

				currentFile = value;
				this.OnPropertyChanged();
			}
		}

		public ICollection<TaxonomyItem> Taxonomies { get; }

		public ICollection<NamespaceItem> Namespaces { get; }

		public ICollection<FileEntry> Files { get; }

		public int IconWidth => IconDimensions.Item1;

		public int IconHeight => IconDimensions.Item2;

		public Tuple<int, int> IconDimensions { get; }

		public ICommand IconDoubleClick { get; }

		public ICommand TagDoubleClick { get; }

		public MainWindow()
		{
			IconDimensions = Tuple.Create(32, 32);
			IconDoubleClick = new TrivialCommand<FileEntry>(OnIconDoubleClick);
			TagDoubleClick = new TrivialCommand<Tag>(OnTagDoubleClick);
			Taxonomies = new ObservableSet<TaxonomyItem>();
			Namespaces = new ObservableCollection<NamespaceItem>();
			Files =
				new ObservableCollection<FileEntry>(
					Directory.EnumerateFileSystemEntries("C:\\Windows").Select(path => new FileEntry(null, path)));
			InitializeComponent();
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			foreach(var taxonomyItem in Taxonomies)
			{
				if(taxonomyItem.Taxonomy.IsValueCreated)
					taxonomyItem.Taxonomy.Value.Dispose();
			}
			//Taxonomy.Dispose();
		}

		private void OnRemoveTagClick(object sender, RoutedEventArgs e)
		{
			var taxonomyItem = (TaxonomyItem)(((Button) sender).Tag);
			var taxonomy = taxonomyItem.Taxonomy.IsValueCreated ? taxonomyItem.Taxonomy.Value : null;
			Taxonomies.Remove(taxonomyItem);
			taxonomy?.Dispose();
		}

		private void OnIconDoubleClick(FileEntry b)
		{
			
		}

		private void OnTagDoubleClick(Tag t)
		{
			SearchQuery += t + " ";
		}

		private void OnIconLeftClick(object sender, RoutedEventArgs e)
		{
			
		}

		private void NewTaxonomyClick(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog();

			if(dialog.ShowDialog() == true)
			{
				var taxonomyItem = new TaxonomyItem(dialog.FileName, "unnamed collection");
				Taxonomies.Add(taxonomyItem);
			}
		}

		private void OpenTaxonomyClick(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			if(dialog.ShowDialog() == true)
			{
				var taxonomyItem = new TaxonomyItem(dialog.FileName, "unnamed collection");
				Taxonomies.Add(taxonomyItem);
			}
		}
	}
}
