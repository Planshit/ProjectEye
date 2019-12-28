
using Project1.UI.Controls.Commands;
using Project1.UI.Controls.Enums;
using Project1.UI.Controls.Models;
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
using System.Windows.Media.Imaging;

namespace Project1.UI.Controls
{
    public class Project1UIDesignContainer : Grid
    {
        private bool isContextMenuOpen = false;
        private bool isItemContextMenuOpen = false;

        private bool isMouseDown = false;
        private bool isControlPointDown = false;
        private Point olPoint = new Point();
        private ControlPoint controlPointType = ControlPoint.LeftTop;
        public Project1UIDesignContainer()
        {
            this.DefaultStyleKey = typeof(Project1UIDesignContainer);
            this.Background = Brushes.White;
            Loaded += Project1UIDesignContainer_Loaded;
        }
        private void Project1UIDesignContainer_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.Children)
            {
                var control = item as Project1UIDesignItem;
                HandleItem(control);
            }
            this.ContextMenu = CreateContextMenu();
            this.MouseRightButtonUp += Project1UIDesignContainer_MouseRightButtonUp;
        }
        #region 处理元素
        private void HandleItem(Project1UIDesignItem item)
        {
            item.ControlPointMouseDown += Control_ControlPointMouseDown;
            item.ControlPointMouseUp += Control_ControlPointMouseUp;
            item.RenderTransform = new TranslateTransform()
            {
                X = 0,
                Y = 0
            };
            item.VerticalAlignment = VerticalAlignment.Top;
            item.HorizontalAlignment = HorizontalAlignment.Left;
            item.MouseLeftButtonUp += Control_MouseLeftButtonUp;
            item.MouseMove += Control_MouseMove;
            item.MouseLeave += Control_MouseLeave;
            item.MouseLeftButtonDown += Control_MouseLeftButtonDown;
            item.MouseRightButtonUp += Control_MouseRightButtonUp;
            item.ContextMenu = CreateItemContextMenu(item);
        }
        #endregion

        #region 创建元素右键菜单
        private ContextMenu CreateItemContextMenu(Project1UIDesignItem item)
        {
            var menu = new ContextMenu();
            var itemRemove = new MenuItem();
            var controlPoint = item.RenderTransform as TranslateTransform;
            itemRemove.Header = "删除此元素";
            itemRemove.Click += (s, e) =>
            {
                Children.Remove(item);
            };
            var itemHcenter = new MenuItem();
            itemHcenter.Header = "水平居中";
            itemHcenter.Click += (s, e) =>
            {
                controlPoint.X = this.ActualWidth / 2 - item.ActualWidth / 2;
            };
            var itemVcenter = new MenuItem();
            itemVcenter.Header = "垂直居中";
            itemVcenter.Click += (s, e) =>
            {
                controlPoint.Y = this.ActualHeight / 2 - item.ActualHeight / 2;
            };
            menu.Items.Add(itemRemove);
            menu.Items.Add(itemHcenter);
            menu.Items.Add(itemVcenter);


            return menu;
        }
        #endregion

        #region 创建容器右键菜单
        private ContextMenu CreateContextMenu()
        {
            var menu = new ContextMenu();
            var itemAdd = new MenuItem();
            itemAdd.Header = "添加元素";
            //图片
            var itemAddImage = new MenuItem();
            itemAddImage.Header = "图片";
            itemAddImage.Click += (s, e) =>
            {
                var item = new Project1UIDesignItem();
                var image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,,/Project1.UI;component/Assets/Images/sunglasses.png", UriKind.RelativeOrAbsolute));
                item.Content = image;
                HandleItem(item);
                this.Children.Add(item);
            };
            //按钮
            var itemAddButton = new MenuItem();
            itemAddButton.Header = "按钮";
            itemAddButton.Click += (s, e) =>
            {
                var item = new Project1UIDesignItem();
                var element = new Project1UIButton();
               
                item.Content = element;
                HandleItem(item);
                this.Children.Add(item);
            };
            itemAdd.Items.Add(itemAddImage);
            itemAdd.Items.Add(itemAddButton);

            menu.Items.Add(itemAdd);


            return menu;
        }
        #endregion

        #region 容器鼠标右键抬起
        private void Project1UIDesignContainer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isItemContextMenuOpen)
            {
                ContextMenu.IsOpen = true;

            }
        }
        #endregion

        #region 元素鼠标右键抬起
        private void Control_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Project1UIDesignItem;
            control.ContextMenu.IsOpen = true;
            isContextMenuOpen = true;
            isItemContextMenuOpen = true;
        }
        #endregion

        

        public void test()
        {
            foreach (var item in this.Children)
            {
                var control = item as Project1UIDesignItem;
                var data = control.DataContext as DesignItemModel;
                Debug.WriteLine(data);
            }
        }

        #region 元素边角控制点松开
        private void Control_ControlPointMouseUp(object sender, ControlPoint controlPoint)
        {
            isControlPointDown = false;
            Debug.WriteLine("控制点松开");

        }
        #endregion

        #region 元素边角控制点按下
        private void Control_ControlPointMouseDown(object sender, ControlPoint controlPoint)
        {
            controlPointType = controlPoint;
            isControlPointDown = true;
            Debug.WriteLine("控制点点击");

        }
        #endregion

        #region 鼠标离开元素区域
        private void Control_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }
        #endregion

        #region 鼠标坐标点击元素
        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            var control = sender as Project1UIDesignItem;

            //属性窗口没有显示时
            if (!control.IsAttPopupOpen && !isContextMenuOpen)
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
            if (isContextMenuOpen)
            {
                isContextMenuOpen = false;
            }

        }
        #endregion

        #region 鼠标在元素释放左键
        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
            var control = sender as FrameworkElement;
            control.ReleaseMouseCapture();
            control.Cursor = Cursors.Arrow;
        }
        #endregion

        #region 鼠标在元素上移动
        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            var control = sender as Project1UIDesignItem;
            if (isMouseDown
                && control != null
                && e.LeftButton == MouseButtonState.Pressed
                && !isContextMenuOpen)
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

                    if (reWidth < 10 || reHeight < 10)
                    {
                        //限制最小宽高
                        return;
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
        #endregion





    }
}
