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
		private string _Title;		
		public string Title
		{
			get { return _Title; }
			set { _Title = value; onPropertyChanged("Title"); }
		}
		
		private const string SEPARATOR = ";";
		
		public string Include { get; set; }
		private string IncludeRegex 
		{ 
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("^");
				foreach (char letter in Include)
				{
					if(letter.Equals('.'))
					{
						sb.Append(@"\s");
					}
					else{ sb.Append(letter); }
				}
				return @sb.ToString();
				
			}
		}
		
		private List<string> _Exclude = new List<string>();		
		public string Exclude {
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
		
		
		public bool FilterEpisode = false;
		
		public int Season = 1;
		public int Episode = 1;
		
		private static bool IsTV(string title)
		{
			return GetEpisodeFromString(title).Success;
		}
		
		private static Match GetEpisodeFromString(string statement)
		{
			Regex regExp = new Regex(@"S(?<season>\d{1,2})E(?<episode>\d{1,2})", RegexOptions.IgnoreCase);
			return regExp.Match(statement);
		}
	}
}
