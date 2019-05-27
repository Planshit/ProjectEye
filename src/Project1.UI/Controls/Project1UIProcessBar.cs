using Project1.UI.Controls.Enums;
using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Project1.UI.Controls
{
    public class Project1UIProcessBar : Control
    {
        private List<System.Windows.Shapes.Rectangle> rectangles = new List<System.Windows.Shapes.Rectangle>();
        private Dictionary<int, List<LinearDoubleKeyFrame>> linearDoubleKeyFrames = new Dictionary<int, List<LinearDoubleKeyFrame>>();
        private Storyboard storyboard = new Storyboard();

        /// <summary>
        /// 动画点大小
        /// </summary>
        public double PointSize
        {
            get { return (double)GetValue(PointSizeProperty); }
            set { SetValue(PointSizeProperty, value); }
        }
        public static readonly DependencyProperty PointSizeProperty =
            DependencyProperty.Register("PointSize", typeof(double), typeof(Project1UIProcessBar), new PropertyMetadata((double)5));
        /// <summary>
        /// 动画点播放间隔
        /// </summary>
        public double PointPlayInterval
        {
            get { return (double)GetValue(PointPlayIntervalProperty); }
            set { SetValue(PointPlayIntervalProperty, value); }
        }
        public static readonly DependencyProperty PointPlayIntervalProperty =
            DependencyProperty.Register("PointPlayInterval", typeof(double), typeof(Project1UIProcessBar), new PropertyMetadata((double).15));
        public double PointRadius
        {
            get { return (double)GetValue(PointRadiusProperty); }
            set { SetValue(PointRadiusProperty, value); }
        }
        public static readonly DependencyProperty PointRadiusProperty =
            DependencyProperty.Register("PointRadius", typeof(double), typeof(Project1UIProcessBar), new PropertyMetadata((double)0));


        public SolidColorBrush PointColor
        {
            get { return (SolidColorBrush)GetValue(PointColorProperty); }
            set { SetValue(PointColorProperty, value); }
        }
        public static readonly DependencyProperty PointColorProperty =
            DependencyProperty.Register("PointColor", typeof(SolidColorBrush), typeof(Project1UIProcessBar), new PropertyMetadata(Project1UIColor.ThemeColor, new PropertyChangedCallback(OnPointColorPropertyChanged)));

        private static void OnPointColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var processBar = (d as Project1UIProcessBar);
            if (processBar != null)
            {
                foreach(var r in processBar.rectangles)
                {
                    r.Fill = processBar.PointColor;
                }
            }
        }

        public bool IsPlay
        {
            get { return (bool)GetValue(IsPlayProperty); }
            set { SetValue(IsPlayProperty, value); }
        }
        public static readonly DependencyProperty IsPlayProperty =
            DependencyProperty.Register("IsPlay", typeof(bool), typeof(Project1UIProcessBar), new PropertyMetadata((bool)false, new PropertyChangedCallback(OnIsPlayPropertyChangedCallback)));

        private static void OnIsPlayPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var processBar = (d as Project1UIProcessBar);
            if (processBar != null)
            {
                if (processBar.IsPlay)
                {
                    processBar.storyboard.Begin();
                }
                else
                {
                    processBar.storyboard.Stop();

                }
            }
        }

        public Project1UIProcessBar()
        {
            this.DefaultStyleKey = typeof(Project1UIProcessBar);
            storyboard.Duration = TimeSpan.FromSeconds(5);
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            storyboard.Stop();
            double v1 = -5;
            double v2 = this.ActualWidth * .33;
            double v3 = this.ActualWidth * .66;
            double v4 = this.ActualWidth;
            double[] values = { v1, v2, v3, v4 };
            for (int i = 0; i < linearDoubleKeyFrames.Count; i++)
            {
                for (int c = 0; c < linearDoubleKeyFrames[i].Count; c++)
                {
                    var ldkf = linearDoubleKeyFrames[i][c];
                    ldkf.Value = values[c];
                }
            }
            if (IsPlay)
            {
                storyboard.Begin();
            }
        }




        public override void OnApplyTemplate()
        {
            
            base.OnApplyTemplate();
            var rootGrid = GetTemplateChild("Root") as Grid;
            if (rootGrid != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    var d = new System.Windows.Shapes.Rectangle();
                    var tf = new TranslateTransform();
                    tf.X = -(PointSize);
                    d.Fill = PointColor;
                    d.Width = PointSize;
                    d.Height = PointSize;
                    d.RadiusX = PointRadius;
                    d.RadiusY = PointRadius;
                    d.HorizontalAlignment = HorizontalAlignment.Left;
                    d.RenderTransform = tf;
                    rectangles.Add(d);
                    rootGrid.Children.Add(d);
                    DoubleAnimationUsingKeyFrames db = new DoubleAnimationUsingKeyFrames();
                    db.BeginTime = TimeSpan.FromSeconds(i * PointPlayInterval);
                    var ldkf1 = new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0));
                    var ldkf2 = new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0.5));
                    var ldkf3 = new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(2));
                    var ldkf4 = new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(2.5));
                    db.KeyFrames.Add(ldkf1);
                    db.KeyFrames.Add(ldkf2);
                    db.KeyFrames.Add(ldkf3);
                    db.KeyFrames.Add(ldkf4);
                    Storyboard.SetTarget(db, rectangles[i]);
                    Storyboard.SetTargetProperty(db, new PropertyPath("RenderTransform.X"));

                    storyboard.Children.Add(db);
                    linearDoubleKeyFrames.Add(i, new List<LinearDoubleKeyFrame>()
                    {
                        ldkf1,ldkf2,ldkf3,ldkf4
                    });
                }
            }
        }

    }
}
