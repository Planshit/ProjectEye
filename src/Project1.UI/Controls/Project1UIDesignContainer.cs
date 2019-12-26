
using Project1.UI.Controls.Commands;
using Project1.UI.Controls.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Project1.UI.Controls
{
    public class Project1UIDesignContainer : Grid
    {

        private bool isMouseDown = false;
        private bool isControlPointDown = false;
        private Point olPoint = new Point();
        private ControlPoint controlPointType = ControlPoint.LeftTop;
        public Project1UIDesignContainer()
        {
            this.DefaultStyleKey = typeof(Project1UIDesignContainer);
            Loaded += Project1UIDesignContainer_Loaded;
        }

        private void Project1UIDesignContainer_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.Children)
            {
                var control = item as Project1UIDesignItem;
                control.ControlPointMouseDown += Control_ControlPointMouseDown;
                control.ControlPointMouseUp += Control_ControlPointMouseUp;
                control.RenderTransform = new TranslateTransform()
                {
                    X = 0,
                    Y = 0
                };
                control.VerticalAlignment = VerticalAlignment.Top;
                control.HorizontalAlignment = HorizontalAlignment.Left;
                control.MouseLeftButtonUp += Control_MouseLeftButtonUp;
                control.MouseMove += Control_MouseMove;
                control.MouseLeave += Control_MouseLeave;
                control.MouseLeftButtonDown += Control_MouseLeftButtonDown;
            }

        }

        private void Control_ControlPointMouseUp(object sender, ControlPoint controlPoint)
        {
            isControlPointDown = false;
            Debug.WriteLine("控制点松开");

        }

        private void Control_ControlPointMouseDown(object sender, ControlPoint controlPoint)
        {
            controlPointType = controlPoint;
            isControlPointDown = true;
            Debug.WriteLine("控制点点击");

        }

        private void Control_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Project1UIDesignItem;
            //属性窗口没有显示时
            if (!control.IsAttPopupOpen)
            {
                isMouseDown = true;
                olPoint = e.GetPosition(null);


                control.CaptureMouse();
                if (!isControlPointDown)
                {
                    control.Cursor = Cursors.SizeAll;
                }
                else
                {
                    if (controlPointType == ControlPoint.LeftBottom || controlPointType == ControlPoint.RightTop)
                    {
                        control.Cursor = Cursors.SizeNESW;
                    }
                    else
                    {
                        control.Cursor = Cursors.SizeNWSE;

                    }
                }
            }

        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
            var control = sender as FrameworkElement;
            control.ReleaseMouseCapture();
            control.Cursor = Cursors.Arrow;
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            var control = sender as Project1UIDesignItem;
            if (isMouseDown && control != null && e.LeftButton == MouseButtonState.Pressed)
            {
                //控件的坐标信息
                var controlPoint = control.RenderTransform as TranslateTransform;
                //当前鼠标坐标信息
                var mousePoint = e.GetPosition(this);
                //鼠标在控件的坐标信息
                var mouseinControlPoint = e.GetPosition(control);


                //最终移动坐标
                double movetoX = e.GetPosition(null).X - olPoint.X + controlPoint.X;
                double movetoY = e.GetPosition(null).Y - olPoint.Y + controlPoint.Y;

                if (isControlPointDown)
                {
                    double reWidth = control.ActualWidth;
                    double reHeight = control.ActualHeight;
                    double reX = controlPoint.X;
                    double reY = controlPoint.Y;
                    //拖动控制点
                    if (movetoX != controlPoint.X)
                    {
                        reWidth = control.ActualWidth + (movetoX - controlPoint.X);
                    }
                    if (movetoY != controlPoint.Y)
                    {
                        reHeight = control.ActualHeight + (movetoY - controlPoint.Y);
                    }

                    if (controlPointType != ControlPoint.RightBottom)
                    {

                        switch (controlPointType)
                        {
                            //拖动左上角控制点
                            case ControlPoint.LeftTop:
                                //拉伸时
                                reWidth = control.ActualWidth + Math.Abs(movetoX - controlPoint.X);
                                reHeight = control.ActualHeight + Math.Abs(movetoY - controlPoint.Y);
                                //缩小时
                                if (movetoX > controlPoint.X)
                                {
                                    reWidth = control.ActualWidth - Math.Abs(movetoX - controlPoint.X);
                                }
                                if (movetoY > controlPoint.Y)
                                {
                                    reHeight = control.ActualHeight - Math.Abs(movetoY - controlPoint.Y);
                                }
                                //重新移动坐标
                                reX = movetoX;
                                reY = movetoY;
                                break;
                            //右上角
                            case ControlPoint.RightTop:
                                reHeight = control.ActualHeight - (movetoY - controlPoint.Y);
                                reY = movetoY;
                                break;
                            //左下角
                            case ControlPoint.LeftBottom:
                                reWidth = control.ActualWidth - (movetoX - controlPoint.X);
                                reX = movetoX;
                                break;
                        }

                    }

                    control.Width = reWidth;
                    control.Height = reHeight;
                    controlPoint.X = reX;
                    controlPoint.Y = reY;
                }
                else
                {
                    //移动项
                    controlPoint.X = movetoX;
                    controlPoint.Y = movetoY;
                }



                olPoint = e.GetPosition(null);
            }
        }




    }
}
