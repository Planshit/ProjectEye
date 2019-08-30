using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Options
{
    [XmlRootAttribute("Style")]
    public class StyleModel
    {
        /// <summary>
        /// 主题
        /// </summary>
        public ThemeModel Theme { get; set; }
        /// <summary>
        /// 提醒内容
        /// </summary>
        public string TipContent { get; set; }
    }
}
