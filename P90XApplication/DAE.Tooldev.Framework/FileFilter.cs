namespace Dae.ToolDev.Framework
{
	public struct FileFilter
	{
		/// <summary>
		/// A friendly name for the extension, i.e. "PowerPoint files"
		/// </summary>
		public readonly string Label;

		/// <summary>
		/// The file extensions, i.e. "*.ppt"
		/// </summary>
		public readonly string[] Extensions;

		public FileFilter(string label, params string[] extensions)
		{
			Label = label;
			Extensions = extensions;
		}

		public override string ToString()
		{
			return Label + "|" + string.Join(";", Extensions);
		}
	}
}