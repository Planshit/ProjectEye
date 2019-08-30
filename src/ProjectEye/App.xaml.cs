using ProjectEye.Core;
using ProjectEye.Core.Models.Options;
using ProjectEye.Core.Service;
using ProjectEye.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
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
        public delegate void AppEventHandler();
        /// <summary>
        /// 服务初始化完成时发生
        /// </summary>
        public event AppEventHandler OnServiceInitialized;
        public App()
        {
            serviceCollection = new ServiceCollection();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //全局异常捕获
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            //重复运行确认
            ReRunCheck();

            //必须按优先级依次添加
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
            //初始化所有服务
            serviceCollection.Initialize();
            OnServiceInitialized?.Invoke();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMsg;
            try
            {
                string error_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Log",
                    $"error_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.log");
                string dir = Path.GetDirectoryName(error_path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(error_path, $"{e.Exception.ToString()}");
                errorMsg = "程序发生了不可预料的错误，已经将错误报告保存在程序运行目录Log文件夹下，请将错误内容提供给我们。";
            }
            catch
            {
                errorMsg = "程序发生了不可预料的错误，但是无法记录，请将以下错误截图提供给我们：\r\n" + e.Exception.ToString();
            }
            MessageBox.Show(errorMsg, "错误提示，程序即将退出", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            this.Shutdown();
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

    }
}
