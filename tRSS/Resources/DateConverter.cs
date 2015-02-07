/*
 * Created by SharpDevelop.
 * User: Alexander
 * Date: 07.02.2015
 * Time: 16:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Data;
using System.Globalization;

namespace tRSS.Resources
{
	/// <summary>
	/// Description of DateConverter.
	/// </summary>
	
	[ValueConversion(typeof(DateTime), typeof(String))]
	public class DateConverter : IValueConverter
	{
		private const string _format = "g";
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{			
			DateTime date = (DateTime)value;
			return date.ToString("g", CultureInfo.CurrentCulture);
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DateTime.ParseExact((string) value, _format, CultureInfo.CurrentCulture);
		}
	}
}
