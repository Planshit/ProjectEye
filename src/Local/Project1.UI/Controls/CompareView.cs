using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Project1.UI.Controls
{
    public enum CompareValueType
    {
        /// <summary>
        /// 上升
        /// </summary>
        Up,
        /// <summary>
        /// 下降
        /// </summary>
        Down,
        /// <summary>
        /// 无变化
        /// </summary>
        NoChange
    }
    /// <summary>
    /// 数据对比控件
    /// </summary>
    public class CompareView : Control
    {
        #region 用于比较的旧数据值A
        /// <summary>
        /// 用于比较的旧数据值A
        /// </summary>
        public double DataA
        {
            get { return (double)GetValue(DataAProperty); }
            set { SetValue(DataAProperty, value); }
        }
        public static readonly DependencyProperty DataAProperty =
            DependencyProperty.Register("DataA",
                typeof(double),
                typeof(CompareView),
                new PropertyMetadata((double)0, new PropertyChangedCallback(OnDataChanged)));


        #endregion

        #region 用于比较且显示的新数据值B
        /// <summary>
        /// 用于比较且显示的新数据值B
        /// </summary>
        public double DataB
        {
            get { return (double)GetValue(DataBProperty); }
            set { SetValue(DataBProperty, value); }
        }
        public static readonly DependencyProperty DataBProperty =
            DependencyProperty.Register("DataB",
                typeof(double),
                typeof(CompareView),
                new PropertyMetadata((double)0, new PropertyChangedCallback(OnDataChanged)));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CompareView;
            control.Calculate();
        }


        #endregion

        #region 显示的差异百分比
        /// <summary>
        /// 显示的差异百分比
        /// </summary>
        public double DiffValue
        {
            get { return (double)GetValue(DiffValueProperty); }
            set { SetValue(DiffValueProperty, value); }
        }
        public static readonly DependencyProperty DiffValueProperty =
            DependencyProperty.Register("DiffValue",
                typeof(double),
                typeof(CompareView),
                new PropertyMetadata((double)0));


        #endregion

        #region 差异值状态
        /// <summary>
        /// 差异值状态
        /// </summary>
        public CompareValueType DiffValueStatus
        {
            get { return (CompareValueType)GetValue(UpProperty); }
            set { SetValue(UpProperty, value); }
        }
        public static readonly DependencyProperty UpProperty =
            DependencyProperty.Register("DiffValueStatus",
                typeof(CompareValueType),
                typeof(CompareView),
                new PropertyMetadata(CompareValueType.NoChange));


        #endregion

        #region 上升颜色
        /// <summary>
        /// 上升颜色
        /// </summary>
        public Brush UPColor
        {
            get { return (Brush)GetValue(UPColorProperty); }
            set { SetValue(UPColorProperty, value); }
        }
        public static readonly DependencyProperty UPColorProperty =
            DependencyProperty.Register("UPColor",
                typeof(Brush),
                typeof(CompareView),
                new PropertyMetadata(Brushes.Red));


        #endregion

        #region 下降颜色
        /// <summary>
        /// 下降颜色
        /// </summary>
        public Brush DownColor
        {
            get { return (Brush)GetValue(DownColorProperty); }
            set { SetValue(DownColorProperty, value); }
        }
        public static readonly DependencyProperty DownColorProperty =
            DependencyProperty.Register("DownColor",
                typeof(Brush),
                typeof(CompareView),
                new PropertyMetadata(Brushes.Green));


        #endregion

        #region 无变化颜色
        /// <summary>
        /// 无变化颜色
        /// </summary>
        public Brush NoChangeColor
        {
            get { return (Brush)GetValue(NoChangeColorProperty); }
            set { SetValue(NoChangeColorProperty, value); }
        }
        public static readonly DependencyProperty NoChangeColorProperty =
            DependencyProperty.Register("NoChangeColor",
                typeof(Brush),
                typeof(CompareView),
                new PropertyMetadata(Brushes.Gray));


        #endregion

        #region 提示文本
        /// <summary>
        /// 提示文本
        /// </summary>
        public string PopupText
        {
            get { return (string)GetValue(PopupTextProperty); }
            set { SetValue(PopupTextProperty, value); }
        }
        public static readonly DependencyProperty PopupTextProperty =
            DependencyProperty.Register("PopupText",
                typeof(string),
                typeof(CompareView),
                new PropertyMetadata("相比上次增加{diffvalue}"));


        #endregion

        private TextBlock DiffValueTextBlock;
        private TextBlock PopupTextBlock;
        public CompareView()
        {
            DefaultStyleKey = typeof(CompareView);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DiffValueTextBlock = GetTemplateChild("DiffValue") as TextBlock;
            PopupTextBlock = GetTemplateChild("PopupText") as TextBlock;
            Calculate();
        }

        private void Calculate()
        {
            if (DiffValueTextBlock == null)
            {
                return;
            }
            DiffValueStatus = DataB > DataA ? CompareValueType.Up : DataB < DataA ? CompareValueType.Down : CompareValueType.NoChange;

            double c = DataB - DataA;

            if (c != 0)
            {
                DiffValue = Math.Round(c / DataA * 100, 1);
                if (DataA == 0)
                {
                    DiffValue = 100;
                }
            }

            switch (DiffValueStatus)
            {
                case CompareValueType.Up:
                    DiffValueTextBlock.Foreground = UPColor;
                    break;
                case CompareValueType.Down:
                    DiffValueTextBlock.Foreground = DownColor;
                    break;
                case CompareValueType.NoChange:
                    DiffValueTextBlock.Foreground = NoChangeColor;
                    break;
            }
            string popupText = PopupText;

            popupText = popupText.Replace("{a}", $"{DataA.ToString()}");
            popupText = popupText.Replace("{b}", $"{DataB.ToString()}");

            string diffvalueText = DiffValue > 0 ? $"+{DiffValue.ToString()}%" : DiffValue == 0 ? "无变化" : $"{DiffValue.ToString()}%";
            if (DataA == DataB)
            {
                diffvalueText = "无变化";
            }
            popupText = popupText.Replace("{diffvalue}", diffvalueText);
            PopupTextBlock.Text = popupText;
        }
    }
}
