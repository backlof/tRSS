using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace tRSS.Utilities
{
	/// <summary>
	/// Abstract base class for all objects with notification and save capabilities
	/// </summary>
	[DataContract()]
	public abstract class ObjectBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		
		private static readonly string SAVE_IN_FOLDER = "Data";

		protected internal void onPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		public static string SaveLocation(string filename)
		{
			return Path.Combine(SAVE_IN_FOLDER, filename + ".xml");
		}
		
		public void Save(string filename)
		{			
			DataContractSerializer dcs = new DataContractSerializer(this.GetType());
			XmlWriterSettings xws = new XmlWriterSettings(){ Indent = true };
			
			if(!Directory.Exists(Path.GetDirectoryName(SaveLocation(filename))))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(SaveLocation(filename)));
			}
			using (XmlWriter xw = XmlWriter.Create(SaveLocation(filename), xws))
			{
				dcs.WriteObject(xw, this);
			}
		}
	}
}
