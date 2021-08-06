using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectEye.Resources.Converters
{
    /// <summary>
    /// 滑块值转换为宽度
    /// </summary>
    public class NumberToStringConver : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value.ToString();
        }

    

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
