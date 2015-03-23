using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using tRSS.Utilities;
using System.Runtime.Serialization;
using System.Xml;

namespace tRSS.Model
{
	[DataContract(Name="Filter")]
	[Serializable()]
	public class Filter : ObjectBase
	{
		public Filter(){}
		
		// Fields have to be static unless they're stored in XML
		// They're only initialized when the class object is
		// Not during deserialization
		
		#region PROPERTIES
		
		private string _Title = "New Filter";
		[DataMember(Name="Title", IsRequired=false)]
		public string Title
		{
			get
			{
				return _Title;
			}
			set
			{
				_Title = value.Trim();
				onPropertyChanged("Title");
			}
		}
		
		private bool _Enabled = false;
		[DataMember(Name="Enabled", IsRequired=false)]
		public bool Enabled
		{
			get
			{
				return _Enabled;
			}
			set
			{
				_Enabled = value;
				onPropertyChanged("Enabled");
			}
		}
		
		private bool _IgnoreCaps = true;
		[DataMember(Name="IgnoreCaps", IsRequired=false)]
		public bool IgnoreCaps
		{
			get
			{
				return _IgnoreCaps;
			}
			set
			{
				_IgnoreCaps = value;
				onPropertyChanged("IgnoreCaps");
			}
		}
		
		private bool _MatchOnce = true;
		[DataMember(Name="MatchOnce", IsRequired=false)]
		public bool MatchOnce
		{
			get
			{
				return _MatchOnce;
			}
			set
			{
				_MatchOnce = value;
				onPropertyChanged("MatchOnce");
			}
		}
		
		private Feed _SearchInFeed = null;
		[IgnoreDataMember]
		public Feed SearchInFeed
		{
			get
			{
				return _SearchInFeed;
			}
			set
			{
				_SearchInFeed = value;
				onPropertyChanged("SearchInFeed");
			}
		}
		
		[NonSerialized]
		private int _SearchInFeedIndex;
		[DataMember(Name="Feed", IsRequired=false)]
		public int SearchInFeedIndex
		{
			get
			{
				return _SearchInFeedIndex;
			}
			set
			{
				_SearchInFeedIndex = value;
				//onPropertyChanged("SearchInFeedIndex"); //Never used
			}
		}
		
		[IgnoreDataMember]
		public bool HasFeed
		{
			get
			{
				return _SearchInFeed != null;
			}
		}
		
		private bool _IsTV = false;
		[DataMember(Name="TV", IsRequired=false)]
		public bool IsTV
		{
			get
			{
				return _IsTV;
			}
			set
			{
				_IsTV = value;
				onPropertyChanged("IsTV");
			}
		}
		
		private int _Season = 1;
		[DataMember(Name="Season", IsRequired=false)]
		public int Season
		{
			get
			{
				return _Season;
			}
			set
			{
				_Season = value;
				onPropertyChanged("Season");
			}
		}
		
		private int _Episode = 1;
		[DataMember(Name="Episode", IsRequired=false)]
		public int Episode
		{
			get
			{
				return _Episode;
			}
			set
			{
				_Episode = value;
				onPropertyChanged("Episode");
			}
		}
		
		private List<Torrent> _DownloadedTorrents = new List<Torrent>();
		[DataMember(Name="Downloads", IsRequired=false)]
		public List<Torrent> DownloadedTorrents
		{
			get
			{
				return _DownloadedTorrents;
			}
			set
			{
				_DownloadedTorrents = value;
				onPropertyChanged("DownloadedTorrents");
			}
		}
		
		#endregion
		
		#region INCLUDE & EXCLUDE
		
		private static readonly char[] INCLUDE_INTERPRET_SEPARATORS = {';', '|', '+'};
		
		[IgnoreDataMember]
		public List<string> IncludeList
		{
			get
			{
				string include = IgnoreCaps ? Include.ToLower() : Include;
				return new List<string>(include.Split(INCLUDE_INTERPRET_SEPARATORS, StringSplitOptions.RemoveEmptyEntries));
			}
		}
		
		private string _Include;
		[DataMember(Name="Include", IsRequired=false)]
		public string Include
		{
			get
			{
				return _Include;
			}
			set
			{
				_Include = Utils.RemoveDiacritics(value.Trim());
				onPropertyChanged("Include");
			}
		}
		
		[IgnoreDataMember]
		public List<string> ExcludeList
		{
			get
			{
				string exclude = IgnoreCaps ? Exclude.ToLower() : Exclude;
				return new List<string>(exclude.Split(INCLUDE_INTERPRET_SEPARATORS, StringSplitOptions.RemoveEmptyEntries));
			}
		}
		
