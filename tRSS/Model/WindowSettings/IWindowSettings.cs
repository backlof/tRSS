
using System;
using System.Runtime.Serialization;
using System.Windows;
using tRSS.Utilities;

namespace tRSS.Model.WindowSettings
{
	[DataContract()]
	public abstract class IWindowSettings : INotifyBase
	{		
		private ResizeMode _Resizable = ResizeMode.CanResize;
		[DataMember()]
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
		[DataMember()]
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
		[DataMember()]
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
		
		private double _Top = 0;
		[DataMember()]
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
		
		private double _Left = 0;
		[DataMember()]
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
		
		private WindowState _State = WindowState.Normal;
		[DataMember()]
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
