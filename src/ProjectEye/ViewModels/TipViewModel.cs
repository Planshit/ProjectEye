using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ProjectEye.ViewModels
{
    public class TipViewModel : TipModel
    {
        /// <summary>
        /// 休息命令
        /// </summary>
        public Command resetCommand { get; set; }
        public Command busyCommand { get; set; }

        private readonly ResetService reset;
        private readonly SoundService sound;


        public TipViewModel(ResetService reset, SoundService sound)
        {
            this.reset = reset;
            this.reset.TimeChanged += new ResetEventHandler(timeChanged);
            this.reset.ResetCompleted += new ResetEventHandler(resetCompleted);

            this.sound = sound;
            resetCommand = new Command(new Action<object>(resetCommand_action));
            busyCommand = new Command(new Action<object>(busyCommand_action));


        }

        private void resetCompleted(object sender, int timed)
        {
            //休息结束
            Init();
            //播放提示音
            if (Config.Sound)
            {
                sound.Play();
            }
        }

        private void Init()
        {
            CountDown = 20;
            CountDownVisibility = System.Windows.Visibility.Hidden;
            TakeButtonVisibility = System.Windows.Visibility.Visible;
        }

        private void resetCommand_action(object obj)
        {
            CountDownVisibility = System.Windows.Visibility.Visible;
            TakeButtonVisibility = System.Windows.Visibility.Hidden;
            reset.Start();
        }
        private void busyCommand_action(object obj)
        {
            WindowManager.Hide("TipWindow");
        }
        private void timeChanged(object sender, int timed)
        {
            CountDown = timed;

        }
    }
}
