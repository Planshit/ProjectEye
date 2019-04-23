using ProjectEye.Core;
using ProjectEye.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEye.ViewModels
{
    public class TipViewModel : TipModel
    {
        /// <summary>
        /// 休息命令
        /// </summary>
        public Command takeCommand { get; set; }
        public Command busyCommand { get; set; }

        private readonly Take take;
        public TipViewModel()
        {
            takeCommand = new Command(new Action<object>(takeCommand_action));
            busyCommand = new Command(new Action<object>(busyCommand_action));

            take = new Take();
            take.TimeChanged += new TakeEventHandler(timeChanged);
        }

        private void Init()
        {
            CountDown = 20;
            CountDownVisibility = System.Windows.Visibility.Hidden;
            TakeButtonVisibility = System.Windows.Visibility.Visible;
        }

        private void takeCommand_action(object obj)
        {
            CountDownVisibility = System.Windows.Visibility.Visible;
            TakeButtonVisibility = System.Windows.Visibility.Hidden;
            take.Start();
        }
        private void busyCommand_action(object obj)
        {
            take.End();
        }
        private void timeChanged(object sender, int timed)
        {
            CountDown = timed;
            if (CountDown <= 0)
            {
                Init();
            }
        }
    }
}
