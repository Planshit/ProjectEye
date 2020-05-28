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

    }
}
