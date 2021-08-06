using Project1.UI.Controls;
using ProjectEye.Core.Models.Options;
using ProjectEye.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 番茄时钟模式服务
    /// </summary>
    public class TomatoService : IService
    {
        private readonly ConfigService config;
        private readonly BackgroundWorkerService backgroundWorker;
        private readonly TrayService tray;
        private readonly SoundService sound;

        private DispatcherTimer workTimer;
        private DispatcherTimer restTimer;
        private DispatcherTimer icorefreshTimer;

        private Models.Statistic.TomatoModel tomatoDataToday;
        private int workCount = 0;
        private int restartCount = -1;
        private int refreshTick = 1;

        private Project1UIToast worktoast;
        private Stopwatch timerWatcher;
        public TomatoService(
            ConfigService config,
            BackgroundWorkerService backgroundWorker,
            TrayService tray,
            SoundService sound)
        {
            this.config = config;
            this.backgroundWorker = backgroundWorker;
            this.tray = tray;
            this.sound = sound;
            timerWatcher = new Stopwatch();
        }

        #region init service
        public void Init()
        {
            backgroundWorker.AddAction(() =>
            {
                //创建本月数据
                CreateMonthlyData();
                //获取设置今日数据
                tomatoDataToday = FindCreateTodayData();
            });

            backgroundWorker.Run();

            config.Changed += Config_Changed;
            tray.MouseClickTrayIcon += Tray_MouseClickTrayIcon;
            tray.MouseMoveTrayIcon += Tray_MouseMoveTrayIcon;
            //工作计时器
            workTimer = new DispatcherTimer();
            workTimer.Tick += WorkTimer_Tick;
            workTimer.Interval = new TimeSpan(0, config.options.Tomato.WorkMinutes, 0);
            //休息计时器
            restTimer = new DispatcherTimer();
            restTimer.Tick += RestTimer_Tick; ;
            restTimer.Interval = new TimeSpan(0, config.options.Tomato.ShortRestMinutes, 0);

            icorefreshTimer = new DispatcherTimer();
            icorefreshTimer.Tick += icorefreshTimer_Tick; ;

            /****调试模式代码****/
#if DEBUG
            workTimer.Interval = new TimeSpan(0, 0, 25);
            restTimer.Interval = new TimeSpan(0, 0, 5);
#endif
        }









        #endregion

        #region event
        private void Config_Changed(object sender, EventArgs e)
        {
            var oldOptions = sender as OptionsModel;
            if (oldOptions.General.IsTomatoMode != config.options.General.IsTomatoMode)
            {
                if (config.options.General.IsTomatoMode)
                {
                    //启动番茄
                    Start();
                    //重置工作次数
                    workCount = 0;
                    //重启计次
                    restartCount++;
                    if (restartCount > 0)
                    {
                        //统计数据
                        tomatoDataToday.RestartCount++;
                    }
                }
                else
                {
                    //关闭番茄
                    Close();
                    if (worktoast != null)
                    {
                        worktoast.Hide();
                    }
                }
            }
        }
        private void Tray_MouseClickTrayIcon(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (config.options.General.IsTomatoMode && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                RestDone();
            }
        }
        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            //工作时间已完成

            //string tip = $"已结束本次工作时间，请休息{config.options.Tomato.ShortRestMinutes}分钟，我会在下一次工作开始时再次提醒您。";
            string tip = Application.Current.Resources["Lang_TomatoWorkfinishTip2"].ToString().Replace("{x}", config.options.Tomato.ShortRestMinutes.ToString());
            string subtitle = "";
            //停止工作
            workTimer.Stop();
            timerWatcher.Stop();
            //计次
            workCount++;
            if (workCount == 4)
            {
                //第四次工作完成，长休息
                restTimer.Interval = new TimeSpan(0, config.options.Tomato.LongRestMinutes, 0);
#if DEBUG
                restTimer.Interval = new TimeSpan(0, 0, 10);
#endif
                //tip = $"获得一个番茄！完成了一组工作，请休息{config.options.Tomato.LongRestMinutes}分钟，我会在下一次工作开始时再次提醒您。";
                string tip1 = Application.Current.Resources["Lang_TomatoWorkfinishTip1"].ToString().Replace("{x}", config.options.Tomato.LongRestMinutes.ToString());
                tip = $"{Application.Current.Resources["Lang_Getatomato"]}{tip1}";

                subtitle = $"{Application.Current.Resources["Lang_Great"]}";
                //数据记录
                tomatoDataToday.TomatoCount++;
                SaveData();
            }


            //进入休息时间
            restTimer.Start();
            timerWatcher.Restart();

            //  启动图标更新
            tray.UpdateIcon("green-tomato-1");
            icorefreshTimer.Interval = new TimeSpan(0, 0, (int)restTimer.Interval.TotalMinutes * 60 / 10);
            icorefreshTimer.Start();


            if (config.options.Tomato.IsEnabledInteractiveModel)
            {
                Dialog($"{Application.Current.Resources["Lang_TomatoTimer"]}", tip, subtitle);
            }
            else
            {
                tray.BalloonTipIcon($"Tomato：{workCount}/4", tip);
            }
        }
        private void RestTimer_Tick(object sender, EventArgs e)
        {
            //休息已完成
            RestDone();
        }
        private void Tray_MouseMoveTrayIcon(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (config.options.General.IsTomatoMode)
            {
                if (workTimer.IsEnabled)
                {
                    //工作中
                    string timestr = string.Format("{0:F}", workTimer.Interval.TotalMinutes - timerWatcher.Elapsed.TotalMinutes);
                    tray.SetText($"Tomato：[{(workCount + 1)}/4] {Application.Current.Resources["Lang_Workingtime"]}\r\n{Application.Current.Resources["Lang_Remainingtime"]}: {timestr} {Application.Current.Resources["Lang_Minutes"]}");
                }
                if (restTimer.IsEnabled)
                {
                    //休息中
                    string timestr = string.Format("{0:F}", restTimer.Interval.TotalMinutes - timerWatcher.Elapsed.TotalMinutes);
                    tray.SetText($"Tomato：[{(workCount + 1)}/4] {Application.Current.Resources["Lang_Breakingtime"]}\r\n{Application.Current.Resources["Lang_Remainingtime"]}: {timestr} {Application.Current.Resources["Lang_Minutes"]}");
                }
            }
        }

        private void icorefreshTimer_Tick(object sender, EventArgs e)
        {
            refreshTick++;
            if (refreshTick >= 10)
            {
                icorefreshTimer.Stop();
                refreshTick = 1;
            }
            else
            {
                //  更新图标
                tray.UpdateIcon((restTimer.IsEnabled ? "green-tomato-" + refreshTick : "red-tomato-" + (10 - refreshTick)));
            }

        }

        #endregion

        #region function

        #region 启动番茄时钟
        public void Start()
        {
            workTimer.Interval = new TimeSpan(0, config.options.Tomato.WorkMinutes, 0);
            restTimer.Interval = new TimeSpan(0, config.options.Tomato.ShortRestMinutes, 0);

            //播放提示音
            if (config.options.Tomato.IsWorkStartSound)
            {
                sound.Play(Enums.SoundType.TomatoWorkStartSound);
            }

            //  交互模式
            if (config.options.Tomato.IsEnabledInteractiveModel)
            {
                WorkDialog();
            }
            else
            {
                workTimer.Start();
                timerWatcher.Restart();
                tray.UpdateIcon("red-tomato-10");
                icorefreshTimer.Interval = new TimeSpan(0, 0, config.options.Tomato.WorkMinutes * 60 / 10);
                icorefreshTimer.Start();
            }
        }
        #endregion

        #region 关闭番茄时钟
        public void Close()
        {
            workCount = 0;
            restartCount++;
            refreshTick = 1;
            workTimer.Stop();
            restTimer.Stop();
            icorefreshTimer.Stop();
            config.SaveOldOptions();
            config.options.General.IsTomatoMode = false;
            config.OnChanged();
            SaveData();
        }
        #endregion

        #region 休息结束
        public void RestDone()
        {
            if (restTimer.IsEnabled)
            {
                Debug.WriteLine("结束番茄时钟休息");
                //停止休息计时
                restTimer.Stop();
                timerWatcher.Stop();
                if (workCount == 4)
                {
                    //重置工作计次
                    workCount = 0;
                    //重置休息时间
                    restTimer.Interval = new TimeSpan(0, config.options.Tomato.ShortRestMinutes, 0);
#if DEBUG
                    restTimer.Interval = new TimeSpan(0, 0, 5);
#endif
                    //统计数据
                    tomatoDataToday.TomatoCount++;
                }

                //继续
                Start();
            }
        }
        #endregion

        #region 创建本月数据
        private void CreateMonthlyData()
        {
            CreateMonthlyData(DateTime.Now);
        }
        /// <summary>
        /// 创建本月的数据
        /// </summary>
        private void CreateMonthlyData(DateTime dateTime)
        {
            int days = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            int today = dateTime.Day;
            using (var db = new StatisticContext())
            {
                for (int i = 0; i < days; i++)
                {
                    var date = dateTime.AddDays(-today + (i + 1)).Date;
                    if (db.Tomatos.Where(m => m.Date == date).Count() == 0)
                    {
                        //补上缺少的日期
                        db.Tomatos.Add(new Models.Statistic.TomatoModel()
                        {
                            Date = date,
                            RestartCount = 0,
                            TomatoCount = 0
                        });
                    }
                }
                db.SaveChanges();
            }
        }
        #endregion

        #region 查找日期数据,如果不存在则创建
        public Models.Statistic.TomatoModel FindCreateTodayData()
        {
            return FindCreateData(DateTime.Now.Date);
        }
        /// <summary>
        /// 查找日期数据,如果不存在则创建
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public Models.Statistic.TomatoModel FindCreateData(DateTime date)
        {
            if (date.Date == DateTime.Now.Date &&
                tomatoDataToday != null &&
                tomatoDataToday.Date == date.Date)
            {
                //当日
                return tomatoDataToday;
            }
            else
            {
                //非当日从数据库中查找
                using (var db = new StatisticContext())
                {
                    var res = db.Tomatos.Where(m => m.Date == date.Date);
                    if (res.Count() == 0)
                    {
                        //数据库中没有时则创建
                        var dateData = new Models.Statistic.TomatoModel()
                        {
                            Date = date.Date,
                            RestartCount = 0,
                            TomatoCount = 0
                        };
                        db.Tomatos.Add(dateData);
                        db.SaveChanges();
                        return dateData;
                    }
                    else
                    {
                        return res.ToList().FirstOrDefault();
                    }
                }
            }
        }

        #endregion

        #region 查找日期范围内的数据
        /// <summary>
        /// 查找范围内的数据
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        public List<Models.Statistic.TomatoModel> GetData(DateTime startDate, DateTime endDate)
        {
            var result = new List<Models.Statistic.TomatoModel>();
            startDate = startDate.AddDays(-1);
            using (var db = new StatisticContext())
            {
                if (db.Tomatos.Where(m => m.Date == endDate.Date).Count() == 0)
                {
                    CreateMonthlyData(endDate);
                }
                result = db.Tomatos.Where(m => m.Date > startDate && m.Date <= endDate).OrderBy(m => m.Date).ToList();
            }
            return result;
        }
        #endregion
        #region 获取某月的数据
        /// <summary>
        /// 获取某月的数据
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns></returns>
        public List<Models.Statistic.TomatoModel> GetData(int year = 0, int month = 0)
        {
            var result = new List<Models.Statistic.TomatoModel>();
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
                result = db.Tomatos.Where(m => m.Date.Year == year && m.Date.Month == month).OrderBy(m => m.Date).ToList();
            }
            return result;
        }
        #endregion
        #region 数据持久化
        /// <summary>
        /// 数据持久化
        /// </summary>
        public void SaveData()
        {
            backgroundWorker.AddAction(() =>
            {
                if (tomatoDataToday == null)
                {
                    tomatoDataToday = FindCreateTodayData();
                }
                using (var db = new StatisticContext())
                {
                    var item = (from c in db.Tomatos where c.Date == tomatoDataToday.Date select c).FirstOrDefault();
                    item.TomatoCount = tomatoDataToday.TomatoCount;
                    item.RestartCount = tomatoDataToday.RestartCount;
                    db.SaveChanges();
                }
            });
            backgroundWorker.Run();
        }
        #endregion

        #region 提醒弹窗

        #region 工作提醒
        //工作提醒
        private void WorkDialog()
        {

            //通知弹窗
            worktoast = new Project1UIToast();
            worktoast.SetIcon("pack://application:,,,/ProjectEye;component/Resources/tomato.ico");
            worktoast.OnButtonClick += Worktoast_OnButtonClick;
            worktoast.OnAutoHide += Worktoast_OnAutoHide;
            worktoast.Alert($"{Application.Current.Resources["Lang_TomatoTimer"]}", $"{Application.Current.Resources["Lang_TomatoWorkfinishTip3"]}", $"{config.options.Tomato.WorkMinutes} {Application.Current.Resources["Lang_Minutes"]}", 60,
                new string[] {
                    $"{Application.Current.Resources["Lang_TomatoStart"]}",
                    $"{Application.Current.Resources["Lang_TomatoEnd"]}"
                });
        }

        private void Worktoast_OnAutoHide(Project1UIToast sender, int type = 0)
        {
            //超时没有点击确定进入工作时间时退出番茄时钟
            Close();
        }

        private void Worktoast_OnButtonClick(string name, Project1UIToast sender)
        {
            if (name == $"{Application.Current.Resources["Lang_TomatoStart"]}")
            {
                workTimer.Start();
                timerWatcher.Restart();
                tray.UpdateIcon("red-tomato-10");
                icorefreshTimer.Interval = new TimeSpan(0, 0, config.options.Tomato.WorkMinutes * 60 / 10);
                icorefreshTimer.Start();
            }
            else
            {
                Close();
            }
            sender.Hide();
        }


        #endregion

        private void Dialog(string title, string content, string subtitle = "")
        {
            //播放提示音
            if (config.options.Tomato.IsWorkEndSound)
            {
                sound.Play(Enums.SoundType.TomatoWorkEndSound);
            }
            //通知弹窗
            var toast = new Project1UIToast();

            toast.SetIcon("pack://application:,,,/ProjectEye;component/Resources/tomato.ico");
            toast.OnButtonClick += Toast_OnButtonClick;
            toast.Alert(title, content, subtitle, 30,
                new string[] {
                    $"{Application.Current.Resources["Lang_Yes"]}",
                });
        }

        private void Toast_OnButtonClick(string name, Project1UIToast sender)
        {
            sender.Hide();
        }

        #endregion


        #endregion
    }
}
