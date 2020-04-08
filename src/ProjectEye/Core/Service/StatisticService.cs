using ProjectEye.Core.Models;
using ProjectEye.Core.Models.Statistic;
using ProjectEye.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 统计类型
    /// </summary>
    public enum StatisticType
    {
        /// <summary>
        /// 用眼时长
        /// </summary>
        WorkingTime,
        /// <summary>
        /// 休息时长
        /// </summary>
        ResetTime,
        /// <summary>
        /// 跳过次数
        /// </summary>
        SkipCount
    }
    /// <summary>
    /// 统计 Service
    /// 记录和管理用眼统计数据
    /// </summary>
    public class StatisticService : IService
    {
        private readonly string xmlPath;
        private readonly App app;
        private readonly BackgroundWorkerService backgroundWorker;
        private XmlExtensions xml;
        //存放文件夹
        private readonly string dir = "Data";
        //统计数据
        /// <summary>
        /// 统计数据（旧版本使用的XML文件存放，仅用于迁移以后不再使用）
        /// </summary>
        private StatisticListModel statisticList;

        /// <summary>
        /// 今日数据
        /// </summary>
        private StatisticModel todayStatistic;

        /// <summary>
        /// 用眼开始时间
        /// </summary>
        private DateTime useEyeStartTime { get; set; }
        public StatisticService(
            App app,
            BackgroundWorkerService backgroundWorker)
        {
            this.app = app;
            this.backgroundWorker = backgroundWorker;
            this.app.Exit += app_Exit;
            statisticList = new StatisticListModel();
            statisticList.Data = new List<StatisticModel>();
            xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                dir,
                "statistic.xml");

        }


        private void app_Exit(object sender, ExitEventArgs e)
        {
            Save();
        }

        public void Init()
        {
            backgroundWorker.AddAction(() =>
            {
                //迁移旧版本的XML数据
                MigrateXMLDataToDb();
                //创建本月数据
                CreateMonthlyItems();
                //获取设置今日数据
                todayStatistic = FindCreate();
            });
            backgroundWorker.AddAction(() =>
            {
                Thread.Sleep(5000);
                Debug.WriteLine("Thread.Sleep(5000);");
            });
            backgroundWorker.Run();
            //开始计时
            ResetStatisticTime();
        }

        //new code
        #region 迁移数据，从xml到sqlite。下一个版本将弃用 ：）
        /// <summary>
        /// 迁移数据，从xml到sqlite。下一个版本将弃用 ：）
        /// </summary>
        private void MigrateXMLDataToDb()
        {
            if (File.Exists(xmlPath))
            {
                //需要迁移
                xml = new XmlExtensions(xmlPath);

                var data = xml.ToModel(typeof(StatisticListModel));
                if (data != null)
                {
                    statisticList = data as StatisticListModel;

                    if (statisticList != null && statisticList.Data != null && statisticList.Data.Count > 0)
                    {
                        using (var db = new StatisticContext())
                        {
                            foreach (var item in statisticList.Data)
                            {
                                db.Statistics.Add(item);
                            }
                            db.SaveChanges();
                            //备份数据文件
                            File.Copy(xmlPath, xmlPath + ".migrate.backup");
                            //删除原数据文件
                            File.Delete(xmlPath);
                            //迁移标记文件，在用户收到迁移提示后将删除
                            File.WriteAllText(xmlPath + ".migrate.mark", "");
                        }
                    }

                }
            }
        }
        #endregion

        #region 创建本月数据
        /// <summary>
        /// 创建本月的数据
        /// </summary>
        private void CreateMonthlyItems()
        {
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int today = DateTime.Now.Day;
            using (var db = new StatisticContext())
            {
                for (int i = 0; i < days; i++)
                {
                    var date = DateTime.Now.AddDays(-today + (i + 1)).Date;
                    if (db.Statistics.Where(m => m.Date == date).Count() == 0)
                    {
                        //补上缺少的日期
                        db.Statistics.Add(new StatisticModel()
                        {
                            Date = date,
                            ResetTime = 0,
                            SkipCount = 0,
                            WorkingTime = 0
                        });
                    }
                }
                db.SaveChanges();
            }
        }
        #endregion

        #region 更新今日统计数据
        /// <summary>
        /// 更新今日统计数据(叠加)
        /// </summary>
        /// <param name="type">统计类型</param>
        /// <param name="value">增加的值(可以为负数)</param>
        public void Add(StatisticType type, double value)
        {
            if (todayStatistic == null ||
                todayStatistic.Date.Date != DateTime.Now.Date)
            {
                //保存之前的数据
                Save();
                //更新当日数据
                todayStatistic = FindCreate(DateTime.Now.Date);
            }

            switch (type)
            {
                case StatisticType.WorkingTime:
                    todayStatistic.WorkingTime = Math.Round(todayStatistic.WorkingTime + value / 60, 1);
                    break;
                case StatisticType.ResetTime:
                    todayStatistic.ResetTime = Math.Round(todayStatistic.ResetTime + value / 60, 1);
                    break;
                case StatisticType.SkipCount:
                    todayStatistic.SkipCount += (int)value;
                    break;
            }
        }
        #endregion

        #region 查找日期数据,如果不存在则创建
        /// <summary>
        /// 查找日期数据,如果不存在则创建
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public StatisticModel FindCreate(DateTime date = default)
        {
            if (date == default)
            {
                date = DateTime.Now;
            }
            if (date.Date == DateTime.Now.Date &&
                todayStatistic != null)
            {
                //当日
                return todayStatistic;
            }
            else
            {
                //非当日从数据库中查找
                using (var db = new StatisticContext())
                {
                    var res = db.Statistics.Where(m => m.Date == date.Date);
                    if (res.Count() == 0)
                    {
                        //数据库中没有时则创建
                        var dateData = GetNewdayData(date);
                        db.Statistics.Add(dateData);
                        db.SaveChanges();
                        return dateData;
                    }
                    else
                    {
                        return res.ToList().Single();
                    }
                }
            }

            //var data = statisticList.Data.Where(m => m.Date == date);
            //if (data.Count() == 1)
            //{
            //    return data.Single();
            //}
        }

        #endregion

        #region 获取一个初始的指定日期统计模型数据
        /// <summary>
        /// 获取一个初始的指定日期统计模型数据
        /// </summary>
        /// <param name="date">日期，留空为当日</param>
        /// <returns></returns>
        private StatisticModel GetNewdayData(DateTime date = default)
        {
            return new StatisticModel()
            {
                Date = date.Date,
                ResetTime = 0,
                SkipCount = 0,
                WorkingTime = 0
            };
        }
        #endregion

        #region 数据持久化
        /// <summary>
        /// 数据持久化
        /// </summary>
        public void Save()
        {
            if (todayStatistic == null)
            {
                todayStatistic = FindCreate();
            }
            using (var db = new StatisticContext())
            {
                //var item = db.Statistics.Where(m => m.Date == todayStatistic.Date).Single();
                var item = (from c in db.Statistics where c.Date == todayStatistic.Date select c).FirstOrDefault();
                item.ResetTime = todayStatistic.ResetTime;
                item.SkipCount = todayStatistic.SkipCount;
                item.WorkingTime = todayStatistic.WorkingTime;

                db.SaveChanges();
            }
            //xml.Save(statisticList);

        }
        #endregion

        #region 计算获得用眼时长
        /// <summary>
        /// 计算获得用眼时长
        /// </summary>
        /// <returns>返回开始统计到当前的总分钟数</returns>
        public double GetCalculateUseEyeMinutes()
        {
            if (useEyeStartTime != null)
            {
                return DateTime.Now.Subtract(useEyeStartTime).TotalMinutes;
            }
            return 0;
        }
        #endregion

        #region 统计用眼时长数据
        /// <summary>
        /// 统计用眼时长数据
        /// </summary>
        public void StatisticUseEyeData()
        {
            double use = GetCalculateUseEyeMinutes();
            if (use > 0)
            {
                Debug.WriteLine("用眼时长 +" + use + " 分钟");
                //增加统计
                Add(StatisticType.WorkingTime, use);
                //重置统计时间
                ResetStatisticTime();
            }
        }
        #endregion

        #region 重置统计时间
        /// <summary>
        /// 重置统计时间
        /// </summary>
        public void ResetStatisticTime()
        {
            useEyeStartTime = DateTime.Now;
        }
        #endregion

        #region 获取今日数据
        public StatisticModel GetTodayData()
        {
            return todayStatistic;
        }
        #endregion

        #region 获取某月的数据
        /// <summary>
        /// 获取某月的数据
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns></returns>
        public List<StatisticModel> GetData(int year = 0, int month = 0)
        {
            var result = new List<StatisticModel>();
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            if (month == 0)
            {
                month = DateTime.Now.Month;
            }
            using (var db = new StatisticContext())
            {
                result = db.Statistics.Where(m => m.Date.Year == year && m.Date.Month == month).ToList();
            }
            return result;
        }
        #endregion

        #region 查找日期范围内的数据
        /// <summary>
        /// 查找范围内的数据
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        public List<StatisticModel> GetData(DateTime startDate, DateTime endDate)
        {
            var result = new List<StatisticModel>();
            startDate = startDate.AddDays(-1);
            using (var db = new StatisticContext())
            {
                result = db.Statistics.Where(m => m.Date > startDate && m.Date <= endDate).ToList();
            }
            return result;
        }
        #endregion

    }
}
