
using System;
using System.Runtime.Serialization;
using System.Windows;

namespace tRSS.Model
{
	[DataContract()]
	public class MainViewSettings : WindowSettings
	{	
		private double _FilterSplitterPosition = 100;
		[DataMember()]
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
		[DataMember()]
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
		[DataMember()]
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
		[DataMember()]
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
		[DataMember()]
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
		[DataMember()]
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
		[DataMember()]
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
	}
}
