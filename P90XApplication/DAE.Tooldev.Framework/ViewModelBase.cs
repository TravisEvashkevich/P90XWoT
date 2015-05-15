using System.ComponentModel;
using System.Windows;

namespace Dae.ToolDev.Framework
{
	public class ViewModelBase : PropertyChangeNotifier
	{
		private static bool? _isInDesignMode;

		/// <summary>
		/// Gets a value indicating whether the control is in design mode (running in Blend or the Visual Studio designer).
		/// </summary>
		public static bool IsInDesignMode
		{
			get
			{
				if (!_isInDesignMode.HasValue)
				{
#if SILVERLIGHT
					_isInDesignMode = DesignerProperties.IsInDesignTool;
#else
					var prop = DesignerProperties.IsInDesignModeProperty;
					_isInDesignMode
						= (bool)DependencyPropertyDescriptor
						.FromProperty(prop, typeof(FrameworkElement))
						.Metadata.DefaultValue;
#endif
				}

				return _isInDesignMode.Value;
			}
		}		 
	}
}