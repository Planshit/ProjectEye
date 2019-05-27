using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Collections.Generic;
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

        public StatisticViewModel(StatisticService statistic)
        {
            this.statistic = statistic;
            Data = new StatisticModel();
            Data.Working = statistic.GetChartData(StatisticType.WorkingTime);
            Data.Reset = statistic.GetChartData(StatisticType.ResetTime);
            Data.Skip = statistic.GetChartData(StatisticType.SkipCount);
            Data.Labels = statistic.GetChartLabels();
        }

    
    }
}
