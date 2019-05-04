using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Options
{
    /// <summary>
    /// 通用设置模型
    /// </summary>
    [XmlRootAttribute("general")]
    public class GeneralModel
    {
        /// <summary>
        /// 开机启动
        /// </summary>
        public bool startup { get; set; } = false;
        /// <summary>
        /// 不要提醒
        /// </summary>
        public bool noreset { get; set; } = false;
        /// <summary>
        /// 统计数据
        /// </summary>
        public bool data { get; set; } = false;
        /// <summary>
        /// 休息结束提示音
        /// </summary>
        public bool sound { get; set; } = true;
        /// <summary>
        /// 离开监听
        /// </summary>
        public bool leavelistener { get; set; } = false;
        /// <summary>
        /// 提醒间隔时间（单位：分钟）
        /// </summary>
        public int warntime { get; set; } = 20;
    }
}
