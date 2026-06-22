using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.UI.Cores
{
    /// <summary>
    /// UI默认设置
    /// </summary>
    public class UIDefaultSetting
    {
        /// <summary>
        /// 获取或设置主题名
        /// </summary>
        public static string DefaultThemeName { get; set; } = "Default";
        /// <summary>
        /// 获取或设置主题路径
        /// </summary>
        public static string DefaultThemePath { get; set; } = null;
        /// <summary>
        /// 获取或设置主题颜色
        /// </summary>
        public static string DefaultThemeColor { get; set; } = "#ffc83d";
        /// <summary>
        /// 获取主题全路径
        /// </summary>
        public static string DefaultThemeFullPath
        {
            get
            {
                return DefaultThemePath != null ? DefaultThemePath + DefaultThemeName : "/Project1.UI;component/Assets/Themes/" + DefaultThemeName;
            }
        }
    }
}
