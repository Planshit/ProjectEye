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
            Analysis();
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
            DateTime lastWeekStartDate = DateTime.Now, lastWeekEndDate = DateTime.Now;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                lastWeekStartDate = DateTime.Now.Date.AddDays(-7);
                lastWeekEndDate = DateTime.Now.Date.AddDays(-1);
            }
            else
            {
                int lastWeekNum = (int)DateTime.Now.DayOfWeek;
                if (lastWeekNum == 0)
                {
                    lastWeekNum = 7;
                }
                lastWeekNum += 6;
                lastWeekStartDate = DateTime.Now.Date.AddDays(-lastWeekNum);
                lastWeekEndDate = lastWeekStartDate.Date.AddDays(6);
            }
            var lastWeekData = statistic.GetData(lastWeekStartDate, lastWeekEndDate);
            Data.LastWeekWork = lastWeekData.Count > 0 ? lastWeekData.Sum(m => m.WorkingTime) : 0;
            Data.LastWeekRest = lastWeekData.Count > 0 ? lastWeekData.Sum(m => m.ResetTime) : 0;
            Data.LastWeekSkip = lastWeekData.Count > 0 ? lastWeekData.Sum(m => m.SkipCount) : 0;

            //计算本周的数据
            DateTime weekStartDate = DateTime.Now, weekEndDate = DateTime.Now;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                weekStartDate = DateTime.Now.Date;
                weekEndDate = DateTime.Now.Date.AddDays(6);
            }
            else
            {
                int weekNum = (int)DateTime.Now.DayOfWeek;
                if (weekNum == 0)
                {
                    weekNum = 7;
                }
                weekNum -= 1;
                weekStartDate = DateTime.Now.Date.AddDays(-weekNum);
                weekEndDate = weekStartDate.Date.AddDays(6);
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

        private void Analysis()
        {
            Data.WorkAnalysis = "正常。工作时间的统计方式是Project Eye运行时且没有进入离开或睡眠状态的总时长。";
            //工作时间
            if (Data.WeekWork > 6)
            {
                //本周平均每天工作时间
                double weekWorkAverage = Data.WeekWork / 7;
                //误差值
                double errValue = 0;
                //工作占用了生活时间的百分比
                double worklifep = Math.Round((11 - (24 - 6 - weekWorkAverage)) / 11 * 100, 0);
                //质量值，越小越好
                double x = weekWorkAverage / 24 - errValue;

                //非常健康
                double l1 = (double)3 / 24;
                //很健康
                double l2 = (double)5 / 24;
                //普通正常人
                double l3 = (double)7 / 24;
                //较忙
                double l4 = (double)9 / 24;
                //很忙
                double l5 = (double)11 / 24;
                //非常忙
                double l6 = (double)13 / 24;
                //危险
                double l7 = (double)15 / 24;

                if (x >= l7)
                {
                    Data.WorkAnalysis = "危险！您本周使用电脑的时间已经超过人体负荷，请务必停止这样的工作状态。";
                }
                else if (x >= l6)
                {
                    Data.WorkAnalysis = "非常忙！您本周使用电脑的时间几乎高于正常水平一倍！除了需要注意您的眼睛状况外还应该保护生活质量。";
                }
                else if (x >= l5)
                {
                    Data.WorkAnalysis = "很忙！您本周使用电脑的时间过长，容易导致近视或近视加重。";
                }
                else if (x >= l4)
                {
                    Data.WorkAnalysis = "较忙！您本周使用电脑的时间略高于正常水平，请注意休息！";
                }
                else if (x >= l3)
                {
                    Data.WorkAnalysis = "正常！您本周使用电脑的时间处于普通上班族水平。";
                }
                else if (x >= l2)
                {
                    Data.WorkAnalysis = "很健康！您本周使用电脑的时间非常少！";
                }
                else
                {
                    Data.WorkAnalysis = "非常健康！";
                }

                if (worklifep > 0)
                {
                    Data.WorkAnalysis += $"平均每天占用了正常生活时间的{worklifep}%。";
                }





            }
        }
    }
}
