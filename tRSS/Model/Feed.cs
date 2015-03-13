using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using tRSS.Model;
using tRSS.Utilities;
using System.Threading.Tasks;

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
		
		public async Task<bool> Update()
		{
			try
			{
				WebRequest wr = WebRequest.Create(URL);
				
				using (WebResponse response = await wr.GetResponseAsync())
				{					
					XmlDocument feed = new XmlDocument();
					feed.Load(response.GetResponseStream());
					
					XmlNode channelNode = feed.SelectSingleNode("rss/channel");
					
					if (String.IsNullOrEmpty(Title) || Title == DEFAULT_TITLE)
					{
						Title = channelNode.SelectSingleNode("title").InnerText.Trim();
					}
					
					XmlNodeList itemNodes = feed.SelectNodes("rss/channel/item");
					
					Items = new ObservableCollection<FeedItem>();
					
					foreach (XmlNode itemNode in itemNodes)
					{
						// RFC822 Format : Wed, 29 Oct 2008 14:14:48 +0000
						
						FeedItem item = new FeedItem();
						item.Title = itemNode.SelectSingleNode("title").InnerText.Trim();
						item.GUID = itemNode.SelectSingleNode("guid").InnerText.Trim();
						item.Published = DateTime.Parse(itemNode.SelectSingleNode("pubDate").InnerText.Trim());
						item.URL = itemNode.SelectSingleNode("enclosure") != null
							? itemNode.SelectSingleNode("enclosure").Attributes["url"].InnerText.Trim()
							: itemNode.SelectSingleNode("link").InnerText.Trim();
						
						Items.Add(item);
					}
					
					onPropertyChanged("Items");
					return true;
				}
			}
			catch (FileNotFoundException fnfe)
			{
				Utils.PrintError("RSS feed not found.", this, fnfe);
				return false;
			}
			catch (NullReferenceException nre)
			{
				Utils.PrintError("Not able to parse RSS feed.", this, nre);
				return false;
			}
			catch (WebException we)
			{
				Utils.PrintError("Connection timed out while loading feed.", this, we);
				return false;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[Feed URL={0}, Title={1}]", _URL, _Title);
		}
		
	}
}
