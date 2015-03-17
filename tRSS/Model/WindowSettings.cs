
using System;
using System.Runtime.Serialization;
using System.Windows;
using tRSS.Utilities;

namespace tRSS.Model
{
	[Serializable()]
	public class WindowSettings : ObjectBase
	{		
		private ResizeMode _Resizable = ResizeMode.CanResize;
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
		
		private WindowState _State = WindowState.Normal;
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
			}
		}
		
		private double _Top = 0;
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
	}
}
