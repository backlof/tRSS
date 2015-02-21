﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using tRSS.Utilities;
using System.Runtime.Serialization;

namespace tRSS.Model
{
	[DataContract()]
	public class Library : ObjectBase
	{		
		public Library()
		{
			// HACK Testing
			Feed tvShows = new Feed("TV Shows", "https://kickass.to/tv/?rss=1");
			Feeds.Add(tvShows);
			Feeds.Add(new Feed("Movies", "https://kickass.to/movies/?rss=1"));
			Feeds.Add(new Feed("Music", "https://kickass.to/music/?rss=1"));
			
			Filter allTVShows = new Filter("All TV Shows");
			allTVShows.IsActive = false;
			allTVShows.TitleFilter = "*";
			allTVShows.IgnoreCaps = true;
			allTVShows.Include = "";
			allTVShows.Exclude = "";
			allTVShows.FilterEpisodes = false;
			allTVShows.Season = 5;
			allTVShows.Episode = 9;
			Filters.Add(allTVShows);
			
			TorrentDropDirectory = FOLDER;	
			if(!Directory.Exists(Path.GetDirectoryName(TorrentDropDirectory)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(TorrentDropDirectory));
			}
			
		}
		
		public static readonly string FOLDER = @"Torrents\";
		
		# region Timer
		
		private DispatcherTimer timedRefresh;
		
		public void StartTimer()
		{
			timedRefresh = new DispatcherTimer();
			timedRefresh.Tick += new EventHandler(timer_Tick);
			timedRefresh.Interval = new TimeSpan(0,UpdateInterval,0); // Hour, minute, second
			timedRefresh.Start();
			NextUpdate = DateTime.Now.AddMinutes(UpdateInterval);
		}
		
		public void RestartTimerWithNewInterval()
		{
			timedRefresh.Stop();
			StartTimer();
		}
		
		private void timer_Tick(object sender, EventArgs e)
		{
			Update();
			NextUpdate = DateTime.Now.AddMinutes(UpdateInterval);
		}
		
		# endregion
		
		# region Properties
		
		private DateTime _LastUpdate;
		[DataMember()]
		public DateTime LastUpdate
		{
			get
			{
				return _LastUpdate;
			}
			set
			{
				_LastUpdate = value;
				onPropertyChanged("LastUpdate");
			}
		}
		
		private DateTime _NextUpdate;
		[IgnoreDataMember()]
		public DateTime NextUpdate
		{
			get
			{
				return _NextUpdate;
			}
			set
			{
				_NextUpdate = value;
				onPropertyChanged("NextUpdate");
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
		
		private string _TorrentDropDirectory;
		[DataMember()]
		public string TorrentDropDirectory
		{
			get
			{
				return _TorrentDropDirectory;
			}
			set
			{
				_TorrentDropDirectory = value;
				onPropertyChanged("TorrentDropDirectory");
			}
		}
		
		// HACK Not sure if I need this, yet
		public List<FeedItem> DownloadedItems
		{
			get
			{
				List<FeedItem> items = new List<FeedItem>();
				foreach (Filter f in Filters)
				{
					items.AddRange(f.DownloadedItems);
				}
				return items;
			}
		}
		
		# endregion
		
		# region Update interval
		
		private static readonly int[] UPDATE_INTERVALS = { 1, 5, 15, 60 };
		private static readonly string[] UPDATE_OPTIONS = { "1 minute", "5 minutes", "15 minutes", "1 hour" };

		[DataMember()]
		public string[] UpdateOptions
		{
			get
			{
				return UPDATE_OPTIONS;
			}
		}
		
		private int _SelectedUpdateOption = 1;
		[DataMember()]
		public int SelectedUpdateOption
		{
			get
			{
				return _SelectedUpdateOption;
			}
			set
			{
				_SelectedUpdateOption = value;
				onPropertyChanged("SelectedUpdateOption");
			}
		}
		
		public int UpdateInterval
		{
			get
			{
				return UPDATE_INTERVALS[SelectedUpdateOption];
			}
		}
		
		# endregion
		
		# region Commands
		
		# region Delete feed
		
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
			
			foreach (Filter filter in Filters)
			{
				// SearchInFeedIndex becomes -1 when feed is deleted
				if (filter.SearchInFeedIndex < 0)
				{ filter.SearchInFeedIndex = 0; }
			}
		}
		
		public bool CanDeleteFeed(object parameter)
		{
			return Feeds.Count > 1;
		}
		
		# endregion
		
		# region Delete filter
		
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
		
		# endregion
		
		# region New feed
		
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
		
		# endregion
		
		# region New filter
		
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
			f.FilterEpisodes = SelectedFilter.FilterEpisodes;
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
		
		# region Choose directory
		
		public ICommand ChooseDirectory
		{
			get
			{
				return new RelayCommand(ExecuteChooseDirectory, CanChooseDirectory);
			}
		}
		
		public void ExecuteChooseDirectory(object parameter)
		{
			
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				try
				{
					dialog.SelectedPath = Path.GetFullPath(TorrentDropDirectory);
				}
				catch(ArgumentException ae)
				{
					dialog.SelectedPath = FOLDER;
					System.Diagnostics.Debug.WriteLine(ae.ToString());
				}
				
				System.Windows.Forms.DialogResult result = dialog.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					string path = dialog.SelectedPath + Path.DirectorySeparatorChar;
					TorrentDropDirectory = Paths.MakeRelativePath(path);
				}
			}
		}
		
		public bool CanChooseDirectory(object parameter)
		{
			return true;
		}
		
		# endregion
		
		# region Refresh feeds
		
		public ICommand Refresh
		{
			get
			{
				return new RelayCommand(ExecuteRefresh, CanRefresh);
			}
		}
		
		public void ExecuteRefresh(object parameter)
		{
			Update();
		}
		
		public bool CanRefresh(object parameter)
		{
			return true;
		}
		
		
		# endregion
		
		# endregion
		
		
		public void Update()
		{
			foreach (Feed feed in Feeds)
			{
				feed.Update();
			}
			foreach (Filter filter in Filters)
			{
				// Needs to use index, because SearchInFeed depends on binding and window isn't loaded
				filter.FilterFeed(Feeds[filter.SearchInFeedIndex], TorrentDropDirectory);
			}
			
			LastUpdate = DateTime.Now;
		}
	}
}
