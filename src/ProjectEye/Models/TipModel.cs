using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ProjectEye.Models
{
    public class TipModel : UINotifyPropertyChanged
    {
        private int WarnTime_ = 20;
        /// <summary>
        /// 提醒间隔时间
        /// </summary>
        public int WarnTime
        {
            get { return WarnTime_; }
            set
            {
                WarnTime_ = value;
                OnPropertyChanged("WarnTime");
            }
        }
        private int CountDown_ = 20;
        /// <summary>
        /// 倒计时
        /// </summary>
        public int CountDown
        {
            get { return CountDown_; }
            set
            {
                CountDown_ = value;
                OnPropertyChanged("CountDown");
            }
        }

        private Visibility CountDownVisibility_ = Visibility.Hidden;
        /// <summary>
        /// 倒计时文本可视状态
        /// </summary>
        public Visibility CountDownVisibility
        {
            get { return CountDownVisibility_; }
            set
            {
                CountDownVisibility_ = value;
                OnPropertyChanged("CountDownVisibility");
            }
        }

        private Visibility TakeButtonVisibility_ = Visibility.Visible;
        /// <summary>
        /// 休息按钮可视状态
        /// </summary>
        public Visibility TakeButtonVisibility
        {
            get { return TakeButtonVisibility_; }
            set
            {
                TakeButtonVisibility_ = value;
                OnPropertyChanged("TakeButtonVisibility");
            }
        }
    }
}
