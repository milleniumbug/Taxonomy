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

namespace Taxonomy
{
	/// <summary>
	/// Interaction logic for BreadCrumbs.xaml
	/// </summary>
	public partial class BreadCrumbs : UserControl
	{
		private ObservableCollection<string> Components { get; }

		public string Path
		{
			get { return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), Components); }

			set
			{
				Components.Clear();
				Components.Add("This Computer");
				foreach(var component in value.Split(System.IO.Path.DirectorySeparatorChar))
				{
					Components.Add(component);
				}
			}
		}

		public BreadCrumbs()
		{
			Components = new ObservableCollection<string>();
			InitializeComponent();
			ItemsControl.ItemsSource = Components;
			Path = @"E:\PROJEKTY\Taxonomy\TaxonomyLib";
		}

		public BreadCrumbs(string path)
		{
			Path = path;
		}

		private void OnComponentClick(object sender, RoutedEventArgs e)
		{
			var tag = ((Button) sender).Tag;
			var list = (IList<string>) Components;
			var index = list.IndexOf((string)tag);
			for(int i = list.Count - 1; i >= index + 1; i--)
			{
				list.RemoveAt(i);
			}
		}
	}
}
