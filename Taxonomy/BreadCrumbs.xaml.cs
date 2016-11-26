using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
	public enum Mode
	{
		BreadCrumbs,
		Editing
	}

	/// <summary>
	/// Interaction logic for BreadCrumbs.xaml
	/// </summary>
	public partial class BreadCrumbs : UserControl, INotifyPropertyChanged
	{
		private ObservableCollection<string> Components { get; }

		private Mode mode;

		public Mode Mode
		{
			get { return mode; }
			set { mode = value; OnPropertyChanged(); }
		}

		public string Path
		{
			get { return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), Components.Skip(1)); }

			set
			{
				Components.Clear();
				Components.Add("This Computer");
				value = value.TrimEnd();
				value = value.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ? value.Substring(0, value.Length-1) : value;
				foreach(var component in value.Split(System.IO.Path.DirectorySeparatorChar))
				{
					Components.Add(component);
				}
				Mode = Mode.BreadCrumbs;
			}
		}

		public BreadCrumbs()
		{
			Components = new ObservableCollection<string>();
			Path = @"E:\PROJEKTY\Taxonomy\TaxonomyLib";
			InitializeComponent();
			ItemsControl.ItemsSource = Components;
		}

		public BreadCrumbs(string path)
		{
			Path = path;
		}

		private void OnComponentClick(object sender, RoutedEventArgs e)
		{
			var tag = ((Button) sender).Tag;
			var list = (IList<string>) Components;
			var index = list.IndexOf((string)tag);
			for(int i = list.Count - 1; i >= index + 1; i--)
			{
				list.RemoveAt(i);
			}
		}

		private void OnEditPathClick(object sender, RoutedEventArgs e)
		{
			Mode = Mode.Editing;
		}

		private void OnLostFocusTextBox(object sender, KeyboardFocusChangedEventArgs e)
		{
			var bindingExpression = BindingOperations.GetBindingExpression((TextBox)sender, TextBox.TextProperty);
			bindingExpression?.UpdateTarget();
			Mode = Mode.BreadCrumbs;
		}


		private void OnKeyPressedDown(object sender, KeyEventArgs e)
		{
			var bindingExpression = BindingOperations.GetBindingExpression((TextBox)sender, TextBox.TextProperty);
			if (e.Key == Key.Enter)
			{
				bindingExpression?.UpdateSource();
				Mode = Mode.BreadCrumbs;
			}
			if(e.Key == Key.Escape)
			{
				bindingExpression?.UpdateTarget();
				Mode = Mode.BreadCrumbs;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void PathEditorLostFocus(object sender, RoutedEventArgs e)
		{
			Mode = Mode.BreadCrumbs;
		}
	}

	public class ModeToVisibilityConverter : IValueConverter
	{
		public static readonly ModeToVisibilityConverter Default = new ModeToVisibilityConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var activeMode = (Mode) value;
			var visibleMode = (Mode) parameter;
			return activeMode == visibleMode ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
