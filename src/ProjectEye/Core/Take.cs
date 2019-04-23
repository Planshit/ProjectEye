using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ProjectEye.Core
{
    public class Take
    {
        /// <summary>
        /// 计时器
        /// </summary>
        private readonly DispatcherTimer timer;
        /// <summary>
        /// 提示窗口
        /// </summary>
        private readonly Window tipWindow;

        /// <summary>
        /// 休息时间
        /// </summary>
        private readonly int TakeTime = 20;

        private int timed = 0;

        /// <summary>
        /// 倒计时更改时发生
        /// </summary>
        public event TakeEventHandler TimeChanged;
        /// <summary>
        /// 休息结束时发生
        /// </summary>
        public event TakeEventHandler TakeCompleted;
        public Take()
        {
            //初始化计时器
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            tipWindow = WindowManager.Get("TipWindow");
        }
        /// <summary>
        /// 开始休息
        /// </summary>
        public void Start()
        {
            timed = TakeTime;
            timer.Start();
        }
        public void End()
        {
            tipWindow.Hide();
            timer.Stop();
            timed = TakeTime;
            OnTakeCompleted();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (timed <= 1)
            {
                End();
            }
            else
            {
                timed--;
                OnTimeChanged();
            }
        }
        private void OnTimeChanged()
        {
            TimeChanged?.Invoke(this, timed);
        }
        private void OnTakeCompleted()
        {
            TakeCompleted?.Invoke(this, 0);
        }
    }
}
