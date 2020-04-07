using Project1.UI.Controls.ChartControl.Models;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEye.ViewModels
{
    public class StatisticViewModel
    {
        public StatisticModel Data { get; set; }

        private readonly StatisticService statistic;



        private int yearmonth = 0;

        public StatisticViewModel(StatisticService statistic)
        {
            this.statistic = statistic;

            yearmonth = DateTime.Now.Year + DateTime.Now.Month;

            Data = new StatisticModel();
            Data.Year = DateTime.Now.Year;
            Data.Month = DateTime.Now.Month;
            Data.MonthRestData = new List<ChartDataModel>();
            Data.MonthWorkData = new List<ChartDataModel>();
            Data.MonthSkipData = new List<ChartDataModel>();

            Data.WeekRestData = new List<ChartDataModel>();
            Data.WeekWorkData = new List<ChartDataModel>();
            Data.WeekSkipData = new List<ChartDataModel>();

            Data.PropertyChanged += Data_PropertyChanged;

            HandleMonthData();
            HandleWeekData();
            //int s = new Random().Next(1, 10);
            //for (int i = 0; i < 31; i++)
            //{
            //    chartDatas.Add(new ChartDataModel()
            //    {
            //        Tag = i.ToString(),
            //        IsSelected = i == 34,
            //        PopupText = i == 34 ? "顶峰值：{value}" : "{value}",
            //        Value = (i + s) * (s % (i + 1)),
            //    });
            //}

        }

        private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (yearmonth != Data.Year + Data.Month)
            {
                yearmonth = Data.Year + Data.Month;

                HandleMonthData();
            }
        }

        private void HandleMonthData()
        {
            //处理本月的数据


            var MonthWorkData = new List<ChartDataModel>();
            var MonthRestData = new List<ChartDataModel>();
            var MonthSkipData = new List<ChartDataModel>();

            //计算上个月的数据
            int lastYear = Data.Year;
            int lastMonth = Data.Month;
            if (lastMonth - 1 == 0)
            {
                lastMonth = 12;
                lastYear -= 1;
            }
            else
            {
                lastMonth -= 1;
            }
            var lastMonthData = statistic.GetData(lastYear, lastMonth);
            Data.LastMonthWork = lastMonthData.Count > 0 ? lastMonthData.Sum(m => m.WorkingTime) : 0;
            Data.LastMonthRest = lastMonthData.Count > 0 ? lastMonthData.Sum(m => m.ResetTime) : 0;
            Data.LastMonthSkip = lastMonthData.Count > 0 ? lastMonthData.Sum(m => m.SkipCount) : 0;

            //计算本月的数据
            var monthData = statistic.GetData(Data.Year, Data.Month);
            Data.MonthWork = monthData.Count > 0 ? monthData.Sum(m => m.WorkingTime) : 0;
            Data.MonthRest = monthData.Count > 0 ? monthData.Sum(m => m.ResetTime) : 0;
            Data.MonthSkip = monthData.Count > 0 ? monthData.Sum(m => m.SkipCount) : 0;

            foreach (var data in monthData)
            {
                bool isSelected = DateTime.Now.Date == data.Date.Date;
                MonthWorkData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = (isSelected ? "今日 " : "") + "{value} 小时",
                    Tag = data.Date.Day.ToString(),
                    Value = data.WorkingTime
                });

                MonthRestData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = (isSelected ? "今日 " : "") + "{value} 分钟",
                    Tag = data.Date.Day.ToString(),
                    Value = data.ResetTime
                });

                MonthSkipData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = (isSelected ? "今日 " : "") + "{value} 次",
                    Tag = data.Date.Day.ToString(),
                    Value = data.SkipCount
                });
            }
            Data.MonthRestData = MonthRestData;
            Data.MonthSkipData = MonthSkipData;
            Data.MonthWorkData = MonthWorkData;

        }


        private void HandleWeekData()
        {
            //处理本周的数据


            var WeekWorkData = new List<ChartDataModel>();
            var WeekRestData = new List<ChartDataModel>();
            var WeekSkipData = new List<ChartDataModel>();

            //计算上周的数据

            //计算本周的数据
            DateTime weekStartDate = DateTime.Now, weekEndDate = DateTime.Now;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                weekStartDate = DateTime.Now;
                weekEndDate = DateTime.Now.AddDays(6);
            }
            else
            {
                int weekNum = (int)DateTime.Now.DayOfWeek;
                if (weekNum == 0)
                {
                    weekNum = 7;
                }
                weekNum -= 1;
                weekStartDate = DateTime.Now.AddDays(-weekNum);
                weekEndDate = weekStartDate.AddDays(6);
            }
            var WeekData = statistic.GetData(weekStartDate, weekEndDate);
            Data.WeekWork = WeekData.Count > 0 ? WeekData.Sum(m => m.WorkingTime) : 0;
            Data.WeekRest = WeekData.Count > 0 ? WeekData.Sum(m => m.ResetTime) : 0;
            Data.WeekSkip = WeekData.Count > 0 ? WeekData.Sum(m => m.SkipCount) : 0;

            string[] weekText = { "日", "一", "二", "三", "四", "五", "六" };
            foreach (var data in WeekData)
            {
                bool isSelected = DateTime.Now.Date == data.Date.Date;
                string weekStr = weekText[(int)data.Date.DayOfWeek];

                string addStr = isSelected ? "今日 " : data.Date.Month + "月" + data.Date.Day + "日 ";
                WeekWorkData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = addStr + "{value} 小时",
                    Tag = weekStr,
                    Value = data.WorkingTime
                });

                WeekRestData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = addStr + "{value} 分钟",
                    Tag = weekStr,
                    Value = data.ResetTime
                });

                WeekSkipData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = addStr + "{value} 次",
                    Tag = weekStr,
                    Value = data.SkipCount
                });
            }
            Data.WeekRestData = WeekRestData;
            Data.WeekSkipData = WeekSkipData;
            Data.WeekWorkData = WeekWorkData;

        }

    }
}
