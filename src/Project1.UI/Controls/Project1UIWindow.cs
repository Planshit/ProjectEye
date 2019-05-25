using Project1.UI.Controls.Commands;
using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Project1.UI.Controls
{
    public class Project1UIWindow : Window
    {
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

            Loaded += new RoutedEventHandler(window_Loaded);

        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            OnSystemButtonsVisibility();
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
    }
}
