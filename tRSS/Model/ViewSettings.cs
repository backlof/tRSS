
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
			}
		}
	}
}