		private string _Exclude = "";
		[DataMember(Name="Exclude", IsRequired=false)]
		public string Exclude
		{
			get
			{
				return _Exclude;
			}
			set
			{
				_Exclude = Utils.RemoveDiacritics(value.Trim());
				onPropertyChanged("Exclude");
			}
		}
		
		#endregion
		
		#region REGEX FILTER
		
		// REGEX OPERATORS *.$^{[(|)]}+?\
		private static readonly string REJECT_CHARS =  @"$^{[(|)]}+\";
		
		private string _TitleFilter = "";
		[DataMember(Name="Filter", IsRequired=false)]
		public string TitleFilter
		{
			get
			{
				return _TitleFilter;
			}
			set
			{
				// Replace all Regex Operators - except the ones I translate
				_TitleFilter = Utils.ReplaceCharacters(value.Trim(), REJECT_CHARS, "");
				onPropertyChanged("TitleFilter");
				RegexPattern = TitleFilter;
			}
		}
		
		private string _RegexPattern = "";
		[IgnoreDataMember]
		public string RegexPattern
		{
			get
			{
				return _RegexPattern;
			}
			set
			{
				StringBuilder sb = new StringBuilder();
				
				// * = Wildcard
				// . = Whitespaces
				// ? = Any character 0 or 1 times
				
				int length = value.Length;
				
				if(length > 0)
				{
					// No wildcard in beginning, then "Begins with"
					if(value[0] != '*')
					{
						sb.Append(@"^");
					}
					
					foreach(char letter in value)
					{
						if (letter == '*')
						{
							sb.Append(@".*");
						}
						else if (letter == '?')
						{
							sb.Append(".?");
						}
						else if (letter == '.')
						{
							sb.Append(@"[\s._-]");
						}
						else
						{
							sb.Append(letter);
						}
					}
				}
				else
				{
					sb.Append(".^"); // No matches
				}
				
				_RegexPattern = @sb.ToString();
				onPropertyChanged("RegexPattern");
			}
		}
		
		#endregion
		
		#region HIGHEST
		
		private bool _HasDownloadedSinceHighest = false;
		[IgnoreDataMember]
		public bool HasDownloadedSinceHighest
		{
			get
			{
				return _HasDownloadedSinceHighest;
			}
			set
			{
				_HasDownloadedSinceHighest = value;
				onPropertyChanged("HasDownloadedSinceHighest");
			}
		}
		
		public void LoadHighest()
		{
			if (DownloadedTorrents.Count > 0)
			{
				Torrent highest = DownloadedTorrents[0];
				
				
				foreach (Torrent downloaded in DownloadedTorrents)
				{
					if (downloaded.IsTV)
					{
						if (downloaded.Season > highest.Season || (downloaded.Season == highest.Season && downloaded.Episode > highest.Episode))
						{
							highest = downloaded;
						}
					}
				}
				
				Season = highest.Season;
				Episode = highest.Episode + 1;
			}
			
			HasDownloadedSinceHighest = false;
		}
		
		#endregion
		
		public bool ShouldDownload(Torrent item)
		{
			if (Enabled)
			{
				if (!DownloadedTorrents.Contains(item))
				{
					string title = Utils.RemoveDiacritics(IgnoreCaps ? item.Title.ToLower() : item.Title);
					RegexOptions option = IgnoreCaps ? RegexOptions.IgnoreCase : RegexOptions.None;
					
					if (Regex.IsMatch(title, RegexPattern, option))
					{
						if (IncludeList.All(title.Contains))
						{
							if (!ExcludeList.Any(title.Contains))
							{
								if (IsTV)
								{
									if (item.IsTV)
									{
										if(IsEpisodeToDownload(item))
										{
											return true;
										}
									}
								}
								else
								{
									return true;
								}
							}
						}
					}
				}
			}
			
			return false;
		}
		
		public bool IsEpisodeToDownload(Torrent item)
		{
			if (item.Season > Season || (item.Season == Season && item.Episode >= Episode))
			{
				if (MatchOnce)
				{
					foreach (Torrent downloaded in DownloadedTorrents)
					{
						if (downloaded.IsTV) // In case filter wasn't always TV
						{
							if (item.Season == downloaded.Season && item.Episode == downloaded.Episode)
							{
								return false; // If same episode is downloaded
							}
						}
					}
					
					return true;
				}
				else { return true; }
			}
			else { return false; }
		}
		
		public override string ToString()
		{
			return string.Format("[Filter Title={0}, Enabled={1}, IgnoreCaps={2}, MatchOnce={3}, IsTV={4}, Season={5}, Include={6}, Exclude={7}, TitleFilter={8}, Episode={9}]", _Title, _Enabled, _IgnoreCaps, _MatchOnce, _IsTV, _Season, _Include, _Exclude, _TitleFilter, _Episode);
		}
	}
}
