using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
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
		
		// REGEX OPERATORS *.$^{[(|)]}+?\
		private const string REJECT_CHARS =  @"$^{[(|)]}+\";
		private const string SEPERATOR = ";";
		private readonly string[] Seperators = {";", "|", "+"};
		
		# region Properties
		
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
		
		private List<string> _Include = new List<string>();
		[DataMember()]
		public string Include
		{
			get
			{
				return String.Join(SEPERATOR, _Include);
			}
			set
			{
				string input = Utils.RemoveDiacritics(value);
				_Include = new List<string>(input.Split(Seperators, StringSplitOptions.RemoveEmptyEntries));
				onPropertyChanged("Include");
			}
		}
		
		private List<string> _Exclude = new List<string>();
		[DataMember()]
		public string Exclude
		{
			get
			{
				return String.Join(SEPERATOR, _Exclude);
			}
			set
			{
				string input = Utils.RemoveDiacritics(value);
				_Exclude = new List<string>(input.Split(Seperators, StringSplitOptions.RemoveEmptyEntries));
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
		
		private ObservableCollection<FeedItem> _DownloadedItems = new ObservableCollection<FeedItem>();
		[DataMember()]
		public ObservableCollection<FeedItem> DownloadedItems
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
		
		# region Filter functionality
		
		public void FilterFeed(Feed toSearch)
		{
			if (IsActive)
			{
				foreach (FeedItem item in toSearch.Items)
				{
					// http://stackoverflow.com/questions/6177219/convert-string-array-to-lowercase
					
					if (!DownloadedItems.Contains(item)) // Special compare in FeedItem class
					{
						string title = Utils.RemoveDiacritics(IgnoreCaps ? item.Title.ToLower() : item.Title);
						RegexOptions option = IgnoreCaps ? RegexOptions.IgnoreCase : RegexOptions.None;
						if (Regex.IsMatch(title, RegexPattern, option))
						{
							// http://stackoverflow.com/questions/14728294/check-if-the-string-contains-all-inputs-on-the-list
							List<string> include = IgnoreCaps ? _Include.ConvertAll(s => s.ToLower()) : _Include;
							
							if (include.All(title.Contains))
							{
								// http://stackoverflow.com/questions/4987873/how-to-find-if-a-string-contains-any-items-of-an-list-of-strings
								List<string> exclude = IgnoreCaps ? _Exclude.ConvertAll(s => s.ToLower()) : _Exclude;
								
								if (!exclude.Any(title.Contains))
								{
									if (FilterEpisode)
									{
										if (item.IsTV)
										{
											if(IsEpisodeToDownload(item))
											{
												DownloadedItems.Add(item);
												item.Download();
											}
										}
									}
									else
									{
										DownloadedItems.Add(item);
										item.Download();
									}
								}
							}
						}
					}
				}
			}
		}
		
		public bool IsEpisodeToDownload(FeedItem item)
		{
			if (item.Season > Season || (item.Season == Season && item.Episode >= Episode))
			{
				if (MatchOnce)
				{
					bool foundSameEpisode = false;
					
					foreach (FeedItem downloaded in DownloadedItems)
					{
						if (item.Season == downloaded.Season && item.Episode == downloaded.Episode)
						{ foundSameEpisode = true; }
					}
					
					if (!foundSameEpisode)
					{
						return true;
					}
				}
				else
				{
					return true;
				}
			}
			
			return false;
		}
		
		# endregion
		
		
		public override string ToString()
		{
			return string.Format("[Filter Title={0}, IsActive={1}, IgnoreCaps={2}, TitleFilter={3}, RegexPattern={4}, Include={5}, Exclude={6}, FilterEpisode={7}, Season={8}, Episode={9}]", _Title, _IsActive, _IgnoreCaps, _TitleFilter, _RegexPattern, Include, Exclude, _FilterEpisode, _Season, _Episode);
		}


	}
}
