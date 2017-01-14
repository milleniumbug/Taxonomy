using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using Gu.Reactive;
using Microsoft.Win32;
using TaxonomyLib;
using File = TaxonomyLib.File;
using System.Runtime.CompilerServices;

namespace TaxonomyWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public int IconWidth => IconDimensions.Item1;

		public int IconHeight => IconDimensions.Item2;

		public Tuple<int, int> IconDimensions { get; }

		public ICommand IconDoubleClick { get; }

		public ICommand TagDoubleClick { get; }

		public MainWindowModel Model { get; }

		public MainWindow()
		{
			IconDimensions = Tuple.Create(32, 32);
			IconDoubleClick = new TrivialCommand<FileEntry>(OnIconDoubleClick);
			TagDoubleClick = new TrivialCommand<Tag>(OnTagDoubleClick);

			InitializeComponent();
			Model = (MainWindowModel)DataContext;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			Model.Dispose();
		}

		private void OnRemoveTagClick(object sender, RoutedEventArgs e)
		{
			var taxonomyItem = (TaxonomyItem)(((Button) sender).Tag);
			Model.CloseTaxonomy(taxonomyItem);
		}

		private void OnIconDoubleClick(FileEntry b)
		{
			Model.IssueDefaultAction(b);
		}

		private void OnTagDoubleClick(Tag tag)
		{
			Model.AddTagToSearchQuery(tag);
		}

		private void NewTaxonomyClick(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog();

			if(dialog.ShowDialog() == true)
			{
				Model.CreateNewTaxonomy(dialog.FileName, System.IO.Path.GetFileNameWithoutExtension(dialog.FileName));
			}
		}

		private void OpenTaxonomyClick(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			if(dialog.ShowDialog() == true)
			{
				Model.OpenTaxonomy(dialog.FileName);
			}
		}

		private Point? tagClickStartPoint;

		private void TagPreviewLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			tagClickStartPoint = e.GetPosition(null);
		}

		private void TagPreviewMouseMove(object sender, MouseEventArgs e)
		{
			if(tagClickStartPoint == null)
				return;

			Point currentPosition = e.GetPosition(null);
			Vector distance = tagClickStartPoint.Value - currentPosition;

			if(e.LeftButton == MouseButtonState.Pressed &&
				Math.Abs(distance.X) > SystemParameters.MinimumHorizontalDragDistance &&
				Math.Abs(distance.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				// Get the dragged ListViewItem
				var treeView = (TreeView)sender;
				var item =
					FindAnchestor<TextBlock>((DependencyObject)e.OriginalSource);

				// Find the data behind the ListViewItem
				var tag = item?.DataContext as Tag;

				if(tag == null)
					return;

				// Initialize the drag & drop operation
				var dragData = new DataObject(typeof(Tag), tag);
				DragDrop.DoDragDrop(item, dragData, DragDropEffects.Move);

				tagClickStartPoint = null;
			}
		}

		private void IconEnterDrag(object sender, DragEventArgs e)
		{
			if(!e.Data.GetDataPresent(typeof(Tag)) || sender == e.Source)
			{
				e.Effects = DragDropEffects.None;;
			}
		}

		private void IconDrop(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(typeof(Tag)))
			{
				var tag = (Tag)e.Data.GetData(typeof(Tag));
				var file = (FileItem)sender;
				Model.AddTagToFile((FileEntry)file.DataContext, tag);
			}
		}

		private static T FindAnchestor<T>(DependencyObject current)
			where T : DependencyObject
		{
			do
			{
				var anchestor = current as T;
				if(anchestor != null)
				{
					return anchestor;
				}
				current = VisualTreeHelper.GetParent(current);
			} while (current != null);
			return null;
		}

		private void AddTagClick(object sender, RoutedEventArgs e)
		{
			var dialog = new AddTagDialog();
			var result = dialog.ShowDialog();
			if(result == true)
			{
				var text = dialog.TagText;
				Model.AddTag(text);
			}
		}
	}
}
