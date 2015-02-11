using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using tRSS.Utilities;

namespace tRSS.Model
{
	/// <summary>
	/// Has lists
	/// Executes business logic so that ViewModels don't have to
	/// Example: Updates
	/// </summary>
	public class Library : INotifyBase
	{
		public Library()
		{
			Test = new Feed();
			onPropertyChanged("Test");
			
			// HACK Testing
			
			Feeds.Add(new Feed("TV Shows", "https://tracker.com"));
			Feeds.Add(new Feed("Movies", "https://tracker.com"));
			Feeds.Add(new Feed("Music", "https://tracker.com"));
			
			Filters.Add(new Filter("The Walking Dead"));
			Filters.Add(new Filter("Family Guy"));
			Filters.Add(new Filter("South Park"));
			Filters.Add(new Filter("Tiny House Nation"));
		}
		
		# region Properties
		
		private Feed _Test;
		public Feed Test
		{
			get
			{
				return _Test;
			}
			set
			{
				_Test = value;
				onPropertyChanged("Test");
			}
		}
		
		private List<Feed> _Feeds = new List<Feed>();
		public List<Feed> Feeds
		{
			get
			{
				return _Feeds;
			}
			set
			{
				_Feeds = value;
				onPropertyChanged("Feeds");
			}
		}
		
		private List<Filter> _Filters = new List<Filter>();
		public List<Filter> Filters
		{
			get
			{
				return _Filters;
			}
			set
			{
				_Filters = value;
				onPropertyChanged("Filters");
			}
		}
		
		private Feed _SelectedFeed;
		public Feed SelectedFeed
		{
			get
			{
				return _SelectedFeed;
			}
			set
			{
				_SelectedFeed = value;
				onPropertyChanged("SelectedFeed");
			}
		}
		
		private Filter _SelectedFilter;
		public Filter SelectedFilter
		{
			get
			{
				return _SelectedFilter;
			}
			set
			{
				_SelectedFilter = value;
				onPropertyChanged("SelectedFilter");
			}
		}
		
		private int _SelectedFeedIndex;
		public int SelectedFeedIndex
		{
			get
			{
				return _SelectedFeedIndex;
			}
			set
			{
				_SelectedFeedIndex = value;
				onPropertyChanged("SelectedFeedIndex");
			}
		}
		
		private int _SelectedFilterIndex;
		public int SelectedFilterIndex
		{
			get
			{
				return _SelectedFilterIndex;
			}
			set
			{
				_SelectedFilterIndex = value;
				onPropertyChanged("SelectedFilterIndex");
			}
		}
		
		private int _UpdateInMinutes = 5;
		public int UpdateInMinutes
		{
			get
			{
				return _UpdateInMinutes;
			}
			set
			{
				_UpdateInMinutes = value;
				onPropertyChanged("UpdateInMinutes");
			}
		}
		
		private string _TorrentDropLocation = AppDomain.CurrentDomain.BaseDirectory; // HACK Might work as application
		public string TorrentDropLocation
		{
			get
			{
				return _TorrentDropLocation;
			}
			set
			{
				_TorrentDropLocation = value;
				onPropertyChanged("TorrentDropLocation");
			}
		}
		
		
		# endregion
		
		public override string ToString()
		{
			return String.Format("[Library UpdateInMinutes={0}, TorrentDropLocation={1}]", _UpdateInMinutes, _TorrentDropLocation);
		}
		
		# region Commands
		
		public ICommand DeleteFeed
		{
			get
			{
				return new RelayCommand(ExecuteDeleteFeed, CanDeleteFeed);
			}
		}
		
		public void ExecuteDeleteFeed(object parameter)
		{
			// FIXME ListBox doesn't update
			int nextIndex = SelectedFeedIndex - 1;
			if (SelectedFeedIndex == 0){
				nextIndex = 0;
			}
			Feeds.RemoveAt(SelectedFeedIndex);
			SelectedFeedIndex = nextIndex;
			onPropertyChanged("Feeds");
			
			foreach (Feed f in Feeds)
			{
				System.Diagnostics.Debug.WriteLine(f.ToString());
			}
		}
		
		public bool CanDeleteFeed(object parameter)
		{
			return Feeds.Count > 1;
		}
		
		public ICommand DeleteFilter
		{
			get
			{
				return new RelayCommand(ExecuteDeleteFilter, CanDeleteFilter);
			}
		}
		
		public void ExecuteDeleteFilter(object parameter)
		{
			// FIXME ListBox doesn't update
			Filters.RemoveAt(SelectedFilterIndex);
			onPropertyChanged("Filters");
		}
		
		public bool CanDeleteFilter(object parameter)
		{
			return Filters.Count > 1;
		}
		
		// --------- NEW FEED ----------------
		
		public ICommand NewFeed
		{
			get
			{
				return new RelayCommand(ExecuteNewFeed, CanNewFeed);
			}
		}
		
		public void ExecuteNewFeed(object parameter)
		{
			// UNDONE Needs new page or extra field to add
			Feeds.Add(new Feed());
			onPropertyChanged("Feeds");
		}
		
		public bool CanNewFeed(object parameter)
		{
			return true;
		}
		
		// --------- NEW FILTER ----------------
		
		public ICommand NewFilter
		{
			get
			{
				return new RelayCommand(ExecuteNewFilter, CanNewFilter);
			}
		}
		
		public void ExecuteNewFilter(object parameter)
		{
			Filter f = new Filter();
			Filters.Add(f);
			SelectedFilter = f;
			onPropertyChanged("Filters");
		}
		
		public bool CanNewFilter(object parameter)
		{
			return true;
		}
		
		
		
		
		# endregion
		
		// ==========================================
		//   MAKE BUSINESS LOGIC FUNCTIONALITY HERE
		// ==========================================
		
		// Update Feeds -> Filter Items
	}
}
