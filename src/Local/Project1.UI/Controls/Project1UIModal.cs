using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Project1.UI.Controls
{
    public class Project1UIModal : ContentControl
    {
        /// <summary>
        /// 点击关闭
        /// </summary>
        public bool ClickClose
        {
            get { return (bool)GetValue(ClickCloseProperty); }
            set { SetValue(ClickCloseProperty, value); }
        }
        public static readonly DependencyProperty ClickCloseProperty =
            DependencyProperty.Register("ClickClose", typeof(bool), typeof(Project1UIModal), new PropertyMetadata(true));
        /// <summary>
        /// 持续时间
        /// </summary>
        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(int), typeof(Project1UIModal), new PropertyMetadata((int)3));
        /// <summary>
        /// 遮罩层透明度
        /// </summary>
        public double MaskLayerOpacity
        {
            get { return (double)GetValue(MaskLayerOpacityProperty); }
            set { SetValue(MaskLayerOpacityProperty, value); }
        }
        public static readonly DependencyProperty MaskLayerOpacityProperty =
            DependencyProperty.Register("MaskLayerOpacity", typeof(double), typeof(Project1UIModal), new PropertyMetadata((double)0, new PropertyChangedCallback(OnMaskLayerOpacityPropertyChanged)));

        private static void OnMaskLayerOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Project1UIModal obj = d as Project1UIModal;
            if (obj != null)
            {
                double value = (double)e.NewValue;
                if (value == 0)
                {
                    obj.Visibility = Visibility.Hidden;
                }
                if (value > 0 && obj.Visibility == Visibility.Hidden)
                {
                    obj.Visibility = Visibility.Visible;
                }
            }
        }

        //public string Text
        //{
        //    get { return (string)GetValue(TextProperty); }
        //    set { SetValue(TextProperty, value); }
        //}
        //public static readonly DependencyProperty TextProperty =
        //    DependencyProperty.Register("Text", typeof(string), typeof(Project1UIModal));
        public bool Show
        {
            get { return (bool)GetValue(ShowProperty); }
            set { SetValue(ShowProperty, value); }
        }
        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.Register("Show", typeof(bool), typeof(Project1UIModal), new PropertyMetadata(false, new PropertyChangedCallback(OnShowPropertyChanged)));

        private DispatcherTimer closeTimer;
        private static void OnShowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Project1UIModal obj = d as Project1UIModal;
            if (obj != null)
            {
                bool value = (bool)e.NewValue;
                if (value)
                {
                    if (obj.Visibility == Visibility.Hidden)
                    {
                        obj.Visibility = Visibility.Visible;
                    }
                    obj.closeTimer.Interval = new TimeSpan(0, 0, obj.Duration);
                    if (!obj.closeTimer.IsEnabled && obj.Duration > 0)
                    {
                        obj.closeTimer.Start();
                    }
                }
            }
        }

        public Project1UIModal()
        {
            DefaultStyleKey = typeof(Project1UIModal);
            Visibility = Visibility.Hidden;
            closeTimer = new DispatcherTimer();
            closeTimer.Tick += CloseTimer_Tick;
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            Show = false;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!ClickClose)
            {
                return;
            }
            if (closeTimer.IsEnabled)
            {
                closeTimer.Stop();
            }
            Show = false;
        }
    }
}
