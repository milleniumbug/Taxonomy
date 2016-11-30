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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaxonomyWpf
{
	/// <summary>
	/// Interaction logic for TagDisplay.xaml
	/// </summary>
	public partial class TagDisplay : UserControl
	{
		public IReadOnlyCollection<KeyValuePair<string, IReadOnlyList<string>>> Tags { get; }

		public TagDisplay()
		{
			Tags = new Dictionary<string, IReadOnlyList<string>>()
			{
				{ "first", new List<string>() {"aaa", "bbbb", "cccc", "ddddd", "eeeee", "ffffff", "gggggggg" } },
				{ "second", new List<string>() {"bbbb", } },
				{ "c", new List<string>() {"aaa", "bbbb", "cccc", "ddddd", "eeeee", "ffffff", "ggffffffffffffffffffffffhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhfffgggggg" } },
				{ "f", new List<string>() {"aaa", "bbbb", "cccc", "ddddd", "eeeee", "ffffff", "gggggggg" } },
				{ "a", new List<string>() {"aaa", "bbbb", "cccc", "ddddd", "eeeee", "ffffff", "gggggggg" } },
				{ "ererererer", new List<string>() {"aaa", "bbbb", "cccc", "ddddd", "eeeee", "ffffff", "gggggggg" } },
				{ "z", new List<string>() {"aaa", "bbbb", "cccc", "ddddd", "eeeee", "ffffff", "gggggggg" } },
				{ "vv", new List<string>() {"aaa", "bbbb", "cccc", "ddddd", "eeeee", "ffffff", "gggggggg" } },
				{ "xx", new List<string>() {"aaa", "bbbb", "cccc", "ddddd", "eeeee", "ffffff", "gggggggg" } },
				{ "wewe", new List<string>() {"aaa", "bbbb", "cccc", "ddddd", "eeeee", "ffffff", "gggggggg" } },
				{ "zw", new List<string>() {"aaa", "bbbb", "cccc", "ddddd", "eeeee", "ffffff", "gggggggg" } },
			};
			InitializeComponent();
		}
	}
}
