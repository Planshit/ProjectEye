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
        private readonly BackgroundWorkerService backgroundWorker;
        private readonly ThemeService theme;
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

        private string lastIcon = string.Empty;
        public TrayService(
            App app,
            MainService mainService,
            ConfigService config,
            BackgroundWorkerService backgroundWorker,
            ThemeService theme)
        {
            this.app = app;
            this.mainService = mainService;
            this.config = config;
            this.backgroundWorker = backgroundWorker;
            this.theme = theme;
            this.config.Changed += new EventHandler(config_Changed);
            this.theme.OnChangedTheme += Theme_OnChangedTheme;
            app.Exit += new ExitEventHandler(app_Exit);
            mainService.OnLeaveEvent += MainService_OnLeaveEvent;
            mainService.OnStart += MainService_OnStart; ;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.OnCompleted += BackgroundWorker_OnCompleted;

            notifyIcon = new System.Windows.Forms.NotifyIcon();
        }

        //主题更改时
        private void Theme_OnChangedTheme(string OldThemeName, string NewThemeName)
        {
            CreateTrayMenu();
        }

        private void MainService_OnStart(object service, int msg)
        {
            if (!backgroundWorker.IsBusy)
            {
                UpdateIcon("sunglasses");
            }
            if (contextMenu != null)
            {
                menuItem_NoReset_OneHour.IsChecked = false;
                menuItem_NoReset_TwoHour.IsChecked = false;
                menuItem_NoReset_Forver.IsChecked = false;
                menuItem_NoReset.IsChecked = false;
                menuItem_NoReset_Off.IsChecked = true;
            }
        }

        private void MainService_OnLeaveEvent(object service, int msg)
        {
            UpdateIcon("sleeping");
        }

        #region Init
        public void Init()
        {
            //托盘菜单
            CreateTrayMenu();


            //notifyIcon.Text = "Project Eye";
            notifyIcon.Visible = true;
            notifyIcon.MouseMove += NotifyIcon_MouseMove;
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
        private void NotifyIcon_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mainService.IsWorkTimerRun() && !backgroundWorker.IsBusy)
            {
                double restCT = mainService.GetRestCountdownMinutes();
                string restStr = Math.Round(restCT, 1) + "分钟";
                if (restCT < 1)
                {
                    restStr = Math.Round((restCT * 60), 0).ToString() + "秒";
                }
                if (restCT > 60)
                {
                    restCT = Math.Round(restCT / 60, 1);

                    restStr = $"{restCT.ToString()}小时";
                    if (restCT.ToString().IndexOf(".") != -1)
                    {
                        restStr = $"{restCT.ToString().Split('.')[0]}小时{restCT.ToString().Split('.')[1]}分钟";
                    }
                }

                notifyIcon.Text = "Project Eye\r\n距离下一次休息：" + restStr;
            }
            else if (config.options.General.Noreset)
            {
                notifyIcon.Text = "Project Eye：不要提醒已开启";
            }
            else if(!backgroundWorker.IsBusy)
            {
                notifyIcon.Text = "Project Eye";
            }
        }

        //有后台工作任务在运行时
        private void BackgroundWorker_DoWork()
        {
            UpdateIcon("overheated", false);
            notifyIcon.Text = "Project Eye：当前后台正在执行一些任务，请稍后。";
        }
        //后台工作任务运行结束时
        private void BackgroundWorker_OnCompleted()
        {
            notifyIcon.Text = "Project Eye";
            UpdateIcon();
        }

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
            theme.HandleDarkMode();

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (backgroundWorker.IsBusy)
                {
                    return;
                }
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
        private void CreateTrayMenu()
        {
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

        }
        public void Remove()
        {
            notifyIcon.Visible = false;
        }
        private void UpdateIcon(string name = "", bool save = true)
        {
            name = name == "" ? lastIcon : name;
            if (name == "")
            {
                name = "sunglasses";
            }
            if (notifyIcon != null && name != "")
            {
                Uri iconUri = new Uri("/ProjectEye;component/Resources/" + name + ".ico", UriKind.RelativeOrAbsolute);
                StreamResourceInfo info = Application.GetResourceStream(iconUri);
                notifyIcon.Icon = new Icon(info.Stream);
                if (save)
                {
                    lastIcon = name;
                }
            }
        }
        /// <summary>
        /// 设置不提醒操作
        /// </summary>
        /// <param name="hour">-1时关闭；0打开；大于0则在到达设定的值（小时）后重新启动</param>
        private void SetNoReset(int hour)
        {
            config.options.General.Noreset = true;
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
                config.options.General.Noreset = false;
                menuItem_NoReset.IsChecked = false;
                mainService.Start();
                UpdateIcon("sunglasses");

            }
            else if (hour == 0)
            {
                //直到下次启动
                menuItem_NoReset.IsChecked = true;
                mainService.Pause(false);
            }
            else
            {
                //指定计时
                menuItem_NoReset.IsChecked = true;
                mainService.Pause(false);

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
