
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace tRSS.Utilities
{
	[ValueConversion(typeof(double), typeof(DataGridLength))]
	public class DataGridWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			DataGridLengthConverter dglc = new DataGridLengthConverter();
			double val = (double)value;
			return dglc.ConvertFrom(val);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((DataGridLength)value).DisplayValue;
		}
	}
}
