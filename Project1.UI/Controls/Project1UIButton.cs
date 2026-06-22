using Project1.UI.Controls.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project1.UI.Controls
{
    public class Project1UIButton : Button
    {
      
        //按钮圆角半径
        /// <summary>
        /// 按钮圆角半径
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Project1UIButton));

        //图标对齐方式
        /// <summary>
        /// 图标对齐方式（仅支持左右）
        /// </summary>
        public HorizontalAlignment IconAlignment
        {
            get { return (HorizontalAlignment)GetValue(IconAlignmentProperty); }
            set { SetValue(IconAlignmentProperty, value); }
        }
        public static readonly DependencyProperty IconAlignmentProperty =
            DependencyProperty.Register("IconAlignment", typeof(HorizontalAlignment), typeof(Project1UIButton));
        //图标
        /// <summary>
        /// 图标
        /// </summary>
        public Project1UIIconType Icon
        {
            get { return (Project1UIIconType)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Project1UIIconType), typeof(Project1UIButton),new PropertyMetadata(Project1UIIconType.Null));
        //图标大小
        /// <summary>
        /// 图标大小
        /// </summary>
        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(Project1UIButton));

        public Project1UIButton()
        {
            DefaultStyleKey = typeof(Project1UIButton);

        }
     

    }
}
