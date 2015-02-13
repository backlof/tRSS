using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using tRSS.Utilities;

namespace tRSS.Model
{
	/// <summary>
	/// Description of Entry.
	/// </summary>
	[DataContract()]
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
		[DataMember()]
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
		
		private string _TorrentLocation;
		[DataMember()]
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
		[DataMember()]
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
		
		private DateTime _Downloaded;
		[DataMember()]
		public DateTime Downloaded
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
		
		[IgnoreDataMember()]
		public bool IsTV
		{
			get
			{
				return EpisodeMatch.Success;
			}
		}
		
		private Match _EpisodeMatch;
		[IgnoreDataMember()]
		public Match EpisodeMatch
		{
			get
			{
				if(_EpisodeMatch == null)
				{
					Regex regExp = new Regex(@"S(?<season>\d{1,2})E(?<episode>\d{1,2})", RegexOptions.IgnoreCase);
					_EpisodeMatch = regExp.Match(Title);
				}
				return _EpisodeMatch;
			}
		}
		
		private int _Season = 0;
		[IgnoreDataMember()]
		public int Season
		{
			get
			{
				if (_Season == 0)
				{
					Match m = EpisodeMatch;
					string season = m.Groups["season"].Value;
					_Season = Int32.Parse(season);
				}
				return _Season;
			}
		}
		
		private int _Episode = 0;
		[IgnoreDataMember()]
		public int Episode
		{
			get
			{
				if (_Episode == 0)
				{
					Match m = EpisodeMatch;
					string episode = m.Groups["episode"].Value;
					_Episode = Int32.Parse(episode);
				}
				return _Episode;
			}
		}
		
		public void Download()
		{
			// TODO Download torrent here
			Downloaded = DateTime.Now;
		}
		
		// FIXME Don't know if references in Filters to downloaded items survive after updating Feeds
		
		public override string ToString()
		{
			return string.Format("[FeedItem GUID={0}, Title={1}, Published={2}, Season={3}, Episode={4}]", _GUID, _Title, _Published, _Season, _Episode);
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			FeedItem other = obj as FeedItem;
			if (other == null)
				return false;
			return this._GUID == other._GUID && this._Title == other._Title && this._TorrentLocation == other._TorrentLocation;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * _GUID.GetHashCode();
				if (_Title != null)
					hashCode += 1000000009 * _Title.GetHashCode();
				if (_TorrentLocation != null)
					hashCode += 1000000021 * _TorrentLocation.GetHashCode();
				hashCode += 1000000033 * _Published.GetHashCode();
				if (_EpisodeMatch != null)
					hashCode += 1000000087 * _EpisodeMatch.GetHashCode();
				hashCode += 1000000093 * _Season.GetHashCode();
				hashCode += 1000000097 * _Episode.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(FeedItem lhs, FeedItem rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(FeedItem lhs, FeedItem rhs)
		{
			return !(lhs == rhs);
		}
		#endregion
	}
}
