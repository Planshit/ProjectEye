using Project1.UI.Controls;
using ProjectEye.ViewModels;
using ProjectEye.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// Main Service
    /// </summary>
    public class MainService : IService
    {
        /// <summary>
        /// 用眼计时器
        /// </summary>
        private DispatcherTimer timer;
        /// <summary>
        /// 离开检测计时器
        /// </summary>
        private DispatcherTimer leave_timer;
        /// <summary>
        /// 回来检测计时器
        /// </summary>
        private DispatcherTimer back_timer;
        /// <summary>
        /// 繁忙检测，用于检测用户在休息提示界面是否超时不操作
        /// </summary>
        private DispatcherTimer busy_timer;
        /// <summary>
        /// 用眼计时，用于定时统计和保存用户的用眼时长
        /// </summary>
        private DispatcherTimer useeye_timer;
        /// <summary>
        /// 预提醒操作
        /// </summary>
        private PreAlertAction preAlertAction;
        #region Service
        private readonly ScreenService screen;
        private readonly ConfigService config;
        private readonly CacheService cache;
        private readonly StatisticService statistic;
        #endregion

        #region win32
        //[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        //public static extern IntPtr GetForegroundWindow();
        //[DllImport("user32", SetLastError = true)]
        //public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        #endregion

        #region Event
        public delegate void MainEventHandler(object service, int msg);
        /// <summary>
        /// 用户离开时发生
        /// </summary>
        public event MainEventHandler OnLeaveEvent;
        /// <summary>
        /// 用户回来时发生
        /// </summary>
        public event MainEventHandler OnComeBackEvent;
        /// <summary>
        /// 计时器重启时发生
        /// </summary>
        public event MainEventHandler OnReStartTimer;
        /// <summary>
        /// 到达休息时间时发生（不管是否进入休息状态）
        /// </summary>
        public event MainEventHandler OnReset;
        /// <summary>
        /// 计时器停止时发生
        /// </summary>
        public event MainEventHandler OnPause;
        /// <summary>
        /// 计时器启动时发生
        /// </summary>
        public event MainEventHandler OnStart;
        #endregion
        public MainService(App app,
            ScreenService screen,
            ConfigService config,
            CacheService cache,
            StatisticService statistic)
        {
            this.screen = screen;
            this.config = config;
            this.cache = cache;
            this.statistic = statistic;
            app.Exit += new ExitEventHandler(app_Exit);
        }

        private void busy_timer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("用户超过20秒未处理");
            //用户超过20秒未处理
            busy_timer.Stop();
            //关闭窗口
            WindowManager.Hide("TipWindow");
            if (config.options.General.LeaveListener)
            {
                //如果打开离开监听则进入离开状态
                OnLeave();
            }
            else
            {
                ReStart();
            }

        }

        private void back_timer_Tick(object sender, EventArgs e)
        {
            if (IsCursorPosChanged())
            {
                Debug.WriteLine("用户回来了");
                //鼠标变化，停止计时器
                back_timer.Stop();
                leave_timer.Start();
                timer.Start();
                //事件响应
                OnComeBackEvent?.Invoke(this, 0);
                if (config.options.General.Data)
                {
                    statistic.ResetStatisticTime();
                    //启动用眼计时
                    useeye_timer.Start();
                }
            }
            SaveCursorPos();
        }

        private void leave_timer_Tick(object sender, EventArgs e)
        {
            if (IsUserLeave())
            {
                //用户离开了电脑
                OnLeave();
            }
            SaveCursorPos();
        }

        public void Init()
        {
            //初始化用眼计时器
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, config.options.General.WarnTime, 0);
            //初始化离开检测计时器
            leave_timer = new DispatcherTimer();
            leave_timer.Tick += new EventHandler(leave_timer_Tick);
            leave_timer.Interval = new TimeSpan(0, 5, 0);
            //初始化回来检测计时器
            back_timer = new DispatcherTimer();
            back_timer.Tick += new EventHandler(back_timer_Tick);
            back_timer.Interval = new TimeSpan(0, 1, 0);
            //初始化繁忙计时器
            busy_timer = new DispatcherTimer();
            busy_timer.Tick += new EventHandler(busy_timer_Tick);
            busy_timer.Interval = new TimeSpan(0, 0, 30);
            //初始化用眼统计计时器
            useeye_timer = new DispatcherTimer();
            useeye_timer.Tick += new EventHandler(useeye_timer_Tick);
            useeye_timer.Interval = new TimeSpan(0, 30, 0);
            /****调试模式代码****/
#if DEBUG
            //30秒提示休息
            timer.Interval = new TimeSpan(10, 0, 30);
            //20秒表示离开
            leave_timer.Interval = new TimeSpan(10, 0, 20);
            //每10秒检测回来
            back_timer.Interval = new TimeSpan(10, 0, 10);
            useeye_timer.Interval = new TimeSpan(10, 1, 0);
