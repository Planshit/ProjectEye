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

        public ObservableCollection<ChartDataModel> chartDatas { get; set; }

        public StatisticViewModel(StatisticService statistic)
        {
            this.statistic = statistic;
            Data = new StatisticModel();
            Data.Working = statistic.GetChartData(StatisticType.WorkingTime);
            Data.Reset = statistic.GetChartData(StatisticType.ResetTime);
            Data.Skip = statistic.GetChartData(StatisticType.SkipCount);
            Data.Labels = statistic.GetChartLabels();

            chartDatas = new ObservableCollection<ChartDataModel>();
            chartDatas.Add(new ChartDataModel()
            {
                Name = "1",
                Value = 20
            });
            chartDatas.Add(new ChartDataModel()
            {
                Name = "2",
                Value = 60
            });

        }


    }
}
