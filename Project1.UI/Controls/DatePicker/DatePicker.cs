using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project1.UI.Controls.DatePicker
{
    /// <summary>
    /// 月份日期选择器
    /// </summary>
    public class DatePicker : Control
    {
        #region 选中年份
        /// <summary>
        /// 选中年份
        /// </summary>
        public int SelectedYear
        {
            get { return (int)GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }
        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear",
                typeof(int),
                typeof(DatePicker),
                new PropertyMetadata((int)2019, new PropertyChangedCallback(OnSelectChanged)));
        #endregion

        #region 选中月份
        /// <summary>
        /// 选中年份
        /// </summary>
        public int SelectedMonth
        {
            get { return (int)GetValue(SelectedMonthProperty); }
            set { SetValue(SelectedMonthProperty, value); }
        }
        public static readonly DependencyProperty SelectedMonthProperty =
            DependencyProperty.Register("SelectedMonth",
                typeof(int),
                typeof(DatePicker),
                new PropertyMetadata((int)1, new PropertyChangedCallback(OnSelectChanged)));
        #endregion
        private static void OnSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DatePicker;
            if (control != null)
            {
                control.HandleSelectChange();
            }
        }

        private ItemList YearsList;
        private ItemList MonthsList;
        public DatePicker()
        {
            DefaultStyleKey = typeof(DatePicker);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            YearsList = GetTemplateChild("YearsList") as ItemList;
            MonthsList = GetTemplateChild("MonthsList") as ItemList;

            YearsList.OnSelected += YearsList_OnSelected;
            MonthsList.OnSelected += MonthsList_OnSelected;
            RenderDate();
            HandleSelectChange();
        }

        private void MonthsList_OnSelected(object sender, int value)
        {
            SelectedMonth = value;
        }

        private void YearsList_OnSelected(object sender, int value)
        {
            SelectedYear = value;
        }

        private void RenderDate()
        {
            var years = new List<int>();
            int endYear = DateTime.Now.Year + 1;
            int startYear = endYear - 12;
            for (int i = startYear; i < endYear; i++)
            {
                years.Add(i);
            }
            YearsList.Items = years;

            var months = new List<int>();
            for (int i = 1; i < 13; i++)
            {
                months.Add(i);
            }
            MonthsList.Items = months;
        }

        private void HandleSelectChange()
        {
            if (YearsList != null)
            {
                YearsList.SelectedValue = SelectedYear;
            }
            if (MonthsList != null)
            {
                MonthsList.SelectedValue = SelectedMonth;
            }

        }
    }
}
