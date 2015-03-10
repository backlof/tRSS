using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using tRSS.Model;
using tRSS.Utilities;

namespace tRSS.Model
{
	[DataContract()]
	public class Feed : ObjectBase
	{
		public Feed()
		{
		}
		
		// HACK Temporary testing constructor
		public Feed(string title, string url)
		{
			Title = title;
			URL = url;
			Update();
		}
		
		private string _URL = "";
		[DataMember()]
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
		
		private string _EditURL = "";
		[IgnoreDataMember()]
		public string EditURL
		{
			get
			{
				return _EditURL;
			}
			set
			{
				_EditURL = value;
				onPropertyChanged("EditURL");
			}
		}
		
		private string _Title = "New Feed";
		[DataMember()]
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
		
		private string _EditTitle = "";
		[IgnoreDataMember()]
		public string EditTitle
		{
			get
			{
				return _EditTitle;
			}
			set
			{
				_EditTitle = value;
				onPropertyChanged("EditTitle");
			}
		}
		
		private string _Description;
		[DataMember()]
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
		
		private ObservableCollection<FeedItem> _Items = new ObservableCollection<FeedItem>();
		[IgnoreDataMember()]
		public ObservableCollection<FeedItem> Items
		{
			get
			{
				return _Items;
			}
			set
			{
				_Items = value;
			}
		}
		
		public void FinalizeEdit()
		{
			URL = EditURL;
			Title = EditTitle;
		}
		
		public void Update()
		{
			try
			{
				XmlDocument feed = new XmlDocument();
				
				feed.Load(URL);
				
				XmlNode channelNode = feed.SelectSingleNode("rss/channel");
				Description = channelNode.SelectSingleNode("description").InnerText;
				
				XmlNodeList itemNodes = feed.SelectNodes("rss/channel/item");
				
				Items = new ObservableCollection<FeedItem>();
				
				foreach (XmlNode itemNode in itemNodes)
				{
					// RFC822 Format : Wed, 29 Oct 2008 14:14:48 +0000
					
					FeedItem item = new FeedItem();
					item.Title = itemNode.SelectSingleNode("title").InnerText;
					item.GUID = itemNode.SelectSingleNode("guid").InnerText;
					item.Published = DateTime.Parse(itemNode.SelectSingleNode("pubDate").InnerText);
					item.URL = itemNode.SelectSingleNode("enclosure") != null
						? itemNode.SelectSingleNode("enclosure").Attributes["url"].InnerText
						: itemNode.SelectSingleNode("link").InnerText;
					
					Items.Add(item);
				}
				onPropertyChanged("Items");
			}
			catch (ArgumentNullException ane)
			{
				System.Diagnostics.Debug.WriteLine("Invalid feed URL");
				System.Diagnostics.Debug.WriteLine(ToString());
				System.Diagnostics.Debug.WriteLine(ane.ToString());
			}
			catch (ArgumentException ae)
			{
				System.Diagnostics.Debug.WriteLine("Invalid feed URL");
				System.Diagnostics.Debug.WriteLine(ToString());
				System.Diagnostics.Debug.WriteLine(ae.ToString());
			}
		}
		
		public override string ToString()
		{
			return String.Format("[Feed URL={0}, Title={1}, Description={2}]", _URL, _Title, _Description);
		}
		
	}
}
