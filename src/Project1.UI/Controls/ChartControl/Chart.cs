using Project1.UI.Controls.ChartControl.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                chart.InvalidateVisual();
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
            Loaded += Chart_Loaded;
        }


        private void Render()
        {
            ItemContainer.Children.Clear();
            RenderItems();
            RenderTick();
        }
        /// <summary>
        /// 渲染刻度
        /// </summary>
        private void RenderTick()
        {
            if (Data == null)
            {
                return;
            }
            double itemTrueHeight = MainContainer.ActualHeight - 30;
            double averageValue = Data.Average(m => m.Value);
            AverageTick.Margin = new Thickness(0, (averageValue / MaxValue) * itemTrueHeight, 0, 0);

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
            //如果设置了固定的最大值则使用，否则查找数据中的最大值
            MaxValue = MaxValue > 0 ? MaxValue : Data.Max(m => m.Value);
            //不允许最大值小于10，否则效果不好看
            if (MaxValue < 10)
            {
                MaxValue = 10;
            }
            MaxValue = Math.Round(MaxValue / 2, MidpointRounding.AwayFromZero) * 2;
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

    }
}
