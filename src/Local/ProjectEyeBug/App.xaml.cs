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
        private System.Threading.Mutex mutex;
        public App()
        {
            Theme theme = new Theme();
            UIDefaultSetting.DefaultThemeName = "blue";
            UIDefaultSetting.DefaultThemePath = "/ProjectEye;component/Resources/Themes/";
            theme.ApplyTheme();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
#if !DEBUG
                if (
                e.Args.Length == 0 ||
                IsRuned()
                )
            {
                Shutdown();
            }

#endif

        }
        #region 获取当前程序是否已运行
        /// <summary>
        /// 获取当前程序是否已运行
        /// </summary>
        private bool IsRuned()
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "ProjectEyeBug", out ret);
            if (!ret)
            {
#if !DEBUG
                return true;

#endif
            }
            return false;
        }
        #endregion


    }
}
