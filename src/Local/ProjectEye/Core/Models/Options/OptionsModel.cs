using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Options
{
    [XmlRootAttribute("Options")]
    public class OptionsModel 
    {
        /// <summary>
        /// 通用设置
        /// </summary>
        public GeneralModel General { get; set; }
        /// <summary>
        /// 外观
        /// </summary>
        public StyleModel Style { get; set; }
        /// <summary>
        /// 快捷键
        /// </summary>
        public KeyboardShortcutModel KeyboardShortcuts { get; set; }
        /// <summary>
        /// 行为
        /// </summary>
        public BehaviorModel Behavior { get; set; }
    }
}
