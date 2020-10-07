using System;
using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Statistic
{
    [XmlRootAttribute("Statistic")]
    public class StatisticModel
    {
        public int ID { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 用眼时长（单位小时）
        /// </summary>
        public double WorkingTime { get; set; } = 0;
        /// <summary>
        /// 休息时长（单位分钟）
        /// </summary>
        public double ResetTime { get; set; } = 0;
        /// <summary>
        /// 跳过次数
        /// </summary>
        public int SkipCount { get; set; } = 0;
    }
}
