using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
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
		public MainViewModel()
		{
			TorrentDropDirectory = FOLDER;
			if(!Directory.Exists(Path.GetDirectoryName(TorrentDropDirectory)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(TorrentDropDirectory));
			}
			
			Feed tvFeed = new Feed(){ Title = "TV Shows", URL = "https://kickass.to/tv/?rss=1"};
			Feeds.Add(tvFeed);
			Feeds.Add(new Feed(){ Title = "Movies", URL = "https://kickass.to/movies/?rss=1" });
			Feeds.Add(new Feed(){ Title = "Music", URL = "https://kickass.to/music/?rss=1" });
			
			Filter tvFilter = new Filter(){ Enabled = false, Title="All TV Shows", SearchInFeed = tvFeed, TitleFilter = "*", IgnoreCaps = true, Include = "720p;H.264", Exclude = "1080p;HDTV;", IsTV = false };
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
			SelectedFilter.DownloadedItems = new List<FeedItem>();
			onPropertyChanged("DownloadedItems");
		}
		
		public bool CanResetFilter(object parameter)
		{
			if (SelectedFilter == null)
			{
				return false;
			}
			else
			{
				return SelectedFilter.DownloadedItems.Count > 0;
			}
		}
		
		#endregion
		
		#region DELETE FILTER
		
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
			onPropertyChanged("DownloadedItems");
		}
		
		public bool CanDeleteFilter(object parameter)
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
		}
		
		public bool CanNewFilter(object parameter)
		{
			return true;
		}
		
		#endregion
		
		#region FILTER FILTER
		
		public ICommand FilterSelected
		{
			get
			{
				return new RelayCommand(ExecuteFilterSelected, CanFilterSelected);
			}
		}
		
		public void ExecuteFilterSelected(object parameter)
		{
			FilterFeed(SelectedFilter);
		}
		
		public bool CanFilterSelected(object parameter)
		{
			if (SelectedFilter == null)
			{
				return false;
			}
			else
			{
				return SelectedFilter.HasFeed;
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
		
		#region DELETE FEED
		
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
		
		#region REFRESH FEEDS
		
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
			ResetTimer();
		}
		
		public bool CanRefresh(object parameter)
		{
			return true;
		}
		
		
		#endregion
		
		#endregion
		
		#region DOWNLOADS
		
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
		
		[NonSerialized()]
		private FeedItem _SelectedDownload;
		public FeedItem SelectedDownload
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
		
		private int _RefreshBand = 0;
		public int RefreshBand
		{
			get
			{
				return _RefreshBand;
			}
			set
			{
				_RefreshBand = value;
				onPropertyChanged("RefreshBand");
			}
		}
		
		private int _RefreshBandIndex = 3;
		public int RefreshBandIndex
		{
			get
			{
				return _RefreshBandIndex;
			}
			set
			{
				_RefreshBandIndex = value;
				onPropertyChanged("RefreshBandIndex");
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
		private DispatcherTimer timedRefresh;
		
		public void StartTimer()
		{
			timedRefresh = new DispatcherTimer();
			timedRefresh.Tick += new EventHandler(timer_Tick);
			timedRefresh.Interval = new TimeSpan(0,UpdateInterval,0); // Hour, minute, second
			timedRefresh.Start();
			NextUpdate = DateTime.Now.AddMinutes(UpdateInterval);
		}
		
		public void ResetTimer()
		{
			timedRefresh.Stop();
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
		
		private string _LastMatch = "None";
		public string LastMatch
		{
			get
			{
				return _LastMatch;
			}
			set
			{
				_LastMatch = value;
				onPropertyChanged("LastMatch");
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
				return NotificationState != TaskbarItemProgressState.None;
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
			await System.Threading.Tasks.Task.Delay(1000);
			NotifyDeactivate();
		}
		
		public void NotifyDeactivate()
		{
			NotificationState = TaskbarItemProgressState.None;
		}
		
		#endregion
		
		#region SAVE RECOVERY
		
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
			
			DataContractSerializer dcs = new DataContractSerializer(Feeds.GetType());
			using (XmlWriter xw = XmlWriter.Create(FeedsPath, xws))
			{
				dcs.WriteObject(xw, Feeds);
			}
			
			dcs = new DataContractSerializer(Filters.GetType());
			using (XmlWriter xw = XmlWriter.Create(FiltersPath, xws))
			{
				dcs.WriteObject(xw, Filters);
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
			SelectedFilter = Filters[0];
			SelectedFeed = Feeds[0];
			
			System.Windows.Application.Current.Shutdown();
		}
		
		public bool CanRestore(object parameter)
		{
			return File.Exists(FeedsPath) && File.Exists(FiltersPath);
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
		
		public static readonly string FOLDER = @"Torrents\";
		
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
					dialog.SelectedPath = FOLDER;
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
		
		
		public async void Update()
		{
			NotifyLoading();
			
			// Refresh
			foreach (Feed feed in Feeds)
			{
				if (!(String.IsNullOrEmpty(feed.URL)))
				{
					bool x = await feed.Refresh();
				}
			}
			
			NotifyDeactivate();
			
			// Filter
			foreach (Filter filter in Filters)
			{
				if (filter.Enabled)
				{
					FilterFeed(filter);
				}
			}
			
			LastUpdate = DateTime.Now;
		}
		
		public async void FilterFeed(Filter filter)
		{
			if (filter.HasFeed)
			{
				foreach (FeedItem item in filter.SearchInFeed.Items)
				{
					if (filter.ShouldDownload(item))
					{
						if (await item.Download(TorrentDropDirectory))
						{
							filter.DownloadedItems.Add(item);
							
							LastMatch = item.Title;
							
							LogDownload(filter, item);
							NotifyNow();
							
							if (filter.MatchOnce && !filter.IsTV)
							{
								filter.Enabled = false;
							}
							
							onPropertyChanged("DownloadedItems");
							onPropertyChanged("CanResetFilter");
						}
					}
				}
			}
		}
		
		public void LogDownload(Filter filter, FeedItem item)
		{
			using(StreamWriter sw = File.AppendText(@"Download.log"))
			{
				sw.WriteLine(String.Format("[{0}]", DateTime.Now.ToString("g")));
				sw.WriteLine(item.ToString());
				sw.WriteLine(filter.ToString() + Environment.NewLine);
			}
		}
		
		
	}
}
