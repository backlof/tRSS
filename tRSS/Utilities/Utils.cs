
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

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
		
		public static T FindAncestor<T>(DependencyObject from) where T : class
		{
			if (from == null)
			{
				return null;
			}

			T candidate = from as T;
			if (candidate != null)
			{
				return candidate;
			}

			return FindAncestor<T>(VisualTreeHelper.GetParent(from));
		}
		
		public static void PrintError(string message, object sender, Exception e)
		{
			System.Diagnostics.Debug.WriteLine(message);
			System.Diagnostics.Debug.WriteLine(sender.ToString());
			System.Diagnostics.Debug.WriteLine(e.ToString() + Environment.NewLine);
		}
	}
}
