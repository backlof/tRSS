/*
 * Created by SharpDevelop.
 * User: Alexander
 * Date: 16.01.2015
 * Time: 21:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
