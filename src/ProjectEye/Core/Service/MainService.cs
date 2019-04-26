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
        /// 计时器
        /// </summary>
        private readonly DispatcherTimer timer;
        private readonly ScreenService screenService;
        private readonly ConfigService config;
        public MainService(App app, ScreenService screenService, ConfigService configService)
        {
            this.screenService = screenService;
            this.config = configService;
            //初始化计时器
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 20, 0);
#if DEBUG
            timer.Interval = new TimeSpan(0, 0, 20);
#endif

            app.Exit += new ExitEventHandler(app_Exit);
        }



        public void Init()
        {
            var tipWindow = WindowManager.GetCreateWindow("TipWindow", true);

            foreach (var window in tipWindow)
            {
                window.IsVisibleChanged += new DependencyPropertyChangedEventHandler(isVisibleChanged);
            }


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
        private void DoStart()
        {

            timer.Start();

        }
        private void DoStop()
        {
            timer.Stop();



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

    }
}
