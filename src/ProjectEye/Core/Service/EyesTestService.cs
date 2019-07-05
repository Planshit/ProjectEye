using ProjectEye.Core.Models;
using ProjectEye.Core.Models.EyesTest;
using ProjectEye.Core.Models.Statistic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEye.Core.Service
{
   
    public class EyesTestService : IService
    {
        private readonly string xmlPath;
        private readonly App app;
        private XmlExtensions xml;
        //存放文件夹
        private readonly string dir = "Data";
        //统计数据
        private EyesTestListModel statisticList;
 
        public EyesTestService(App app)
        {
            this.app = app;
            this.app.Exit += app_Exit;
            statisticList = new EyesTestListModel();
            statisticList.Data = new List<EyesTestModel>();
            xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                dir,
                "eyestest.xml");
            xml = new XmlExtensions(xmlPath);
        }

        private void app_Exit(object sender, ExitEventArgs e)
        {
            Save();
        }

        public void Init()
        {
            LoadStatisticData();
            
        }

        #region 添加统计数据
        public void SetTodayData(double score)
        {
            var today = statisticList.Data.Where(m => m.Date == DateTime.Now.Date);
            var todayData = new EyesTestModel();
            if (today.Count() != 1)
            {
                //不存在
                todayData.Date = DateTime.Now.Date;
                todayData.Score = score;
                statisticList.Data.Add(todayData);
            }
            else
            {
                todayData = today.Single();
                todayData.Score = score;
            }
        }
        #endregion

        #region 加载统计数据
        /// <summary>
        /// 加载统计数据
        /// </summary>
        public void LoadStatisticData()
        {
            if (File.Exists(xmlPath))
            {
                var data = xml.ToModel(typeof(EyesTestListModel));
                if (data != null)
                {
                    statisticList = data as EyesTestListModel;
                }
                else
                {
                    xml.Save(statisticList);
                }
            }
            else
            {
                xml.Save(statisticList);
            }

        }

        #endregion  

        #region 获取图表数据
        /// <summary>
        /// 获取图表数据
        /// </summary>
        /// <returns></returns>
        public double[] GetChartData()
        {
            ArrayList result = new ArrayList();
            foreach (EyesTestModel statistic in statisticList.Data)
            {
                result.Add(statistic.Score);
            }
            return (double[])result.ToArray().Select(o => Convert.ToDouble(o)).ToArray();
        }
        #endregion

        #region 获取图表标签
        /// <summary>
        /// 获取图表标签
        /// </summary>
        /// <returns></returns>
        public string[] GetChartLabels()
        {
            ArrayList result = new ArrayList();
            foreach (EyesTestModel statistic in statisticList.Data)
            {

                result.Add(statistic.Date.Day + "日");

            }
            return (string[])result.ToArray(typeof(string));
        }
        #endregion

        #region 数据持久化
        /// <summary>
        /// 数据持久化
        /// </summary>
        public void Save()
        {
            xml.Save(statisticList);
        }
        #endregion

      
    }
}
