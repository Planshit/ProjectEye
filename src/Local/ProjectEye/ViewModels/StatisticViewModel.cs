using Npoi.Mapper;
using Project1.UI.Controls.ChartControl.Models;
using Project1.UI.Cores;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Command exportDataCommand { get; set; }
        public Command openURLCommand { get; set; }


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
            exportDataCommand = new Command(new Action<object>(OnExportDataCommand));
            openURLCommand = new Command(new Action<object>(OnOpenURLCommand));

            Data = new StatisticModel();
            Data.Year = DateTime.Now.Year;
            Data.Month = DateTime.Now.Month;
            Data.MonthRestData = new List<ChartDataModel>();
            Data.MonthWorkData = new List<ChartDataModel>();
            Data.MonthSkipData = new List<ChartDataModel>();
            Data.MonthTomatoData = new List<ChartDataModel>();

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
            LoadImages();
        }



        private void LoadImages()
        {
            string worktimeimgpath = string.IsNullOrEmpty(config.options.Style.DataWindowWorkTimeImagePath) ? "pack://application:,,,/ProjectEye;component/Resources/web_developer.png" : config.options.Style.DataWindowWorkTimeImagePath;
            string resttimeimgpath = string.IsNullOrEmpty(config.options.Style.DataWindowRestTimeImagePath) ? "pack://application:,,,/ProjectEye;component/Resources/coffee_lover.png" : config.options.Style.DataWindowRestTimeImagePath;
            string skipimgpath = string.IsNullOrEmpty(config.options.Style.DataWindowSkipImagePath) ? "pack://application:,,,/ProjectEye;component/Resources/office_work_.png" : config.options.Style.DataWindowSkipImagePath;

            Data.WorktimeImageSource = BitmapImager.Load(worktimeimgpath);
            Data.ResttimeImageSource = BitmapImager.Load(resttimeimgpath);
            Data.SkipImageSource = BitmapImager.Load(skipimgpath);

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
        private void OnOpenURLCommand(object obj)
        {
            Process.Start(new ProcessStartInfo(obj.ToString()));
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
            var MonthTomatoData = new List<ChartDataModel>();

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

            var lastTomatoData = tomato.GetData(lastYear, lastMonth);
            Data.LastMonthTomato = lastTomatoData.Count > 0 ? lastTomatoData.Sum(m => m.TomatoCount) : 0;

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

            //  番茄时钟数据
            var monthTomatoData = tomato.GetData(Data.Year, Data.Month);
            foreach (var data in monthTomatoData)
            {
                bool isSelected = DateTime.Now.Date == data.Date.Date;
                MonthTomatoData.Add(new ChartDataModel()
                {
                    IsSelected = isSelected,
                    PopupText = (isSelected ? $"{Application.Current.Resources["Lang_today"]} " : "") + "{value} ",
                    Tag = data.Date.Day.ToString(),
                    Value = data.TomatoCount
                });
            }
            Data.MonthTomatoData = MonthTomatoData;
            Data.MonthTomato = monthTomatoData.Count > 0 ? monthTomatoData.Sum(m => m.TomatoCount) : 0;

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

            //  本周已过天数
            //int weekNum = Data.WeekTrueWorkDays;
            int weekNum = (int)DateTime.Now.DayOfWeek;
            if (weekNum == 0)
            {
                weekNum = 7;
            }

            //本周平均工作时间
            double weekWorkAverage = weekNum > 0 ? Data.WeekWork / weekNum : 0;

            //工作时间
            if (weekWorkAverage <= 8)
            {
                //  正常
                Data.WorkAnalysis = $"{Application.Current.Resources["Lang_Normal"]}";
                Data.WeekWorkLevel = 0;
            }
            else if (weekWorkAverage > 8 && weekWorkAverage < 10)
            {
                //  较长
                Data.WorkAnalysis = $"{Application.Current.Resources["Lang_Busy"]}";
                Data.WeekWorkLevel = 1;

            }
            else
            {
                //  超负荷工作
                Data.WorkAnalysis = $"{Application.Current.Resources["Lang_Overload"]}";
                Data.WeekWorkLevel = 2;
            }


            //  休息时间
            if (weekWorkAverage <= 0)
            {
                Data.RestAnalysis = $"{Application.Current.Resources["Lang_Normal"]}";
                Data.WeekRestLevel = 0;
            }
            else
            {
                //  按照20-20-20规则为基准每1小时至少休息1分钟
                double avgRest = Data.WeekRest / Data.WeekWork;

                if (avgRest >= 1)
                {
                    //  达成目标
                    Data.RestAnalysis = $"{Application.Current.Resources["Lang_Goodjob"]}";
                    Data.WeekRestLevel = 3;
                }
                else if (avgRest >= 0.8)
                {
                    //  较少
                    Data.RestAnalysis = $"{Application.Current.Resources["Lang_Normal"]}";
                    Data.WeekRestLevel = 0;
                }
                else if (avgRest >= 0.6)
                {
                    //  注意休息
                    Data.RestAnalysis = $"{Application.Current.Resources["Lang_Haveagoodrest"]}";
                    Data.WeekRestLevel = 1;
                }
                else
                {
                    //  疲劳
                    Data.RestAnalysis = $"{Application.Current.Resources["Lang_Exhausted"]}";
                    Data.WeekRestLevel = 2;
                }
            }

            //  跳过次数，每1小时建议有3次休息

            if (weekWorkAverage <= 0 || Data.WeekSkip <= 0)
            {
                //  保持现状
                Data.SkipAnalysis = $"{Application.Current.Resources["Lang_Keepnow"]}";
                Data.WeekSkipLevel = 2;
            }
            else
            {
                double skipRate = Data.WeekSkip / (Data.WeekWork * 3 / 2);
                if (skipRate >= 0.45)
                {
                    //  过于频繁
                    Data.SkipAnalysis = $"{Application.Current.Resources["Lang_Toooften"]}";
                    Data.WeekSkipLevel = 1;
                }
                else
                {
                    //  较少
                    Data.SkipAnalysis = $"{Application.Current.Resources["Lang_Normal"]}";
                    Data.WeekSkipLevel = 0;
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
        public struct XlsxData
        {

        }
        private void OnExportDataCommand(object obj)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Project Eye statistic data " + Data.Year + Data.Month;
                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "(.xlsx)|*.xlsx";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    //  获取选择月份的数据
                    var monthData = statistic.GetData(Data.Year, Data.Month);

                    //  重新构建友好结构

                    var mapper = new Mapper();
                    mapper
                        .Map<Core.Models.Statistic.StatisticModel>("Date 日期", o => o.Date)
                        .Map<Core.Models.Statistic.StatisticModel>("Work(hours) 工作（小时）", o => o.WorkingTime)
                        .Map<Core.Models.Statistic.StatisticModel>("Rest(minutes) 休息（分钟）", o => o.ResetTime)
                        .Map<Core.Models.Statistic.StatisticModel>("Skip 跳过（次）", o => o.SkipCount)
                        .Ignore<Core.Models.Statistic.StatisticModel>(o => o.ID)
                        .Save(dlg.FileName, monthData, $"{Data.Year}{Data.Month}", overwrite: true);
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                MessageBox.Show("导出数据失败，了解详情请查看错误日志");
            }
        }
    }
}
