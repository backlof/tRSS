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
using System.Runtime.Serialization.Formatters.Binary;

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
		}
		
		public void Load()
		{			
			if (File.Exists(ObjectBase.SaveLocation(DATA_FILENAME)))
			{
				using (Stream stream = File.Open(ObjectBase.SaveLocation(DATA_FILENAME), FileMode.Open))
				{
					BinaryFormatter bFormatter = new BinaryFormatter();
					Data = bFormatter.Deserialize(stream) as Library;
				}
			}
			else
			{
				Data = new Library();
			}
			
			if (File.Exists(ObjectBase.SaveLocation(VIEW_FILENAME)))
			{
				using (Stream stream = File.Open(ObjectBase.SaveLocation(VIEW_FILENAME), FileMode.Open))
				{
					BinaryFormatter bFormatter = new BinaryFormatter();
					View = bFormatter.Deserialize(stream) as MainViewSettings;
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
		
		public void Update()
		{
			Data.Update();
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
