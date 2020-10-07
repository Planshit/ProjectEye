using Project1.UI.Controls.ChartControl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ProjectEye.Core
{
    class DataReportImageHelper
    {
        private List<ChartDataModel> chartData;
        private readonly double lastMonthlyWork, monthlyWork;
        private readonly string savePath, title;
        private Canvas canvas;
        private StackPanel panel;
        private Window drawWindow;
        private Project1.UI.Controls.ChartControl.Chart chart;
        public DataReportImageHelper(
            string title,
            string savePath,
            List<ChartDataModel> chartData,
            double lastMonthlyWork,
            double monthlyWork)
        {
            this.title = title;
            this.savePath = savePath;
            this.chartData = chartData;
            this.lastMonthlyWork = lastMonthlyWork;
            this.monthlyWork = monthlyWork;

            chart = new Project1.UI.Controls.ChartControl.Chart();
            canvas = new Canvas();
            panel = new StackPanel();
            panel.Margin = new Thickness(20);
            panel.HorizontalAlignment = HorizontalAlignment.Left;
            panel.VerticalAlignment = VerticalAlignment.Top;
            var border = new Border();
            border.BorderThickness = new Thickness(4);
            border.Background = Project1.UI.Cores.Project1UIColor.Get("#ffffff");
            border.BorderBrush = Project1.UI.Cores.Project1UIColor.Get("#4f6bed");
            border.Child = panel;
            border.Loaded += (e, c) =>
            {
                var timer = new DispatcherTimer();
                timer.Tick += (e2, c2) =>
                {
                    timer.Stop();
                    WriteFile((int)border.ActualWidth, (int)border.ActualHeight);
                };
                timer.Interval = new TimeSpan(1000);
                timer.Start();
            };
            canvas.Children.Add(border);

            drawWindow = new Window();
            drawWindow.WindowStyle = WindowStyle.None;
            drawWindow.ShowActivated = false;
            drawWindow.Width = 0;
            drawWindow.Height = 0;
            drawWindow.Visibility = Visibility.Hidden;
            drawWindow.Content = canvas;
        }
        private void Draw()
        {

            //绘制头部
            DrawHeader();
            //绘制月数据
            DrawMonthlyChart();

            drawWindow.Show();
        }

        /// <summary>
        /// 绘制头部
        /// </summary>
        private void DrawHeader()
        {
            //绘制标题
            var title = new TextBlock();
            title.Text = this.title;
            title.FontSize = 20;
            title.FontWeight = FontWeights.Bold;
            title.Foreground = Brushes.White;
            var titleBorder = new Border();
            titleBorder.Background = Project1.UI.Cores.Project1UIColor.Get("#4f6bed");
            titleBorder.HorizontalAlignment = HorizontalAlignment.Left;
            titleBorder.Margin = new Thickness(0, 0, 0, 40);
            titleBorder.Padding = new Thickness(10);
            titleBorder.Child = title;
            panel.Children.Add(titleBorder);
        }

        /// <summary>
        /// 绘制月图表
        /// </summary>
        private void DrawMonthlyChart()
        {
            //titleBorder.CornerRadius = new CornerRadius(0, 5, 5, 0);
            var containerBorder = new Border();
            containerBorder.Background = Project1.UI.Cores.Project1UIColor.Get("#4f6bed", .05);
            containerBorder.Padding = new Thickness(10);
            containerBorder.CornerRadius = new CornerRadius(2);

            //图表标题
            var chartTitleText = new TextBlock();
            chartTitleText.Text = "工作";
            chartTitleText.FontSize = 20;
            chartTitleText.FontWeight = FontWeights.Bold;
            chartTitleText.Margin = new Thickness(5, 0, 0, 0);
            chartTitleText.VerticalAlignment = VerticalAlignment.Center;

            var chartTitle = new StackPanel();
            chartTitle.Margin = new Thickness(0, 0, 0, 20);
            chartTitle.Orientation = Orientation.Horizontal;
            chartTitle.Children.Add(new Image()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Width = 20,
                Source = Project1.UI.Cores.BitmapImager.Load("pack://application:,,,/ProjectEye;component/Resources/desktop-computer.ico")
            });
            chartTitle.Children.Add(chartTitleText);
            //对比上个月
            var compare = new Project1.UI.Controls.CompareView();
            compare.DataA = lastMonthlyWork;
            compare.DataB = monthlyWork;
            compare.Margin = new Thickness(0, 0, 0, 20);
            //图表
            //var chart = new Project1.UI.Controls.ChartControl.Chart();
            chart.Data = chartData;
            chart.TickText = "{value} 小时";
            chart.IsAnimation = false;
            chart.Background = Brushes.Transparent;
            chart.ItemColor = Project1.UI.Cores.Project1UIColor.Get("#4f6bed");
            chart.Height = 360;
            containerBorder.Child = chart;

            panel.Children.Add(chartTitle);
            panel.Children.Add(compare);
            panel.Children.Add(containerBorder);
        }
        private void WriteFile(int width, int height)
        {
            RenderTargetBitmap rtp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtp.Render(canvas);
            JpegBitmapEncoder jpeg = new JpegBitmapEncoder();
            jpeg.QualityLevel = 100;
            jpeg.Frames.Add(BitmapFrame.Create(rtp));
            FileStream fs = new FileStream(savePath, FileMode.Create);
            jpeg.Save(fs);
            fs.Close();
            fs.Dispose();
            drawWindow.Close();
        }
        public void Generate()
        {
            Draw();
        }
    }
}
