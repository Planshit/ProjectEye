using Project1.UI.Controls.ChartControl.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Project1.UI.Controls.ChartControl
{
    public class Chart : Control
    {
        //滚动视图依赖属性
        #region 滚动位置
        public double HOffset
        {
            get { return (double)GetValue(HOffsetProperty); }
            set { SetValue(HOffsetProperty, value); }
        }
        public static readonly DependencyProperty HOffsetProperty =
            DependencyProperty.Register("HOffset",
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata((double)0, new PropertyChangedCallback(OnHOffsetChanged)));


        private static void OnHOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = d as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
            }
        }


        #endregion

        //控件依赖属性
        #region 图表数据
        /// <summary>
        /// 图表数据
        /// </summary>
        public ObservableCollection<ChartDataModel> Data
        {
            get { return (ObservableCollection<ChartDataModel>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data",
                typeof(ObservableCollection<ChartDataModel>),
                typeof(Chart),
                new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged))
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
                typeof(Chart),
                new PropertyMetadata((double)0));


        #endregion

        #region 间距值
        /// <summary>
        /// 间距值
        /// </summary>
        public double GapValue
        {
            get { return (double)GetValue(GapValueProperty); }
            set { SetValue(GapValueProperty, value); }
        }
        public static readonly DependencyProperty GapValueProperty =
            DependencyProperty.Register("GapValue",
                typeof(double),
                typeof(Chart),
                new PropertyMetadata((double)10));


        #endregion

        #region 平均值
        /// <summary>
        /// 平均值
        /// </summary>
        public double Average
        {
            get { return (double)GetValue(AverageProperty); }
            set { SetValue(AverageProperty, value); }
        }
        public static readonly DependencyProperty AverageProperty =
            DependencyProperty.Register("Average",
                typeof(double),
                typeof(Chart),
                new PropertyMetadata((double)0));


        #endregion

        #region 向左滚动控制按钮显示状态
        /// <summary>
        /// 获取或设置向左滚动控制按钮显示状态
        /// </summary>
        public Visibility ScrollLeftButtonVisibility
        {
            get { return (Visibility)GetValue(ScrollLeftButtonVisibilityProperty); }
            set { SetValue(ScrollLeftButtonVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ScrollLeftButtonVisibilityProperty =
            DependencyProperty.Register("ScrollLeftButtonVisibility",
                typeof(Visibility),
                typeof(Chart),
                new PropertyMetadata(Visibility.Hidden));


        #endregion

        #region 向右滚动控制按钮显示状态
        /// <summary>
        /// 获取或设置向右滚动控制按钮显示状态
        /// </summary>
        public Visibility ScrollRightButtonVisibility
        {
            get { return (Visibility)GetValue(ScrollRightButtonVisibilityProperty); }
            set { SetValue(ScrollRightButtonVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ScrollRightButtonVisibilityProperty =
            DependencyProperty.Register("ScrollRightButtonVisibility",
                typeof(Visibility),
                typeof(Chart),
                new PropertyMetadata(Visibility.Hidden));


        #endregion
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (d as Chart);
            if (chart != null)
            {
                //重绘图表
                //chart.InvalidateVisual();

                chart.Render();
            }

        }
        /// <summary>
        /// 数据容器
        /// </summary>
        private StackPanel ItemContainer;
        /// <summary>
        /// 主容器
        /// </summary>
        private Grid MainContainer;
        /// <summary>
        /// 项目滚动容器
        /// </summary>
        private ScrollViewer ItemsScrollViewer;
        /// <summary>
        /// 平均值刻度
        /// </summary>
        private Border AverageTick;
        /// <summary>
        /// 底部刻度
        /// </summary>
        private Rectangle BottomTick;
        /// <summary>
        /// 最大值标注
        /// </summary>
        private TextBlock MaxValueLabel;
        /// <summary>
        /// 平均值标注容器
        /// </summary>
        private Border AverageBorder;
        /// <summary>
        /// 平均值标注
        /// </summary>
        private TextBlock AverageLabel;
        /// <summary>
        /// 底部值标注容器
        /// </summary>
        private Border BottomValueBorder;
        /// <summary>
        /// 底部值标注
        /// </summary>
        private TextBlock BottomValueLabel;
        /// <summary>
        /// 平均值
        /// </summary>
        private double averageValue;
        /// <summary>
        /// 平均值Y
        /// </summary>
        private double averageTickY;
        /// <summary>
        /// 平均值标注Y
        /// </summary>
        private double averageLabelY;
        /// <summary>
        /// 动画
        /// </summary>
        private Storyboard storyboard;
        /// <summary>
        /// 向左滚动按钮
        /// </summary>
        private Button ScrollLeftButton;
        /// <summary>
        /// 向右滚动按钮
        /// </summary>
        private Button ScrollRightButton;
        /// <summary>
        /// 项目容器滚动动画容器
        /// </summary>
        private Storyboard scrollStoryboard;
        /// <summary>
        /// 通用动画函数
        /// </summary>
        private SineEase sineEase;
        /// <summary>
        /// 项目容器滚动动画
        /// </summary>
        private DoubleAnimation scrollAnimation;
        /// <summary>
        /// 是否在执行滚动动画
        /// </summary>
        private bool isScrollAnimationActive = false;
        public Chart()
        {
            DefaultStyleKey = typeof(Chart);
            storyboard = new Storyboard();
            storyboard.Duration = TimeSpan.FromSeconds(1);
            scrollStoryboard = new Storyboard();
            scrollStoryboard.Duration = TimeSpan.FromSeconds(1);
            scrollStoryboard.Completed += (e, c) =>
            {
                isScrollAnimationActive = false;
            };
            sineEase = new SineEase() { EasingMode = EasingMode.EaseInOut };
            scrollAnimation = new DoubleAnimation();
        }

        private void Chart_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
            if (ScrollLeftButton != null)
            {
                ScrollLeftButton.Click += ScrollButton_Click;
            }
            if (ScrollRightButton != null)
            {
                ScrollRightButton.Click += ScrollButton_Click;
            }

            if (ItemsScrollViewer != null)
            {
                ItemsScrollViewer.PreviewMouseWheel += ItemsScrollViewer_PreviewMouseWheel;
                ItemsScrollViewer.MouseEnter += ItemsScrollViewer_MouseEnter;
                ItemsScrollViewer.ScrollChanged += ItemsScrollViewer_ScrollChanged;

            }
            if (AverageTick != null)
            {
                AverageTick.MouseEnter += (s, c) =>
                {
                    VisualStateManager.GoToElementState(AverageTick, "AverageTickMouseEnter", true);
                };
                AverageTick.MouseLeave += (s, c) =>
                {
                    VisualStateManager.GoToElementState(AverageTick, "AverageTickMouseLeave", true);
                };
            }
            this.MouseLeave += Chart_MouseLeave;
        }


        private void Chart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ScrollLeftButtonVisibility = Visibility.Hidden;
            ScrollRightButtonVisibility = Visibility.Hidden;
        }

        private void ItemsScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            HandleScrollViewerState();
        }

        private void ItemsScrollViewer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HandleScrollViewerState();
        }

        #region 项目容器滚动按钮点击事件
        private void ScrollButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsScrollViewer != null)
            {
                if (sender == ScrollLeftButton)
                {
                    Scroll(0);
                }
                else
                {
                    Scroll(1);
                }
            }
        }
        #endregion

        #region 项目容器鼠标滚轮事件

        private void ItemsScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            HandleScrollViewerState();
            if (e.Delta > 0)
            {
                Scroll(0);
            }
            else
            {
                Scroll(1);
            }
            e.Handled = true;
        }
        #endregion
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemContainer = GetTemplateChild("Container") as StackPanel;
            MainContainer = GetTemplateChild("MainContainer") as Grid;
            AverageTick = GetTemplateChild("AverageTick") as Border;
            BottomTick = GetTemplateChild("BottomTick") as Rectangle;
            MaxValueLabel = GetTemplateChild("MaxValueLabel") as TextBlock;
            AverageBorder = GetTemplateChild("AverageBorder") as Border;
            AverageLabel = GetTemplateChild("AverageLabel") as TextBlock;
            BottomValueBorder = GetTemplateChild("BottomValueBorder") as Border;
            BottomValueLabel = GetTemplateChild("BottomValueLabel") as TextBlock;
            ItemsScrollViewer = GetTemplateChild("ItemsScrollViewer") as ScrollViewer;
            ScrollLeftButton = GetTemplateChild("ScrollLeftButton") as Button;
            ScrollRightButton = GetTemplateChild("ScrollRightButton") as Button;


            Loaded += Chart_Loaded;
        }


        private void Render()
        {
            if (ItemContainer == null)
            {
                return;
            }
            ItemContainer.Children.Clear();
            Calculate();
            RenderItems();
            RenderTick();
            InitAnimation();
            InitScrollAnimation();
            storyboard.Begin();
        }
        /// <summary>
        /// 计算
        /// </summary>
        private void Calculate()
        {
            //计算最大值
            //如果设置了固定的最大值则使用，否则查找数据中的最大值
            MaxValue = MaxValue > 0 ? MaxValue : Data.Max(m => m.Value);
            //不允许最大值小于10，否则效果不好看
            if (MaxValue < 10)
            {
                MaxValue = 10;
            }
            MaxValue = Math.Round(MaxValue / 2, MidpointRounding.AwayFromZero) * 2;
        }
        #region 渲染刻度
        /// <summary>
        /// 渲染刻度
        /// </summary>
        private void RenderTick()
        {
            if (Data == null || MaxValue <= 0)
            {
                return;
            }

            double itemTrueHeight = ItemContainer.ActualHeight - 30;
            averageValue = Data.Average(m => m.Value);
            double bottomValue = Data.Where(m => m.Value >= 0).Min(m => m.Value);

            averageTickY = (averageValue / MaxValue) * itemTrueHeight + 30;
            double bottomTickMargin = (bottomValue / MaxValue) * itemTrueHeight + 30;
            TranslateTransform averageTransform = new TranslateTransform();
            averageTransform.Y = 0;
            AverageTick.RenderTransform = averageTransform;
            //AverageTick.Margin = new Thickness(0, 0, 0, averageTickMargin);
            BottomTick.Margin = new Thickness(0, 0, 0, bottomTickMargin);

            //刻度标注
            MaxValueLabel.Text = MaxValue.ToString();
            averageLabelY = averageTickY - AverageLabel.ActualHeight - AverageTick.ActualHeight / 2;
            //AverageBorder.Margin = new Thickness(0, 0, 0, averageTickMargin - AverageBorder.ActualHeight / 2 - AverageTick.Height / 2);
            AverageBorder.RenderTransform = new TranslateTransform()
            {
                Y = 0
            };
            //AverageLabel.Text = ((int)averageValue).ToString();

            BottomValueBorder.Margin = new Thickness(0, 0, 0, bottomTickMargin - BottomValueBorder.ActualHeight / 2 - BottomTick.Height / 2);
            BottomValueLabel.Text = bottomValue.ToString();

        }
        #endregion

        #region 渲染图表项目
        /// <summary>
        /// 渲染项目
        /// </summary>
        private void RenderItems()
        {
            if (Data == null)
            {
                return;
            }

            for (int i = 0; i < Data.Count; i++)
            {
                ChartDataModel data = Data[i];
                ChartItem item = GetCreateItem(data, MaxValue);
                if (i > 0)
                {
                    //添加间距
                    item.Margin = new Thickness(GapValue, 0, 0, 0);
                }
                ItemContainer.Children.Add(item);
            }
        }

        private ChartItem GetCreateItem(ChartDataModel chartData, double maxValue)
        {
            ChartItem item = new ChartItem();
            item.TagName = chartData.Name;
            item.Value = chartData.Value;
            item.MaxValue = maxValue;
            item.IsSelected = chartData.IsSelected;
            return item;
        }
        #endregion
        /// <summary>
        /// 初始化动画
        /// </summary>
        private void InitAnimation()
        {
            //通用动画函数
            //SineEase sineEase = new SineEase() { EasingMode = EasingMode.EaseIn };
            //平均值刻度动画
            DoubleAnimation averageAnimation = new DoubleAnimation();
            averageAnimation.From = 0;
            averageAnimation.To = -averageTickY;
            averageAnimation.EasingFunction = sineEase;
            Storyboard.SetTarget(averageAnimation, AverageTick);
            Storyboard.SetTargetProperty(averageAnimation, new PropertyPath("RenderTransform.Y"));
            //平均值标注动画
            DoubleAnimation averageLabelAnimation = new DoubleAnimation();
            averageLabelAnimation.From = 0;
            averageLabelAnimation.To = -averageLabelY;
            averageLabelAnimation.EasingFunction = sineEase;
            Storyboard.SetTarget(averageLabelAnimation, AverageBorder);
            Storyboard.SetTargetProperty(averageLabelAnimation, new PropertyPath("RenderTransform.Y"));
            //平均值标注数值动画
            DoubleAnimation averageLabelValueAnimation = new DoubleAnimation();
            averageLabelValueAnimation.From = 0;
            averageLabelValueAnimation.To = (int)averageValue;
            averageLabelValueAnimation.EasingFunction = sineEase;
            Storyboard.SetTarget(averageLabelValueAnimation, this);
            Storyboard.SetTargetProperty(averageLabelValueAnimation, new PropertyPath(AverageProperty));


            storyboard.Children.Add(averageAnimation);
            storyboard.Children.Add(averageLabelAnimation);
            storyboard.Children.Add(averageLabelValueAnimation);

        }

        /// <summary>
        /// 初始化容器滚动动画（只允许初始化一次）
        /// </summary>
        private void InitScrollAnimation()
        {
            scrollAnimation.EasingFunction = new ExponentialEase()
            {
                EasingMode = EasingMode.EaseOut
            };
            Storyboard.SetTarget(scrollAnimation, ItemsScrollViewer);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath(HOffsetProperty));
            scrollStoryboard.Children.Add(scrollAnimation);
        }
        /// <summary>
        /// 容器滚动
        /// </summary>
        /// <param name="d">0左，1右</param>
        private void Scroll(int d)
        {
            if (ItemsScrollViewer != null &&
                ItemsScrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible &&
                !isScrollAnimationActive
)
            {
                isScrollAnimationActive = true;
                //取一个数据控件
                double LostWidth = 0;

                if (ItemContainer.Children.Count > 1)
                {
                    //取第二个，才有margin属性
                    ChartItem chartItem = ItemContainer.Children[1] as ChartItem;
                    double oneItemWidth = chartItem.ActualWidth + chartItem.Margin.Left;
                    //每屏最多多少个项目
                    double onePageMaxItemNum = ItemsScrollViewer.ActualWidth / oneItemWidth;
                    if ((int)onePageMaxItemNum != onePageMaxItemNum)
                    {
                        //带小数点，需要计算
                        LostWidth = oneItemWidth - double.Parse(("0." + onePageMaxItemNum.ToString().Split('.')[1])) * oneItemWidth - chartItem.Margin.Left;
                    }
                }

                //计算一屏滚动值
                double onePageScrollValue = ItemsScrollViewer.ActualWidth - LostWidth - 2;
                //滚动值
                double to;
                if (d == 0)
                {
                    //左滚
                    double leftScrollValue = ItemsScrollViewer.HorizontalOffset - onePageScrollValue;
                    to = leftScrollValue > 0 ? leftScrollValue : 0;
                }
                else
                {
                    //右滚
                    double rightScrollValue = ItemsScrollViewer.HorizontalOffset + onePageScrollValue;
                    to = rightScrollValue < ItemsScrollViewer.ScrollableWidth ? rightScrollValue : ItemsScrollViewer.ScrollableWidth;
                }

                if (ItemsScrollViewer.HorizontalOffset == to)
                {
                    //不执行无变化的动画
                    isScrollAnimationActive = false;
                    return;
                }

                scrollAnimation.From = ItemsScrollViewer.HorizontalOffset;
                scrollAnimation.To = to;
                scrollStoryboard.Begin();

            }
        }
        private void HandleScrollViewerState()
        {
            if (ItemsScrollViewer != null && ItemsScrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
            {


                //Debug.WriteLine(ItemsScrollViewer.HorizontalOffset + "/" + ItemsScrollViewer.ScrollableWidth + "/全宽：" + ItemContainer.ActualWidth + "/可视区域宽：" + ItemsScrollViewer.ActualWidth);

                //向左滚动判断
                if (ItemsScrollViewer.HorizontalOffset > 0)
                {
                    //是
                    ScrollLeftButtonVisibility = Visibility.Visible;
                }
                else
                {
                    //否
                    ScrollLeftButtonVisibility = Visibility.Hidden;
                }

                //向右滚动判断
                if (ItemsScrollViewer.HorizontalOffset < ItemsScrollViewer.ScrollableWidth)
                {
                    //是
                    ScrollRightButtonVisibility = Visibility.Visible;
                }
                else
                {
                    //否
                    ScrollRightButtonVisibility = Visibility.Hidden;
                }

            }


        }

        #region 重写
        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);
            RenderTick();
        }
        #endregion
    }
}