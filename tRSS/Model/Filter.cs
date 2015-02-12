using System;
using System.Collections.Generic;
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
	/// <summary>
	/// Description of Filter.
	/// </summary>
	[DataContract()]
	public class Filter : INotifyBase
	{
		public Filter(string title)
		{
			Title = title;
		}
		
		public Filter(){}
		
		private const string SEPARATOR = ";";
		
		private string _Title = "New Filter";
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
		
		
		private bool _IsActive = false;
		[DataMember()]
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
		[DataMember()]
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
		[DataMember()]
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
		
		private int _SearchInFeedIndex = 0;
		[DataMember()]
		public int SearchInFeedIndex
		{
			get
			{
				return _SearchInFeedIndex;
			}
			set
			{
				_SearchInFeedIndex = value;
				onPropertyChanged("SearchInFeedIndex");
			}
		}
		
		private string _TitleFilter = "";
		[DataMember()]
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
				RegexPattern = TitleFilter;
				onPropertyChanged("TitleFilter");
			}
		}
		
		private const string REJECT_CHARS =  @"$^{[(|)]}+\";
		
		// REGEX OPERATORS *.$^{[(|)]}+?\
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
				// ? = Any character
				
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
							// Any symbol - any number of times
							sb.Append(@".*");
						}
						else if (letter == '?')
						{
							// Any symbol - 0 or 1 times
							sb.Append(".?");
						}
						else if (letter == '.')
						{
							// Whitespace 1 time
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
		
		
		private string _Include = "";
		[DataMember()]
		public string Include
		{
			get
			{
				return _Include;
			}
			set
			{
				_Include = value;
				onPropertyChanged("Include");
			}
		}
		
		private string _Exclude = "";
		[DataMember()]
		public string Exclude
		{
			get
			{
				return _Exclude;
			}
			set
			{
				_Exclude = value;
				onPropertyChanged("Exclude");
			}
		}
		
		private bool _FilterEpisode = false;
		[DataMember()]
		public bool FilterEpisode
		{
			get
			{
				return _FilterEpisode;
			}
			set
			{
				_FilterEpisode = value;
				onPropertyChanged("FilterEpisode");
			}
		}
		
		private int _Season = 1;
		[DataMember()]
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
		[DataMember()]
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
		
		public void FilterFeed(Feed toSearch)
		{
			if (IsActive)
			{
				foreach (FeedItem item in toSearch.Items)
				{
					bool match = true;
					
					if (DownloadedItems.Contains(item))
					{
						match = false;
					}
					
					
					
					// Deactivate IsActive when download && !IsTV
					/*
						if (MatchOnce && DownloadedItems.Count > 0 && !FilterEpisode)
						{
							break;
						}*/
					
					RegexOptions option = IgnoreCaps ? RegexOptions.IgnoreCase : RegexOptions.None;
					
					// Name
					if (!Regex.IsMatch(item.Title, RegexPattern, option))
					{
						match = false;
					}
					
					/*
					System.Diagnostics.Debug.WriteLine("Pass");
					System.Diagnostics.Debug.WriteLine(item.ToString());
					System.Diagnostics.Debug.WriteLine(ToString());
					 */
					
					string title = Utils.RemoveDiacritics(IgnoreCaps ? item.Title.ToLower() : item.Title);
					
					// Include
					string include = IgnoreCaps ? Include.ToLower() : Include;
					foreach (string element in include.Split(SEPARATOR[0]))
					{
						if(!title.Contains(element))
						{
							match = false;
						}
					}
					
					// Exclude
					// Make list from array - array from separated string - check if string contains any elements from list
					// http://stackoverflow.com/questions/4987873/how-to-find-if-a-string-contains-any-items-of-an-list-of-strings
					// http://stackoverflow.com/questions/251924/string-split-returns-a-string-i-want-a-liststring-is-there-a-one-liner-to-co
					string exclude = IgnoreCaps ? Exclude.ToLower() : Exclude;
					if ((new List<string>(exclude.Split(SEPARATOR[0]))).Any(title.Contains))
					{
						match = false;
					}
					
					// Episode
					if (FilterEpisode)
					{
						Match m = GetEpisodeFromString(item.Title);
						if (m.Success)
						{
							string season  = m.Groups["season"].Value;
							string episode = m.Groups["episode"].Value;
							// TODO Rest of logic
						}
						else
						{
							match = false;
						}
					}
					
					if(match)
					{
						// Ready to download
						item.DownloadedDateTime = DateTime.Now;
						DownloadedItems.Add(item);
					}
					
					
				}
			}
		}
		
		[DataMember()]
		private List<FeedItem> DownloadedItems = new List<FeedItem>();
		
		private static bool IsTV(string title)
		{
			return GetEpisodeFromString(title).Success;
		}
		
		private static Match GetEpisodeFromString(string statement)
		{
			Regex regExp = new Regex(@"S(?<season>\d{1,2})E(?<episode>\d{1,2})", RegexOptions.IgnoreCase);
			return regExp.Match(statement);
		}
		
		public override string ToString()
		{
			return String.Format("[Filter Title={0}, Exclude={1}, FilterEpisode={2}, Season={3}, Episode={4}, Include={5}]", _Title, _Exclude, FilterEpisode, Season, Episode, Include);
		}

	}
}
