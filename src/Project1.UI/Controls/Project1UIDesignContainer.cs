
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
            isControlPointDown = true;
            Debug.WriteLine("控制点点击");

        }

        private void Control_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
            olPoint = e.GetPosition(null);

            var control = sender as FrameworkElement;
            control.CaptureMouse();
            if (!isControlPointDown)
            {
                control.Cursor = Cursors.SizeAll;
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
            var control = sender as UIElement;
            if (isMouseDown && control != null && e.LeftButton == MouseButtonState.Pressed)
            {
                //控件的坐标信息
                var controlPoint = control.RenderTransform as TranslateTransform;
                //当前鼠标坐标信息
                var mousePoint = e.GetPosition(this);
                //鼠标在控件的坐标信息
                var mouseinControlPoint = e.GetPosition(control);

                Debug.WriteLine("控件的坐标:" + controlPoint.X + "," + controlPoint.Y);
                Debug.WriteLine("当前鼠标坐标:" + mousePoint.X + "," + mousePoint.Y);
                Debug.WriteLine("鼠标在控件的坐标:" + mouseinControlPoint.X + "," + mouseinControlPoint.Y);

                //最终移动坐标
                double movetoX = e.GetPosition(null).X - olPoint.X + controlPoint.X;
                double movetoY = e.GetPosition(null).Y - olPoint.Y + controlPoint.Y;

                controlPoint.X = movetoX;
                controlPoint.Y = movetoY;
                olPoint = e.GetPosition(null);
            }
        }




    }
}
