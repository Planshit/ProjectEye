using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Options
{
    /// <summary>
    /// 行为模型
    /// </summary>
    [XmlRootAttribute("Behavior")]
    public class BehaviorModel
    {
        /// <summary>
        /// 全屏时跳过休息
        /// </summary>
        public bool IsFullScreenBreak { get; set; } = false;
        /// <summary>
        /// 是否启用进程跳过功能
        /// </summary>
        public bool IsBreakProgressList { get; set; } = false;
        /// <summary>
        /// 跳过进程名单
        /// </summary>
        public ObservableCollection<string> BreakProgressList { get; set; } = new ObservableCollection<string>();
        /// <summary>
        /// 是否禁用跳过休息（为true时将不允许跳过而是直接进入休息
        /// </summary>
        public bool IsDisabledSkip { get; set; } = false;
    }
}
