
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using tRSS.Utilities;

namespace tRSS.Model
{
	[Serializable()]
	public class TaskbarNotify : ObjectBase
	{
		public TaskbarNotify(){}
		
		private static readonly int INTERVAL_MILLISEC = 2000;
		
		private TaskbarItemProgressState _State = TaskbarItemProgressState.Normal;
		public TaskbarItemProgressState State
		{
			get
			{
				return _State;
			}
			set
			{
				_State = value;
				onPropertyChanged("Notification");
			}
		}
		
		private double _Progress = 1;
		public double Progress
		{
			get
			{
				return _Progress;
			}
			set
			{
				_Progress = value;
				onPropertyChanged("Progress");
			}
		}
		
		private bool _WindowFocus = true;
		public bool WindowFocus
		{
			get
			{
				return _WindowFocus;
			}
			set
			{
				if (IsActive)
				{
					Deactivate();
				}
				_WindowFocus = value;
				onPropertyChanged("WindowFocus");
			}
		}
		
		public bool IsActive
		{ 
			get
			{
				return State != TaskbarItemProgressState.None;
			}
		}
		
		public bool WindowIsActive = true;
		
		
		public void Indeterminate()
		{
			Activate(TaskbarItemProgressState.Indeterminate);
		}
		
		public void Error()
		{
			Activate(TaskbarItemProgressState.Error);
		}
		
		public void Regular()
		{
			Activate(TaskbarItemProgressState.Normal);
		}
		
		private void Activate(TaskbarItemProgressState state)
		{
			State = state;
			Progress = 1;
			
			if (WindowIsActive)
			{
				TimedDeactive(INTERVAL_MILLISEC);
			}
		}
		
		private async void TimedDeactive(int millisecs)
		{
			await System.Threading.Tasks.Task.Delay(2000);
			Deactivate();
		}
		
		private void Deactivate()
		{
			State = TaskbarItemProgressState.None;
			Progress = 0;
		}
	}
}
