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
		public Feed(string url)
		{
			//test constructor
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
				onPropertyChanged("Items");
			}
		}
		
		public override string ToString()
		{
			return string.Format("[Channel URL={0}, Title={1}, Description={2}, Items={3}]", URL, Title, Description, string.Join(Environment.NewLine, Items));
		}

		
		public void Update()
		{			
			if(!string.IsNullOrEmpty(URL))
			{
				XmlDocument feed = new XmlDocument();
				feed.Load("rss.xml"); //temporary offline testing
				
				XmlNode channel = feed.SelectSingleNode("rss/channel");
				XmlNodeList items = feed.SelectNodes("rss/channel/item");
				
				Title = channel.SelectSingleNode("title").InnerText;
				Description = channel.SelectSingleNode("description").InnerText;
				
				Items = new List<FeedItem>();
				foreach (XmlNode item in items)
				{
					Items.Add(new FeedItem(item));
				}
				onPropertyChanged("Entries");
			}
		}
	}
}
