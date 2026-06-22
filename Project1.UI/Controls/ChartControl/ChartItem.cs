using Project1.UI.Controls.ChartControl.Models;
using Project1.UI.Controls.Models;
using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        #region PopupText
        /// <summary>
        /// PopupText
        /// </summary>
        public string PopupText
        {
            get { return (string)GetValue(PopupTextProperty); }
            set { SetValue(PopupTextProperty, value); }
        }
        public static readonly DependencyProperty PopupTextProperty =
            DependencyProperty.Register("PopupText",
                typeof(string),
                typeof(ChartItem),
                new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged))
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
        /// IsSelected
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
                new PropertyMetadata(false,new PropertyChangedCallback(OnPropertyChanged))
                );


        #endregion

        #region AnimationLock
        /// <summary>
        /// AnimationLock控制动画锁定，为true时不允许执行动画，防止UI卡顿
        /// </summary>
        public bool AnimationLock
        {
            get { return (bool)GetValue(AnimationLockProperty); }
            set { SetValue(AnimationLockProperty, value); }
        }
        public static readonly DependencyProperty AnimationLockProperty =
            DependencyProperty.Register("AnimationLock",
                typeof(bool),
                typeof(ChartItem),
                new PropertyMetadata(false)
                );


        #endregion

        #region 是否启用动画
        /// <summary>
        /// 是否启用动画
        /// </summary>
        public bool IsAnimation
        {
            get { return (bool)GetValue(IsAnimationProperty); }
            set { SetValue(IsAnimationProperty, value); }
        }
        public static readonly DependencyProperty IsAnimationProperty =
            DependencyProperty.Register("IsAnimation",
                typeof(bool),
                typeof(ChartItem),
                new PropertyMetadata(true)
                );


        #endregion
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartItem chartItem = d as ChartItem;
            if (e.Property == ItemColorProperty)
            {
                chartItem.FillColor();
            }
            if (e.Property == PopupTextProperty)
            {
                if (e.NewValue != null)
                {
                    chartItem.HandlePopupText();
                }
            }
            if (e.Property == IsSelectedProperty)
            {
                if (e.NewValue != null)
                {
                    if ((bool)e.NewValue && chartItem.Label!=null)
                    {
                        chartItem.Label.Foreground = chartItem.ItemColor;
                    }
                }
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

        /// <summary>
        /// 图表容器控件
        /// </summary>
        private Chart chart;
        /// <summary>
        /// 列
        /// </summary>
        private TextBlock Label;
        public ChartItem(Chart chart)
        {
            DefaultStyleKey = typeof(ChartItem);
            this.chart = chart;
            chart.OnAnimationLockEvent += Chart_OnAnimationLockEvent;
            storyboard = new Storyboard();
            storyboard.Duration = TimeSpan.FromSeconds(1);

            MarkStoryboard = new Storyboard();
            MarkStoryboard.Duration = TimeSpan.FromSeconds(1);
            //MarkStoryboard.RepeatBehavior = RepeatBehavior.Forever;
        }

        private void Chart_OnAnimationLockEvent(object sender, bool animationlock, int type)
        {
            if (type == 0)
            {
                AnimationLock = animationlock;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ValueControl = GetTemplateChild("ValueControl") as Rectangle;
            TextContainer = GetTemplateChild("TextContainer") as Canvas;
            SelectedContainer = GetTemplateChild("SelectedContainer") as Border;
            CheckMark = GetTemplateChild("CheckMark") as Path;
            Label = GetTemplateChild("Label") as TextBlock;

            Loaded += ChartItem_Loaded;
        }

        private void ChartItem_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
        }

        /// <summary>
        /// 处理弹出窗口文本变量
        /// </summary>
        /// <param name="value"></param>
        private void HandlePopupText(string value = null)
        {
            if (value == null)
            {
                value = PopupText;
            }

            if (PopupText != value || Regex.IsMatch(value, @"\{(.*?)\}"))
            {
                value = value.Replace("{value}", Value.ToString());
                PopupText = value;
            }
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
            HandlePopupText();
            Calculate();
            ValueControl.Height = TrueHeight;

            FillColor();

            InitAnimation();
            BeginAnimation();

            if (IsSelected)
            {
                Label.Foreground = ItemColor;
            }
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
                    Color = Project1UIColor.BrushToColor(ItemColor, .5),
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
            if (IsAnimation)
            {
                storyboard.Begin();
                if (IsSelected)
                {
                    MarkStoryboard.Begin();
                }
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);
            //Render();
        }
    }
}
