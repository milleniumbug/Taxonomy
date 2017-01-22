using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TaxonomyMobile
{
    public class MainPageModel
    {
		public ObservableCollection<TaxonomyItem> Taxonomies { get; }

	    public MainPageModel()
	    {
		    Taxonomies = new ObservableCollection<TaxonomyItem>() {new TaxonomyItem("asdf"), new TaxonomyItem("lol")};

	    }
	}
}
