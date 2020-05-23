using Project1.UI.Controls;
using ProjectEye.Core.Models.Options;
using ProjectEye.Core.Models.Statistic;
using ProjectEye.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private DispatcherTimer workTimer;
        private DispatcherTimer restTimer;

        private TomatoModel tomatoDataToday;
        private int workCount = 0;
        private int restartCount = -1;
        public TomatoService(
            ConfigService config,
            BackgroundWorkerService backgroundWorker,
            TrayService tray)
        {
            this.config = config;
            this.backgroundWorker = backgroundWorker;
            this.tray = tray;
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
            workTimer.Interval = new TimeSpan(0, 25, 0);
            //休息计时器
            restTimer = new DispatcherTimer();
            restTimer.Tick += RestTimer_Tick; ;
            restTimer.Interval = new TimeSpan(0, 5, 0);
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
                        tomatoDataToday.RestartCount++;
                    }
                }
                else
                {
                    //关闭番茄
                    Close();
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

            string tip = "已结束本次工作时间，请休息5分钟，我会在下一次工作开始时再次提醒您。";
            //停止工作
            workTimer.Stop();

            //计次
            workCount++;
            if (workCount == 4)
            {
                //第四次工作完成，长休息
                restTimer.Interval = new TimeSpan(0, 30, 0);
#if DEBUG
                restTimer.Interval = new TimeSpan(0, 0, 10);
#endif
                tip = "获得一个番茄！完成了一组工作，请休息30分钟，我会在下一次工作开始时再次提醒您。";
            }

            //进入休息时间
            restTimer.Start();
            Dialog("番茄提示", tip);
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
                    tray.SetText($"Project Tomato：[{(workCount + 1)}/4] 工作中");
                }
                if (restTimer.IsEnabled)
                {
                    //休息中
                    tray.SetText($"Project Tomato：[{(workCount + 1)}/4] 休息中");
                }
            }
        }
        #endregion

        #region function

        #region 启动番茄时钟
        public void Start()
        {
            workTimer.Start();
            Dialog("番茄提示", "工作时间已开始，请保持专注。我会在结束时提醒您。");
        }
        #endregion

        #region 关闭番茄时钟
        public void Close()
        {
            workTimer.Stop();
            restTimer.Stop();
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

                if (workCount == 4)
                {
                    //重置工作计次
                    workCount = 0;
                    //重置休息时间
                    restTimer.Interval = new TimeSpan(0, 5, 0);
                    //统计数据
                    tomatoDataToday.TomatoCount++;
                }

                //继续工作计时
                workTimer.Start();

                Dialog("番茄提示", "工作时间已开始，请保持专注。我会在结束时提醒您。");
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
                        db.Tomatos.Add(new TomatoModel()
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
        public TomatoModel FindCreateTodayData()
        {
            return FindCreateTodayData(DateTime.Now.Date);
        }
        /// <summary>
        /// 查找日期数据,如果不存在则创建
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public TomatoModel FindCreateTodayData(DateTime date)
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
                        var dateData = new TomatoModel()
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

        #region 提醒弹窗
        private void Dialog(string title, string content)
        {
            //通知弹窗
            var toast = new Project1UIToast();

            toast.SetIcon("pack://application:,,,/ProjectEye;component/Resources/tomato.ico");
            toast.OnAutoHide += Toast_OnAutoHide;
            toast.OnButtonClick += Toast_OnButtonClick;
            toast.Alert(title, content, "", 0,
                new string[] {
                    "好的",
                    "重新开始"
                });
        }

        private void Toast_OnButtonClick(string name, Project1UIToast sender)
        {
            
        }

        private void Toast_OnAutoHide(Project1UIToast sender, int type)
        {
           
        }
        #endregion
        #endregion
    }
}
