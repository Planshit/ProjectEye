using Project1.UI.Controls.ChartControl.Models;
using Project1.UI.Controls.Models;
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
                new PropertyMetadata((double)100, new PropertyChangedCallback(OnMaxValuePropertyChanged))
                );

        private static void OnMaxValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var chart = (d as Project1UIChart);
            //if (chart != null)
            //{
            //    //重绘图表
            //    chart.InvalidateVisual();
            //}

        }
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
                typeof(ChartItem));

        #endregion

        //计算的真实高度
        private double TrueHeight = 0;

        private Rectangle ValueControl;
        private Canvas TextContainer;

        //动画
        private Storyboard storyboard;
        public ChartItem()
        {
            DefaultStyleKey = typeof(ChartItem);
            storyboard = new Storyboard();
            storyboard.Duration = TimeSpan.FromSeconds(1);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ValueControl = GetTemplateChild("ValueControl") as Rectangle;
            TextContainer = GetTemplateChild("TextContainer") as Canvas;
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
            animation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(animation, ValueControl);
            Storyboard.SetTargetProperty(animation, new PropertyPath(HeightProperty));
            storyboard.Children.Add(animation);
        }
        private void Render()
        {
            Calculate();
            ValueControl.Height = TrueHeight;
            InitAnimation();
            BeginAnimation();
        }

        /// <summary>
        /// 计算真实高度，应该在控件完成显示后调用
        /// </summary>
        private void Calculate()
        {
            //计算真实高度
            TrueHeight = (Value / MaxValue) * (ActualHeight - TextContainer.ActualHeight);
        }

        private void BeginAnimation()
        {
            storyboard.Begin();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);
            Render();
        }
    }
}
