using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEyeBug
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Theme theme = new Theme();
            UIDefaultSetting.DefaultThemeName = "blue";
            UIDefaultSetting.DefaultThemePath = "/ProjectEye;component/Resources/Themes/";
            theme.ApplyTheme();
        }
    }
}
