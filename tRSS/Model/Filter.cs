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

	[Serializable()]
	public class Filter : ObjectBase
	{
		public Filter(){}
		
		// Fields have to be static unless they're stored in XML
		// They're only initialized when the class object is
		// Not during deserialization
		
		# region Properties
		
		private string _Title = "New Filter";
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
		
		private bool _IsActive = false;
		public bool IsActive
		{
			get
			{
				return _IsActive;
			}
			set
			{
				_IsActive = value;
				onPropertyChanged("IsActive");
			}
		}
		
		private bool _IgnoreCaps = true;
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
		
		private Feed _SearchInFeed;
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
		
		public bool HasFeed
		{
			get
			{
				return _SearchInFeed != null;
			}
		}
		
		private bool _IsTV = false;
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
		
		private List<FeedItem> _DownloadedItems = new List<FeedItem>();
		public List<FeedItem> DownloadedItems
		{
			get
			{
				return _DownloadedItems;
			}
			set
			{
				_DownloadedItems = value;
				onPropertyChanged("DownloadedItems");
			}
		}
		
		# endregion
		
		# region Include / Exclude
		
		private static readonly char[] INCLUDE_INTERPRET_SEPARATORS = {';', '|', '+'};
		
		public List<string> IncludeList
		{
			get
			{
				string include = IgnoreCaps ? Include.ToLower() : Include;
				return new List<string>(include.Split(INCLUDE_INTERPRET_SEPARATORS, StringSplitOptions.RemoveEmptyEntries));
			}
		}
		
		private string _Include;
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
		

		public List<string> ExcludeList
		{
			get
			{
				string exclude = IgnoreCaps ? Exclude.ToLower() : Exclude;
				return new List<string>(exclude.Split(INCLUDE_INTERPRET_SEPARATORS, StringSplitOptions.RemoveEmptyEntries));
			}
		}
		
		private string _Exclude = "";
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
		
		# endregion
		
		# region Regex filter
		
		// REGEX OPERATORS *.$^{[(|)]}+?\
		private static readonly string REJECT_CHARS =  @"$^{[(|)]}+\";
		
		private string _TitleFilter = "";
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
		
		// TODO Make function that sets season and episode as highest or latest
		
		private string _RegexPattern = "";
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
		
		# endregion
		
		# region Filter functionality
		
		public bool ShouldDownload(FeedItem item)
		{
			if (!DownloadedItems.Contains(item))
			{
				if (IsActive)
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
		
		public bool IsEpisodeToDownload(FeedItem item)
		{
			if (item.Season > Season || (item.Season == Season && item.Episode >= Episode))
			{
				if (MatchOnce)
				{
					foreach (FeedItem downloaded in DownloadedItems)
					{
						if (downloaded.IsTV) // In case filter used to be NotTV
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
		
		# endregion
		
		
		public override string ToString()
		{
			return string.Format("[Filter Title={0}, IsActive={1}, IgnoreCaps={2}, TitleFilter={3}, RegexPattern={4}, Include={5}, Exclude={6}, FilterEpisode={7}, Season={8}, Episode={9}]", _Title, _IsActive, _IgnoreCaps, _TitleFilter, _RegexPattern, Include, Exclude, _IsTV, _Season, _Episode);
		}
	}
}
