using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        private StackPanel buttonsPanel;

        public Project1UIToast()
        {
            this.DefaultStyleKey = typeof(Project1UIToast);
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
                    buttonsPanel.Children.Add(btn);
                }
            }
        }
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
                this.Visibility = Visibility.Visible;
                CreateButtons(buttons);
                
            };
            this.Show();
        }
    }
}
