using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace TaxonomyWpf
{
	/// <summary>
	/// Interaction logic for AddTagDialog.xaml
	/// </summary>
	public partial class AddTagDialog : Window
	{
		public AddTagDialog()
		{
			InitializeComponent();
		}

		public string TagText { get; set; }

		private void Cancel(object sender, RoutedEventArgs e)
		{
			TagText = null;
			Close();
		}

		private void Confirm(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			if(TagText == null)
				TagText = "";
			Close();
		}
	}
}
