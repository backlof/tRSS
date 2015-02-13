
using System;
using System.Runtime.Serialization;
using System.Windows;

namespace tRSS.Model.WindowSettings
{
	[DataContract()]
	public class MainViewSettings : IWindowSettings
	{
		public MainViewSettings()
		{
		}
		
		public const string FILENAME = "MainView";
		
		[IgnoreDataMemberAttribute()]
		public string Print 
		{ 
			get
			{
				return "X=" + Width + "Y=" + Height;
			}
		}

	}
}
