using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using tRSS.Utilities;

namespace tRSS.Model
{
	/// <summary>
	/// Description of Filter.
	/// </summary>
	public class Filter : INotifyBase
	{
		private const string SEPARATOR = ";";
		
		private string _Title;
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
		
		private bool _Active;
		public bool Active
		{
			get
			{
				return _Active;
			}
			set
			{
				_Active = value;
				onPropertyChanged("Active");
			}
		}
		
		private Feed _SelectedFeed;
		public Feed SelectedFeed
		{
			get
			{
				return _SelectedFeed;
			}
			set
			{
				_SelectedFeed = value;
				onPropertyChanged("SelectedFeed");
			}
		}
		
		private string _TitleFilter;
		public string TitleFilter
		{
			get
			{
				return _TitleFilter;
			}
			set
			{
				_TitleFilter = value;
				onPropertyChanged("Filter");
			}
		}
		
		public string TitleFilterRegex
		{
			get
			{
				// UNDONE Implement a simple filter for torrent titles
				StringBuilder sb = new StringBuilder();
				sb.Append("^");
				foreach (char letter in TitleFilter)
				{
					if(letter.Equals('.'))
					{
						sb.Append(@"\s");
					}
					else{ sb.Append(letter); } // FIXME Not sure if this is correct way to Regex word blocks
				}
				return @sb.ToString();
			}
		}
		
		private List<string> _Include;
		public string Include
		{
			get
			{
				return string.Join(SEPARATOR, _Include.ToArray());
			}
			set
			{
				_Include = new List<string>(value.Split(SEPARATOR[0]));
				onPropertyChanged("Include");
			}
		}
		
		private List<string> _Exclude = new List<string>();
		public string Exclude
		{
			get 
			{
				return string.Join(SEPARATOR, _Exclude.ToArray());
			}
			set
			{
				_Exclude = new List<string>(value.Split(SEPARATOR[0]));
				onPropertyChanged("Exclude");
			}
		}
		
		private bool _FilterEpisode = false;
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
