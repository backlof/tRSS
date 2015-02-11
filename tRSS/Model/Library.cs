using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			Feed tvShows = new Feed("TV Shows", "https://tracker.com");
			Feeds.Add(tvShows);
			Feeds.Add(new Feed("Movies", "https://tracker.com"));
			Feeds.Add(new Feed("Music", "https://tracker.com"));
			
			
			Filter walkingDead = new Filter("The Walking Dead");
			walkingDead.IsActive = true;
			walkingDead.TitleFilter = "The.Walking.Dead*";
			walkingDead.IgnoreCaps = true;
			walkingDead.Include = "1080p;720p";
			walkingDead.Exclude = "WEB-DL;HDTV;1080i;DIMENSION;MPEG;";
			walkingDead.SearchInFeed = tvShows;
			walkingDead.FilterEpisode = true;
			walkingDead.Season = 5;
			walkingDead.Episode = 10;
			Filters.Add(walkingDead);
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
		
		private ObservableCollection<Feed> _Feeds = new ObservableCollection<Feed>();
		public ObservableCollection<Feed> Feeds
		{
			get
			{
				return _Feeds;
			}
			set
			{
				_Feeds = value;
			}
		}
		
		private ObservableCollection<Filter> _Filters = new ObservableCollection<Filter>();
		public ObservableCollection<Filter> Filters
		{
			get
			{
				return _Filters;
			}
			set
			{
				_Filters = value;
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
			Feeds.Remove(SelectedFeed);
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
			Filters.Remove(SelectedFilter);
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
			Feed f = new Feed();
			Feeds.Add(f);
			SelectedFeed = f;
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
			
			f.TitleFilter = SelectedFilter.TitleFilter;
			f.SearchInFeed = SelectedFilter.SearchInFeed;
			f.IgnoreCaps = SelectedFilter.IgnoreCaps;
			f.Include = SelectedFilter.Include;
			f.Exclude = SelectedFilter.Exclude;
			f.FilterEpisode = SelectedFilter.FilterEpisode;
			f.Season = SelectedFilter.Season;
			f.Episode = SelectedFilter.Episode;
			
			Filters.Add(f);
			SelectedFilter = f;
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
