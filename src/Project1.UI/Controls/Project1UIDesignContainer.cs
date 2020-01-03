
using Project1.UI.Controls.Commands;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Project1.UI.Controls
{
    public class Project1UIDesignContainer : Grid
    {
        #region 容器实例
        public Project1UIDesignContainer Instance
        {
            get { return (Project1UIDesignContainer)GetValue(InstanceProperty); }
            set { SetValue(InstanceProperty, value); }
        }
        public static readonly DependencyProperty InstanceProperty =
            DependencyProperty.Register("Instance", typeof(Project1UIDesignContainer), typeof(Project1UIDesignContainer));
        #endregion
        private bool isContextMenuOpen = false;
        private bool isItemContextMenuOpen = false;
        private Popup attrPopup;
        private bool isMouseDown = false;
        private bool isControlPointDown = false;
        private Point olPoint = new Point();
        private ControlPoint controlPointType = ControlPoint.LeftTop;
        public ContainerModel containerModel { get; set; }
        public Project1UIDesignContainer()
        {
            //this.DefaultStyleKey = typeof(Project1UIDesignContainer);
            this.Background = Brushes.Transparent;
            Loaded += Project1UIDesignContainer_Loaded;
            containerModel = new ContainerModel();
            containerModel.Background = Brushes.White;
            containerModel.Opacity = .8;

            Border container = new Border();
            container.Width = Double.NaN;
            container.Height = Double.NaN;
            container.SetValue(ZIndexProperty, -1);
            this.Children.Add(container);
            BindingOperations.SetBinding(container, BackgroundProperty, new Binding()
            {
                Source = containerModel,
                Path = new PropertyPath("Background"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

            });
            BindingOperations.SetBinding(container, OpacityProperty, new Binding()
            {
                Source = containerModel,
                Path = new PropertyPath("Opacity"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

            });
        }
        private void Project1UIDesignContainer_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            this.ContextMenu = CreateContextMenu();
            this.MouseRightButtonUp += Project1UIDesignContainer_MouseRightButtonUp;
            MouseLeftButtonDown += Project1UIDesignContainer_MouseLeftButtonDown;
            CreateAttrWindow();

        }

        private void Project1UIDesignContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            attrPopup.IsOpen = false;
        }

        #region 处理元素
        private void HandleItem(Project1UIDesignItem item)
        {
            item.ControlPointMouseDown += Control_ControlPointMouseDown;
            item.ControlPointMouseUp += Control_ControlPointMouseUp;

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
                var data = item.DataContext as DesignItemModel;
                data.Image = "pack://application:,,,/Project1.UI;component/Assets/Images/sunglasses.png";
                data.Width = 100;
                data.Height = 100;
                var image = new Image();
                //image.Source = new BitmapImage(new Uri("pack://application:,,,/Project1.UI;component/Assets/Images/sunglasses.png", UriKind.RelativeOrAbsolute));
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
                var data = item.DataContext as DesignItemModel;
                data.ButtonText = "按钮";
                data.Width = 100;
                data.Height = 25;
                var element = new Project1UIButton();

                item.Content = element;
                HandleItem(item);
                this.Children.Add(item);
            };
            //文本
            var itemAddText = new MenuItem();
            itemAddText.Header = "文本";
            itemAddText.Click += (s, e) =>
            {
                var item = new Project1UIDesignItem();
                var data = item.DataContext as DesignItemModel;
                data.Text = "文本";
                data.Width = 100;
                data.Height = 25;
                var element = new TextBlock();
                element.TextWrapping = TextWrapping.Wrap;
                item.Content = element;
                HandleItem(item);
                this.Children.Add(item);
            };
            //属性编辑
            var itemAttr = new MenuItem();
            itemAttr.Header = "属性";
            itemAttr.Click += (s, e) =>
            {
                attrPopup.IsOpen = true;
            };
            itemAdd.Items.Add(itemAddText);
            itemAdd.Items.Add(itemAddImage);
            itemAdd.Items.Add(itemAddButton);

            menu.Items.Add(itemAdd);
            menu.Items.Add(itemAttr);


            return menu;
        }
        #endregion

        #region 创建容器属性编辑窗口
        private void CreateAttrWindow()
        {
            attrPopup = new Popup();
            attrPopup.StaysOpen = false;
            attrPopup.SetValue(ZIndexProperty, 999);
            attrPopup.DataContext = containerModel;
            attrPopup.Placement = PlacementMode.MousePoint;
            attrPopup.Width = 259;
            attrPopup.Height = Double.NaN;
            attrPopup.AllowsTransparency = true;
            var border = new Border();
            border.Margin = new Thickness(5);
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Project1UIColor.Get("#ccc");
            border.Background = Project1UIColor.Get("#ffffff");
            border.Effect = new DropShadowEffect()
            {
                BlurRadius = 5,
                Opacity = .3,
                ShadowDepth = 0
            };
            var grid = new Grid();
            grid.Margin = new Thickness(10, 5, 10, 10);
            grid.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(30)
            });
            grid.RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            grid.Children.Add(new TextBlock()
            {
                Text = "窗口属性编辑面板",
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center
            });
            var stackpanel = new StackPanel();
            stackpanel.SetValue(Grid.RowProperty, 1);
            var stackPanelGrid = new Grid();
            stackPanelGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(80)
            });
            stackPanelGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            stackPanelGrid.RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            stackPanelGrid.RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            //添加透明度属性
            var opacityName = new TextBlock();
            opacityName.Text = "透明度";
            opacityName.VerticalAlignment = VerticalAlignment.Center;
            var opacityTextBox = new Project1UIInput();
            opacityTextBox.Height = 25;
            opacityTextBox.Width = Double.NaN;
            opacityTextBox.VerticalAlignment = VerticalAlignment.Center;
            opacityTextBox.SetValue(Grid.ColumnProperty, 1);
            BindingOperations.SetBinding(opacityTextBox, Project1UIInput.TextProperty, new Binding()
            {
                Path = new PropertyPath("Opacity"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

            });
            stackPanelGrid.Children.Add(opacityName);
            stackPanelGrid.Children.Add(opacityTextBox);
            //添加背景颜色属性
            var backgroundName = new TextBlock();
            backgroundName.Text = "背景颜色";
            backgroundName.Margin = new Thickness(0, 10, 0, 0);
            backgroundName.VerticalAlignment = VerticalAlignment.Center;
            backgroundName.SetValue(Grid.RowProperty, 1);
            var backgroundSelect = new Project1UIColorSelect();
            backgroundSelect.Width = 20;
            backgroundSelect.Height = 20;
            backgroundSelect.Margin = new Thickness(0, 10, 0, 0);

            backgroundSelect.HorizontalAlignment = HorizontalAlignment.Left;
            backgroundSelect.VerticalAlignment = VerticalAlignment.Center;
            backgroundSelect.SetValue(Grid.ColumnProperty, 1);
            backgroundSelect.SetValue(Grid.RowProperty, 1);

            BindingOperations.SetBinding(backgroundSelect, Project1UIColorSelect.ColorProperty, new Binding()
            {
                Path = new PropertyPath("Background"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

            });
            stackPanelGrid.Children.Add(backgroundName);
            stackPanelGrid.Children.Add(backgroundSelect);

            stackpanel.Children.Add(stackPanelGrid);
            grid.Children.Add(stackpanel);
            border.Child = grid;
            attrPopup.Child = border;
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

        #region 设置容器属性
        public void SetContainerAttr(ContainerModel data)
        {
            containerModel.Background = data.Background;
            containerModel.Opacity = data.Opacity;
        }
        #endregion

        #region 获取容器的属性信息
        public ContainerModel GetContainerAttr()
        {
            return this.containerModel;
        }
        #endregion

        #region 获取容器中所有元素信息
        public List<ElementModel> GetElements()
        {
            var res = new List<ElementModel>();
            foreach (var item in this.Children)
            {
                var control = item as Project1UIDesignItem;
                if (control != null)
                {
                    var data = control.DataContext as DesignItemModel;
                    var controlPoint = control.RenderTransform as TranslateTransform;
                    var element = new ElementModel();
                    element.Type = control.ItemType;
                    element.X = controlPoint.X;
                    element.Y = controlPoint.Y;
                    element.Width = data.Width;
                    element.Height = data.Height;
                    element.IsTextBold = data.IsFontBold;
                    element.Opacity = data.Opacity;
                    element.Style = data.ButtonStyleName;
                    element.Image = data.Image;
                    element.TextColor = data.TextColor;
                    element.FontSize = data.FontSize;
                    switch (control.ItemType)
                    {
                        case DesignItemType.Text:
                            element.Text = data.Text;
                            break;
                        case DesignItemType.Button:
                            element.Text = data.ButtonText;
                            element.Command = data.Command;
                            break;
                    }
                    res.Add(element);
                }

            }
            return res;
        }
        #endregion

        #region 加载元素信息
        public void ImportElements(List<ElementModel> elements)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    var item = new Project1UIDesignItem();
                    var itemModel = item.DataContext as DesignItemModel;
                    var controlPoint = item.RenderTransform as TranslateTransform;
                    //分配共用的属性
                    itemModel.Width = element.Width;
                    itemModel.Height = element.Height;
                    itemModel.Opacity = element.Opacity;
                    controlPoint.X = element.X;
                    controlPoint.Y = element.Y;
                    //分配独有的属性
                    switch (element.Type)
                    {
                        case DesignItemType.Button:
                            var button = new Project1UIButton();
                            if (element.Style != null)
                            {
                                var res = this.TryFindResource(element.Style);
                                if (res != null)
                                {
                                    button.Style = res as Style;
                                }
                                else
                                {
                                    res = this.TryFindResource("default");
                                    if (res != null)
                                    {
                                        button.Style = res as Style;
                                    }
                                }
                            }
                            itemModel.ButtonText = element.Text;
                            itemModel.ButtonStyleName = element.Style;
                            itemModel.IsFontBold = element.IsTextBold;
                            itemModel.FontSize = element.FontSize;
                            itemModel.Command = element.Command;
                            item.Content = button;
                            break;
                        case DesignItemType.Image:
                            var image = new Image();
                            itemModel.Image = element.Image;
                            item.Content = image;
                            break;
                        case DesignItemType.Text:
                            var text = new TextBlock();
                            itemModel.Text = element.Text;
                            itemModel.TextColor = element.TextColor;
                            itemModel.IsFontBold = element.IsTextBold;
                            itemModel.FontSize = element.FontSize;
                            item.Content = text;
                            break;

                    }
                    //将生成的元素加入设计容器界面
                    HandleItem(item);
                    this.Children.Add(item);
                }
            }
        }
        #endregion

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
            attrPopup.IsOpen = false;

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
