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
using System.Windows.Threading;

namespace tRSS.ViewModel
{
	public class MainViewModel : ObjectBase
	{
		public Library Data { get; set; }
		public MainViewSettings View { get; set; }
		
		public MainViewModel()
		{
			Load();
			Data.Update();
		}
		
		private const string DATA_LOCATION = "Data";
		
		private string Location(string filename)
		{
			return Path.Combine(DATA_LOCATION, filename + ".xml");
		}
		
		public void Load()
		{
			string path;
			
			path = ObjectBase.SaveLocation(typeof(Library));			
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
			
			
			path = ObjectBase.SaveLocation(typeof(MainViewSettings));
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
		
		public void SaveData()
		{
			Data.Save();
			View.Save();
		}
		
		public ICommand SaveCommand
		{
			get
			{
				return new RelayCommand(ExecuteSaveCommand, CanSaveCommand);
			}
		}
		
		public void ExecuteSaveCommand(object parameter)
		{
			SaveData();
		}
		
		public bool CanSaveCommand(object parameter)
		{
			return true;
		}
	}
}
