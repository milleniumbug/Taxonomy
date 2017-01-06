using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	// a dumb workaround until Johan adds ObservableBatchCollection
	// to the Gu.Reactive's public interface
	public class ObservableBatchCollection<TItem> : ObservableCollection<TItem>
	{
		public void AddRange(IEnumerable<TItem> elements)
		{
			foreach(var element in elements)
			{
				Add(element);
			}
		}
	}
}
