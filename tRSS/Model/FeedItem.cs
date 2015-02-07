/*
 * Created by SharpDevelop.
 * User: Alexander
 * Date: 16.01.2015
 * Time: 11:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		
		public string URL { get; set; }
		
		public string Title { get; private set; }
		
		public string Description { get; private set; }
		
		
		List<FeedItem> _Items = new List<FeedItem>();
		public List<FeedItem> Items {
			get { return _Items; }
			private set { _Items = value; onPropertyChanged("Items"); }
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
