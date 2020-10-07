using Project1.UI.Controls.ChartControl.Models;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ProjectEye.ViewModels
{
    public class StatisticViewModel
    {
        public StatisticModel Data { get; set; }

        private readonly StatisticService statistic;
        private readonly ConfigService config;
        private readonly TomatoService tomato;

        private int yearmonth = 0;
        ///// <summary>
        ///// 本周真实工作天数（筛选用眼时长>0）
        ///// </summary>
        //private int weekTrueWorkDays = 0;

        public Command CloseOnboardingCommand { get; set; }
        public Command GenerateMonthlyDataImgCommand { get; set; }

        public StatisticViewModel(
            StatisticService statistic,
            ConfigService config,
            TomatoService tomato)
        {
            this.statistic = statistic;
            this.config = config;
            this.tomato = tomato;

            yearmonth = DateTime.Now.Year + DateTime.Now.Month;

            CloseOnboardingCommand = new Command(new Action<object>(OnCloseOnboardingCommand));
            GenerateMonthlyDataImgCommand = new Command(new Action<object>(OnGenerateMonthlyDataImgCommand));
            Data = new StatisticModel();
            Data.Year = DateTime.Now.Year;
            Data.Month = DateTime.Now.Month;
            Data.MonthRestData = new List<ChartDataModel>();
            Data.MonthWorkData = new List<ChartDataModel>();
            Data.MonthSkipData = new List<ChartDataModel>();

            Data.WeekRestData = new List<ChartDataModel>();
            Data.WeekWorkData = new List<ChartDataModel>();
            Data.WeekSkipData = new List<ChartDataModel>();

            Data.TomatoWeekData = new List<ChartDataModel>();

            Data.PropertyChanged += Data_PropertyChanged;

            Data.IsAnimation = config.options.Style.IsAnimation;

            MigrateCheck();
            HandleMonthData();
            HandleWeekData();
            Analysis();
        }



        private void MigrateCheck()
        {
            Data.IsShowOnboarding = statistic.IsMigrated;
        }
        private void OnCloseOnboardingCommand(object obj)
        {
            Data.IsShowOnboarding = false;
            statistic.MigrateDone();
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
                    PopupText = (isSelected ? $"{Application.Current.Resources["Lang_today"]} " : "") + "{value} " + Application.Current.Resources["Lang_Hours_n"],
                    Tag = data.Date.Day.ToString(),
                    Value = data.WorkingTime
                });

                MonthRestData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = (isSelected ? $"{Application.Current.Resources["Lang_today"]} " : "") + "{value} " + Application.Current.Resources["Lang_Minutes_n"],
                    Tag = data.Date.Day.ToString(),
                    Value = data.ResetTime
                });

                MonthSkipData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = (isSelected ? $"{Application.Current.Resources["Lang_today"]} " : "") + "{value} " + Application.Current.Resources["Lang_x_n"],
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
            var tomatoData = new List<ChartDataModel>();
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

            var lastTomatoData = tomato.GetData(lastWeekStartDate, lastWeekEndDate);
            Data.TomatoLastWeekCount = lastTomatoData.Count > 0 ? lastTomatoData.Sum(m => m.TomatoCount) : 0;
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
            Data.WeekTrueWorkDays = WeekData.Count > 0 ? WeekData.Where(m => m.WorkingTime > 0).Count() : 0;

            var tomatoWeekData = tomato.GetData(weekStartDate, weekEndDate);
            Data.TomatoWeekCount = tomatoWeekData.Count > 0 ? tomatoWeekData.Sum(m => m.TomatoCount) : 0;

            //string[] weekText = { "日", "一", "二", "三", "四", "五", "六" };
            string[] weekText = { $"{Application.Current.Resources["Lang_sun"]}", $"{Application.Current.Resources["Lang_mon"]}", $"{Application.Current.Resources["Lang_tues"]}", $"{Application.Current.Resources["Lang_wed"]}", $"{Application.Current.Resources["Lang_thur"]}", $"{Application.Current.Resources["Lang_fri"]}", $"{Application.Current.Resources["Lang_sat"]}" };

            foreach (var data in WeekData)
            {
                bool isSelected = DateTime.Now.Date == data.Date.Date;
                string weekStr = weekText[(int)data.Date.DayOfWeek];

                //string addStr = isSelected ? "今日 " : data.Date.Month + "月" + data.Date.Day + "日 ";
                string addStr = isSelected ? $"{Application.Current.Resources["Lang_today"]} " : data.Date.Month + $"{Application.Current.Resources["Lang_xmonth"]}" + data.Date.Day + $"{Application.Current.Resources["Lang_xday"]} ";

                WeekWorkData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = addStr + "{value} " + Application.Current.Resources["Lang_Hours_n"],
                    Tag = weekStr,
                    Value = data.WorkingTime
                });

                WeekRestData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = addStr + "{value} " + Application.Current.Resources["Lang_Minutes_n"],
                    Tag = weekStr,
                    Value = data.ResetTime
                });

                WeekSkipData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = addStr + "{value} " + Application.Current.Resources["Lang_x_n"],
                    Tag = weekStr,
                    Value = data.SkipCount
                });
            }

            foreach (var data in tomatoWeekData)
            {
                bool isSelected = DateTime.Now.Date == data.Date.Date;
                string weekStr = weekText[(int)data.Date.DayOfWeek];

                string addStr = isSelected ? $"{Application.Current.Resources["Lang_today"]} " : data.Date.Month + $"{Application.Current.Resources["Lang_xmonth"]}" + data.Date.Day + $"{Application.Current.Resources["Lang_xday"]} ";
                tomatoData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = addStr + "{value}",
                    Tag = weekStr,
                    Value = data.TomatoCount
                });
            }

            Data.WeekRestData = WeekRestData;
            Data.WeekSkipData = WeekSkipData;
            Data.WeekWorkData = WeekWorkData;
            Data.TomatoWeekData = tomatoData;
        }

        private void Analysis()
        {
            if (!config.options.General.IsWeekDataAnalysis)
            {
                //关闭了数据建议
                return;
            }
            //本周天数
            //int weekNum = (int)DateTime.Now.DayOfWeek;
            int weekNum = Data.WeekTrueWorkDays;
            //int weekNum = Data.WeekTrueWorkDays > 1 ? Data.WeekTrueWorkDays - 1 : Data.WeekTrueWorkDays;
            ////减去今日的数据
            //var todayData = statistic.GetTodayData();
            //Data.WeekWork = Data.WeekWork - todayData.WorkingTime;
            //Data.WeekRest = Data.WeekRest - todayData.ResetTime;
            //Data.WeekSkip = Data.WeekSkip - todayData.SkipCount;

            Data.WorkAnalysis = $"{Application.Current.Resources["Lang_Normal"]}";
            //本周平均每天工作时间
            double weekWorkAverage = weekNum > 0 ? Data.WeekWork / weekNum : 0;

            //工作时间
            if (weekWorkAverage >= 3)
            {

                //误差值
                double errValue = 0;
                //工作占用了生活时间的百分比
                double worklifep = Math.Round((11 - (24 - 6 - weekWorkAverage)) / 11 * 100, 0);
                //质量值，越小越好
                double x = weekWorkAverage / 24 - errValue;

                //非常健康
                //double l1 = (double)3 / 24;
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
                    Data.WorkAnalysis = $"{Application.Current.Resources["Lang_Overload"]}";
                }
                else if (x >= l4)
                {
                    Data.WorkAnalysis = $"{Application.Current.Resources["Lang_Busy"]}";
                }






            }

            Data.RestAnalysis = weekWorkAverage > 3 ? $"{Application.Current.Resources["Lang_Exhausted"]}" : $"{Application.Current.Resources["Lang_Normal"]}";
            //休息时间
            if (Data.WeekRest > 0 && weekWorkAverage > 0)
            {
                //根据选项设置每日应休息时间（分钟)
                double optionDayRestM = (int)((weekWorkAverage * 60) / config.options.General.WarnTime * config.options.General.RestTime / 60);
                //本周的平均每日休息时间（分钟）
                double dayRestM = Data.WeekRest / weekNum;
                //是否达成目标
                bool isReached = dayRestM >= optionDayRestM;
                //百分比
                //double reachedTTTP = Math.Round(dayRestM / optionDayRestM * 100, 0);
                Data.RestAnalysis = isReached ? $"{Application.Current.Resources["Lang_Goodjob"]}" : $"{Application.Current.Resources["Lang_Haveagoodrest"]}";
                //if (reachedTTTP < 100)
                //{
                //    Data.RestAnalysis += $"根据20-20-20规则来看，您只达到了{reachedTTTP}%。";
                //}
            }

            Data.SkipAnalysis = $"{Application.Current.Resources["Lang_Normal"]}";
            //跳过次数
            if (weekWorkAverage >= 1 && Data.WeekSkip > 0)
            {
                //根据设置每天应该休息的次数
                double optionDayRestNum = (weekWorkAverage * 60) / config.options.General.WarnTime;
                //根据设置每天建议休息至少一半的次数
                double optionRecommendDayRestNum = optionDayRestNum / 2;
                //当前每天跳过次数
                double daySkipCount = Data.WeekSkip / Data.WeekTrueWorkDays;
                //跳过的次数基于建议的百分比
                double skipP = Math.Round(daySkipCount / optionRecommendDayRestNum * 100, 0);
                if (daySkipCount > optionRecommendDayRestNum)
                {
                    Data.SkipAnalysis = $"{Application.Current.Resources["Lang_Toooften"]}";
                }
                else if (skipP < 10)
                {
                    Data.SkipAnalysis = $"{Application.Current.Resources["Lang_Keepnow"]}";
                }
                else
                {
                    //Data.SkipAnalysis = $"较为频繁，请减少跳过次数！您已接近建议的跳过次数{skipP}%，根据设置以及本周的工作时间，建议您每天的跳过次数应不超过{optionRecommendDayRestNum}次。";
                    Data.SkipAnalysis = $"{Application.Current.Resources["Lang_Toooften"]}";
                }
            }

        }

        private void OnGenerateMonthlyDataImgCommand(object obj)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Project Eye " + Data.Year + Data.Month;
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "(.jpg)|*.jpg";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                new DataReportImageHelper(
               "Project Eye - " + Data.Year + " / " + Data.Month,
              dlg.FileName,
              Data.MonthWorkData,
              Data.LastMonthWork,
              Data.MonthWork).Generate();
            }
        }
    }
}
