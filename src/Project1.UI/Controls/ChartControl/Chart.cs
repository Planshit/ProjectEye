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
using System.Windows.Shapes;

namespace Project1.UI.Controls.ChartControl
{
    public class Chart : Control
    {
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
        /// 平均值刻度
        /// </summary>
        private Rectangle AverageTick;
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
        public Chart()
        {
            DefaultStyleKey = typeof(Chart);

        }

        private void Chart_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemContainer = GetTemplateChild("Container") as StackPanel;
            MainContainer = GetTemplateChild("MainContainer") as Grid;
            AverageTick = GetTemplateChild("AverageTick") as Rectangle;
            BottomTick = GetTemplateChild("BottomTick") as Rectangle;
            MaxValueLabel = GetTemplateChild("MaxValueLabel") as TextBlock;
            AverageBorder = GetTemplateChild("AverageBorder") as Border;
            AverageLabel = GetTemplateChild("AverageLabel") as TextBlock;
            BottomValueBorder = GetTemplateChild("BottomValueBorder") as Border;
            BottomValueLabel = GetTemplateChild("BottomValueLabel") as TextBlock;

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
            Debug.WriteLine(1);
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
            double averageValue = Data.Average(m => m.Value);
            double bottomValue = Data.Where(m => m.Value > 0).Min(m => m.Value);

            double averageTickMargin = (averageValue / MaxValue) * itemTrueHeight + 30;
            double bottomTickMargin = (bottomValue / MaxValue) * itemTrueHeight + 30;
            AverageTick.Margin = new Thickness(0, 0, 0, averageTickMargin);
            BottomTick.Margin = new Thickness(0, 0, 0, bottomTickMargin);

            //刻度标注
            MaxValueLabel.Text = MaxValue.ToString();

            AverageBorder.Margin = new Thickness(0, 0, 0, averageTickMargin - AverageBorder.ActualHeight / 2 - AverageTick.Height / 2);
            AverageLabel.Text = ((int)averageValue).ToString();

            BottomValueBorder.Margin = new Thickness(0, 0, 0, bottomTickMargin - BottomValueBorder.ActualHeight / 2 - BottomTick.Height / 2);
            BottomValueLabel.Text = bottomValue.ToString();

        }
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
            return item;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);
            RenderTick();
        }
    }
}
