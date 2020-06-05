using Newtonsoft.Json;
using Project1.UI.Controls;
using Project1.UI.Controls.Models;
using Project1.UI.Cores;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using ProjectEye.Models.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjectEye.ViewModels
{
    public class TipViewModel : TipModel, IViewModel
    {
        public string ScreenName { get; set; }
        public Window WindowInstance { get; set; }

        /// <summary>
        /// 休息命令
        /// </summary>
        public Command resetCommand { get; set; }
        /// <summary>
        /// 跳过命令
        /// </summary>
        public Command busyCommand { get; set; }

        private readonly RestService reset;
        private readonly SoundService sound;
        private readonly ConfigService config;
        private readonly StatisticService statistic;
        private readonly MainService main;
        private readonly KeyboardShortcutsService keyboardShortcuts;
        private readonly PreAlertService preAlert;
        private readonly ThemeService theme;

        public event ViewModelEventHandler ChangedEvent;

        public TipViewModel(RestService reset,
            SoundService sound,
            ConfigService config,
            StatisticService statistic,
            MainService main,
            App app,
            KeyboardShortcutsService keyboardShortcuts,
            PreAlertService preAlert,
            ThemeService theme)
        {
            this.reset = reset;
            this.reset.TimeChanged += new RestEventHandler(timeChanged);
            this.reset.RestCompleted += new RestEventHandler(resetCompleted);

            this.sound = sound;
            this.config = config;
            this.config.Changed += config_Changed;


            resetCommand = new Command(new Action<object>(resetCommand_action));
            busyCommand = new Command(new Action<object>(busyCommand_action));

            this.statistic = statistic;

            this.main = main;
            this.keyboardShortcuts = keyboardShortcuts;
            this.preAlert = preAlert;
            this.theme = theme;
            theme.OnChangedTheme += Theme_OnChangedTheme;
            ChangedEvent += TipViewModel_ChangedEvent;
            LoadConfig();

        }

        private void Theme_OnChangedTheme(string OldThemeName, string NewThemeName)
        {
            CreateUI();
        }

        private void TipViewModel_ChangedEvent()
        {
            CreateUI();
            //WindowInstance.Activated += WindowInstance_Activated;
            WindowInstance.KeyUp += WindowInstance_KeyUp;
            (WindowInstance as Project1UIWindow).OnWShow += TipViewModel_OnWShow;
        }

        private void TipViewModel_OnWShow(object sender, EventArgs e)
        {
            UpdateVariable();
            if (!config.options.Style.IsThruTipWindow)
            {
                WindowInstance.Focus();
            }
            HandleAutoAction();
        }

        private void WindowInstance_KeyUp(object sender, KeyEventArgs e)
        {
            //处理执行快捷键命令
            keyboardShortcuts.Execute(e);
        }

        private void WindowInstance_Activated(object sender, EventArgs e)
        {
            //UpdateVariable();
            //var window = sender as Window;
            //window.Focus();
            //HandleAutoAction();
        }

        private void UpdateVariable()
        {
            COUNTDOWN = config.options.General.RestTime;
            //提醒间隔变量
            T = config.options.General.WarnTime.ToString();
            //当前时间
            TIME = DateTime.Now.ToString();
            //年
            Y = DateTime.Now.ToString("yyyy");
            //月
            M = DateTime.Now.ToString("MM");
            //日
            D = DateTime.Now.ToString("dd");
            //时
            H = DateTime.Now.ToString("HH");
            //分
            MINUTES = DateTime.Now.ToString("mm");
            //今日用眼时长
            TWT = statistic.GetTodayData().WorkingTime.ToString();
            //今日休息时长
            TRT = statistic.GetTodayData().ResetTime.ToString();
            //今日跳过次数
            TSC = statistic.GetTodayData().SkipCount.ToString();
        }
        private void CreateUI()
        {
            var container = new Grid();
            string uiFilePath = $"UI\\{config.options.Style.Theme.ThemeName}_{ScreenName}.json";
            var data = JsonConvert.DeserializeObject<UIDesignModel>(FileHelper.Read(uiFilePath));
            if (data == null)
            {
                data = theme.GetCreateDefaultTipWindowUI(config.options.Style.Theme.ThemeName, ScreenName);

                FileHelper.Write(uiFilePath, JsonConvert.SerializeObject(data));
            }
            var containerBG = new Border();
            containerBG.Width = Double.NaN;
            containerBG.Height = Double.NaN;
            containerBG.SetValue(Grid.ZIndexProperty, -1);
            containerBG.Background = data.ContainerAttr.Background;
            containerBG.Opacity = data.ContainerAttr.Opacity;
            container.Children.Add(containerBG);
            foreach (var element in data.Elements)
            {
                var ttf = new TranslateTransform()
                {
                    X = element.X,
                    Y = element.Y
                };
                switch (element.Type)
                {
                    case Project1.UI.Controls.Enums.DesignItemType.Text:
                        var textElement = CreateTextElemenet(element);
                        textElement.RenderTransform = ttf;
                        container.Children.Add(textElement);
                        break;
                    case Project1.UI.Controls.Enums.DesignItemType.Button:
                        var buttonElement = CreateButtonElement(element);
                        buttonElement.RenderTransform = ttf;
                        container.Children.Add(buttonElement);
                        break;
                    case Project1.UI.Controls.Enums.DesignItemType.Image:
                        var imageElement = new Image();
                        imageElement.HorizontalAlignment = HorizontalAlignment.Left;
                        imageElement.VerticalAlignment = VerticalAlignment.Top;
                        imageElement.RenderTransform = ttf;
                        imageElement.Width = element.Width;
                        imageElement.Height = element.Height;
                        imageElement.Opacity = element.Opacity;
                        imageElement.Stretch = Stretch.Fill;
                        try
                        {
                            //imageElement.Source = new BitmapImage(new Uri(element.Image, UriKind.RelativeOrAbsolute));
                            imageElement.Source = BitmapImager.Load(element.Image);

                        }
                        catch
                        {
                            imageElement.Source = BitmapImager.Load("pack://application:,,,/Project1.UI;component/Assets/Images/sunglasses.png");
                            //imageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Project1.UI;component/Assets/Images/sunglasses.png", UriKind.RelativeOrAbsolute));
                        }
                        container.Children.Add(imageElement);
                        break;
                }
            }



            WindowInstance.Content = container;
        }

        private Project1UIButton CreateButtonElement(ElementModel element)
        {
            var button = new Project1UIButton();
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Width = element.Width;
            button.Height = element.Height;
            button.FontSize = element.FontSize;
            button.Opacity = element.Opacity;
            button.FontWeight = element.IsTextBold ? FontWeights.Bold : FontWeights.Normal;
            if (element.Style != null)
            {
                var res = WindowInstance.TryFindResource(element.Style);
                if (res != null)
                {
                    button.Style = res as Style;
                }
                else
                {
                    res = WindowInstance.TryFindResource("default");
                    if (res != null)
                    {
                        button.Style = res as Style;
                    }
                }
            }

            button.Content = element.Text;
            var binding = new Binding();
            binding.Source = this;
            binding.Mode = BindingMode.OneWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Path = new PropertyPath("TakeButtonVisibility");

            switch (element.Command)
            {
                case "rest":
                    button.Command = resetCommand;
                    BindingOperations.SetBinding(button, Button.VisibilityProperty, binding);
                    break;
                case "break":
                    button.Command = busyCommand;
                    BindingOperations.SetBinding(button, Button.VisibilityProperty, binding);
                    break;
            }

            return button;
        }
        private TextBlock CreateTextElemenet(ElementModel element)
        {
            var textBlock = new TextBlock();
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.VerticalAlignment = VerticalAlignment.Top;
            textBlock.Width = element.Width;
            textBlock.Height = element.Height;
            textBlock.Opacity = element.Opacity;
            textBlock.FontSize = element.FontSize;
            textBlock.FontWeight = element.IsTextBold ? FontWeights.Bold : FontWeights.Normal;
            textBlock.Foreground = element.TextColor;
            textBlock.TextWrapping = TextWrapping.Wrap;
            TextAlignment textAlignment = TextAlignment.Left;
            switch (element.TextAlignment)
            {
                case 0:
                    textAlignment = TextAlignment.Left;
                    break;
                case 1:
                    textAlignment = TextAlignment.Center;
                    break;
                case 2:
                    textAlignment = TextAlignment.Right;
                    break;
            }
            textBlock.TextAlignment = textAlignment;
            if (!Regex.IsMatch(element.Text, @"\{(.*?)\}"))
            {
                textBlock.Text = element.Text;
            }
            else
            {
                if (Regex.IsMatch(element.Text, @"\{countdown\}"))
                {
                    //带有倒计时变量
                    BindingOperations.SetBinding(textBlock, TextBlock.VisibilityProperty, new Binding()
                    {
                        Source = this,
                        Path = new PropertyPath("CountDownVisibility"),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });
                }
                //带有变量的文本
                var ms = Regex.Matches(element.Text, @"\{(.*?)\}");
                int startIndex = 0;
                for (int i = 0; i < ms.Count; i++)
                {
                    var item = ms[i];
                    string text = string.Empty;
                    if (startIndex != item.Index)
                    {
                        text = element.Text.Substring(startIndex, item.Index - startIndex);
                    }
                    startIndex = startIndex + text.Length + item.Value.Length;
                    if (text != string.Empty)
                    {
                        textBlock.Inlines.Add(text);
                    }
                    var variable = new Run();
                    string matchVariable = item.Value.Replace("{", "").Replace("}", "");
                    BindingOperations.SetBinding(variable, Run.TextProperty, new Binding()
                    {
                        Source = this,
                        Path = new PropertyPath(matchVariable.ToUpper()),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged

                    });
                    textBlock.Inlines.Add(variable);
                    if (i == ms.Count - 1)
                    {
                        //最后一个
                        textBlock.Inlines.Add(element.Text.Substring(startIndex));
                    }
                }

            }
            return textBlock;
        }


        //加载配置
        private void LoadConfig()
        {
            //创建快捷键命令
            if (!string.IsNullOrEmpty(config.options.KeyboardShortcuts.Reset))
            {
                keyboardShortcuts.Set(config.options.KeyboardShortcuts.Reset, resetCommand);
            }
            if (!string.IsNullOrEmpty(config.options.KeyboardShortcuts.NoReset))
            {
                keyboardShortcuts.Set(config.options.KeyboardShortcuts.NoReset, busyCommand);
            }
            //动画开关
            IsAnimation = config.options.Style.IsAnimation;

            //动画类型
            WindowAnimationType = config.options.Style.TipWindowAnimation.AnimationType;

            //鼠标穿透
            IsThruWindow = config.options.Style.IsThruTipWindow;

        }

        //配置文件被修改时
        private void config_Changed(object sender, EventArgs e)
        {
            LoadConfig();
        }

        private void resetCompleted(object sender, int timed)
        {
            //休息结束
            Init();
            //播放提示音
            if (config.options.General.Sound)
            {
                sound.Play();
            }
            //重启计时
            main.ReStart();
        }

        private void Init()
        {
            COUNTDOWN = 20;
            CountDownVisibility = System.Windows.Visibility.Hidden;
            TakeButtonVisibility = System.Windows.Visibility.Visible;
        }

        private void resetCommand_action(object obj)
        {
            main.StopBusyListener();
            CountDownVisibility = System.Windows.Visibility.Visible;
            TakeButtonVisibility = System.Windows.Visibility.Hidden;
            reset.Start();
            if (config.options.General.Data)
            {
                statistic.Add(StatisticType.ResetTime, config.options.General.RestTime);
            }
        }
        private void busyCommand_action(object obj)
        {
            main.StopBusyListener();
            main.ReStart();
            WindowManager.Hide("TipWindow");
            if (config.options.General.Data)
            {
                statistic.Add(StatisticType.SkipCount, 1);
            }
        }
        private void timeChanged(object sender, int timed)
        {
            COUNTDOWN = timed;

        }

        ///// <summary>
        ///// 窗口监听
        ///// </summary>
        //private void WindowsListener()
        //{
        //    var windows = WindowManager.GetWindows("TipWindow");
        //    foreach (var window in windows)
        //    {
        //        window.Activated += Window_Activated;
        //        window.KeyDown += Window_KeyDown;
        //    }
        //}




        /// <summary>
        /// 处理自动操作
        /// </summary>
        private void HandleAutoAction()
        {
            //询问
            if (!config.options.Style.IsTipAsk)
            {
                //进入休息
                resetCommand_action(null);
                return;
            }
            //预提醒
            if (config.options.Style.IsPreAlert)
            {
                if (preAlert.PreAlertAction == PreAlertAction.Goto)
                {
                    //进入休息
                    resetCommand_action(null);
                    return;
                }
            }
            //禁用跳过休息
            if (config.options.Behavior.IsDisabledSkip)
            {
                //进入休息
                resetCommand_action(null);
                return;
            }

        }

        public void OnChanged()
        {
            ChangedEvent?.Invoke();
        }
    }
}
