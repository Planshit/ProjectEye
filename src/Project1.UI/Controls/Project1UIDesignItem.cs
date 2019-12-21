using Project1.UI.Controls.Enums;
using Project1.UI.Controls.Models;
using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Project1.UI.Controls
{
    public class Project1UIDesignItem : ContentControl
    {
        public delegate void ControlPointEventHandler(object sender, ControlPoint controlPoint);
        public event ControlPointEventHandler ControlPointMouseDown;
        public event ControlPointEventHandler ControlPointMouseUp;

        private DesignItemModel designItemModel;
        public Project1UIDesignItem()
        {
            this.DefaultStyleKey = typeof(Project1UIDesignItem);
            designItemModel = new DesignItemModel();
            designItemModel.ControlPointVisibility = Visibility.Hidden;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var container = GetTemplateChild("Container") as Grid;
            if (container != null)
            {
                CreateControlPoints(container);
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            designItemModel.ControlPointVisibility = Visibility.Visible;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            designItemModel.ControlPointVisibility = Visibility.Hidden;

        }
        private void CreateControlPoints(Grid grid)
        {
            var leftTop = CreateControlPoint(ControlPoint.LeftTop);
            var leftBottom = CreateControlPoint(ControlPoint.LeftBottom);
            var rightTop = CreateControlPoint(ControlPoint.RightTop);
            var rightBottom = CreateControlPoint(ControlPoint.RightBottom);
            grid.Children.Add(leftTop);
            grid.Children.Add(leftBottom);
            grid.Children.Add(rightTop);
            grid.Children.Add(rightBottom);

        }
        private Border CreateControlPoint(ControlPoint controlPoint)
        {
            var point = new Border();

            point.MouseDown += (s, e) =>
            {
                point.CaptureMouse();
                ControlPointMouseDown?.Invoke(this, controlPoint);
            };
            point.MouseUp += (s, e) =>
            {
                point.ReleaseMouseCapture();
                ControlPointMouseUp?.Invoke(this, controlPoint);
            };
            //point.MouseLeave += (s, e) =>
            //{
            //    ControlPointMouseUp?.Invoke(this, controlPoint);
            //};
            Binding binding = new Binding()
            {
                Source = designItemModel,
                Path = new PropertyPath("ControlPointVisibility"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(point, VisibilityProperty, binding);
            point.Width = 10;
            point.Height = 10;
            point.CornerRadius = new System.Windows.CornerRadius()
            {
                TopLeft = 10,
                TopRight = 10,
                BottomLeft = 10,
                BottomRight = 10
            };
            point.SetCurrentValue(Panel.ZIndexProperty, 2);
            point.Background = Project1UIColor.Get("#6495ED");

            var cursorEnter = Cursors.Hand;
            switch (controlPoint)
            {
                case ControlPoint.LeftTop:
                    cursorEnter = Cursors.SizeNWSE;
                    point.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    point.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    point.Margin = new System.Windows.Thickness(-5, -5, 0, 0);
                    break;
                case ControlPoint.RightTop:
                    cursorEnter = Cursors.SizeNESW;
                    point.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    point.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    point.Margin = new System.Windows.Thickness(0, -5, -5, 0);
                    break;
                case ControlPoint.LeftBottom:
                    cursorEnter = Cursors.SizeNESW;

                    point.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    point.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    point.Margin = new System.Windows.Thickness(-5, 0, 0, -5);
                    break;
                case ControlPoint.RightBottom:
                    cursorEnter = Cursors.SizeNWSE;

                    point.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    point.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    point.Margin = new System.Windows.Thickness(0, 0, -5, -5);
                    break;
            }
            point.Cursor = cursorEnter;

            return point;
        }

    }
}
