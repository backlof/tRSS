using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using System.Xml;
using tRSS.Model;
using tRSS.Utilities;
using System.Windows.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace tRSS.ViewModel
{
	[Serializable]
	public class MainViewModel : ObjectBase
	{
		public static readonly string FILENAME = "MainView";
		
		public MainViewModel()
		{
			TorrentDropDirectory = DROP_FOLDER;
			
			if(!Directory.Exists(Path.GetDirectoryName(TorrentDropDirectory)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(TorrentDropDirectory));
			}
			
			Feed tvFeed = new Feed(){ Title = "TV Shows", URL = "https://kickass.to/tv/?rss=1"};
			Feeds.Add(tvFeed);
			Feeds.Add(new Feed(){ Title = "Movies", URL = "https://kickass.to/movies/?rss=1" });
			Feeds.Add(new Feed(){ Title = "Music", URL = "https://kickass.to/music/?rss=1" });
			
			Filter tvFilter = new Filter(){ Enabled = false, SearchInFeed = tvFeed, TitleFilter = "*", IgnoreCaps = true, Include = "", Exclude = "1080p;HDTV;", IsTV = false };
			Filters.Add(tvFilter);
			
			SelectedFeed = tvFeed;
			SelectedFilter = tvFilter;
		}
		
		#region FILTERS
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
		
		#region RESET FILTER
		public ICommand ResetFilter
		{
			get
			{
				return new RelayCommand(ExecuteResetFilter, CanResetFilter);
			}
		}
		
		public void ExecuteResetFilter(object parameter)
		{
			SelectedFilter.DownloadedTorrents = new List<Torrent>();
			RefreshAllDownloads();
		}
		
		public bool CanResetFilter(object parameter)
		{
			if (SelectedFilter == null)
			{
				return false;
			}
			else
			{
				return SelectedFilter.DownloadedTorrents.Count > 0;
			}
		}
		
		#endregion
		
		#region REMOVE FILTER
		public ICommand RemoveFilter
		{
			get
			{
				return new RelayCommand(ExecuteRemoveFilter, CanRemoveFilter);
			}
		}
		
		public void ExecuteRemoveFilter(object parameter)
		{
			int index = Filters.IndexOf(SelectedFilter);
			
			Filters.Remove(SelectedFilter);
			
			//SelectedFilter = index > 0? Filters[index - 1] : Filters[0];
			SelectedFilter = null;
			
			RefreshAllDownloads();
			
			onPropertyChanged("CountFilters");
		}
		
		public bool CanRemoveFilter(object parameter)
		{
			return Filters.Count > 1;
		}
		
		#endregion
		
		#region NEW FILTER
		public ICommand NewFilter
		{
			get
			{
				return new RelayCommand(ExecuteNewFilter, CanNewFilter);
			}
		}
		
		public void ExecuteNewFilter(object parameter)
		{
			Filter f = new Filter(){
				TitleFilter = "",
				SearchInFeed = SelectedFilter.SearchInFeed,
				IgnoreCaps = SelectedFilter.IgnoreCaps,
				MatchOnce = SelectedFilter.MatchOnce,
				Include = SelectedFilter.Include,
				Exclude = SelectedFilter.Exclude,
			};
			
			Filters.Add(f);
			SelectedFilter = f;
			onPropertyChanged("CountFilters");
		}
		
		public bool CanNewFilter(object parameter)
		{
			return true;
		}
		
		#endregion
		
		#region FILTER SELECTED FILTER
		public ICommand FilterSelected
		{
			get
			{
				return new RelayCommand(ExecuteFilterSelected, CanFilterSelected);
			}
		}
		
		public async void ExecuteFilterSelected(object parameter)
		{
			NotifyLoading();
			
			if (await Filter(SelectedFilter))
			{
				NotifyNow();
			}
			else
			{
				NotifyDeactivate();
			}
		}
		
		public bool CanFilterSelected(object parameter)
		{
			if (SelectedFilter == null)
			{
				return false;
			}
			else
			{
				return SelectedFilter.HasFeed && SelectedFilter.Enabled;
			}
		}
		
		#endregion
		
		#region LOAD HIGHEST EPISODE
		public ICommand LoadHighestEpisode
		{
			get
			{
				return new RelayCommand(ExecuteLoadHighestEpisode, CanLoadHighestEpisode);
			}
		}
		
		public void ExecuteLoadHighestEpisode(object parameter)
		{
			SelectedFilter.LoadHighest();
		}
		
		public bool CanLoadHighestEpisode(object parameter)
		{
			if (SelectedFilter == null)
			{
				return false;
			}
			else
			{
				if (SelectedFilter.DownloadedTorrents.Count > 0 && SelectedFilter.HasDownloadedSinceHighest)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		#endregion
		
		
		
		#endregion
		
		#region FEEDS
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
		
		#region REMOVE FEED
		public ICommand RemoveFeed
		{
			get
			{
				return new RelayCommand(ExecuteRemoveFeed, CanRemoveFeed);
			}
		}
		
		public void ExecuteRemoveFeed(object parameter)
		{
			int index = Feeds.IndexOf(SelectedFeed);
			
			Feeds.Remove(SelectedFeed);
			
			//SelectedFeed = index > 0? Feeds[index - 1] : Feeds[0];
			SelectedFeed = null;
			onPropertyChanged("CountFeeds");
		}
		
		public bool CanRemoveFeed(object parameter)
		{
			return Feeds.Count > 1;
		}
		
		
		#endregion
		
		#region NEW FEED
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
			tRSS.View.EditFeed edit = new tRSS.View.EditFeed(f);
			edit.Show();
			onPropertyChanged("CountFeeds");
		}
		
		public bool CanNewFeed(object parameter)
		{
			return true;
		}
		
		#endregion
		
		#region EDIT FEED
		public ICommand EditFeed
		{
			get
			{
				return new RelayCommand(ExecuteEditFeed, CanEditFeed);
			}
		}
		
		public void ExecuteEditFeed(object parameter)
		{
			tRSS.View.EditFeed edit = new tRSS.View.EditFeed(SelectedFeed);
			edit.Show();
		}
		
		public bool CanEditFeed(object parameter)
		{
			return true;
		}
		
		#endregion
		
		#region UPDATE FEEDS
		public ICommand UpdateFeeds
		{
			get
			{
				return new RelayCommand(ExecuteUpdateFeeds, CanUpdateFeeds);
			}
		}
		
		public void ExecuteUpdateFeeds(object parameter)
		{
			Update();
			ResetTimer();
		}
		
		public bool CanUpdateFeeds(object parameter)
		{
			return true;
		}
		
		
		#endregion
		
		
		
		#endregion
		
		#region DOWNLOADS
		[NonSerialized]
		private List<Torrent> _AllDownloads = null;
		public List<Torrent> AllDownloads
		{
			get
			{
				if (_AllDownloads == null)
				{
					List<Torrent> items = new List<Torrent>();
					foreach (Filter f in Filters)
					{
						items.AddRange(f.DownloadedTorrents);
					}
					
					items.AddRange(ManuallyDownloadedTorrents);
					
					_AllDownloads = items.OrderByDescending(o=>o.Downloaded).ToList();
				}
				
				return _AllDownloads;
			}
			set
			{
				_AllDownloads = value;
				onPropertyChanged("AllDownloads");
			}
		}
		
		[NonSerialized]
		private Torrent _SelectedDownload;
		public Torrent SelectedDownload
		{
			get
			{
				return _SelectedDownload;
			}
			set
			{
				_SelectedDownload = value;
				onPropertyChanged("SelectedDownload");
			}
		}
		
		[NonSerialized]
		private Torrent _SelectedTorrent;
		public Torrent SelectedTorrent
		{
			get
			{
				return _SelectedTorrent;
			}
			set
			{
				_SelectedTorrent = value;
				onPropertyChanged("SelectedTorrent");
			}
		}
		
		// Manual downloads by user from feeds
		private List<Torrent> _ManuallyDownloadedTorrents = new List<Torrent>();
		public List<Torrent> ManuallyDownloadedTorrents
		{
			get
			{
				return _ManuallyDownloadedTorrents;
			}
			set
			{
				_ManuallyDownloadedTorrents = value;
				onPropertyChanged("ManuallyDownloadedTorrents");
			}
		}
		
		public void RefreshAllDownloads()
		{
			AllDownloads = null;
			onPropertyChanged("CountDownloads");
			onPropertyChanged("LatestDownload");
		}
		
		#region DOWNLOAD TORRENT
		public ICommand ManualDownload
		{
			get
			{
				return new RelayCommand(ExecuteManualDownload, CanManualDownload);
			}
		}
		
		public async void ExecuteManualDownload(object parameter)
		{
			Torrent t = SelectedTorrent;
			
			if (await t.Download(TorrentDropDirectory))
			{
				ManuallyDownloadedTorrents.Add(t);
				
				RefreshAllDownloads();
			}
		}
		
		public bool CanManualDownload(object parameter)
		{
			if (SelectedTorrent == null)
			{
				return false;
			}
			else
			{
				return !ManuallyDownloadedTorrents.Contains(SelectedTorrent);
			}
		}
		
		#endregion
		
		#region REMOVE DOWNLOAD
		public ICommand RemoveDownload
		{
			get
			{
				return new RelayCommand(ExecuteRemoveDownload, CanRemoveDownload);
			}
		}
		
		public void ExecuteRemoveDownload(object parameter)
		{
			if (ManuallyDownloadedTorrents.Contains(SelectedDownload))
			{
				ManuallyDownloadedTorrents.Remove(SelectedDownload);
			}
			else
			{
				foreach (Filter filter in Filters)
				{
					if (filter.DownloadedTorrents.Contains(SelectedDownload))
					{
						filter.DownloadedTorrents.Remove(SelectedDownload);
					}
				}
			}
			
			RefreshAllDownloads();
		}
		
		public bool CanRemoveDownload(object parameter)
		{
			return SelectedDownload != null;
		}
		
		#endregion
		
		
		
		#endregion
		
		#region WINDOW
		private WindowModel _Window = new WindowModel();
		public WindowModel Window
		{
			get
			{
				return _Window;
			}
			set
			{
				_Window = value;
				onPropertyChanged("Window");
			}
		}
		
		#region INFORMATION
		public string LatestDownload
		{
			get
			{
				if (AllDownloads.Count > 0)
				{
					return AllDownloads[0].Title;
				}
				else
				{
					return "None";
				}
			}
		}
		
		private int _CountUpdates = 0;
		public int CountUpdates
		{
			get
			{
				return _CountUpdates;
			}
			set
			{
				_CountUpdates = value;
				onPropertyChanged("CountUpdates");
			}
		}
		
		public string CountDownloads
		{
			get
			{
				return "" + AllDownloads.Count;
			}
		}
		
		public string CountFilters
		{
			get
			{
				return "" + Filters.Count;
			}
		}
		
		public string CountFeeds
		{
			get
			{
				return "" + Feeds.Count;
			}
		}
		
		#endregion
		
		#region TOOLBAR
		private int _SaveBand = 0;
		public int SaveBand
		{
			get
			{
				return _SaveBand;
			}
			set
			{
				_SaveBand = value;
				onPropertyChanged("SaveBand");
			}
		}
		
		private int _SaveBandIndex = 0;
		public int SaveBandIndex
		{
			get
			{
				return _SaveBandIndex;
			}
			set
			{
				_SaveBandIndex = value;
				onPropertyChanged("SaveBandIndex");
			}
		}
		
		private int _BrowseBand = 0;
		public int BrowseBand
		{
			get
			{
				return _BrowseBand;
			}
			set
			{
				_BrowseBand = value;
				onPropertyChanged("BrowseBand");
			}
		}
		
		private int _BrowseBandIndex = 1;
		public int BrowseBandIndex
		{
			get
			{
				return _BrowseBandIndex;
			}
			set
			{
				_BrowseBandIndex = value;
				onPropertyChanged("BrowseBandIndex");
			}
		}
		
		private int _IntervalBand = 0;
		public int IntervalBand
		{
			get
			{
				return _IntervalBand;
			}
			set
			{
				_IntervalBand = value;
				onPropertyChanged("IntervalBand");
			}
		}
		
		private int _IntervalBandIndex = 2;
		public int IntervalBandIndex
		{
			get
			{
				return _IntervalBandIndex;
			}
			set
			{
				_IntervalBandIndex = value;
				onPropertyChanged("IntervalBandIndex");
			}
		}
		
		private int _UpdateBand = 0;
		public int UpdateBand
		{
			get
			{
				return _UpdateBand;
			}
			set
			{
				_UpdateBand = value;
				onPropertyChanged("UpdateBand");
			}
		}
		
		private int _UpdateBandIndex = 3;
		public int UpdateBandIndex
		{
			get
			{
				return _UpdateBandIndex;
			}
			set
			{
				_UpdateBandIndex = value;
				onPropertyChanged("UpdateBandIndex");
			}
		}
		
		#endregion
		
		#region GRIDSPLITTERS
		private double _FilterSplitterPosition = 100;
		public double FilterSplitterPosition
		{
			get
			{
				return _FilterSplitterPosition;
			}
			set
			{
				_FilterSplitterPosition = value;
				onPropertyChanged("FilterSplitterPosition");
			}
		}
		
		private double _FeedSplitterPosition = 100;
		public double FeedSplitterPosition
		{
			get
			{
				return _FeedSplitterPosition;
			}
			set
			{
				_FeedSplitterPosition = value;
				onPropertyChanged("FeedSplitterPosition");
			}
		}
		
		#endregion
		
		#region DATAGRID COLUMNS
		private double _FeedWidthTitle = 250;
		public double FeedWidthTitle
		{
			get
			{
				return _FeedWidthTitle;
			}
			set
			{
				_FeedWidthTitle = value;
				onPropertyChanged("FeedTitleWidth");
			}
		}
		
		private double _FeedWidthPublished = 100;
		public double FeedWidthPublished
		{
			get
			{
				return _FeedWidthPublished;
			}
			set
			{
				_FeedWidthPublished = value;
				onPropertyChanged("FeedPublishedWidth");
			}
		}
		
		private double _FilterWidthTitle = 250;
		public double FilterWidthTitle
		{
			get
			{
				return _FilterWidthTitle;
			}
			set
			{
				_FilterWidthTitle = value;
				onPropertyChanged("FilterWidthTitle");
			}
		}
		
		private double _FilterWidthPublished = 100;
		public double FilterWidthPublished
		{
			get
			{
				return _FilterWidthPublished;
			}
			set
			{
				_FilterWidthPublished = value;
				onPropertyChanged("FilterWidthPublished");
			}
		}
		
		private double _FilterWidthDownloaded = 100;
		public double FilterWidthDownloaded
		{
			get
			{
				return _FilterWidthDownloaded;
			}
			set
			{
				_FilterWidthDownloaded = value;
				onPropertyChanged("FilterWidthDownloaded");
			}
			
		}
		#endregion
		
		
		
		#endregion
		
		#region TIMER
		[NonSerialized]
		private DispatcherTimer timedUpdate;
		
		public void StartTimer()
		{
			timedUpdate = new DispatcherTimer();
			timedUpdate.Tick += new EventHandler(timer_Tick);
			timedUpdate.Interval = new TimeSpan(0,UpdateInterval,0); // Hour, minute, second
			timedUpdate.Start();
			NextUpdate = DateTime.Now.AddMinutes(UpdateInterval);
		}
		
		public void ResetTimer()
		{
			timedUpdate.Stop();
			StartTimer();
		}
		
		private void timer_Tick(object sender, EventArgs e)
		{
			Update();
			NextUpdate = DateTime.Now.AddMinutes(UpdateInterval);
		}
		
		private DateTime _LastUpdate;
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
		
		[NonSerialized()]
		private DateTime _NextUpdate;
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
		
		
		
		#endregion
		
		#region NOTIFICATION
		private TaskbarItemProgressState _NotificationState = TaskbarItemProgressState.None;
		public TaskbarItemProgressState NotificationState
		{
			get
			{
				return _NotificationState;
			}
			set
			{
				_NotificationState = value;
				onPropertyChanged("NotificationState");
			}
		}
		
		public static bool WindowIsActive = true; // Is set by code-behind
		
		public bool IsNotifying
		{
			get
			{
				return NotificationState == TaskbarItemProgressState.Normal; // Not affecting loading
			}
		}
		
		public void NotifyLoading()
		{
			NotificationState = TaskbarItemProgressState.Indeterminate;
		}
		
		public void NotifyNow()
		{
			NotificationState = TaskbarItemProgressState.Normal;
			
			if (WindowIsActive)
			{
				NotifyTimedDeactive(2000);
			}
		}
		
		private async void NotifyTimedDeactive(int millisecs)
		{
			await System.Threading.Tasks.Task.Delay(millisecs);
			NotifyDeactivate();
		}
		
		public void NotifyDeactivate()
		{
			NotificationState = TaskbarItemProgressState.None;
		}
		
		
		
		#endregion
		
		#region RECOVERY
		public static readonly string BACKUP_DIR = "Backup";
		
		public string FiltersPath
		{
			get
			{
				return Path.Combine(BACKUP_DIR, "Filters.xml");
			}
		}
		
		public string FeedsPath
		{
			get
			{
				return Path.Combine(BACKUP_DIR, "Feeds.xml");
			}
		}
		
		public string ManuallyDownloadedTorrentsPath
		{
			get
			{
				return Path.Combine(BACKUP_DIR, "Manual_Downloads.xml");
			}
		}
		
		
		#region SAVE BACKUP
		public ICommand SaveBackup
		{
			get
			{
				return new RelayCommand(ExecuteSaveBackup, CanSaveBackup);
			}
		}
		
		public void ExecuteSaveBackup(object parameter)
		{
			if (!Directory.Exists(Path.GetDirectoryName(FeedsPath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(FeedsPath));
			}
			
			XmlWriterSettings xws = new XmlWriterSettings(){ Indent = true };
			DataContractSerializer dcs;
			
			// Feeds
			dcs = new DataContractSerializer(Feeds.GetType());
			using (XmlWriter xw = XmlWriter.Create(FeedsPath, xws))
			{
				dcs.WriteObject(xw, Feeds);
			}
			
			// Saves the index of SearchInFeed so that the feed can be loaded on Restore
			foreach (Filter f in Filters)
			{
				if (f.HasFeed)
				{
					f.SearchInFeedIndex = Feeds.IndexOf(f.SearchInFeed);
				}
				else
				{
					f.SearchInFeedIndex = -1;
				}
			}
			
			// Filters
			dcs = new DataContractSerializer(Filters.GetType());
			using (XmlWriter xw = XmlWriter.Create(FiltersPath, xws))
			{
				dcs.WriteObject(xw, Filters);
			}
			
			// Downloads
			dcs = new DataContractSerializer(ManuallyDownloadedTorrents.GetType());
			using (XmlWriter xw = XmlWriter.Create(ManuallyDownloadedTorrentsPath, xws))
			{
				dcs.WriteObject(xw, ManuallyDownloadedTorrents);
			}
		}
		
		public bool CanSaveBackup(object parameter)
		{
			return true;
		}
		
		#endregion
		
		#region RESTORE
		public ICommand Restore
		{
			get
			{
				return new RelayCommand(ExecuteRestore, CanRestore);
			}
		}
		
		public void ExecuteRestore(object parameter)
		{
			// TODO Implement backup of manual downloads
			
			using (FileStream fs = new FileStream(FeedsPath, FileMode.Open, FileAccess.Read))
			{
				DataContractSerializer dcs = new DataContractSerializer(Feeds.GetType());
				Feeds = dcs.ReadObject(fs) as ObservableCollection<Feed>;
			}
			
			using (FileStream fs = new FileStream(FiltersPath, FileMode.Open, FileAccess.Read))
			{
				DataContractSerializer dcs = new DataContractSerializer(Filters.GetType());
				Filters = dcs.ReadObject(fs) as ObservableCollection<Filter>;
			}
			
			using (FileStream fs = new FileStream(ManuallyDownloadedTorrentsPath, FileMode.Open, FileAccess.Read))
			{
				DataContractSerializer dcs = new DataContractSerializer(ManuallyDownloadedTorrents.GetType());
				ManuallyDownloadedTorrents = dcs.ReadObject(fs) as List<Torrent>;
			}
			
			// Uses saved indexes to load Feeds
			foreach (Filter f in Filters)
			{
				if (f.SearchInFeedIndex >= 0 && f.SearchInFeedIndex < Filters.Count)
				{
					f.SearchInFeed = Feeds[f.SearchInFeedIndex];
				}
			}
			
			SelectedFeed = Feeds[0];
			SelectedFilter = Filters[0];
			
			System.Windows.Application.Current.Shutdown();
		}
		
		public bool CanRestore(object parameter)
		{
			return File.Exists(FeedsPath) && File.Exists(FiltersPath) && File.Exists(ManuallyDownloadedTorrentsPath);
		}
		
		
		#endregion
		
		
		#endregion
		
		#region UPDATE INTERVAL
		private static readonly int[] UPDATE_INTERVALS = { 1, 2, 3, 5, 15, 30, 60 };
		private static readonly string[] UPDATE_OPTIONS = { "1 minute", "2 minutes", "3 minutes", "5 minutes", "15 minutes", "30 minutes", "1 hour" };

		public string[] UpdateOptions{ get{ return UPDATE_OPTIONS; } }
		public int UpdateInterval{ get{ return UPDATE_INTERVALS[SelectedUpdateOption]; } }
		
		
		private int _SelectedUpdateOption = 4;
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
		
		#endregion
		
		#region DOWNLOAD DIRECTORY
		public static readonly string DROP_FOLDER = @"Torrents\";
		
		private string _TorrentDropDirectory;
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
		
		#region CHOOSE DIRECTORY
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
				catch(ArgumentException)
				{
					dialog.SelectedPath = DROP_FOLDER;
				}
				
				System.Windows.Forms.DialogResult result = dialog.ShowDialog();
				
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					TorrentDropDirectory = Paths.MakeRelativeDirectory(dialog.SelectedPath);
				}
			}
		}
		
		public bool CanChooseDirectory(object parameter)
		{
			return true;
		}
		
		#endregion
		
		
		
		#endregion
		
		#region SAVE
		public void Save()
		{
			Save(FILENAME);
		}
		
		#region SAVE COMMAND
		public ICommand SaveCommand
		{
			get
			{
				return new RelayCommand(ExecuteSaveCommand, CanSaveCommand);
			}
		}
		
		public void ExecuteSaveCommand(object parameter)
		{
			Save();
		}
		
		public bool CanSaveCommand(object parameter)
		{
			return true;
		}
		
		#endregion
		
		
		
		#endregion
		
		#region FUNCTIONALITY
		public async void Update()
		{
			NotifyLoading();
			
			if (await UpdateAllFeeds())
			{
				LastUpdate = DateTime.Now;
				CountUpdates++;
				
				if (await FilterAll())
				{
					Save();
					NotifyNow();
				}
				else
				{
					NotifyDeactivate();
				}
			}
			else
			{
				NotifyDeactivate();
			}
		}
		
		public async Task<bool> UpdateAllFeeds()
		{
			// bool[] results = await Task.WhenAll(feeds.Select(feed => feed.Update()));
			
			List<Task<bool>> tasks = new List<Task<bool>>();
			
			foreach (Feed feed in Feeds)
			{
				tasks.Add(feed.Update());
			}

			await Task.WhenAll(tasks);
			
			return (from t in tasks where t.Result select t).Any();
		}
		
		public async Task<bool> FilterAll()
		{
			List<Task<bool>> tasks = new List<Task<bool>>();
			
			foreach (Filter filter in Filters)
			{
				if (filter.HasFeed && filter.Enabled)
				{
					tasks.Add(Filter(filter));
				}
			}

			await Task.WhenAll(tasks);
			
			return (from t in tasks where t.Result select t).Any();
		}
		
		public async Task<bool> Filter(Filter filter)
		{
			// REMEMBER I add torrent to downloadeditems prematurely so that ShouldDownload can filter
			
			List<Task<bool>> tasks = new List<Task<bool>>();
			
			foreach (Torrent torrent in filter.SearchInFeed.Items)
			{
				if (filter.ShouldDownload(torrent))
				{
					filter.DownloadedTorrents.Add(torrent);
					tasks.Add(DownloadTorrent(filter, torrent));
					
					if (filter.MatchOnce && !filter.IsTV)
					{
						break; // Get out of foreach
					}
				}
			}
			
			await Task.WhenAll(tasks);
			
			return (from t in tasks where t.Result select t).Any();
		}
		
		public async Task<bool> DownloadTorrent(Filter filter, Torrent torrent)
		{
			if (await torrent.Download(TorrentDropDirectory))
			{
				filter.HasDownloadedSinceHighest = true;
				
				LogDownload(filter, torrent);
				
				RefreshAllDownloads();
				
				onPropertyChanged("LoadHighestEpisode");
				onPropertyChanged("ResetFilter");
				onPropertyChanged("FilterSelected");
				
				
				if (filter.MatchOnce && !filter.IsTV)
				{
					filter.Enabled = false;
				}
				
				return true;
			}
			else
			{
				if (filter.DownloadedTorrents.Contains(torrent))
				{
					filter.DownloadedTorrents.Remove(torrent);
				}
				
				return false;
			}
		}
		
		public void LogDownload(Filter filter, Torrent item)
		{
			using(StreamWriter sw = File.AppendText(@"Matches.log"))
			{
				sw.WriteLine(String.Format("[{0}]", DateTime.Now.ToString("g")));
				sw.WriteLine(item.ToString());
				sw.WriteLine(filter.ToString() + Environment.NewLine);
			}
		}
		
		#endregion
	}
}
