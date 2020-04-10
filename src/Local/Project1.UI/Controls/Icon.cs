using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project1.UI.Controls
{
    /// <summary>
    /// Fabric icon
    /// </summary>
    public class Icon : Control
    {
        #region 图标
        /// <summary>
        /// Fabric icon unicode
        /// </summary>
        public string Unicode
        {
            get { return (string)GetValue(UnicodeProperty); }
            set { SetValue(UnicodeProperty, value); }
        }
        public static readonly DependencyProperty UnicodeProperty =
            DependencyProperty.Register("Unicode",
                typeof(string),
                typeof(Icon),
                new PropertyMetadata("E783"));


        #endregion

        public Icon()
        {
            DefaultStyleKey = typeof(Icon);
        }
    }
}
