using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using tRSS.Model;
using tRSS.Utilities;

namespace tRSS.Model
{
	/// <summary>
	/// Description of Channel.
	/// </summary>
	/// 
	public class Feed : INotifyBase
	{
		public Feed()
		{
			Update();
		}
		
		// HACK Temporary testing constructor
		public Feed(string title, string url)
		{
			Title = title;
			URL = url;
			Update();
		}
		
		private string _URL;
		public string URL
		{
			get
			{
				return _URL;
			}
			set
			{
				_URL = value;
				onPropertyChanged("URL");
			}
		}
		
		private string _Title = "New Feed";
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
		
		private string _Description;
		public string Description
		{
			get
			{
				return _Description;
			}
			private set
			{
				_Description = value;
				onPropertyChanged("Description");
			}
		}
		
		private List<FeedItem> _Items = new List<FeedItem>();
		public List<FeedItem> Items
		{
			get
			{
				return _Items;
			}
			private set
			{
				_Items = value;
				//onPropertyChanged("Items");
			}
		}
		
		public void Update()
		{
			// UNDONE Check URL before updating
			
			XmlDocument feed = new XmlDocument();
			feed.Load("rss.xml"); //temporary offline testing
			
			XmlNode channel = feed.SelectSingleNode("rss/channel");
			XmlNodeList items = feed.SelectNodes("rss/channel/item");
			
			// Temporary
			if(String.IsNullOrEmpty(Title))
			{
				Title = channel.SelectSingleNode("title").InnerText;
			}
			
			Description = channel.SelectSingleNode("description").InnerText;
			
			Items = new List<FeedItem>();
			foreach (XmlNode item in items)
			{
				Items.Add(new FeedItem(item));
			}
			onPropertyChanged("Items");
		}
		
		public override string ToString()
		{
			return String.Format("[Feed URL={0}, Title={1}, Description={2}]", _URL, _Title, _Description);
		}
		
	}
}
