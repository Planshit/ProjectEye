using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Project1.UI.Controls
{
    public class Project1UIToast : Window
    {
        #region 依赖属性
        #region 消息
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(Project1UIToast));
        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        #endregion
        #region 副标题
        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register("Subtitle", typeof(string), typeof(Project1UIToast));
        /// <summary>
        /// 消息
        /// </summary>
        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }
        #endregion

        #endregion

        #region 事件通知
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        /// <param name="name">按钮名</param>
        /// <param name="sender">按钮所在的通知窗口</param>
        public delegate void ButtonClickEventHandler(string name, Project1UIToast sender);
        /// <summary>
        /// 按钮被点击时发生
        /// </summary>
        public event ButtonClickEventHandler OnButtonClick;
        /// <summary>
        /// 通用事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="type"></param>
        public delegate void ToastEventHandler(Project1UIToast sender, int type = 0);
        public event ToastEventHandler OnAutoHide;
        #endregion
        /// <summary>
        /// 指示当前通知按钮是否被点击
        /// </summary>
        public bool IsButtonClicked { get; set; } = false;
        private StackPanel buttonsPanel;
        private DispatcherTimer timer;
        private Storyboard alertStoryboard, closeStoryboard;
        public Project1UIToast()
        {
            this.DefaultStyleKey = typeof(Project1UIToast);
            alertStoryboard = new Storyboard();
            closeStoryboard = new Storyboard();
            //alertStoryboard.Duration = TimeSpan.FromSeconds(1);
            //closeStoryboard.Duration = TimeSpan.FromSeconds(1);
            closeStoryboard.Completed += CloseStoryboard_Completed;

            Left = SystemParameters.PrimaryScreenWidth;
            Top = 0;
            SetIcon("pack://application:,,,/Project1.UI;component/Assets/Images/sunglasses.png");
            ShowActivated = false;
        }

        /// <summary>
        /// 设置通知图标路径
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        public void SetIcon(string filePath)
        {
            //        Icon = new BitmapImage(
            //new Uri(filePath));
            Icon = BitmapImager.Load(filePath);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            buttonsPanel = GetTemplateChild("buttonsPanel") as StackPanel;
        }
        private void CreateButtons(string[] buttons)
        {
            if (buttonsPanel != null)
            {
                foreach (string name in buttons)
                {
                    var btn = new Project1UIButton();
                    btn.Content = name;
                    btn.Margin = new Thickness(10, 0, 0, 0);
                    btn.Click += (e, c) =>
                    {
                        IsButtonClicked = true;
                        OnButtonClick?.Invoke(name, this);
                    };
                    if ((buttons.Last() != name))
                    {
                        btn.Style = (Style)this.FindResource("basic");
                    }
                    buttonsPanel.Children.Add(btn);
                }
            }
        }
        /// <summary>
        /// 弹出通知
        /// </summary>
        /// <param name="title">通知主标题</param>
        /// <param name="message">通知内容</param>
        /// <param name="subtitle">通知副标题</param>
        /// <param name="duration">通知持续时间（秒）</param>
        /// <param name="buttons">通知按钮文本</param>
        public void Alert(string title, string message,
        string subtitle = "",
        int duration = 0,
        string[] buttons = null)
        {

            this.Title = title;
            this.Message = message;
            this.Subtitle = subtitle;
            this.Loaded += (e, c) =>
            {
                CreateButtons(buttons);
                CreateAnimation();
                CreateTimer(duration);
                alertStoryboard.Begin();
            };
            this.Show();
            ToastManager.Add(this);
        }

        /// <summary>
        /// 弹出通知
        /// </summary>
        /// <param name="model">通知数据模型</param>
        /// <param name="duration">显示时长（默认永久）</param>
        /// <param name="buttons">按钮</param>
        public void Alert(object model,
            int duration = 0,
            string[] buttons = null)
        {
            this.DataContext = model;
            var titleBinding = new Binding("Title");
            this.SetBinding(TitleProperty, titleBinding);
            var messageBinding = new Binding("Message");
            this.SetBinding(MessageProperty, messageBinding);
            var subtitleBinding = new Binding("Subtitle");
            this.SetBinding(SubtitleProperty, subtitleBinding);

            this.Loaded += (e, c) =>
            {
                CreateButtons(buttons);
                CreateAnimation();
                CreateTimer(duration);
                alertStoryboard.Begin();
            };
            this.Show();
        }

        /// <summary>
        /// 隐藏（关闭）此条通知
        /// </summary>
        public new void Hide()
        {
            timer?.Stop();
            closeStoryboard.Begin();
        }

        /// <summary>
        /// 创建通知计时器
        /// </summary>
        /// <param name="duration"></param>
        private void CreateTimer(int duration)
        {
            if (duration > 0)
            {
                timer = new DispatcherTimer();
                //需要提前1秒
                timer.Interval = new TimeSpan(0, 0, duration > 1 ? duration - 1 : duration);
                timer.Tick += (e, c) =>
                {
                    OnAutoHide?.Invoke(this);
                    timer.Stop();
                    this.Hide();
                };
                timer.Start();
            }
        }
        #region 动画
        #region 创建动画
        private void CreateAnimation()
        {
            //弹出动画
            DoubleAnimation alertDA = new DoubleAnimation();
            alertDA.To = SystemParameters.PrimaryScreenWidth - this.ActualWidth;
            alertDA.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            alertDA.Duration = TimeSpan.FromSeconds(.5);
            Storyboard.SetTarget(alertDA, this);
            Storyboard.SetTargetProperty(alertDA, new PropertyPath(Canvas.LeftProperty));
            //关闭动画
            DoubleAnimation closeDA = new DoubleAnimation();
            closeDA.To = SystemParameters.PrimaryScreenWidth;
            closeDA.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            closeDA.Duration = TimeSpan.FromSeconds(.5);
            Storyboard.SetTarget(closeDA, this);
            Storyboard.SetTargetProperty(closeDA, new PropertyPath(Canvas.LeftProperty));

            alertStoryboard.Children.Add(alertDA);
            closeStoryboard.Children.Add(closeDA);
        }
        #endregion

        /// <summary>
        /// 关闭动画结束时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseStoryboard_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
