using Project1.UI.Controls.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Project1.UI.Controls.Models
{
    /// <summary>
    /// 元素模型
    /// </summary>
    public class ElementModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        public DesignItemType Type { get; set; }
        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 文字颜色
        /// </summary>
        public Brush TextColor { get; set; }
        /// <summary>
        /// 是否是粗体文本
        /// </summary>
        public bool IsTextBold { get; set; }
        /// <summary>
        /// 不透明度
        /// </summary>
        public double Opacity { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 元素样式名
        /// </summary>
        public string Style { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public int TextAlignment { get; set; }

    }
}
