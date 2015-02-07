/*
 * Created by SharpDevelop.
 * User: Alexander
 * Date: 16.01.2015
 * Time: 21:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		
        /*
        private bool _IsLoading = true;
        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            set
            {
                _IsLoading = value;
                onPropertyChanged("IsLoading");
                onPropertyChanged("IsLoaded");
            }
        }

        public bool IsLoaded
        {
            get
            {
                return !IsLoading;
            }
        }*/
		
        // Can be used to notify all Models and ViewModels when application has internet access        
        /*
        private bool _IsOffline = false;
        public bool IsOffline
        {
            get
            {
                return _IsOffline;
            }
            set
            {
                _IsOffline = value;
                onPropertyChanged("IsOffline");
            }
        }
        */
	}
}
