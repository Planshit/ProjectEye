using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.UI.Controls.ChartControl.Models
{
    /// <summary>
    /// 图表数据模型
    /// </summary>
    public class ChartDataModel
    {
        /// <summary>
        /// 值
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 悬浮文本
        /// </summary>
        public string PopupText { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
