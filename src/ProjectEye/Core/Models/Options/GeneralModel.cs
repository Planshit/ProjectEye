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
        public bool startup { get; set; }
        /// <summary>
        /// 不要提醒
        /// </summary>
        public bool noreset { get; set; }
        /// <summary>
        /// 统计数据
        /// </summary>
        public bool data { get; set; }
        /// <summary>
        /// 休息结束提示音
        /// </summary>
        public bool sound { get; set; }

    }
}
