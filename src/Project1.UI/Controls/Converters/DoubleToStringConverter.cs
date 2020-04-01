using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Project1.UI.Controls.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "0";
            if (value != null && (double)value > 0)
            {
                result = Math.Round((double)value, 1).ToString();
                //string[] resultSplitArray = result.Split('.');
                //int decimalValue = int.Parse(resultSplitArray[1]);
                //if (decimalValue == 0)
                //{
                //    result = resultSplitArray[0];
                //}
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
