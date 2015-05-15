using System.Windows.Input;

namespace Dae.ToolDev.Framework
{
	public interface IDelegateCommand : ICommand
	{
		/// <summary>
		/// Updates the command by raising the CanExecuteChanged event,
		/// causing re-evaluation of the CanExecute method.
		/// </summary>
		void Update();
	}
}