using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace tRSS.Utilities
{
	[Serializable()]
	public abstract class ObjectBase : INotifyPropertyChanged
	{
		[field:NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected internal void onPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private static readonly string SAVE_IN_FOLDER = "Data";
		
		public static string SaveLocation(string filename)
		{
			return Path.Combine(SAVE_IN_FOLDER, filename + ".bin");
		}
		
		public void Save(string filename)
		{
			if(!Directory.Exists(Path.GetDirectoryName(SaveLocation(filename))))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(SaveLocation(filename)));
			}
			using (Stream stream = File.Open(SaveLocation(filename), FileMode.Create))
			{
				BinaryFormatter bFormatter = new BinaryFormatter();
				bFormatter.Serialize(stream, this);
			}
		}
	}
}
