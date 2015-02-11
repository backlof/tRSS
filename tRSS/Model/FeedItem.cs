using System;
using System.Xml;
using System.Xml.Serialization;
using tRSS.Utilities;

namespace tRSS.Model
{
	/// <summary>
	/// Description of Entry.
	/// </summary>
	/// 
	public class FeedItem : INotifyBase
	{
		public FeedItem(XmlNode item)
		{
			Title = item.SelectSingleNode("title").InnerText;
			TorrentLocation = item.SelectSingleNode("link").InnerText;
			GUID = Convert.ToInt32(item.SelectSingleNode("guid").InnerText);
			// RFC822 Format : Wed, 29 Oct 2008 14:14:48 +0000
			Published = DateTime.Parse(item.SelectSingleNode("pubDate").InnerText);
		}
		
		private int _GUID;
		public int GUID
		{
			get
			{
				return _GUID;
			}
			set
			{
				_GUID = value;
				onPropertyChanged("GUID");
			}
		}
		
		private string _Title;
		public string Title
		{
			get
			{
				return _Title;
			}
			set
			{
				_Title = value;
				onPropertyChanged("Title");
			}
		}
		
		private string _TorrentLocation;
		public string TorrentLocation
		{
			get
			{
				return _TorrentLocation;
			}
			set
			{
				_TorrentLocation = value;
				onPropertyChanged("TorrentLocation");
			}
		}
		
		private DateTime _Published;
		public DateTime Published
		{
			get
			{
				return _Published;
			}
			set
			{
				_Published = value;
				onPropertyChanged("Published");
			}
		}
		
		private bool _Filtered = false;
		public bool Filtered
		{
			get
			{
				return _Filtered;
			}
			set
			{
				_Filtered = value;
				onPropertyChanged("Filtered");
			}
		}
		
		private bool _Downloaded = false;
		public bool Downloaded
		{
			get
			{
				return _Downloaded;
			}
			set
			{
				_Downloaded = value;
				onPropertyChanged("Downloaded");
			}
		}
		
		public override string ToString()
		{
			return String.Format("[Item GUID={0}, Title={1}, Link={2}, Published={3}]", GUID, Title, TorrentLocation, Published);
		}
	}
}
