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
    public class Project1UIIcon : Control
    {
        public Project1UIIconType Icon
        {
            get { return (Project1UIIconType)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Project1UIIconType), typeof(Project1UIIcon), new PropertyMetadata(Project1UIIconType.Null));

        public Project1UIIcon()
        {
            DefaultStyleKey = typeof(Project1UIIcon);
        }
    }
}
