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
        ///// <summary>
        ///// 是否启用预提醒自动操作
        ///// </summary>
        //public bool IsPreAlertAutoAction { get; set; } = true;
        /// <summary>
        /// 预提醒标题
        /// </summary>
        public string PreAlertTitle { get; set; } = "Project Eye";
        /// <summary>
        /// 预提醒副标题
        /// </summary>
        public string PreAlertSubtitle { get; set; } = "提醒剩余 {t} 秒";
        /// <summary>
        /// 预提醒内容
        /// </summary>
        public string PreAlertMessage { get; set; } = "您已持续用眼20分钟，休息一会吧！请将注意力集中在至少6米远的地方20秒！";
        /// <summary>
        /// 预提醒图标
        /// </summary>
        public string PreAlertIcon { get; set; } = "";
        /// <summary>
        /// 预提醒提示音
        /// </summary>
        public bool IsPreAlertSound { get; set; } = true;

        /// <summary>
        /// 是否启用自动切换深色主题
        /// </summary>
        public bool IsAutoDarkMode { get; set; } = false;
        /// <summary>
        /// 自动切换深色主题开始时
        /// </summary>
        public int AutoDarkStartH { get; set; } = 18;
        /// <summary>
        /// 自动切换深色主题开始分
        /// </summary>
        public int AutoDarkStartM { get; set; } = 0;
        /// <summary>
        /// 自动切换深色主题结束时
        /// </summary>
        public int AutoDarkEndH { get; set; } = 6;
        /// <summary>
        /// 自动切换深色主题结束分
        /// </summary>
        public int AutoDarkEndM { get; set; } = 0;


        /// <summary>
        /// 动画效果
        /// </summary>
        public bool IsAnimation { get; set; } = false;
        /// <summary>
        /// 提示窗口动画类型
        /// </summary>
        public AnimationModel TipWindowAnimation { get; set; }
        /// <summary>
        /// 休息提示询问，如果开启可以使用预提醒功能以及在全屏提示窗口点击，关闭后则每到休息时间直接开始计时不询问，且支持鼠标穿透
        /// </summary>
        public bool IsTipAsk { get; set; } = true;
        /// <summary>
        /// 全屏提示窗口鼠标穿透
        /// </summary>
        public bool IsThruTipWindow { get; set; } = false;

        /// <summary>
        /// 语言
        /// </summary>
        public ComboxModel Language { get; set; } = new ComboxModel() { DisplayName = "中文", Value = "zh" };
        /// <summary>
        /// 数据统计窗口工作时间占位图路径
        /// </summary>
        public string DataWindowWorkTimeImagePath { get; set; }
        /// <summary>
        /// 数据统计窗口休息时间占位图路径
        /// </summary>
        public string DataWindowRestTimeImagePath { get; set; }
        /// <summary>
        /// 数据统计窗口跳过次数占位图路径
        /// </summary>
        public string DataWindowSkipImagePath { get; set; }
    }
}
