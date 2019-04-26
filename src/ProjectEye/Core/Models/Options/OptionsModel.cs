using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Options
{
    [XmlRootAttribute("options")]
    public class OptionsModel
    {
        /// <summary>
        /// 通用设置
        /// </summary>
        public GeneralModel general { get; set; }
    }
}
