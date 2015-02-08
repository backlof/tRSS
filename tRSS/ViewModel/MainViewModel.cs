using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using tRSS.Model;
using tRSS.Utilities;

namespace tRSS.ViewModel
{
	/// <summary>
	/// Description of MainViewModel.
	/// </summary>
	public class MainViewModel : INotifyBase
	{
		public Library Data { get; set; }
		
		public MainViewModel()
		{
			Data = new Library();
			onPropertyChanged("Data");
		}
	}
}
