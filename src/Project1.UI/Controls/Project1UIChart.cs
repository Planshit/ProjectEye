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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Project1.UI.Controls
{
    /// <summary>
    /// 图表控件
    /// </summary>
    public class Project1UIChart : Control
    {
        #region 依赖属性

        #region 图表数据
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
                //重绘图表
                chart.InvalidateVisual();
            }

        }
        #endregion

        #region 图表标签
        /// <summary>
        /// 图表标签
        /// </summary>
        public string[] Labels
        {
            get { return (string[])GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }
        public static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels",
                typeof(string[]),
                typeof(Project1UIChart),
                new PropertyMetadata(null, new PropertyChangedCallback(OnDataPropertyChanged))
                );

        #endregion
        #endregion

        public string Label { get; set; }

        public double MaxData { get; set; } = 0;

        #region 私有属性
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
        /// <summary>
        /// 画布
        /// </summary>
        private Canvas rootCanvas;
        /// <summary>
        /// X轴起点坐标
        /// </summary>
        private Point XStartPoint;
        /// <summary>
        /// X轴终点坐标
        /// </summary>
        private Point XEndPoint;

        #endregion

        public Project1UIChart()
        {
            DefaultStyleKey = typeof(Project1UIChart);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            rootCanvas = GetTemplateChild("root") as Canvas;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //擦除画板
            if (rootCanvas != null)
            {
                rootCanvas.Children.Clear();
            }
            //计算图表数据
            CalculateChartValue();
            DrawBasicGrid(drawingContext);
            DrawDataGrid(drawingContext);
            DrawColorBlock();
            DrawDataPoint(drawingContext);
            DrawDataPointLinkLine(drawingContext);

        }

        #region 绘制文字函数

        public void DrawText(DrawingContext dc, string text, Point point)
        {

            dc.DrawText(
               new FormattedText(text,
                  CultureInfo.CurrentCulture,
                  FlowDirection.LeftToRight,
                  new Typeface("Verdana"),
                  12,
                  new SolidColorBrush((Color)ColorConverter.ConvertFromString("#464646"))),
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
        private void Draw(DrawingContext dc, Point start, Point end, string color = "#dad9d9", double thickness = 1)
        {
            Pen _pen = new Pen();
            _pen.Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            _pen.Thickness = thickness;

            DrawSnappedLinesBetweenPoints(dc, _pen, 1, new[]
            {
                start,
                end,
            });

        }
        #endregion

        //0.
        #region 计算一些图表值
        /// <summary>
        /// 计算一些图表值
        /// </summary>
        private void CalculateChartValue()
        {
            if (Data == null || Data.Length == 0)
            {
                Data = new double[] { 0 };
            }
            double max = Data.Max();
            if (max < 10)
            {
                max = 10;
            }
            if (MaxData != 0)
            {
                max = MaxData;
            }
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


            //计算数据点坐标
            DataPoints = new Point[DataLength];
            var height = this.ActualHeight;
            var width = this.ActualWidth;
            //偏移量
            X_Offset = width * .1;
            Y_Offset = height * .1;

            //网格宽度（真正的部分）
            double GridWidth = width - X_Offset;
            //数据间隔宽度值
            double DataInterval = GridWidth / (DataLength - 1);

            //网格的真实高度（数据区域）
            double GridHeight = height - Y_Offset;
            //计算数值占用的高度
            double Value = GridHeight / MaxValue;
            if (DataLength == 1)
            {
                //只有一个数据时
                double x = X_Offset;
                double y = GridHeight - Value * Data[0];
                var dataPoint = new Point()
                {
                    X = x,
                    Y = y
                };
                DataPoints[0] = dataPoint;
            }
            else
            {
                //超过一个数据时
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
                    //记录数据点坐标
                    DataPoints[i] = dataPoint;
                }
            }
        }
        #endregion

        //1.
        #region 绘制基础网格线条和Y轴刻度文字
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
                    Y = IntervalHieght * i - 5
                };
                //绘制Y轴刻度值
                DrawText(dc, Tick[Line - (i + 1)].ToString(), textpoint);
            }
            //绘制Y轴刻度值0
            var zeroPoint = new Point()
            {
                X = 0,
                Y = height - Y_Offset - 5
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

            XStartPoint = new Point()
            {
                X = X_Offset,
                Y = height - Y_Offset
            };
            XEndPoint = X_End;
        }
        #endregion

        //2.
        #region 绘制数据网格线和X轴标签文字
        private void DrawDataGrid(DrawingContext dc)
        {
            var height = this.ActualHeight;
            var width = this.ActualWidth;
            double YValueWidth = height * 0.05;

            //网格宽度（真正的部分）
            double GridWidth = width - X_Offset;
            //数据间隔宽度值
            double DataInterval = GridWidth / (DataLength - 1);

            if (Data.Length == 1)
            {
                //只有一个数据时
                var dataTextPoint = new Point()
                {
                    X = X_Offset - 10,
                    Y = height - YValueWidth
                };
                //绘制X轴标签
                if (Labels != null && Labels.Length == Data.Length)
                {
                    //设置了标签则绘制标签文字
                    DrawText(dc, Labels[0], dataTextPoint);
                }
                else
                {
                    //否则绘制数据
                    DrawText(dc, Data[0].ToString(), dataTextPoint);
                }
            }
            else
            {
                for (int i = 0; i < DataLength; i++)
                {
                    //绘制X轴标签文字
                    var dataTextPoint = new Point()
                    {
                        X = X_Offset + ((i) * DataInterval) - 10,
                        Y = height - YValueWidth
                    };
                    //绘制X轴标签
                    if (Labels != null && Labels.Length == Data.Length)
                    {
                        //设置了标签则绘制标签文字
                        DrawText(dc, Labels[i], dataTextPoint);
                    }
                    else
                    {
                        //否则绘制数据
                        DrawText(dc, Data[i].ToString(), dataTextPoint);
                    }

                    //绘制Y轴网格线
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
        }
        #endregion

        //3.
        #region 绘制渐变层
        private void DrawColorBlock()
        {
            if (DataPoints.Length > 1)
            {
                var block = new Path();
                var brush = new LinearGradientBrush();
                string startcolor = UIDefaultSetting.DefaultThemeColor;
                string endcolor = UIDefaultSetting.DefaultThemeColor;
                if (UIDefaultSetting.DefaultThemeColor.Length == 9)
                {
                    startcolor = "#B3" + startcolor.Substring(3);
                    endcolor = "#29" + endcolor.Substring(3);

                }
                else
                {
                    startcolor = startcolor.Replace("#", "#B3");
                    endcolor = endcolor.Replace("#", "#29");

                }

                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                brush.GradientStops.Add(new GradientStop()
                {
                    Color = (Color)ColorConverter.ConvertFromString(startcolor),
                    Offset = 0
                });

                brush.GradientStops.Add(new GradientStop()
                {
                    Color = (Color)ColorConverter.ConvertFromString(endcolor),
                    Offset = 1,
                });
                block.Fill = brush;
                string source = $"M{DataPoints[0].X},{DataPoints[0].Y}";
                for (int i = 1; i < DataPoints.Length; i++)
                {
                    source += $" L{DataPoints[i].X},{DataPoints[i].Y}";
                }
                source += $" L{XEndPoint.X},{XEndPoint.Y} L{XStartPoint.X},{XStartPoint.Y}";
                block.Data = Geometry.Parse(source);
                if (rootCanvas != null)
                {
                    rootCanvas.Children.Add(block);
                }
            }
        }
        #endregion

        //4.
        #region 绘制数据标记点
        private void DrawDataPoint(DrawingContext dc)
        {
            for (int i = 0; i < DataLength; i++)
            {
                CreateDataPoint(DataPoints[i], Data[i]);
            }
        }

        #region 创建数据点
        private void CreateDataPoint(Point point, double data)
        {
            if (rootCanvas != null)
            {
                var dataPoint = new Ellipse();

                dataPoint.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(UIDefaultSetting.DefaultThemeColor));

                dataPoint.Width = 8;
                dataPoint.Height = 8;

                //中心位置
                double centerX = point.X - (dataPoint.Width / 2);
                double centerY = point.Y - (dataPoint.Height / 2);

                //坐标对象
                var translateTransform = new TranslateTransform();
                translateTransform.X = centerX;
                translateTransform.Y = centerY;

                //缩放对象
                var scaleTransform = new ScaleTransform();
                scaleTransform.ScaleX = 1;
                scaleTransform.ScaleY = 1;
                scaleTransform.CenterX = centerX + (dataPoint.Width / 2);
                scaleTransform.CenterY = centerY + (dataPoint.Width / 2);

                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(translateTransform);
                transformGroup.Children.Add(scaleTransform);

                //动画
                var storyboard = new Storyboard();

                storyboard.Completed += (e, c) =>
                {
                    Debug.WriteLine("complete");
                };
                var doubleAnimation = new DoubleAnimation();
                doubleAnimation.To = 1.5;
                doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(.1));
                var doubleAnimation2 = new DoubleAnimation();
                doubleAnimation2.To = 1.5;
                doubleAnimation2.Duration = new Duration(TimeSpan.FromSeconds(.1));
                var doubleAnimation_Opcatify = new DoubleAnimation();
                doubleAnimation_Opcatify.To = .7;
                doubleAnimation_Opcatify.Duration = new Duration(TimeSpan.FromSeconds(.1));

                dataPoint.RenderTransform = transformGroup;
                dataPoint.Cursor = Cursors.Hand;
                dataPoint.ToolTip = data + " " + Label;

                //设置动画对象
                Storyboard.SetTarget(doubleAnimation, dataPoint);
                Storyboard.SetTarget(doubleAnimation2, dataPoint);
                Storyboard.SetTarget(doubleAnimation_Opcatify, dataPoint);

                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("RenderTransform.Children[1].ScaleX"));
                Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("RenderTransform.Children[1].ScaleY"));
                Storyboard.SetTargetProperty(doubleAnimation_Opcatify, new PropertyPath("Opacity"));
                //鼠标滑入滑出时处理动画播放停止
                dataPoint.MouseEnter += (e, c) =>
                {
                    storyboard.Begin();
                };
                dataPoint.MouseLeave += (e, c) =>
                {
                    storyboard.Stop();
                };

                //添加动画到故事板
                storyboard.Children.Add(doubleAnimation);
                storyboard.Children.Add(doubleAnimation2);
                storyboard.Children.Add(doubleAnimation_Opcatify);

                rootCanvas.Children.Add(dataPoint);

            }
        }


        #endregion
        #endregion

        //5.
        #region 绘制数据标记点连接线
        private void DrawDataPointLinkLine(DrawingContext dc)
        {
            for (int i = 0; i < DataLength; i++)
            {
                if (i + 1 < DataLength)
                {
                    var startPoint = DataPoints[i];
                    var endPoint = DataPoints[i + 1];
                    Draw(dc, startPoint, endPoint, UIDefaultSetting.DefaultThemeColor,2);
                }
            }
        }
        #endregion

    }
}
