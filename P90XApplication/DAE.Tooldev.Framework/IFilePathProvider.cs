namespace Dae.ToolDev.Framework
{
	/// <summary>
	/// Since one of the purposes of view models is writing automated tests, 
	/// we shouldn't directly create modal dialog boxes for opening and saving files.
	/// As usual an extra level of indirection solves this problem...
	/// </summary>
	public interface IFilePathProvider
	{
		string GetFilePathForSaving(string title, string currentFilePath, string extension, params FileFilter[] fileFilters);

		string GetFilePathForLoading(string title, string currentFilePath, string extension, params FileFilter[] fileFilters);
	}
}