using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Project1.UI.Controls
{
    /// <summary>
    /// 颜色选择控件
    /// </summary>
    public class Project1UIColorSelect : Control
    {
        /// <summary>
        /// 当前选择颜色
        /// </summary>
        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(Project1UIColorSelect), new PropertyMetadata(Brushes.Black));

        public Project1UIColorSelect()
        {
            DefaultStyleKey = typeof(Project1UIColorSelect);
            ToolTip = "选择颜色";
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)

            {

                System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(colorDialog.Color);

                SolidColorBrush solidColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B));

                Color = solidColorBrush;

            }

        }
    }
}
