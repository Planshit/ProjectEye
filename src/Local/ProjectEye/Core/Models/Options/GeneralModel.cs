using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Options
{
    /// <summary>
    /// 通用设置模型
    /// </summary>
    [XmlRootAttribute("General")]
    public class GeneralModel
    {
        /// <summary>
        /// 开机启动
        /// </summary>
        public bool Startup { get; set; } = false;
        /// <summary>
        /// 不要提醒
        /// </summary>
        public bool Noreset { get; set; } = false;
        /// <summary>
        /// 统计数据
        /// </summary>
        public bool Data { get; set; } = false;
        /// <summary>
        /// 休息结束提示音
        /// </summary>
        public bool Sound { get; set; } = true;
        /// <summary>
        /// 离开监听
        /// </summary>
        public bool LeaveListener { get; set; } = false;
        /// <summary>
        /// 提醒间隔时间（单位：分钟）
        /// </summary>
        public int WarnTime { get; set; } = 20;
        /// <summary>
        /// 休息结束提示音效文件路径，为空时使用默认
        /// </summary>
        public string SoundPath { get; set; } = "";
        /// <summary>
        /// 休息时间（单位：秒）
        /// </summary>
        public int RestTime { get; set; } = 20;
        /// <summary>
        /// 是否启用一周数据分析
        /// </summary>
        public bool IsWeekDataAnalysis { get; set; } = false;
        /// <summary>
        /// 是否是番茄时钟模式
        /// </summary>
        public bool IsTomatoMode { get; set; } = false;
    }
}
