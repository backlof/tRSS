using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using tRSS.Model;
using tRSS.Model.WindowSettings;
using tRSS.Utilities;

namespace tRSS.ViewModel
{
	/// <summary>
	/// Description of MainViewModel.
	/// </summary>
	public class MainViewModel : INotifyBase
	{
		public Library Data { get; set; }
		public MainViewSettings View { get; set; }
		
		// TODO DataContractSerialization for XML (referanser kan spesifiseres)
		
		public MainViewModel()
		{
			Load();
		}
		
		
		private const string DATA_LOCATION = "Data";
		
		private string Location(string filename)
		{
			return Path.Combine(DATA_LOCATION, filename + ".xml");
		}
		
		public void Load()
		{
			string path;
			
			
			path = Location(Library.FILENAME);
			if(File.Exists(path))
			{
				using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer dcs = new DataContractSerializer(typeof(Library));
					Data = dcs.ReadObject(fs) as Library;
				}
			}
			else
			{
				Data = new Library();
			}
			
			
			path = Location(MainViewSettings.FILENAME);
			if(File.Exists(path))
			{
				using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer dcs = new DataContractSerializer(typeof(MainViewSettings));
					View = dcs.ReadObject(fs) as MainViewSettings;
				}
			}
			else
			{
				View = new MainViewSettings();
			}
		}
		
		public void Save()
		{
			string path;
			DataContractSerializer dcs;
			XmlWriterSettings xws = new XmlWriterSettings(){ Indent = true };
			
			path = Location(Library.FILENAME);
			dcs = new DataContractSerializer(typeof(Library));
			if(!Directory.Exists(Path.GetDirectoryName(path)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			}
			using (XmlWriter xw = XmlWriter.Create(path, xws))
			{
				dcs.WriteObject(xw, Data);
			}
			
			
			path = Location(MainViewSettings.FILENAME);
			dcs = new DataContractSerializer(typeof(MainViewSettings));
			if(!Directory.Exists(Path.GetDirectoryName(path)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			}
			using (XmlWriter xw = XmlWriter.Create(path, xws))
			{
				dcs.WriteObject(xw, View);
			}
		}
	}
}
