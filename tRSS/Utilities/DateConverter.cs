using System;
using System.Windows.Data;
using System.Globalization;

namespace tRSS.Utilities
{
	[ValueConversion(typeof(DateTime), typeof(String))]
	public class DateConverter : IValueConverter
	{	
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{			
			DateTime date = (DateTime)value;
			String format = parameter as string;
			
			if(date.Equals(default(DateTime)))
			{
				return "";
			}
			
			return date.ToString(format, CultureInfo.CurrentCulture);
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DateTime.Parse((string) value, CultureInfo.CurrentCulture);
		}
	}
}
