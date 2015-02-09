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
		public ViewSettings View { get; set; }
		
		// TODO Add IsActive in filter
		// TODO Make filter part of grid into a stackpanel instead of grid, only the upper row is grid 2 columns
		// TODO Remember column width in datagrid
		// TODO Make filter groupbox static size
		// TODO Make TV Show groupbox minimum size
		
		public MainViewModel()
		{
			Data = new Library();
			View = new ViewSettings();
			onPropertyChanged("Data");
		}
	}
}
