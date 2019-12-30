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

namespace Project1.UI.Controls
{
    public class Project1UIDesignItem : ContentControl
    {
        #region 依赖属性

        #region 元素类型
        public DesignItemType ItemType
        {
            get { return (DesignItemType)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }
        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof(DesignItemType), typeof(Project1UIDesignItem));
        #endregion

        #region 元素类型名称
        public string ItemTypeName
        {
            get { return (string)GetValue(ItemTypeNameProperty); }
            set { SetValue(ItemTypeNameProperty, value); }
        }
        public static readonly DependencyProperty ItemTypeNameProperty =
            DependencyProperty.Register("ItemTypeName", typeof(string), typeof(Project1UIDesignItem));
        #endregion

        #region 属性编辑文字输入可见性
        public Visibility TextInputVisibility
        {
            get { return (Visibility)GetValue(TextInputVisibilityProperty); }
            set { SetValue(TextInputVisibilityProperty, value); }
        }
        public static readonly DependencyProperty TextInputVisibilityProperty =
            DependencyProperty.Register("TextInputVisibility", typeof(Visibility), typeof(Project1UIDesignItem), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region 属性编辑图片输入可见性
        public Visibility ImageInputVisibility
        {
            get { return (Visibility)GetValue(ImageInputVisibilityProperty); }
            set { SetValue(ImageInputVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ImageInputVisibilityProperty =
            DependencyProperty.Register("ImageInputVisibility", typeof(Visibility), typeof(Project1UIDesignItem), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region 属性编辑背景和边宽颜色可见性
        public Visibility ColorVisibility
        {
            get { return (Visibility)GetValue(ColorVisibilityProperty); }
            set { SetValue(ColorVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ColorVisibilityProperty =
            DependencyProperty.Register("ColorVisibility", typeof(Visibility), typeof(Project1UIDesignItem), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region 属性编辑按钮可见性
        public Visibility ButtonVisibility
        {
            get { return (Visibility)GetValue(ButtonVisibilityProperty); }
            set { SetValue(ButtonVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ButtonVisibilityProperty =
            DependencyProperty.Register("ButtonVisibility", typeof(Visibility), typeof(Project1UIDesignItem), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region 属性编辑文本大小可见性
        public Visibility FontSizeVisibility
        {
            get { return (Visibility)GetValue(FontSizeVisibilityProperty); }
            set { SetValue(FontSizeVisibilityProperty, value); }
        }
        public static readonly DependencyProperty FontSizeVisibilityProperty =
            DependencyProperty.Register("FontSizeVisibility", typeof(Visibility), typeof(Project1UIDesignItem), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region 属性编辑文本颜色可见性
        public Visibility TextColorVisibility
        {
            get { return (Visibility)GetValue(TextColorVisibilityProperty); }
            set { SetValue(TextColorVisibilityProperty, value); }
        }
        public static readonly DependencyProperty TextColorVisibilityProperty =
            DependencyProperty.Register("TextColorVisibility", typeof(Visibility), typeof(Project1UIDesignItem), new PropertyMetadata(Visibility.Collapsed));
        #endregion
        #endregion

        public bool IsAttPopupOpen { get; set; } = false;

        public delegate void ControlPointEventHandler(object sender, ControlPoint controlPoint);
        public event ControlPointEventHandler ControlPointMouseDown;
        public event ControlPointEventHandler ControlPointMouseUp;

        private DesignItemModel designItemModel;
        /// <summary>
        /// 属性编辑窗口
        /// </summary>
        private Popup attPopup;
        public Project1UIDesignItem()
        {
            this.DefaultStyleKey = typeof(Project1UIDesignItem);
            designItemModel = new DesignItemModel();
            designItemModel.ControlPointVisibility = Visibility.Hidden;
            this.DataContext = designItemModel;
            RenderTransform = new TranslateTransform()
            {
                X = 0,
                Y = 0
            };
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
        }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var container = GetTemplateChild("Container") as Grid;
            attPopup = GetTemplateChild("popup") as Popup;
            if (container != null)
            {
                CreateControlPoints(container);
                this.MouseDoubleClick += OnPopupOpen;
                //处理内容控件
                HandleControl();

            }
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            this.ContextMenu.IsOpen = true;
        }
        private void HandleControl()
        {
            var control = Content as FrameworkElement;
            //control.DataContext = designItemModel;
            control.IsHitTestVisible = false;
            //绑定宽高
            BindingOperations.SetBinding(control, WidthProperty, new Binding()
            {
                Source = designItemModel,
                Path = new PropertyPath("Width"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

            });
            BindingOperations.SetBinding(control, HeightProperty, new Binding()
            {
                Source = designItemModel,
                Path = new PropertyPath("Height"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

            });
            BindingOperations.SetBinding(control, OpacityProperty, new Binding()
            {
                Source = designItemModel,
                Path = new PropertyPath("Opacity"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

            });
            switch (Content.GetType().Name.ToLower())
            {
                case "project1uibutton":
                    ItemType = DesignItemType.Button;
                    ItemTypeName = "按钮";
                    ColorVisibility = Visibility.Collapsed;
                    ButtonVisibility = Visibility.Visible;
                    FontSizeVisibility = Visibility.Visible;
                    //designItemModel.Width = 100;
                    //designItemModel.Height = 25;
                    //designItemModel.ButtonText = "按钮";
                    BindingOperations.SetBinding(control, Button.ContentProperty, new Binding()
                    {
                        Source = designItemModel,
                        Path = new PropertyPath("ButtonText"),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

                    });
                    BindingOperations.SetBinding(control, Button.FontSizeProperty, new Binding()
                    {
                        Source = designItemModel,
                        Path = new PropertyPath("FontSize"),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

                    });
                    BindingOperations.SetBinding(control, Button.FontWeightProperty, new Binding()
                    {
                        Source = designItemModel,
                        Path = new PropertyPath("FontWeight"),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

                    });
                    designItemModel.PropertyChanged += (s, e) =>
                    {
                        if (this.designItemModel.ButtonStyleName != null)
                        {

                            var res = this.TryFindResource(designItemModel.ButtonStyleName);
                            if (res != null)
                            {
                                control.Style = res as Style;
                            }
                            else
                            {
                                res = this.TryFindResource("default");
                                if (res != null)
                                {
                                    control.Style = res as Style;
                                }
                            }
                        }
                        else
                        {
                            var res = this.TryFindResource("default");
                            if (res != null)
                            {
                                control.Style = res as Style;
                            }
                        }

                    };
                    break;
                case "image":
                    ItemType = DesignItemType.Image;
                    ItemTypeName = "图片";
                    ImageInputVisibility = Visibility.Visible;
                    var image = control as Image;
                    if (image.Source != null)
                    {
                        designItemModel.ImageSource = image.Source;
                        image.Stretch = System.Windows.Media.Stretch.Fill;
                        double imageWidth = image.Source.Width;
                        double imageHeight = image.Source.Height;
                        double windowWidth = (double)this.Parent.GetValue(ActualWidthProperty);
                        double windowHeight = (double)this.Parent.GetValue(ActualHeightProperty);
                        if (imageWidth >= windowWidth / 2)
                        {
                            //图像宽度超过了屏幕50%时缩放50%
                            imageWidth = (int)(imageWidth / 2);
                            imageHeight = (int)(imageHeight / 2);
                        }

                        designItemModel.Width = imageWidth;
                        designItemModel.Height = imageHeight;
                    }

                    BindingOperations.SetBinding(control, Image.SourceProperty, new Binding()
                    {
                        Source = designItemModel,
                        Path = new PropertyPath("ImageSource"),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

                    });
                    break;
                case "textblock":
                    ItemType = DesignItemType.Text;
                    ItemTypeName = "文本";
                    TextInputVisibility = Visibility.Visible;
                    FontSizeVisibility = Visibility.Visible;
                    TextColorVisibility = Visibility.Visible;
                    //designItemModel.Width = 100;
                    //designItemModel.Height = 25;
                    //designItemModel.Text = "文本";
                    BindingOperations.SetBinding(control, TextBlock.TextProperty, new Binding()
                    {
                        Source = designItemModel,
                        Path = new PropertyPath("Text"),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

                    });
                    BindingOperations.SetBinding(control, TextBlock.FontSizeProperty, new Binding()
                    {
                        Source = designItemModel,
                        Path = new PropertyPath("FontSize"),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

                    });
                    BindingOperations.SetBinding(control, TextBlock.ForegroundProperty, new Binding()
                    {
                        Source = designItemModel,
                        Path = new PropertyPath("TextColor"),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

                    });
                    BindingOperations.SetBinding(control, TextBlock.FontWeightProperty, new Binding()
                    {
                        Source = designItemModel,
                        Path = new PropertyPath("FontWeight"),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

                    });
                    break;
            }
        }
        private void OnPopupOpen(object sender, MouseButtonEventArgs e)
        {
            IsAttPopupOpen = true;
            attPopup.IsOpen = true;
            this.MouseDown += OnPopupClose;
        }

        private void OnPopupClose(object sender, MouseButtonEventArgs e)
        {
            IsAttPopupOpen = false;

            attPopup.IsOpen = false;
            this.MouseDown -= OnPopupClose;

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

            point.MouseLeave += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    point.ReleaseMouseCapture();
                    ControlPointMouseUp?.Invoke(this, controlPoint);
                }
            };
            Binding binding = new Binding()
            {
                //Source = designItemModel,
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
