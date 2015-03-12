using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
		public Feed(){}
		
		public static readonly String DEFAULT_TITLE = "New Feed";
		
		# region Properties
		
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
		
		private string _Title = DEFAULT_TITLE;
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
		
		# endregion
		
		# region Edit functionality
		
		public void FinalizeEdit()
		{
			URL = EditURL.Trim();
			Title = EditTitle.Trim();
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
		
		# endregion
		
		public void Update()
		{
			try
			{
				XmlDocument feed = new XmlDocument();
				
				feed.Load(URL);
				// FIXME When site is down, application freezes
				
				XmlNode channelNode = feed.SelectSingleNode("rss/channel");
				
				if (String.IsNullOrEmpty(Title) || Title == DEFAULT_TITLE)
				{
					Title = channelNode.SelectSingleNode("title").InnerText;
				}
				
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
			catch (FileNotFoundException fnfe)
			{
				Utils.PrintError("RSS feed not found.", this, fnfe);
			}
			catch (NullReferenceException nre)
			{
				Utils.PrintError("Not able to parse RSS feed.", this, nre);
			}
		}
		
		public override string ToString()
		{
			return String.Format("[Feed URL={0}, Title={1}]", _URL, _Title);
		}
		
	}
}
