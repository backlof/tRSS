using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using tRSS.Utilities;

namespace tRSS.Model
{
	[DataContract()]
	public class FeedItem : ObjectBase
	{
		private string _GUID;
		[DataMember()]
		public string GUID
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
		
		private string _URL;
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
		
		public async Task<bool> Download(string toLocation)
		{
			// http://stackoverflow.com/questions/18207730/how-to-make-ordinary-webrequest-async-and-awaitable
			
			if (await GetTorrent(toLocation))
			{
				Downloaded = DateTime.Now;
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public async Task<bool> GetTorrent(string toLocation)
		{
			// http://stackoverflow.com/questions/9857709/downloading-a-torrent-file-with-webclient-results-in-corrupt-file
			
			try
			{
				byte[] result;
				byte[] buffer = new byte[4096];

				WebRequest wr = WebRequest.Create(URL);
				wr.ContentType = "application/x-bittorrent";
				
				if (!Directory.Exists(toLocation))
				{
					Directory.CreateDirectory(toLocation);
				}
				
				using (WebResponse response = await wr.GetResponseAsync())
				{
					bool gzip = response.Headers["Content-Encoding"] == "gzip";
					
					var responseStream = gzip ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress) : response.GetResponseStream();

					using (MemoryStream memoryStream = new MemoryStream())
					{
						int count = 0;
						do
						{
							count = responseStream.Read(buffer, 0, buffer.Length);
							memoryStream.Write(buffer, 0, count);
						}
						while (count != 0);

						result = memoryStream.ToArray();
						
						using (BinaryWriter writer = new BinaryWriter(new FileStream(toLocation + Paths.CleanFileName(Title) + ".torrent", FileMode.Create)))
						{
							writer.Write(result);
						}
					}
				}
				
				return true;
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Can't download torrent");
				System.Diagnostics.Debug.WriteLine(e.ToString());
				System.Diagnostics.Debug.WriteLine(ToString());
				return false;
			}
		}
		
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
			return this._GUID == other._GUID && this._Title == other._Title && this.URL == other.URL;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * _GUID.GetHashCode();
				if (_Title != null)
					hashCode += 1000000009 * _Title.GetHashCode();
				if (URL != null)
					hashCode += 1000000021 * URL.GetHashCode();
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
