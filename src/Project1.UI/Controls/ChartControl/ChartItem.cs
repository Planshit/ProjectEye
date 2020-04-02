using Project1.UI.Controls.ChartControl.Models;
using Project1.UI.Controls.Models;
using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Project1.UI.Controls.ChartControl
{
    public class ChartItem : Control
    {
        #region 值
        /// <summary>
        /// 值
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(double),
                typeof(ChartItem),
                new PropertyMetadata((double)0, new PropertyChangedCallback(OnValuePropertyChanged))
                );

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var chart = (d as Project1UIChart);
            //if (chart != null)
            //{
            //    //重绘图表
            //    chart.InvalidateVisual();
            //}

        }
        #endregion

        #region Tag
        /// <summary>
        /// Tag
        /// </summary>
        public string TagName
        {
            get { return (string)GetValue(TagNameProperty); }
            set { SetValue(TagNameProperty, value); }
        }
        public static readonly DependencyProperty TagNameProperty =
            DependencyProperty.Register("TagName",
                typeof(string),
                typeof(ChartItem),
                new PropertyMetadata("Tag")
                );

        #endregion

        #region MaxValue
        /// <summary>
        /// 值
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue",
                typeof(double),
                typeof(ChartItem),
                new PropertyMetadata((double)100)
                );


        #endregion


        #region ItemColor
        /// <summary>
        /// ItemColor
        /// </summary>
        public Brush ItemColor
        {
            get { return (Brush)GetValue(ItemColorProperty); }
            set { SetValue(ItemColorProperty, value); }
        }
        public static readonly DependencyProperty ItemColorProperty =
            DependencyProperty.Register("ItemColor",
                typeof(Brush),
                typeof(ChartItem),
                new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnPropertyChanged)));

        #endregion

        #region IsSelected
        /// <summary>
        /// 值
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected",
                typeof(bool),
                typeof(ChartItem),
                new PropertyMetadata(false)
                );


        #endregion
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartItem chartItem = d as ChartItem;
            if (e.Property == ItemColorProperty)
            {
                chartItem.FillColor();
            }

        }
        //计算的真实高度
        private double TrueHeight = 0;

        private Rectangle ValueControl;
        private Canvas TextContainer;
        /// <summary>
        /// 选中标记容器
        /// </summary>
        private Border SelectedContainer;

        //动画
        private Storyboard storyboard;
        /// <summary>
        /// 标记动画
        /// </summary>
        private Storyboard MarkStoryboard;

        /// <summary>
        /// 选中标记
        /// </summary>
        private Path CheckMark;
        public ChartItem()
        {
            DefaultStyleKey = typeof(ChartItem);
            storyboard = new Storyboard();
            storyboard.Duration = TimeSpan.FromSeconds(1);

            MarkStoryboard = new Storyboard();
            MarkStoryboard.Duration = TimeSpan.FromSeconds(1);
            //MarkStoryboard.RepeatBehavior = RepeatBehavior.Forever;

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ValueControl = GetTemplateChild("ValueControl") as Rectangle;
            TextContainer = GetTemplateChild("TextContainer") as Canvas;
            SelectedContainer = GetTemplateChild("SelectedContainer") as Border;
            CheckMark = GetTemplateChild("CheckMark") as Path;
            Loaded += ChartItem_Loaded;
        }

        private void ChartItem_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
        }

        /// <summary>
        /// 初始化动画
        /// </summary>
        private void InitAnimation()
        {
            //弹出动画
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = TrueHeight;
            animation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(animation, ValueControl);
            Storyboard.SetTargetProperty(animation, new PropertyPath(HeightProperty));
            storyboard.Children.Add(animation);


            if (CheckMark != null)
            {
                CheckMark.RenderTransform = new TranslateTransform() { Y = 5 };
                DoubleAnimation markAnimation = new DoubleAnimation();
                markAnimation.From = 3;
                markAnimation.To = 6;
                markAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };
                MarkStoryboard.AutoReverse = true;
                MarkStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                Storyboard.SetTarget(markAnimation, CheckMark);
                Storyboard.SetTargetProperty(markAnimation, new PropertyPath("RenderTransform.Y"));
                MarkStoryboard.Children.Add(markAnimation);
            }
        }
        private void Render()
        {
            Calculate();
            ValueControl.Height = TrueHeight;

            FillColor();

            InitAnimation();
            BeginAnimation();
        }

        /// <summary>
        /// 填充颜色
        /// </summary>
        private void FillColor()
        {
            if (ValueControl != null)
            {
                LinearGradientBrush brush = new LinearGradientBrush();

                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                brush.GradientStops.Add(new GradientStop()
                {
                    Color = Project1UIColor.BrushToColor(ItemColor, .8),
                    Offset = 0
                });

                brush.GradientStops.Add(new GradientStop()
                {
                    Color = Project1UIColor.BrushToColor(ItemColor, 1),
                    Offset = 1,
                });
                ValueControl.Fill = brush;
            }
        }

        /// <summary>
        /// 计算真实高度，应该在控件完成显示后调用
        /// </summary>
        private void Calculate()
        {
            //计算真实高度
            TrueHeight = (Value / MaxValue) * (ActualHeight - TextContainer.ActualHeight - SelectedContainer.ActualHeight);
        }

        private void BeginAnimation()
        {
            storyboard.Begin();
            if (IsSelected)
            {
                MarkStoryboard.Begin();
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);
            //Render();
        }
    }
}
