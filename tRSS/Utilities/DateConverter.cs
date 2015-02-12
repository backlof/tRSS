using System;
using System.Windows.Data;
using System.Globalization;

namespace tRSS.Utilities
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
			
			if(date.Equals(default(DateTime)))
			{
				return "";
			}
			
			return date.ToString(_format, CultureInfo.CurrentCulture);
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DateTime.ParseExact((string) value, _format, CultureInfo.CurrentCulture);
		}
	}
}
