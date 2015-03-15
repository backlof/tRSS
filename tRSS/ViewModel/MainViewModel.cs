using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using tRSS.Model;
using tRSS.Utilities;
using System.Windows.Threading;

namespace tRSS.ViewModel
{
	public class MainViewModel : ObjectBase
	{
		public Library Data { get; set; }
		public MainViewSettings View { get; set; }
		
		public static readonly string DATA_FILENAME = "Library";
		public static readonly string VIEW_FILENAME = "Window-Main";
		
		public MainViewModel()
		{
			Load();
			Data.Update();
		}
		
		public void Load()
		{
			string path;
			
			path = ObjectBase.SaveLocation(DATA_FILENAME);			
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
			
			
			path = ObjectBase.SaveLocation(VIEW_FILENAME);
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
			Data.Save(DATA_FILENAME);
			View.Save(VIEW_FILENAME);
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
