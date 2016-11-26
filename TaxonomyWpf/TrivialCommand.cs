using System;
using System.Windows.Input;

namespace TaxonomyWpf
{
	public class TrivialCommand<T> : ICommand
	{
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			command((T) parameter);
		}

		public event EventHandler CanExecuteChanged;

		public TrivialCommand(Action<T> command)
		{
			this.command = command;
		}

		private readonly Action<T> command;
	}
}