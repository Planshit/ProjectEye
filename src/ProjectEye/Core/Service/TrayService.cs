using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Threading;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 管理和显示托盘图标
    /// </summary>
    public class TrayService : IService
    {
        //托盘图标
        private System.Windows.Forms.NotifyIcon notifyIcon;

        //Service
        private readonly App app;
        private readonly MainService mainService;
        private readonly ConfigService config;

        //托盘菜单项
        private ContextMenu contextMenu;
        private MenuItem menuItem_NoReset;
        private MenuItem menuItem_Sound;
        private MenuItem menuItem_Statistic;
        private MenuItem menuItem_Options;
        private MenuItem menuItem_Quit;

        private MenuItem menuItem_NoReset_OneHour;
        private MenuItem menuItem_NoReset_TwoHour;
        private MenuItem menuItem_NoReset_Forver;
        private MenuItem menuItem_NoReset_Off;

        private DispatcherTimer noresetTimer;
        public TrayService(App app, MainService mainService, ConfigService config)
        {
            this.app = app;
            this.mainService = mainService;
            this.config = config;
            this.config.Changed += new EventHandler(config_Changed);

            app.Exit += new ExitEventHandler(app_Exit);
            mainService.OnLeaveEvent += MainService_OnLeaveEvent;
            mainService.OnComeBackEvent += MainService_OnComeBackEvent;

        }

        private void MainService_OnComeBackEvent(object service, int msg)
        {
            UpdateIcon("sunglasses");
        }

        private void MainService_OnLeaveEvent(object service, int msg)
        {
            UpdateIcon("sleeping");
        }

        #region Init
        public void Init()
        {
            //托盘菜单
            contextMenu = new ContextMenu();
            App.Current.Deactivated += (e, c) =>
            {
                contextMenu.IsOpen = false;
            };
            //托盘菜单项
            menuItem_Statistic = new MenuItem();
            menuItem_Statistic.Header = "查看数据统计";
            menuItem_Statistic.Visibility = config.options.General.Data ? Visibility.Visible : Visibility.Collapsed;
            menuItem_Statistic.Click += menuItem_Statistic_Click;

            menuItem_Options = new MenuItem();
            menuItem_Options.Header = "选项";
            menuItem_Options.Click += menuItem_Options_Click;


            menuItem_NoReset = new MenuItem();
            menuItem_NoReset.Header = "不要提醒我";

            menuItem_NoReset_OneHour = new MenuItem();
            menuItem_NoReset_OneHour.Header = "1小时";
            menuItem_NoReset_OneHour.Click += MenuItem_NoReset_OneHour_Click;
            menuItem_NoReset_TwoHour = new MenuItem();
            menuItem_NoReset_TwoHour.Header = "2小时";
            menuItem_NoReset_TwoHour.Click += MenuItem_NoReset_TwoHour_Click;
            menuItem_NoReset_Forver = new MenuItem();
            menuItem_NoReset_Forver.Header = "直到下次启动";
            menuItem_NoReset_Forver.Click += MenuItem_NoReset_Forver_Click;
            menuItem_NoReset_Off = new MenuItem();
            menuItem_NoReset_Off.Header = "关闭";
            menuItem_NoReset_Off.IsChecked = true;
            menuItem_NoReset_Off.Click += MenuItem_NoReset_Off_Click;

            menuItem_NoReset.Items.Add(menuItem_NoReset_OneHour);
            menuItem_NoReset.Items.Add(menuItem_NoReset_TwoHour);
            menuItem_NoReset.Items.Add(menuItem_NoReset_Forver);
            menuItem_NoReset.Items.Add(menuItem_NoReset_Off);

            menuItem_Sound = new MenuItem();
            menuItem_Sound.Header = "提示音";
            menuItem_Sound.IsChecked = config.options.General.Sound;
            menuItem_Sound.Click += menuItem_Sound_Click;

            menuItem_Quit = new MenuItem();
            menuItem_Quit.Header = "退出";
            menuItem_Quit.Click += menuItem_Exit_Click;

            //添加托盘菜单项
            contextMenu.Items.Add(menuItem_Statistic);
            contextMenu.Items.Add(menuItem_Options);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(menuItem_NoReset);
            contextMenu.Items.Add(menuItem_Sound);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(menuItem_Quit);


            //托盘图标添加
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            UpdateIcon("sunglasses");
            notifyIcon.Text = "Project Eye";
            notifyIcon.Visible = true;
            notifyIcon.MouseClick += notifyIcon_MouseClick;
            //在win10中将显示通知
            //notifyIcon.BalloonTipTitle = "test";
            //notifyIcon.BalloonTipText = "content";
            //notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            //notifyIcon.ShowBalloonTip(3000);
            noresetTimer = new DispatcherTimer();
        }


        #endregion

        #region Events
        private void MenuItem_NoReset_Off_Click(object sender, RoutedEventArgs e)
        {
            OnNoResetAction(sender, -1);
        }

        private void MenuItem_NoReset_Forver_Click(object sender, RoutedEventArgs e)
        {
            OnNoResetAction(sender, 0);
        }

        private void MenuItem_NoReset_TwoHour_Click(object sender, RoutedEventArgs e)
        {
            OnNoResetAction(sender, 2);
        }

        private void MenuItem_NoReset_OneHour_Click(object sender, RoutedEventArgs e)
        {
            OnNoResetAction(sender, 1);
        }
        private void menuItem_Statistic_Click(object sender, EventArgs e)
        {
            WindowManager.CreateWindowInScreen("StatisticWindow");
            WindowManager.Show("StatisticWindow");
        }

        private void config_Changed(object sender, EventArgs e)
        {
            menuItem_NoReset.IsChecked = config.options.General.Noreset;
            menuItem_Sound.IsChecked = config.options.General.Sound;
            menuItem_Statistic.Visibility = config.options.General.Data ? Visibility.Visible : Visibility.Collapsed;
        }

        private void menuItem_Options_Click(object sender, EventArgs e)
        {
            WindowManager.CreateWindowInScreen("OptionsWindow");
            WindowManager.Show("OptionsWindow");
        }



        private void menuItem_Sound_Click(object sender, EventArgs e)
        {
            var item = sender as MenuItem;
            item.IsChecked = !item.IsChecked;
            config.options.General.Sound = item.IsChecked;
            config.Save();
        }

        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //右键单击弹出托盘菜单
                contextMenu.IsOpen = true;
                //激活主窗口，用于处理关闭托盘菜单
                App.Current.MainWindow.Activate();

            }
        }



        private void menuItem_Exit_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void app_Exit(object sender, ExitEventArgs e)
        {
            mainService.Exit();
            Remove();
        }
        #endregion

        #region Function
        public void Remove()
        {
            notifyIcon.Visible = false;
        }
        private void UpdateIcon(string name)
        {
            Uri iconUri = new Uri("/ProjectEye;component/Resources/" + name + ".ico", UriKind.RelativeOrAbsolute);
            StreamResourceInfo info = Application.GetResourceStream(iconUri);
            notifyIcon.Icon = new Icon(info.Stream);
        }
        /// <summary>
        /// 设置不提醒操作
        /// </summary>
        /// <param name="hour">-1时关闭；0打开；大于0则在到达设定的值（小时）后重新启动</param>
        private void SetNoReset(int hour)
        {
            menuItem_NoReset_OneHour.IsChecked = false;
            menuItem_NoReset_TwoHour.IsChecked = false;
            menuItem_NoReset_Forver.IsChecked = false;
            menuItem_NoReset_Off.IsChecked = false;
            menuItem_NoReset.IsChecked = true;
            noresetTimer.Stop();
            UpdateIcon("dizzy");
            if (hour == -1)
            {
                //关闭
                menuItem_NoReset.IsChecked = false;
                mainService.Start();
                UpdateIcon("sunglasses");

            }
            else if (hour == 0)
            {
                //直到下次启动
                menuItem_NoReset.IsChecked = true;
                mainService.Pause();
            }
            else
            {
                //指定计时
                menuItem_NoReset.IsChecked = true;
                mainService.Pause();
                
                noresetTimer.Interval = new TimeSpan(hour, 0, 0);
                noresetTimer.Tick += (e, c) =>
                {
                    SetNoReset(-1);
                    menuItem_NoReset_Off.IsChecked = true;
                    noresetTimer.Stop();
                };
                noresetTimer.Start();
            }
        }
        private void OnNoResetAction(object sender, int hour)
        {
            var item = sender as MenuItem;
            if (!item.IsChecked)
            {
                SetNoReset(hour);
                item.IsChecked = true;
            }
        }
        #endregion
    }
}
