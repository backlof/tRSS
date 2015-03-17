
using System;
using System.Runtime.Serialization;
using System.Windows;

namespace tRSS.Model
{
	[Serializable()]
	public class MainViewSettings : WindowSettings
	{
		# region Datagrid column width
		
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
		
		# region Toolbar
		
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
		
		# endregion
	}
}
