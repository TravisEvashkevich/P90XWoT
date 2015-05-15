using System.Windows;

using Microsoft.Win32;

namespace Dae.ToolDev.Framework
{
	public class ModalDialogFilePathProvider : IFilePathProvider
	{
		public string GetFilePathForSaving(string title, string currentFilePath, string extension, params FileFilter[] fileFilters)
		{
			var dialog = new SaveFileDialog
			{
				FileName = currentFilePath,
				AddExtension = true,
				DefaultExt = extension,
				Filter = string.Join("|", fileFilters),
				Title = title
			};

			return dialog.ShowDialog(Application.Current.MainWindow) == true ? dialog.FileName : null;
		}

		public string GetFilePathForLoading(string title, string currentFilePath, string extension, params FileFilter[] fileFilters)
		{
			var dialog = new OpenFileDialog
			{
				FileName = currentFilePath,
				DefaultExt = extension,
				Filter = string.Join("|", fileFilters),
				Title = title
			};

			return dialog.ShowDialog(Application.Current.MainWindow) == true ? dialog.FileName : null;
		}
	}
}