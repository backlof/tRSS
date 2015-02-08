using System;
using System.ComponentModel;

namespace tRSS.Utilities
{
	/// <summary>
	/// Description of INotifyBase.
	/// </summary>
	public abstract class INotifyBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

        protected internal void onPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                 PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
	}
}
