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
using Common;
using TaxonomyLib;

namespace TaxonomyWpf
{
	/// <summary>
	/// Interaction logic for TagDisplay.xaml
	/// </summary>
	public partial class TagDisplay : UserControl
	{
		private readonly ObservableBatchCollection<KeyValuePair<Namespace, IReadOnlyList<Tag>>> tags;
		public IReadOnlyCollection<KeyValuePair<Namespace, IReadOnlyList<Tag>>> Tags => tags;

		public static readonly DependencyProperty FileProperty = DependencyProperty.Register(
			nameof(File),
			typeof(File),
			typeof(TagDisplay),
			new PropertyMetadata(default(File), OnFileChanged));

		public File File
		{
			get { return (File)GetValue(FileProperty); }
			set { SetValue(FileProperty, value); }
		}

		private static void OnFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var self = (TagDisplay)d;
			var file = (File)e.NewValue;
			self.tags.Clear();
			if(file != null)
				self.tags.AddRange(file.Tags
					.GroupBy(tag => tag.Namespace)
					.Select(group => new KeyValuePair<Namespace, IReadOnlyList<Tag>>(group.Key, group.ToList())));

		}

		public TagDisplay()
		{
			tags = new ObservableBatchCollection<KeyValuePair<Namespace, IReadOnlyList<Tag>>>();
			InitializeComponent();
		}
	}
}
