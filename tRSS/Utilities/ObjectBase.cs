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
		
		private static readonly string SAVE_LOCATION = "Data";

		protected internal void onPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private string SaveLocation()
		{
			return SaveLocation(this.GetType());
		}
		
		public static string SaveLocation(Type type)
		{
			return Path.Combine(SAVE_LOCATION, type.Name + ".xml");
		}
		
		public void Save()
		{
			DataContractSerializer dcs = new DataContractSerializer(this.GetType());
			XmlWriterSettings xws = new XmlWriterSettings(){ Indent = true };
			
			if(!Directory.Exists(Path.GetDirectoryName(SaveLocation())))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(SaveLocation()));
			}
			using (XmlWriter xw = XmlWriter.Create(SaveLocation(), xws))
			{
				dcs.WriteObject(xw, this);
			}
		}
	}
}
