
using System;
using tRSS.Utilities;

namespace tRSS.Model
{
	/// <summary>
	/// Description of ViewSettings.
	/// </summary>
	public class ViewSettings : INotifyBase
	{
		public ViewSettings()
		{
		}
				
		private int _WindowWidth = 600;
		public int WindowWidth
		{
			get
			{
				return _WindowWidth;
			}
			set
			{
				_WindowWidth = value;
				onPropertyChanged("WindowWidth");
				onPropertyChanged("Stats");
			}
		}
		
		private int _WindowHeight = 400;
		public int WindowHeight
		{
			get
			{
				return _WindowHeight;
			}
			set
			{
				_WindowHeight = value;
				onPropertyChanged("WindowHeight");
				onPropertyChanged("Stats");
			}
		}
		
		public string Stats
		{
			get
			{
				//return "X=" + WindowWidth + ", Y=" + WindowHeight;
				return string.Format("[ViewSettings WindowWidth={0}, WindowHeight={1}]", _WindowWidth, _WindowHeight);
			}
		}		
		
		private int _MinWidth = 620;
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
		
		private int _MinHeight = 580;
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
		
		private int _LeftColumnMinWidth = 100;
		public int LeftColumnMinWidth
		{
			get
			{
				return _LeftColumnMinWidth;
			}
			set
			{
				_LeftColumnMinWidth = value;
				onPropertyChanged("LeftColumnMinWidth");
			}
		}
		
		// FIXME Fungerer ikke med int - trenger converter. Kanskje en annen måte å lagre bredde.
		private int _LeftColumnWitdth = 100;
		public int LeftColumnWitdth
		{
			get
			{
				return _LeftColumnWitdth;
			}
			set
			{
				_LeftColumnWitdth = value;
				onPropertyChanged("LeftColumnWitdth");
			}
		}
	}
}
