using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace TaxonomyMobile
{
	public partial class FileListPage : ContentPage
	{
		public FileListPage()
		{
			InitializeComponent();
		}

		private async void FileTapped(object sender, ItemTappedEventArgs e)
		{
			await Navigation.PushModalAsync(new FilePreview());
		}
	}
}
