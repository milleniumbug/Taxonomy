using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TaxonomyLib;

namespace TaxonomyWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public File File { get; }

		public ICollection<TaxonomyItem> Taxonomies { get; }

		public ICollection<NamespaceItem> Namespaces { get; }

		public ICollection<FileItem> Files { get; }

		public int IconWidth => IconDimensions.Item1;

		public int IconHeight => IconDimensions.Item2;

		public Tuple<int, int> IconDimensions { get; }

		public ICommand IconDoubleClick { get; }

		public MainWindow()
		{
			IconDimensions = Tuple.Create(32, 32);
			IconDoubleClick = new TrivialCommand<Button>(button => { });
			Taxonomies = new ObservableCollection<TaxonomyItem>();
			Taxonomies.Add(new TaxonomyItem(@"C:\Windows\whatever", "sample taxonomy"));
			Namespaces = new ObservableCollection<NamespaceItem>() {new NamespaceItem(new Namespace("kind"))};
			Files = new ObservableCollection<FileItem>()
			{
				new FileItem(null, @"C:\Windows"),
				new FileItem(null, @"C:\Windows\regedit.exe"),
				new FileItem(null, @"C:\Windows\notepad.exe")
			};
			foreach(var i in Enumerable.Repeat(new FileItem(null, @"C:\Windows\notepad.exe"), 1000))
				Files.Add(i);
			InitializeComponent();
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
			Taxonomies.Remove(taxonomyItem);
		}

		private void OnIconLeftClick(object sender, RoutedEventArgs e)
		{
			
		}
	}
}
