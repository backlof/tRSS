
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace tRSS.Model.WindowSettings
{
	[DataContract()]
	public class MainViewSettings : IWindowSettings
	{
		public MainViewSettings()
		{
		}
		
		public const string FILENAME = "MainView";
	}
}
