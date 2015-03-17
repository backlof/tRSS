using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using tRSS.Utilities;
using System.Runtime.Serialization;
using tRSS.View;

namespace tRSS.Model
{
	//[DataContract()]
	[Serializable()]
	public class Library : ObjectBase
	{
		public Library()
		{
			Feed tvFeed = new Feed(){ Title = "TV Shows", URL = "https://kickass.to/tv/?rss=1"};
			Feeds.Add(tvFeed);
			Feeds.Add(new Feed(){ Title = "Movies", URL = "https://kickass.to/movies/?rss=1" });
			Feeds.Add(new Feed(){ Title = "Music", URL = "https://kickass.to/music/?rss=1" });
			
			Filter tvFilter = new Filter(){ IsActive = false, Title="All TV Shows", SearchInFeed = tvFeed, TitleFilter = "*", IgnoreCaps = true, Include = "720p;H.264", Exclude = "1080p;HDTV;", IsTV = false };
			Filters.Add(tvFilter);
			
			SelectedFeed = tvFeed;
			SelectedFilter = tvFilter;
			
			TorrentDropDirectory = FOLDER;
			if(!Directory.Exists(Path.GetDirectoryName(TorrentDropDirectory)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(TorrentDropDirectory));
			}
		}
		
		public static readonly string FOLDER = @"Torrents\";
		
		private int _FeedIdCount = 0;
		public int FeedIdCount
		{
			get
			{
				_FeedIdCount++;
				return _FeedIdCount;
			}
			set
			{
				_FeedIdCount = value;
				onPropertyChanged("FeedIdCount");
			}
		}
		
		# region Timer
		
		[NonSerializedAttribute()]
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
		
		# endregion
		
		# region Properties
		
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
		
		# endregion
		
		# region Selected
				
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
		
		# endregion
		
		
		
		# region Update interval
		
		private static readonly int[] UPDATE_INTERVALS = { 1, 2, 3, 5, 15, 30, 60 };
		private static readonly string[] UPDATE_OPTIONS = { "1 minute", "2 minutes", "3 minutes", "5 minutes", "15 minutes", "30 minutes", "1 hour" };

		public string[] UpdateOptions
		{
			get
			{
				return UPDATE_OPTIONS;
			}
		}
		
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
		}
		
		public bool CanDeleteFeed(object parameter)
		{
			return Feeds.Count > 1;
		}
		
		# endregion
		
		# region Reset filter
		
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
			onPropertyChanged("DownloadedItems");
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
			EditFeed edit = new EditFeed(f);
			edit.Show();
		}
		
		public bool CanNewFeed(object parameter)
		{
			return true;
		}
		
		# endregion
		
		# region Edit feed
		
		public ICommand EditFeed
		{
			get
			{
				return new RelayCommand(ExecuteEditFeed, CanEditFeed);
			}
		}
		
		public void ExecuteEditFeed(object parameter)
		{
			EditFeed edit = new EditFeed(SelectedFeed);
			edit.Show();
		}
		
		public bool CanEditFeed(object parameter)
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
			ResetTimer();
		}
		
		public bool CanRefresh(object parameter)
		{
			return true;
		}
		
		
		# endregion
		
		# region Remove download
		
		public ICommand RemoveDownload
		{
			get
			{
				return new RelayCommand(ExecuteRemoveDownload, CanRemoveDownload);
			}
		}
		
		public void ExecuteRemoveDownload(object parameter)
		{
			foreach (Filter filter in Filters)
			{
				if (filter.DownloadedItems.Contains(SelectedDownload))
				{
					filter.DownloadedItems.Remove(SelectedDownload);
					onPropertyChanged("DownloadedItems");
				}
			}
		}
		
		public bool CanRemoveDownload(object parameter)
		{
			return SelectedDownload != null;
		}
		
		# endregion
		
		
		# region Filter selected
		
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
		
		# endregion
		
		# endregion
		
		public async void Update()
		{
			// Refresh
			foreach (Feed feed in Feeds)
			{
				if (!(String.IsNullOrEmpty(feed.URL)))
				{
					bool x = await feed.Refresh();
				}
			}
			
			// Filter
			foreach (Filter filter in Filters)
			{
				if (filter.IsActive && filter.HasFeed)
				{
					FilterFeed(filter);
				}
			}
			
			LastUpdate = DateTime.Now;
		}
		
		public async void FilterFeed(Filter filter)
		{
			foreach (FeedItem item in filter.SearchInFeed.Items)
			{
				if (filter.ShouldDownload(item))
				{
					if (await item.Download(TorrentDropDirectory))
					{
						filter.DownloadedItems.Add(item);
						LastMatch = item.Title;
						
						if (filter.MatchOnce && !filter.IsTV)
						{
							filter.IsActive = false;
						}
						
						onPropertyChanged("DownloadedItems");
						onPropertyChanged("CanResetFilter");
					}
				}
			}
		}
	}
}
