
using System;
using System.Windows;

namespace tRSS.Model.WindowSettings
{
	/// <summary>
	/// Description of MainViewSettings.
	/// </summary>
	public class MainViewSettings : IWindowSettings
	{
		public MainViewSettings()
		{
			Resizable = ResizeMode.CanResize;
			State = WindowState.Normal;
			Width = 0;
			MinWidth = 630; //620;
			Height = 800;
			MinHeight = 660; //650;
		}
		
		private int _MinWidth;
		public int MinWidth
		{
			get
			{
				return _MinWidth;
			}
			set
			{
				_MinWidth = value;
				onPropertyChanged("MinWidth");
			}
		}
		
		private int _MinHeight;
		public int MinHeight
		{
			get
			{
				return _MinHeight;
			}
			set
			{
				_MinHeight = value;
				onPropertyChanged("MinHeight");
			}
		}
		
		public string Print 
		{ 
			get
			{
				return "X=" + Width + "Y=" + Height;
			}
		}

	}
}
