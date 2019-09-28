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

        /// <summary>
        /// 是否启用预提醒
        /// </summary>
        public bool IsPreAlert { get; set; } = false;
        /// <summary>
        /// 预提醒时间（秒）
        /// </summary>
        public int PreAlertTime { get; set; } = 20;
        /// <summary>
        /// 预提醒不操作时执行的动作
        /// </summary>
        public ComboxModel PreAlertAction { get; set; } = new ComboxModel() { DisplayName = "进入本次休息", Value = "1" };
        /// <summary>
        /// 是否启用预提醒自动操作
        /// </summary>
        public bool IsPreAlertAutoAction { get; set; } = true;
        /// <summary>
        /// 预提醒标题
        /// </summary>
        public string PreAlertTitle { get; set; } = "Project Eye";
        /// <summary>
        /// 预提醒副标题
        /// </summary>
        public string PreAlertSubtitle { get; set; } = "即将休息，还有{t}秒！";
        /// <summary>
        /// 预提醒内容
        /// </summary>
        public string PreAlertMessage { get; set; } = "请做好准备~";
        /// <summary>
        /// 预提醒图标
        /// </summary>
        public string PreAlertIcon { get; set; } = "";
        /// <summary>
        /// 预提醒提示音
        /// </summary>
        public bool IsPreAlertSound { get; set; } = true;
    }
}
