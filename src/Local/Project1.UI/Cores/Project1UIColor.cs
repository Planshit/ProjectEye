using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Project1.UI.Cores
{
    public class Project1UIColor
    {

        /// <summary>
        /// 红色
        /// </summary>
        public static SolidColorBrush Red { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f63b36"));
        /// <summary>
        /// 绿色
        /// </summary>
        public static SolidColorBrush Green { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#24cf5f"));
        /// <summary>
        /// 黑色
        /// </summary>
        public static SolidColorBrush Black { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#424242"));

        public static Color Yellow { get; set; } = (Color)ColorConverter.ConvertFromString("#fff000");

        /// <summary>
        /// 黄色
        /// </summary>
        public static SolidColorBrush YellowBrush { get; set; } = new SolidColorBrush(Yellow);

        /// <summary>
        /// 紫色
        /// </summary>
        public static Color Violet { get; set; } = (Color)ColorConverter.ConvertFromString("#0000EF");
        /// <summary>
        /// 紫色笔刷
        /// </summary>
        public static SolidColorBrush VioletBrush { get; set; } = new SolidColorBrush(Violet);

        /// <summary>
        /// 主题色
        /// </summary>
        public static SolidColorBrush ThemeColor { get; set; } = VioletBrush;

        public static SolidColorBrush Get(string color, double opacity = 1)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color))
            {
                Opacity = opacity
            };
        }
        public static Color GetMediaColor(string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);

        }

        public static Color BrushToColor(Brush value, double opacity = 1)
        {
            var b = value;
            var res = (Color)b.GetValue(SolidColorBrush.ColorProperty);
            res.ScA = (float)opacity;
            return res;
        }
    }
}
