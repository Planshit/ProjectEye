using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project1.UI.Controls
{
    public class Project1UIModal : Control
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Project1UIModal));
        public bool Show
        {
            get { return (bool)GetValue(ShowProperty); }
            set { SetValue(ShowProperty, value); }
        }
        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.Register("Show", typeof(bool), typeof(Project1UIModal), new PropertyMetadata(false, new PropertyChangedCallback(ShowPropertyChangedCallback)));

        private static void ShowPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Project1UIModal obj = d as Project1UIModal;
            if (obj != null)
            {
                obj.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Project1UIModal()
        {
            DefaultStyleKey = typeof(Project1UIModal);
        }
    }
}
