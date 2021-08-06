using Project1.UI.Controls.Commands;
using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Project1.UI.Controls
{
    public class Project1UIWindow : Window
    {
        /// <summary>
        /// 窗口动画类型
        /// </summary>
        public enum AnimationType
        {
            /// <summary>
            /// 无
            /// </summary>
            None,
            /// <summary>
            /// 右下角缩放移动动画
            /// </summary>
            RightBottomScale,
            /// <summary>
            /// 渐隐渐出动画
            /// </summary>
            Opacity,
            Cool
        }
        #region 1.依赖属性

        #region 窗体logo
        public static readonly DependencyProperty LogoProperty = DependencyProperty.Register("Logo", typeof(object), typeof(Project1UIWindow));
        /// <summary>
        /// 窗体LOGO
        /// </summary>
        public object Logo
        {
            get { return GetValue(LogoProperty); }
            set { SetValue(LogoProperty, value); }
        }
        #endregion

        #region 导航栏视图
        public static readonly DependencyProperty NavigationViewProperty = DependencyProperty.Register("NavigationView", typeof(object), typeof(Project1UIWindow));
        /// <summary>
        /// 导航栏视图
        /// </summary>
        public object NavigationView
        {
            get { return GetValue(NavigationViewProperty); }
            set { SetValue(NavigationViewProperty, value); }
        }
        #endregion

        #region 是否打开导航栏视图

        public static readonly DependencyProperty IsOpenNavigationViewProperty = DependencyProperty.Register("IsOpenNavigationView", typeof(bool), typeof(Project1UIWindow));

        /// <summary>
        /// 是否打开导航栏视图
        /// </summary>
        public bool IsOpenNavigationView
        {
            get { return (bool)GetValue(IsOpenNavigationViewProperty); }
            set { SetValue(IsOpenNavigationViewProperty, value); }
        }
        #endregion

        #region 是否自动显示导航栏视图
        public static readonly DependencyProperty IsAutoOpenNavigationViewProperty = DependencyProperty.Register("IsAutoOpenNavigationView", typeof(bool), typeof(Project1UIWindow));

        /// <summary>
        /// 是否自动显示导航栏视图
        /// </summary>
        public bool IsAutoOpenNavigationView
        {
            get { return (bool)GetValue(IsAutoOpenNavigationViewProperty); }
            set { SetValue(IsAutoOpenNavigationViewProperty, value); }
        }
        #endregion

        #region 最小化按钮显示状态

        public static readonly DependencyProperty MinimizeVisibilityProperty = DependencyProperty.Register("MinimizeVisibility", typeof(Visibility), typeof(Project1UIWindow));

        /// <summary>
        /// 最小化按钮显示状态
        /// </summary>
        public Visibility MinimizeVisibility
        {
            get { return (Visibility)GetValue(MinimizeVisibilityProperty); }
            set { SetValue(MinimizeVisibilityProperty, value); }
        }
        #endregion   

        #region 最大化按钮显示状态

        public static readonly DependencyProperty MaximizeVisibilityProperty = DependencyProperty.Register("MaximizeVisibility", typeof(Visibility), typeof(Project1UIWindow));

        /// <summary>
        /// 最大化按钮显示状态
        /// </summary>
        public Visibility MaximizeVisibility
        {
            get { return (Visibility)GetValue(MaximizeVisibilityProperty); }
            set { SetValue(MaximizeVisibilityProperty, value); }
        }
        #endregion

        #region 关闭按钮显示状态

        public static readonly DependencyProperty CloseVisibilityProperty = DependencyProperty.Register("CloseVisibility", typeof(Visibility), typeof(Project1UIWindow));

        /// <summary>
        /// 关闭按钮显示状态
        /// </summary>
        public Visibility CloseVisibility
        {
            get { return (Visibility)GetValue(CloseVisibilityProperty); }
            set { SetValue(CloseVisibilityProperty, value); }
        }
        #endregion

        #region 拓展内容
        public static readonly DependencyProperty ExtElementProperty = DependencyProperty.Register("ExtElement", typeof(object), typeof(Project1UIWindow));
        /// <summary>
        /// 拓展内容
        /// </summary>
        public object ExtElement
        {
            get { return GetValue(ExtElementProperty); }
            set { SetValue(ExtElementProperty, value); }
        }
        #endregion

        #region 是否启用动画效果
        public static readonly DependencyProperty IsAnimationProperty = DependencyProperty.Register("IsAnimation", typeof(bool), typeof(Project1UIWindow), new PropertyMetadata(false));

        /// <summary>
        /// 是否启用动画效果
        /// </summary>
        public bool IsAnimation
        {
            get { return (bool)GetValue(IsAnimationProperty); }
            set { SetValue(IsAnimationProperty, value); }
        }
        #endregion

        #region 窗口显示关闭使用的动画类型
        public static readonly DependencyProperty WindowAnimationTypeProperty = DependencyProperty.Register("WindowAnimationType", typeof(AnimationType), typeof(Project1UIWindow), new PropertyMetadata(AnimationType.None, new PropertyChangedCallback(OnWindowAnimationTypeChanged)));

        private static void OnWindowAnimationTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Project1UIWindow;
            if (window != null && e.NewValue != e.OldValue)
            {
                var atype = (AnimationType)e.NewValue;
                if (atype == AnimationType.None)
                {
                    return;
                }
                (window.RenderTransform as TransformGroup).Children.Clear();
                window.CreateWindowOpenAnimation();
                window.CreateWindowCloseAnimation();

            }
        }

        /// <summary>
        /// 窗口显示关闭使用的动画类型
        /// </summary>
        public AnimationType WindowAnimationType
        {
            get { return (AnimationType)GetValue(WindowAnimationTypeProperty); }
            set { SetValue(WindowAnimationTypeProperty, value); }
        }
        #endregion

        #region 是否穿透窗口

        public static readonly DependencyProperty IsThruWindowProperty = DependencyProperty.Register("IsThruWindow", typeof(bool), typeof(Project1UIWindow), new PropertyMetadata(false));

        /// <summary>
        /// 是否穿透窗口
        /// </summary>
        public bool IsThruWindow
        {
            get { return (bool)GetValue(IsThruWindowProperty); }
            set { SetValue(IsThruWindowProperty, value); }
        }
        #endregion
        #endregion

        #region 2.私有属性
        #region 导航栏视图宽度
        /// <summary>
        /// 导航栏视图宽度
        /// </summary>
        private double NavigationViewWidth = 0;
        #endregion

        #region 导航栏视图动画
        private DoubleAnimation NavigationViewAnimation;
        private Storyboard NavigationViewStoryboard;
        #endregion

        private readonly Theme theme;

        //窗口开关动画
        private Storyboard openWindowStoryboard;
        private Storyboard closeWindowStoryboard;

        //屏幕尺寸
        private Rectangle ScreenArea;
        #endregion

        #region custom events
        /// <summary>
        /// 当窗口调用wshow完成时
        /// </summary>
        public event EventHandler OnWShow;
        /// <summary>
        /// 当窗口调用whide完成时
        /// </summary>
        public event EventHandler OnWHide;

        #endregion

        #region 3.初始化
        public Project1UIWindow()
        {
            this.DefaultStyleKey = typeof(Project1UIWindow);
            //添加当前窗体到窗体集合
            WindowsCollection.Add(this);
            NavigationViewAnimation = new DoubleAnimation();
            NavigationViewStoryboard = new Storyboard();
            theme = new Theme();
            theme.ApplyTheme();
            //命令绑定
            this.CommandBindings.Add(new CommandBinding(Project1UIWindowCommands.MinimizeWindowCommand, OnMinimizeWindowCommand));
            this.CommandBindings.Add(new CommandBinding(Project1UIWindowCommands.MaximizeWindowCommand, OnMaximizeWindowCommand));
            this.CommandBindings.Add(new CommandBinding(Project1UIWindowCommands.RestoreWindowCommand, OnRestoreWindowCommand));
            this.CommandBindings.Add(new CommandBinding(Project1UIWindowCommands.CloseWindowCommand, OnCloseWindowCommand));
            this.CommandBindings.Add(new CommandBinding(Project1UIWindowCommands.LogoButtonClickCommand, OnLogoButtonClickCommand));

            //HandleAnimationSetting();

            Loaded += new RoutedEventHandler(window_Loaded);

            //动画
            var transformGroup = new TransformGroup();
            RenderTransform = transformGroup;

            //获取屏幕尺寸
            var intPtr = new WindowInteropHelper(this).Handle;//获取当前窗口的句柄
            var screen = System.Windows.Forms.Screen.FromHandle(intPtr);//获取当前屏幕
            ScreenArea = screen.Bounds;
        }

        ///// <summary>
        ///// 配置动画所需的属性设置
        ///// </summary>
        //private void HandleAnimationSetting()
        //{
        //    //if (IsAnimation)
        //    //{
        //    if (WindowAnimationType == AnimationType.RightBottomScale)
        //    {
        //        Opacity = 0;
        //    }
        //    //}
        //}
        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            OnSystemButtonsVisibility();

            if (IsAnimation)
            {
                CreateWindowOpenAnimation();
                CreateWindowCloseAnimation();
            }

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            var NavigationViewGrid = GetTemplateChild("NavigationViewGrid") as Grid;

            var NavigationView = GetTemplateChild("NavigationView") as ContentControl;

            if (NavigationViewGrid != null
                && NavigationView != null)
            {
                NavigationViewGrid.Loaded += (e, c) =>
                {
                    NavigationViewWidth = NavigationViewGrid.ActualWidth;
                    NavigationViewGrid.Width = 0;

                    if (IsAutoOpenNavigationView)
                    {
                        IsOpenNavigationView = !IsOpenNavigationView;
                        ShowNavigationView();
                    }
                };


                NavigationViewAnimation.Duration = TimeSpan.FromSeconds(0.1);
                Storyboard.SetTarget(NavigationViewAnimation, NavigationViewGrid);
                Storyboard.SetTargetProperty(NavigationViewAnimation, new PropertyPath("Width"));

                NavigationViewStoryboard.Children.Add(NavigationViewAnimation);
            }

        }


        /// <summary>
        /// 根据窗口ResizeMode设置系统按钮的可视状态
        /// </summary>
        private void OnSystemButtonsVisibility()
        {
            switch (ResizeMode)
            {
                case ResizeMode.NoResize:
                    //仅显示关闭按钮
                    MinimizeVisibility = Visibility.Collapsed;
                    MaximizeVisibility = Visibility.Collapsed;
                    CloseVisibility = Visibility.Visible;
                    break;
                case ResizeMode.CanResize:
                    MinimizeVisibility = Visibility.Visible;
                    MaximizeVisibility = Visibility.Visible;
                    CloseVisibility = Visibility.Visible;
                    break;
                case ResizeMode.CanMinimize:
                    MinimizeVisibility = Visibility.Visible;
                    MaximizeVisibility = Visibility.Collapsed;
                    CloseVisibility = Visibility.Visible;
                    break;
            }
        }
        #endregion

        #region 4.动画
        public void ShowNavigationView()
        {

            NavigationViewAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };

            NavigationViewAnimation.To = NavigationViewWidth;

            NavigationViewStoryboard.Begin();

        }
        public void CloseNavigationView()
        {

            NavigationViewAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            NavigationViewAnimation.To = 0;

            NavigationViewStoryboard.Begin();

        }

        private void CreateTransformGroup()
        {
            if ((RenderTransform as TransformGroup).Children.Count == 0)
            {
                var translateTF = new TranslateTransform()
                {
                    X = 0,
                    Y = 0
                };

                var scaleTF = new ScaleTransform()
                {
                    //右下角
                    CenterX = ScreenArea.Width,
                    CenterY = ScreenArea.Height,
                    //左上角
                    //CenterX = 0,
                    //CenterY = 0,
                    ScaleX = 1,
                    ScaleY = 1
                };
                (RenderTransform as TransformGroup).Children.Add(translateTF);
                (RenderTransform as TransformGroup).Children.Add(scaleTF);
            }
        }
        private void CreateWindowOpenAnimation()
        {
            CreateTransformGroup();

            Opacity = 0;
            openWindowStoryboard = new Storyboard();
            var duration = TimeSpan.FromSeconds(1);

            if (WindowAnimationType == AnimationType.RightBottomScale)
            {
                //位移动画


                var easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                DoubleAnimation translateXAnimation = new DoubleAnimation();
                translateXAnimation.From = ScreenArea.Width;
                translateXAnimation.To = Left;
                translateXAnimation.Duration = duration;
                //BackEase,QuarticEase
                translateXAnimation.EasingFunction = easingFunction;
                Storyboard.SetTarget(translateXAnimation, this);
                Storyboard.SetTargetProperty(translateXAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));
                DoubleAnimation translateYAnimation = new DoubleAnimation();
                translateYAnimation.From = ScreenArea.Height;
                translateYAnimation.To = Top;
                translateYAnimation.Duration = duration;
                translateYAnimation.EasingFunction = easingFunction;
                Storyboard.SetTarget(translateYAnimation, this);
                Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));

                //缩放动画
                var scaleEasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseInOut };
                DoubleAnimation scaleXAnimation = new DoubleAnimation();
                scaleXAnimation.From = 0;
                scaleXAnimation.To = 1;
                scaleXAnimation.Duration = duration;
                //BackEase,QuarticEase
                scaleXAnimation.EasingFunction = scaleEasingFunction;
                Storyboard.SetTarget(scaleXAnimation, this);
                Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
                DoubleAnimation scaleYAnimation = new DoubleAnimation();
                scaleYAnimation.From = 0;
                scaleYAnimation.To = 1;
                scaleYAnimation.Duration = duration;
                scaleYAnimation.EasingFunction = scaleEasingFunction;
                Storyboard.SetTarget(scaleYAnimation, this);
                Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));



                openWindowStoryboard.Children.Add(translateXAnimation);
                openWindowStoryboard.Children.Add(translateYAnimation);
                openWindowStoryboard.Children.Add(scaleXAnimation);
                openWindowStoryboard.Children.Add(scaleYAnimation);
            }
            else if (WindowAnimationType == AnimationType.Opacity)
            {
                DoubleAnimation opacityAnimation = new DoubleAnimation();
                opacityAnimation.From = 0;
                opacityAnimation.To = 1;
                opacityAnimation.Duration = duration;
                opacityAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
                Storyboard.SetTarget(opacityAnimation, this);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));
                openWindowStoryboard.Children.Add(opacityAnimation);
            }
            else if (WindowAnimationType == AnimationType.Cool)
            {
                //RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

                //  保存布局
                var ui = Content as Grid;
                Content = null;
                //  创建新的容器
                var container = new Grid();
                container.RenderTransform = new TransformGroup();
                container.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

                (container.RenderTransform as TransformGroup).Children.Add(new ScaleTransform()
                {
                    ScaleX = 1,
                    ScaleY = 1
                });
                container.Children.Add(ui);
                Content = container;

                //Opacity = 1;

                var icon = new System.Windows.Controls.Image();
                icon.Source = BitmapImager.Load($"pack://application:,,,/ProjectEye;component/Resources/sunglasses.ico");
                icon.HorizontalAlignment = HorizontalAlignment.Left;
                icon.VerticalAlignment = VerticalAlignment.Top;
                icon.RenderTransform = new TransformGroup();
                icon.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                var scaleTF = new ScaleTransform()
                {
                    ScaleX = 1,
                    ScaleY = 1
                };
                var ttf = new TranslateTransform()
                {
                    X = ScreenArea.Width / 2 - 150,
                    Y = -300
                };
                (icon.RenderTransform as TransformGroup).Children.Add(scaleTF);
                (icon.RenderTransform as TransformGroup).Children.Add(ttf);
                icon.IsHitTestVisible = false;
                icon.Width = 300;
                icon.Height = 300;
                container.Children.Add(icon);




                //位移动画
                var easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };

                DoubleAnimation translateYAnimation = new DoubleAnimation();
                translateYAnimation.From =  - 300;
                translateYAnimation.To = ScreenArea.Height / 2 - 150;
                translateYAnimation.Duration = TimeSpan.FromSeconds(1);
                translateYAnimation.EasingFunction = easingFunction;
                //translateYAnimation.AutoReverse = true;
                Storyboard.SetTarget(translateYAnimation, icon);
                Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)"));

                //  logo放大缩小动画
                DoubleAnimationUsingKeyFrames scaleAnimation = new DoubleAnimationUsingKeyFrames();
                scaleAnimation.KeyFrames.Add(
              new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
                scaleAnimation.KeyFrames.Add(
               new LinearDoubleKeyFrame(2, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.8))));
                scaleAnimation.KeyFrames.Add(
               new LinearDoubleKeyFrame(15, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));

                Storyboard.SetTarget(scaleAnimation, icon);
                DoubleAnimationUsingKeyFrames scaleAnimation2 = new DoubleAnimationUsingKeyFrames();
                scaleAnimation2.KeyFrames.Add(
             new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
                scaleAnimation2.KeyFrames.Add(
              new LinearDoubleKeyFrame(2, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.8))));
                scaleAnimation2.KeyFrames.Add(
               new LinearDoubleKeyFrame(15, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));

                Storyboard.SetTarget(scaleAnimation, icon);
                Storyboard.SetTarget(scaleAnimation2, icon);

                Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                Storyboard.SetTargetProperty(scaleAnimation2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

                //  logo透明动画

                DoubleAnimationUsingKeyFrames iconOpacityAnimation = new DoubleAnimationUsingKeyFrames();
                iconOpacityAnimation.KeyFrames.Add(
               new LinearDoubleKeyFrame(.8, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.8))));
                iconOpacityAnimation.KeyFrames.Add(
               new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.2))));
                Storyboard.SetTarget(iconOpacityAnimation, icon);
                Storyboard.SetTargetProperty(iconOpacityAnimation, new PropertyPath(OpacityProperty));


                //  ui透明动画
                DoubleAnimation opacityAnimation2 = new DoubleAnimation();
                opacityAnimation2.From = 0;

                opacityAnimation2.To = 1;
                opacityAnimation2.Duration = TimeSpan.FromSeconds(1.5);
                opacityAnimation2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
                Storyboard.SetTarget(opacityAnimation2, ui);
                Storyboard.SetTargetProperty(opacityAnimation2, new PropertyPath(OpacityProperty));





                //  窗口透明动画
                DoubleAnimation windowOpacityAnimation = new DoubleAnimation();
                windowOpacityAnimation.From = 0;
                windowOpacityAnimation.To = 1;
                windowOpacityAnimation.Duration = TimeSpan.FromSeconds(1.5);
                windowOpacityAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
                Storyboard.SetTarget(windowOpacityAnimation, this);
                Storyboard.SetTargetProperty(windowOpacityAnimation, new PropertyPath(OpacityProperty));

                openWindowStoryboard.Children.Add(translateYAnimation);

                openWindowStoryboard.Children.Add(opacityAnimation2);
                openWindowStoryboard.Children.Add(iconOpacityAnimation);
                openWindowStoryboard.Children.Add(scaleAnimation);
                openWindowStoryboard.Children.Add(scaleAnimation2);
                openWindowStoryboard.Children.Add(windowOpacityAnimation);
            }
        }

        private void CreateWindowCloseAnimation()
        {
            CreateTransformGroup();

            closeWindowStoryboard = new Storyboard();
            var duration = TimeSpan.FromSeconds(1);

            if (WindowAnimationType == AnimationType.RightBottomScale)
            {
                //位移动画
                var easingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
                DoubleAnimation translateXAnimation = new DoubleAnimation();
                translateXAnimation.To = ScreenArea.Width;
                translateXAnimation.Duration = duration;

                //BackEase,QuarticEase
                translateXAnimation.EasingFunction = easingFunction;
                Storyboard.SetTarget(translateXAnimation, this);
                Storyboard.SetTargetProperty(translateXAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));
                DoubleAnimation translateYAnimation = new DoubleAnimation();
                translateYAnimation.To = ScreenArea.Height;
                translateYAnimation.Duration = duration;
                translateYAnimation.EasingFunction = easingFunction;
                Storyboard.SetTarget(translateYAnimation, this);
                Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));

                //缩放动画
                var scaleEasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseInOut };
                DoubleAnimation scaleXAnimation = new DoubleAnimation();
                scaleXAnimation.To = 0;
                scaleXAnimation.Duration = duration;
                //BackEase,QuarticEase
                scaleXAnimation.EasingFunction = scaleEasingFunction;
                Storyboard.SetTarget(scaleXAnimation, this);
                Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleX)"));
                DoubleAnimation scaleYAnimation = new DoubleAnimation();
                scaleYAnimation.To = 0;
                scaleYAnimation.Duration = duration;
                scaleYAnimation.EasingFunction = scaleEasingFunction;
                Storyboard.SetTarget(scaleYAnimation, this);
                Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(ScaleTransform.ScaleY)"));

                closeWindowStoryboard.Children.Add(translateXAnimation);
                closeWindowStoryboard.Children.Add(translateYAnimation);
                closeWindowStoryboard.Children.Add(scaleXAnimation);
                closeWindowStoryboard.Children.Add(scaleYAnimation);
            }
            else if (WindowAnimationType == AnimationType.Opacity)
            {
                DoubleAnimation opacityAnimation = new DoubleAnimation();
                opacityAnimation.From = 1;
                opacityAnimation.To = 0;
                opacityAnimation.Duration = duration;
                opacityAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
                Storyboard.SetTarget(opacityAnimation, this);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));
                closeWindowStoryboard.Children.Add(opacityAnimation);
            }
            else if (WindowAnimationType == AnimationType.Cool)
            {

                var container = Content as Grid;
                
                DoubleAnimationUsingKeyFrames scaleAnimation = new DoubleAnimationUsingKeyFrames();
                scaleAnimation.AutoReverse = true;
                scaleAnimation.KeyFrames.Add(
              new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
                scaleAnimation.KeyFrames.Add(
               new LinearDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.5))));
                scaleAnimation.KeyFrames.Add(
               new LinearDoubleKeyFrame(18, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.6))));

                DoubleAnimationUsingKeyFrames scaleAnimation2 = new DoubleAnimationUsingKeyFrames();
                scaleAnimation2.AutoReverse = true;
                scaleAnimation2.KeyFrames.Add(
              new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
                scaleAnimation2.KeyFrames.Add(
              new LinearDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.5))));
                scaleAnimation2.KeyFrames.Add(
               new LinearDoubleKeyFrame(18, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.6))));


                Storyboard.SetTarget(scaleAnimation, container);
                Storyboard.SetTarget(scaleAnimation2, container);

                Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                Storyboard.SetTargetProperty(scaleAnimation2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

                DoubleAnimation opacityAnimation2 = new DoubleAnimation();
                opacityAnimation2.From = 1;

                opacityAnimation2.To = 0;
                opacityAnimation2.Duration = TimeSpan.FromSeconds(.8);
                opacityAnimation2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
                Storyboard.SetTarget(opacityAnimation2, this);
                Storyboard.SetTargetProperty(opacityAnimation2, new PropertyPath(OpacityProperty));

                closeWindowStoryboard.Children.Add(scaleAnimation);
                closeWindowStoryboard.Children.Add(scaleAnimation2);
                closeWindowStoryboard.Children.Add(opacityAnimation2);


            }
        }

        #endregion

        #region 5.命令
        private void OnLogoButtonClickCommand(object sender, ExecutedRoutedEventArgs e)
        {
            IsOpenNavigationView = !IsOpenNavigationView;
            if (IsOpenNavigationView)
            {
                ShowNavigationView();
            }
            else
            {
                CloseNavigationView();

            }



        }

        private void OnCloseWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnRestoreWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void OnMaximizeWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void OnMinimizeWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region 6.事件

        //关闭窗口完毕
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WindowsCollection.Remove(this);
        }
        protected override void OnActivated(EventArgs args)
        {
            base.OnActivated(args);
            //Project1.UI.Cores.Little.FocusWindow = this;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (IsThruWindow)
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle; uint extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            }
        }
        #endregion

        #region 7.主题
        public void SetTheme(string s)
        {
            theme.SetTheme(s);
        }
        public void SetThemeColor(string color)
        {
            theme.SetThemeColor(color);
        }

        #endregion

        #region Method
        /// <summary>
        /// 动画结束后的操作
        /// </summary>
        public enum CompletedActionType
        {
            Hide,
            Show,
            Close
        }
        #region 隐藏窗口
        /// <summary>
        /// 隐藏窗口,替代原有的Hide();
        /// </summary>
        public void WHide()
        {
            AnimationHide(CompletedActionType.Hide);
        }
        #endregion

        #region 显示窗口
        /// <summary>
        /// 显示窗口,替代原有的Show();
        /// </summary>
        public void WShow()
        {
            AnimationShow(CompletedActionType.Show);
        }
        #endregion

        #region 关闭窗口
        /// <summary>
        /// 关闭窗口,替代原有的close
        /// </summary>
        public void WClose()
        {
            AnimationHide(CompletedActionType.Close);
        }
        #endregion

        #region 隐藏窗口动画
        /// <summary>
        /// 隐藏窗口动画
        /// </summary>
        private void AnimationHide(CompletedActionType completedAction)
        {
            if (IsAnimation && WindowAnimationType != AnimationType.None)
            {
                if ((RenderTransform as TransformGroup).Children.Count == 0)
                {
                    CreateWindowOpenAnimation();
                    CreateWindowCloseAnimation();
                }
                switch (WindowAnimationType)
                {
                    case AnimationType.None:
                        CompletedAction(completedAction);
                        OnWHide?.Invoke(this, null);

                        break;
                    case AnimationType.RightBottomScale:
                    case AnimationType.Opacity:
                    case AnimationType.Cool:
                        closeWindowStoryboard.Completed += (e, c) =>
                        {
                            Opacity = 0;
                            CompletedAction(completedAction);
                            OnWHide?.Invoke(this, null);
                        };
                        closeWindowStoryboard.Begin();
                        break;

                }
            }
            else
            {
                if ((RenderTransform as TransformGroup).Children.Count != 0)
                {
                    (RenderTransform as TransformGroup).Children.Clear();
                }
                Opacity = 0;
                CompletedAction(completedAction);
                OnWHide?.Invoke(this, null);
            }
        }
        #endregion

        #region 显示窗口动画
        /// <summary>
        /// 显示窗口动画
        /// </summary>
        private void AnimationShow(CompletedActionType completedAction)
        {
            if (IsAnimation && WindowAnimationType != AnimationType.None)
            {
                if ((RenderTransform as TransformGroup).Children.Count == 0)
                {
                    CreateWindowOpenAnimation();
                    CreateWindowCloseAnimation();
                }
                CompletedAction(completedAction);
                switch (WindowAnimationType)
                {
                    case AnimationType.RightBottomScale:
                    case AnimationType.Cool:
                        Opacity = 1;
                        openWindowStoryboard.Begin();
                        break;
                    default:
                        openWindowStoryboard.Begin();
                        break;
                }
            }
            else
            {
                if ((RenderTransform as TransformGroup).Children.Count != 0)
                {
                    (RenderTransform as TransformGroup).Children.Clear();
                }
                Opacity = 1;
                CompletedAction(completedAction);
            }
            OnWShow?.Invoke(this, null);
        }
        #endregion

        private void CompletedAction(CompletedActionType completedAction)
        {
            switch (completedAction)
            {
                case CompletedActionType.Close:
                    Close();
                    break;
                case CompletedActionType.Hide:
                    Hide();
                    break;
                case CompletedActionType.Show:
                    //如果开启了鼠标穿透则不抢占焦点
                    ShowActivated = !IsThruWindow;
                    Show();
                    break;
            }
        }

        #endregion

        #region win32
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_EXSTYLE = (-20);

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);
        #endregion
    }
}
