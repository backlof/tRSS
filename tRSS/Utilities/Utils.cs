
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace tRSS.Utilities
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public static class Utils
	{
		public static string ReplaceCharacters(this string replace, string characters, string with)
		{
			string[] temp;

			temp = replace.Split(characters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			return String.Join( with, temp );
		}
		
		public static string RemoveDiacritics(string text)
		{
			var normalizedString = text.Normalize(NormalizationForm.FormD);
			StringBuilder sb = new StringBuilder();
			
			foreach (var c in normalizedString)
			{
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					sb.Append(c);
				}
			}
			
			return sb.ToString().Normalize(NormalizationForm.FormC);
		}
	}
}
