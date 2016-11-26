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
		public IList<string> Components { get; }

		public BreadCrumbs()
		{
			Components = new ObservableCollection<string> {"aaa", "bbb", "ccc"};
			InitializeComponent();
		}
	}
}
