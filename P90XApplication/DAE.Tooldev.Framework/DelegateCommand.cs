using System;
using System.Windows.Input;

namespace Dae.ToolDev.Framework
{
	public class DelegateCommand : IDelegateCommand
	{
		public DelegateCommand(Action execute, Func<bool> canExecute = null)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			Execute = execute;
			CanExecute = canExecute;
		}

		public Action Execute { get; private set; }

		public Func<bool> CanExecute { get; private set; }

		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Updates the command by raising the CanExecuteChanged event,
		/// causing re-evaluation of the CanExecute method.
		/// </summary>
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
			return CanExecute == null || CanExecute();
		}

		void ICommand.Execute(object parameter)
		{
			Execute();
		}
	}
}