using ProjectEye.ViewModels;
using ProjectEye.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace ProjectEye.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Main
    {
        /// <summary>
        /// 计时器
        /// </summary>
        private readonly DispatcherTimer timer;
        /// <summary>
        /// 休息提示窗口
        /// </summary>
        private readonly Window tipWindow;
        public Main()
        {
            //初始化计时器
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            //timer.Interval = new TimeSpan(0, 20, 0);
            timer.Interval = new TimeSpan(0, 0, 30);

            tipWindow = WindowManager.Get("TipWindow");
            tipWindow.IsVisibleChanged += new DependencyPropertyChangedEventHandler(isVisibleChanged);
        }

        private void isVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (tipWindow.IsVisible)
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

        /// <summary>
        /// 开始主进程。启动时调用
        /// </summary>
        public void Run()
        {
            DoRun();
        }

        /// <summary>
        /// 停止主进程。退出程序时调用
        /// </summary>
        public void Stop()
        {
            DoStop();
        }

        private void DoRun()
        {
            
            timer.Start();

        }
        private void DoStop()
        {
            timer.Stop();
            var tipWindow = WindowManager.Get("TipWindow");
            if (tipWindow != null)
            {
                tipWindow.Close();
            }


        }
        private void ShowTip()
        {
            tipWindow.Show();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ShowTip();
        }
    }
}
