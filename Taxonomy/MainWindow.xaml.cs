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

		public ICollection<Tuple<string, Lazy<TaxonomyLib.Taxonomy>>> Taxonomies { get; }

		public MainWindow()
		{
			Taxonomies = new ObservableCollection<Tuple<string, Lazy<TaxonomyLib.Taxonomy>>>();
			Taxonomies.Add(Tuple.Create(
				"sample taxonomy",
				new Lazy<TaxonomyLib.Taxonomy>(
					() => new TaxonomyLib.Taxonomy(@"F:\mietczynski_masochista\test.sql"))));
			InitializeComponent();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			foreach(var nameTaxonomyPair in Taxonomies)
			{
				if(nameTaxonomyPair.Item2.IsValueCreated)
					nameTaxonomyPair.Item2.Value.Dispose();
			}
			//Taxonomy.Dispose();
		}
	}
}
