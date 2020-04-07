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
using System.Windows.Controls.Primitives;
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

        #region 依赖属性
        //控件依赖属性
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
                typeof(Chart),
                new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnPropertyChanged)));

        #endregion

        #region TickLabelWidth
        /// <summary>
        /// TickLabelWidth
        /// </summary>
        public double TickLabelWidth
        {
            get { return (double)GetValue(TickLabelWidthProperty); }
            set { SetValue(TickLabelWidthProperty, value); }
        }
        public static readonly DependencyProperty TickLabelWidthProperty =
            DependencyProperty.Register("TickLabelWidth",
                typeof(double),
                typeof(Chart),
                new PropertyMetadata((double)50));


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
                typeof(Chart),
                new PropertyMetadata(true));


        #endregion

        #region 图表数据
        /// <summary>
        /// 图表数据
        /// </summary>
        public IEnumerable<ChartDataModel> Data
        {
            get { return (IEnumerable<ChartDataModel>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data",
                typeof(IEnumerable<ChartDataModel>),
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
                new PropertyMetadata((double)0, new PropertyChangedCallback(OnPropertyChanged)));


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

        #region 刻度值内容
        /// <summary>
        /// 刻度值内容 变量：{value}
        /// </summary>
        public string TickText
        {
            get { return (string)GetValue(TickTextProperty); }
            set { SetValue(TickTextProperty, value); }
        }
        public static readonly DependencyProperty TickTextProperty =
            DependencyProperty.Register("TickText",
                typeof(string),
                typeof(Chart),
                new PropertyMetadata("{value}"));


        #endregion

        #region 平均值文本
        /// <summary>
        /// 平均值文本
        /// </summary>
        public string AverageText
        {
            get { return (string)GetValue(AverageTextProperty); }
            set { SetValue(AverageTextProperty, value); }
        }
        public static readonly DependencyProperty AverageTextProperty =
            DependencyProperty.Register("AverageText",
                typeof(string),
                typeof(Chart),
                new PropertyMetadata(""));


        #endregion
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (d as Chart);
            if (e.Property == DataProperty)
            {

                var newData = e.NewValue as IEnumerable<ChartDataModel>;

                if (newData != null)
                {
                    chart.Render();
                }
            }
            if (e.Property == AverageProperty)
            {
                double value = (double)e.NewValue;

                if (chart.averageValue != (int)chart.averageValue)
                {
                    //平均值非整数
                    value = Math.Round((double)e.NewValue, 1);
                }
                else
                {
                    value = (int)value;
                }
                chart.AverageText = chart.TickText.Replace("{value}", value.ToString());
            }

        }

        #endregion

        #region 私有属性
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

        private Popup Popup;
        #endregion

        #region 事件
        /// <summary>
        /// 动画锁定事件
        /// </summary>
        /// <param name="sender">图表控件</param>
        /// <param name="animationlock">是否锁定</param>
        /// <param name="type">0：容器滚动动画；1：刻度值动画；2刻度线动画</param>
        public delegate void AnimationEventHandler(object sender, bool animationlock, int type);
        /// <summary>
        /// 动画锁定事件
        /// </summary>
        public event AnimationEventHandler OnAnimationLockEvent;
        #endregion
        public Chart()
        {
            DefaultStyleKey = typeof(Chart);
            storyboard = new Storyboard();
            storyboard.Duration = TimeSpan.FromSeconds(1);
            scrollStoryboard = new Storyboard();
            scrollStoryboard.Duration = TimeSpan.FromSeconds(1);
            scrollStoryboard.Completed += (e, c) =>
            {
                //容器滚动动画结束

                //发送通知
                OnAnimationLockEvent?.Invoke(this, false, 0);

                isScrollAnimationActive = false;
            };
            sineEase = new SineEase() { EasingMode = EasingMode.EaseInOut };
            scrollAnimation = new DoubleAnimation();
        }


        private void Chart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ScrollLeftButtonVisibility = Visibility.Hidden;
            ScrollRightButtonVisibility = Visibility.Hidden;
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
            var eventArg = new System.Windows.Input.MouseWheelEventArgs(e.MouseDevice,
                e.Timestamp,
                e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            this.RaiseEvent(eventArg);
            //ScrollViewer scrollviewer = sender as ScrollViewer;
            //HandleScrollViewerState();
            //if (e.Delta > 0)
            //{
            //    Scroll(0);
            //}
            //else
            //{
            //    Scroll(1);
            //}
            e.Handled = false;
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
            Popup = GetTemplateChild("Popup") as Popup;


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

            }
            if (AverageTick != null)
            {
                AverageTick.MouseEnter += (s, c) =>
                {
                    VisualStateManager.GoToElementState(AverageTick, "AverageTickMouseEnter", true);
                    Popup.IsOpen = true;
                };
                AverageTick.MouseLeave += (s, c) =>
                {
                    VisualStateManager.GoToElementState(AverageTick, "AverageTickMouseLeave", true);
                    if (!Popup.IsFocused)
                    {
                        Popup.IsOpen = false;
                    }
                };
            }
            MouseLeave += Chart_MouseLeave;
            Render();
            //Loaded += Chart_Loaded;
        }


        private void Render()
        {
            if (ItemContainer == null || Data == null)
            {
                return;
            }
            ItemContainer.Children.Clear();
            Calculate();
            RenderItems();
            RenderTick();
            InitAnimation();
            InitScrollAnimation();
            if (IsAnimation)
            {
                storyboard.Begin();
            }
        }
        /// <summary>
        /// 计算
        /// </summary>
        private void Calculate()
        {
            //计算最大值
            //如果设置了固定的最大值则使用，否则查找数据中的最大值
            MaxValue = MaxValue > 0 ? MaxValue : Data.Count() > 0 ? Data.Max(m => m.Value) : 0;
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

            double itemTrueHeight = ItemContainer.ActualHeight - 30 - 20;
            averageValue = Data.Count() > 0 ? Data.Average(m => m.Value) : 0;
            double bottomValue = Data.Count() > 0 ? Data.Where(m => m.Value >= 0).Min(m => m.Value) : 0;

            averageTickY = (averageValue / MaxValue) * itemTrueHeight + 30 - AverageTick.ActualHeight / 2;

            //最终显示的平均值需要处理小数点，仅保留一位
            averageValue = Math.Round(averageValue, 1);
            Average = averageValue;

            double bottomTickMargin = (bottomValue / MaxValue) * itemTrueHeight + 30;
            TranslateTransform averageTransform = new TranslateTransform()
            {
                Y = -averageTickY
            };
            AverageTick.RenderTransform = averageTransform;
            //AverageTick.Margin = new Thickness(0, 0, 0, averageTickMargin);
            BottomTick.Margin = new Thickness(0, 0, 0, bottomTickMargin);

            //刻度标注
            MaxValueLabel.Text = TickText.Replace("{value}", MaxValue.ToString());
            averageLabelY = averageTickY - AverageLabel.ActualHeight - AverageTick.ActualHeight / 2;
            //AverageBorder.Margin = new Thickness(0, 0, 0, averageTickMargin - AverageBorder.ActualHeight / 2 - AverageTick.Height / 2);
            AverageBorder.RenderTransform = new TranslateTransform()
            {
                Y = -averageLabelY
            };
            //AverageLabel.Text = TickText.Replace("{value}", Math.Round(averageValue, 1).ToString());

            BottomValueBorder.Margin = new Thickness(0, 0, 0, bottomTickMargin - BottomValueBorder.ActualHeight / 2 - BottomTick.Height / 2);
            BottomValueLabel.Text = TickText.Replace("{value}", bottomValue.ToString());

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

            for (int i = 0; i < Data.Count(); i++)
            {
                ChartDataModel data = Data.ElementAt(i);
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
            ChartItem item = new ChartItem(this);
            item.TagName = chartData.Tag;
            item.Value = chartData.Value;
            item.MaxValue = maxValue;
            item.ItemColor = ItemColor;
            item.PopupText = chartData.PopupText;
            item.IsSelected = chartData.IsSelected;
            item.IsAnimation = IsAnimation;
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
            averageLabelValueAnimation.To = averageValue;
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
                isScrollAnimationActive = IsAnimation;
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
                if (IsAnimation)
                {
                    OnAnimationLockEvent?.Invoke(this, true, 0);
                    scrollAnimation.From = ItemsScrollViewer.HorizontalOffset;
                    scrollAnimation.To = to;
                    scrollStoryboard.Begin();

                }
                else
                {
                    ItemsScrollViewer.ScrollToHorizontalOffset(to);
                    HandleScrollViewerState();
                }
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