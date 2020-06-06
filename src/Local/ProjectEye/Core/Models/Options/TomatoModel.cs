using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Options
{
    [XmlRootAttribute("Tomato")]
    public class TomatoModel
    {
        /// <summary>
        /// 工作时间
        /// </summary>
        public int WorkMinutes { get; set; } = 25;
        /// <summary>
        /// 短休息
        /// </summary>
        public int ShortRestMinutes { get; set; } = 5;
        /// <summary>
        /// 长休息
        /// </summary>
        public int LongRestMinutes { get; set; } = 30;
        /// <summary>
        /// 是否开启工作开始提示音
        /// </summary>
        public bool IsWorkStartSound { get; set; } = true;
        /// <summary>
        /// 是否开启工作结束提示音
        /// </summary>
        public bool IsWorkEndSound { get; set; } = true;
        /// <summary>
        /// 工作开始提示音路径
        /// </summary>
        public string WorkStartSoundPath { get; set; } = "";
        /// <summary>
        /// 工作结束提示音路径
        /// </summary>
        public string WorkEndSoundPath { get; set; } = "";

    }
}
