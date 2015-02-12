using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Xml;
using tRSS.Utilities;
using System.Runtime.Serialization;

namespace tRSS.Model
{
	/// <summary>
	/// Has lists
	/// Executes business logic so that ViewModels don't have to
	/// Example: Updates
	/// </summary>
	
	[DataContract()]
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
			walkingDead.FilterEpisode = true;
			walkingDead.Season = 5;
			walkingDead.Episode = 10;
			Filters.Add(walkingDead);
			Filters.Add(new Filter("Family Guy"));
			Filters.Add(new Filter("South Park"));
			Filters.Add(new Filter("Tiny House Nation"));
		}
		
		public const string FILENAME = "Library";
		
		# region Properties
		
		private Feed _Test;
		[IgnoreDataMember()]
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
		[DataMember()]
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
		[DataMember()]
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
		[IgnoreDataMember()]
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
		
		
		private int _SelectedFeedIndex;
		[IgnoreDataMember()]
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
		[IgnoreDataMember()]
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
		
		private Filter _SelectedFilter;
		[IgnoreDataMember()]
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
		
		private int _UpdateInMinutes = 5;
		[DataMember()]
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
		[DataMember()]
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
		
		public void Update()
		{
			foreach (Feed feed in Feeds)
			{
				feed.Update();
			}
			foreach (Filter filter in Filters)
			{				
				filter.FilterFeed(Feeds[filter.SearchInFeedIndex]);
			}
		}
		
		# region Commands
		
		// --------- DELETE FEED ----------------
		
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
		
		// --------- DELETE FILTER ----------------
		
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
			f.SearchInFeedIndex = SelectedFilter.SearchInFeedIndex;
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
		
		// --------- BROWSE DIRECTORY ----------------
		
		
		public ICommand ChooseDirectory
		{
			get
			{
				return new RelayCommand(ExecuteChooseDirectory, CanChooseDirectory);
			}
		}
		
		public void ExecuteChooseDirectory(object parameter)
		{
			// UNDONE Relative paths don't work
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				dialog.SelectedPath = TorrentDropLocation;
				System.Windows.Forms.DialogResult result = dialog.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					string path = dialog.SelectedPath + Path.DirectorySeparatorChar;
					TorrentDropLocation = Paths.MakeRelativePath(path);
				}
			}
		}
		
		public bool CanChooseDirectory(object parameter)
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
