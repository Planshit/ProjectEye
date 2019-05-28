using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ProjectEye.Core.Service
{
    public class ResetService : IService
    {
        /// <summary>
        /// 计时器
        /// </summary>
        private readonly DispatcherTimer timer;

        /// <summary>
        /// 休息时间
        /// </summary>
        private readonly int TakeTime = 20;

        private int timed = 0;

        /// <summary>
        /// 倒计时更改时发生
        /// </summary>
        public event ResetEventHandler TimeChanged;
        /// <summary>
        /// 休息结束时发生
        /// </summary>
        public event ResetEventHandler ResetCompleted;

        private readonly StatisticService statistic;
        private readonly ConfigService config;
        public ResetService(StatisticService statistic,
            ConfigService config)
        {
            this.statistic = statistic;
            this.config = config;
            //初始化计时器
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);

        }



        public void Init()
        {
        }
        /// <summary>
        /// 开始休息
        /// </summary>
        public void Start()
        {
            timed = TakeTime;
            timer.Start();
        }
        /// <summary>
        /// 休息结束
        /// </summary>
        private void End()
        {
            WindowManager.Hide("TipWindow");
            timer.Stop();
            timed = TakeTime;
            OnResetCompleted();

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
        private void OnResetCompleted()
        {
            ResetCompleted?.Invoke(this, 0);
        }


    }
}