#endif

            //在所有屏幕上创建全屏提示窗口
            var tipWindow = WindowManager.GetCreateWindow("TipWindow", true);

            foreach (var window in tipWindow)
            {
                window.IsVisibleChanged += new DependencyPropertyChangedEventHandler(isVisibleChanged);
            }
            //记录鼠标坐标
            SaveCursorPos();

            Start();

        }

        #region 到达统计时间
        private void useeye_timer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("统计用眼时长");
            //更新用眼时长
            statistic.StatisticUseEyeData();
            //数据持久化
            statistic.Save();

        }
        #endregion

        #region 结束繁忙超时监听
        /// <summary>
        /// 结束繁忙超时监听
        /// </summary>
        public void StopBusyListener()
        {
            if (busy_timer.IsEnabled)
            {
                busy_timer.Stop();
            }
        }
        #endregion

        #region 进入离开状态
        /// <summary>
        /// 进入离开状态
        /// </summary>
        public void OnLeave()
        {
            Debug.WriteLine("用户离开了");
            //用户可能是离开电脑了
            leave_timer.Stop();
            //启动back timer监听鼠标状态
            back_timer.Start();
            timer.Stop();
            //事件响应
            OnLeaveEvent?.Invoke(this, 0);

            //停止用眼计时
            useeye_timer.Stop();

        }
        #endregion

        #region 停止主进程。退出程序时调用
        /// <summary>
        /// 停止主进程。退出程序时调用
        /// </summary>
        public void Exit()
        {
            if (config.options.General.Data)
            {
                //更新用眼时长
                statistic.StatisticUseEyeData();
                //数据持久化
                statistic.Save();
            }

            screen.Dispose();
            DoStop();
            WindowManager.Close("TipWindow");
        }
        #endregion

        #region 启动计时
        public void Start()
        {
            DoStart();
        }
        #endregion

        #region 暂停计时
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            DoStop();
        }
        #endregion

        #region 打开离开监听
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
        #endregion

        #region 关闭离开监听
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
        #endregion

        #region 设置提醒间隔时间并重新启动休息计时
        /// <summary>
        /// 设置提醒间隔时间并重新启动休息计时
        /// </summary>
        /// <param name="minutes"></param>
        public void SetWarnTime(int minutes)
        {
            if (timer.Interval.TotalMinutes != minutes)
            {
                Debug.WriteLine(timer.Interval.TotalMinutes + "," + minutes);
                timer.Interval = new TimeSpan(0, minutes, 0);
                ReStart();
            }
        }
        #endregion

        #region 重新启动计时
        /// <summary>
        /// 重新启动休息计时
        /// </summary>
        public void ReStart()
        {
            Debug.WriteLine("重新启动休息计时");
            DoStop();
            DoStart();
            OnReStartTimer?.Invoke(this, 0);
        }

        #endregion

        #region 启动计时实际操作
        private void DoStart()
        {
            //休息提醒
            timer.Start();
            if (config.options.General.LeaveListener)
            {
                //离开检测
                leave_timer.Start();
            }
            if (config.options.General.Data)
            {
                //用眼统计
                useeye_timer.Start();
            }
            OnStart?.Invoke(this, 0);
        }
        #endregion

        #region 停止计时实际操作
        private void DoStop()
        {
            timer.Stop();

            leave_timer.Stop();

            back_timer.Stop();

            OnPause?.Invoke(this, 0);
        }
        #endregion

        #region 显示休息提示窗口
        /// <summary>
        /// 显示休息提示窗口
        /// </summary>
        private void ShowTipWindow()
        {
            if (config.options.Style.IsPreAlert && preAlertAction == PreAlertAction.Break)
            {
                //预提醒设置跳过
                statistic.Add(StatisticType.SkipCount, 1);
            }
            else
            {
                if (isBreakReset())
                {
                    statistic.Add(StatisticType.SkipCount, 1);
                }
                else
                {
                    busy_timer.Start();
                    WindowManager.Show("TipWindow");
                }
            }
        }
        #endregion

        #region 提示窗口显示时 Event
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
        #endregion

        #region 用眼到达设定时间 Event
        private void timer_Tick(object sender, EventArgs e)
        {
            ShowTipWindow();
            OnReset?.Invoke(this, 0);
        }
        #endregion

        #region 程序退出 Event
        private void app_Exit(object sender, ExitEventArgs e)
        {
            Exit();
        }
        #endregion

        #region 保存光标坐标
        /// <summary>
        /// 保存光标坐标
        /// </summary>
        private void SaveCursorPos()
        {
            Win32APIHelper.GetCursorPos(out Point point);
            cache["CursorPos"] = point.ToString();
        }
        #endregion

        #region 指示光标是否变化了
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
        #endregion

        #region 指示用户是否离开了电脑
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

        #endregion

        #region 设置预提醒状态
        public void SetPreAlertAction(PreAlertAction preAlertAction)
        {
            this.preAlertAction = preAlertAction;
        }
        #endregion

        #region 是否跳过本次休息
        /// <summary>
        /// 是否跳过本次休息
        /// </summary>
        /// <returns>true跳过，false不跳过</returns>
        public bool isBreakReset()
        {
            if (!config.options.General.Noreset)
            {
                //0.全屏跳过判断
                if (config.options.Behavior.IsFullScreenBreak)
                {
                    var info = Win32APIHelper.GetFocusWindowInfo();
                    if (info.IsFullScreen)
                    {
                        return true;
                    }
                }

                //1.进程跳过判断
                if (config.options.Behavior.IsBreakProgressList)
                {
                    Process[] processes = Process.GetProcesses();
                    foreach (Process process in processes)
                    {
                        try
                        {
                            if (config.options.Behavior.BreakProgressList.Contains(process.ProcessName))
                            {
                                return true;
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                return false;
            }

            return true;
        }
        #endregion
    }
}
