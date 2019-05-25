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
        public ThemeService(ConfigService config)
        {
            this.config = config;
        }
        public void Init()
        {
            Project1.UI.Cores.UIDefaultSetting.DefaultThemeName = config.options.Style.Theme.ThemeName;

            Project1.UI.Cores.UIDefaultSetting.DefaultThemePath = "/ProjectEye;component/Resources/Themes/";
        }
    }
}
