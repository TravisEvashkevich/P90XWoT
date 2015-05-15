using System;
using System.Windows.Input;

namespace Dae.ToolDev.Framework
{
	/// <summary>
	/// A command that takes a parameter.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DelegateCommand<T> : IDelegateCommand
	{
		public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			Execute = execute;
			CanExecute = canExecute;
		}

		public DelegateCommand(Action<T> execute, Func<bool> canExecute) 
			: this(execute, parameter => canExecute())
		{
		}

		public Action<T> Execute { get; private set; }

		public Func<T, bool> CanExecute { get; private set; }

		public event EventHandler CanExecuteChanged;

		public void Update()
		{
			if (CanExecute != null)
			{
				EventHandler handler = CanExecuteChanged;
				if (handler != null)
					handler(this, EventArgs.Empty);
			}
		}

		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute == null || CanExecute((T)parameter);
		}

		void ICommand.Execute(object parameter)
		{
			Execute((T)parameter);
		}
	}
}