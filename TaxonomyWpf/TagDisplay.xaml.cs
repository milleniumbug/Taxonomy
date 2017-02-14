using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
			typeof(FileEntry),
			typeof(TagDisplay),
			new FrameworkPropertyMetadata(default(FileEntry), OnFileChanged) {BindsTwoWayByDefault = true});

		public FileEntry File
		{
			get { return (FileEntry)GetValue(FileProperty); }
			set { SetValue(FileProperty, value); }
		}

		private void UpdateTagDisplay()
		{
			tags.Clear();
			tags.AddRange(File?.File?.Tags
				.GroupBy(tag => tag.Namespace)
				.Select(group => new KeyValuePair<Namespace, IReadOnlyList<Tag>>(group.Key, group.ToList()))
				?? Enumerable.Empty<KeyValuePair<Namespace, IReadOnlyList<Tag>>>());
		}

		private static void OnFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var self = (TagDisplay)d;
			var fileEntry = (FileEntry)e.NewValue;
			var file = fileEntry?.File;
			var oldFile = ((FileEntry)e.OldValue)?.File;
			self.tags.Clear();
			{
				var notifyCollectionChanged = oldFile?.Tags as INotifyCollectionChanged;
				if(notifyCollectionChanged != null)
				{
					notifyCollectionChanged.CollectionChanged -= self.TagCollectionChanged;
				}
			}
			{
				var notifyCollectionChanged = file?.Tags as INotifyCollectionChanged;
				if(notifyCollectionChanged != null)
				{
					notifyCollectionChanged.CollectionChanged += self.TagCollectionChanged;
				}
			}
			self.UpdateTagDisplay();
		}

		private void TagCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			UpdateTagDisplay();
		}

		public TagDisplay()
		{
			tags = new ObservableBatchCollection<KeyValuePair<Namespace, IReadOnlyList<Tag>>>();
			InitializeComponent();
		}
	}
}
