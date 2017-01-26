using System.Collections.Generic;
using Xamarin.Forms;

namespace TaxonomyMobile
{
	public partial class MainPage : TabbedPage
	{
		public MainPage()
		{
			Children.Add(new TaxonomyPage());
			Children.Add(new FileListPage());
			InitializeComponent();
		}
	}
}
