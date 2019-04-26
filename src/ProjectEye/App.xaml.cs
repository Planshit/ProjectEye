using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;

namespace ProjectEye
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceCollection serviceCollection;
        private readonly System.Threading.Mutex mutex;
        public App()
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "projecteye", out ret);

            if (!ret)
            {
                App.Current.Shutdown();
            }
            serviceCollection = new ServiceCollection();
            serviceCollection.AddInstance(this);
            //serviceCollection.Add<ConfigService>();
            serviceCollection.Add<ScreenService>();
            serviceCollection.Add<MainService>();
            serviceCollection.Add<TrayService>();
            serviceCollection.Add<ResetService>();
            serviceCollection.Add<SoundService>();
            

            WindowManager.serviceCollection = serviceCollection;
            serviceCollection.Initialize();


        }

    }
}
