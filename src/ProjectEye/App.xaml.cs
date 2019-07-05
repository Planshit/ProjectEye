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
            //数据统计
            serviceCollection.Add<StatisticService>();
            //系统资源
            serviceCollection.Add<SystemResourcesService>();
            //内存缓存
            serviceCollection.Add<CacheService>();
            //配置文件
            serviceCollection.Add<ConfigService>();
            //主题
            serviceCollection.Add<ThemeService>();
            //扩展显示器
            serviceCollection.Add<ScreenService>();
            //主要
            serviceCollection.Add<MainService>();
            //托盘
            serviceCollection.Add<TrayService>();
            //休息
            serviceCollection.Add<ResetService>();
            //声音
            serviceCollection.Add<SoundService>();
            serviceCollection.Add<EyesTestService>();



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
        }
        #endregion
    }
}
