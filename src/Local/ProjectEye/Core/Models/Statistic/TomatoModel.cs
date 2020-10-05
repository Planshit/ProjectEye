using System;

namespace ProjectEye.Core.Models.Statistic
{
    /// <summary>
    /// 番茄工作数据统计实体模型
    /// </summary>
    public class TomatoModel
    {
        public int ID { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 番茄数，每完成4次工作并休息获得一个
        /// </summary>
        public int TomatoCount { get; set; }
        /// <summary>
        /// 重启次数
        /// </summary>
        public int RestartCount { get; set; }
    }
}
