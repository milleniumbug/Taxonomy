﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common;
using TaxonomyLib;
using File = TaxonomyLib.File;

namespace TaxonomyWpf
{
	public sealed class MainWindowModel : IDisposable, INotifyPropertyChanged
	{
		private TaxonomyItem currentTaxonomy;
		private string searchQuery;
		private string currentDirectory;
		private FileEntry currentFile;
		private bool disposed;
		private readonly ObservableSet<TaxonomyItem> taxonomies;
		private readonly ObservableBatchCollection<NamespaceItem> namespaces;
		private readonly ObservableBatchCollection<FileEntry> files;

		public event PropertyChangedEventHandler PropertyChanged;

		public ICollection<TaxonomyItem> Taxonomies => taxonomies;

		public ICollection<NamespaceItem> Namespaces => namespaces;

		public ICollection<FileEntry> Files => files;

		public string CurrentDirectory
		{
			get
			{
				return currentDirectory;
			}

			set
			{
				if(value == currentDirectory)
				{
					return;
				}

				currentDirectory = value;
				UpdateDirectoryListing(value);
				OnPropertyChanged();
			}
		}

		public TaxonomyItem CurrentTaxonomy
		{
			get { return currentTaxonomy; }
			set
			{
				if(ReferenceEquals(value, currentTaxonomy))
				{
					return;
				}

				currentTaxonomy = value;
				ChangeActiveTaxonomy(value);
				OnPropertyChanged();
			}
		}

		public FileEntry CurrentFile
		{
			get
			{
				return currentFile;
			}

			set
			{
				if (ReferenceEquals(value, currentFile))
				{
					return;
				}

				currentFile = value;
				OnPropertyChanged();
			}
		}

		public string SearchQuery
		{
			get
			{
				return searchQuery;
			}

			set
			{
				if (value == searchQuery)
				{
					return;
				}

				searchQuery = value;
				OnPropertyChanged();
			}
		}

		public void IssueDefaultAction(FileEntry file)
		{
			var attributes = System.IO.File.GetAttributes(file.Path);
			if((attributes & FileAttributes.Directory) != 0)
			{
				CurrentDirectory = file.Path;
			}
			else
			{
				using(var process = Process.Start(file.Path))
				{
					// do nothing
				}
			}
		}

		// text: tag in the format 'namespace:tag'
		public void AddTag(string text)
		{
			var split = text.Trim().Split(':');
			var ns = new Namespace(split[0]);
			var tagName = new TagName(split[1]);
			var taxonomy = CurrentTaxonomy.Taxonomy.Value;
			var tag = taxonomy.AddTag(ns, tagName);
			NamespaceItem namespaceItem = namespaces.FirstOrDefault(ni => ni.Namespace == ns);
			if(namespaceItem == null)
			{
				namespaceItem = new NamespaceItem(ns);
				namespaces.Add(namespaceItem);
			}
			namespaceItem.Tags.Add(tag);
		}

		public void RemoveTag(Tag tag)
		{
			
		}

		public void AddTagToFile(FileEntry file, Tag tag)
		{
			file.File?.Tags.Add(tag);
		}

		public void CloseTaxonomy(TaxonomyItem taxonomyItem)
		{
			if(taxonomyItem.Taxonomy.IsValueCreated)
			{
				var taxonomy = taxonomyItem.Taxonomy.Value;
				Taxonomies.Remove(taxonomyItem);
				taxonomy.Dispose();
			}
		}

		public void Dispose()
		{
			if(disposed)
			{
				return;
			}
			foreach(var taxonomyItem in Taxonomies)
			{
				if(taxonomyItem.Taxonomy.IsValueCreated)
					taxonomyItem.Taxonomy.Value.Dispose();
			}
			this.disposed = true;
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void ChangeActiveTaxonomy(TaxonomyItem newTaxonomyItem)
		{
			// handle the case where no taxonomy is selected
			// this can happen when all taxonomies are closed
			if(newTaxonomyItem == null)
			{
				Namespaces.Clear();
				CurrentDirectory = "";
				return;
			}

			var taxonomy = newTaxonomyItem.Taxonomy.Value;
			Namespaces.Clear();
			foreach(var ns in taxonomy.AllNamespaces())
			{
				var namespaceItem = new NamespaceItem(ns);
				foreach(var tag in taxonomy.TagsInNamespace(ns))
					namespaceItem.Tags.Add(tag);
				Namespaces.Add(namespaceItem);
			}
			CurrentDirectory = taxonomy.RootPath;
		}

		private void UpdateDirectoryListing(string directoryPath)
		{
			files.Clear();
			if(string.IsNullOrEmpty(directoryPath))
				return;
			var directory = new DirectoryInfo(directoryPath);
			files.AddRange(directory.EnumerateDirectories()
				.Select(dir => new FileEntry(null, dir.FullName, CurrentTaxonomy.Taxonomy.Value)));
			files.AddRange(directory.EnumerateFiles()
				.Select(file => new FileEntry(null, file.FullName, CurrentTaxonomy.Taxonomy.Value)));
		}

		public void AddTagToSearchQuery(Tag tag)
		{
			SearchQuery += tag + " ";
		}

		public void CreateNewTaxonomy(string path, string shortName)
		{
			var taxonomyItem = new TaxonomyItem(path, p => Taxonomy.CreateNew(p, shortName));
			Taxonomies.Add(taxonomyItem);
		}

		public void OpenTaxonomy(string path)
		{
			var taxonomyItem = new TaxonomyItem(path, p => new Taxonomy(p));
			Taxonomies.Add(taxonomyItem);
		}

		private void ThrowIfDisposed()
		{
			if(disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		public MainWindowModel()
		{
			taxonomies = new ObservableSet<TaxonomyItem>();
			namespaces = new ObservableBatchCollection<NamespaceItem>();
			files = new ObservableBatchCollection<FileEntry>();
		}
	}
}
