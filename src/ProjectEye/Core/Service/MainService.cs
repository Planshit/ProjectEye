using ProjectEye.ViewModels;
using ProjectEye.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class MainService : IService
    {
        /// <summary>
        /// 用眼计时器
        /// </summary>
        private readonly DispatcherTimer timer;
        /// <summary>
        /// 离开检测计时器
        /// </summary>
        private readonly DispatcherTimer leave_timer;
        /// <summary>
        /// 回来检测计时器
        /// </summary>
        private readonly DispatcherTimer back_timer;

        private readonly ScreenService screenService;
        private readonly ConfigService config;
        private readonly CacheService cache;
        public MainService(App app,
            ScreenService screenService,
            ConfigService configService,
            CacheService cache)
        {
            this.screenService = screenService;
            this.config = configService;
            this.cache = cache;

            //初始化用眼计时器
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 20, 0);
#if DEBUG
            timer.Interval = new TimeSpan(0, 0, 20);
#endif

            //初始化离开检测计时器
            leave_timer = new DispatcherTimer();
            leave_timer.Tick += new EventHandler(leave_timer_Tick);
            leave_timer.Interval = new TimeSpan(0, 5, 0);

            //初始化回来检测计时器
            back_timer = new DispatcherTimer();
            back_timer.Tick += new EventHandler(back_timer_Tick);
            back_timer.Interval = new TimeSpan(0, 1, 0);

            app.Exit += new ExitEventHandler(app_Exit);
        }

        private void back_timer_Tick(object sender, EventArgs e)
        {
            if (IsCursorPosChanged())
            {
                //Debug.WriteLine("用户回来了");
                //鼠标变化，停止计时器
                back_timer.Stop();
                leave_timer.Start();
                timer.Start();
            }
            SaveCursorPos();
        }

        private void leave_timer_Tick(object sender, EventArgs e)
        {
            if (IsUserLeave())
            {
                //Debug.WriteLine("用户离开了");
                //用户可能是离开电脑了
                leave_timer.Stop();
                //启动back timer监听鼠标状态
                back_timer.Start();
                timer.Stop();
            }
            SaveCursorPos();
        }

        public void Init()
        {
            var tipWindow = WindowManager.GetCreateWindow("TipWindow", true);

            foreach (var window in tipWindow)
            {
                window.IsVisibleChanged += new DependencyPropertyChangedEventHandler(isVisibleChanged);
            }

            //记录鼠标坐标
            SaveCursorPos();

            Start();
        }




        /// <summary>
        /// 停止主进程。退出程序时调用
        /// </summary>
        public void Exit()
        {
            screenService.Dispose();
            DoStop();
            WindowManager.Close("TipWindow");
        }
        public void Start()
        {
            DoStart();
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            DoStop();
        }
        /// <summary>
        /// 打开离开监听
        /// </summary>
        public void OpenLeaveListener()
        {
            if (!leave_timer.IsEnabled)
            {
                leave_timer.Start();
            }
        }
        /// <summary>
        /// 关闭离开监听
        /// </summary>
        public void CloseLeaveListener()
        {
            leave_timer.Stop();
            back_timer.Stop();
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }
        private void DoStart()
        {
            //休息提醒
            timer.Start();
            if (config.options.general.leavelistener)
            {
                //离开检测
                leave_timer.Start();
            }
        }
        private void DoStop()
        {
            timer.Stop();

            leave_timer.Stop();

            back_timer.Stop();

        }
        /// <summary>
        /// 显示休息提示窗口
        /// </summary>
        private void ShowTipWindow()
        {
            if (!config.options.general.noreset)
            {
                WindowManager.Show("TipWindow");
            }
        }
        private void isVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var window = sender as Window;
            if (window.IsVisible)
            {
                //显示提示窗口时停止计时
                timer.Stop();
            }
            else
            {
                //隐藏时继续计时
                timer.Start();
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {

            ShowTipWindow();
        }

        private void app_Exit(object sender, ExitEventArgs e)
        {
            Exit();
        }

        /// <summary>
        /// 保存光标坐标
        /// </summary>
        private void SaveCursorPos()
        {
            Win32APIHelper.GetCursorPos(out Point point);
            cache["CursorPos"] = point.ToString();
        }
        /// <summary>
        /// 指示光标是否变化了
        /// </summary>
        /// <returns></returns>
        private bool IsCursorPosChanged()
        {
            Win32APIHelper.GetCursorPos(out Point point);
            var beforePos = cache["CursorPos"];
            if (beforePos == null)
            {
                return true;
            }
            return !(beforePos.ToString() == point.ToString());
        }
        /// <summary>
        /// 指示用户是否离开了电脑
        /// </summary>
        /// <returns></returns>
        private bool IsUserLeave()
        {

            if (!IsCursorPosChanged() && !AudioHelper.IsWindowsPlayingSound())
            {
                //鼠标没动且电脑没在播放声音
                return true;
            }
            return false;
        }
    }
}
