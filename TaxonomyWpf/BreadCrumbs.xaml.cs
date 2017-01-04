using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace TaxonomyWpf
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
		public ObservableCollection<KeyValuePair<int, string>> Components { get; }

		private Mode mode;

		public Mode Mode
		{
			get { return mode; }
			set { mode = value; OnPropertyChanged(); }
		}

		public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
			nameof(Path),
			typeof(string),
			typeof(BreadCrumbs),
			new PropertyMetadata(default(string), OnPathChanged, CoercePath));

		private static object CoercePath(DependencyObject d, object baseValue)
		{
			var self = (BreadCrumbs) d;
			var newValue = (string) baseValue;
			return self.JoinPath(self.SplitPath(newValue));
		}

		private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
		{
			var self = (BreadCrumbs) d;
			var newValue = (string) eventArgs.NewValue;
			self.Components.Clear();
			foreach (var indexComponentPair in self.SplitPath(newValue).Select((component, index) => new KeyValuePair<int, string>(index, component)))
			{
				self.Components.Add(indexComponentPair);
			}
			self.Mode = Mode.BreadCrumbs;
			self.OnPropertyChanged(nameof(Path));
		}

		public string Path
		{
			get { return (string) GetValue(PathProperty); }
			set { SetValue(PathProperty, value); }
		}

		private string JoinPath(IEnumerable<string> components)
		{
			return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), components.Skip(1));
		}

		private IEnumerable<string> SplitPath(string path)
		{
			var components = new List<string> {"This Computer"};
			path = path.TrimEnd();
			path = path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ? path.Substring(0, path.Length - 1) : path;
			components.AddRange(path.Split(new[]{ System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries));
			return components;
		}

		public BreadCrumbs()
		{
			Components = new ObservableCollection<KeyValuePair<int, string>>();
			Path = "";
			InitializeComponent();
		}

		public BreadCrumbs(string path)
		{
			Path = path;
		}

		private void OnComponentClick(object sender, RoutedEventArgs e)
		{
			var index = (int)((Button) sender).Tag;
			var list = Components;
			for(int i = list.Count - 1; i >= index + 1; i--)
			{
				list.RemoveAt(i);
			}
			OnPropertyChanged(nameof(Path));
		}

		private void OnEditPathClick(object sender, RoutedEventArgs e)
		{
			Mode = Mode.Editing;
			EditPathTextBox.Focus();
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
