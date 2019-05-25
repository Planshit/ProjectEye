using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ProjectEye.Resources.Converters
{
    /// <summary>
    /// 滑块值转换为宽度
    /// </summary>
    public class SliderValueToWidthConver : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double sliderValue =(double)values[0];
            double sliderMaxValue =(double)values[1];
            double sliderWidth = (double)values[2];
            double result = 0;
            if (sliderWidth > 0)
            {
                result = sliderValue / sliderMaxValue * sliderWidth;
            }
            return result;
        }

        public object ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
