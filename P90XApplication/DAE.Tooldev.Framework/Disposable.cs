using System;

namespace Dae.ToolDev.Framework
{
	/// <summary>
	/// Calls an action when disposed.
	/// </summary>
	public sealed class Disposable : IDisposable
	{
		private Action _disposer;

		public Disposable(Action disposer)
		{
			_disposer = disposer;
		}

		public void Dispose()
		{
			var disposer = _disposer;
			if (disposer != null)
			{
				_disposer = null;
				disposer();
			}
		}
	}
}