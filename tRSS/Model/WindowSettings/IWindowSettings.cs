
using System;
using System.Windows;
using tRSS.Utilities;

namespace tRSS.Model.WindowSettings
{
	/// <summary>
	/// Abstract class for window settings
	/// </summary>
	public abstract class IWindowSettings : INotifyBase
	{
		public string Filename
		{
			get
			{
				return this.GetType().Name + ".xml";
			}
		}
		
		private ResizeMode _Resizable;
		public ResizeMode Resizable
		{
			get
			{
				return _Resizable;
			}
			set
			{
				_Resizable = value;
				onPropertyChanged("Resizable");
			}
		}
		
		private double _Width;
		public double Width
		{
			get
			{
				return _Width;
			}
			set
			{
				_Width = value;
				onPropertyChanged("Width");
				// FIXME Kan slettes
				onPropertyChanged("Print");
			}
		}
		
		private double _Height;
		public double Height
		{
			get
			{
				return _Height;
			}
			set
			{
				_Height = value;
				onPropertyChanged("Height");
				// FIXME Kan slettes
				onPropertyChanged("Print");
			}
		}
		
		private double _Top;
		public double Top
		{
			get
			{
				return _Top;
			}
			set
			{
				_Top = value;
				onPropertyChanged("Top");
			}
		}
		
		private double _Left;
		public double Left
		{
			get
			{
				return _Left;
			}
			set
			{
				_Left = value;
				onPropertyChanged("Left");
			}
		}
		
		private WindowState _State;
		public WindowState State
		{
			get
			{
				return _State;
			}
			set
			{
				_State = value;
				onPropertyChanged("State");
			}
		}
	}
}
