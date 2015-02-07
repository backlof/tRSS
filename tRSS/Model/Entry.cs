/*
 * Created by SharpDevelop.
 * User: Alexander
 * Date: 16.01.2015
 * Time: 11:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
			Link = item.SelectSingleNode("link").InnerText;
			GUID = Convert.ToInt32(item.SelectSingleNode("guid").InnerText);
			// RFC822 Format : Wed, 29 Oct 2008 14:14:48 +0000
			Published = DateTime.Parse(item.SelectSingleNode("pubDate").InnerText);
		}
		
		public int GUID { get; private set; }
		
		private string _Title;		
		public string Title {
			get { return _Title; }
			private set { _Title = value; onPropertyChanged("Title"); }
		}
		
		public string Link { get; private set; }
		
		public DateTime Published { get; private set; }
		
		public override string ToString()
		{
			return string.Format("[Item GUID={0}, Title={1}, Link={2}, Published={3}]", GUID, Title, Link, Published);
		}
	}
}
