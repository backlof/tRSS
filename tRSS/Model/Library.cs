using System;
using System.Collections.Generic;
using System.Reflection;
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
			
			for (int i = 0; i < 20; i++)
			{
				Feeds.Add(new Feed("TV Shows", "https://tracker.com"));
			}
						
			for (int i = 0; i < 20; i++)
			{
				Filter f = new Filter();
				Filters.Add(f);
				f.Active = true;
				f.Title = "Guardians of the Galaxy #" + i;
			}
		}
		
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
		
		public override string ToString()
		{
			return String.Format("[Library UpdateInMinutes={0}, TorrentDropLocation={1}]", _UpdateInMinutes, _TorrentDropLocation);
		}
		
		// ==========================================
		//   MAKE BUSINESS LOGIC FUNCTIONALITY HERE
		// ==========================================
		
		// Update Feeds -> Filter Items
	}
}
