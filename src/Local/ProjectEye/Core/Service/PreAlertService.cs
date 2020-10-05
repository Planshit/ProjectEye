using Project1.UI.Controls;
using ProjectEye.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 预提醒操作类型
    /// </summary>
    public enum PreAlertAction
    {
        /// <summary>
        /// 继续
        /// </summary>
        Goto,
        /// <summary>
        /// 跳过
        /// </summary>
        Break
    }
    public class PreAlertModel : UINotifyPropertyChanged
    {
        private string Title_;
        public string Title
        {
            get
            {
                return Title_;
            }
            set
            {
                Title_ = value;
                OnPropertyChanged();
            }
        }
        private string Subtitle_;
        public string Subtitle
        {
            get
            {
                return Subtitle_;
            }
            set
            {
                Subtitle_ = value;
                OnPropertyChanged();
            }
        }
        private string Message_;
        public string Message
        {
            get
            {
                return Message_;
            }
            set
            {
                Message_ = value;
                OnPropertyChanged();
            }
        }
    }
    public class PreAlertService : IService
    {
        private readonly ConfigService config;
        private readonly MainService main;
        private readonly RestService reset;
        private readonly StatisticService statistic;
        private readonly SoundService sound;

        private DispatcherTimer preAlertTimer;
        private PreAlertAction PreAlertAction_;
        /// <summary>
        /// 获取预提醒操作
        /// </summary>
        public PreAlertAction PreAlertAction
        {
            get
            {
                return PreAlertAction_;
            }
        }
        private bool isPreAlert;
        private int preAlertTime;
        /// <summary>
        /// 预提醒剩余秒数
        /// </summary>
        private int preAlertHasTime;
        public PreAlertService(ConfigService config,
            MainService main,
            RestService reset,
            StatisticService statistic,
            SoundService sound)
        {
            this.config = config;
            this.main = main;
            this.reset = reset;
            this.statistic = statistic;
            this.sound = sound;

            main.OnReStartTimer += Main_OnReStartTimer;
            main.OnReset += Main_OnReset;
            main.OnLeaveEvent += Main_OnLeaveEvent;
            main.OnPause += Main_OnPause;
            main.OnStart += Main_OnStart;
            config.Changed += Config_Changed;
            reset.RestStart += Reset_ResetStart;
            reset.RestCompleted += Reset_ResetCompleted;


        }

        private void Main_OnStart(object service, int msg)
        {
            if (config.options.Style.IsPreAlert)
            {
                InitPreAlert();
            }

        }

        private void Main_OnPause(object service, int msg)
        {
            if (config.options.Style.IsPreAlert && preAlertTimer.IsEnabled)
            {
                preAlertTimer.Stop();
            }
        }


        private void Main_OnLeaveEvent(object service, int msg)
        {
            if (config.options.Style.IsPreAlert)
            {
                preAlertTimer.Stop();
            }
        }

        private void Main_OnReset(object service, int msg)
        {
            if (config.options.Style.IsPreAlert)
            {
                //达到休息时间时重启预提醒
                preAlertTimer?.Stop();
                preAlertTimer?.Start();
            }
        }

        private void Reset_ResetCompleted(object sender, int timed)
        {
            if (config.options.Style.IsPreAlert)
            {
                //休息结束时启动预提醒
                preAlertTimer?.Start();
            }
        }

        private void Reset_ResetStart(object sender, int timed)
        {
            if (config.options.Style.IsPreAlert)
            {
                //休息开始时停止预提醒
                preAlertTimer?.Stop();
            }
        }

        private void Main_OnReStartTimer(object service, int msg)
        {
            if (config.options.Style.IsPreAlert)
            {
                //当休息计时重启时初始化预提醒
                InitPreAlert();
            }
        }

        private void Config_Changed(object sender, EventArgs e)
        {
            var oldOptions = sender as ProjectEye.Core.Models.Options.OptionsModel;
            if (config.options.Style.IsPreAlert != oldOptions.Style.IsPreAlert ||
                config.options.General.IsTomatoMode != oldOptions.General.IsTomatoMode)
            {
                if (config.options.Style.IsPreAlert && !config.options.General.IsTomatoMode)
                {
                    //当关于预提醒的配置被修改时，重启休息计时和预提醒
                    main.ReStart();
                    InitPreAlert();
                }
                else
                {
                    preAlertTimer?.Stop();
                }
            }
        }

        public void Init()
        {
            InitPreAlert();
        }
        /// <summary>
        /// 初始化预提醒
        /// </summary>
        public void InitPreAlert()
        {

            isPreAlert = bool.Parse(config.options.Style.IsPreAlert.ToString());
            preAlertTime = int.Parse(config.options.Style.PreAlertTime.ToString());
            if (preAlertTimer != null && preAlertTimer.IsEnabled)
            {
                preAlertTimer.Stop();
            }


            if (config.options.Style.IsPreAlert)
            {
                //初始化计时器
                preAlertTimer = new DispatcherTimer();
                preAlertTimer.Tick += new EventHandler(preAlertTimer_Tick);
                preAlertTimer.Interval = new TimeSpan(0, config.options.General.WarnTime - 1, 60 - config.options.Style.PreAlertTime);
                Debug.WriteLine("预提醒将在：" + preAlertTimer.Interval.TotalSeconds);
                preAlertTimer.Start();
            }
        }

        private void preAlertTimer_Tick(object sender, EventArgs e)
        {
            //到达预提醒时间弹出通知
            Debug.WriteLine(DateTime.Now.ToString());

            if (main.IsBreakReset())
            {
                //跳过本次
                SetPreAlertAction(PreAlertAction.Break);
            }
            else
            {
                //预提醒弹出

                preAlertHasTime = config.options.Style.PreAlertTime - 1;

                //通知数据模型
                var toastModel = new PreAlertModel();
                ParseModel(toastModel);

                //通知弹窗
                var toast = new Project1UIToast();
                if (config.options.Style.PreAlertIcon != "")
                {
                    toast.SetIcon(config.options.Style.PreAlertIcon);
                }
                toast.OnAutoHide += Toast_OnAutoHide;
                toast.OnButtonClick += Toast_OnButtonClick;

                //处理禁用时
                var btns = new string[] {
                    "好的",
                    "跳过本次"
                    };
                if (config.options.Behavior.IsDisabledSkip)
                {
                    btns = new string[] {
                    "好的",
                    };
                }
                toast.Alert(toastModel, config.options.Style.PreAlertTime, btns);

                //播放通知提示音
                if (config.options.Style.IsPreAlertSound)
                {
                    sound.Play();
                }

                //计时器
                var timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += (ee, cc) =>
                {
                    preAlertHasTime--;
                    ParseModel(toastModel);
                    if (preAlertHasTime <= 0)
                    {
                        timer.Stop();
                    }
                };
                timer.Start();
            }
        }

        /// <summary>
        /// 通知按钮被点击
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sender"></param>
        private void Toast_OnButtonClick(string name, Project1UIToast sender)
        {
            sender.Hide();
            if (name == "好的")
            {
                SetPreAlertAction(PreAlertAction.Goto);
            }
            else
            {
                //跳过本次
                SetPreAlertAction(PreAlertAction.Break);
            }
        }

        private void Toast_OnAutoHide(Project1UIToast sender, int type = 0)
        {
            //没有点击操作自动关闭时
            if (!sender.IsButtonClicked)
            {
                if (config.options.Behavior.IsDisabledSkip)
                {
                    //禁用了跳过休息
                    SetPreAlertAction(PreAlertAction.Goto);
                }
                else if (config.options.Style.PreAlertAction.Value == "1")
                {
                    //进入本次休息
                    SetPreAlertAction(PreAlertAction.Goto);
                }
                else if (config.options.Style.PreAlertAction.Value == "2")
                {
                    //跳过本次休息
                    SetPreAlertAction(PreAlertAction.Break);
                }
            }
        }

        private void SetPreAlertAction(PreAlertAction preAlertAction)
        {
            //设置预提醒行为
            this.PreAlertAction_ = preAlertAction;
            main.SetPreAlertAction(preAlertAction);
        }

        private void ParseModel(PreAlertModel model)
        {
            model.Title = ParseContent(config.options.Style.PreAlertTitle);
            model.Subtitle = ParseContent(config.options.Style.PreAlertSubtitle);
            model.Message = ParseContent(config.options.Style.PreAlertMessage);
        }
        /// <summary>
        /// 解析文本中的变量
        /// </summary>
        /// <param name="tipContent"></param>
        /// <returns></returns>
        private string ParseContent(string tipContent)
        {
            string pattern = @"\{(?<value>[a-zA-Z]*?)\}";
            var variableArray = Regex.Matches(tipContent, pattern)
                 .OfType<Match>()
                 .Select(m => m.Value)
                 .Distinct();
            foreach (string variable in variableArray)
            {
                string replace = "";
                switch (variable)
                {
                    case "{t}":
                        replace = preAlertHasTime.ToString();
                        break;

                    case "{twt}":
                        //今日用眼时长
                        replace = statistic.GetTodayData().WorkingTime.ToString();
                        break;
                    case "{trt}":
                        //今日休息时长
                        replace = statistic.GetTodayData().ResetTime.ToString();
                        break;
                    case "{tsc}":
                        //今日跳过次数
                        replace = statistic.GetTodayData().SkipCount.ToString();
                        break;
                }
                if (!string.IsNullOrEmpty(replace))
                {
                    tipContent = tipContent.Replace(variable, replace);
                }
            }
            return tipContent;
        }
    }
}
