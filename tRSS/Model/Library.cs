using System;
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
			walkingDead.TitleFilter = "The Walking Dead";
			walkingDead.IgnoreCaps = true;
			walkingDead.Include = "720p;HDTV;";
			walkingDead.Exclude = "WEB-DL;1080i;MPEG;";
			walkingDead.FilterEpisode = true;
			walkingDead.Season = 5;
			walkingDead.Episode = 9;
			Filters.Add(walkingDead);
			Filters.Add(new Filter("Family Guy"));
			Filters.Add(new Filter("South Park"));
			Filters.Add(new Filter("Tiny House Nation"));
		}
		
		public const string FILENAME = "Library";
		
		private DispatcherTimer timedRefresh;
		
		public void StartTimer()
		{
			timedRefresh = new DispatcherTimer();
			timedRefresh.Tick += new EventHandler(timer_Tick);
			timedRefresh.Interval = new TimeSpan(0,UpdateInterval,0);
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
			// UNDONE Update feeds here when it's functioning
		}
		
		private DateTime _LastUpdate;
		[IgnoreDataMember()]
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
		
		private string _TorrentDropDirectory = ".";
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
		
		// FIXME Might not be needed
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
		
		private int[] _UpdateIntervals = { 1, 5, 15, 60 };
		[DataMember()]
		public int[] UpdateIntervals
		{
			get
			{
				return _UpdateIntervals;
			}
			set
			{
				_UpdateIntervals = value;
				onPropertyChanged("UpdateIntervals");
			}
		}
		
		private string[] _UpdateOptions = { "1 minute", "5 minutes", "15 minutes", "1 hour" };
		[DataMember()]
		public string[] UpdateOptions
		{
			get
			{
				return _UpdateOptions;
			}
			set
			{
				_UpdateOptions = value;
				onPropertyChanged("UpdateOptions");
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
				return _UpdateIntervals[SelectedUpdateOption];
			}
		}
		
		# endregion
		
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
			
			LastUpdate = DateTime.Now;
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
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				try
				{
					dialog.SelectedPath = Path.GetFullPath(TorrentDropDirectory);
				}
				catch(ArgumentException ae)
				{
					dialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
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
		
		// ==========================================
		//   MAKE BUSINESS LOGIC FUNCTIONALITY HERE
		// ==========================================
		
		// Update Feeds -> Filter Items
	}
}
