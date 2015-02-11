using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
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
			Data = new Library();
			View = new MainViewSettings();
			onPropertyChanged("Data");
		}
		
		public ICommand DoubleClick { get; set; }
	}
}
