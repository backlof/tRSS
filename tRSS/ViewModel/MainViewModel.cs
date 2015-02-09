using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
		
		// TODO DataContractSerialization for XML (References are kept)
		
		public MainViewModel()
		{
			Data = new Library();
			View = new MainViewSettings();
			onPropertyChanged("Data");
		}
		
		
	}
}
