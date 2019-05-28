using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEye.Core.Service
{
    public class ThemeService : IService
    {
        private readonly ConfigService config;
        private readonly Theme theme;
        public ThemeService(ConfigService config)
        {
            this.config = config;
            theme = new Theme();
        }
        public void Init()
        {
            Project1.UI.Cores.UIDefaultSetting.DefaultThemeName = config.options.Style.Theme.ThemeName;

            Project1.UI.Cores.UIDefaultSetting.DefaultThemePath = "/ProjectEye;component/Resources/Themes/";
        }
        /// <summary>
        /// 设置主题
        /// </summary>
        /// <param name="themeName"></param>
        public void SetTheme(string themeName)
        {
            if (Project1.UI.Cores.UIDefaultSetting.DefaultThemeName != themeName)
            {
                Project1.UI.Cores.UIDefaultSetting.DefaultThemeName = themeName;

                Project1.UI.Cores.UIDefaultSetting.DefaultThemePath = "/ProjectEye;component/Resources/Themes/";

                theme.ApplyTheme();
            }
        }
    }
}
