using ProjectEye.Core;
using ProjectEye.Core.Models.Options;
using ProjectEye.Core.Service;
using ProjectEye.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;

namespace ProjectEye
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        private readonly ServiceCollection serviceCollection;
        private System.Threading.Mutex mutex;
        public App()
        {
            ReRunCheck();

            //必须按优先级依次添加
            serviceCollection = new ServiceCollection();
            serviceCollection.AddInstance(this);
            serviceCollection.Add<SystemResourcesService>();
            serviceCollection.Add<CacheService>();
            serviceCollection.Add<ConfigService>();
            serviceCollection.Add<ThemeService>();
            serviceCollection.Add<ScreenService>();
            serviceCollection.Add<MainService>();
            serviceCollection.Add<TrayService>();
            serviceCollection.Add<ResetService>();
            serviceCollection.Add<SoundService>();


            WindowManager.serviceCollection = serviceCollection;


            Startup += new StartupEventHandler(onStartup);

        }

        #region 重复运行确认
        /// <summary>
        /// 重复运行确认
        /// </summary>
        private void ReRunCheck()
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "projecteye", out ret);

            if (!ret)
            {
#if !DEBUG
                //仅允许运行一次进程
                App.Current.Shutdown();
#endif
            }
        }
        #endregion

        #region onStartup
        /// <summary>
        /// onStartup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onStartup(object sender, StartupEventArgs e)
        {
            //初始化所有服务
            serviceCollection.Initialize();

            WindowManager.GetCreateWindow("StatisticWindow",false)[0].Show();

        }
        #endregion
    }
}
