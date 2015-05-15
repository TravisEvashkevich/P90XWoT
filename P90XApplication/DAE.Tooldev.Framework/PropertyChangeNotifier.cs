using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Dae.ToolDev.Framework
{
	/// <summary>
	/// Implements INotifyPropertyChanged. 
	/// Uses features from .NET Framework 4.5 to automatically detect the property-name 
	/// </summary>
	public abstract class PropertyChangeNotifier : INotifyPropertyChanged
	{
		private int _propertyChangedEventSuppressionCounter;

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the PropertyChanged event, unless it has been suppressed.
		/// </summary>
		/// <remarks>
		/// Pass string.Empty to indicate that all properties have been changed
		/// </remarks>
		/// <param name="propertyName"></param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			Debug.Assert(GetType().GetProperty(propertyName) != null);

			if (_propertyChangedEventSuppressionCounter == 0)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
					handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public bool IsPropertyChangedEventSuppressed
		{
			get { return _propertyChangedEventSuppressionCounter > 0; }
		}

		/// <summary>
		/// Suppress the PropertyChanged event. 
		/// To enable to it again, call Dispose on the result 
		/// (typically you put this call inside a using statement)
		/// </summary>
		/// <returns></returns>
		public IDisposable SuppressPropertyChangedEvent()
		{
			++_propertyChangedEventSuppressionCounter;
			return new Disposable(() => --_propertyChangedEventSuppressionCounter);
		}

		private void SetValue<T>(ref T field, T newValue, Action<T> onChanged, IEqualityComparer<T> comparer, string propertyName)
		{
			if (!comparer.Equals(field, newValue))
			{
				var oldValue = field;

				field = newValue;

				OnPropertyChanged(propertyName);

				if (onChanged != null)
					onChanged(oldValue);
			}
		}

		/// <summary>
		/// If the field stores a different value (as determined by the comparer),
		/// sets the field to the new value, then raises the PropertyChanged event, 
		/// then invokes a callback passing in the old value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <param name="newValue"></param>
		/// <param name="comparer"></param>
		/// <param name="propertyName">The property that changed, or null if all properties changed</param>
		/// <param name="onChanged">Action that will be called when the property changed</param>
		protected void Set<T>(ref T field, T newValue, Action<T> onChanged, IEqualityComparer<T> comparer, [CallerMemberName] string propertyName = null)
		{
			SetValue(ref field, newValue, onChanged, comparer, propertyName);
		}

		/// <summary>
		/// If the field stores a different value,
		/// sets the field to the new value, then raises the PropertyChanged event, 
		/// then invokes a callback passing in the old value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <param name="newValue"></param>
		/// <param name="propertyName">The property that changed, or null if all properties changed</param>
		/// <param name="onChanged">Action that will be called when the property changed</param>
		protected void Set<T>(ref T field, T newValue, Action<T> onChanged, [CallerMemberName] string propertyName = null)
		{
			SetValue(ref field, newValue, onChanged, EqualityComparer<T>.Default, propertyName);
		}

		/// <summary>
		/// If the field stores a different value (as determined by the comparer),
		/// sets the field to the new value, then raises the PropertyChanged event, 
		/// then invokes a callback.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <param name="newValue"></param>
		/// <param name="comparer"></param>
		/// <param name="propertyName">The property that changed, or null if all properties changed</param>
		/// <param name="onChanged">Action that will be called when the property changed</param>
		protected void Set<T>(ref T field, T newValue, Action onChanged, IEqualityComparer<T> comparer, [CallerMemberName] string propertyName = null)
		{
			SetValue(ref field, newValue, oldValue => onChanged(), comparer, propertyName);
		}

		/// <summary>
		/// If the field stores a different value,
		/// sets the field to the new value, then raises the PropertyChanged event, 
		/// then invokes a callback.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <param name="newValue"></param>
		/// <param name="propertyName">The property that changed, or null if all properties changed</param>
		/// <param name="onChanged">Action that will be called when the property changed</param>
		protected void Set<T>(ref T field, T newValue, Action onChanged, [CallerMemberName] string propertyName = null)
		{
			SetValue(ref field, newValue, oldValue => onChanged(), EqualityComparer<T>.Default, propertyName);
		}

		/// <summary>
		/// If the field stores a different value (as determined by the comparer),
		/// sets the field to the new value, then raises the PropertyChanged event, 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <param name="newValue"></param>
		/// <param name="comparer"></param>
		/// <param name="propertyName">The property that changed, or null if all properties changed</param>
		protected void Set<T>(ref T field, T newValue, IEqualityComparer<T> comparer, [CallerMemberName] string propertyName = null)
		{
			SetValue(ref field, newValue, null, comparer, propertyName);
		}

		/// <summary>
		/// If the field stores a different value,
		/// sets the field to the new value, then raises the PropertyChanged event, 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <param name="newValue"></param>
		/// <param name="propertyName">The property that changed, or null if all properties changed</param>
		protected void Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			SetValue(ref field, newValue, null, EqualityComparer<T>.Default, propertyName);
		}
	}
}

