using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Dae.ToolDev.Framework
{
	/// <summary>
	/// Handy helper class to load/save objects to XML.
	/// </summary>
	public static class Serializer
	{
		public static void Save(object root, Stream stream, bool preserveReferences)
		{
			DataContractSerializer serializer = CreateSerializer(root.GetType(), preserveReferences);

			var settings = new XmlWriterSettings
			{
				// Set Indent to true to get more human readable XML files
				Indent = true,

				// We don't want to close the output stream when done.
				CloseOutput = false
			};

			using (var writer = XmlWriter.Create(stream, settings))
			{
				serializer.WriteObject(writer, root);
			}
		}

		public static T Load<T>(Stream stream, bool preserveReferences)
		{
			DataContractSerializer serializer = CreateSerializer(typeof(T), preserveReferences);
			var loaded = serializer.ReadObject(stream);
			return (T)loaded;
		}

		public static DataContractSerializer CreateSerializer(Type rootType, bool preserveReferences)
		{
			var serializer = new DataContractSerializer(
				rootType, null, int.MaxValue, true, preserveReferences, null);

			return serializer;
		}
	}
}