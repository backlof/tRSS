
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
			Width = 0;
			Height = 800;
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
