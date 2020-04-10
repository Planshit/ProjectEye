using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEye.Core.Models.EyesTest
{
    [XmlRootAttribute("EyesTest")]
    public class EyesTestModel
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 分数
        /// </summary>
        public double Score { get; set; } = 0;
    }
}
