using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Project1.UI.Controls
{
    /// <summary>
    /// 图表控件
    /// ** 写得比较急，很多细节没处理好，等把v1.0.4的todolist完成再回来优化（2019年5月25日留
    /// </summary>
    public class Project1UIChart : Control
    {
        /// <summary>
        /// 图表数据
        /// </summary>
        public double[] Data
        {
            get { return (double[])GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data",
                typeof(double[]),
                typeof(Project1UIChart),
                new PropertyMetadata(null, new PropertyChangedCallback(OnDataPropertyChanged))
                );

        private static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (d as Project1UIChart);
            if (chart != null)
            {
                chart.InvalidateVisual();
            }

        }
        /// <summary>
        /// Y轴最大值 
        /// </summary>
        private double MaxValue;
        /// <summary>
        /// 行数
        /// </summary>
        private int Line;
        /// <summary>
        /// 间隔值
        /// </summary>
        private double Interval;
        /// <summary>
        /// 数据长度
        /// </summary>
        private int DataLength;
        /// <summary>
        /// 刻度值
        /// </summary>
        private double[] Tick;
        /// <summary>
        /// X轴偏移量（真实宽度）
        /// </summary>
        private double X_Offset;
        /// <summary>
        /// Y轴偏移量（真实宽度）
        /// </summary>
        private double Y_Offset;
        /// <summary>
        /// 数据标记点坐标
        /// </summary>
        private Point[] DataPoints;

        public Project1UIChart()
        {
            DefaultStyleKey = typeof(Project1UIChart);
            //Data = new double[] { 0, 10, 24, 100 };
            Data = new double[] { 10, 20, 30, 80, 15, 37, 120 };

            CalculateChartValue();
            //Loaded += (e, c) =>
            //{
            //    //InvalidateVisual();
            //};

            //测试绘图数据
            //Data = new double[] { 0, 1, 2, 4 };
            //CalculateLine();
            //Data = new double[] { 0, 1, 2, 29 };
            //CalculateLine();
            //Data = new double[] { 0, 1, 2, 300 };
            //CalculateLine();
            //Data = new double[] { 0, 1, 2, 23 };
            //CalculateLine();
            //Data = new double[] { 0, 1, 2, 37 };
            //CalculateLine();
            //Data = new double[] { 0, 1, 2, 400 };
            //CalculateLine();
            //Data = new double[] { 0, 1, 2, 2900 };
            //CalculateLine();
            //Data = new double[] { 0, 1, 2, 300 };
            //CalculateLine();
            //Data = new double[] { 0, 1, 2, 23 };
            //CalculateLine();
            //Data = new double[] { 0, 1, 2, 37 };
            //CalculateLine();
        }


        protected override void OnRender(DrawingContext drawingContext)
        {

            DrawBasicGrid(drawingContext);
            DrawDataGrid(drawingContext);
            DrawDataPoint(drawingContext);
            DrawDataPointLinkLine(drawingContext);

        }
        #region 绘制文字函数

        public void DrawText(DrawingContext dc, string text, Point point)
        {
           
            dc.DrawText(
               new FormattedText(text,
                  CultureInfo.GetCultureInfo("en-us"),
                  FlowDirection.LeftToRight,
                  new Typeface("Verdana"),
                  12, System.Windows.Media.Brushes.Black),
                 point);

           
        }
        #endregion

        #region 绘制点函数
        private void DrawPoint(DrawingContext dc, Point point, string color = "#000000")
        {
            //Pen _pen = new Pen();
            //_pen.Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cecece"));
            //_pen.Thickness = 1;

            dc.DrawEllipse(new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)), null, point, 5, 5);

        }
        #endregion
        #region 绘线函数
        public void DrawSnappedLinesBetweenPoints(DrawingContext dc,
        Pen pen, double lineThickness, params Point[] points)
        {
            var guidelineSet = new GuidelineSet();
            foreach (var point in points)
            {
                guidelineSet.GuidelinesX.Add(point.X);
                guidelineSet.GuidelinesY.Add(point.Y);
            }
            var half = lineThickness / 2;
            points = points.Select(p => new Point(p.X + half, p.Y + half)).ToArray();
            dc.PushGuidelineSet(guidelineSet);
            for (var i = 0; i < points.Length - 1; i = i + 2)
            {
                dc.DrawLine(pen, points[i], points[i + 1]);
            }
            dc.Pop();
        }
        private void Draw(DrawingContext dc, Point start, Point end, string color = "#dad9d9")
        {
            Pen _pen = new Pen();
            _pen.Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            _pen.Thickness = 1;

            DrawSnappedLinesBetweenPoints(dc, _pen, 1, new[]
            {
                start,
                end,
            });

        }
        #endregion

        //1.
        #region 绘制基础网格线条
        /// <summary>
        /// 绘制基础网格线条
        /// </summary>
        /// <param name="dc"></param>
        private void DrawBasicGrid(DrawingContext dc)
        {

            var height = this.ActualHeight;
            var width = this.ActualWidth;
            //预留空间宽度
            double XValueWidth = width * 0.05;
            double YValueWidth = height * 0.05;

            //偏移量
            X_Offset = width * .1;
            Y_Offset = height * .1;



            //绘制剩余网格线
            //高度间隔值
            double IntervalHieght = (height - Y_Offset) / Line;
            for (int i = 0; i < Line; i++)
            {
                var XStartPoint = new Point()
                {
                    X = XValueWidth,
                    Y = IntervalHieght * i
                };
                var XEndPoint = new Point()
                {
                    X = width,
                    Y = IntervalHieght * i
                };
                Draw(dc, XStartPoint, XEndPoint);
                var textpoint = new Point()
                {
                    X = 0,
                    Y = IntervalHieght * i
                };
                //绘制刻度值
                DrawText(dc, Tick[Line - (i + 1)].ToString(), textpoint);
            }
            //绘制刻度值0
            var zeroPoint = new Point()
            {
                X = 0,
                Y = height - Y_Offset
            };
            DrawText(dc, "0", zeroPoint);


            //绘制X轴和Y轴
            var X_Start = new Point()
            {
                X = XValueWidth,
                Y = height - Y_Offset
            };
            var X_End = new Point()
            {
                X = width,
                Y = height - Y_Offset
            };


            var Y_Start = new Point()
            {
                X = X_Offset,
                Y = 0
            };
            var Y_End = new Point()
            {
                X = X_Offset,
                Y = height - YValueWidth
            };

            var Y2_Start = new Point()
            {
                X = width,
                Y = 0
            };
            var Y2_End = new Point()
            {
                X = width,
                Y = height - YValueWidth
            };

            Draw(dc, Y_Start, Y_End, "#b3b3b3");
            Draw(dc, X_Start, X_End, "#b3b3b3");
            Draw(dc, Y2_Start, Y2_End);
        }
        #endregion

        //2.
        #region 绘制数据网格线
        private void DrawDataGrid(DrawingContext dc)
        {
            var height = this.ActualHeight;
            var width = this.ActualWidth;
            double YValueWidth = height * 0.05;

            //网格宽度（真正的部分）
            double GridWidth = width - X_Offset;
            //数据间隔宽度值
            double DataInterval = GridWidth / (DataLength - 1);
            for (int i = 0; i < DataLength; i++)
            {
                var dataTextPoint = new Point()
                {
                    X = X_Offset + ((i) * DataInterval) - 10,
                    Y = height - YValueWidth
                };
                DrawText(dc, Data[i].ToString(), dataTextPoint);
                if (i > 0 && i < (DataLength - 1))
                {
                    var Start = new Point()
                    {
                        X = X_Offset + ((i) * DataInterval),
                        Y = 0
                    };
                    var End = new Point()
                    {
                        X = X_Offset + ((i) * DataInterval),
                        Y = height - YValueWidth
                    };
                    Draw(dc, Start, End);
                }
            }
        }
        #endregion

        //3.
        #region 绘制数据标记点
        private void DrawDataPoint(DrawingContext dc)
        {
            var height = this.ActualHeight;
            var width = this.ActualWidth;
            //网格宽度（真正的部分）
            double GridWidth = width - X_Offset;
            //数据间隔宽度值
            double DataInterval = GridWidth / (DataLength - 1);

            //网格的真实高度（数据区域）
            double GridHeight = height - Y_Offset;
            //计算数值占用的高度
            double Value = GridHeight / MaxValue;

            DataPoints = new Point[DataLength];
            for (int i = 0; i < DataLength; i++)
            {
                double x = X_Offset + (i * DataInterval);
                double y = GridHeight - Value * Data[i];

                if (i == 0)
                {
                    //0坐标
                    x = X_Offset;
                }
                if (i == (DataLength - 1))
                {
                    //最末坐标
                    x = width;
                }
                var dataPoint = new Point()
                {
                    X = x,
                    Y = y
                };
                DataPoints[i] = dataPoint;

                DrawPoint(dc, dataPoint, UIDefaultSetting.DefaultThemeColor);
            }
        }
        #endregion

        //4.
        #region 绘制数据标记点连接线
        private void DrawDataPointLinkLine(DrawingContext dc)
        {
            for (int i = 0; i < DataLength; i++)
            {
                if (i + 1 < DataLength)
                {
                    var startPoint = DataPoints[i];
                    var endPoint = DataPoints[i + 1];
                    Draw(dc, startPoint, endPoint, UIDefaultSetting.DefaultThemeColor);
                }
            }
        }
        #endregion

        #region 计算一些图表值
        /// <summary>
        /// 计算一些图表值
        /// </summary>
        private void CalculateChartValue()
        {
            double max = Data.Last();
            //将最大值转为偶数
            max = Math.Round(max / 2, MidpointRounding.AwayFromZero) * 2;
            DataLength = Data.Length;
            //取得一个间隔值（百分之0.1）
            double interval = Math.Ceiling(max * 0.1);
            //获得行数
            double line = Math.Ceiling(max / interval);
            //计算最终间隔值
            double realInterval = max / line;
            if (interval - realInterval > 0)
            {
                max = max + ((interval - realInterval) * line);
                realInterval = max / line;
            }
            MaxValue = max;
            Line = (int)line;
            Interval = realInterval;
            //Debug.WriteLine($"最大值：{max}，间隔值：{interval}，行数：{line}，最终间隔值：{realInterval}");
            Tick = new double[(int)line];
            for (int i = 0; i < line; i++)
            {
                Tick[i] = (i + 1) * realInterval;
            }
        }
        #endregion
    }
}
